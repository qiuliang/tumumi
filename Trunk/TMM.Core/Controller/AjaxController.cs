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
    /// <summary>
    /// 不带权限的ajax方法
    /// </summary> 
    [Helper(typeof(UserHelper))]
    public class AjaxController : BaseController
    {
        [AjaxAction]
        public void CheckEmail(string email) {
            email = email.ToUrlDecode();
            UserService userService = Context.GetService<UserService>();
            U_UserInfo u = userService.UserInfoBll.FindUserByEmail(email);
            if (u != null) {
                RenderText("0");
            }
            RenderText("1");
        }
        
        public void CommentList(int docId) {
            CancelLayout();
            DocService ds = Context.GetService<DocService>();
            IList<D_Comment> list = ds.DCommentBll.GetList(docId);
            list.ToList().ForEach(dc => dc.Content = dc.Content.ReplaceEnterStr());
            PropertyBag["list"] = list;
        }
        [AjaxAction]
        public void InsertComment([DataBind("D_Comment")]D_Comment model) 
        {
            try
            {
                DocService ds = Context.GetService<DocService>();
                model.Content = model.Content.ToUrlDecode().FilterHtml();
                model.CreateTime = DateTime.Now;
                model.IsDisp = true;
                ds.DCommentBll.Insert(model);
                //更新评论量
                ds.DDocInfoBll.UpdateCommentCount(model.DocId);
                RenderText("1");
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex);
                RenderText("-1");
            }

        }
        [AjaxAction]
        public void AddUp(int docId) {
            try
            {
                DocService ds = Context.GetService<DocService>();
                ds.DDocInfoBll.UpdateUpCount(docId);
                RenderText("1");
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex);
                RenderText("-1");
            }
        }
        [AjaxAction]
        public void AddFav(int docId) {
            try {
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                if (u == null)
                {
                    RenderText("0");
                }
                else {
                    IList<MFavorite> fList = us.MFavoriteBll.GetList(docId);
                    if (fList == null || fList.Count == 0)
                    {
                        MFavorite fav = new MFavorite()
                        {
                            UserId = u.UserId,
                            DocId = docId,
                            CreateTime = DateTime.Now,
                            FavCateId = 0
                        };
                        us.MFavoriteBll.Insert(fav);
                        us.DocInfoBll.UpdateFavCount(docId);
                        RenderText("1");
                    }
                    else {
                        RenderText("2");
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error(ex);
                RenderText("-1");
            }
        }

        /// <summary>
        /// 检测下载
        /// </summary>
        [AjaxAction]
        public void CheckDown(int docId) 
        {
            try
            {
                U_UserInfo logonUser = base.GetUser();
                //是否登录
                if (logonUser == null) {
                    RenderText("{code:0,msg:''}");
                    return;
                }
                DocService ds = Context.GetService<DocService>();
                AccountService accs = Context.GetService<AccountService>();
                //
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                if (doc == null)
                {
                    RenderText("{code:1,msg:''}");
                    return;
                }
                else {
                    if (doc.UserId == logonUser.UserId) //是否自己的文档
                    {
                        //输出下载URL
                        RenderText("{code:3,msg:'" + Utils.TmmUtils.GetDocDownValKey(logonUser.UserId,docId,doc.FileId.Value.ToString()) + "'}");
                        return;
                    }
                    else { 
                        //是否已经购买
                        bool isPurchase = accs.MPurchaseBll.IsPurchase(logonUser.UserId, docId);
                        if (isPurchase)
                        {
                            //输出下载URL
                            RenderText("{code:3,msg:'" + Utils.TmmUtils.GetDocDownValKey(logonUser.UserId, docId, doc.FileId.Value.ToString()) + "'}");
                            return;
                        }
                        else { 
                            //判断文档价格
                            if (doc.Price <= 0)
                            {
                                int todayDownCount = ds.DownloadLogBll.GetDownCountToday(logonUser.UserId);
                                if (todayDownCount >= ConfigHelper.FreeDownCount)
                                {
                                    //超过免费下载文档次数
                                    RenderText("{code:4,msg:''}");
                                }
                                else
                                {
                                    //输出下载URL
                                    RenderText("{code:3,msg:'" + Utils.TmmUtils.GetDocDownValKey(logonUser.UserId, docId, doc.FileId.Value.ToString()) + "'}");

                                }
                                return;
                            }
                            else {                                 
                                //输出消息
                                RenderText("{code:2,msg:'" + string.Format("{0},{1}",docId,doc.Price) + "'}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex);
                RenderText("{code:-1,msg:''}");
            }
        }

        [AjaxAction]        
        public void CateTree()
        {
            CancelLayout();
            SystemService ss = Context.GetService<SystemService>();
            IList<S_Catalog> tree = ss.SCatalogBll.GetTopNode();
            PropertyBag["treeList"] = tree;
        }

    }
}
