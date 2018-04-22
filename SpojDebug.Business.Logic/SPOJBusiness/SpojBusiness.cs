using Newtonsoft.Json;
using SpojDebug.Business.SPOJBusiness;
using SpojDebug.Core.Constant;
using SpojDebug.Core.Models.SpojModels;
using SpojDebug.Ultil.Spoj;
using SpojDebug.Ultil.SpojDebugException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace SpojDebug.Business.Logic.SPOJBusiness
{
    public class SpojBusiness : ISpojBusiness
    {
        private readonly static string _spojLoginUri = @"http://www.spoj.com/login/";

        private readonly static string s_downloadUrl = string.Format("http://www.spoj.com/{0}/problems/{0}/0.in", "EIUDISC2");

        private readonly static string s_rankUrl = string.Format("http://www.spoj.com/{0}/ranks/", "EIUDISC2");

        private HttpClient _client = null;

        private HttpClientHandler _handler = null;

        private CookieContainer _cookieContainer = null;

        public SpojBusiness()
        {
            _cookieContainer = new CookieContainer();

            _handler = new HttpClientHandler
            {
                UseCookies = true,
                UseDefaultCredentials = true,
                CookieContainer = _cookieContainer
            };

            _client = new HttpClient();
        }

        public void GetSpojInfo()
        {
            var text = "";

            // Todo: customize to get encode then decode in database
            // Login()
            Login();

            text = GetText(s_rankUrl);
            Thread.Sleep(1000);
            text = GetText(s_downloadUrl);

            var tokenizer = new SpojDataTokenizer(text);

            var contest = ParseContest(tokenizer);
            contest.ProblemsInfo = ParseProblems(tokenizer);
            contest.Users = ParseUsers(tokenizer);

            ParseUserSubmissions(tokenizer, contest.Users, contest.ProblemsInfo);

            File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                    "Data.json",
                    JsonConvert.SerializeObject(contest, Formatting.Indented)
                );
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

        #region Private

        private (string, string) GetAdminUsernameAndPassword()
        {
            // Todo: Get from data server and decode
            var username = "user";
            var password = "password";

            return (username, password);
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
