using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections.Generic;
using AlipayClass;

/// <summary>
/// 功能：付完款后跳转的页面（页面跳转同步通知页面）
/// 版本：3.1
/// 日期：2010-10-29
/// 说明：
/// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
/// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
/// 
/// ///////////////////////页面功能说明///////////////////////
/// 该页面可在本机电脑测试
/// 该页面称作“页面跳转同步通知页面”，是由支付宝服务器同步调用，可当作是支付完成后的提示信息页，如“您的某某某订单，多少金额已支付成功”。
/// 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
/// 该页面可以使用ASP.NET开发工具调试，也可以使用写文本函数Log_result进行调试，该函数已被默认关闭
/// TRADE_FINISHED(表示交易已经成功结束，为普通即时到帐的交易状态成功标识);
/// TRADE_SUCCESS(表示交易已经成功结束，为高级即时到帐的交易状态成功标识);
/// </summary>
public partial class return_url : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SortedDictionary<string, string> sArrary = GetRequestGet();
        ///////////////////////以下参数是需要设置的相关配置参数，设置后不会更改的//////////////////////
        AlipayConfig con = new AlipayConfig();
        string partner = con.Partner;
        string key = con.Key;
        string input_charset = con.Input_charset;
        string sign_type = con.Sign_type;
        string transport = con.Transport;
        //////////////////////////////////////////////////////////////////////////////////////////////

        if (sArrary.Count > 0)//判断是否有带返回参数
        {
            AlipayNotify aliNotify = new AlipayNotify(sArrary, Request.QueryString["notify_id"],partner,key,input_charset,sign_type,transport);
            string responseTxt = aliNotify.ResponseTxt; //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            string sign = Request.QueryString["sign"];  //获取支付宝反馈回来的sign结果
            string mysign = aliNotify.Mysign;           //获取通知返回后计算后（验证）的签名结果

            //写日志记录（若要调试，请取消下面两行注释）
            //string sWord = "responseTxt=" + responseTxt + "\n return_url_log:sign=" + Request.QueryString["sign"] + "&mysign=" + mysign + "\n return回来的参数：" + aliNotify.PreSignStr;
            //AlipayFunction.log_result(Server.MapPath("log/" + DateTime.Now.ToString().Replace(":", "")) + ".txt",sWord);

            //判断responsetTxt是否为ture，生成的签名结果mysign与获得的签名结果sign是否一致
            //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            //mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            if (responseTxt == "true" && sign == mysign)//验证成功
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //请在这里加上商户的业务逻辑程序代码

                //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                string trade_no = Request.QueryString["trade_no"];      //支付宝交易号
                string order_no = Request.QueryString["out_trade_no"];	//获取订单号
                string total_fee = Request.QueryString["total_fee"];	//获取总金额
                string subject = Request.QueryString["subject"];        //商品名称、订单名称
                string body = Request.QueryString["body"];              //商品描述、订单备注、描述
                string buyer_email = Request.QueryString["buyer_email"];//买家支付宝账号
                string trade_status = Request.QueryString["trade_status"];//交易状态

                //打印页面
                lbTrade_no.Text = trade_no;
                lbOut_trade_no.Text = order_no;
                lbTotal_fee.Text = total_fee;
                lbSubject.Text = subject;
                lbBody.Text = body;
                lbBuyer_email.Text = buyer_email;
                lbTrade_status.Text = trade_status;
                lbVerify.Text = "验证成功";

                if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")
                {
                    //判断该笔订单是否在商户网站中已经做过处理（可参考“集成教程”中“3.4返回数据处理”）
			            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
			            //如果有做过处理，不执行商户的业务程序
                }
                else
                {
                    Response.Write("trade_status=" + Request.QueryString["trade_status"]);
                }
                //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            else//验证失败
            {
                lbVerify.Text = "验证失败";
            }
        }
        else
        {
            lbVerify.Text = "无返回参数";
        }
    }

    /// <summary>
    /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
    /// </summary>
    /// <returns>request回来的信息组成的数组</returns>
    public SortedDictionary<string, string> GetRequestGet()
    {
        int i = 0;
        SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
        NameValueCollection coll;
        //Load Form variables into NameValueCollection variable.
        coll = Request.QueryString;

        // Get names of all forms into a string array.
        String[] requestItem = coll.AllKeys;

        for (i = 0; i < requestItem.Length; i++)
        {
            sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
        }

        return sArray;
    }
}
