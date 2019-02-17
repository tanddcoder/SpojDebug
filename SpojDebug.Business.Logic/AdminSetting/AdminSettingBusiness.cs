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
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Data.Repositories.Submission;
using SpojDebug.Data.Repositories.Problem;
using SpojDebug.Data.Repositories.TestCase;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Business.Cache;
using System.Threading.Tasks;

namespace SpojDebug.Business.Logic.AdminSetting
{
    public class AdminSettingBusiness : Business<IAdminSettingRepository, AdminSettingEntity>, IAdminSettingBusiness
    {
        private readonly string _downloadUrl;

        private readonly string _rankUrl;

        private readonly string _inputTestCaseUrl = "/problems/{0}/{1}.in";

        private readonly string _outputTestCaseUrl = "/problems/{0}/{1}.out";

        private readonly string _spojProblemInfoUrl = "/problems/{0}/edit/";

        private readonly string _submissionInfoUrl = "/{0}/files/psinfo/{1}/";

        private readonly string _testCasePostUrl = "/problems/{0}/edit2/";

        private const int _chunkSize = 1000;

        private static readonly object LockSpojInfo = new object();

        private static readonly object LockDowloadTestCase = new object();

        private readonly IResultRepository _resultRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubmissionCacheBusiness _submissionCacheBusiness;
        private readonly IProblemCacheBusiness _problemCacheBusiness;
        private readonly IAdminSettingCacheBusiness _adminSettingCacheBusiness;

        public AdminSettingBusiness(
            IAdminSettingRepository repository,
            IMapper mapper,
            IResultRepository resultRepository,
            ISubmissionRepository submissionRepository,
            IProblemRepository problemRepository,
            ITestCaseRepository testCaseRepository,
            IAccountRepository accountRepository,
            ISubmissionCacheBusiness submissionCacheBusiness,
            IProblemCacheBusiness problemCacheBusiness,
            IAdminSettingCacheBusiness adminSettingCacheBusiness
            ) : base(repository, mapper)
        {
            _downloadUrl = "/{0}/problems/{0}/0.in";
            _rankUrl = "/{0}/ranks/";
            _resultRepository = resultRepository;
            _submissionRepository = submissionRepository;
            _problemRepository = problemRepository;
            _testCaseRepository = testCaseRepository;
            _accountRepository = accountRepository;
            _submissionCacheBusiness = submissionCacheBusiness;
            _problemCacheBusiness = problemCacheBusiness;
            _adminSettingCacheBusiness = adminSettingCacheBusiness;
        }
        /// <summary>
        ///     Get general information
        /// </summary>
        public void GetSpojInfo()
        {
            if (JobLocker.IsDownloadSpojInfoInProcess) return;

            JobLocker.IsDownloadSpojInfoInProcess = true;

            try
            {
                var text = "";

                using (var client = new SpojClient())
                {

                    var adminAccountTask = _adminSettingCacheBusiness.GetFullInfo();
                    adminAccountTask.Wait();

                    var rankUrl = string.Format(_rankUrl, adminAccountTask.Result.ContestName);
                    var downloadUrl = string.Format(_downloadUrl, adminAccountTask.Result.ContestName);

                    var adminAccount = adminAccountTask.Result;
                    if (string.IsNullOrEmpty(adminAccount.Username) || string.IsNullOrEmpty(adminAccount.Password))
                    {
                        JobLocker.IsDownloadSpojInfoInProcess = false;
                        return;
                    }

                    var result = client.LoginAsync(adminAccount.Username, adminAccount.Password);
                    result.Wait();

                    text = client.GetText(rankUrl);
                    Thread.Sleep(1000);
                    text = client.GetText(downloadUrl);
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

            JobLocker.IsDownloadSpojInfoInProcess = false;
        }

        /// <summary>
        ///     Update admin SPOJ Account
        /// </summary>
        /// <param name="model"></param>
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

                Repository.SaveChanges();
                return;
            }


            setting.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, ApplicationConfigs.SpojKey.ForUserName);
            setting.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, ApplicationConfigs.SpojKey.ForPassword);
            Repository.Update(setting, x => x.SpojPasswordEncode, x => x.SpojUserNameEncode);

