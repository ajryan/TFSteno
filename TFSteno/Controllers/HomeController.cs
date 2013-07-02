using System;
using System.Web.Mvc;

namespace TFSteno.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Team Foundation Stenographer";
            return View();
        }
    }
}
