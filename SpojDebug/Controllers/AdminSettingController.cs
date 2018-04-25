using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Service.SPOJExternal;

namespace SpojDebug.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSettingController : Controller
    {
        private readonly IAdminSettingService _adminSettingService;

        public AdminSettingController(IAdminSettingService adminSettingService)
        {
            _adminSettingService = adminSettingService;
        }

        public IActionResult SpojAccount()
        {
            var response = _adminSettingService.GetSpojAccount();

            return View(response);
        }

        public IActionResult UpdateSpojAccount()
        {
            var response = _adminSettingService.UpdateSpojAccount();

            return View(response);
        }
    }
}