using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Helper;
using TMM.Core.Extends;
using TMM.Core.Pay;

namespace TMM.Core.Controller
{    
    [Filter(ExecuteEnum.BeforeAction, typeof(Filter.MyTmmFilter))]
    [Helper(typeof(FormatHelper))]
    public class PayController : BaseController
    {
        /// <summary>
        /// 转支付接口
        /// </summary>
        /// <param name="PayWay">alipay，tenpay，netbankpay</param>
        /// <param name="total"></param>
        /// <param name="pname">商品名称</param>
        /// <param name="docId"></param>
        /// <param name="chargeType">充值类型 1：下载产生订单进行充值 0：直接充值</param>
        [AccessibleThrough(Verb.Post)]        
        public void GotoPay(string PayWay, decimal total, string pname, int docId,int chargeType) 
        {
            OrderService os = Context.GetService<OrderService>();
            AccountService accs = Context.GetService<AccountService>();
            U_UserInfo logonUser = base.GetUser();
            decimal orderId = Utils.TmmUtils.GenOrderId();

            DDocInfo doc = new DDocInfo();
            TOrder order = new TOrder();
            TOrderDetail detail = new TOrderDetail();

            int orderType = 0;  //订单类型

            if (chargeType == 1)
            {
                doc = os.DDocInfoBll.Get(docId);
                orderType = (int)OrderType.DownDocOrder;
            }
            else if (chargeType == 0) { 
                //如果是直接充值，这里虚拟一个doc对象，来作为订单的商品
                doc = new DDocInfo() { 
                    DocId = -1,
                    Title = "直接充值：" + total.ToString(),
                    Price = total
                };
                orderType = (int)OrderType.DirectCharge;
            }
            

            detail = new TOrderDetail()
            {
                DocId = doc.DocId,
                DocTitle = doc.Title,
                GoodsCount = 1,
                Price = doc.Price,
                OrderId = orderId
            };
            order = new TOrder()
            {
                OrderType = orderType,
                OrderId = orderId,
                UserId = logonUser.UserId,
                Email = logonUser.Email,
                Total = detail.Price * detail.GoodsCount,
                Ip = Context.Request.UserHostAddress,
                Status = (int)OrderStatus.NewOrder,
                CreateTime = DateTime.Now,
                PayWay = Helper.FormatHelper.GetPayWay(PayWay),
                OrderDetails = new List<TOrderDetail>() { detail }
            };
            os.TOrderBll.SaveOrder(order);

           
            
            TOrder o = os.TOrderBll.Get(orderId);


            decimal amount = 0;//除扣除账户余额外还需要支付金额
            amount = total;
            
            //if (total > minfo.Amount)
            //{
            //    amount = total - minfo.Amount;
            //}
            //else {
            //    //账户支付
            //}

            #region 转至支付接口
            if (PayWay.ToLower() == "tenpay")
            {
                TenPay pay = new TenPay();
                pay.UserId = logonUser.UserId;
                pay.Send(orderId.ToString(), amount.ToString(), pname);
                
            }

            if (PayWay.ToLower() == "chinabank")
            {
                ChinaBankPay pay = new ChinaBankPay();
                pay.Send(orderId.ToString(), amount.ToString(), pname);

                // Response.Write("ChinaBank");
            }
            if (PayWay.ToLower() == "alipay")
            {
                AliPay pay = new AliPay();
                pay.Send(orderId.ToString(), amount.ToString(), pname,logonUser.UserId);

                // Response.Write("ChinaBank");
            }
            if (PayWay.ToLower() == "useraccount")
            {
                MAccount minfo = accs.MAccountBll.GetByUserId(logonUser.UserId);    //账户

                if (o.Total <= minfo.Amount && (o.Status == (int)OrderStatus.NewOrder))
                {
                    //ms.MAccount.AccountExpend(o.OrderId);
                    ////ms.MOrder.UpdateOrder2Paid(o.OrderId, 0, "账户支付", (int)Models.MOrderStateInfo.己付款);
                    //Web.Common.OrderCallBack oCallBack = new MamShare.Mall.Web.Common.OrderCallBack(
                    //            o.UserId, o.OrderId, 0, (int)Models.MOrderStateInfo.己付款, "账户支付");
                    //oCallBack.Update2Paid();

                    //Hashtable p = new Hashtable();
                    //p.Add("OrderId", o.OrderId);
                    //RedirectToAction("payok.do", p);
                    os.MAccountBll.AccountExpend(o.OrderId, Utils.TmmUtils.IPAddress());    //账户花销
                    Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                    oCallBack.UserId = o.UserId;
                    oCallBack.OrderId = o.OrderId;
                    oCallBack.PayWay = 0;
                    oCallBack.Status = (int)OrderStatus.IsPaied;
                    oCallBack.PayDetail = "账户支付";
                    oCallBack.GotoUrl = "/my/purchase.do";

                    oCallBack.ExecAfterPaid();

                }
                else
                {

                    Redirect("NetPay.do?orderid=" + o.OrderId);
                }

            }
            #endregion
            RenderView("pay");
        }

        [SkipFilter]
        public void ChinaBankReceive()
        {
            ChinaBankPay pay = new ChinaBankPay();
            pay.Receive();
            RenderView("PayResult");

        }
        /// <summary>
        /// 网银在线支付异步通知处理，需要人工把地址发送给对方客服
        /// </summary>
        [SkipFilter]
        public void ChinaBankNotify()
        {
            try
            {
                ChinaBankPay pay = new ChinaBankPay();
                pay.Notify();
                CancelLayout();
                CancelView();
            }
            catch (Exception e)
            {
                Utils.Log4Net.Error(e);
                CancelLayout();
                CancelView();
            }
        }
        /// <summary>
        /// 财付通支付返回通知处理
        /// </summary>
        [SkipFilter]
        public void TenPayReceive()
        {
            CancelLayout();
            TenPay pay = new TenPay();
            pay.Receive();
            RenderView("PayResult");
        }
        /// <summary>
        /// 财付通支付异步通知处理，没用，因为财付通直接多次请求返回通知
        /// </summary>
        [SkipFilter]
        public void TenPayNotify()
        {
            TenPay pay = new TenPay();
            pay.Notify();
            CancelLayout();
            CancelView();
        }
        /// <summary>
        /// 支付宝支付返回处理
        /// </summary>
        [SkipFilter]
        public void AliPayReceive()
        {
            AliPay pay = new AliPay();
            pay.Receive();
            RenderView("PayResult");
        }
        /// <summary>
        /// 支付宝支付异步通知处理
        /// </summary>
        [SkipFilter]
        public void AliPayNotify()
        {
            AliPay pay = new AliPay();
            pay.Notify();
            CancelLayout();
            CancelView();
            // RenderView("PayResult");
        }

        public void PayError(decimal orderId,string message)
        { 
            //更新订单状态
        }

        public void PayOk(decimal orderId)
        {
            Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
            TOrder o = obll.GetOrderAndDetail(orderId);
            string url = string.Empty;
            if (o.SingleDetail.DocId > 0)
            {
                url = "/my/Purchase.do";
            }
            else
            {
                url = "/my/AccountDetail.do";
            }
            PropertyBag["gotoUrl"] = url;
        }
    }


}
