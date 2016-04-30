using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KnowledgeBase.Models
{
    public class LinkEditViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> Keywords { get; set; }
        public List<string> SelectedKeywords { get; set; }
    }
}