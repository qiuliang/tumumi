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
using System.Collections;
using AlipayClass;

/// <summary>
/// 功能：设置商品有关信息（入口页）
/// 详细：该页面是接口入口页面，生成支付时的URL
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
public partial class alipayto : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ///////////////////////以下参数是需要设置的相关配置参数，设置后不会更改的///////////////////////////
        AlipayConfig con = new AlipayConfig();
        string partner = con.Partner;
        string key = con.Key;
        string seller_email = con.Seller_email;
        string input_charset = con.Input_charset;
        string notify_url = con.Notify_url;
        string return_url = con.Return_url;
        string show_url = con.Show_url;
        string sign_type = con.Sign_type;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////以下参数是需要通过下单时的订单数据传入进来获得////////////////////////////////
        //必填参数
        string out_trade_no = DateTime.Now.ToString("yyyyMMddHHmmss");  //请与贵网站订单系统中的唯一订单号匹配
        string subject = Request.Form["aliorder"];                      //订单名称，显示在支付宝收银台里的“商品名称”里，显示在支付宝的交易管理的“商品名称”的列表里。
        string body = Request.Form["alibody"];                          //订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
        string total_fee = Request.Form["alimoney"];                    //订单总金额，显示在支付宝收银台里的“应付总额”里

        //扩展功能参数——默认支付方式
        string paymethod = "";                                          //默认支付方式，四个值可选：bankPay(网银); cartoon(卡通); directPay(余额); CASH(网点支付)，初始值
        string defaultbank = "";                                        //默认网银代号，代号列表见http://club.alipay.com/read.php?tid=8681379 初始值
        string pay_mode = Request.Form["pay_bank"];
        if (pay_mode == "directPay")
        {
            paymethod = "directPay";
        }
        else
        {
            paymethod = "bankPay";
            defaultbank = pay_mode;
        }


        //扩展功能参数——防钓鱼
        //请慎重选择是否开启防钓鱼功能
        //exter_invoke_ip、anti_phishing_key一旦被设置过，那么它们就会成为必填参数
        //建议使用POST方式请求数据
        string anti_phishing_key = "";                                  //防钓鱼时间戳
        string exter_invoke_ip = "";                                    //获取客户端的IP地址，建议：编写获取客户端IP地址的程序
        //如：
        //exter_invoke_ip = "";
        //anti_phishing_key = AlipayFunction.Query_timestamp(partner);  //获取防钓鱼时间戳函数

        //扩展功能参数——其他
        string extra_common_param = "";                                 //自定义参数，可存放任何内容（除=、&等特殊字符外），不会显示在页面上
        string buyer_email = "";			                            //默认买家支付宝账号

        //扩展功能参数——分润(若要使用，请按照注释要求的格式赋值)
        string royalty_type = "";                                   //提成类型，该值为固定值：10，不需要修改
        string royalty_parameters = "";
        //提成信息集，与需要结合商户网站自身情况动态获取每笔交易的各分润收款账号、各分润金额、各分润说明。最多只能设置10条
        //各分润金额的总和须小于等于total_fee
        //提成信息集格式为：收款方Email_1^金额1^备注1|收款方Email_2^金额2^备注2
        //如：
        //royalty_type = "10";
        //royalty_parameters = "111@126.com^0.01^分润备注一|222@126.com^0.01^分润备注二";

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        //构造请求函数，无需修改
        AlipayService aliService = new AlipayService(partner, seller_email, return_url, notify_url, show_url, out_trade_no, subject, body, total_fee, paymethod, defaultbank, anti_phishing_key, exter_invoke_ip, extra_common_param, buyer_email, royalty_type, royalty_parameters, key, input_charset, sign_type);
        string sHtmlText = aliService.Build_Form();

        //打印页面
        lbOut_trade_no.Text = out_trade_no;
        lbTotal_fee.Text = total_fee;
        lbButton.Text = sHtmlText;
    }
}
