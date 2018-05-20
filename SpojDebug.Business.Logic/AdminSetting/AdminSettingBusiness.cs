using System;
using AutoMapper;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Business.AdminSetting;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.SpojModels;
using SpojDebug.Data.Repositories.AdminSetting;
using SpojDebug.Ultil.Spoj;
using SpojDebug.Ultil.Exception;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Ultil.DataSecurity;
using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Data.Repositories.Problem;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Data.Repositories.Submission;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Data.Repositories.TestCase;
using SpojDebug.Ultil.Logger;

namespace SpojDebug.Business.Logic.AdminSetting
{
    public class AdminSettingBusiness : Business<IAdminSettingRepository, AdminSettingEntity>, IAdminSettingBusiness
    {
        private readonly string _downloadUrl;

        private readonly string _rankUrl;

        private readonly SpojKey _spojKey;

        private readonly SpojInfo _spojInfo;

        private readonly string _inputTestCaseUrl = "http://www.spoj.com/problems/{0}/{1}.in";

        private readonly string _outputTestCaseUrl = "http://www.spoj.com/problems/{0}/{1}.out";

        private readonly string _spojProblemInfoUrl = "http://www.spoj.com/problems/{0}/edit/";

        private readonly string _submissionInfoUrl = "http://www.spoj.com/{0}/files/psinfo/{1}/";

        private readonly IProblemRepository _problemRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly ISubmissionRepository _submissionRepository;

        private readonly IResultRepository _resultRepository;

        private readonly ITestCaseRepository _testCaseRepository;

        private readonly SystemInfo _systemInfo;

        public AdminSettingBusiness(
            IAdminSettingRepository repository,
            IMapper mapper,
            SpojKey spojKey,
            SpojInfo spojInfo,
            IProblemRepository problemRepository,
            IAccountRepository accountRepository,
            ISubmissionRepository submissionRepository,
            IResultRepository resultRepository,
            ITestCaseRepository testCaseRepository, SystemInfo systemInfo) : base(repository, mapper)
        {
            _spojKey = spojKey;
            _spojInfo = spojInfo;
            _downloadUrl = string.Format("http://www.spoj.com/{0}/problems/{0}/0.in", _spojInfo.ContestName);
            _rankUrl = $"http://www.spoj.com/{_spojInfo.ContestName}/ranks/";

            _problemRepository = problemRepository;
            _accountRepository = accountRepository;
            _submissionRepository = submissionRepository;
            _resultRepository = resultRepository;
            _testCaseRepository = testCaseRepository;
            _systemInfo = systemInfo;
        }

