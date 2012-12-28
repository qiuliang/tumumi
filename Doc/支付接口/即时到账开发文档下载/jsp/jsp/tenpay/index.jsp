<%@ page language="java" contentType="text/html; charset=GBK"
    pageEncoding="GBK"%>

<%@ page import="java.text.SimpleDateFormat" %>    
<%@ page import="java.util.Date" %>
<%@ page import="com.tenpay.util.TenpayUtil" %>
<%@ page import="com.tenpay.PayRequestHandler"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GBK">
<title>财付通支付支付测试</title>
</head>
<body>
<%
//商户号
String bargainor_id = "1900000109";

//密钥
String key = "8934e7d15453e97507ef794cf7b0519d";

//回调通知URL
String return_url = "http://localhost:8080/tenpay/return_url.jsp";

//当前时间 yyyyMMddHHmmss
String currTime = TenpayUtil.getCurrTime();

//8位日期
String strDate = currTime.substring(0, 8);

//6位时间
String strTime = currTime.substring(8, currTime.length());

//四位随机数
String strRandom = TenpayUtil.buildRandom(4) + "";

//10位序列号,可以自行调整。
String strReq = strTime + strRandom;

//商家订单号,长度若超过32位，取前32位。财付通只记录商家订单号，不保证唯一。
String sp_billno = strReq;

//财付通交易单号，规则为：10位商户号+8位时间（YYYYmmdd)+10位流水号
String transaction_id = bargainor_id + strDate + strReq;

//创建PayRequestHandler实例
PayRequestHandler reqHandler = new PayRequestHandler(request, response);

//设置密钥
reqHandler.setKey(key);

//初始化
reqHandler.init();

//-----------------------------
//设置支付参数
//-----------------------------
reqHandler.setParameter("bargainor_id", bargainor_id);			//商户号
reqHandler.setParameter("sp_billno", sp_billno);				//商家订单号
reqHandler.setParameter("transaction_id", transaction_id);		//财付通交易单号
reqHandler.setParameter("return_url", return_url);				//支付通知url
reqHandler.setParameter("desc", "订单号：" + transaction_id);	//商品名称
reqHandler.setParameter("total_fee", "1");						//商品金额,以分为单位

//用户ip,测试环境时不要加这个ip参数，正式环境再加此参数
reqHandler.setParameter("spbill_create_ip",request.getRemoteAddr());

//获取请求带参数的url
String requestUrl = reqHandler.getRequestURL();

//获取debug信息
String debuginfo = reqHandler.getDebugInfo();

//System.out.println("requestUrl:" + requestUrl);
//System.out.println("debuginfo:" + debuginfo);


%>
<br/><a target="_blank" href="<%=requestUrl %>">财付通支付</a>
</body>
</html>