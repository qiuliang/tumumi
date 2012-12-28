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
public partial class index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //创建工具对象，读取配置
        TenpayUtil util= new TenpayUtil();
        if (!IsPostBack)
        {

            //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            string strReq = DateTime.Now.ToString("HHmmss") + TenpayUtil.BuildRandomStr(4);

            //当前时间 yyyyMMdd
            string date = DateTime.Now.ToString("yyyyMMdd");
 
            //商户订单号，不超过32位，财付通只做记录，不保证唯一性
            string sp_billno = date + strReq;
            lbl_Name.Text = TenpayUtil.bargainor_id;
            order_no.Text = sp_billno;
        }

    }
    protected void ibtn_Next_Click(object sender, ImageClickEventArgs e)
    {
        string url = string.Format(
            "tenpay.aspx?order_no={0}&product_name={1}&order_price={2}&remarkexplain={3}",
            this.order_no.Text,
            Server.UrlEncode(this.product_name.Text),
            Server.UrlEncode(this.order_price.Text),
            Server.UrlEncode(this.remarkexplain.Text)
            );
        Response.Redirect(url);
    }
}
