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
public partial class tenpay : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sp_billno = Request["order_no"];
        string product_name = Request["product_name"];
        string order_price  = Request["order_price"];;
        string remarkexplain= Request["remarkexplain"];
        double money = 0;
        if (null == Request["order_price"])
        {
            Response.End();
            return;
        }
        try
        {
            money = Convert.ToDouble(order_price);
        }
        catch
        {
            //Response.Write("支付金额格式错误！");
            //Response.End();
            //return;
        }
        if (null == sp_billno)
        {
            //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            sp_billno = DateTime.Now.ToString("HHmmss") + TenpayUtil.BuildRandomStr(4);
        }
        else
        {
            sp_billno = Request["order_no"].ToString();
        }


        //财付通订单号，10位商户号+8位日期+10位序列号，需保证全局唯一
        string transaction_id = TenpayUtil.bargainor_id + sp_billno;

        //创建PayRequestHandler实例
        PayRequestHandler reqHandler = new PayRequestHandler(Context);

        //设置密钥
        reqHandler.setKey(TenpayUtil.tenpay_key);

        //初始化
        reqHandler.init();

        //-----------------------------
        //设置支付参数
        //-----------------------------
        reqHandler.setParameter("bargainor_id", TenpayUtil.bargainor_id);			//商户号
        reqHandler.setParameter("sp_billno", sp_billno);				//商家订单号
        reqHandler.setParameter("transaction_id", transaction_id);		//财付通交易单号
        reqHandler.setParameter("return_url", TenpayUtil.tenpay_return);				//支付通知url
        reqHandler.setParameter("desc", "订单号：" + transaction_id);	//商品名称
        reqHandler.setParameter("total_fee", (money * 100).ToString());						//商品金额,以分为单位

        reqHandler.setParameter("cs", "UTF-8");
        reqHandler.setParameter("spbill_create_ip", Request.ServerVariables["REMOTE_ADDR"]);

        //用户ip,测试环境时不要加这个ip参数，正式环境再加此参数
        //reqHandler.setParameter("spbill_create_ip",Page.Request.UserHostAddress);

        //获取请求带参数的url
        string requestUrl = reqHandler.getRequestURL();

        this.hl_main.NavigateUrl = requestUrl;


        //post实现方式
        /*
        reqHandler.getRequestURL();
        Response.Write("<form method=\"post\" action=\""+ reqHandler.getGateUrl() + "\" >\n");
        Hashtable ht = reqHandler.getAllParameters();
        foreach(DictionaryEntry de in ht) 
        {
            Response.Write("<input type=\"hidden\" name=\"" + de.Key + "\" value=\"" + de.Value + "\" >\n");
        }
        Response.Write("<input type=\"submit\" value=\"财付通支付\" >\n</form>\n");
        */

        //获取debug信息
        //string debuginfo = reqHandler.getDebugInfo();
        //Response.Write("<br/>" + debuginfo + "<br/>");

        //重定向到财付通支付
        //reqHandler.doSend();
    }
}
