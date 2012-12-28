using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using TMM.Core.Helper;
using TMM.Core.Common;
using TMM.Service;
using TMM.Model;

namespace TMM.Core.Controller
{
    [Helper(typeof(UserHelper))]
    [Helper(typeof(FormatHelper))]
    public class SearchController : BaseController
    {
        public void Index(string key,int? first,string docType) 
        {
            if (!first.HasValue)
                first = 0;
            int rows = 10;
            int count = 0;
            DocService ds = Context.GetService<DocService>();
            if(!string.IsNullOrEmpty(key))
            {
                //IList<DDocInfo> list = ds.DDocInfoBll.GetList(key, first.Value, rows, out count);
                Core.Search.Index sIndexer = new TMM.Core.Search.Index();
                sIndexer.TableName = "vwDocInfo";
                sIndexer.DocType = docType;
                long timeCost = 0;
                IList list = sIndexer.Search(key, first.Value, rows, out count, out timeCost);
                ListPage lp = new ListPage((IList)list,first.Value,rows,count);
                PropertyBag["lp"] = lp;
                PropertyBag["seconds"] = string.Format("{0:F3}", decimal.Parse(timeCost.ToString()) / 1000);
            }
            
            
        }
    }
}
