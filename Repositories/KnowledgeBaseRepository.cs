using KnowledgeBase.Database;
using KnowledgeBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KnowledgeBase.Repositories
{
    public class KnowledgeBaseRepository
    {
        public void AddLink(LinkEditViewModel linkVm)
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
            

                var linksKeywordsDb = new List<LinksKeywords>();
                var keywordsDb = new List<Keywords>();

                if (linkVm.SelectedKeywords != null && linkId != -1)
                {
                    linkVm.SelectedKeywords.ForEach(k =>
                    {
                        var intId = Convert.ToInt32(k);
                        var keywordDb = new Keywords();
                        keywordDb.Keyword = ctx.Keywords.Single(kwd => kwd.ID == intId).Keyword;
                        //keywordDb.Keyword = k.Text;
                        keywordsDb.Add(keywordDb);

                        var linkKeywordDb = new LinksKeywords();
                        linkKeywordDb.KeywordID = Convert.ToInt32(k);
                        linkKeywordDb.LinkID = linkId;
                        linksKeywordsDb.Add(linkKeywordDb);
                    });

                ctx.Keywords.AddRange(keywordsDb);
                ctx.LinksKeywords.AddRange(linksKeywordsDb);

                ctx.SaveChanges();
            }
            }
        }

        public void SaveLink(LinkEditViewModel linkVm)
        {
            using (var ctx = new DbModel())
            {
                var linkDb = ctx.Links.FirstOrDefault(l => l.ID == linkVm.Id);
                if (linkDb != null)
                {
                    linkDb.Title = linkVm.Title;
                    linkDb.Description = linkVm.Description;
                    linkDb.Url = linkVm.Url;
                }

                var linkKeywords = ctx.LinksKeywords.Where(lk => lk.LinkID == linkVm.Id).ToList();

                foreach (var sk in linkVm.SelectedKeywords)
                {
                    var skInt = Convert.ToInt32(sk);

                    var existingKeyword = linkKeywords.FirstOrDefault(lk => lk.KeywordID == skInt);
                    if (existingKeyword == null)
                    {
                        ctx.LinksKeywords.Add(new LinksKeywords { LinkID = linkVm.Id, KeywordID = skInt });
                    }
                }

                foreach (var linkKeyword in linkKeywords)
                {
                    var selectedKeyword = linkVm.SelectedKeywords.FirstOrDefault(sk => sk == linkKeyword.KeywordID.ToString());
                    if (selectedKeyword == null)
                    {
                        ctx.LinksKeywords.Remove(linkKeyword);
                    }
                }

                ctx.SaveChanges();
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

                var x = from k in ctx.Keywords
                        join lk in ctx.LinksKeywords on k.ID equals lk.KeywordID
                        where linkIds.Contains(lk.LinkID)
                        select new { KeywordID = k.ID, LinkID = lk.LinkID, Keyword = k.Keyword };

                var linksKeywords = new List<LinksKeywords>();
                var keywordsPerLink = new List<KeywordViewModel>();
                x.ToList().ForEach(y =>
                {
                    var linkKeyword = new LinksKeywords
                    {
                        LinkID = y.LinkID,
                        KeywordID = y.KeywordID
                    };
                    var keyword = new KeywordViewModel
                    {
                        Id = y.KeywordID,
                        Keyword = y.Keyword
                    };

                    linksKeywords.Add(linkKeyword);
                    keywordsPerLink.Add(keyword);
                });

                //var linksKeywords = from k in ctx.Keywords
                //                    join lk in ctx.LinksKeywords on k.ID equals lk.KeywordID
                //                    where linkIds.Contains(lk.LinkID)
                //                    select new LinksKeywords { KeywordID = k.ID, LinkID = lk.LinkID };

                //var keywordsPerLink = from k in ctx.Keywords
                //                      join lk in ctx.LinksKeywords on k.ID equals lk.KeywordID
                //                      where linkIds.Contains(lk.LinkID)
                //                      select new KeywordViewModel { Id = k.ID, Keyword = k.Keyword };


                resultVm.ForEach(rvm =>
                {
                    var lk = linksKeywords.Where(linksKey => linksKey.LinkID == rvm.Id);
                    if (lk != null && lk.Any())
                    {
                        var lkIds = lk.Select(xy => xy.KeywordID);
                        rvm.Keywords = keywordsPerLink.Where(k => lkIds.Contains(k.Id))
                                                        .Select(k => new KeywordViewModel { Id = k.Id, Keyword = k.Keyword})
                                                        .ToList();
                    }
                    else
                    {
                        rvm.Keywords = new List<KeywordViewModel>();
                    }
                });

                //resultVm.ForEach(rvm =>
                //{
                //    rvm.Keywords = keywordsPerLink.Where(keywords => keywords.LinkId == rvm.Id).ToList();
                //});

                return resultVm;
            }
        }

        public LinkEditViewModel GetLinkById(int id)
        {
            using (var ctx = new DbModel())
            {
                var linkDb = ctx.Links.Single(l => l.ID == id);

                var linkEditVm = new LinkEditViewModel
                {
                    Id = linkDb.ID,
                    Title = linkDb.Title,
                    Description = linkDb.Description,
                    Url = linkDb.Url
                };

                var keywordsDb = from k in ctx.Keywords
                                 join lk in ctx.LinksKeywords on k.ID equals lk.KeywordID
                                 where lk.LinkID == id
                                 select k.ID;

                linkEditVm.Keywords = GetTagsList();

                linkEditVm.SelectedKeywords = new List<string>();
                foreach (var keyword in keywordsDb)
                {
                    linkEditVm.SelectedKeywords.Add(keyword.ToString());
                }
                
                return linkEditVm;
            }
        }

        public List<SelectListItem> GetTagsList()
        {
            var tags = new List<SelectListItem>();

            using (var ctx = new DbModel())
            {
                tags.AddRange(ctx.Keywords.Select(k => new SelectListItem { Value = k.ID.ToString(), Text = k.Keyword }).ToList());
            }

            return tags;
        }
    }
}