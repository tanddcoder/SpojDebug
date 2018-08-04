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

namespace SpojDebug.Business.Logic.TestCase
{
    public class TestCaseBusiness : Business<ITestCaseRepository, TestCaseInfoEntity>, ITestCaseBusiness
    {
        private readonly ISubmissionRepository _submissionRepository;

        public TestCaseBusiness(ITestCaseRepository repository, IMapper mapper,
            ISubmissionRepository submissionRepository) : base(repository, mapper)
        {
            _submissionRepository = submissionRepository;
        }

        public async Task<TestCaseResponseModel> GetFirstFailForFailerAsync(int submissionId, string userId)
        {
            var submission = await GetSubmissionFailDetail(submissionId, userId);

            if (submission == null)
                throw new SpojDebugException("Submission not found");

            return ParseTestCase(submission);
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

            return ParseTestCase(submission);
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

        private TestCaseResponseModel ParseTestCase(SubmissionGetFirstFailModel submission)
        {
            var firstFailResult = submission.FirstFailTestCase;

            var input = "Test case has not downloaded!";
            var output = "Test case has not downloaded!";
            var testSeq = firstFailResult == null ? int.MaxValue : firstFailResult.SeqNum;
            var inputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.ProblemCode, $"{testSeq}.in"));
            var outputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.ProblemCode, $"{testSeq}.out"));
            if (File.Exists(inputPath))
                input = FileUltils.ReadFileAllText(inputPath);

            if (File.Exists(outputPath))
                output = FileUltils.ReadFileAllText(outputPath);

            if (input.Length > 2000)
                input = input.Substring(0, 2000)
                    .Replace("\r\n", "\n").Replace("\n", "\r\n")
                    + "...";

            if (output.Length > 2000)
                output = output.Substring(0, 2000)
                    .Replace("\r\n", "\n").Replace("\n", "\r\n")
                    + "...";

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
