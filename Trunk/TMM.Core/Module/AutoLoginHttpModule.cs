using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using TMM.Model;

namespace TMM.Core.Module
{
    public class AutoLoginHttpModule : IHttpModule,IRequiresSessionState
    {
        public void Dispose() { 
        }
        public void Init(HttpApplication httpApplication) {
            httpApplication.PreRequestHandlerExecute += new EventHandler(httpApplication_PreRequestHandlerExecute);
        
        }

        void httpApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext ctx = application.Context;
            if (!IsMonoRailRequest(ctx))    //如果不是monorail请求，直接返回
                return;
            if (ctx.Session == null)
                return;
            HttpCookie cookie = ctx.Request.Cookies["tmm"];
            if (cookie != null)
            {
                int userId = Int32.Parse(cookie["UserId"]);
                string flag = cookie["Flag"];
                if (ctx.Session["logonUser"] != null)
                {
                    return;
                }
                else {
                    try
                    {
                        U_UserInfo _logonUser = Utils.TmmUtils.CheckLoginCookie(userId, flag);                      

                        ctx.Session["logonUser"] = _logonUser;
                    }
                    catch (Exception ex)
                    {
                        Utils.Log4Net.Error("自动登录失败");
                        Utils.Log4Net.Error(ex);
                        ctx.Response.Cookies.Remove("tmm");
                    }
                }
            }
            else {
                U_UserInfo _logonUser = ctx.Session["logonUser"] as U_UserInfo;
                //if (_logonUser != null)
                //{
                //    ctx.Session.Remove("logonUser");
                //    ctx.Session.Clear();
                //}
            }
        }

        private bool IsMonoRailRequest(HttpContext context)
        {
            return context.Items.Contains("is.mr.request");
        }
    }
}
