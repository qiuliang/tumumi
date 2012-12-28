using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Extends;
using TMM.Core.Common;

namespace TMM.Core.Controller.Admin
{
    [Layout("adminContent")]
    [Helper(typeof(Helper.FormatHelper))]
    public class ConfigController : BaseController
    {
        public void Default() {
            SystemService ss = Context.GetService<SystemService>();
            S_Config sc = ss.SConfigBll.Get();
            PropertyBag["model"] = sc;
        }
        public void DoConfig([DataBind("S_Config")] S_Config sc)
        {
            SystemService ss = Context.GetService<SystemService>();
            if (sc.Id == 0)
            {
                sc.Id = 1;
                ss.SConfigBll.Insert(sc);
            }
            else
            {
                ss.SConfigBll.Update(sc);
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }

        public void FriendLink() 
        {
            Service.Bll.Sys.S_FriendLinkBll bll = new TMM.Service.Bll.Sys.S_FriendLinkBll();
            IList<S_FriendLink> list = bll.GetList();
            PropertyBag["list"] = list;
        }
        public void EditFriendLink(int? fid)
        {
            Service.Bll.Sys.S_FriendLinkBll bll = new TMM.Service.Bll.Sys.S_FriendLinkBll();
            if (fid.HasValue) {
                S_FriendLink sl = bll.Get(fid.Value);
                PropertyBag["model"] = sl;
            }
        }
        public void DoAddFriendLink([DataBind("S_FriendLink")]S_FriendLink sl)
        {
            try
            {
                if (string.IsNullOrEmpty(sl.Title))
                    throw new TmmException("链接名称不能为空");
                if(string.IsNullOrEmpty(sl.LinkUrl))
                    throw new TmmException("链接地址不能为空");

                Service.Bll.Sys.S_FriendLinkBll bll = new TMM.Service.Bll.Sys.S_FriendLinkBll();

                if (sl.Fid != 0)
                {
                    bll.Update(sl);
                }
                else
                {
                    
                    bll.Insert(sl);
                }
                base.SuccessInfo();
            }
            catch (TmmException te) {
                AddError(te.Message);
            }
            RedirectToReferrer();
        }
        public void DeleteFriendLink(int fid) {
            Service.Bll.Sys.S_FriendLinkBll bll = new TMM.Service.Bll.Sys.S_FriendLinkBll();
            bll.Delete(fid);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        

        
    }
}
