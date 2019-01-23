﻿using SpojDebug.Core.Entities.Submission;
using SpojDebug.Business.Submission;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Service.Submission;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpojDebug.Core.Models.TestCase;
using SpojDebug.Business.TestCase;
using SpojDebug.Business.Account;
using Hangfire;

namespace SpojDebug.Service.Logic.Submission
{
    public class SubmissionService : Service<ISubmissionBusiness, SubmissionEntity>, ISubmissionService
    {
        private readonly ISubmissionBusiness _submissionBusiness;
        private readonly ITestCaseBusiness _testCaseBusiness;
        private readonly IAccountBusiness _accountBusiness;

        public SubmissionService(ISubmissionBusiness submissionBusiness, ITestCaseBusiness testCaseBusiness, IAccountBusiness accountBusiness)
        {
            _submissionBusiness = submissionBusiness;
            _testCaseBusiness = testCaseBusiness;
            _accountBusiness = accountBusiness;
        }

        public async Task<ApplicationResult> EnqueueToDownloadAsync(string userId, int submissionId)
        {
            var (acountId, accountName) = await _accountBusiness.GetSpojAccountUsernameAsync(userId);
            await _submissionBusiness.InstantDownLoadSubmissionAsync(acountId, accountName, submissionId) ;
            return ApplicationResult.Ok();
        }

        public async Task<ApplicationResult<SubmissionFirstFailModel>> GetFirstFailForFailerAsync(int submissionId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationResult<List<SubmissionHomeModel>>> GetUserSubmissionAsync(string userId)
        {
            var response = await _submissionBusiness.GetUserSubmissionAsync(userId);

            return ApplicationResult<List<SubmissionHomeModel>>.Ok(response);
        }

        public async Task<ApplicationResult<TestCaseResponseModel>> SearchSubmssionAsync(string userId, int submissionId)
        {
            var response = await _testCaseBusiness.SearchFirstFailForFailerAsync(submissionId, userId);

            return ApplicationResult<TestCaseResponseModel>.Ok(response);
        }
    }
}
