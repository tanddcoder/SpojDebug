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
using System.Threading.Tasks;
using SpojDebug.Core.AppSetting;
using SpojDebug.Business.AdminSetting;
using System.Text.RegularExpressions;
using System.Text;
using SpojDebug.Core.Entities.Result;
using System;
using SpojDebug.Data.Repositories.Result;
using SpojDebug.Business.Cache;

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, SubmissionEntity>, ISubmissionBusiness
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IAdminSettingBusiness _adminSettingBusiness;
        private readonly IAdminSettingCacheBusiness _adminSettingCacheBusiness;
        private readonly IResultRepository _resultRepository;
        private readonly ISubmissionCacheBusiness _submissionCacheBusiness;
        private readonly IProblemCacheBusiness _problemCacheBusiness;

        private const string UserSubmissionHistory = "https://www.spoj.com/{0}/status/{1}/all/start={2}";
        private const string _submissionInfoUrl = "/{0}/files/psinfo/{1}/";

        public SubmissionBusiness(
            ISubmissionRepository repository,
            IMapper mapper, 
            IAccountRepository accountRepository, 
            IMemoryCache memoryCache, 
            IAdminSettingBusiness adminSettingBusiness,
            IResultRepository resultRepository,
            ISubmissionCacheBusiness submissionCacheBusiness,
            IProblemCacheBusiness problemCacheBusiness,
            IAdminSettingCacheBusiness adminSettingCacheBusiness) : base(repository, mapper)
        {
            _accountRepository = accountRepository;
            _memoryCache = memoryCache;
            _adminSettingBusiness = adminSettingBusiness;
            _resultRepository = resultRepository;
            _submissionCacheBusiness = submissionCacheBusiness;
            _problemCacheBusiness = problemCacheBusiness;
            _adminSettingCacheBusiness = adminSettingCacheBusiness;
        }

        public async Task InstantDownLoadSubmissionAsync(int accountId, string accountName, int submissionId)
        {
            var listTask = new List<Task<byte[]>>();
            using (var client = new SpojClient())
            {
                var adminAccount = await _adminSettingCacheBusiness.GetFullInfo();
                var loginTask = client.LoginAsync(adminAccount.Username, adminAccount.Password);
                loginTask.Wait();
                for (int i = 0; i < 5; i++)
                {
                    var pageNum = i * 20;
                    var downloadUrl = string.Format(UserSubmissionHistory, adminAccount.ContestName, accountName, pageNum);
                    listTask.Add(client.GetByteArrayAsync(downloadUrl));
                }

                var allTask = Task.WhenAll(listTask);
                allTask.Wait();

                string problemCode = null;
                DateTime submitTime = new DateTime();
                float runtime = -1;

                var listResult = allTask.Result;
                foreach (var listByte in listResult)
                {
                    var pattern = $"/{adminAccount.ContestName}/submit/([0-9A-Z]+)/id={submissionId}";
                    var html = Encoding.UTF8.GetString(listByte);

                    var match = Regex.Match(html, pattern);
                    if (!match.Success)
                        continue;
                    problemCode = match.Groups[1].ToString();

                    //Get submit time
                    var dateSubmitPattern = $"{submissionId}" + " \\([A-Z]+\\), (\\d{4}[-]\\d{2}[-]\\d{2} \\d{2}:\\d{2}:\\d{2})\"";
                    var matchTime = Regex.Match(html, dateSubmitPattern);
                    submitTime = DateTime.ParseExact(matchTime.Groups[1].ToString(), "yyyy-MM-dd HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture);
                    break;
                }
                if (problemCode == null)
                    return;
                string plaintext;

                try
                {
                    plaintext = client.GetText(string.Format(_submissionInfoUrl, adminAccount.ContestName, submissionId));
                }
                catch (Exception e)
                {
                    return;
                }

                // Get results
                var matches = Regex.Matches(plaintext, "test (\\d+) - (\\w+)");

                var problemId = _problemCacheBusiness.GetProblemIdByCode(problemCode);
                if (problemId == 0) return;
                var submissionEntity = new SubmissionEntity
                {
                    ProblemId = problemId,
                    AccountId = accountId,
                    DownloadedTime = DateTime.Now,
                    IsDownloadedInfo = true,
                    SpojId = submissionId,
                    RunTime = runtime,
                    SubmitTime = submitTime,
                    TotalResult = matches.Count
                };
                Repository.Insert(submissionEntity);

                var listResultEnities = new List<ResultEntity>();
                var acCount = 0;
                foreach (Match match in matches)
                {
                    var resultType = GetResultType(match.Groups[2].Value);
                    if (resultType == Enums.ResultType.Accepted)
                        acCount++;
                    listResultEnities.Add(new ResultEntity
                    {
                        SubmissionId = submissionEntity.Id,
                        TestCaseSeq = int.Parse(match.Groups[1].Value),
                        Result = resultType,
                    });
                }

                submissionEntity.Score = ((float)acCount / matches.Count) * 100;

                _resultRepository.InsertRange(listResultEnities);
                //_resultRepository.SaveChanges();
                _resultRepository.SaveChanges();

                Repository.SaveChanges();

                _submissionCacheBusiness.AddId(submissionId);
            }
        }

        public async Task<List<SubmissionHomeModel>> GetUserSubmissionAsync(string userId)
        {
            var account = await _accountRepository.Get(x => x.UserId == userId).FirstOrDefaultAsync();
            if (account == null) return new List<SubmissionHomeModel>();

            var availableSubmission = await Repository.Get(x => x.AccountId == account.Id && x.Results.Any() && x.Score < 100).OrderByDescending(x => x.SpojId)
                .Take(10)
                .Select(x => new
                {
                    Result = x.Results.OrderBy(y => y.TestCaseSeq).FirstOrDefault(y => y.Result != Enums.ResultType.Accepted),
                    x.SpojId,
                    AcceptedTestCase = x.Results.Count(y => y.Result == Enums.ResultType.Accepted),
                    TotalTestCase = x.Results.Count,
                    ProblemCode = x.Problem.Code
                })
                .ToListAsync();
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
    }
}
