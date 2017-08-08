using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Common.WepApi;
using WebApp.Models.Model;
using WebApp.Models.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [Route("Account/Index/{accountId}")]
        public ActionResult Index(int accountId)
        {
            ViewBag.AccountId = accountId;
            ViewBag.Title = "Home Page";
            return View();
        }
        [Route("Account/Details/{parentId}/{accountId}")]
        public ActionResult Details(int parentId, int accountId)
        {
            ViewBag.AccountId = accountId;
            ViewBag.ParentId = parentId;
            ViewBag.Title = "Home Page";
            return View();
        }
        public ActionResult PopupAsociarEstudio()
        {
            return View();
        }
        public ActionResult PopupAsociarAsignatura()
        {
            return View();
        }
        public ActionResult PopupGenerarPeriodo()
        {
            return View();
        }
        public ActionResult PopupAccountPeriodo()
        {
            return View();
        }
        public ActionResult PopupTableMigration()
        {
            return View();
        }
        [Route("Account/DetailsPeriodo/{parentId}/{accountId}")]
        public ActionResult DetailsPeriodo(int parentId, int accountId)
        {
            ViewBag.AccountId = accountId;
            ViewBag.ParentId = parentId;
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}