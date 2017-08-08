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
    public class CourseController : Controller
    {
        private readonly CanvasExtendContenxt _context;
        public CourseController(CanvasExtendContenxt context)
        {
            _context = context;
        }

        [Route("Course/Details/{accountId}/{courseId}")]
        public ActionResult Details(int accountId, int courseId)
        {
            ViewBag.CourseId = courseId;
            ViewBag.AccountId = accountId;
            ViewBag.Title = "Home Page";
            return View();
        }
    }
}