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

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, SubmissionEntity>, ISubmissionBusiness
    {
        private readonly IAccountRepository _accountRepository;

        public SubmissionBusiness(ISubmissionRepository repository, IMapper mapper, IAccountRepository accountRepository) : base(repository, mapper)
        {
            _accountRepository = accountRepository;
        }

        public List<SubmissionHomeModel> GetUserSubmission(string userId)
        {
            var account = _accountRepository.Get(x => x.UserId == userId).FirstOrDefault();
            if (account == null) return new List<SubmissionHomeModel>();

            var availableSubmission = Repository.Get(x => x.AccountId == account.Id && x.Results.Any()).OrderByDescending(x => x.SpojId).Include(x => x.Results).Include(x => x.Problem).Take(2).ToList();
            var finalResult = new List<SubmissionHomeModel>();
            foreach (var submission in availableSubmission)
            {
                var resultEntity = submission.Results.OrderBy(x => x.TestCaseSeq).FirstOrDefault(y => y.Result != Enums.ResultType.Accepted);

                finalResult.Add(new SubmissionHomeModel
                {
                    SubmissionId = submission.SpojId,
                    AcceptedTestCase = submission.Results.Count(y => y.Result == Enums.ResultType.Accepted),
                    TotalTestCase = submission.Results.Count,
                    ProblemCode = submission.Problem.Code,
                    FirtFailTestCase = resultEntity == null? -1 :resultEntity.TestCaseSeq
                });
            }

            return finalResult.OrderByDescending(x => x.SubmissionId).ToList();
        }
    }
}
