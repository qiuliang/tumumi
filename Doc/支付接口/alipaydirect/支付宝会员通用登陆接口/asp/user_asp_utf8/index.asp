<%@LANGUAGE="VBSCRIPT" CODEPAGE="65001"%>
<%
	'功能：会员免注册登录接口的入口页面，生成请求URL
	'版本：3.1
	'日期：2010-11-26
	'说明：
	'以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	'该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
	
'''''''''''''''''注意'''''''''''''''''''''''''
'如果您在接口集成过程中遇到问题，
'您可以到商户服务中心（https://b.alipay.com/support/helperApply.htm?action=consultationApply），提交申请集成协助，我们会有专业的技术工程师主动联系您协助解决，
'您也可以到支付宝论坛（http://club.alipay.com/read-htm-tid-8681712.html）寻找相关解决方案tml
''''''''''''''''''''''''''''''''''''''''''''''
%>

<!--#include file="alipay_config.asp"-->
<!--#include file="class/alipay_user_service.asp"-->

<%
'选填参数
email = ""		'会员免注册登录时，会员的支付宝账号

''''''''''''''''''''''''''''''''''''''''''''''''''''
'构造要请求的参数数组，无需改动
para = Array("service=user_authentication","partner="&partner,"return_url="&return_url,"email="&email,"_input_charset="&input_charset)

'构造请求函数
alipay_user_service(para)
sHtmlText = build_form()
%>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>支付宝会员通用登录</title>
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
<table align="center" width="350" cellpadding="5" cellspacing="0">
  <tr>
    <td align="center" class="font_title">支付宝会员通用登录</td>
  </tr>
  <tr>
    <td align="center"><%=sHtmlText%></td>
  </tr>
</table>
</body>
</html>
