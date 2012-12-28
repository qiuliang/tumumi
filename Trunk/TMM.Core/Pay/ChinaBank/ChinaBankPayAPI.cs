using System;
using System.Data;
using System.Configuration;
using System.Web;
using TMM.Model;

namespace TMM.Core.Pay
{
    public class ChinaBankPay
    {
        int PayWay = 3;//支付宝是1，财付通是2，网银在线是3，手工支付10
        private string payErrorUrl = "http://{0}/pay/payError.do?orderId={1}&message={2}";
        protected string payUrl = "https://pay3.chinabank.com.cn/PayGate";
        protected string payOkUrl = "http://{0}/pay/payOK.do?orderId={1}";
        protected string v_url = "http://{0}/Pay/ChinaBankReceive.do"; // 商户自定义返回接收支付结果的页面
        protected string v_amount;       // 订单金额
        protected string v_moneytype = "CNY";
        protected string v_md5info;      // 对拼凑串MD5私钥加密后的值
        protected string v_mid = "21837738";// 商户号，这里为测试商户号20000400，替换为自己的商户号即可        
        protected string v_oid;		 // 推荐订单号构成格式为 年月日-商户号-小时分钟秒
        string key = "mall7804jhsl9389ijfk3533";				 // 如果您还没有设置MD5密钥请登陆我们为您提供商户后台，地址：https://merchant3.chinabank.com.cn/
        protected string v_remark1;
        protected string v_pstatus;	// 支付状态码
        //20（支付成功，对使用实时银行卡进行扣款的订单）；
        //30（支付失败，对使用实时银行卡进行扣款的订单）；
        protected string v_pstring;	//支付状态描述
        protected string v_pmode;	//支付银行
        protected string remark1;	// 备注1
        protected string remark2 = "url:=http://{0}/pay/ChinaBankNotify.do";	// 备注2,异步返回
        protected string v_md5str;
        
