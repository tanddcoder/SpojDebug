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
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Data.Repositories.Problem;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Data.Repositories.Submission;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Core.Entities.Result;

namespace SpojDebug.Business.Logic.AdminSetting
{
    public class AdminSettingBusiness : Business<IAdminSettingRepository, AdminSettingEntity>, IAdminSettingBusiness
    {
        private readonly string _spojLoginUri;

        private readonly string _downloadUrl;

        private readonly string _rankUrl;

        private readonly SpojKey _spojKey;

        private readonly SpojInfo _spojInfo;

        private readonly IProblemRepository _problemRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly ISubmissionRepository _submissionRepository;

        private readonly IResultRepository _resultRepository;

        private readonly IResultDetailRepository _resultDetailRepository;

        public AdminSettingBusiness(
            IAdminSettingRepository repository, 
            IMapper mapper, 
            SpojKey spojKey, 
            SpojInfo spojInfo,
            IProblemRepository problemRepository,
            IAccountRepository accountRepository,
            ISubmissionRepository submissionRepository,
            IResultRepository resultRepository,
            IResultDetailRepository resultDetailRepository) : base(repository, mapper)
        {
            _spojKey = spojKey;
            _spojInfo = spojInfo;
            _downloadUrl = string.Format("http://www.spoj.com/{0}/problems/{0}/0.in", _spojInfo.ContestName);
            _rankUrl = string.Format("http://www.spoj.com/{0}/ranks/", _spojInfo.ContestName);

            _problemRepository = problemRepository;
            _accountRepository = accountRepository;
            _submissionRepository = submissionRepository;
            _resultRepository = resultRepository;
            _resultDetailRepository = resultDetailRepository;
        }

        public void GetSpojInfo()
        {
            var text = "";

            using (var client = new SpojClient())
            {

                var (username, password) = GetAdminUsernameAndPassword();
                var result = client.LoginAsync(username, password);
                result.Wait();
                text = client.GetText(_rankUrl);
                Thread.Sleep(1000);
                text = client.GetText(_downloadUrl);
            }

            var tokenizer = new SpojDataTokenizer(text);

            var contest = ParseContest(tokenizer);
            contest.ProblemsInfo = ParseProblems(tokenizer);
            contest.Users = ParseUsers(tokenizer);

            ParseUserSubmissions(tokenizer, contest.Users, contest.ProblemsInfo);

            // Todo: Convert data to Entity and save to data base, then remove below line --> FileUltils.SaveFile
            FileUltils.SaveFile(Directory.GetCurrentDirectory(), "Data.json", JsonConvert.SerializeObject(contest, Formatting.Indented));
        }

        public void UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                throw new SpojDebugException("Invalid model");

            if (model.Password != model.ConfirmPassword)
                throw new SpojDebugException("Invalid Password");

            var setting = Repository.GetSingle();

            if (setting == null)
            {
                setting = new AdminSettingEntity
                {
                    SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, _spojKey.ForUserName),
                    SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, _spojKey.ForPassword)
                };

                Repository.Insert(setting);
                Repository.SaveChanges();
                return;
            }


            setting.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, _spojKey.ForUserName);
            setting.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, _spojKey.ForPassword);
            Repository.Update(setting);
            Repository.SaveChanges();
        }

        public AdminSettingSpojAccountResponseModel GetSpojAccount()
        {
            var (username, password) = GetAdminUsernameAndPassword();

            return new AdminSettingSpojAccountResponseModel
            {
                UserName = username
            };
        }

        #region Private
        private (string, string) GetAdminUsernameAndPassword()
        {
            var setting = Repository.GetSingle();
            if (setting == null)
                return (null, null);
            return (DataSecurityUltils.Decrypt(setting.SpojUserNameEncode, _spojKey.ForUserName), DataSecurityUltils.Decrypt(setting.SpojPasswordEncode, _spojKey.ForPassword));
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

                _problemRepository.Insert(new ProblemEntity
                {
                    SpojId = problem.Id,
                    SpojCode = problem.Code,
                    SpojLink = ""
                });

                _problemRepository.SaveChanges();
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
                    default: languageText = "Other Languages"; break;
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


                //Todo: use mapper
                var submissionEntity = _submissionRepository.Insert(new SubmissionEntity
                {
                    SpojId = id,
                    SubmitTime = time,
                    Score = status == 15 && problemInfo.Type == 2 ? score : (status == 15 && problemInfo.Type == 0 ? 100 : 0),
                    RunTime = runTime,
                    Language = languageText
                });

                _resultRepository.Insert(new ResultEntity
                {
                    SubmmissionId = submissionEntity.Id
                });

                var problemEntity = _problemRepository.Get(x => x.SpojId == problemId);

                _submissionRepository.SaveChanges();

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
