using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AlipayClass;

/// <summary>
/// 功能：快速付款入口模板页
/// 详细：该页面是针对不涉及到购物车流程、充值流程等业务流程，只需要实现买家能够快速付款给卖家的付款功能。
/// 版本：3.1
/// 日期：2010-10-29
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
    protected string show_url = "";
    protected string mainname = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        AlipayConfig con = new AlipayConfig();
        show_url = con.Show_url;
        mainname = con.Mainname;
    }
}
