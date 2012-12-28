using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using TMM.Core.Extends;
using TMM.Service;
using TMM.Model;
using TMM.Core.Helper;

namespace TMM.Core.Controller
{
   
    public class AboutController : BaseController
    {
        [Cache(HttpCacheability.Public)]
        public void AboutUs()
        { }
        [Cache(HttpCacheability.Public)]
        public void Faq()
        { }
        [Cache(HttpCacheability.Public)]
        public void Help()
        { }
        [Cache(HttpCacheability.Public)]
        public void Service()
        { }
        [Cache(HttpCacheability.Public)]
        public void Qqcl()
        { }
        [Cache(HttpCacheability.Public)]
        public void Mzsm()
        { }
        [Cache(HttpCacheability.Public)]
        public void Private()
        { }
        [Cache(HttpCacheability.Public)]
        public void Copyright()
        { }
        [Cache(HttpCacheability.Public,Duration=1)]
        public void FriendLink()
        {
            SystemService ss = Context.GetService<SystemService>();
            var list = ss.SFriendLinkBll.GetList();
            PropertyBag["List"] = list;
        }
        [Cache(HttpCacheability.Public)]
        public void Contact()
        { }

        public void Business()
        { }

        public void JoinUs()
        { }
        [Cache(HttpCacheability.Public, Duration = 60)]
        public void SiteMap()
        {
            SystemService sysService = Context.GetService<SystemService>();         

            PropertyBag["firstCatalog"] = sysService.SCatalogBll.GetTopNode();
        }

    }
}
