﻿using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;

namespace SpojDebug.Business.Submission
{
    public interface ISubmissionBusiness : IBusiness<SubmissionEntity>
    {
        List<SubmissionHomeModel> GetUserSubmission(string userId);
    }
}
