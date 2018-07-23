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
        
        [HttpGet]
        [ResponseCache(Duration = 1440)]
        public IActionResult WhereFailerTakePlace(int? id)
        {
            if (id == null)
                throw new SpojDebugException("Id Null");

            var userId = _userManager.GetUserId(User); 
            var response = _testCaseService.GetFirstFailForFailer(id.Value, userId);

            return View(response);
        }
    }
}