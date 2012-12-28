<%@ page language="java" contentType="text/html; charset=GBK"
    pageEncoding="GBK"%>
 
<%@ page import="com.tenpay.util.TenpayUtil" %>
<%@ page import="com.tenpay.PayResponseHandler"%>       
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%
//密钥
String key = "8934e7d15453e97507ef794cf7b0519d";

//创建PayResponseHandler实例
PayResponseHandler resHandler = new PayResponseHandler(request, response);

resHandler.setKey(key);

//判断签名
if(resHandler.isTenpaySign()) {
	//交易单号
	String transaction_id = resHandler.getParameter("transaction_id");
	
	//金额金额,以分为单位
	String total_fee = resHandler.getParameter("total_fee");
	
	//支付结果
	String pay_result = resHandler.getParameter("pay_result");
	
	if( "0".equals(pay_result) ) {
		//------------------------------
		//处理业务开始
		//------------------------------ 
		
		//注意交易单不要重复处理
		//注意判断返回金额
		
		//------------------------------
		//处理业务完毕
		//------------------------------
			
		//调用doShow, 打印meta值跟js代码,告诉财付通处理成功,并在用户浏览器显示$show页面.
		resHandler.doShow("http://localhost:8080/tenpay/show.jsp");
	} else {
		//当做不成功处理
		out.println("支付失败");
	}
	
} else {
	out.println("认证签名失败");
	//String debugInfo = resHandler.getDebugInfo();
	//System.out.println("debugInfo:" + debugInfo);
}

%>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GBK">
<title>财付通支付回调处理</title>
</head>
<body>

</body>
</html>