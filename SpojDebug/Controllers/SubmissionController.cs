using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpojDebug.Service.Submission;
using SpojDebug.Ultil.Exception;

namespace SpojDebug.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }
        
        [HttpGet]
        public IActionResult WhereFailerTakePlaceLoL(int? id)
        {
            if (id == null)
                throw new SpojDebugException("Id can not be null, please try again!");
            var resonse = _submissionService.GetFirstFailForFailer(id.Value);
            return View();
        }
    }
}