using System;
using System.Text;
using System.Web;
using System.Web.UI;

namespace tenpayApp
{
	/// <summary>
	/// PayRequestHandler 的摘要说明。
	/// </summary>
	/**
	* 即时到帐请求类
	* ============================================================================
	* api说明：
	* init(),初始化函数，默认给一些参数赋值，如cmdno,date等。
	* getGateURL()/setGateURL(),获取/设置入口地址,不包含参数值
	* getKey()/setKey(),获取/设置密钥
	* getParameter()/setParameter(),获取/设置参数值
	* getAllParameters(),获取所有参数
	* getRequestURL(),获取带参数的请求URL
	* doSend(),重定向到财付通支付
	* getDebugInfo(),获取debug信息
	* 
	* ============================================================================
	*
	*/
	public class PayRequestHandler:RequestHandler
	{
		public PayRequestHandler(HttpContext httpContext) : base(httpContext)
		{
			
			this.setGateUrl("http://service.tenpay.com/cgi-bin/v3.0/payservice.cgi");
		}


		/**
			* @Override
			* 初始化函数，默认给一些参数赋值，如cmdno,date等。
		*/
		public override void init() 
		{

			//任务代码
			this.setParameter("cmdno", "1");
		
			//日期
			this.setParameter("date",DateTime.Now.ToString("yyyyMMdd"));
		
			//商户号
			this.setParameter("bargainor_id", "");
		
			//财付通交易单号
			this.setParameter("transaction_id", "");
		
			//商家订单号
			this.setParameter("sp_billno", "");
		
			//商品价格，以分为单位
			this.setParameter("total_fee", "");
		
			//货币类型
			this.setParameter("fee_type",  "1");
		
			//返回url
			this.setParameter("return_url",  "");
		
			//自定义参数
			this.setParameter("attach",  "");
		
			//用户ip
			this.setParameter("spbill_create_ip",  "");
		
			//商品名称
			this.setParameter("desc",  "");
		
			//银行编码
			this.setParameter("bank_type",  "0");
		
			//字符集编码
			this.setParameter("cs","gb2312");
		
			//摘要
			this.setParameter("sign", "");
		}



		/**
	 * @Override
	 * 创建签名
	 */
		protected override void createSign() 
		{
		
			//获取参数
			string cmdno = getParameter("cmdno");
			string date = getParameter("date");
			string bargainor_id = getParameter("bargainor_id");
			string transaction_id = getParameter("transaction_id");
			string sp_billno = getParameter("sp_billno");
			string total_fee = getParameter("total_fee");
			string fee_type = getParameter("fee_type");
			string return_url = getParameter("return_url");
			string attach = getParameter("attach");
			string spbill_create_ip = getParameter("spbill_create_ip");
			string key = getParameter("key");
		
			//组织签名
			StringBuilder sb = new StringBuilder();
			sb.Append("cmdno=" + cmdno + "&");
			sb.Append("date=" + date + "&");
			sb.Append("bargainor_id=" + bargainor_id + "&");
			sb.Append("transaction_id=" + transaction_id + "&");
			sb.Append("sp_billno=" + sp_billno + "&");
			sb.Append("total_fee=" + total_fee + "&");
			sb.Append("fee_type=" + fee_type + "&");
			sb.Append("return_url=" + return_url + "&");
			sb.Append("attach=" + attach + "&");
			if( !"".Equals(spbill_create_ip) ) 
			{
				sb.Append("spbill_create_ip=" + spbill_create_ip + "&");
			}
			sb.Append("key=" + getKey());
		
			//算出摘要
			string sign = MD5Util.GetMD5(sb.ToString(),getCharset());
				
			setParameter("sign", sign);
	
			//debug信息
			setDebugInfo(sb.ToString() + " => sign:"  + sign);
		
		}

	}
}
