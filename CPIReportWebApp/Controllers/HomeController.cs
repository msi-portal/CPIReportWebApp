using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CPIReportWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            String config = new ServiceRepository<String>().GetConfiguration();

            ViewBag.Message = config;

            return View();
        }

        public ActionResult Contact()
        {
            String GetToken = new ServiceRepository<String>().GetToken();
            
            ViewBag.Message = GetToken;

            return View();
        }
    }
}