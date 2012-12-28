using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using TMM.Model;

namespace TMM.Core.Pay
{
    public class AliPay
    {
        int PayWay = 1;//支付宝是1，财付通是2，网银在线是3，手工支付10
        private string payErrorUrl = "http://{0}/pay/payError.do?orderId={1}&message={2}";
         protected string payUrl = "https://www.alipay.com/cooperate/gateway.do?";//'支付接口
       // protected string payUrl = "http://notify.alipay.com/trade/notify_query.do?";  //'支付接口
        protected string payOkUrl = "http://{0}/pay/payOK.do?orderId={1}";
        protected string returnUrl = "http://{0}/Pay/AliPayReceive.do"; // 商户自定义返回接收支付结果的页面
        protected string NotifyUrl = "http://{0}/Pay/AliPayNotify.do"; // 商户自定义异步通知页面
        string alipayNotifyURL = "https://www.alipay.com/cooperate/gateway.do?service=notify_verify";//验证地址
        string QueryNotifyURL = "http://notify.alipay.com/trade/notify_query.do?service=notify_verify";//通知验证地址,服务器不支持https访问外网，非ie状态啊
        protected string key = "d2rb7czc5uxq2keavyfpopsv3098ayrk"; //partner账户的支付宝安全校验码
        protected string alipayAccount = "7109137@qq.com";// "liu.man@corp.0-6.com";
        protected string partner = "2088502888458253"; //partner		合作伙伴ID			保留字段   
        string service = "create_direct_pay_by_user";
        protected string productUrl = "http://{0}";
        string _input_charset = "utf-8";//编码类型
        string payment_type = "1";                  //支付类型	
        string sign_type = "MD5";
        AliPayPackage ap = new AliPayPackage();
        public AliPay()
        {
            string domain = HttpContext.Current.Request.Url.Host;
            returnUrl = returnUrl.Replace("{0}", domain);
            NotifyUrl = NotifyUrl.Replace("{0}", domain);
            payOkUrl = payOkUrl.Replace("{0}", domain);
            payErrorUrl = payErrorUrl.Replace("{0}", domain);
            productUrl = productUrl.Replace("{0}", domain);
        }
        public void Send(string orderNo, string total, string remark,int userId)
        {
           
            //业务参数赋值；
            string gateway = payUrl;	//'支付接口  
            string subject = remark;	//subject		商品名称
            string body = remark;		//body			商品描述               
            string price = total;
            string quantity = "1";
            string show_url = productUrl;
            string seller_email = alipayAccount;             //卖家账号            
            string return_url = returnUrl; //服务器通知返回接口
            string notify_url = NotifyUrl; //服务器通知返回接口           
            string logistics_type = "EMS";//货物发送方式
            string logistics_fee = "0";//货运费用
            string logistics_payment = "BUYER_PAY";//支付方              
            string[] para ={
        "service="+service,
        "partner=" + partner,
        "seller_email=" + seller_email,
        "out_trade_no=" + orderNo,
        "subject=" + subject,
        "body=" + body,
        "total_fee=" + total, 
        "show_url=" + show_url,
        "payment_type=1",
        "notify_url=" + notify_url,
        "return_url=" + return_url,
        "_input_charset="+_input_charset
        };
            //支付URL生成
            string aliay_url = AliPayPackage.CreatUrl(
                gateway,//GET方式传递参数时请去掉注释
                para,
                _input_charset,
                sign_type,
                key
                );
            //订单支付记录
            MPayLog payrecordinfo = new MPayLog();
            //Models.MPayLogInfo payrecordinfo = new Models.MPayLogInfo();
            payrecordinfo.PayUrl = aliay_url;
            payrecordinfo.UserId = userId;
            payrecordinfo.OrderId = decimal.Parse(orderNo);
            payrecordinfo.PayMoney = decimal.Parse(total);
            payrecordinfo.TransactionId = orderNo;
            payrecordinfo.PayResult = "100";
            payrecordinfo.PayWay = PayWay;
            payrecordinfo.BackUrl = PayReturnUrl;
            payrecordinfo.CreateTime = DateTime.Now;

            TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();
            pbll.Insert(payrecordinfo);

            //TMM.Core.Utils.Log4Net.Error(aliay_url);

            HttpContext.Current.Response.Redirect(aliay_url);
            
        }
      
