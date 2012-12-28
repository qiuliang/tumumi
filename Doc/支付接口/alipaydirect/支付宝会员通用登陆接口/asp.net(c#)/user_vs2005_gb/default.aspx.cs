using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using AlipayClass;

/// <summary>
/// 功能：会员通用登录接口的入口页面，生成请求URL
/// 版本：3.1
/// 日期：2010-11-26
/// 说明：
/// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
/// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
/// 
/// /////////////////注意///////////////////////////////////////////////////////////////
/// 如果您在接口集成过程中遇到问题，
/// 您可以到商户服务中心（https://b.alipay.com/support/helperApply.htm?action=consultationApply），提交申请集成协助，我们会有专业的技术工程师主动联系您协助解决，
/// 您也可以到支付宝论坛（http://club.alipay.com/read-htm-tid-8681712.html）寻找相关解决方案
/// 
/// 如果不想使用扩展功能请把扩展功能参数赋空值。
/// 要传递的参数要么不允许为空，要么就不要出现在数组与隐藏控件或URL链接里。
/// </summary>
public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //基础配置参数
        AlipayConfig con = new AlipayConfig();
        string partner = con.Partner;
        string key = con.Key;
        string return_url = con.Return_url;
        string input_charset = con.Input_charset;
        string sign_type = con.Sign_type;

        //选填参数
        string email = "";      //会员通用登录时，会员的支付宝账号

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        //构造请求函数
        AlipayUserService aliService = new AlipayUserService(
            partner,
            return_url,
            email,
            key,
            input_charset,
            sign_type);

        string sHtmlText = aliService.Build_Form();

        //打印页面
        lbButton.Text = sHtmlText;
    }
}
