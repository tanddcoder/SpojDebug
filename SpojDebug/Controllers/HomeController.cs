using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core;
using SpojDebug.Core.User;
using SpojDebug.Service.Submission;

namespace SpojDebug.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ISubmissionService submissionService, UserManager<ApplicationUser> userManager)
        {
            _submissionService = submissionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

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
