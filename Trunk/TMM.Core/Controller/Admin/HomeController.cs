using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Core.Extends;
using TMM.Model;

namespace TMM.Core.Controller.Admin
{
    public class HomeController : BaseController
    {
        [SkipFilter]
        public void Login() { 
        }

        [AccessibleThrough(Verb.Post)]
        [SkipFilter]
        public void DoLogin(string userName,string password,string backUrl) {
            try
            {
                AdminService ads = Context.GetService<AdminService>();
                
                if (ads.ManageUserBll.IsExist(userName, password.ToMd5()))
                {
                    Session["adminUser"] = ads.ManageUserBll.Get(userName);
                    if (string.IsNullOrEmpty(backUrl))
                    {
                        RedirectToAction("Index");
                    }
                    else
                        Redirect(backUrl);
                    return;
                }
                else {
                    AddError("用户名或密码不正确");
                }
            }
            catch(Exception ex) {
                AddError(ex.Message);
                throw ex;
            }
            RedirectToReferrer();
        }

        [Layout("adminContent")]
        public void Index() { 
            
        }

        public string TestSql(string tagId,Hashtable p)
        {
            CancelLayout();
            
            if (!string.IsNullOrEmpty(tagId))
            {
                IBatisNet.DataMapper.Configuration.Statements.IStatement statement =
                    TMM.Persistence.SqlMapper.Get().GetMappedStatement(tagId).Statement;
                IBatisNet.DataMapper.MappedStatements.IMappedStatement mappedStatement =
                    TMM.Persistence.SqlMapper.Instance().GetMappedStatement(tagId);
                IBatisNet.DataMapper.Scope.RequestScope rs =
                    statement.Sql.GetRequestScope(mappedStatement, p, new IBatisNet.DataMapper.SqlMapSession(TMM.Persistence.SqlMapper.Instance()));
                string a = rs.PreparedStatement.PreparedSql;
                return a;
            }
            return "no sql";
        }
        [SkipFilter]
        public void InitAdminUser()
        {
            CancelLayout();
            CancelView();
            AdminService ams = Context.GetService<AdminService>();
            ManageUser mu = ams.ManageUserBll.Get("admin");
            if (mu == null)
            {
                mu = new ManageUser()
                {
                    CreateTime = DateTime.Now,
                    UserName = "admin",
                    Password = ("123456").ToMd5(),
                    TrueName = "管理员",
                    Remark = "系统自动生成"
                };
                ams.ManageUserBll.Insert(mu);
                Response.Write("Success");
            }
            else
            {
                Response.Write("admin is isexist");
            }
            
        }

        [Layout("adminContent")]
        public void ChangePwd()
        { 
        }

        [Castle.MonoRail.Framework.AccessibleThrough(Verb.Post)]
        public void ChangePwd(string oldPwd,string newPwd,string newPwd2)
        {
            AdminService ads = Context.GetService<AdminService>();
            var admin = ads.ManageUserBll.Get("admin");
            if (admin == null)
                base.AddError("未查询到用户"); 
            if (oldPwd.ToMd5() != admin.Password)
                base.AddError("旧密码不正确");
            if (newPwd != newPwd2)
                base.AddError("两次输入的密码不一致");
            if (string.IsNullOrEmpty(newPwd))
                base.AddError("密码不能为空");

            if (ErrorCount == 0)
            {

                admin.Password = newPwd.ToMd5();
                admin.UpdateTime = DateTime.Now;
                ads.ManageUserBll.Update(admin);
                base.AddSuccess("密码修改成功");
            }
            RedirectToAction("ChangePwd");
        }
    }
}
