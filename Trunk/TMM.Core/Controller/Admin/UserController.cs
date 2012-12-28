using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Extends;
using TMM.Core.Common; 

namespace TMM.Core.Controller.Admin
{
    [Layout("adminContent")]
    public class UserController : BaseController
    {
        public void Index(int first,int rows,string email,int? status) {
            if (rows == 0)
                rows = 20;
            UserService us = Context.GetService<UserService>();
            Hashtable p = new Hashtable();
            if (!string.IsNullOrEmpty(email))
                p.Add("Email",email);
            if (status.HasValue)
                p.Add("IsStop",status.Value);
            IList<U_UserInfo> ulist = us.UserInfoBll.GetList(p, "RegTime DESC", first, rows);
            int count = us.UserInfoBll.GetCount(p);
            ListPage lp = new ListPage((IList)ulist, first, rows, count);
            PropertyBag["userList"] = lp;
        }

        public void ResetPwd(int userId) {
            CancelLayout();
            CancelView();
            try
            {
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = us.UserInfoBll.Get(userId);
                if (u != null)
                {
                    u.Password = ("123456").ToMd5();
                    us.UserInfoBll.Update(u);
                    RenderText("1");
                }
            }
            catch {
                RenderText("-1");
            }
        }

        public void UpdateStatus(int userId,bool status) {
            CancelLayout();
            CancelView();
            try
            {
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = us.UserInfoBll.Get(userId);
                if (u != null)
                {
                    u.IsStop = status;
                    us.UserInfoBll.Update(u);
                    RenderText("1");
                }
            }
            catch
            {
                RenderText("-1");
            }
        }

        public void DoMjUsers(int[] userIds,bool isCancel)
        {
            UserService us = Context.GetService<UserService>();
            foreach (int uid in userIds)
            {
                us.UserInfoBll.UpdateMj(uid, !isCancel);
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }

        #region 账户管理
        public void Account(int first,string email) 
        {
            UserService us = Context.GetService<UserService>();
            Hashtable p = new Hashtable();
            if (!string.IsNullOrEmpty(email)) {
                p.Add("Email",email);
            }
            int rows = 20;
            int count = 0;
            count = us.MAccountBll.GetCount(p);
            IList<MAccount> list = us.MAccountBll.GetList(p, null, first, rows);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        public void AccountLog(int first, int userId) 
        {
            UserService us = Context.GetService<UserService>();
            Hashtable p = new Hashtable();
            int rows = 20;
            int count = 0;
            IList<AccountLog> list = us.AccountLogBll.GetList(userId, null, null, null, first, rows, out count);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        public void ManualOperate(int userId)
        {
            UserService us = Context.GetService<UserService>();
            PropertyBag["account"] = us.MAccountBll.GetByUserId(userId);
        }
        public void DoManualOperate(int userId, int accountWay, decimal amount, string remark)
        {
            try
            {
                UserService us = Context.GetService<UserService>();
                if (amount == 0)
                    throw new TmmException("金额不能为0");
                if (accountWay == 0)
                    throw new TmmException("请选择类型");
                if (accountWay == (int)AmountWay.MIn) {
                    if (amount <= 0)
                        throw new TmmException("增加金额不能小于0");
                }
                if (accountWay == (int)AmountWay.AOut)
                {
                    if (amount >= 0)
                        throw new TmmException("扣除金额不能大于0");
                }
                if (string.IsNullOrEmpty(remark))
                    throw new TmmException("备注不能为空");
                MAccount acc = us.MAccountBll.GetByUserId(userId);
                //写账户日志
                AccountLog log = new AccountLog() { 
                    AccountWay = accountWay,
                    AdminRemark = remark,
                    Amount = amount,
                    CreateTime = DateTime.Now,
                    Ip = Utils.TmmUtils.IPAddress(),
                    UserId = userId
                    
                };
                us.AccountLogBll.Insert(log);

                acc.Amount += amount;
                us.MAccountBll.Update(acc);
                //发消息-异步
                M_Message msg = new M_Message()
                {
                    Content = string.Format("【帐户通知】，您的账户由土木迷管理员{0}￥{1:F2}",
                    amount > 0 ? "充入" : "扣除", amount),
                    CreateTime = DateTime.Now,
                    IsRead = false,
                    Mtype = (int)Model.Enums.MessageType.Inform,
                    RecieverId = userId,
                    SenderId = Core.Helper.ConfigHelper.AdminUserId,
                    Title = "帐户通知"
                };
                Queue<M_Message> queueMsg = new Queue<M_Message>();
                queueMsg.Enqueue(msg);
                Common.AsynMessage am = new AsynMessage(queueMsg);
                am.Send();

                base.SuccessInfo();
                Redirect("account.do");
                return;
            }
            catch (TmmException te)
            {
                AddError(te.Message);
                Flash["accountWay"] = accountWay;
                Flash["amount"] = amount;
                Flash["remark"] = remark;
            }
            RedirectToReferrer();
        }
        public void DeleteAccount(int accId)
        {
            UserService us = Context.GetService<UserService>();
            MAccount acc = us.MAccountBll.Get(accId);
            if (acc.Amount == 0)
            {
                us.MAccountBll.Delete(accId);
                base.SuccessInfo();
            }
            else
            {
                AddError("账户余额不为0，不能删除");
            }
            RedirectToReferrer();

        }
        #endregion
    }
}
