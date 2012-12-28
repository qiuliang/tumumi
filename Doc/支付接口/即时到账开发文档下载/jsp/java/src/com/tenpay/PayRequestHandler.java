package com.tenpay;

import java.text.SimpleDateFormat;
import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tenpay.util.MD5Util;
import com.tenpay.util.TenpayUtil;

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
public class PayRequestHandler extends RequestHandler {

	public PayRequestHandler(HttpServletRequest request,
			HttpServletResponse response) {
		
		super(request, response); 

		//支付网关地址
		this.setGateUrl("http://service.tenpay.com/cgi-bin/v3.0/payservice.cgi");
		
	}

	/**
	 * @Override
	 * 初始化函数，默认给一些参数赋值，如cmdno,date等。
	 */
	public void init() {

		Date now = new Date();
		SimpleDateFormat dfDay = new SimpleDateFormat("yyyyMMdd");
		String strDay = dfDay.format(now);
		
		//任务代码
		this.setParameter("cmdno", "1");
		
		//日期
		this.setParameter("date",  strDay);
		
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
		this.setParameter("cs", "gbk");
		
		//摘要
		this.setParameter("sign", "");
	}

	/**
	 * @Override
	 * 创建签名
	 */
	protected void createSign() {
		
		//获取参数
		String cmdno = this.getParameter("cmdno");
		String date = this.getParameter("date");
		String bargainor_id = this.getParameter("bargainor_id");
		String transaction_id = this.getParameter("transaction_id");
		String sp_billno = this.getParameter("sp_billno");
		String total_fee = this.getParameter("total_fee");
		String fee_type = this.getParameter("fee_type");
		String return_url = this.getParameter("return_url");
		String attach = this.getParameter("attach");
		String spbill_create_ip = this.getParameter("spbill_create_ip");
		String key = this.getKey();
		
		//组织签名
		StringBuffer sb = new StringBuffer();
		sb.append("cmdno=" + cmdno + "&");
		sb.append("date=" + date + "&");
		sb.append("bargainor_id=" + bargainor_id + "&");
		sb.append("transaction_id=" + transaction_id + "&");
		sb.append("sp_billno=" + sp_billno + "&");
		sb.append("total_fee=" + total_fee + "&");
		sb.append("fee_type=" + fee_type + "&");
		sb.append("return_url=" + return_url + "&");
		sb.append("attach=" + attach + "&");
		if(!"".equals(spbill_create_ip)) {
			sb.append("spbill_create_ip=" + spbill_create_ip + "&");
		}
		sb.append("key=" + key);
		
		String enc = TenpayUtil.getCharacterEncoding(
				this.getHttpServletRequest(), this.getHttpServletResponse());
		//算出摘要
		String sign = MD5Util.MD5Encode(sb.toString(), enc).toLowerCase();
				
		this.setParameter("sign", sign);
		
		//debug信息
		this.setDebugInfo(sb.toString() + " => sign:"  + sign);
		
	}
	
	
	
	
	

}
