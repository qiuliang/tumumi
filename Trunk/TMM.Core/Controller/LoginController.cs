using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using System.Text.RegularExpressions;
using TMM.Model;
using TMM.Core.Extends;
using TMM.Service;

namespace TMM.Core.Controller
{
    public class LoginController : BaseController
    {
        public void Default() {
            
        }

        public void Register() {
            PropertyBag["aa"] = 11;
        }

        [AccessibleThrough(Verb.Post)]
        public void DoRegister([DataBind("U_UserInfo")]U_UserInfo userInfo) {
            string vcode = Request["validate_code"];
            object s = Session["VerifyCode"];
            try
            {
                if (vcode.ToLower() == s.ToString().ToLower())
                {
                    UserService userService = Context.GetService<UserService>();
                    U_UserInfo existUser = userService.UserInfoBll.FindUserByEmail(userInfo.Email);
                    if (existUser != null) {
                        AddError("此Email已经注册");
                        RedirectToReferrer();
                        return;
                    }
                    userInfo.Password = userInfo.Password.ToMd5();
                    userInfo.RegTime = DateTime.Now;
                    userInfo.RegIp = Context.Request.UserHostAddress;
                    userInfo.UserId = userService.UserInfoBll.Insert(userInfo);
                    //写cookie
                    HttpCookie cookie = new HttpCookie("tmm");
                    cookie["UserId"] = userInfo.UserId.ToString();
                    cookie["Flag"] = Utils.TmmUtils.GenLoginFlagStr(userInfo);
                    cookie.Path = "/";                    
                    HttpContext.Response.Cookies.Add(cookie);

                    Session["logonUser"] = userInfo;
                    Redirect("/home/MyDefault.do");
                    return;
                }
                else
                {
                    AddError("验证码错误");                    
                }
            }
            catch(Exception ex)  {
                AddError("系统错误，请重试");
                Utils.Log4Net.Error(ex);
            }
            Flash["userInfo"] = userInfo;
            RedirectToReferrer();
        }

        [AccessibleThrough(Verb.Post)]
        public void DoLogin(string email,string password,string backUrl,bool isRemember)
        {
            email = email ?? string.Empty;
            password = password ?? string.Empty;
            password = password.ToMd5();
            try
            {
                UserService us = Context.GetService<UserService>();
                U_UserInfo logonUser = us.UserInfoBll.FindUserByLogin(email, password);
                if (logonUser != null) {
                    //写cookies
                    HttpCookie cookie = new HttpCookie("tmm");
                    cookie["UserId"] = logonUser.UserId.ToString();
                    cookie["Flag"] = Utils.TmmUtils.GenLoginFlagStr(logonUser);                    
                    cookie.Path = "/";
                    if (isRemember)
                    {
                        cookie.Expires = DateTime.Now + new TimeSpan(30, 0, 0, 0);  //生存期30天
                    }
                    HttpContext.Response.Cookies.Add(cookie);

                    Session["logonUser"] = logonUser;
                    if (!string.IsNullOrEmpty(backUrl.ToUrlDecode())) {
                        Redirect(backUrl);
                        return;
                    }
                    Redirect("/home/MyDefault.do");
                    return;
                }
                else
                {
                    AddError("登陆失败，请检查用户名或密码");
                }
            }
            catch {
                AddError("系统错误，请重试");
            }
            Flash["userInfo"] = new U_UserInfo() { Email=email};
            RedirectToReferrer();
        }

        public void LoginOut() 
        {
            HttpCookie ck = HttpContext.Request.Cookies["tmm"];
            if (ck != null) {
                ck.Path = "/";
                ck.Expires = DateTime.Now.AddDays(-10);
                HttpContext.Response.Cookies.Add(ck);
            }
            ClearCookie("__qc__k");
            ClearCookie("__qc_wId");
            Session["logonUser"] = null;
            Session.Clear();
            Redirect("/index.html");
            return;
        }

        private void ClearCookie(string ckName)
        {
            HttpCookie ck = HttpContext.Request.Cookies[ckName];
            if (ck != null) {
                ck.Path = "/";
                //ck.Domain = ".tumumi.com";
                ck.Expires = DateTime.Now.AddDays(-10);
                HttpContext.Response.Cookies.Add(ck);
            }
        }

        public void FindPwd()
        { 
        }
        public void DoFindPwd(string email)
        {
            UserService us = Context.GetService<UserService>();
            U_UserInfo findUser = us.UserInfoBll.FindUserByEmail(email);
            if (findUser == null) {
                AddError("此email不存在");
                RedirectToReferrer();
                return;
            }
            findUser.Password = ("123456").ToMd5();
            us.UserInfoBll.Update(findUser);
            //发送邮件
            Core.Utils.TmmUtils.SendEmail(email, "土木迷用户密码找回通知", 
                string.Format("<p>{0}，您好：</p><p>您的密码已经被重置为{1}，点此立即<a href='http://www.tumumi.com/login'>登录</a>土木迷。</p>",
                email,"123456"));
            Flash["SendEmailSuccess"] = "邮件发送成功";
            Flash["SendToEmail"] = email;
            RedirectToReferrer();
        }

        public void QQLogin()
        {
           
        }

       
        public void DoQQLogin(string nickName,string headImg)
        {
            nickName = Context.Server.UrlDecode(nickName);
            headImg = Context.Server.UrlDecode(headImg);
            var token = Request.ReadCookie("qq_openid");
            
            TMM.Core.Utils.Log4Net.Error(string.Format("获取openid：{0}",token));
            
            UserService userService = Context.GetService<UserService>();
            var existUser = userService.UserInfoBll.FindUserByOpenId(token);
            if (existUser == null)
            {
                var userInfo = new U_UserInfo();
                userInfo.NickName = nickName;
                userInfo.RegTime = DateTime.Now;
                userInfo.RegIp = Context.Request.UserHostAddress;
                userInfo.RegFrom = (int)TMM.Model.Enums.RegType.OAuthQQ;
                userInfo.OpenId = token;
                userInfo.Email = string.Format("{0}@qq.com",nickName);
                userInfo.Password = token;
                userInfo.HeadIcon = headImg;

                userInfo.UserId = userService.UserInfoBll.Insert(userInfo);
                
                TMM.Core.Utils.Log4Net.Error(string.Format("插入用户获取ID：{0}",userInfo.UserId));
                //写cookie
                HttpCookie cookie = new HttpCookie("tmm");
                cookie["UserId"] = userInfo.UserId.ToString();
                cookie["Flag"] = Utils.TmmUtils.GenLoginFlagStr(userInfo);
                cookie.Path = "/";
                HttpContext.Response.Cookies.Add(cookie);

                Session["logonUser"] = userInfo;
            }
            else { 
                HttpCookie cookie = new HttpCookie("tmm");
                cookie["UserId"] = existUser.UserId.ToString();
                cookie["Flag"] = Utils.TmmUtils.GenLoginFlagStr(existUser);
                cookie.Path = "/";
                HttpContext.Response.Cookies.Add(cookie);

                Session["logonUser"] = existUser;
            }
            Redirect("/home/mydefault.do");
        }

        public void RegFromOpenApi(string nickName,string headImg,int regFrom)
        {
            nickName = Context.Server.UrlDecode(nickName);
            UserService userService = Context.GetService<UserService>();
            var existUser = userService.UserInfoBll.FindUserByNickName(nickName);
            if (existUser == null) { 
                //自动注册
            }
        }
    }
}
