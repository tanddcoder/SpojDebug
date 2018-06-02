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
using System.IO;
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
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Ultil.Logger;
using SpojDebug.Core;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.EF.Repositories.Problem;
using SpojDebug.Data.EF.Repositories.Submission;
using SpojDebug.Data.EF.Repositories.Result;
using SpojDebug.Data.EF.Repositories.TestCase;
using SpojDebug.Data.EF.Repositories.Account;

namespace SpojDebug.Business.Logic.AdminSetting
{
    public class AdminSettingBusiness : Business<IAdminSettingRepository, AdminSettingEntity>, IAdminSettingBusiness
    {
        private readonly string _downloadUrl;

        private readonly string _rankUrl;

        private readonly string _inputTestCaseUrl = "http://www.spoj.com/problems/{0}/{1}.in";

        private readonly string _outputTestCaseUrl = "http://www.spoj.com/problems/{0}/{1}.out";

        private readonly string _spojProblemInfoUrl = "http://www.spoj.com/problems/{0}/edit/";

        private readonly string _submissionInfoUrl = "http://www.spoj.com/{0}/files/psinfo/{1}/";

        private readonly string _testCasePostUrl = "http://www.spoj.com/problems/{0}/edit2/";

        private const int _chunkSize = 1000;

        private static readonly object LockSubmission = new object();

        private static readonly object LockSpojInfo = new object();

        private static readonly object LockDowloadTestCase = new object();

        public AdminSettingBusiness(
            IAdminSettingRepository repository,
            IMapper mapper) : base(repository, mapper)
        {
            _downloadUrl = string.Format("http://www.spoj.com/{0}/problems/{0}/0.in", ApplicationConfigs.SpojInfo.ContestName);
            _rankUrl = $"http://www.spoj.com/{ApplicationConfigs.SpojInfo.ContestName}/ranks/";
        }

        public void GetSpojInfo()
        {
            if (JobLocker.IsDownloadSpojInfoInProcess) return;

            JobLocker.IsDownloadSpojInfoInProcess = true;

            try
            {
                var text = "";

                using (var client = new SpojClient())
                {

                    var (username, password) = GetAdminUsernameAndPassword();
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        return;
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
            }
            catch (Exception e)
            {
                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
            }

            JobLocker.IsDownloadSpojInfoInProcess = true;
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
                    SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, ApplicationConfigs.SpojKey.ForUserName),
                    SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, ApplicationConfigs.SpojKey.ForPassword)
                };

                Repository.Insert(setting);
                try
                {
                    Repository.SaveChanges();
                }
                catch (Exception e)
                {
                    LogHepler.WriteDataErrorLog(e, setting, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                }
                return;
            }


