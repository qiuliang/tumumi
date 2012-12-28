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
using AlipayClass;

/// <summary>
/// 功能：支付宝退款接口的入口页面，生成请求URL
/// 版本：3.1
/// 日期：2010-12-02
/// 说明：
/// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
/// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
/// 
/// /////////////////注意///////////////////////////////////////////////////////////////
/// 如果不想使用扩展功能请把扩展功能参数赋空值。
/// 该页面测试时出现“调试错误”请参考：http://club.alipay.com/read-htm-tid-8681712.html
/// 要传递的参数要么不允许为空，要么就不要出现在数组与隐藏控件或URL链接里。
/// </summary>

public partial class refund : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ///////////////////////以下参数是需要设置的相关配置参数，设置后不会更改的////////////////////////////
        AlipayConfig con = new AlipayConfig();
        string partner = con.Partner;
        string key = con.Key;
        string seller_email = con.Seller_email;
        string input_charset = con.Input_charset;
        string notify_url = con.Notify_url;
        string sign_type = con.Sign_type;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////请求参数/////////////////////////////////////////////////////////////////////
        //退款当天日期，获取当天日期，格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
        string refund_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //商家网站里的批次号，保证其唯一性，格式：当天日期[8位]+序列号[3至24位]，如：201008010000001
        string batch_no = DateTime.Now.ToString("yyyyMMddHHmmss");

        //退款笔数，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
        string batch_num = Request.Form["batch_num"];

        //退款详细数据
        string detail_data = Request.Form["detail_data"];
        //格式：第一笔交易#第二笔交易#第三笔交易
        //第N笔交易格式：交易退款信息
        //交易退款信息格式：原付款支付宝交易号^退款总金额^退款理由
        //注意：
        //1.detail_data中的退款笔数总和要等于参数batch_num的值
        //2.detail_data的值中不能有“^”、“|”、“#”、“$”等影响detail_data的格式的特殊字符
        //3.detail_data中退款总金额不能大于交易总金额
        //4.一笔交易可以多次退款，只需要遵守多次退款的总金额不超过该笔交易付款时金额。
        //5.不支持退分润功能
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        //构造请求函数
        AlipayRefundService aliRefundService = new AlipayRefundService(
            partner,
            seller_email,
            notify_url,
            refund_date,
            batch_no,
            batch_num,
            detail_data,
            key,
            input_charset,
            sign_type);

        string sHtmlText = aliRefundService.Build_Form();
        Response.Write(sHtmlText);
    }
}
