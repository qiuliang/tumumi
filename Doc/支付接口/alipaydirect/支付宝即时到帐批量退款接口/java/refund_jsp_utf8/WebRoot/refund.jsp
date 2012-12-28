<%
	/*
	功能：支付宝退款接口的入口页面，生成请求URL
	 *版本：3.1
	 *日期：2010-12-02
	 *说明：
	 *以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	 *该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
	
	 *************************注意*****************
	如果您在接口集成过程中遇到问题，
	您可以到商户服务中心（https://b.alipay.com/support/helperApply.htm?action=consultationApply），提交申请集成协助，我们会有专业的技术工程师主动联系您协助解决，
	您也可以到支付宝论坛（http://club.alipay.com/read-htm-tid-8681712.html）寻找相关解决方案
	要传递的参数要么不允许为空，要么就不要出现在数组与隐藏控件或URL链接里。
	 **********************************************
	 */
%>
<%@ page language="java" contentType="text/html; charset=UTF-8"
	pageEncoding="UTF-8"%>
<%@ page import="com.alipay.config.*"%>
<%@ page import="com.alipay.util.*"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
		<title>支付宝批量退款接口</title>
	</head>
	<body>
	<%
		//request.setCharacterEncoding("UTF-8");
			//AlipyConfig.java中配置信息（不可以修改）
			String input_charset = AlipayConfig.input_charset;
			String sign_type = AlipayConfig.sign_type;
			String partner = AlipayConfig.partner;
			String key = AlipayConfig.key;
			String seller_email = AlipayConfig.seller_email;
			String notify_url = AlipayConfig.notify_url;
			/////////////////////////////////////////请求参数////////////////////////////////////////////////////
			UtilDate date = new UtilDate();//调取支付宝工具类生成订单号
			//退款当天日期，获取当天日期，格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
			String refund_date = date.getDateFormatter();
			
			//商家网站里的批次号，保证其唯一性，格式：当天日期[8位]+序列号[3至24位]，如：201008010000001
			String batch_no = date.getOrderNum();
			
			//退款笔数，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
			String batch_num = request.getParameter("batch_num");		
			
			//退款详细数据
			String detail_data = new String(request.getParameter("detail_data").getBytes("ISO-8859-1"),"UTF-8");
	        //格式：第一笔交易#第二笔交易#第三笔交易
	        //第N笔交易格式：交易退款信息
	        //交易退款信息格式：原付款支付宝交易号^退款总金额^退款理由
	        //注意：
	        //1.detail_data中的退款笔数总和要等于参数batch_num的值
	        //2.detail_data的值中不能有“^”、“|”、“#”、“$”等影响detail_data的格式的特殊字符
	        //3.detail_data中退款总金额不能大于交易总金额
	        //4.一笔交易可以多次退款，只需要遵守多次退款的总金额不超过该笔交易付款时金额。
	        //5.不支持退分润功能

	        /////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//构造函数
	        String sHtmlText = AlipayRefundService.BuildForm(partner,seller_email,notify_url,
	        refund_date,batch_no,batch_num,detail_data,input_charset,key,sign_type);
	        out.println(sHtmlText);
	%>
	</body>
</html>
