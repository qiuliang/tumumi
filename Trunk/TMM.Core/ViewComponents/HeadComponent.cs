using System;
using Castle.MonoRail.Framework;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using TMM.Model;
using TMM.Service;


namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// 主头部导航模块
    /// </summary>
    public class HeadComponent : ViewComponent
    {

        private U_UserInfo userInfo;
        /// <summary>
        /// 是否登录
        /// </summary>
        [ViewComponentParam]
        public U_UserInfo UserInfo {
            get { return userInfo; }
            set { userInfo = value; }
        }

        private int headType = 1;
        /// <summary>
        /// 输出头部的类型 1 为整站通用头部 2 文档浏览页头部
        /// </summary>
        [ViewComponentParam]
        public int HeadType
        {
            get { return headType; }
            set { headType = value; }
        }

        public override void Render()
        {
            PropertyBag["userInfo"] = this.userInfo;
            if (this.userInfo != null)
            {
                UserService us = RailsContext.GetService<UserService>();
                U_UserInfo u = Session["logonUser"] as U_UserInfo;
                PropertyBag["newMsgCount"] = us.MessageBll.GetNewMsgCount(u.UserId);
                //账户余额
                PropertyBag["accountBalance"] = us.MAccountBll.GetByUserId(u.UserId).Amount;
            }

            if (this.headType == 1)
            {
                base.Render();
            }
            else {
                RenderView("docHead");
            }
        }
    }
}
