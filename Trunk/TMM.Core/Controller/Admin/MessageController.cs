using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Extends;
using TMM.Core.Helper;

namespace TMM.Core.Controller.Admin
{
    [Layout("adminContent")]
    [Helper(typeof(FormatHelper))]
    [Helper(typeof(UserHelper))]
    public class MessageController : BaseController
    {
        public void System(int first,string likeTitle) {
            AdminService ams = Context.GetService<AdminService>();
            IList<M_Message> mlist = ams.MessageBll.GetSysMessageList(first, 10);
            mlist.ToList().ForEach(m => m.Content = m.Content.ReplaceEnterStr());
            Hashtable p = new Hashtable();
            p.Add("Mtype",(int)Model.Enums.MessageType.SysMsg);
            if (!string.IsNullOrEmpty(likeTitle)) {
                p.Add("LikeTitle",likeTitle);
            }
            int count = ams.MessageBll.GetCount(p);
            Common.ListPage lp = new TMM.Core.Common.ListPage((IList)mlist, first, 10, count);
            PropertyBag["listPage"] = lp;
        }

        public void AddSysMsg() { 
            
        }

        public void DoAddSysMsg([DataBind("M_Message")]M_Message msg) {
            try
            {
                if (string.IsNullOrEmpty(msg.Title)) {
                    throw new Exception("标题不能为空");
                }
                if (string.IsNullOrEmpty(msg.Content))
                {
                    throw new Exception("内容不能为空");
                }
                AdminService ams = Context.GetService<AdminService>();
                if (msg.Mid != 0)
                {
                    M_Message oldMsg = ams.MessageBll.Get(msg.Mid);
                    if (oldMsg != null) {
                        msg.CreateTime = oldMsg.CreateTime;
                        msg.Mtype = oldMsg.Mtype;
                        ams.MessageBll.Update(msg);
                    }
                }
                else
                {
                    msg.CreateTime = DateTime.Now;
                    msg.Mtype = (int)Model.Enums.MessageType.SysMsg;
                    ams.MessageBll.Insert(msg);
                }
                base.AddSuccess("操作成功");                
            }
            catch (Exception ex)
            {
                base.AddError(ex.Message);
                Flash["model"] = msg;
                
            }
            
            RedirectToReferrer();
        }

        public void DeleteMsg(int[] mids) 
        {
            CancelLayout();
            CancelView();
            try
            {
                AdminService ams = Context.GetService<AdminService>();
                mids.ToList().ForEach(mid => ams.MessageBll.Delete(mid));
                //ams.MessageBll.Delete(mid);
                RenderText("1");
            }
            catch
            {
                RenderText("-1");
            }
        }

        public void EditMsg(int mid) 
        {
            AdminService ams = Context.GetService<AdminService>();
            M_Message model = ams.MessageBll.Get(mid);
            PropertyBag["model"] = model;
            RenderView("AddSysMsg");
        }
        /// <summary>
        /// 用户咨询
        /// </summary>
        public void Question(int first)
        {
            int rows = 20;
            AdminService ams = Context.GetService<AdminService>();
            int count = 0;
            IList<M_Message> mlist = ams.MessageBll.GetUserMsgList(first, rows, out count, Helper.ConfigHelper.AdminUserId);
            
            Common.ListPage lp = new TMM.Core.Common.ListPage((IList)mlist, first, rows, count);
            PropertyBag["listPage"] = lp;
        }
        /// <summary>
        /// 回复用户咨询
        /// </summary>
        public void ReplyQuestion(int mid)
        {
            AdminService ams = Context.GetService<AdminService>();
            M_Message model = ams.MessageBll.Get(mid);
            PropertyBag["model"] = model;
        }
        public void DoReplyQuestion([DataBind("M_Message")]M_Message msg)
        {
            try
            {
                if (string.IsNullOrEmpty(msg.Title))
                {
                    throw new Exception("标题不能为空");
                }
                if (string.IsNullOrEmpty(msg.Content))
                {
                    throw new Exception("内容不能为空");
                }
                AdminService ams = Context.GetService<AdminService>();
                msg.SenderId = ConfigHelper.AdminUserId;
                msg.CreateTime = DateTime.Now;
                msg.Mtype = (int)Model.Enums.MessageType.Message;
                msg.IsRead = false;
                ams.MessageBll.Insert(msg);
                base.AddSuccess("操作成功");
            }
            catch (Exception ex)
            {
                base.AddError(ex.Message);
                Flash["model"] = msg;
            }
            RedirectToReferrer();
        }

        public void SendMsg(string uids,int mtype) 
        { 
            UserService us = Context.GetService<UserService>();
            
            string[] ids = uids.Split(',');
            Hashtable p = new Hashtable();
            p.Add("UserIds",ids);
            IList<U_UserInfo> ulist = us.UserInfoBll.GetList(p, null, 0, 100);
            PropertyBag["recieverList"] = ulist;
        }
        public void DoSendMsg(string uids, int mtype, string title, string content)
        {
            UserService us = Context.GetService<UserService>();
            string[] ids = uids.Split(',');
            foreach (string s in ids)
            {
                M_Message msg = new M_Message() { 
                    Content = content,
                    CreateTime = DateTime.Now,
                    IsRead = false,
                    Mtype = mtype,
                    RecieverId = int.Parse(s),
                    SenderId = ConfigHelper.AdminUserId,
                    Title = title
                };
                us.MessageBll.Insert(msg);
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }
        /// <summary>
        /// 普通消息列表
        /// </summary>
        /// <param name="direct">1:发送 2:接收</param>
        public void UserMsg(int first,int direct,string email,int userId)
        {
            //AdminService ams = Context.GetService<AdminService>();
            UserService us = Context.GetService<UserService>();
            int rows = 20;
            int count = 0;
            Hashtable p = new Hashtable();
            if (userId == 0 && !string.IsNullOrEmpty(email)) {
                U_UserInfo u = us.UserInfoBll.FindUserByEmail(email);
                if (u != null)
                    userId = u.UserId;
            }
            if (direct == 1 && userId != 0)
                p.Add("SenderId",userId);
            if (direct == 2 && userId != 0)
                p.Add("RecieverId",userId);

            p.Add("Mtype",(int)Model.Enums.MessageType.Message);

            count = us.MessageBll.GetCount(p);
            IList<M_Message> mlist = us.MessageBll.GetList(p, null, first, rows);
            //mlist.ToList().ForEach(m => m.Content = m.Content.ReplaceEnterStr());
            Common.ListPage lp = new TMM.Core.Common.ListPage((IList)mlist, first, rows, count);
            PropertyBag["listPage"] = lp;
        }
    }
}
