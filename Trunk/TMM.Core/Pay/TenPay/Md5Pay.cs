using System;
using System.Text;
using System.Security.Cryptography;
using  System.Collections.Specialized;
using System.Configuration;

namespace TMM.Core.Pay
{
	/// <summary>
	/// 完成功能如下
	/// 1:支付请求
	/// 2:支付结果处理。
	/// 3:查询订单请求.
	/// 4:查询订单结果处理.
	/// </summary>
	public class Md5Pay
	{
        /// <summary>
        /// 商户号（替换为自已的商户号）
        /// </summary>
        private string bargainor_id = "1209903101";// "1205322801";
        public string Bargainor_id
        {
            get { return bargainor_id; }
            set { bargainor_id = value; }
        }

        /// <summary>
        /// 商户KEY（替换为自已的KEY）
        /// </summary>
        private string key = "0540a658ace2b2749af55fdfab3c9782";// "123wsx456rfv789tgb654yhn2e23w3ed";
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

		/// <summary>
		/// 财付通支付网关URL
		/// </summary>
		private string paygateurl ="https://www.tenpay.com/cgi-bin/v1.0/pay_gate.cgi";
		public string Paygateurl
		{
			get{return paygateurl;}
			set{paygateurl = value;}
		}
	
		/// <summary>
		/// 财付通查询请求URL
		/// </summary>
		private string querygateurl = "http://portal.tenpay.com/cfbiportal/cgi-bin/cfbiqueryorder.cgi";
		public string Querygateurl
		{
			get{return querygateurl;}
			set{querygateurl = value;}
		}

		/// <summary>
		/// 支付结果回跳页面
		/// 推荐使用ip地址的方式(最长255个字符)
		/// 可以使用相对地址或配置,在使用前拼装全地址就行.这样方便部署.
		/// </summary>
		private string return_url = "";
		public string Return_url
		{
			get{return return_url;}
			set{return_url = value;}
		}

		/// <summary>
		/// 查询结果回跳页面
		/// 推荐使用ip地址的方式(最长255个字符)
		/// 可以使用相对地址或配置,在使用前拼装全地址就行.这样方便部署.
		/// </summary>
		private string queryreturn_url ="http://tuan.0-6.com/pay/Notify.do";
		public string Queryreturn_url
		{
			get{return queryreturn_url;}
			set{queryreturn_url = value;}
		}

		/// <summary>
		/// 支付命令.1
		/// </summary>
		private const int cmdno = 1;
		
		/// <summary>
		/// 查询命令.2
		/// </summary>
		private const int querycmdno = 2;

		/// <summary>
		/// 费用类型,现在暂只支持 1:人民币
		/// </summary>
		private int fee_type = 1;

		private string date;

		#region 日期字段设置,格式为yyyyMMdd		

		/// <summary>
		/// 支付日期,yyyyMMdd
		/// </summary>
		public string Date
		{
			get
			{
				if(date == null)
				{
					date = DateTime.Now.ToString("yyyyMMdd");					
				}

				return date;
			}
			set
			{
				if(value == null || value.Trim().Length != 8)
				{
					date = DateTime.Now.ToString("yyyyMMdd");
				}
				else
				{
					try
					{
						string strTmp = value.Trim();
						date = DateTime.Parse(strTmp.Substring(0,4) + "-" + strTmp.Substring(4,2) + "-" 
							+ strTmp.Substring(6,2)).ToString("yyyyMMdd");
					}
					catch
					{
						date = DateTime.Now.ToString("yyyyMMdd");
					}

				}
			}
		}

		#endregion

		private string sp_billno = "";
		/// <summary>
		/// 商户订单号,10位正整数
		/// </summary>
		public string Sp_billno
		{
			get{return sp_billno;}
			set{sp_billno = value;}
		}

		private long total_fee = 0;

		/// <summary>
		/// 订单金额,以分为单位
		/// </summary>
		public long Total_fee
		{
			get{return total_fee;}
			set{total_fee = value;}
		}


