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
    public class OrderController : BaseController
    {
        public void Index(int first, int? orderType, string email, int[] orderStatus, bool from)
        {
            OrderService os = Context.GetService<OrderService>();
            int count = 0;
            int rows = 20;
            Hashtable p = new Hashtable();
            if (orderType.HasValue) {
                p.Add("OrderType",orderType.Value);
            }
            if (!string.IsNullOrEmpty(email))
                p.Add("Email",email);
            if (orderStatus.Length > 0)
                p.Add("Statuses",orderStatus);
            count = os.TOrderBll.GetCount(p);
            IList<TOrder> list = os.TOrderBll.GetList(p, null, first, rows);
            ListPage lp = new ListPage((IList)list,first,rows,count);
            PropertyBag["lp"] = lp;

            if (from)
                PropertyBag["isExchange"] = true;   //兑换
            else
                PropertyBag["isExchange"] = false;

            PropertyBag["orderStatus"] = orderStatus;
        }
        public void Detail(decimal orderId) {
            OrderService os = Context.GetService<OrderService>();
            TOrder o = os.TOrderBll.GetOrderAndDetail(orderId);
            PropertyBag["order"] = o;

            Hashtable p = new Hashtable();

            //支付日志
            p.Add("OrderId",orderId);
            IList<MPayLog> plist = os.MPayLogBll.GetList(p, null, 0, 10000);
            PropertyBag["paylist"] = plist;
        }

        public void Exchange(int first, string email, int[] orderStatus)
        {            
            Index(first, (int)OrderType.Exchange, email, orderStatus,true);
            RenderView("index");
        }
        public void ConfirmExchange(decimal orderId)
        {
            OrderService os = Context.GetService<OrderService>();
            TOrder o = os.TOrderBll.Get(orderId);
            o.Status = (int)OrderStatus.ConfirmExchange;
            o.UpdateTime = DateTime.Now;
            os.TOrderBll.Update(o);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void DoExchange(decimal orderId)
        {
            OrderService os = Context.GetService<OrderService>();
            TOrder o = os.TOrderBll.Get(orderId);
            o.Status = (int)OrderStatus.AdminDoExchange;
            o.UpdateTime = DateTime.Now;
            os.TOrderBll.Update(o);
            //写账户日志
            AccountLog accLog = new AccountLog() { 
                AccountWay = (int)AmountWay.EOut,
                Amount = -o.Total,
                CreateTime = DateTime.Now,
                Ip = Utils.TmmUtils.IPAddress(),
                OrderId = o.OrderId,
                UserId = o.UserId,
                PayWay = 0,
                AdminRemark = "兑换"
            };
            os.MAccountLogBll.Insert(accLog);
            //更新账户信息
            MAccount acc = os.MAccountBll.GetByUserId(o.UserId);
            acc.FrozenAmount -= o.Total;
            acc.TotalExchange += o.Total;
            acc.UpdateTime = DateTime.Now;
            os.MAccountBll.Update(acc);
            //发系统消息
            M_Message msg = new M_Message() { 
                SenderId = Helper.ConfigHelper.AdminUserId,
                RecieverId = o.UserId,
                CreateTime = DateTime.Now,
                IsRead = false,
                Mtype = (int)Model.Enums.MessageType.Inform,
                Title = "您的兑换申请已经被受理",
                Content = string.Format("您的兑换申请已被管理员受理，请注意查收您的{0}账号",o.Remark == "1" ? "支付宝" : "银行")
            };
            Service.Bll.User.M_MessageBLL msgBll = new TMM.Service.Bll.User.M_MessageBLL();
            msgBll.Insert(msg);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void ExchangeDetail(decimal orderId)
        {
            OrderService os = Context.GetService<OrderService>();
            TOrder o = os.TOrderBll.GetOrderAndDetail(orderId);
            PropertyBag["order"] = o;
        }

        public void Delete(decimal orderId)
        {
            OrderService os = Context.GetService<OrderService>();
            os.TOrderBll.Delete(orderId);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        
    }
}
