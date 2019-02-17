using SpojDebug.Business.TestCase;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Data.Repositories.TestCase;
using AutoMapper;
using SpojDebug.Core.Models.TestCase;
using SpojDebug.Ultil.FileHelper;
using SpojDebug.Data.Repositories.Submission;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SpojDebug.Ultil.Exception;
using SpojDebug.Core.Constant;
using SpojDebug.Ultil.Reflection;
using System.IO;
using SpojDebug.Core.AppSetting;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Submission;
using SpojDebug.Data.Repositories.Problem;
using System.Text.RegularExpressions;
using SpojDebug.Ultil.DataSecurity;
using SpojDebug.Data.Repositories.AdminSetting;
using System;
using SpojDebug.Ultil.Logger;
using SpojDebug.Business.Cache;
using SpojDebug.Core.Models.AdminSetting;

namespace SpojDebug.Business.Logic.TestCase
{
    public class TestCaseBusiness : Business<ITestCaseRepository, TestCaseInfoEntity>, ITestCaseBusiness
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly IAdminSettingRepository _adminSettingRepository;
        private readonly IAdminSettingCacheBusiness _adminSettingCacheBusiness;

        private readonly string _inputTestCaseUrl = "/problems/{0}/{1}.in";

        private readonly string _outputTestCaseUrl = "/problems/{0}/{1}.out";

        public TestCaseBusiness(ITestCaseRepository repository, IMapper mapper,
            ISubmissionRepository submissionRepository,
            IProblemRepository problemRepository,
            ITestCaseRepository testCaseRepository,
            IAdminSettingRepository adminSettingRepository,
            IAdminSettingCacheBusiness adminSettingCacheBusiness) : base(repository, mapper)
        {
            _submissionRepository = submissionRepository;
            _problemRepository = problemRepository;
            _testCaseRepository = testCaseRepository;
            _adminSettingRepository = adminSettingRepository;
            _adminSettingCacheBusiness = adminSettingCacheBusiness;
        }

        public async Task<TestCaseResponseModel> GetFirstFailForFailerAsync(int submissionId, string userId)
        {
            var submission = await GetSubmissionFailDetail(submissionId, userId);

            if (submission == null)
                throw new SpojDebugException("Submission not found");

            return await ParseTestCase(submission);
        }

        public async Task<TestCaseDetailResonseModel> GetTestCaseDetailAsync(int testCaseSeq)
        {
            var model = new TestCaseDetailResonseModel { TestCaseSeq = testCaseSeq };
            return new TestCaseDetailResonseModel();
        }

        public async Task<TestCaseResponseModel> SearchFirstFailForFailerAsync(int submissionId, string userId)
        {
            var submission = await GetSubmissionFailDetail(submissionId, userId);

            if (submission == null)
                return null;

            return await ParseTestCase(submission);
        }
        private AdminAccountModel GetAdminUsernameAndPassword()
        {
            var setting = _adminSettingCacheBusiness.GetAdminAccount();
            return setting;
        }

