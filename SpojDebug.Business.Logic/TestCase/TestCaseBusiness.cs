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

        public TestCaseResponseModel GetFirstFailForFailer(int submissionId, string userId)
        {
            var submission = _submissionRepository.Get(x => x.SpojId == submissionId && x.Account.UserId == userId)
                .Include(x => x.Account)
                .Include(x => x.Problem)
                .Include(x => x.Results)
                .FirstOrDefault();
            if (submission == null)
                throw new SpojDebugException("Submission not found");
            var firstFailResult = submission.Results.OrderBy(x => x.TestCaseSeq).FirstOrDefault(x => x.Result != Enums.ResultType.Accepted);

            var input = "Test case has not downloaded!";
            var output = "Test case has not downloaded!";
            var testSeq = firstFailResult == null ? int.MaxValue: firstFailResult.TestCaseSeq;
            var inputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.Problem.Code, $"{testSeq}.in"));
            var outputPath = Path.Combine(ApplicationConfigs.SystemInfo.TestCaseFolder, Path.Combine(submission.Problem.Code, $"{testSeq}.out"));
            if (File.Exists(inputPath))
                input = FileUltils.ReadFileAllText(inputPath);

            if (File.Exists(outputPath))
                output = FileUltils.ReadFileAllText(outputPath);

            if (input.Length > 2000)
                input = input.Substring(0, 2000) + "...";

            if (output.Length > 2000)
                output = output.Substring(0, 2000) + "...";

            var model = new TestCaseResponseModel
            {
                SubmissionId = submission.Id,
                ProblemCode = submission.Problem.Code,
                ResultName = firstFailResult == null ? Enums.ResultType.Accepted.GetDisplayName() : firstFailResult.Result.GetDisplayName(),
                TestCaseSeq = firstFailResult == null ? -1 : firstFailResult.TestCaseSeq,
                Input = input,
                Output = output
            };

            return model;
        }

        public TestCaseDetailResonseModel GetTestCaseDetail(int testCaseSeq)
        {
            var model = new TestCaseDetailResonseModel { TestCaseSeq = testCaseSeq };
            return new TestCaseDetailResonseModel();
        }
    }
}
