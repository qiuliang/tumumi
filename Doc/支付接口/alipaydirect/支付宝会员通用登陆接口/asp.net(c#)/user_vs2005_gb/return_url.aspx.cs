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
using System.Text;
using System.Collections.Generic;
using AlipayClass;

/// <summary>
/// 功能：付完款后跳转的页面（页面跳转同步通知页面）
/// 版本：3.1
/// 日期：2010-10-22
/// 说明：
/// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
/// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
/// 
/// ///////////////////////页面功能说明///////////////////////
/// 该页面可在本机电脑测试
/// 该页面称作“页面跳转同步通知页面”，是由支付宝服务器同步调用
/// 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
/// 建议：
/// 在商户网站会员数据库中增加一个字段：user_id（支付宝用户唯一ID）
/// 会员信息的数据表中的唯一ID号是商户网站会员数据表中的
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
            AlipayNotify aliNotify = new AlipayNotify(sArrary, Request.QueryString["notify_id"], partner, key, input_charset, sign_type, transport);
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
                ///////////////////////////请在这里加上商户的业务逻辑程序代码///////////////////////////////////////////////////////////////////////////////
                //请根据您的业务逻辑来编写程序（以下代码仅作参考）
                //获取支付宝的通知返回参数
                string user_id = Request.QueryString["user_id"];	        //获取支付宝用户唯一ID号

                //以下是示例——
                //判断获取到的user_id的值是否在商户会员数据库中存在（即：是否曾经做过支付宝会员通用登录）
                //	若不存在，则程序自动为会员快速注册一个会员，把信息插入商户网站会员数据表中，
                //	且把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登录信息，如登录时间、次数、IP等。
                //	若存在，判断该会员在商户网站上的登录状态是否是“已登录”状态
                //		若不是，则把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登录信息，如登录时间、次数、IP等。
                //		若是，则不做任何数据库业务逻辑处理。判定该次反馈信息为重复刷新返回链接导致。

                //打印页面
                StringBuilder sbHtml = new StringBuilder();
                sbHtml.Append("<table align=center width=350 cellpadding=5 cellspacing=0>");
                sbHtml.Append("<tr><td align=center class=font_title>亲爱的商城会员，" + user_id + "：<br />您已经登录成功</td>");
                sbHtml.Append("</tr></table>");
                LblShow.Text = sbHtml.ToString();
                //请根据您的业务逻辑来编写程序（以上代码仅作参考）
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            else//验证失败
            {
                LblShow.Text = "系统出错，验证失败";
            }
        }
        else
        {
            LblShow.Text = "无返回参数";
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
