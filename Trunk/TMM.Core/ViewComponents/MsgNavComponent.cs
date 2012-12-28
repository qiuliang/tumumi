using System;
using Castle.MonoRail.Framework;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using TMM.Service;
using TMM.Model;


namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// 我的土木迷-消息-头部导航模块
    /// </summary>
    public class MsgNavComponent : ViewComponent
    {

        private string showCurNavFlag;
        /// <summary>
        /// 控制当前导航菜单样式的flag
        /// </summary>
        [ViewComponentParam]
        public string ShowCurNavFlag
        {
            get { return showCurNavFlag; }
            set { showCurNavFlag = value; }
        }

        public override void Render()
        {
            PropertyBag[this.showCurNavFlag] = true;
            UserService us = RailsContext.GetService<UserService>();
            U_UserInfo u = Session["logonUser"] as U_UserInfo;
            Hashtable p = new Hashtable();
            p.Add("RecieverId",u.UserId);
            p.Add("Mtype",(int)Model.Enums.MessageType.Message);
            p.Add("IsRead",0);
            int unReadMsgCount = us.MessageBll.GetCount(p);
            PropertyBag["unReadMsgCount"] = unReadMsgCount;
            //
            p.Clear();
            p.Add("RecieverId",u.UserId);
            p.Add("Mtype",(int)Model.Enums.MessageType.Inform);
            p.Add("IsRead",0);
            p.Add("RecieveDeleteFlag",0);
            int unReadInformCount = us.MessageBll.GetCount(p);
            PropertyBag["unReadInformCount"] = unReadInformCount;
            base.Render();
        }
    }
}
