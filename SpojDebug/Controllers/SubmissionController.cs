using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Core.User;
using SpojDebug.Service.Submission;
using SpojDebug.Ultil.Exception;

namespace SpojDebug.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubmissionController(ISubmissionService submissionService, UserManager<ApplicationUser> userManager)
        {
            _submissionService = submissionService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> WhereFailerTakePlaceLoL(int? id)
        {
            if (id == null)
                throw new SpojDebugException("Id cannot be null, please try again!");
            var resonse = await _submissionService.GetFirstFailForFailerAsync(id.Value);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search([Bind("SubmissionId")] int? submissionId)
        {
            if (submissionId == null)
                throw new SpojDebugException("Id cannot be null, please try again!");

            var userId = _userManager.GetUserId(User);
            var response = await _submissionService.SearchSubmssionAsync(userId, submissionId.Value);

            if (response.Data == null)
                throw new SpojDebugException("Submission not exist!");

            return View("~/Views/TestCase/WhereFailerTakePlace.cshtml", response);
        }

        [HttpPost]
        public async Task<IActionResult> SearchWithoutCheck([Bind("SubmissionId")] int? submissionId)
        {
            if (submissionId == null)
                throw new SpojDebugException("Id cannot be null, please try again!");

            var userId = _userManager.GetUserId(User);
            var response = await _submissionService.SearchSubmssionAsync(userId, submissionId.Value);
            await _submissionService.EnqueueToDownloadAsync(userId, submissionId.Value);
            response = await _submissionService.SearchSubmssionAsync(userId, submissionId.Value);
            if (response.Data == null)
                throw new SpojDebugException("Submission not exist!");

            return View("~/Views/TestCase/WhereFailerTakePlace.cshtml", response);
        }
    }
}