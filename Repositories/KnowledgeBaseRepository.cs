using KnowledgeBase.Database;
using KnowledgeBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeBase.Repositories
{
    public class KnowledgeBaseRepository
    {
        public void AddLink(LinkViewModel linkVm)
        {
            var linkDb = new Links();
            linkDb.Title = linkVm.Title;
            linkDb.Description = linkVm.Description;
            linkDb.Url = linkVm.Url;

            int linkId = -1;
            using (var ctx = new DbModel())
            {
                ctx.Links.Add(linkDb);

                ctx.SaveChanges();
                linkId = linkDb.ID;
            }

            var linksKeywordsDb = new List<LinksKeywords>();
            var keywordsDb = new List<Keywords>();

            if (linkVm.Keywords != null && linkId != -1)
            {
                linkVm.Keywords.ForEach(k =>
                {
                    var keywordDb = new Keywords();
                    keywordDb.Keyword = k.Keyword;
                    keywordsDb.Add(keywordDb);

                    var linkKeywordDb = new LinksKeywords();
                    linkKeywordDb.KeywordID = k.Id;
                    linkKeywordDb.LinkID = linkVm.Id;
                    linksKeywordsDb.Add(linkKeywordDb);
                });


                using (var ctx = new DbModel())
                {
                    ctx.Keywords.AddRange(keywordsDb);
                    ctx.LinksKeywords.AddRange(linksKeywordsDb);

                    ctx.SaveChanges();
                }
            }
        }

        public void DeleteLink(Links link)
        {
            using (var ctx = new DbModel())
            {
                ctx.Links.Remove(link);
                ctx.SaveChanges();
            }
        }

        public List<LinkViewModel> GetLinks(int count)
        {
            using (var ctx = new DbModel())
            {
                var resultVm = new List<LinkViewModel>();

                var links = ctx.Links.OrderByDescending(l => l.ID).Take(count).ToList();
                links.ForEach(l =>
                {
                    var link = new LinkViewModel
                    {
                        Id = l.ID,
                        Title = l.Title,
                        Description = l.Description,
                        Url = l.Url
                    };
                    resultVm.Add(link);
                });

                var linkIds = links.Select(l => l.ID);

                var keywordsPerLink = from k in ctx.Keywords
                                      join lk in ctx.LinksKeywords on k.ID equals lk.KeywordID
                                      where linkIds.Contains(lk.LinkID)
                                      select new KeywordViewModel { Id = k.ID, Keyword = k.Keyword, LinkId = lk.LinkID };

                resultVm.ForEach(rvm =>
                {
                    rvm.Keywords = keywordsPerLink.Where(keywords => keywords.LinkId == rvm.Id).ToList();
                });

                return resultVm;
            }
        }

        public Links GetLinkById(int id)
        {
            using (var ctx = new DbModel())
            {
                var link = new Links();
                link = ctx.Links.Single(l => l.ID == id);

                return link;
            }
        }
    }
}