       public void Receive()
       {       

           alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + HttpContext.Current.Request.QueryString["notify_id"];
           //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
          // alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + HttpContext.Current.Request.QueryString["notify_id"];

           //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
           string responseTxt = AliPayPackage.Get_Http(alipayNotifyURL, 120000);
           Utils.Log4Net.Error("支付宝ATN返回结果：" + responseTxt);
           //*******加密签名程序开始//*******
           int i;
           System.Collections.Specialized.NameValueCollection coll;
           //Load Form variables into NameValueCollection variable.
           coll = HttpContext.Current.Request.QueryString;

           // Get names of all forms into a string array.
           String[] requestarr = coll.AllKeys;

           //进行排序；
           string[] Sortedstr = AliPayPackage.BubbleSort(requestarr);

           //构造待md5摘要字符串 ；

           StringBuilder prestr = new StringBuilder();

           for (i = 0; i < Sortedstr.Length; i++)
           {
               if (HttpContext.Current.Request.Form[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
               {
                   if (i == Sortedstr.Length - 1)
                   {
                       prestr.Append(Sortedstr[i] + "=" + HttpContext.Current.Request.QueryString[Sortedstr[i]]);
                   }
                   else
                   {
                       prestr.Append(Sortedstr[i] + "=" + HttpContext.Current.Request.QueryString[Sortedstr[i]] + "&");

                   }
               }
           }

           prestr.Append(key);

           //生成Md5摘要；
           string mysign = AliPayPackage.GetMD5(prestr.ToString(), _input_charset);
           //*******加密签名程序结束*******

           string sign = HttpContext.Current.Request.QueryString["sign"];     
           //  Response.Write(prestr.ToString());  
           MPayLog payrecordinfo = new MPayLog();
           //Models.MPayLogInfo payrecordinfo = new Models.MPayLogInfo();
           payrecordinfo.PayUrl = HttpContext.Current.Request.UrlReferrer + "";//返回地址
           payrecordinfo.OrderId = decimal.Parse(HttpContext.Current.Request.QueryString["out_trade_no"]);
           if (responseTxt=="true")
                payrecordinfo.PayResult = "1";
           else
                payrecordinfo.PayResult = "0";
           payrecordinfo.PayMoney = decimal.Parse(HttpContext.Current.Request.QueryString["total_fee"]);
           payrecordinfo.TransactionId = HttpContext.Current.Request.QueryString["trade_no"];
           payrecordinfo.PayWay = PayWay;
           payrecordinfo.BackUrl = HttpContext.Current.Request.Url.AbsoluteUri;
           payrecordinfo.CreateTime = DateTime.Now;

           TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();
           pbll.Insert(payrecordinfo);

           if (mysign == sign && responseTxt == "true")   //验证支付发过来的消息，签名是否正确
           {

               //更新自己数据库的订单语句，请自己填写一下

               Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
               Service.Bll.User.MAccountBLL abll = new TMM.Service.Bll.User.MAccountBLL();
               TOrder o = obll.GetOrderAndDetail(payrecordinfo.OrderId);
               if (o.Status >= 0 && o.Status < 10) //新订单 -- 已付款 之间的过程
               {
                   //充值
                   abll.AddAmount(payrecordinfo.OrderId, payrecordinfo.PayMoney,Utils.TmmUtils.IPAddress(),this.PayWay);
                   bool expandResult = true;
                   //docid小于0 的情况为特殊订单，不执行账户花销的动作
                   if (o.SingleDetail.DocId > 0)
                   {
                       expandResult = abll.AccountExpend(payrecordinfo.OrderId, Utils.TmmUtils.IPAddress());
                   }

                   if (expandResult)
                   {                     
                       
                       Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                       oCallBack.UserId = o.UserId;
                       oCallBack.OrderId = o.OrderId;
                       oCallBack.PayWay = PayWay;
                       oCallBack.Status = (int)OrderStatus.IsPaied;
                       oCallBack.PayDetail = "淘宝支付";

                       oCallBack.ExecAfterPaid();

                       //
                       payOkUrl = payOkUrl.Replace("{1}", o.OrderId.ToString());
                       HttpContext.Current.Response.Redirect(payOkUrl);
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
               // Response.Write("success");     //返回给支付宝消息，成功


           }
           else
           {
               
               payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
               payErrorUrl = payErrorUrl.Replace("{2}", "校验失败,支付失败" + responseTxt);
               HttpContext.Current.Response.Redirect(payErrorUrl);
           }
       }

        public void Notify()
        {
            try
            {
                QueryNotifyURL = QueryNotifyURL + "&partner=" + partner + "&notify_id=" + HttpContext.Current.Request["notify_id"].ToString();
                //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
                // alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + HttpContext.Current.Request.QueryString["notify_id"];

                //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
                string responseTxt = AliPayPackage.Get_Http(QueryNotifyURL, 120000);

                //*******加密签名程序开始//*******
                int i;
                System.Collections.Specialized.NameValueCollection coll;
                //Load Form variables into NameValueCollection variable.
                coll = HttpContext.Current.Request.Form;

                // Get names of all forms into a string array.
                String[] requestarr = coll.AllKeys;

                //进行排序；
                string[] Sortedstr = AliPayPackage.BubbleSort(requestarr);

                //构造待md5摘要字符串 ；

                StringBuilder prestr = new StringBuilder();
               // string temp = "";
                for (i = 0; i < Sortedstr.Length; i++)
                {
                    if (RQ(Sortedstr[i]) != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                    {
                        if (i == Sortedstr.Length - 1)
                        {
                            prestr.Append(Sortedstr[i] + "=" + RQ(Sortedstr[i]));
                        }
                        else
                        {
                            prestr.Append(Sortedstr[i] + "=" + RQ(Sortedstr[i]) + "&");

                        }
                    }
                   // temp+=Sortedstr[i];
                }
              //  MamShare.Utils.Log4Net.Error(temp);
                prestr.Append(key);

                //生成Md5摘要；
                string mysign = AliPayPackage.GetMD5(prestr.ToString(), _input_charset);
                //*******加密签名程序结束*******

                
                string sign = RQ("sign");
                //  Response.Write(prestr.ToString());  
                string trade_status = RQ("trade_status");
                MPayLog payrecordinfo = new MPayLog();
                payrecordinfo.PayUrl = QueryNotifyURL;//返回地址
                payrecordinfo.OrderId = decimal.Parse(RQ("out_trade_no"));//HttpContext.Current.Request.QueryString["out_trade_no"]);
                if (responseTxt == "true")
                    payrecordinfo.PayResult = "1";
                else
                    payrecordinfo.PayResult = "0";
                payrecordinfo.PayMoney = decimal.Parse(RQ("total_fee")); //decimal.Parse(HttpContext.Current.Request.QueryString["total_fee"]);
                payrecordinfo.TransactionId = RQ("trade_no"); //HttpContext.Current.Request.QueryString["trade_no"];
                payrecordinfo.PayWay = PayWay;
                payrecordinfo.BackUrl = HttpContext.Current.Request.Url.AbsoluteUri;
                payrecordinfo.CreateTime = DateTime.Now;
                TMM.Service.Bll.Order.MPayLogBLL pbll = new TMM.Service.Bll.Order.MPayLogBLL();
                pbll.Insert(payrecordinfo);

                if (responseTxt == "true")   //验证支付发过来的消息，签名是否正确
                {

                    //更新自己数据库的订单语句，请自己填写一下
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {
                        Service.Bll.Order.TOrderBLL obll = new TMM.Service.Bll.Order.TOrderBLL();
                        Service.Bll.User.MAccountBLL abll = new TMM.Service.Bll.User.MAccountBLL();
                        TOrder o = obll.GetOrderAndDetail(payrecordinfo.OrderId);
                        if (o.Status >= 0 && o.Status < 10) //新订单 -- 已付款 之间的过程
                        {
                            //充值
                            abll.AddAmount(payrecordinfo.OrderId, payrecordinfo.PayMoney, Utils.TmmUtils.IPAddress(), this.PayWay);
                            bool expandResult = true;
                            //docid小于0 的情况为特殊订单，不执行账户花销的动作
                            if (o.SingleDetail.DocId > 0)
                            {
                                expandResult = abll.AccountExpend(payrecordinfo.OrderId, Utils.TmmUtils.IPAddress());
                            }

                            
                            if (expandResult)
                            {
                                Common.OrderCallBack oCallBack = new TMM.Core.Common.OrderCallBack();
                                oCallBack.UserId = o.UserId;
                                oCallBack.OrderId = o.OrderId;
                                oCallBack.PayWay = PayWay;
                                oCallBack.Status = (int)OrderStatus.IsPaied;
                                oCallBack.PayDetail = "淘宝支付";

                                oCallBack.ExecAfterPaid();
                                Utils.Log4Net.Error(string.Format("支付宝接口异步调用回调方法成功，跳转到URL：{0}", payOkUrl));
                                
                            }
                            else
                            {
                                payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                                payErrorUrl = payErrorUrl.Replace("{2}", "账户余额不足！");

                            }
                        }
                    }
                    HttpContext.Current.Response.Write("success");
                    // Response.Write("success");     //返回给支付宝消息，成功


                }
                else
                {
                    //HttpContext.Current.Response.Write("------------------------------------------");
                    //HttpContext.Current.Response.Write("<br>Result:responseTxt=" + responseTxt);
                    //HttpContext.Current.Response.Write("<br>Result:mysign=" + mysign);
                    //HttpContext.Current.Response.Write("<br>Result:sign=" + sign);
                    //HttpContext.Current.Response.Write("fail");
                    payErrorUrl = payErrorUrl.Replace("{1}", payrecordinfo.OrderId.ToString());
                    payErrorUrl = payErrorUrl.Replace("{2}", "校验失败,支付失败" + responseTxt);
                    HttpContext.Current.Response.Write("fail");
                    //  HttpContext.Current.Response.Redirect(payErrorUrl);
                }
            }
            catch(Exception e)
            {
                Utils.Log4Net.Error(e);
                HttpContext.Current.Response.Write("fail");
            }
        }
        /// <summary>
        /// request请求
        /// </summary>
        /// <param name="pname"></param>
        /// <returns></returns>
        public string RQ(string pname)
        {
            try
            {
                return HttpContext.Current.Request[pname].ToString();
            }
            catch
            {
                return "";
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
       public string PayReturnUrl
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