		/// <summary>
		/// 取时间戳生成随即数,替换交易单号中的后10位流水号
		/// 财付通的交易单号中不允许出现非数字的字符
		/// </summary>
		/// <returns></returns>
		public UInt32 UnixStamp()
		{
			TimeSpan ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			return Convert.ToUInt32(ts.TotalSeconds);
		}

		private string transaction_id ="";
		/// <summary>
		/// 交易单号,商户号(10)+支付日期(8)+商户订单号(10,不足的话左补0)=28位.
		/// </summary>
		public string Transaction_id
		{
			get
			{
				return transaction_id;
			}
			set
			{
				transaction_id = value;
			}
		}

		private string desc = "";
		/// <summary>
		/// 商品名称
		/// </summary>
		public string Desc
		{
            get
            {
                return
                System.Web.HttpUtility.UrlDecode(desc, System.Text.Encoding.GetEncoding("GB2312"));
               // UrlDecode(desc);
            }
			set{
                desc = System.Web.HttpUtility.UrlEncode(value, System.Text.Encoding.GetEncoding("GB2312")); ;//UrlEncode(value);
            
            }
		}

		private string attach = "";
		/// <summary>
		/// 指令标识,每次指令都会有这个字段,财付通在处理完成后会原样返回.
		/// </summary>
		public string Attach
		{
			get{return UrlDecode(attach);}
			set{attach = UrlEncode(value);}
		}
		
		private string purchaser_id = "";
		/// <summary>
		/// 买家财付通帐号
		/// </summary>
		public string Purchaser_id
		{
			get{return purchaser_id;}
			set{purchaser_id=value;}
		}

		private string spbill_create_ip= "";
		/// <summary>
		/// 用户ip
		/// </summary>
		public string Spbill_create_ip
		{
			get{return spbill_create_ip;}
			set{spbill_create_ip = value;}
		}

		private int pay_result;
		/// <summary>
		///  支付結果
		/// </summary>
		public const int PAYOK = 0;
		public const int PAYSPERROR = 1;
		public const int PAYMD5ERROR = 2;
		public const int PAYERROR = 3;
		/// <summary>
		/// 支付结果 
		/// 0:支付成功.
		/// 1:商户号错.
		/// 2:签名错误.
		/// 3:支付失败.
		/// </summary>
		public int Pay_Result
		{
			get{return pay_result;}
		}


		/// <summary>
		/// 支付结果说明字段
		/// </summary>
		public string PayResultStr
		{
			get
			{
				switch(pay_result)
				{
					case PAYOK :
						return "支付成功";
					case PAYSPERROR :
						return "商户号错";
					case PAYMD5ERROR :
						return "签名错误";
					case PAYERROR :
						return "支付失败";
					default :
                        return "未知类型(" + pay_result +")";
				}
			}
		}

		private string payerrmsg = "";

		/// <summary>
		/// 如果为支付失败时,财付通返回的错误信息
		/// </summary>
		public string PayErrMsg
		{
			get{return payerrmsg;}
		}

		/// <summary>
		/// 对字符串进行URL编码
		/// </summary>
		/// <param name="instr">待编码的字符串</param>
		/// <returns>编码结果</returns>
		private static string UrlEncode(string instr)
		{
			if(instr == null || instr.Trim() == "")
				return "";
			else
			{
				return instr.Replace("%","%25").Replace("=","%3d").Replace("&","%26").
					Replace("\"","%22").Replace("?","%3f").Replace("'","%27").Replace(" ","%20");
			}
		}

		/// <summary>
		/// 对字符串进行URL解码
		/// </summary>
		/// <param name="instr">待解码的字符串</param>
		/// <returns>解码结果</returns>
		private static string UrlDecode(string instr)
		{
			if(instr == null || instr.Trim() == "")
				return "";
			else
			{
				return instr.Replace("%3d","=").Replace("%26","&").Replace("%22","\"").Replace("%3f","?")
					.Replace("%27","'").Replace("%20"," ").Replace("%25","%");
			}
		}

