using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using TMM.Model;
using TMM.Core;

namespace TMM.Core.Controller
{
    [Helper(typeof(Helper.UserHelper))]
    [Helper(typeof(Helper.FormatHelper))]
    public class RewardController : BaseController
    {
        public void Index(int first) {
            Service.Bll.Doc.TReqDocBLL tbll = new TMM.Service.Bll.Doc.TReqDocBLL();
            Service.Bll.Doc.TJoinDocBLL tjbll = new TMM.Service.Bll.Doc.TJoinDocBLL();
            int rows = 10;
            int count = 0;
            IList<TReqDoc> list = tbll.GetList(first, rows, out count, null);
            Common.ListPage lp = new TMM.Core.Common.ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
            //排行榜
            PropertyBag["phb"] = tbll.GetList(0, 10);
            //最新中标
            PropertyBag["zxzb"] = tjbll.GetList(0, 10);
        }

        
    }
}
