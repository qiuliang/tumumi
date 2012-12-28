<%@ page language="java" contentType="text/html; charset=GBK"
    pageEncoding="GBK"%>

<%@ page import="java.text.SimpleDateFormat" %>    
<%@ page import="java.util.Date" %>
<%@ page import="com.tenpay.util.TenpayUtil" %>
<%@ page import="com.tenpay.PayRequestHandler"%>

<%@ include file="tenpay_config.jsp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GBK">
<title>财付通支付支付测试</title>
</head>
<body>

<%
//设置编码
request.setCharacterEncoding("GBK");
//获取提交的商品价格
String order_price=request.getParameter("order_price");  
//获取提交的商品名称
String product_name=request.getParameter("product_name");  
//获取提交的备注信息
String remarkexplain=request.getParameter("remarkexplain");  
//获取提交的订单号
String order_no=request.getParameter("order_no");  

/* 商品价格（包含运费），以分为单位 */
double total_fee = (Double.valueOf(order_price) * 100);
int fee = (int)total_fee;


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
String sp_billno = order_no;

String desc = "商品：" + product_name+",备注："+remarkexplain;

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
reqHandler.setParameter("sp_billno", sp_billno);				     //商家订单号
reqHandler.setParameter("transaction_id", transaction_id);		//财付通交易单号
reqHandler.setParameter("return_url", return_url);				//支付通知url
reqHandler.setParameter("desc", desc);	                            //商品名称
reqHandler.setParameter("total_fee", String.valueOf(fee));			          //商品金额,以分为单位
reqHandler.setParameter("purchaser_id", "");						//商品金额,以分为单位

//用户客户端ip,其值为空或者为本地地址，只能支付1毛钱以下金额
reqHandler.setParameter("spbill_create_ip",request.getRemoteAddr());

//获取请求带参数的url
String requestUrl = reqHandler.getRequestURL();

//获取debug信息
String debuginfo = reqHandler.getDebugInfo();

//System.out.println("requestUrl:" + requestUrl);
//System.out.println("debuginfo:" + debuginfo);

//重定向跳转
//reqHandler.doSend();

%>
<br/><a target="_blank" href="<%=requestUrl %>">财付通支付</a>
</body>
</html>