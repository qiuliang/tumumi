package com.tenpay;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tenpay.util.MD5Util;
import com.tenpay.util.TenpayUtil;

/**
 * 即时到帐应答类
 * ============================================================================
 * api说明：
 * getKey()/setKey(),获取/设置密钥
 * getParameter()/setParameter(),获取/设置参数值
 * getAllParameters(),获取所有参数
 * isTenpaySign(),是否财付通签名,true:是 false:否
 * doShow(),显示处理结果
 * getDebugInfo(),获取debug信息
 * 
 * ============================================================================
 *
 */
public class PayResponseHandler extends ResponseHandler {

	public PayResponseHandler(HttpServletRequest request,
			HttpServletResponse response) {
		
		super(request, response);
		
	}

	/**
	 * 是否财付通签名
	 * @Override
	 * @return boolean
	 */
	public boolean isTenpaySign() {
		
		//获取参数
		String cmdno = this.getParameter("cmdno");
		String pay_result = this.getParameter("pay_result");
		String date = this.getParameter("date");
		String transaction_id = this.getParameter("transaction_id");
		String sp_billno = this.getParameter("sp_billno");
		String total_fee = this.getParameter("total_fee");		
		String fee_type = this.getParameter("fee_type");
		String attach = this.getParameter("attach");
		String key = this.getKey();
		String tenpaySign = this.getParameter("sign").toLowerCase();
		
		//组织签名串
		StringBuffer sb = new StringBuffer();
		sb.append("cmdno=" + cmdno + "&");
		sb.append("pay_result=" + pay_result + "&");
		sb.append("date=" + date + "&");
		sb.append("transaction_id=" + transaction_id + "&");
		sb.append("sp_billno=" + sp_billno + "&");
		sb.append("total_fee=" + total_fee + "&");
		sb.append("fee_type=" + fee_type + "&");
		sb.append("attach=" + attach + "&");
		sb.append("key=" + key);
		
		String enc = TenpayUtil.getCharacterEncoding(
				this.getHttpServletRequest(), this.getHttpServletResponse());
		//算出摘要
		String sign = MD5Util.MD5Encode(sb.toString(), enc).toLowerCase();
		
		//debug信息
		this.setDebugInfo(sb.toString() + " => sign:" + sign +
				" tenpaySign:" + tenpaySign);
		
		return tenpaySign.equals(sign);
	} 
	
}