        public void GetSpojInfo()
        {
            while (true)
            {
                try
                {
                    var text = "";

                    using (var client = new SpojClient())
                    {

                        var (username, password) = GetAdminUsernameAndPassword();
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                            continue;
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

                    Thread.Sleep(300000);
                }
                catch (Exception e)
                {
                    LogHepler.WriteSystemErrorLog(e, _systemInfo.ErrorLogFolderPath);
                }
            }
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
                try
                {
                    Repository.SaveChanges();
                }
                catch (Exception e)
                {
                    LogHepler.WriteDataErrorLog(e, setting, _systemInfo.DataError + "/SaveChanges");
                }
                return;
            }


            setting.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, _spojKey.ForUserName);
            setting.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, _spojKey.ForPassword);
            Repository.Update(setting, x => x.SpojPasswordEncode, x => x.SpojUserNameEncode);
            try
            {
                Repository.SaveChanges();
            }
            catch (Exception e)
            {
                LogHepler.WriteDataErrorLog(e, setting, _systemInfo.DataError + "/SaveChanges");
            }
        }

        public void DownloadSpojTestCases()
        {
            while (true)
            {
                try
                {
                    var watch = Stopwatch.StartNew();
                    using (var client = new SpojClient())
                    {
                        var (username, password) = GetAdminUsernameAndPassword();
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                            continue;
                        var result = client.LoginAsync(username, password);
                        result.Wait();
                        while (true)
                        {
                            Thread.Sleep(1000);
                            var problem = _problemRepository.Get().FirstOrDefault(x => x.IsDownloadedTestCase != true && x.IsSkip != true);
                            if (problem == null)
                                continue;
                            if (!Regex.IsMatch(problem.Code, "^EI\\w+"))
                            {
                                problem.IsSkip = true;
                                _problemRepository.Update(problem, x => x.IsSkip);

                                try
                                {
                                    _problemRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, problem, _systemInfo.DataError + "/SaveChanges");
                                }
                                continue;
                            }

                            var maxTestCase = 0;
                            try
                            {
                                maxTestCase = client.GetTotalTestCase(problem.Code);
                            }
                            catch (Exception e)
                            {
                                LogHepler.WriteSystemErrorLog(e, _systemInfo.ErrorLogFolderPath);
                            }

                            var path = Path.Combine(Directory.GetCurrentDirectory(), $"TestCases/{problem.Code}");
                            Directory.CreateDirectory(path);

                            _testCaseRepository.Insert(new TestCaseInfoEntity
                            {
                                ProblemId = problem.Id,
                                TotalTestCase = maxTestCase,
                                Path = path
                            });

                            for (var i = 0; i <= maxTestCase; i++)
                            {
                                var input = "";
                                var output = "";
                                try
                                {
                                    input = client.GetText(string.Format(_inputTestCaseUrl, problem.Code, i));
                                    output = client.GetText(string.Format(_outputTestCaseUrl, problem.Code, i));
                                    File.WriteAllText(Path.Combine(path, $"{i}.in"), input);
                                    File.WriteAllText(Path.Combine(path, $"{i}.out"), output);
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteSystemErrorLog(e, _systemInfo.ErrorLogFolderPath);
                                }
                                Thread.Sleep(200);
                            }

                            problem.IsDownloadedTestCase = true;
                            problem.DownloadTestCaseTime = DateTime.Now;
                            _problemRepository.Update(problem, x => x.IsDownloadedTestCase, x => x.DownloadTestCaseTime);
                            try
                            {
                                _problemRepository.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                LogHepler.WriteDataErrorLog(e, problem, _systemInfo.DataError + "/SaveChanges");
                            }
                            // after 10 minutes, we login again
                            var a = watch.ElapsedMilliseconds;
                            if (a > 600000) break;
                        }
                    }
                    watch.Stop();
                }
                catch (Exception e)
                {
                    LogHepler.WriteSystemErrorLog(e, _systemInfo.DataError);
                    Thread.Sleep(30000);
                }
            }
        }

        public void GetSubmissionInfo()
        {
            while (true)
            {
                try
                {
                    var watch = Stopwatch.StartNew();
                    using (var client = new SpojClient())
                    {
                        var (username, password) = GetAdminUsernameAndPassword();
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                            continue;
                        var result = client.LoginAsync(username, password);
                        result.Wait();

                        while (true)
                        {
                            Thread.Sleep(1000);
                            var submission = _submissionRepository.Get().Where(x => x.IsDownloadedInfo != true && x.IsNotHaveEnoughInfo != true && x.Problem.IsSkip != true).Include(x => x.Problem).FirstOrDefault();
                            if (submission == null) continue;
                            if (submission.Problem == null)
                            {
                                submission.IsNotHaveEnoughInfo = true;
                                _submissionRepository.Update(submission, x => x.IsNotHaveEnoughInfo);

                                try
                                {
                                    _submissionRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, submission, _systemInfo.DataError + "/SaveChanges");
                                }
                                continue;
                            }
                            if (!Regex.IsMatch(submission.Problem.Code, "^EI\\w+"))
                            {
                                submission.Problem.IsSkip = true;
                                _problemRepository.Update(submission.Problem, x => x.IsSkip);
                                try
                                {
                                    _problemRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, submission.Problem, _systemInfo.DataError + "/SaveChanges");
                                }
                                continue;
                            }

                            var plaintext = client.GetText(string.Format(_submissionInfoUrl, _spojInfo.ContestName, submission.SpojId));
                            var matches = Regex.Matches(plaintext, "test (\\d+) - (\\w+)");

                            foreach (Match match in matches)
                            {
                                var resultType = GetResultType(match.Groups[2].Value);
                                var dta = new ResultEntity
                                {
                                    SubmmissionId = submission.Id,
                                    TestCaseSeq = int.Parse(match.Groups[1].Value),
                                    Result = resultType
                                };
                                _resultRepository.Insert(dta);

                                try
                                {
                                    _resultRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, dta, _systemInfo.DataError + "/SaveChanges");
                                }
                                Thread.Sleep(200);
                            }

                            submission.IsDownloadedInfo = true;
                            submission.DownloadedTime = DateTime.Now;
                            _submissionRepository.Update(submission, x => x.IsDownloadedInfo, x => x.DownloadedTime);
                            try
                            {
                                _submissionRepository.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                LogHepler.WriteDataErrorLog(e, submission, _systemInfo.DataError + "/SaveChanges");
                            }
                            // after 10 minutes, we login again
                            var a = watch.ElapsedMilliseconds;
                            if (a > 600000) break;
                        }

                    }
                    watch.Stop();
                }
                catch (Exception e)
                {
                    LogHepler.WriteSystemErrorLog(e, _systemInfo.ErrorLogFolderPath);
                    Thread.Sleep(30000);
                }
            }
        }

        private Enums.ResultType GetResultType(string resultString)
        {
            switch (resultString)
            {
                case "AC":
                    return Enums.ResultType.Accepted;

                case "WA":
                    return Enums.ResultType.WrongAnswer;

                case "TLE":
                    return Enums.ResultType.TimeLimited;

                case "RE":
                    return Enums.ResultType.RuntimeError;
            }

            return Enums.ResultType.Unknown;
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
            return setting == null ? (null, null) : (DataSecurityUltils.Decrypt(setting.SpojUserNameEncode, _spojKey.ForUserName), DataSecurityUltils.Decrypt(setting.SpojPasswordEncode, _spojKey.ForPassword));
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
            var listCheckingIds = new List<int>();

            var listChunk = new List<SpojProblemInfoModel>();
            for (var i = 0; i < nProblems; i++)
            {
                var problemModel = new SpojProblemInfoModel()
                {
                    Id = tokenizer.GetInt(),
                    TimeLimit = tokenizer.GetFloat(),
                    Code = tokenizer.GetNext(),
                    Name = tokenizer.GetNext(),
                    Type = tokenizer.GetInt(),
                    ProblemSet = tokenizer.GetNext()
                };

                listChunk.Add(problemModel);
                listCheckingIds.Add(problemModel.Id);


                if (listCheckingIds.Count >= 1000)
                {
                    var ids = listCheckingIds;
                    var listExisting = _problemRepository.Get(x => ids.Contains(x.SpojId.Value)).Select(x => x.Id).ToList();
                    var listEntitties = listChunk.Where(x => !listExisting.Contains(x.Id)).Select(model => new ProblemEntity
                    {
                        SpojId = model.Id,
                        TimeLimit = model.TimeLimit,
                        Code = model.Code,
                        Name = model.Name,
                        Type = model.Type,
                        SpojProblemSet = model.ProblemSet
                    })
                    .ToList();

                    _problemRepository.InsertRange(listEntitties);
                    _problemRepository.SaveChanges();
                    listChunk = new List<SpojProblemInfoModel>();
                    listCheckingIds = new List<int>();
                }

                Thread.Sleep(200);
                tokenizer.Skip(nLines - 6);
                prolems[problemModel.Id] = problemModel;
            }

            _problemRepository.SaveChanges();
            return prolems;
        }

        private Dictionary<int, SpojUserModel> ParseUsers(SpojDataTokenizer tokenizer)
        {
            var nUsers = tokenizer.GetInt();
            var nLines = tokenizer.GetInt();
            var users = new Dictionary<int, SpojUserModel>();

            var listChunk = new List<SpojUserModel>();
            var listChunkIds = new List<int>();

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

                listChunk.Add(user);
                listChunkIds.Add(user.UserId);
                if (listChunk.Count >= 1000)
                {
                    var ids = listChunkIds;
                    var listExist = _accountRepository.Get(x => ids.Contains(x.SpojUserId)).Select(x => x.SpojUserId);
                    var listEntites = listChunk.Where(x => !listExist.Contains(x.UserId)).Select(x => new AccountEntity
                    {
                        SpojUserId = user.UserId,
                        UserName = user.Username,
                        DisplayName = user.DisplayName,
                        Email = user.Email
                    });

                    _accountRepository.InsertRange(listEntites);
                    _accountRepository.SaveChanges();

                    listChunk = new List<SpojUserModel>();
                    listChunkIds = new List<int>();
                }

                Thread.Sleep(200);
            }
            return users;
        }

        private void ParseUserSubmissions(SpojDataTokenizer tokenizer, Dictionary<int, SpojUserModel> users, Dictionary<int, SpojProblemInfoModel> problemsInfo)
        {
            var nSeries = tokenizer.GetInt();
            var nLine = tokenizer.GetInt();
            tokenizer.Skip(1);
            var nSubmissions = tokenizer.GetInt();
            
            var listChunk = new List<SpojSubmissionModel>();
            var listChunkIds = new List<int>();

            for (var i = 0; i < nSubmissions; i++)
            {
                var userId = tokenizer.GetInt();
                var spojProblemId = tokenizer.GetInt();
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

                if (!problemsInfo.ContainsKey(spojProblemId)) continue;
                var problemInfo = problemsInfo[spojProblemId];

                var submission = new SpojSubmissionModel
                {
                    Id = id,
                    Time = time,
                    Score = status == 15 && problemInfo.Type == 2 ? score : (status == 15 && problemInfo.Type == 0 ? 100 : 0),
                    RunTime = runTime,
                    Language = languageText,
                    UserId = userId,
                    ProblemId = spojProblemId
                };

                listChunk.Add(submission);
                listChunkIds.Add(submission.Id);

                if (listChunk.Count >= 1000)
                {
                    var ids = listChunkIds;
                    var listExist = _submissionRepository.Get(x => ids.Contains(x.SpojId))
                        .Select(x => x.SpojId);

                    var listNotExist = listChunk.Where(x => !listExist.Contains(x.Id));

                    var listEntities = (from item in listNotExist
                        let internalProblemId = _problemRepository.Get(x => x.SpojId == item.ProblemId).Select(x => x.Id).FirstOrDefault()
                        let internalAccountId = _accountRepository.Get(x => x.SpojUserId == item.UserId).Select(x => x.Id).FirstOrDefault()
                        select new SubmissionEntity
                        {
                            SpojId = item.Id,
                            SubmitTime = item.Time,
                            Score = item.Score,
                            RunTime = item.RunTime,
                            Language = item.Language,
                            ProblemId = internalProblemId == 0 ? (int?) null : internalProblemId,
                            AccountId = internalAccountId == 0 ? (int?) null : internalAccountId
                        }).ToList();
                    _submissionRepository.InsertRange(listEntities);
                    _submissionRepository.SaveChanges();

                    listChunk = new List<SpojSubmissionModel>();
                    listChunkIds = new List<int>();
                }

                Thread.Sleep(200);
                SpojUserModel user = null;
                if (users.TryGetValue(userId, out user))
                {
                    SpojProblemModel problem = null;
                    if (!user.Problems.TryGetValue(spojProblemId, out problem))
                    {
                        problem = new SpojProblemModel() { Id = spojProblemId, Code = problemInfo.Code };
                        user.Problems[spojProblemId] = problem;
                    }
                    problem.Submissions.Add(submission);
                }
            }

        }

        #endregion
    }
}
