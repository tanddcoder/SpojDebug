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
        public IActionResult UpdateSpojAccount([Bind("UserName,Password,ConfirmPassword")] AdminSettingSpojAccountUpdateModel model)
        {
            _adminSettingService.UpdateSpojAccount(model);

            return View();
        }

        [HttpPut]
        public async Task<IActionResult> SyncTestCase([FromRoute] int problemId)
        {
            await _testCaseService.SyncTestCase(problemId);

            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> AdminSetting()
        {
            var response = await _adminSettingService.GetAdminSetting();

            return View(response);
        }

        public async Task<IActionResult> AdminSetting(AdminSettingUpdateModel model)
        {
            var response = await _adminSettingService.UpdateAdminSetting(model);

            return View(response);
        }
    }
}