        public  ChinaBankPay()
        {
            string domain = HttpContext.Current.Request.Url.Host;
            v_url = v_url.Replace("{0}", domain);
            payOkUrl = payOkUrl.Replace("{0}", domain);
            payErrorUrl = payErrorUrl.Replace("{0}", domain);
            remark2 = remark2.Replace("{0}", domain);
        }
        public void Send(string orderNo, string total, string remark)
        {
          
            v_oid = orderNo;
            if (v_oid == null || v_oid.Equals(""))
            {
                return;
            }
            v_amount = total;            
            string text = v_amount + v_moneytype + v_oid + v_mid + v_url + key; // 拼凑加密串
            v_md5info = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(text, "md5").ToUpper();
            v_remark1 = remark;
            string para = "";
            para += "v_mid=" + v_mid;
            para += "&v_url=" + v_url;
            para += "&v_oid=" + v_oid;
            para += "&v_amount=" + v_amount;
            para += "&v_moneytype=" + v_moneytype;
            para += "&v_md5info=" + v_md5info;
            para += "&v_remark1=" + v_remark1;
            para += "&remark2=" + remark2;
            payUrl += "?" + para;

            MPayLog payrecordinfo = new MPayLog();
            payrecordinfo.PayUrl = payUrl;
            payrecordinfo.OrderId = decimal.Parse(orderNo);
            payrecordinfo.PayMoney = decimal.Parse(v_amount);
            payrecordinfo.TransactionId = orderNo;
            payrecordinfo.PayResult = "100";
            payrecordinfo.PayWay = PayWay;           
            payrecordinfo.BackUrl = BackUrl;

            TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();            
            pbll.Insert(payrecordinfo);

            HttpContext.Current.Response.Redirect(payUrl);
           
            
        }
        /// <summary>
        /// 链接接收
        /// </summary>
        public void Receive()
        {

            v_oid = HttpContext.Current.Request["v_oid"];
            v_pstatus = HttpContext.Current.Request["v_pstatus"];
            v_pstring = HttpContext.Current.Request["v_pstring"];
            v_pmode = HttpContext.Current.Request["v_pmode"];
            v_md5str = HttpContext.Current.Request["v_md5str"];
            v_amount = HttpContext.Current.Request["v_amount"];
            v_moneytype = HttpContext.Current.Request["v_moneytype"];
            remark1 = HttpContext.Current.Request["remark1"];
            remark2 = HttpContext.Current.Request["remark2"];

            string str = v_oid + v_pstatus + v_amount + v_moneytype + key;

            str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToUpper();

            MPayLog payrecordinfo = new MPayLog();
            payrecordinfo.PayUrl = HttpContext.Current.Request.UrlReferrer + "";
            payrecordinfo.OrderId = decimal.Parse(v_oid);
            payrecordinfo.PayResult = v_pstatus;
            payrecordinfo.PayMoney = decimal.Parse(v_amount);
            payrecordinfo.TransactionId = v_oid.ToString();
            payrecordinfo.PayWay = PayWay;
            payrecordinfo.BackUrl = HttpContext.Current.Request.Url.ToString();

            TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();            
            pbll.Insert(payrecordinfo);
            if (str == v_md5str)
            {

                if (v_pstatus.Equals("20"))
                {

                    Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
                    Service.Bll.User.MAccountBLL abll = new TMM.Service.Bll.User.MAccountBLL();
                    TOrder o = obll.Get(payrecordinfo.OrderId);
                    if (o.Status >= 0 && o.Status < 10) //新订单 -- 已付款 之间的过程
                    {
                        abll.AddAmount(payrecordinfo.OrderId, payrecordinfo.PayMoney,Utils.TmmUtils.IPAddress(),this.PayWay);
                        if (abll.AccountExpend(payrecordinfo.OrderId,Utils.TmmUtils.IPAddress()))
                        {
                            
                            //Web.Common.OrderCallBack oCallBack = new MamShare.Mall.Web.Common.OrderCallBack(
                            //    o.UserId, o.OrderId, PayWay, (int)Models.MOrderStateInfo.己付款, "ChinaBank支付");
                            //oCallBack.Update2Paid();
                            //payOkUrl = payOkUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                            //HttpContext.Current.Response.Redirect(payOkUrl);
                            Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                            oCallBack.UserId = o.UserId;
                            oCallBack.OrderId = o.OrderId;
                            oCallBack.PayWay = PayWay;
                            oCallBack.Status = (int)OrderStatus.IsPaied;
                            oCallBack.PayDetail = "ChinaBank支付";

                            oCallBack.ExecAfterPaid();
                        }
                        else
                        {
                            payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                            payErrorUrl = payErrorUrl.Replace("{2}", "账户余额不足！");
                        }
                    }
                    
                    else
                    {
                        Utils.Log4Net.Error(string.Format("支付接口直接调用回调方法【失败】，跳转到URL：{0}", payOkUrl));
                        payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                        payErrorUrl = payErrorUrl.Replace("{2}", "订单错误！");
                    }
                }
            }
            else
            {
                payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                payErrorUrl = payErrorUrl.Replace("{2}", "校验失败,数据可疑");
                HttpContext.Current.Response.Redirect(payErrorUrl);
                //  HttpContext.Current.Response.Write("校验失败,数据可疑");
            }
        }
        /// <summary>
        /// 异步通知
        /// </summary>
        public void Notify()
        {
            v_oid = HttpContext.Current.Request["v_oid"];
            v_pstatus = HttpContext.Current.Request["v_pstatus"];
            v_pstring = HttpContext.Current.Request["v_pstring"];
            v_pmode = HttpContext.Current.Request["v_pmode"];
            v_md5str = HttpContext.Current.Request["v_md5str"];
            v_amount = HttpContext.Current.Request["v_amount"];
            v_moneytype = HttpContext.Current.Request["v_moneytype"];
            remark1 = HttpContext.Current.Request["remark1"];
            remark2 = HttpContext.Current.Request["remark2"];

            string str = v_oid + v_pstatus + v_amount + v_moneytype + key;

            str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToUpper();

            MPayLog payrecordinfo = new MPayLog();
            payrecordinfo.PayUrl = HttpContext.Current.Request.UrlReferrer + "";
            payrecordinfo.OrderId = decimal.Parse(v_oid);
            payrecordinfo.PayResult = v_pstatus;
            payrecordinfo.PayMoney = decimal.Parse(v_amount);
            payrecordinfo.TransactionId = v_oid.ToString();
            payrecordinfo.PayWay = PayWay;
            payrecordinfo.BackUrl = HttpContext.Current.Request.Url.ToString();

            TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();  
            pbll.Insert(payrecordinfo);
            if (str == v_md5str)
            {

                if (v_pstatus.Equals("20"))
                {
                    string domain = HttpContext.Current.Request.Url.Host;
                    //确保是m6go下的订单
                    if (remark2.ToLower().Contains(domain.ToLower()) == true)
                    {
                        Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
                        Service.Bll.User.MAccountBLL abll = new TMM.Service.Bll.User.MAccountBLL();
                        TOrder o = obll.Get(payrecordinfo.OrderId);
                        if (o.Status >= 0 && o.Status < 10) //新订单 -- 已付款 之间的过程
                        {
                            abll.AddAmount(payrecordinfo.OrderId, payrecordinfo.PayMoney,Utils.TmmUtils.IPAddress(),this.PayWay);
                            if (abll.AccountExpend(payrecordinfo.OrderId,Utils.TmmUtils.IPAddress()))
                            {
                                
                                //Web.Common.OrderCallBack oCallBack = new MamShare.Mall.Web.Common.OrderCallBack(
                                //    o.UserId, o.OrderId, PayWay, (int)Models.MOrderStateInfo.己付款, "ChinaBank支付");
                                //oCallBack.Update2Paid();
                                //payOkUrl = payOkUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                                //HttpContext.Current.Response.Redirect(payOkUrl);
                                Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                                oCallBack.UserId = o.UserId;
                                oCallBack.OrderId = o.OrderId;
                                oCallBack.PayWay = PayWay;
                                oCallBack.Status = (int)OrderStatus.IsPaied;
                                oCallBack.PayDetail = "ChinaBank支付";

                                oCallBack.ExecAfterPaid();
                            }
                            else
                            {
                                payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                                payErrorUrl = payErrorUrl.Replace("{2}", "账户余额不足！");
                                HttpContext.Current.Response.Redirect(payErrorUrl);
                            }
                        }
                    }
                    HttpContext.Current.Response.Write("ok");
                }
                else
                    HttpContext.Current.Response.Write("error");
            }
            else
            {
                payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                payErrorUrl = payErrorUrl.Replace("{2}", "校验失败,数据可疑");
                HttpContext.Current.Response.Write("error");
                // HttpContext.Current.Response.Redirect(payErrorUrl);
                //  HttpContext.Current.Response.Write("校验失败,数据可疑");
            }
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
            get { return this.v_url; }
            set { this.v_url = value; }
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
