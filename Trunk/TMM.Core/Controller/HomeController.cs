using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Helper;
using TMM.Core.Extends;

namespace TMM.Core.Controller
{
    [Helper(typeof(Helper.FormatHelper))]    
    public class HomeController : BaseController
    {
        [Cache(HttpCacheability.ServerAndPrivate,Duration=60)]
        public void Default() {
            SystemService sysService = Context.GetService<SystemService>();
            DocService ds = Context.GetService<DocService>();
            OrderService os = Context.GetService<OrderService>();

            PropertyBag["firstCatalog"] = sysService.SCatalogBll.GetTopNode();
            //大家在阅读什么
            IList<DDocInfo> hotList = ds.DDocInfoBll.GetHotList(0, 12);
            PropertyBag["hotList"] = hotList;
            //精选文档
            PropertyBag["recommendList"] = ds.DDocInfoBll.GetRecommendList(0, 10);
            //最新文档
            PropertyBag["newList"] = ds.DDocInfoBll.GetNewList(0, 10);
            //最新悬赏top 10
            PropertyBag["rewardList"] = ds.TReqDocBll.GetList(0, 10, "CreateTime DESC");
            //阅读top 10
            //同hotList
            
            //tags
            IList<D_Tag> tags = ds.DTagBll.GetList(null, "UseCount Desc", 0, 15); 
            var t = from tag in tags.ToList() orderby tag.TagId descending
                        select tag;
            if (t != null && t.Count()>0)
            {
                int maxUseCount = t.Max(tt => tt.UseCount);
                int minUseCount = t.Min(tt => tt.UseCount);
                t.ToList().ForEach(g =>
                {
                    if (g.UseCount == maxUseCount)
                    {
                        g.TagWeight = 1;
                    }
                    else if (g.UseCount == minUseCount)
                    {
                        g.TagWeight = 0;
                    }
                    else
                    {
                        g.TagWeight = Math.Round((decimal)g.UseCount / maxUseCount, 2);
                    }
                });
                PropertyBag["tags"] = t;
            }
            //新闻公告（3条系统公告）
            Service.Bll.User.M_MessageBLL msgBll = new TMM.Service.Bll.User.M_MessageBLL();
            IList<M_Message> mlist = msgBll.GetSysMessageList(0, 3);
            mlist.ToList().ForEach(m => m.Content=m.Content.ReplaceEnterStr());
            PropertyBag["messages"] = mlist;
            //最近兑换
            PropertyBag["exchangeList"] = os.TOrderBll.GetListForExchange(0, 6);


            
        }

        public void MyDefault()
        {
            if (!base.IsLogin)
                Redirect("/index.html");

            SystemService sysService = Context.GetService<SystemService>();
            DocService ds = Context.GetService<DocService>();
            OrderService os = Context.GetService<OrderService>();

            PropertyBag["firstCatalog"] = sysService.SCatalogBll.GetTopNode();
            //大家在阅读什么
            IList<DDocInfo> hotList = ds.DDocInfoBll.GetHotList(0, 12);
            PropertyBag["hotList"] = hotList;
            
            //最新文档
            PropertyBag["newList"] = ds.DDocInfoBll.GetNewList(0, 10);
            
            //阅读top 10
            //同hotList

            
            //新闻公告（3条系统公告）
            Service.Bll.User.M_MessageBLL msgBll = new TMM.Service.Bll.User.M_MessageBLL();
            IList<M_Message> mlist = msgBll.GetSysMessageList(0, 3);
            mlist.ToList().ForEach(m => m.Content = m.Content.ReplaceEnterStr());
            PropertyBag["messages"] = mlist;

            UserService us = Context.GetService<UserService>();
            U_UserInfo u = base.GetUser();
            //账户余额
            PropertyBag["accountBalance"] = us.MAccountBll.GetByUserId(u.UserId).Amount;
            //最近账单
            PropertyBag["billList"] = os.MAccountLogBll.GetList(u.UserId, 0, 3);
            //可能喜欢的文档
            //获取最近一条购买记录
            IList<MPurchase> purchaseList = us.MPurchaseBll.GetList(u.UserId, 0, 1);
            if (purchaseList == null || purchaseList.Count == 0)
            {
                PropertyBag["maybeLikeList"] = ds.DDocInfoBll.GetRecommendList(0, 12);
            }
            else
            {
                MPurchase mp = purchaseList[0];
                Core.Search.Index si = new TMM.Core.Search.Index();
                si.TableName = "vwDocInfo";
                ArrayList maybeLike = si.SearchMaybeLike(mp.Title, 0, 12, mp.DocId);
                if (maybeLike.Count < 12)
                {
                    maybeLike.AddRange(ds.DDocInfoBll.GetRecommendList(0, 12 - maybeLike.Count).ToArray());
                }
                PropertyBag["maybeLikeList"] = maybeLike;
            }
            RenderView("LogonDefault");
        }

        public void Test() {

            string a = Context.Server.MapPath("/");
            Response.Write(a);
            CancelLayout();
            CancelView();
        }

        public void HomePage(int userId) 
        { 
        }
    }
}
