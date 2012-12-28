<%
/* *
 功能：支付宝会员登录完成后跳转返回的页面（返回页）
 版本：3.1
 日期：2010-10-26
 说明：
 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。

 //***********页面功能说明***********
 该页面可在本机电脑测试
 该页面称作“返回页”，是由支付宝服务器同步调用
 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
 建议：
 在商户网站会员数据库中增加一个字段：user_id（支付宝用户唯一ID），
 若返回的信息不止有参数user_id，那么再增加支付宝会员信息的数据表。
 会员信息的数据表中的唯一ID号是商户网站会员数据表中的
 //********************************
 * */
%>
<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.util.*"%>
<%@ page import="com.alipay.util.*"%>
<%@ page import="com.alipay.config.*"%>
<html>
  <head>
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
		<title>支付宝会员免注册登录返回信息</title>
<style type="text/css">
.font_content{
    font-family:"宋体";
    font-size:14px;
    color:#FF6600;
}
.font_title{
    font-family:"宋体";
    font-size:16px;
    color:#FF0000;
    font-weight:bold;
}
table{
    border: 1px solid #CCCCCC;
}
</style>
  </head>
  <body>
<%
	String key = AlipayConfig.key;
	//获取支付宝GET过来反馈信息
	Map params = new HashMap();
	Map requestParams = request.getParameterMap();
	for (Iterator iter = requestParams.keySet().iterator(); iter.hasNext();) {
		String name = (String) iter.next();
		String[] values = (String[]) requestParams.get(name);
		String valueStr = "";
		for (int i = 0; i < values.length; i++) {
			valueStr = (i == values.length - 1) ? valueStr + values[i]
					: valueStr + values[i] + ",";
		}
		//乱码解决，这段代码在出现乱码时使用。如果mysign和sign不相等也可以使用这段代码转化
		valueStr = new String(valueStr.getBytes("ISO-8859-1"), "UTF-8");
		params.put(name, valueStr);
	}
	
	//判断responsetTxt是否为ture，生成的签名结果mysign与获得的签名结果sign是否一致
	//responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
	//mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
	String mysign = AlipayNotify.GetMysign(params,key);
	String responseTxt = AlipayNotify.Verify(request.getParameter("notify_id"));
	String sign = request.getParameter("sign");
	
	//写日志记录（若要调试，请取消下面两行注释）
	//String sWord = "responseTxt=" + responseTxt + "\n return_url_log:sign=" + sign + "&mysign=" + mysign + "\n return回来的参数：" + AlipayFunction.CreateLinkString(params);
	//AlipayFunction.LogResult(sWord);

	if(mysign.equals(sign) && responseTxt.equals("true")){
	///////////////////////////请在这里加上商户的业务逻辑程序代码/////////////////////////////////
    //请根据您的业务逻辑来编写程序（以下代码仅作参考）
	    //获取支付宝的通知返回参数
		String user_id = request.getParameter("user_id");		//获取支付宝用户唯一ID号
		
		//请在这里加上商户的业务逻辑程序代码
		//以下是示例——
		//判断获取到的user_id的值是否在商户会员数据库中存在（即：是否曾经做过支付宝会员免注册登陆）
		//	若不存在，则程序自动为会员快速注册一个会员，把信息插入商户网站会员数据表中，
		//	且把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登陆信息，如登陆时间、次数、IP等。
		//	若存在，判断该会员在商户网站上的登录状态是否是“已登录”状态
		//		若不是，则把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登陆信息，如登陆时间、次数、IP等。
		//		若是，则不做任何数据库业务逻辑处理。判定该次反馈信息为重复刷新返回链接导致。
	//请根据您的业务逻辑来编写程序（以上代码仅作参考）
    ///////////////////////////////////////////////////////////////////////////////////////
%>
<table align="center" width="350" cellpadding="5" cellspacing="0">
	<tr>
	    <td align="center" class="font_title">亲爱的商城会员：<%=user_id%>：<br />您已经登录成功</td>
	</tr>
</table>
<%
	}else{
%>
<table align="center" width="350" cellpadding="5" cellspacing="0">
  <tr>
    <td align="center" class="font_title">系统出错，验证失败</td>
  </tr>
</table>
<%
	}
%>
  </body>
</html>
