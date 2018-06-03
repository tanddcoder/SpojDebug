using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core.User;
using SpojDebug.Service.TestCase;
using SpojDebug.Ultil.Exception;

namespace SpojDebug.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestCaseService _testCaseService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestCaseController(ITestCaseService testCaseService, UserManager<ApplicationUser> userManager)
        {
            _testCaseService = testCaseService;
            _userManager = userManager;
        }
        
        [HttpPost]
        [Route("WhereFailerTakePlace/{submissionId}")]
        public IActionResult WhereFailerTakePlace(int? submissionId)
        {
            if (submissionId == null)
                throw new SpojDebugException("Id Null");

            var userId = _userManager.GetUserId(User); 
            var response = _testCaseService.GetFirstFailForFailer(submissionId.Value, userId);

            return View();
        }
    }
}