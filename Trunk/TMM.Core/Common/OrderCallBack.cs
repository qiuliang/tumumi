using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TMM.Service;
using TMM.Model;

namespace TMM.Core.Common
{
    /// <summary>
    /// 支付接口回调类
    /// </summary>
    public class OrderCallBack
    {
        public int UserId { get; set; }
        public decimal OrderId { get; set; }
        public int Status { get; set; }
        public string PayDetail { get; set; }
        public int PayWay { get; set; }
        public string GotoUrl { get; set; }        

      
        public OrderCallBack() { }
        /// <summary>
        /// 支付成功的后续事件
        /// </summary>
        public void ExecAfterPaid() 
        {
            OrderService os = new OrderService();
            os.Initialize();

            U_UserInfo u = os.UserInfoBll.Get(this.UserId);
            TOrder o = os.TOrderBll.GetOrderAndDetail(this.OrderId);           
            //检测订单状态
            CheckOrder(o, new OrderStatus[] { OrderStatus.NewOrder }, u);
            //更新订单状态
            os.TOrderBll.UpdateState2Paid(o.OrderId, this.Status, this.PayWay, this.PayDetail);

            //加入到我的购买
            DDocInfo buyDoc = os.DDocInfoBll.Get(o.OrderDetails[0].DocId);
            if (buyDoc != null)
            {
                MPurchase mp = new MPurchase()
                {
                    DocId = buyDoc.DocId,
                    Price = buyDoc.Price,
                    PurchaseTime = DateTime.Now,
                    UserId = this.UserId,
                    Title = buyDoc.Title,
                    Saler = buyDoc.UserId,
                    DocType = buyDoc.DocType
                };
                os.MPurchaseBll.Insert(mp);
                //给上传人返利
                AddAmountForUploader(o.OrderId, buyDoc.UserId, o.Total, os, buyDoc.DocId, buyDoc.Title, this.UserId, this.PayWay);
            }
            if (!string.IsNullOrEmpty(this.GotoUrl)) {
                HttpContext.Current.Response.Redirect(GotoUrl);
            }
        }

        protected void CheckOrder(TOrder m, OrderStatus[] status,U_UserInfo u)
        {
            if (m == null)
                throw new Exception("无效的订单");

            bool checkStatus = false;
            foreach (OrderStatus os in status)
            {
                if (m.Status == (int)os)
                {
                    checkStatus = true;
                    break;
                }
            }
            if (!checkStatus)
                throw new Exception("错误的订单状态");
            
            if (m.UserId != u.UserId)
                throw new Exception("请从正确的地址访问");
        }

        /// <summary>
        /// 支付成功后给上传人返利
        /// </summary>
        protected void AddAmountForUploader(
            decimal orderId,int uploaderId,decimal amount,
            OrderService os,int docId,string docTitle,int buyerId,int payway)
        {
            //先写入账户日志
            AccountLog al = new AccountLog() { 
                OrderId = orderId,
                UserId = uploaderId,
                Amount = amount,
                AccountWay = (int)AmountWay.INCOME,
                Ip = Utils.TmmUtils.IPAddress(),
                PayWay = payway
            };
            int a = os.MAccountLogBll.Insert(al);
            //账户增加收入
            if (a > 0) {
                os.MAccountBll.AddAmount(uploaderId, amount);
            }
            //发送系统通知
            string tmp = "您的文档<a href='/p-{0}.html' target='_blank'>{1}</a>"
                + "被<a href='/home/{2}.html' target='_blank'>{3}</a>下载，获得收入￥{4}";
            M_Message msg = new M_Message() { 
                SenderId = Helper.ConfigHelper.AdminUserId,
                Title = "恭喜您获得文档销售收入",
                CreateTime = DateTime.Now,
                Content = string.Format(tmp,docId,docTitle,buyerId,Helper.UserHelper.GetUserById(buyerId).TmmDispName,amount),
                IsRead = false,
                Mtype = (int)Model.Enums.MessageType.Inform,
                RecieverId = uploaderId
            };
            Service.Bll.User.M_MessageBLL mbll = new TMM.Service.Bll.User.M_MessageBLL();
            mbll.Insert(msg);
        }
        
    }
}
