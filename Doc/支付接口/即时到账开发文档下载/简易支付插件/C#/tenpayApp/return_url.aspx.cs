using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using tenpayApp;
public partial class return_url : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //创建PayResponseHandler实例
        PayResponseHandler resHandler = new PayResponseHandler(Context);

        resHandler.setKey(TenpayUtil.tenpay_key);

        //判断签名
        if (resHandler.isTenpaySign())
        {
            //交易单号
            string transaction_id = resHandler.getParameter("transaction_id");

            //金额金额,以分为单位
            string total_fee = resHandler.getParameter("total_fee");

            //支付结果
            string pay_result = resHandler.getParameter("pay_result");

            if ("0".Equals(pay_result))
            {
                //------------------------------
                //处理业务开始
                //------------------------------ 

                //注意交易单不要重复处理
                //注意判断返回金额

                //------------------------------
                //处理业务完毕
                //------------------------------

                //调用doShow, 打印meta值跟js代码,告诉财付通处理成功,并在用户浏览器显示$show页面.
                resHandler.doShow(TenpayUtil.tenpay_show);
            }
            else
            {
                //当做不成功处理
                Response.Write("支付失败");
            }

        }
        else
        {
            Response.Write("认证签名失败");
            //string debugInfo = resHandler.getDebugInfo();
            //Response.Write("<br/>debugInfo:" + debugInfo);
        }
    }
}
