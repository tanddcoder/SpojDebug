using AutoMapper;
using Newtonsoft.Json;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Business.AdminSetting;
using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.SpojModels;
using SpojDebug.Data.Repositories.AdminSetting;
using SpojDebug.Ultil.Spoj;
using SpojDebug.Ultil.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using SpojDebug.Ultil.DataSecurity;
using SpojDebug.Ultil.FileHelper;
using SpojDebug.Core.AppSetting;

namespace SpojDebug.Business.Logic.AdminSetting
{
    public class AdminSettingBusiness : Business<IAdminSettingRepository, AdminSettingEntity>, IAdminSettingBusiness
    {
        private readonly string _spojLoginUri;

        private readonly string _downloadUrl;

        private readonly string _rankUrl;

        private HttpClient _client = null;

        private HttpClientHandler _handler = null;

        private CookieContainer _cookieContainer = null;

        private readonly SpojKey _spojKey;

        private readonly SpojInfo _spojInfo;

        protected AdminSettingBusiness(IAdminSettingRepository repository, IMapper mapper, SpojKey spojKey, SpojInfo spojInfo) : base(repository, mapper)
        {
            _cookieContainer = new CookieContainer();

            _handler = new HttpClientHandler
            {
                UseCookies = true,
                UseDefaultCredentials = true,
                CookieContainer = _cookieContainer
            };

            _client = new HttpClient();
            _spojKey = spojKey;
            _spojInfo = spojInfo;
            _spojLoginUri = @"http://www.spoj.com/login/";
            _downloadUrl = string.Format("http://www.spoj.com/{0}/problems/{0}/0.in", _spojInfo.ContestName);
            _rankUrl = string.Format("http://www.spoj.com/{0}/ranks/", _spojInfo.ContestName);
        }

        public void GetSpojInfo()
        {
            var text = "";
            
            Login();

            text = GetText(_rankUrl);
            Thread.Sleep(1000);
            text = GetText(_downloadUrl);

            var tokenizer = new SpojDataTokenizer(text);

            var contest = ParseContest(tokenizer);
            contest.ProblemsInfo = ParseProblems(tokenizer);
            contest.Users = ParseUsers(tokenizer);

            ParseUserSubmissions(tokenizer, contest.Users, contest.ProblemsInfo);
            
            FileUltils.SaveFile(Directory.GetCurrentDirectory(), "Data.json", JsonConvert.SerializeObject(contest, Formatting.Indented));
        }

        public bool Login()
        {
            var (username, password) = GetAdminUsernameAndPassword();

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                var response = _client.PostAsync(_spojLoginUri, content);
                response.Wait();

                if (response.IsFaulted)
                    throw new SpojDebugException(Error.Fault);

                if (response.IsCanceled)
                    throw new SpojDebugException(Error.Cancelled);

                return true;
            }
            catch (Exception e)
            {

                throw new SpojDebugException(e.Message);
            }
        }

        public void UpdateSetting()
        {
            throw new NotImplementedException();
        }

        #region Private

        private (string, string) GetAdminUsernameAndPassword()
        {
            var setting = Repository.GetSingle();
            return (DataSecurityUltils.Decrypt(setting.SpojUserNameEncode, _spojKey.ForUserName), DataSecurityUltils.Decrypt(setting.SpojPasswordEncode, _spojKey.ForPassword));
        }

        private string GetText(string url)
        {
            var response = _client.GetByteArrayAsync(url);
            response.Wait();
            var bytes = response.Result;
            var textResult = Encoding.UTF8.GetString(bytes);
            return textResult;
        }

        private SpojContestModel ParseContest(SpojDataTokenizer tokenizer)
        {
            var nLines = tokenizer.GetInt();
            var contest = new SpojContestModel()
            {
                Start = tokenizer.GetUnixTime(),
                End = tokenizer.GetUnixTime(),
            };
            tokenizer.Skip(1);
            contest.Name = tokenizer.GetNext();
            tokenizer.Skip(nLines - 4);
            return contest;
        }

        private Dictionary<int, SpojProblemInfoModel> ParseProblems(SpojDataTokenizer tokenizer)
        {
            var nProblems = tokenizer.GetInt();
            var nLines = tokenizer.GetInt();
            var prolems = new Dictionary<int, SpojProblemInfoModel>();
            for (var i = 0; i < nProblems; i++)
            {
                var problem = new SpojProblemInfoModel()
                {
                    Id = tokenizer.GetInt(),
                    TimeLimit = tokenizer.GetFloat(),
                    Code = tokenizer.GetNext(),
                    Name = tokenizer.GetNext(),
                    Type = tokenizer.GetInt(),
                    ProblemSet = tokenizer.GetNext()
                };
                tokenizer.Skip(nLines - 6);
                prolems[problem.Id] = problem;
            }
            return prolems;
        }

        private Dictionary<int, SpojUserModel> ParseUsers(SpojDataTokenizer tokenizer)
        {
            var nUsers = tokenizer.GetInt();
            var nLines = tokenizer.GetInt();
            var users = new Dictionary<int, SpojUserModel>();
            for (var i = 0; i < nUsers; i++)
            {
                var user = new SpojUserModel()
                {
                    UserId = tokenizer.GetInt(),
                    Username = tokenizer.GetNext(),
                    DisplayName = tokenizer.GetNext(),
                    Email = tokenizer.GetNext()
                };
                tokenizer.Skip(nLines - 4);
                users[user.UserId] = user;
            }
            return users;
        }

        private void ParseUserSubmissions(SpojDataTokenizer tokenizer, Dictionary<int, SpojUserModel> users, Dictionary<int, SpojProblemInfoModel> problemsInfo)
        {
            var nSeries = tokenizer.GetInt();
            var nLine = tokenizer.GetInt();
            tokenizer.Skip(1);
            var nSubmissions = tokenizer.GetInt();
            for (var i = 0; i < nSubmissions; i++)
            {
                var userId = tokenizer.GetInt();
                var problemId = tokenizer.GetInt();
                var time = tokenizer.GetUnixTime();
                var status = tokenizer.GetInt();
                var language = tokenizer.GetInt();
                var score = tokenizer.GetFloat();
                var runTime = tokenizer.GetFloat();
                tokenizer.Skip(1);
                var id = tokenizer.GetInt();

                var languageText = "";
                switch (language)
                {
                    case 10: languageText = "Java"; break;
                    case 27: languageText = "C#"; break;
                    case 32: languageText = "JS2"; break;
                }
                tokenizer.Skip(nLine - 9);

                if (!problemsInfo.ContainsKey(problemId)) continue;
                var problemInfo = problemsInfo[problemId];

                var submission = new SpojSubmissionModel()
                {
                    Id = id,
                    Time = time,
                    Score = status == 15 && problemInfo.Type == 2 ? score : (status == 15 && problemInfo.Type == 0 ? 100 : 0),
                    RunTime = runTime,
                    Language = languageText
                };

                SpojUserModel user = null;
                if (users.TryGetValue(userId, out user))
                {
                    SpojProblemModel problem = null;
                    if (!user.Problems.TryGetValue(problemId, out problem))
                    {
                        problem = new SpojProblemModel() { Id = problemId, Code = problemInfo.Code };
                        user.Problems[problemId] = problem;
                    }
                    problem.Submissions.Add(submission);
                }
            }
        }
        #endregion
    }
}
