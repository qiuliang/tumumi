package com.alipay.util;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Map;

import com.alipay.config.AlipayConfig;

/**
 *类名：alipay_notify
 *功能：支付宝服务器通知类
 *详细：该页面是请求参数核心处理文件，不需要修改
 *版本：3.1
 *修改日期：2010-10-26
 *说明：
  以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
  该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
 */

public class AlipayNotify {
	/**
	 * *功能：根据反馈回来的信息，生成签名结果
	 * @param Params 通知返回来的参数数组
	 * @param key 安全校验码
	 * @return 生成的签名结果
	 */
	public static String GetMysign(Map Params, String key){
		Map sParaNew = AlipayFunction.ParaFilter(Params);//过滤空值、sign与sign_type参数
		String mysign = AlipayFunction.BuildMysign(sParaNew, key);//获得签名结果
		
		return mysign;
	}
	
	/**
	* *功能：获取远程服务器ATN结果,验证返回URL
	* @param notify_id 通知校验ID
	* @return 服务器ATN结果
	* 验证结果集：
	* invalid命令参数不对 出现这个错误，请检测返回处理中partner和key是否为空 
	* true 返回正确信息
	* false 请检查防火墙或者是服务器阻止端口问题以及验证时间是否超过一分钟
	*/
	public static String Verify(String notify_id){
		//获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
		String transport = AlipayConfig.transport;
		String partner = AlipayConfig.partnerID;
		String veryfy_url = "";
		if(transport.equalsIgnoreCase("https")){
			veryfy_url = "https://www.alipay.com/cooperate/gateway.do?service=notify_verify";
		} else{
			veryfy_url = "http://notify.alipay.com/trade/notify_query.do?";
		}
		veryfy_url = veryfy_url + "&partner=" + partner + "&notify_id=" + notify_id;
		
		String responseTxt = CheckUrl(veryfy_url);
		
		return responseTxt;
	}
	
	/**
	* *功能：获取远程服务器ATN结果
	* @param urlvalue 指定URL路径地址
	* @return 服务器ATN结果
	* 验证结果集：
	* invalid命令参数不对 出现这个错误，请检测返回处理中partner和key是否为空 
	* true 返回正确信息
	* false 请检查防火墙或者是服务器阻止端口问题以及验证时间是否超过一分钟
	*/
	public static String CheckUrl(String urlvalue){
		String inputLine = "";

		try {
			URL url = new URL(urlvalue);
			HttpURLConnection urlConnection = (HttpURLConnection) url
					.openConnection();
			BufferedReader in = new BufferedReader(new InputStreamReader(
					urlConnection.getInputStream()));
			inputLine = in.readLine().toString();
		} catch (Exception e) {
			e.printStackTrace();
		}

		return inputLine;
	}
}
