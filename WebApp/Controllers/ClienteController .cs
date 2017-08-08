using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Common.WepApi;
using WebApp.Models.Services;

namespace WebApp.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
        public ActionResult List()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}