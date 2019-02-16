using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Service.Problem;
using SpojDebug.Service.SPOJExternal;
using SpojDebug.Service.TestCase;
using System.Threading.Tasks;

namespace SpojDebug.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSettingController : Controller
    {
        private readonly IAdminSettingService _adminSettingService;
        private readonly IProblemService _problemService; 
        private readonly ITestCaseService _testCaseService;

        public AdminSettingController(IAdminSettingService adminSettingService, 
            IProblemService problemService,
            ITestCaseService testCaseService)
        {
            _adminSettingService = adminSettingService;
            _problemService = problemService;
            _testCaseService = testCaseService;
        }

        public async Task<IActionResult> SpojAccountAsync()
        {
            var response = await _adminSettingService.GetSpojAccountAsync();

            return View(response);
        }

        [HttpGet]
        public IActionResult UpdateSpojAccount()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UpdateSpojAccount([FromBody] AdminSettingSpojAccountUpdateModel model)
        {
            _adminSettingService.UpdateSpojAccount(model);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SyncTestCase([FromForm] string problemCode)
        {
            _testCaseService.SyncTestCase(problemCode);

            return RedirectToAction("AdminSetting");
        }


        [HttpGet]
        public async Task<IActionResult> AdminSetting()
        {
            var response = await _adminSettingService.GetAdminSetting();
            var data = new AdminSettingUpdateModel
            {
                ContestName = response.ContestName,
                Id = response.Id,
                UserName = response.UserName,
                TestCaseLimitation = response.TestCaseLimitation,
                SystemEmail = response.SystemEmail,
                Unlimited = response.TestCaseLimitation == null ? true : false
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> AdminSetting([FromForm] AdminSettingUpdateModel model)
        {
            var response = await _adminSettingService.UpdateAdminSetting(model);

            var data = new AdminSettingUpdateModel
            {
                ContestName = response.ContestName,
                Id = response.Id,
                UserName = response.UserName,
                TestCaseLimitation = response.TestCaseLimitation,
                SystemEmail = response.SystemEmail
            };

            return View(data);
        }
    }
}