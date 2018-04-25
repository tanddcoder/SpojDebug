using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core.Models.AdminSetting;
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
    }
}