            setting.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, ApplicationConfigs.SpojKey.ForUserName);
            setting.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, ApplicationConfigs.SpojKey.ForPassword);
            Repository.Update(setting, x => x.SpojPasswordEncode, x => x.SpojUserNameEncode);
            try
            {
                Repository.SaveChanges();
            }
            catch (Exception e)
            {
                LogHepler.WriteDataErrorLog(e, setting, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
            }
        }

        public void DownloadSpojTestCases()
        {
            if (JobLocker.IsDownloadTestCasesInProcess) return;

            JobLocker.IsDownloadTestCasesInProcess = true;

            try
            {
                using (var context = new SpojDebugDbContext())
                {
                    using (var problemRepository = new ProblemRepository(context))
                    using (var testCaseRepository = new TestCaseRepository(context))
                    {
                        using (var client = new SpojClient())
                        {
                            var (username, password) = GetAdminUsernameAndPassword();
                            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                                return;
                            var result = client.LoginAsync(username, password);
                            result.Wait();
                            Thread.Sleep(1000);
                            var problem = problemRepository.Get().FirstOrDefault(x => x.IsDownloadedTestCase != true && x.IsSkip != true);
                            if (problem == null)
                                return;
                            if (!Regex.IsMatch(problem.Code, "^EI\\w+"))
                            {
                                problem.IsSkip = true;
                                problemRepository.Update(problem, x => x.IsSkip);

                                try
                                {
                                    problemRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, problem, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                                }
                                return;
                            }

                            var maxTestCase = 0;
                            try
                            {
                                maxTestCase = client.GetTotalTestCase(problem.Code);
                            }
                            catch (Exception e)
                            {
                                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
                            }

                            var path = Path.Combine(Directory.GetCurrentDirectory(), $"TestCases/{problem.Code}");
                            Directory.CreateDirectory(path);

                            testCaseRepository.Insert(new TestCaseInfoEntity
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
                                    LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
                                }
                                Thread.Sleep(200);
                            }

                            problem.IsDownloadedTestCase = true;
                            problem.DownloadTestCaseTime = DateTime.Now;
                            problemRepository.Update(problem, x => x.IsDownloadedTestCase, x => x.DownloadTestCaseTime);
                            try
                            {
                                problemRepository.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                LogHepler.WriteDataErrorLog(e, problem, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.DataError);
            }

            JobLocker.IsDownloadTestCasesInProcess = false;
        }

        public void GetSubmissionInfo()
        {
            if (JobLocker.IsDownloadSubmissionInfoInProcess) return;

            JobLocker.IsDownloadSubmissionInfoInProcess = true;

            try
            {
                using (var context = new SpojDebugDbContext())
                {
                    using (var problemRepository = new ProblemRepository(context))
                    using (var submissionRepository = new SubmissionRepository(context))
                    using (var resultRepository = new ResultRepository(context))
                    {
                        using (var client = new SpojClient())
                        {
                            var (username, password) = GetAdminUsernameAndPassword();
                            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                                return;
                            var result = client.LoginAsync(username, password);
                            result.Wait();
                            var submissions = submissionRepository.Get()
                                .Where(x => x.IsDownloadedInfo != true && x.IsNotHaveEnoughInfo != true &&
                                            x.Problem.IsSkip != true).Include(x => x.Problem).Take(1000);

                            foreach (var submission in submissions)
                            {
                                if (submission == null) continue;
                                if (submission.Problem == null)
                                {
                                    submission.IsNotHaveEnoughInfo = true;
                                    submissionRepository.Update(submission, x => x.IsNotHaveEnoughInfo);

                                    try
                                    {
                                        submissionRepository.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        LogHepler.WriteDataErrorLog(e, submission, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                                    }
                                    continue;
                                }
                                if (!Regex.IsMatch(submission.Problem.Code, "^EI\\w+"))
                                {
                                    submission.Problem.IsSkip = true;
                                    problemRepository.Update(submission.Problem, x => x.IsSkip);
                                    try
                                    {
                                        problemRepository.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        LogHepler.WriteDataErrorLog(e, submission.Problem, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                                    }
                                    continue;
                                }

                                var plaintext = client.GetText(string.Format(_submissionInfoUrl, ApplicationConfigs.SpojInfo.ContestName, submission.SpojId));
                                var matches = Regex.Matches(plaintext, "test (\\d+) - (\\w+)");

                                var listResultEnities = new List<ResultEntity>();

                                foreach (Match match in matches)
                                {
                                    var resultType = GetResultType(match.Groups[2].Value);

                                    listResultEnities.Add(new ResultEntity
                                    {
                                        SubmmissionId = submission.Id,
                                        TestCaseSeq = int.Parse(match.Groups[1].Value),
                                        Result = resultType
                                    });
                                }

                                resultRepository.InsertRange(listResultEnities);

                                try
                                {
                                    resultRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteCustomErrorLog(e, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                                }
                                Thread.Sleep(5000);

                                submission.IsDownloadedInfo = true;
                                submission.DownloadedTime = DateTime.Now;
                                submissionRepository.Update(submission, x => x.IsDownloadedInfo, x => x.DownloadedTime);
                                try
                                {
                                    submissionRepository.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    LogHepler.WriteDataErrorLog(e, submission, ApplicationConfigs.SystemInfo.DataError + "/SaveChanges");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
            }

            JobLocker.IsDownloadSubmissionInfoInProcess = false;
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
            return setting == null ? (null, null) : (DataSecurityUltils.Decrypt(setting.SpojUserNameEncode, ApplicationConfigs.SpojKey.ForUserName), DataSecurityUltils.Decrypt(setting.SpojPasswordEncode, ApplicationConfigs.SpojKey.ForPassword));
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
            var prolems = new Dictionary<int, SpojProblemInfoModel>();
            using (var context = new SpojDebugDbContext())
            {
                using (var problemRepository = new ProblemRepository(context))
                {
                    var nProblems = tokenizer.GetInt();
                    var nLines = tokenizer.GetInt();

                    var listCheckingIds = new List<int>();

                    var listChunk = new List<SpojProblemInfoModel>();
                    var chunkSteps = (int)Math.Ceiling((double)nProblems / _chunkSize);
                    for (int i = 0; i < chunkSteps; i++)
                    {
                        for (int j = i * _chunkSize; j < (i + 1) * _chunkSize && j < nProblems; j++)
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

                            tokenizer.Skip(nLines - 6);
                            prolems[problemModel.Id] = problemModel;
                        }

                        var ids = listCheckingIds;
                        var listExisting = problemRepository.Get(x => ids.Contains(x.SpojId.Value)).Select(x => x.Id).ToList();
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

                        problemRepository.InsertRange(listEntitties);
                        problemRepository.SaveChanges();
                        listChunk = new List<SpojProblemInfoModel>();
                        listCheckingIds = new List<int>();
                        Thread.Sleep(5000);
                    }
                    problemRepository.SaveChanges();
                }
            }

            return prolems;
        }

        private Dictionary<int, SpojUserModel> ParseUsers(SpojDataTokenizer tokenizer)
        {
            var users = new Dictionary<int, SpojUserModel>();
            using (var context = new SpojDebugDbContext())
            {
                using (var accountRepository = new AccountRepository(context))
                {
                    var nUsers = tokenizer.GetInt();
                    var nLines = tokenizer.GetInt();

                    var listChunk = new List<SpojUserModel>();
                    var listChunkIds = new List<int>();

                    var chunkSteps = (int)Math.Ceiling((double)nUsers / _chunkSize);

                    for (int i = 0; i < chunkSteps; i++)
                    {
                        for (int j = i * _chunkSize; j < (i + 1) * _chunkSize && j < nUsers; j++)
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
                        }
                        var ids = listChunkIds;
                        var listExist = accountRepository.Get(x => ids.Contains(x.SpojUserId)).Select(x => x.SpojUserId);
                        var listEntites = listChunk.Where(x => !listExist.Contains(x.UserId)).Select(x => new AccountEntity
                        {
                            SpojUserId = x.UserId,
                            UserName = x.Username,
                            DisplayName = x.DisplayName,
                            Email = x.Email
                        });

                        accountRepository.InsertRange(listEntites);
                        accountRepository.SaveChanges();

                        listChunk = new List<SpojUserModel>();
                        listChunkIds = new List<int>();
                        Thread.Sleep(5000);
                    }
                }
            }
            return users;
        }

        private void ParseUserSubmissions(SpojDataTokenizer tokenizer, Dictionary<int, SpojUserModel> users, Dictionary<int, SpojProblemInfoModel> problemsInfo)
        {

            using (var context = new SpojDebugDbContext())
            {
                using (var submissionRepository = new SubmissionRepository(context))
                using (var problemRepository = new ProblemRepository(context))
                using (var accountRepository = new AccountRepository(context))
                {
                    var nSeries = tokenizer.GetInt();
                    var nLine = tokenizer.GetInt();
                    tokenizer.Skip(1);
                    var nSubmissions = tokenizer.GetInt();

                    var listChunk = new List<SpojSubmissionModel>();
                    var listChunkIds = new List<int>();

                    var chunkSteps = (int)Math.Ceiling((double)nSubmissions / _chunkSize);
                    for (int i = 0; i < chunkSteps; i++)
                    {
                        for (int j = i * _chunkSize; j < (i + 1) * _chunkSize && j < nSubmissions; j++)
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

                        var ids = listChunkIds;
                        var listExist = submissionRepository.Get(x => ids.Contains(x.SpojId))
                            .Select(x => x.SpojId);

                        var listNotExist = listChunk.Where(x => !listExist.Contains(x.Id));

                        var listEntities = (from item in listNotExist
                                            let internalProblemId = problemRepository.Get(x => x.SpojId == item.ProblemId).Select(x => x.Id).FirstOrDefault()
                                            let internalAccountId = accountRepository.Get(x => x.SpojUserId == item.UserId).Select(x => x.Id).FirstOrDefault()
                                            select new TestCaseEntity
                                            {
                                                SpojId = item.Id,
                                                SubmitTime = item.Time,
                                                Score = item.Score,
                                                RunTime = item.RunTime,
                                                Language = item.Language,
                                                ProblemId = internalProblemId == 0 ? (int?)null : internalProblemId,
                                                AccountId = internalAccountId == 0 ? (int?)null : internalAccountId
                                            }).ToList();
                        submissionRepository.InsertRange(listEntities);
                        submissionRepository.SaveChanges();

                        listChunk = new List<SpojSubmissionModel>();
                        listChunkIds = new List<int>();

                        Thread.Sleep(5000);
                    }
                }
            }
        }

        #endregion
    }
}
