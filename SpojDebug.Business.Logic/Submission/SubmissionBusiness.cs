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

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, TestCaseEntity>, ISubmissionBusiness
    {
        private readonly IAccountRepository _accountRepository;

        public SubmissionBusiness(ISubmissionRepository repository, IMapper mapper, IAccountRepository accountRepository) : base(repository, mapper)
        {
            _accountRepository = accountRepository;
        }

        public List<SubmissionHomeModel> GetUserSubmission(string userId)
        {
            var account = _accountRepository.Get(x => x.UserId == userId).FirstOrDefault();
            if (account == null) return null;

            return Repository.Get(x => x.AccountId == account.Id).Select(x => new SubmissionHomeModel {
                SubmissionId = x.SpojId,
                AcceptedTestCase = x.Results.Count(y => y.Result == Enums.ResultType.Accepted),
                TotalTestCase = x.Results.Count,
                ProblemCode = x.Problem.Code,
                FirtFailTestCase = x.Results.FirstOrDefault(y => y.Result != Enums.ResultType.Accepted).TestCaseSeq
            }).ToList();
        }
    }
}
