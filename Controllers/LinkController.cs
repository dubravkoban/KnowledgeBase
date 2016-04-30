using KnowledgeBase.Models;
using KnowledgeBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KnowledgeBase.Controllers
{
    public class LinkController : Controller
    {
        private KnowledgeBaseRepository repo;
        private int listCount;

        public LinkController()
        {
            repo = new KnowledgeBaseRepository();
            listCount = 10;
        }

        // GET: Link
        [HttpGet]
        public ActionResult Index()
        {
            var links = repo.GetLinks(listCount);
            return View(links);
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(LinkViewModel vm)
        {
            repo.AddLink(vm);

            return RedirectToAction("Index");
        }
    }
}