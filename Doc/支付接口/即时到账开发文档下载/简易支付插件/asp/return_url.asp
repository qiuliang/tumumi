<%@LANGUAGE="VBSCRIPT" CODEPAGE="936"%> 
<!--#include file="./classes/PayResponseHandler.asp"-->
<!--#include file="./tenpay_config.asp"-->
<%
'---------------------------------------------------------
'财付通即时到帐支付应答（处理回调）示例，商户按照此文档进行开发即可
'---------------------------------------------------------

'密钥
Dim key
key = tenpay_key

'创建支付应答对象
Dim resHandler
Set resHandler = new PayResponseHandler
resHandler.setKey(key)

'判断签名
If resHandler.isTenpaySign() = True Then
	
	Dim transaction_id
	Dim total_fee

	'交易单号
	transaction_id = resHandler.getParameter("transaction_id")

	'商品金额,以分为单位
	total_fee = resHandler.getParameter("total_fee")
	
	'支付结果
	pay_result = resHandler.getParameter("pay_result")
	
	If "0" = pay_result Then
	
		'------------------------------
		'处理业务开始
		'------------------------------
		
		'注意交易单不要重复处理
		'注意判断返回金额
		
		'------------------------------
		'处理业务完毕
		'------------------------------	
		
		'调用doShow
		resHandler.doShow(tenpay_show)
	
	Else
		'当做不成功处理
		Response.Write("支付失败")
	End If	

Else

	'签名失败
	Response.Write("签名签证失败")

	Dim debugInfo
	'debugInfo = resHandler.getDebugInfo()
	'Response.Write("<br/>debugInfo:" & debugInfo & "<br/>")

End If


%>