        public void SyncTestCase(string problemCode)
        {
            using (var client = new SpojClient())
            {
                var adminAccount = GetAdminUsernameAndPassword();
                if (string.IsNullOrEmpty(adminAccount.Username) || string.IsNullOrEmpty(adminAccount.Password))
                {
                    return;
                }

                var loginTask = client.LoginAsync(adminAccount.Username, adminAccount.Password);
                loginTask.Wait();
                var thisProblem = _problemRepository.Get(x => x.Code == problemCode).FirstOrDefault();
                if (thisProblem == null)
                    return;
                if (!Regex.IsMatch(thisProblem.Code, "^EI\\w+"))
                {
                    thisProblem.IsSkip = true;
                    _problemRepository.Update(thisProblem, x => x.IsSkip);

                    _problemRepository.SaveChanges();

                    return;
                }
                var numberOfTestCase = client.GetTotalTestCase(thisProblem.Code);
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"TestCases/{thisProblem.Code}");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var testCaseEntity = _testCaseRepository.Get(x => x.ProblemId == thisProblem.Id).FirstOrDefault();
                if (testCaseEntity == null)
                {
                    testCaseEntity = new TestCaseInfoEntity
                    {
                        ProblemId = thisProblem.Id,
                        TotalTestCase = numberOfTestCase,
                        Path = path
                    };
                    _testCaseRepository.Insert(testCaseEntity);
                }

                for (int i = 0; i < numberOfTestCase; i++)
                {

                    var input = "";
                    var output = "";
                    try
                    {
                        input = client.GetText(string.Format(_inputTestCaseUrl, thisProblem.Code, i));
                        output = client.GetText(string.Format(_outputTestCaseUrl, thisProblem.Code, i));
                        File.WriteAllText(Path.Combine(path, $"{i}.in"), input);
                        File.WriteAllText(Path.Combine(path, $"{i}.out"), output);
                    }
                    catch (Exception e)
                    {
                        LogHepler.WriteSystemErrorLog(e, ApplicationConfigs.SystemInfo.ErrorLogFolderPath);
                    }
                }

                thisProblem.IsDownloadedTestCase = true;
                thisProblem.DownloadTestCaseTime = DateTime.Now;
                _problemRepository.Update(thisProblem, x => x.IsDownloadedTestCase, x => x.DownloadTestCaseTime);

                _problemRepository.SaveChanges();
            }
        }

        private async Task<SubmissionGetFirstFailModel> GetSubmissionFailDetail(int submissionId, string userId)
        {
            return await _submissionRepository.Get(x => x.SpojId == submissionId && x.Account.UserId == userId)
                .Select(x => new SubmissionGetFirstFailModel
                {
                    FirstFailTestCase = x.Results.OrderBy(y => y.TestCaseSeq).Select(y => new TestCaseResultSeqPairModel
                    {
                        Result = y.Result,
                        SeqNum = y.TestCaseSeq,
                    }).FirstOrDefault(y => y.Result != Enums.ResultType.Accepted),
                    ProblemCode = x.Problem.Code,
                    SubmissionSpojId = x.SpojId
                })
                .FirstOrDefaultAsync();
        }

        private async Task<TestCaseResponseModel> ParseTestCase(SubmissionGetFirstFailModel submission)
        {
            var firstFailResult = submission.FirstFailTestCase;
            var configs = await _adminSettingCacheBusiness.GetCache();
            var input = "Test case has not downloaded!";
            var output = "Test case has not downloaded!";
            var testSeq = firstFailResult == null ? int.MaxValue : firstFailResult.SeqNum;
            var inputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.ProblemCode, $"{testSeq}.in"));
            var outputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.ProblemCode, $"{testSeq}.out"));
            if (File.Exists(inputPath))
                input = FileUltils.ReadFileAllText(inputPath);

            if (File.Exists(outputPath))
                output = FileUltils.ReadFileAllText(outputPath);

            if (configs?.TestCaseLimitation != null)
            {
                if (input.Length > configs.TestCaseLimitation)
                    input = input.Substring(0, configs.TestCaseLimitation.Value)
                        .Replace("\r\n", "\n").Replace("\n", "\r\n")
                        + "...";

                if (output.Length > configs.TestCaseLimitation)
                    output = output.Substring(0, configs.TestCaseLimitation.Value)
                        .Replace("\r\n", "\n").Replace("\n", "\r\n")
                        + "...";
            }

            var model = new TestCaseResponseModel
            {
                SubmissionId = submission.SubmissionSpojId,
                ProblemCode = submission.ProblemCode,
                ResultName = firstFailResult == null ? Enums.ResultType.Accepted.GetDisplayName() : firstFailResult.Result.GetDisplayName(),
                TestCaseSeq = firstFailResult == null ? -1 : firstFailResult.SeqNum,
                Input = input,
                Output = output
            };

            return model;
        }
    }
}
