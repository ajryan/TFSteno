using System;
using System.Web.Mvc;
using TFSteno.Services;

namespace TFSteno.Controllers
{
#if !DEBUG
    [RequireHttps]
#endif
    public class SignupController : Controller
    {
        public ActionResult Landing()
        {
            ViewBag.Title = "Team Foundation Stenographer - Sign Up";
            return View();
        }

        public ActionResult Complete(string signupEmail)
        {
            ViewBag.Title = "Team Foundation Stenographer - Ready!";
            return View(model: signupEmail);
        }

        public ActionResult Confirm(string confirmationCode)
        {
            ViewBag.Title = "Team Foundation Steographer - Confirm Registration";
            var outcome = RegistrationService.ConfirmRegistration(confirmationCode);

            return View(outcome);
        }
    }
}
