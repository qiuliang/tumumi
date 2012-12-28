using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using TMM.Model;

namespace TMM.Core.Pay
{
    public class TenPay
    {
        int PayWay = 2;//支付宝是1，财付通是2，网银在线是3，手工支付10
        private string returnUrl = "http://{0}/pay/TenPayReceive.do";
        private string payOkUrl = "http://{0}/pay/payOK.do?orderId={1}";
        private string payUrl = "https://www.tenpay.com/cgi-bin/v1.0/pay_gate.cgi";
        private string payErrorUrl = "http://{0}/pay/payError.do?orderId={1}&message={2}";
        
        private int chargeType; //1：下载产生订单进行充值 0：直接充值

        #region 属性
        public int UserId { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public TenPay()
        {

            string domain = HttpContext.Current.Request.Url.Host;
            returnUrl = returnUrl.Replace("{0}", domain);           
            payOkUrl = payOkUrl.Replace("{0}", domain);
            payErrorUrl=payErrorUrl.Replace("{0}",domain);
            
        }
        //public TenPay()
        //{
        //    string domain = HttpContext.Current.Request.Url.Host;
        //    returnUrl = returnUrl.Replace("{0}", domain).Replace("{1}", chargeType.ToString());  
        //    payOkUrl = payOkUrl.Replace("{0}", domain);
        //    payErrorUrl = payErrorUrl.Replace("{0}", domain);
        //}
        /// <summary>
        /// 发送支付请求
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="total"></param>
        /// <param name="remark"></param>
        public void Send(string orderno, string total, string remark)
        {
            Md5Pay p = new Md5Pay();
            p.Paygateurl = payUrl;
            p.Desc = remark;
          
            p.Total_fee = (long)(decimal.Parse(total)*100); 
            p.Sp_billno = orderno;
            p.Attach = "1";
            p.Return_url = returnUrl;
            p.Transaction_id = p.Bargainor_id + p.Date + p.UnixStamp();
            p.Spbill_create_ip =HttpContext.Current.Request.UserHostAddress;
            string url = "";
            
            if (!p.GetPayUrl(out url))
            {
                HttpContext.Current.Response.Write("TenPay Error");
                //labErrmsg.Text = Server.HtmlEncode(url);
            }
            else
            {
                /*在这里可以把
                 * 交易单号			md5pay.Transaction_id
                 * 商户订单号		md5pay.Sp_billno
                 * 订单金额			md5pay.Total_fee
                 * 等信息记入数据库.
                 * */


                MPayLog payrecordinfo = new MPayLog();
                payrecordinfo.PayUrl = url;
                payrecordinfo.OrderId = decimal.Parse(orderno);
                payrecordinfo.PayMoney = decimal.Parse(total);
                payrecordinfo.UserId = this.UserId;
                payrecordinfo.PayResult = "100";
                payrecordinfo.PayWay = PayWay;
                payrecordinfo.TransactionId = p.Transaction_id;
                payrecordinfo.BackUrl = p.Return_url;
                payrecordinfo.CreateTime = DateTime.Now;

                TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();
                pbll.Insert(payrecordinfo);

                HttpContext.Current.Response.Redirect(url);
            }
        }
        /// <summary>
        /// 接受支付成功返回
        /// </summary>
        public void Receive()
        {
            string suchtml = "<meta content=\"China TENCENT\" name=\"TENCENT_ONLINE_PAYMENT\">\n"
                    + "<script language=\"javascript\">\n"
                    + "window.location.href='" + payOkUrl + "';\n"
                    + "</script>";


            string errmsg = "";

            //string key = "tenpaytesttenpaytesttenpaytest12";
            // string bargainor_id = "1202437801";

            Md5Pay md5pay = new Md5Pay();

            //md5pay.Key = key;
            //md5pay.Bargainor_id = bargainorId;
         
            //判断签名
            if (md5pay.GetPayValueFromUrl(HttpContext.Current.Request.QueryString, out errmsg))
            {
                Decimal orderid = 0;
                orderid = Decimal.Parse(md5pay.Sp_billno); 
                MPayLog payrecordinfo = new MPayLog();

                payrecordinfo.PayUrl = HttpContext.Current.Request.UrlReferrer+"";
                payrecordinfo.OrderId = orderid;
                payrecordinfo.PayResult = md5pay.Pay_Result.ToString();
                payrecordinfo.PayMoney = decimal.Parse(md5pay.Total_fee.ToString())/100;
                payrecordinfo.PayWay = PayWay;
                payrecordinfo.TransactionId = md5pay.Transaction_id;
                payrecordinfo.BackUrl = HttpContext.Current.Request.Url.AbsoluteUri;
                payrecordinfo.CreateTime = DateTime.Now;
                TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();
                pbll.Insert(payrecordinfo);
                //认证签名成功
                //支付判断
                if (md5pay.Pay_Result == Md5Pay.PAYOK)
                {                   
                    Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
                    Service.Bll.User.MAccountBLL abll = new TMM.Service.Bll.User.MAccountBLL();
                    TOrder o = obll.GetOrderAndDetail(payrecordinfo.OrderId);
                    if (o.Status >= 0 && o.Status < 10) //新订单 -- 已付款 之间的过程
                    {                        
                        //充值
                        abll.AddAmount(payrecordinfo.OrderId, payrecordinfo.PayMoney, Utils.TmmUtils.IPAddress(),this.PayWay);
                        
                        bool expandResult = true;
                        //docid小于0 的情况为特殊订单，不执行账户花销的动作
                        if (o.SingleDetail.DocId > 0)
                        {
                            expandResult = abll.AccountExpend(payrecordinfo.OrderId, Utils.TmmUtils.IPAddress());
                        }
                        //继续调用回调接口
                        if (expandResult)
                        {
                            #region 订单回调接口
                            //obll.UpdateOrder2Paid(payrecordinfo.OrderId, PayWay, "财付通支付", (int)Models.MOrderStateInfo.己付款);
                            Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                            oCallBack.UserId = o.UserId;
                            oCallBack.OrderId = o.OrderId;
                            oCallBack.PayWay = PayWay;
                            oCallBack.Status = (int)OrderStatus.IsPaied;
                            oCallBack.PayDetail = "财付通支付";

                            oCallBack.ExecAfterPaid();
                            suchtml = suchtml.Replace("{1}", orderid.ToString());
                            //支付成功，同定单号md5pay.Transaction_id可能会多次通知，请务必注意判断订单是否重复的逻辑
                            //处理业务逻辑，处理db之类的
                            //errmsg = "支付成功";
                            //Response.Write(errmsg+"<br/>");
                            //Response.Write("财付通定单号:"+ md5pay.Transaction_id +"(请牢记定单号)"+"<br/>");	
                            //跳转到成功页面，财付通收到<meta content=\"China TENCENT\" name=\"TENCENT_ONLINE_PAYMENT\">，认为通知成功
                            payOkUrl = payOkUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                            Utils.Log4Net.Error(string.Format("【财付通】-【同步回调】-【成功】-【{0}】", payOkUrl));
                            HttpContext.Current.Response.Write(suchtml);
                            #endregion

                        }
                        else
                        {
                            Utils.Log4Net.Error(string.Format("【财付通】-【同步回调】-【失败AccountExpend】-【{0}】", payOkUrl));
                            payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                            payErrorUrl = payErrorUrl.Replace("{2}", "账户余额不足！");
                        }
                    }                    
                    else
                    {
                        Utils.Log4Net.Error(string.Format("【财付通】-【同步回调】-【失败】-【{0}】【订单状态{1}】", payOkUrl,o.Status));
                        payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                        payErrorUrl = payErrorUrl.Replace("{2}", "订单错误！");
                    }
                }
                else
                {
                    //支付失败，请不要按成功处理
                    //  Response.Write(md5pay.Pay_Result+"  "+Md5Pay.PAYOK);
                    payErrorUrl = payErrorUrl.Replace("{1}", md5pay.Sp_billno);
                    payErrorUrl = payErrorUrl.Replace("{2}", "支付失败" + errmsg);
                    HttpContext.Current.Response.Redirect(payErrorUrl);
                    //HttpContext.Current.Response.Write("支付失败" + errmsg + "<br/>");                  
                }

            }
            else
            {
                //认证签名失败
                payErrorUrl = payErrorUrl.Replace("{1}", md5pay.Sp_billno);
                payErrorUrl = payErrorUrl.Replace("{2}", "认证签名失败");
                HttpContext.Current.Response.Redirect(payErrorUrl);
               // errmsg = "认证签名失败";
               // HttpContext.Current.Response.Write("认证签名失败" + "<br/>");
            }
        }

       

        /// <summary>
        /// 财付通不用异步通知，直接调用返回接口
        /// </summary>
        public void Notify()
        {
            
        }
        /// <summary>
        /// 支付地址，默认写在接口中
        /// </summary>
        public string PayUrl
        {
            get { return this.payUrl; }
            set { this.payUrl = value; }
        }
        /// <summary> 
        /// 支付返回地址，默认写在接口中
        /// </summary>
        public string BackUrl
        {
            get { return this.returnUrl; }
            set { this.returnUrl = value; }
        }
        /// <summary>
        /// 支付成功地址，默认写在接口中，如：http://www.iyoupiao.com/pay/payshow.do?orderId={0}，表示带参数到成功地址
        /// </summary>
        public string PayOkUrl
        {
            get { return this.payOkUrl; }
            set { this.payOkUrl = value; }
        }
        /// <summary>
        /// 支付失败地址，默认写在接口中，如：http://www.iyoupiao.com/pay/payshow.do?orderId={0}&message={1}，表示带参数到成功地址
        /// </summary>
        public string PayErrorUrl
        {
            get { return this.PayErrorUrl; }
            set { this.payErrorUrl = value; }
        }
    }
}
