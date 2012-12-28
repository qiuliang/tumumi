using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework;
using TMM.Model;

namespace TMM.Core.Filter
{
    /// <summary>
    /// 我的土木迷过滤器
    /// </summary>
    public class MyTmmFilter : IFilter
    {
        public bool Perform(ExecuteEnum exec,IRailsEngineContext context,Castle.MonoRail.Framework.Controller controller) 
        {
            U_UserInfo u = context.Session["logonUser"] as U_UserInfo;
            if (u == null) {
                Hashtable p = new Hashtable();
                p.Add("backUrl",context.Request.Uri.AbsoluteUri);
                controller.Redirect("/login",p);
                return false;
            }
            return true;
        }
    }
}