            Repository.SaveChanges();

        }

        /// <summary>
        ///     Cron job download TestCase
        /// </summary>
        public void DownloadSpojTestCases()
        {
            if (JobLocker.IsDownloadTestCasesInProcess) return;

            JobLocker.IsDownloadTestCasesInProcess = true;

            try
            {
                using (var client = new SpojClient())
                {
                    var adminAccountTask = _adminSettingCacheBusiness.GetAdminAccountAsync();
                    adminAccountTask.Wait();
                    var adminAccount = adminAccountTask.Result;
                    if (string.IsNullOrEmpty(adminAccount.Username) || string.IsNullOrEmpty(adminAccount.Password))
                    {
                        JobLocker.IsDownloadTestCasesInProcess = false;
                        return;
                    }

                    var result = client.LoginAsync(adminAccount.Username, adminAccount.Password);
                    result.Wait();
                    var problems = _problemRepository.Get().Where(x => x.IsDownloadedTestCase != true && x.IsSkip != true).Take(100).ToList();
                    foreach (var problem in problems)
                    {

                        if (!Regex.IsMatch(problem.Code, "^EI\\w+"))
                        {
                            problem.IsSkip = true;
                            _problemRepository.Update(problem, x => x.IsSkip);

                            _problemRepository.SaveChanges();

                            continue;
                        }

                        var maxTestCase = 0;
                        maxTestCase = client.GetTotalTestCase(problem.Code);

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
                                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
                            }
                        }

                        problem.IsDownloadedTestCase = true;
                        problem.DownloadTestCaseTime = DateTime.Now;
                        _problemRepository.Update(problem, x => x.IsDownloadedTestCase, x => x.DownloadTestCaseTime);

                        _problemRepository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.DataError);
            }

            JobLocker.IsDownloadTestCasesInProcess = false;
        }

        /// <summary>
        ///     Down load submission details
        /// </summary>
        public void GetSubmissionInfo()
        {
            if (JobLocker.IsDownloadSubmissionInfoInProcess) return;

            JobLocker.IsDownloadSubmissionInfoInProcess = true;

            try
            {

                using (var client = new SpojClient())
                {
                    var adminAccountTask = _adminSettingCacheBusiness.GetFullInfo();
                    adminAccountTask.Wait();
                    var adminAccount = adminAccountTask.Result;
                    if (string.IsNullOrEmpty(adminAccount.Username) || string.IsNullOrEmpty(adminAccount.Password))
                    {
                        JobLocker.IsDownloadSubmissionInfoInProcess = false;
                        return;
                    }
                    var result = client.LoginAsync(adminAccount.Password, adminAccount.Password);
                    result.Wait();
                    var submissions = _submissionRepository.Get(x => x.IsDownloadedInfo != true && x.IsNotHaveEnoughInfo != true &&
                                    x.Problem.IsSkip != true).Include(x => x.Problem).OrderByDescending(x => x.SubmitTime).Take(50).ToList();
                    foreach (var submission in submissions)
                    {
                        if (submission == null) continue;
                        if (submission.Problem == null)
                        {
                            submission.IsNotHaveEnoughInfo = true;
                            _submissionRepository.Update(submission, x => x.IsNotHaveEnoughInfo);

                            _submissionRepository.SaveChanges();
                            continue;
                        }
                        if (!Regex.IsMatch(submission.Problem.Code, "^EI\\w+"))
                        {
                            submission.Problem.IsSkip = true;
                            _problemRepository.Update(submission.Problem);
                            _problemRepository.SaveChanges();
                            continue;
                        }

                        var plaintext = client.GetText(string.Format(_submissionInfoUrl, adminAccountTask.Result.ContestName, submission.SpojId));
                        var matches = Regex.Matches(plaintext, "test (\\d+) - (\\w+)");

                        var listResultEnities = new List<ResultEntity>();

                        foreach (Match match in matches)
                        {
                            var resultType = GetResultType(match.Groups[2].Value);
                            listResultEnities.Add(new ResultEntity
                            {
                                SubmissionId = submission.Id,
                                TestCaseSeq = int.Parse(match.Groups[1].Value),
                                Result = resultType
                            });
                        }

                        _resultRepository.InsertRange(listResultEnities);
                        _resultRepository.SaveChanges();

                        submission.IsDownloadedInfo = true;
                        submission.DownloadedTime = DateTime.Now;
                        _submissionRepository.Update(submission, x => x.IsDownloadedInfo, x => x.DownloadedTime);
                        _submissionRepository.SaveChanges();

                        listResultEnities = new List<ResultEntity>();
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

        public async Task<AdminSettingSpojAccountResponseModel> GetSpojAccountAsync()
        {
            var data = await _adminSettingCacheBusiness.GetAdminAccountAsync();

            return new AdminSettingSpojAccountResponseModel
            {
                UserName = data.Username
            };
        }

        #region Private

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

                var listIdsCache = _problemCacheBusiness.GetIds();
                List<int> listNonExisting = new List<int>();
                foreach (var id in listCheckingIds)
                {
                    if (!listIdsCache.Contains(id))
                        listNonExisting.Add(id);
                }

                var listEntitties = listChunk.Where(x => listNonExisting.Contains(x.Id)).Select(model => new ProblemEntity
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
                _problemCacheBusiness.AddRangeIds(listNonExisting);
            }
            _problemRepository.SaveChanges();


            return prolems;
        }

        private Dictionary<int, SpojUserModel> ParseUsers(SpojDataTokenizer tokenizer)
        {
            var users = new Dictionary<int, SpojUserModel>();
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
                var listExist = _accountRepository.Get(x => ids.Contains(x.SpojUserId)).Select(x => x.SpojUserId).ToList();
                var listEntites = listChunk.Where(x => !listExist.Contains(x.UserId)).Select(x => new AccountEntity
                {
                    SpojUserId = x.UserId,
                    UserName = x.Username,
                    DisplayName = x.DisplayName,
                    Email = x.Email
                });

                _accountRepository.InsertRange(listEntites);
                _accountRepository.SaveChanges();

                listChunk = new List<SpojUserModel>();
                listChunkIds = new List<int>();
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
                var listIdsCache = _submissionCacheBusiness.GetIds();
                List<int> listNonExisting = new List<int>();
                foreach (var id in listChunkIds)
                {
                    if (!listIdsCache.Contains(id))
                        listNonExisting.Add(id);
                }

                var listNotExist = listChunk.Where(x => listNonExisting.Contains(x.Id));

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
                                        ProblemId = internalProblemId == 0 ? (int?)null : internalProblemId,
                                        AccountId = internalAccountId == 0 ? (int?)null : internalAccountId
                                    }).ToList();
                _submissionRepository.InsertRange(listEntities);
                _submissionRepository.SaveChanges();

                listChunk = new List<SpojSubmissionModel>();
                listChunkIds = new List<int>();
                _submissionCacheBusiness.AddRangeIds(listNonExisting);
            }

        }

        public async Task<AdminSettingModel> UpdateAdminSetting(AdminSettingUpdateModel model)
        {
            var adminInfo = await _adminSettingCacheBusiness.GetFullInfo();
            var entityToUpdate = new AdminSettingEntity
            {
                Id = adminInfo.Id,
                TestCaseLimit = adminInfo.TestCaseLimitation,
                ContestName = adminInfo.ContestName,
                SpojUserNameEncode = DataSecurityUltils.Encrypt(adminInfo.Username, ApplicationConfigs.SpojKey.ForUserName),
                SpojPasswordEncode = DataSecurityUltils.Encrypt(adminInfo.Password, ApplicationConfigs.SpojKey.ForPassword),
                SystemEmail = adminInfo.SystemEmail,
                SystemEmailPasswordEncode = DataSecurityUltils.Encrypt(adminInfo.EmailPassword, ApplicationConfigs.SpojKey.ForPassword)
            };


            // If update spoj account
            if (model.IsUpdateAccount)
            {
                if(model.IsNewAccount)
                {
                    if(model.NewPassword != model.ConfirmPassword)
                        throw new SpojDebugException("New password and confirmed password not match");

                }
                else
                {
                    if(model.OldPassword != adminInfo.Password)
                    {
                        throw new SpojDebugException("Wrong password");

                    }

                    if(model.NewPassword != model.ConfirmPassword)
                    {
                        throw new SpojDebugException("New password and confirmed password not match");
                    }
                }


                entityToUpdate.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.Username, ApplicationConfigs.SpojKey.ForUserName);
                entityToUpdate.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.NewPassword, ApplicationConfigs.SpojKey.ForPassword);
            }


            if (model.IsUpdateEmail)
            {
                if (model.IsNewEmail)
                {
                    if (model.NewEmailPassword != model.ConfirmEmailPassword)
                        throw new SpojDebugException("New email password and confirmed email password not match");
                }
                else
                {
                    if(model.OldEmailPassword != adminInfo.EmailPassword)
                    {
                        throw new SpojDebugException("Wrong email password");
                    }

                    if(model.NewEmailPassword != model.ConfirmEmailPassword)
                    {
                        throw new SpojDebugException("Email new password and confirmed password not match");
                    }
                }

                entityToUpdate.SystemEmail = model.SystemEmail;
                entityToUpdate.SystemEmailPasswordEncode = DataSecurityUltils.Encrypt(model.NewEmailPassword, ApplicationConfigs.SpojKey.ForPassword);

            }

            if (model.IsUpdateConfig)
            {
                entityToUpdate.ContestName = model.ContestName;

                if (model.Unlimited)
                    entityToUpdate.TestCaseLimit = null;
                else
                    entityToUpdate.TestCaseLimit = model.TestCaseLimitation;
            }
            

            Repository.Update(entityToUpdate, 
                x => x.SpojUserNameEncode,
                x => x.SpojPasswordEncode,
                x => x.SystemEmail,
                x => x.SystemEmailPasswordEncode,
                x => x.TestCaseLimit,
                x => x.ContestName);

            Repository.SaveChanges();

            _adminSettingCacheBusiness.RemoveCache();

            return await _adminSettingCacheBusiness.GetCache();
        }

        #endregion
    }
}
