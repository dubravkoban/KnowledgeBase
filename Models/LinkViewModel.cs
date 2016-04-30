using KnowledgeBase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeBase.Models
{
    public class LinkViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<KeywordViewModel> Keywords { get; set; }
    }
}