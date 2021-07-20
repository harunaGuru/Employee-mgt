using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        [Route("error/{statusCode}")]
        public IActionResult httpsStatusCodeHandler(int statusCode)
        {
            var statuscodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry the Resource you requested could not be found";
                    logger.LogWarning($"404 Errror ocurred. path = {statuscodeResult.OriginalPath}" +
                        $"and Querystring = {statuscodeResult.OriginalQueryString}");

                    //ViewBag.Path = statuscodeResult.OriginalPath;
                    //ViewBag.QS = statuscodeResult.OriginalQueryString;
                    break;
            }
            return View("Not Found");
        }
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError($"The Path {exceptionDetails.Path} threw an Exception {exceptionDetails.Error}");
            //ViewBag.ErrorMessage = exceptionDetails.Error.Message;
            //ViewBag.ErrorPath = exceptionDetails.Path;
            //ViewBag.ExceptionStacktrace = exceptionDetails.Error.StackTrace;

            return View("Error");
        }
    }
}
