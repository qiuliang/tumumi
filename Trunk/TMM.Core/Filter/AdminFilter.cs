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
    /// 管理后台过滤器
    /// </summary>
    public class AdminFilter : IFilter
    {
        public bool Perform(ExecuteEnum exec,IRailsEngineContext context,Castle.MonoRail.Framework.Controller controller) 
        {
            
            ManageUser mu = context.Session["adminUser"] as ManageUser;
            if (mu == null) {
                Hashtable p = new Hashtable();
                p.Add("backUrl",context.Request.Uri.AbsoluteUri);
                controller.Redirect("/admin",p);
                return false;
            }
            return true;
        }
    }
}
