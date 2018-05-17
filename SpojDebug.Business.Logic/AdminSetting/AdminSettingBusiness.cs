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

        public AdminSettingBusiness(
            IAdminSettingRepository repository,
            IMapper mapper,
            SpojKey spojKey,
            SpojInfo spojInfo,
            IProblemRepository problemRepository,
            IAccountRepository accountRepository,
            ISubmissionRepository submissionRepository,
            IResultRepository resultRepository,
            ITestCaseRepository testCaseRepository) : base(repository, mapper)
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
                    //ignore
                    var path = Path.Combine(Directory.GetCurrentDirectory(), $"Log/Error.txt");
                    File.AppendAllText(path, e.StackTrace);
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
                Repository.SaveChanges();
                return;
            }


            setting.SpojUserNameEncode = DataSecurityUltils.Encrypt(model.UserName, _spojKey.ForUserName);
            setting.SpojPasswordEncode = DataSecurityUltils.Encrypt(model.Password, _spojKey.ForPassword);
            Repository.TryToUpdate(setting);
            Repository.SaveChanges();
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
                            Thread.Sleep(60000);
                            var aa = _problemRepository.Get();
                            var problem = _problemRepository.Get().FirstOrDefault(x => x.IsDownloadedTestCase != true && x.IsSkip != true);
                            if (problem == null)
                                continue;
                            if (!Regex.IsMatch(problem.Code, "^EI\\w+"))
                            {
                                problem.IsSkip = true;
                                _problemRepository.TryToUpdate(problem);
                                _problemRepository.SaveChanges();
                                continue;
                            }

                            var maxTestCase = 0;
                            try
                            {
                                maxTestCase = client.GetTotalTestCase(problem.Code);
                            }
                            catch (Exception e)
                            {
                                
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
                                }
                                catch (Exception e)
                                {
                                    
                                }
                                File.WriteAllText(Path.Combine(path, $"{i}.in"), input);
                                File.WriteAllText(Path.Combine(path, $"{i}.out"), output);
                            }

                            problem.IsDownloadedTestCase = true;
                            problem.DownloadTestCaseTime = DateTime.Now;
                            _problemRepository.TryToUpdate(problem);
                            _problemRepository.SaveChanges();

                            // after 10 minutes, we login again
                            var a = watch.ElapsedMilliseconds;
                            if (a > 600000) break;
                        }
                    }
                    watch.Stop();
                }
                catch (Exception e)
                {
                    // ignore
                    var path = Path.Combine(Directory.GetCurrentDirectory(), $"Error.txt");
                    File.AppendAllText(path, e.StackTrace);
                    Thread.Sleep(120000);
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
                            Thread.Sleep(60000);
                            var aa = _submissionRepository.Get()
                                .Where(x => x.IsDownloadedInfo != true && x.IsNotHaveEnoughInfo != true)
                                .Include(x => x.Problem).ToList(); ;
                            var submission = _submissionRepository.Get().Where(x => x.IsDownloadedInfo != true && x.IsNotHaveEnoughInfo != true).Include(x => x.Problem).FirstOrDefault();
                            if (submission == null) continue;
                            if (submission.Problem == null)
                            {
                                submission.IsNotHaveEnoughInfo = true;
                                _submissionRepository.TryToUpdate(submission);
                                _submissionRepository.SaveChanges();
                                continue;
                            }
                            if (!Regex.IsMatch(submission.Problem.Code, "^EI\\w+"))
                            {
                                submission.Problem.IsSkip = true;
                                _problemRepository.TryToUpdate(submission.Problem);
                                _problemRepository.SaveChanges();
                                continue;
                            }

                            var plaintext = client.GetText(string.Format(_submissionInfoUrl, _spojInfo.ContestName, submission.SpojId));
                            var matches = Regex.Matches(plaintext, "test (\\d+) - (\\w+)");

                            foreach (Match match in matches)
                            {
                                var resultType = GetResultType(match.Groups[2].Value);

                                _resultRepository.Insert(new ResultEntity
                                {
                                    SubmmissionId = submission.Id,
                                    TestCaseSeq = int.Parse(match.Groups[1].Value),
                                    Result = resultType
                                });

                                _resultRepository.SaveChanges();
                            }

                            submission.IsDownloadedInfo = true;
                            submission.DownloadedTime = DateTime.Now;
                            _submissionRepository.TryToUpdate(submission);

                            // after 10 minutes, we login again
                            var a = watch.ElapsedMilliseconds;
                            if (a > 600000) break;
                        }
                    }
                    watch.Stop();
                }
                catch (Exception e)
                {
                    // ignore / writelog
                    var path = Path.Combine(Directory.GetCurrentDirectory(), $"Error.txt");
                    File.AppendAllText(path, e.StackTrace);
                    Thread.Sleep(120000);
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
            for (var i = 0; i < nProblems; i++)
            {
                var problemDetail = new SpojProblemInfoModel()
                {
                    Id = tokenizer.GetInt(),
                    TimeLimit = tokenizer.GetFloat(),
                    Code = tokenizer.GetNext(),
                    Name = tokenizer.GetNext(),
                    Type = tokenizer.GetInt(),
                    ProblemSet = tokenizer.GetNext()
                };

                if (!_problemRepository.Get(x => x.SpojId == problemDetail.Id).Any())
                {
                    _problemRepository.Insert(new ProblemEntity
                    {
                        SpojId = problemDetail.Id,
                        TimeLimit = problemDetail.TimeLimit,
                        Code = problemDetail.Code,
                        Name = problemDetail.Name,
                        Type = problemDetail.Type,
                        SpojProblemSet = problemDetail.ProblemSet
                    });
                    _problemRepository.SaveChanges();
                }
                tokenizer.Skip(nLines - 6);
                prolems[problemDetail.Id] = problemDetail;
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

                if (!_accountRepository.Get(x => x.SpojUserId == user.UserId).Any())
                    _accountRepository.Insert(new AccountEntity
                    {
                        SpojUserId = user.UserId,
                        UserName = user.Username,
                        DisplayName = user.DisplayName,
                        Email = user.Email
                    });
            }

            _accountRepository.SaveChanges();
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
                    Language = languageText
                };

                if (!_submissionRepository.Get(x => x.SpojId == submission.Id).Any())
                {
                    var internalProblemId = _problemRepository.Get(x => x.SpojId == spojProblemId).Select(x => x.Id).FirstOrDefault();
                    var internalAccountId = _accountRepository.Get(x => x.SpojUserId == userId).Select(x => x.Id).FirstOrDefault();

                    var entity = new SubmissionEntity
                    {
                        SpojId = submission.Id,
                        SubmitTime = submission.Time,
                        Score = submission.Score,
                        RunTime = submission.RunTime,
                        Language = languageText,
                        ProblemId = internalProblemId == 0 ? (int?)null : internalProblemId,
                        AccountId = internalAccountId == 0 ? (int?)null : internalAccountId
                    };
                    _submissionRepository.Insert(entity);
                    _submissionRepository.SaveChanges();
                }

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

        private void GetSubmissionResult(int spojId)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
