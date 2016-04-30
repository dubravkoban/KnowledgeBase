using KnowledgeBase.Database;
using KnowledgeBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KnowledgeBase.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var newLink = new Links();
            newLink.Title = "my new link";
            newLink.Description = "this is my new link";
            newLink.Url = "http://www.google.com";

            using (var ctx = new DbModel())
            {
                ctx.Links.Add(newLink);

                ctx.SaveChanges();


                var links = ctx.Links.Select(l => l.Title).ToList();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}