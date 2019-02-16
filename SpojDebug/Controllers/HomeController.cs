using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core;
using SpojDebug.Core.User;
using SpojDebug.Service.Submission;
using SpojDebug.Service.User;

namespace SpojDebug.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public HomeController(ISubmissionService submissionService, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _submissionService = submissionService;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var userInfo = _userService.GetCurrentUser(User);

            if(userInfo.Accounts == null || !userInfo.Accounts.Any())
            {
                return RedirectToAction("SpojAccountCenter", "Manage");
            }
            var response = await _submissionService.GetUserSubmissionAsync(userId);

            return View(response);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
