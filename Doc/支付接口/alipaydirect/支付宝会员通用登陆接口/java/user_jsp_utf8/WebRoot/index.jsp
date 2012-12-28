
<%
	/*
	 功能：会员免注册登录接口的入口页面，生成请求URL
	 *版本：3.1.1
	 *日期：2010-11-30
	 *说明：
	 *以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	 *该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
	
	 *************************注意*****************
	 如果您在接口集成过程中遇到问题，
	 您可以提交申请集成协助，我们会有专业的技术工程师主动联系您协助解决，
	 您也可以到支付宝论坛（http://club.alipay.com/read-htm-tid-8681712.html）寻找相关解决方案
	 
	 如果不想使用扩展功能请把扩展功能参数赋空值。
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
		<title>支付宝会员通用登陆</title>
	</head>
	<%
		//request.setCharacterEncoding("UTF-8");
		//AlipyConfig.java中配置信息（不可以修改）
		String input_charset = AlipayConfig.input_charset;
		String sign_type = AlipayConfig.sign_type;
		String partner = AlipayConfig.partnerID;
		String key = AlipayConfig.key;

		String return_url = AlipayConfig.return_url;
		
		///////////////////////////////////////////////////////////////////////////////////
		
		//选填参数
        String email = "";      //会员免注册登陆时，会员的支付宝账号

        /////////////////////////////////////////////////////////////////////////////////////////////////////

		//构造函数，生成请求URL
		String sHtmlText = AlipayService.BuildForm(partner,return_url,email,input_charset,key,sign_type);
	%>

	<body>
		<style type="text/css">
<!--
.style1 {
	color: #FF0000
}
-->
</style>
<br><br>
		<table width="30%" border="0" align="center">
			<tr>
				<th scope="col" style="FONT-SIZE: 14px; COLOR: #FF6600; FONT-FAMILY: Verdana">
					支付宝会员通用登陆
				</th>
			</tr>
			<tr>
				<td ><%= sHtmlText%></td>
			</tr>
			<tr>
				<td height="2" bgcolor="#ff7300"></td>
			</tr>
		</table>
	</body>
</html>
