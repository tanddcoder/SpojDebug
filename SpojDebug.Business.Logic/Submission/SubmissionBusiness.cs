using SpojDebug.Business.Submission;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Data.Repositories.Submission;
using AutoMapper;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;
using SpojDebug.Data.Repositories.Account;
using System.Linq;
using SpojDebug.Core.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, SubmissionEntity>, ISubmissionBusiness
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMemoryCache _memoryCache;


        public SubmissionBusiness(ISubmissionRepository repository, IMapper mapper, IAccountRepository accountRepository, IMemoryCache memoryCache) : base(repository, mapper)
        {
            _accountRepository = accountRepository;
            _memoryCache = memoryCache;
        }

        public List<SubmissionHomeModel> GetUserSubmission(string userId)
        {
            var account = _accountRepository.Get(x => x.UserId == userId).FirstOrDefault();
            if (account == null) return new List<SubmissionHomeModel>();

            var availableSubmission = Repository.Get(x => x.AccountId == account.Id && x.Results.Any() && x.Score < 100).OrderByDescending(x => x.SpojId)
                .Include(x => x.Results)
                .Include(x => x.Problem)
                .Take(20)
                .Select(x => new
                {
                    Result = x.Results.OrderBy(y => y.TestCaseSeq).FirstOrDefault(y => y.Result != Enums.ResultType.Accepted),
                    x.SpojId,
                    AcceptedTestCase = x.Results.Count(y => y.Result == Enums.ResultType.Accepted),
                    TotalTestCase = x.Results.Count,
                    ProblemCode = x.Problem.Code
                })
                .ToList();
            var finalResult = new List<SubmissionHomeModel>();
            foreach (var submission in availableSubmission)
            {
                var resultEntity = submission.Result;

                finalResult.Add(new SubmissionHomeModel
                {
                    SubmissionId = submission.SpojId,
                    AcceptedTestCase = submission.AcceptedTestCase,
                    TotalTestCase = submission.TotalTestCase,
                    ProblemCode = submission.ProblemCode,
                    FirtFailTestCase = resultEntity == null ? -1 : resultEntity.TestCaseSeq
                });
            }

            return finalResult.OrderByDescending(x => x.SubmissionId).ToList();
        }

       
    }
}
