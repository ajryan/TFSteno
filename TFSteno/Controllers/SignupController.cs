﻿using System;
using System.Web.Mvc;

namespace TFSteno.Controllers
{
    public class SignupController : Controller
    {
#if !DEBUG
        [RequireHttps]
#endif
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
    }
}