		/// <summary>
		/// 获取大写的MD5签名结果
		/// </summary>
		/// <param name="encypStr"></param>
		/// <returns></returns>
		private static string GetMD5(string encypStr)
		{
			string retStr;
			MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

			//创建md5对象
			byte[] inputBye;
			byte[] outputBye;

			//使用GB2312编码方式把字符串转化为字节数组．
			inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
			
			outputBye = m5.ComputeHash(inputBye);

			retStr = System.BitConverter.ToString(outputBye);
			retStr = retStr.Replace("-", "").ToUpper();
			return retStr;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Md5Pay()
		{

		}
 


		/// <summary>
		/// 获取支付签名
		/// </summary>
		/// <returns>根据参数得到签名</returns>
		private string GetPaySign()
		{
			string sign_text = "cmdno=" + cmdno + "&date=" + Date + "&bargainor_id=" + bargainor_id 
				+ "&transaction_id=" + Transaction_id + "&sp_billno=" + sp_billno +  "&total_fee=" 
				+ total_fee + "&fee_type=" + fee_type + "&return_url=" + return_url +  "&attach=" + Attach;
			if(spbill_create_ip!="")
			{
				sign_text += "&spbill_create_ip=" + spbill_create_ip;
			}
			sign_text += "&key=" + key;

			return GetMD5(sign_text);
		}

		/// <summary>
		/// 获取支付结果签名
		/// </summary>
		/// <returns>根据参数得到签名</returns>
		private string GetPayResultSign()
		{
			string sign_text = "cmdno=" + cmdno + "&pay_result=" + pay_result + "&date=" + date + "&transaction_id=" + transaction_id 
				+ "&sp_billno=" + sp_billno + "&total_fee=" + total_fee + "&fee_type=" + fee_type + "&attach=" + attach + "&key=" + key;
           // System.Web.HttpContext.Current.Response.Write(sign_text);
			return GetMD5(sign_text);
		}


		/// <summary>
		/// 获取支付页面URL
		/// </summary>
		/// <param name="url">如果函数返回真,是支付URL,如果函数返回假,是错误信息</param>
		/// <returns>函数执行是否成功</returns>
		public bool GetPayUrl(out string url)
		{
			try
			{				
				string sign = GetPaySign();
				
				url = paygateurl +"?cmdno=" + cmdno + "&date=" + date + "&bank_type=0&desc="+ desc +"&purchaser_id=&bargainor_id=" 
					+ bargainor_id + "&transaction_id=" + transaction_id + "&sp_billno=" + sp_billno + "&total_fee=" + total_fee
					+ "&fee_type=" + fee_type + "&return_url=" + return_url + "&attach=" + attach;
				if(spbill_create_ip!="")
				{
					url += "&spbill_create_ip=" + spbill_create_ip;
				}
				url += "&sign=" + sign;

				return true;
			}
			catch(Exception err)
			{
				url = "创建URL时出错,错误信息:" + err.Message;
				return false;
			}
		}

		/// <summary>
		/// 从支付结果页面的URL请求参数中获取结果信息
		/// </summary>
		/// <param name="querystring">支付结果页面的URL请求参数</param>
		/// <param name="errmsg">函数执行不成功的话,返回错误信息</param>
		/// <returns>函数执行是否成功</returns>
		public bool GetPayValueFromUrl(NameValueCollection querystring, out string errmsg)
		{
			//结果URL参数样例如下
/*
?cmdno=1&pay_result=0&pay_info=OK&date=20070423&bargainor_id=1201143001&transaction_id=1201143001200704230000000013
&sp_billno=13&total_fee=1&fee_type=1&attach=%D5%E2%CA%C7%D2%BB%B8%F6%B2%E2%CA%D4%BD%BB%D2%D7%B5%A5				
&sign=ADD7475F2CAFA793A3FB35051869E301
*/

			#region 进行参数校验

			if(querystring == null || querystring.Count == 0)
			{
				errmsg = "参数为空";
				return false;
			}

			if(querystring["cmdno"] == null || querystring["cmdno"].ToString().Trim() != cmdno.ToString())
			{
				errmsg = "没有cmdno参数或cmdno参数不正确";
				return false;
			}

			if(querystring["pay_result"] == null)
			{
				errmsg = "没有pay_result参数";
				return false;
			}

			if(querystring["date"] == null)
			{
				errmsg = "没有date参数";
				return false;
			}

			if(querystring["pay_info"] == null)
			{
				errmsg = "没有pay_info参数";
				return false;
			}

			if(querystring["bargainor_id"] == null)
			{
				errmsg = "没有bargainor_id参数";
				return false;
			}

			if(querystring["transaction_id"] == null)
			{
				errmsg = "没有transaction_id参数";
				return false;
			}

			if(querystring["sp_billno"] == null)
			{
				errmsg = "没有sp_billno参数";
				return false;
			}

			if(querystring["total_fee"] == null)
			{
				errmsg = "没有total_fee参数";
				return false;
			}

			if(querystring["fee_type"] == null)
			{
				errmsg = "没有fee_type参数";
				return false;
			}

			if(querystring["attach"] == null)
			{
				errmsg = "没有attach参数";
				return false;
			}

			if(querystring["sign"] == null)
			{
				errmsg = "没有sign参数";
				return false;
			}

			#endregion

			errmsg = "";

			try
			{
				pay_result = Int32.Parse(querystring["pay_result"].Trim());
                
				payerrmsg = UrlDecode(querystring["pay_info"].Trim());
				Date = querystring["date"];
				transaction_id = querystring["transaction_id"];
				sp_billno = querystring["sp_billno"];
				total_fee = long.Parse(querystring["total_fee"]);
				fee_type = Int32.Parse(querystring["fee_type"]);
				attach = querystring["attach"];
                
				if(querystring["bargainor_id"] != bargainor_id)
				{
					pay_result = PAYSPERROR;
					return true;
				}

				string strsign = querystring["sign"];
				string sign = GetPayResultSign();
               
				if(sign != strsign)
				{
					pay_result = PAYMD5ERROR;
				}
				
				return true;
			}
			catch(Exception err)
			{
				errmsg = "解析参数出错:" + err.Message;
				return false;
			}
		}


		/// <summary>
		/// 查询请求设置参数函数
		/// </summary>
		/// <param name="adate">支付日期</param>
		/// <param name="atransaction_id">交易单号</param>
		/// <param name="asp_billno">商户订单号</param>
		/// <param name="aattach">指令标识数据</param>
		public void InitQueryParam(string adate, string atransaction_id, string asp_billno,string aattach)
		{
			Date = adate;
			Sp_billno = asp_billno;
			Transaction_id = atransaction_id;
			Attach = aattach;
		}

		/// <summary>
		/// 获取查询签名
		/// </summary>
		/// <returns>根据参数得到签名</returns>
		private string GetQuerySign()
		{
			string sign_text = "cmdno=" + querycmdno + "&date=" + date + "&bargainor_id=" + bargainor_id + "&transaction_id=" 
				+ transaction_id + "&sp_billno=" + sp_billno + "&return_url=" + queryreturn_url + "&attach=" + Attach + "&key=" + key;
            
			return GetMD5(sign_text);
		}

		/// <summary>
		/// 获取查询结果签名
		/// </summary>
		/// <returns>根据参数得到签名</returns>
		private string GetQueryResultSign()
		{
			string sign_text = "cmdno=" + querycmdno + "&pay_result=" + pay_result + "&date=" + date + "&transaction_id=" + transaction_id 
				+ "&sp_billno=" + sp_billno + "&total_fee=" + total_fee + "&fee_type=" + fee_type + "&attach=" + attach + "&key=" + key;
           
			return GetMD5(sign_text);
		}

		/// <summary>
		/// 获取查询页面URL
		/// </summary>
		/// <param name="url">如果函数返回真,是查询URL,如果函数返回假,是错误信息</param>
		/// <returns>函数执行是否成功</returns>
		public bool GetQueryUrl(out string url)
		{

			try
			{				
				string sign = GetQuerySign();
				
				url = querygateurl +"?cmdno=" + querycmdno + "&date=" + date + "&bargainor_id=" + bargainor_id + "&transaction_id=" 
					+ transaction_id + "&sp_billno=" + sp_billno + "&return_url=" + queryreturn_url + "&attach=" + attach + "&sign=" + sign;

				return true;
			}
			catch(Exception err)
			{
				url = "创建URL时出错,错误信息:" + err.Message;
				return false;
			}
		}

		/// <summary>
		/// 从查询结果页面的URL请求参数中获取结果信息
		/// </summary>
		/// <param name="querystring">查询结果页面的URL请求参数</param>
		/// <param name="errmsg">函数执行不成功的话,返回错误信息</param>
		/// <returns>函数执行是否成功</returns>
		public bool GetQueryValueFromUrl(NameValueCollection querystring, out string errmsg)
		{
			//结果URL参数样例如下
			/*
			?cmdno=2&pay_result=0&pay_info=OK&date=20070423&bargainor_id=1201143001&transaction_id=1201143001200704230000000001
			&sp_billno=1&total_fee=1&fee_type=1&attach=test11&sign=E80632F587263EF0AFA4A8EEC84A467C&PcacheTime=353851
			*/

			#region 进行参数校验

			if(querystring == null || querystring.Count == 0)
			{
				errmsg = "参数为空";
				return false;
			}

			if(querystring["cmdno"] == null || querystring["cmdno"].ToString().Trim() != querycmdno.ToString())
			{
				errmsg = "没有cmdno参数或cmdno参数不正确";
				return false;
			}

			if(querystring["pay_result"] == null)
			{
				errmsg = "没有pay_result参数";
				return false;
			}

			if(querystring["date"] == null)
			{
				errmsg = "没有date参数";
				return false;
			}

			if(querystring["pay_info"] == null)
			{
				errmsg = "没有pay_info参数";
				return false;
			}

			if(querystring["bargainor_id"] == null)
			{
				errmsg = "没有bargainor_id参数";
				return false;
			}

			if(querystring["transaction_id"] == null)
			{
				errmsg = "没有transaction_id参数";
				return false;
			}

			if(querystring["sp_billno"] == null)
			{
				errmsg = "没有sp_billno参数";
				return false;
			}

			if(querystring["total_fee"] == null)
			{
				errmsg = "没有total_fee参数";
				return false;
			}

			if(querystring["fee_type"] == null)
			{
				errmsg = "没有fee_type参数";
				return false;
			}

			if(querystring["attach"] == null)
			{
				errmsg = "没有attach参数";
				return false;
			}

			if(querystring["sign"] == null)
			{
				errmsg = "没有sign参数";
				return false;
			}

			#endregion

			errmsg = "";

			try
			{
				pay_result = Int32.Parse(querystring["pay_result"].Trim());
                
				payerrmsg = UrlDecode(querystring["pay_info"].Trim());
				Date = querystring["date"];
				transaction_id = querystring["transaction_id"];
				sp_billno = querystring["sp_billno"];
				total_fee = long.Parse(querystring["total_fee"]);
				fee_type = Int32.Parse(querystring["fee_type"]);
				attach = querystring["attach"];

				if(querystring["bargainor_id"] != bargainor_id)
				{
					pay_result = PAYSPERROR;
					return true;
				}

				string strsign = querystring["sign"];
				string sign = GetQueryResultSign();

				if(sign != strsign)
				{
					pay_result = PAYMD5ERROR;
				}
				
				return true;
			}
			catch(Exception err)
			{
				errmsg = "解析参数出错:" + err.Message;
				return false;
			}
		}
	}
}
