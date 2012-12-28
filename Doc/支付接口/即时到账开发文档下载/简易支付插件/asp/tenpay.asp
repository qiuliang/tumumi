<%@LANGUAGE="VBSCRIPT" CODEPAGE="936"%> 
<!--#include file="./classes/PayRequestHandler.asp"-->
<!--#include file="./tenpay_config.asp"-->
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=gbk">
<title>财付通即时到帐支付请求示例</title>
</head>
<body>
<%
'---------------------------------------------------------
'财付通即时到帐支付请求示例，商户按照此文档进行开发即可
'---------------------------------------------------------
order_price=trim(request("order_price"))
product_name=trim(request("product_name"))
remarkexplain=trim(request("remarkexplain"))
order_no=trim(request("order_no"))

Dim strDate
Dim strTime
Dim randNumber
Dim strReq

'8位日期格式YYYYmmdd
strDate = getServerDate()

'6位时间,格式hhmiss
strTime = getTime()

'4位随机数
randNumber = getStrRandNumber(1000,9999)

'10位序列号,可以自行调整。
strReq = strTime & randNumber

Dim key
Dim bargainor_id
Dim sp_billno
Dim transaction_id
Dim total_fee
Dim desc
Dim return_url

'密钥
key = tenpay_key

'商户号
bargainor_id = tenpay_id

'返回地址
return_url = tenpay_return

'商家订单号,长度若超过32位，取前32位。财付通只记录商家订单号，不保证唯一。
sp_billno = order_no

'财付通交易单号，规则为：10位商户号+8位时间（YYYYmmdd)+10位流水号,保证唯一性
transaction_id = bargainor_id & strDate & strReq

'商品价格（包含运费），以分为单位
total_fee = csng(order_price*100)

'商品名称
desc = "商品：" & product_name&",备注："&remarkexplain



'创建支付请求对象
Dim reqHandler
Set reqHandler = new PayRequestHandler

'初始化
reqHandler.init()

'设置密钥
reqHandler.setKey(key)

'-----------------------------
'设置支付参数
'-----------------------------
reqHandler.setParameter "bargainor_id", bargainor_id		'设置商户号
reqHandler.setParameter "sp_billno", sp_billno				'商户订单号
reqHandler.setParameter "transaction_id", transaction_id	'财付通交易单号
reqHandler.setParameter "total_fee", total_fee			'商品总金额,以分为单位
reqHandler.setParameter "return_url", return_url			'返回地址
reqHandler.setParameter "desc", desc						'商品名称

'用户ip,测试环境时不要加这个ip参数，正式环境再加此参数
reqHandler.setParameter "spbill_create_ip", Request.ServerVariables("REMOTE_ADDR")


'请求的URL
Dim reqUrl
reqUrl = reqHandler.getRequestURL()

'debug信息
'Dim debugInfo
'debugInfo = reqHandler.getDebugInfo()

'Response.Write("<br/>debugInfo:" & debugInfo & "<br/>")

'Response.Write("<br/>reqUrl" & reqUrl & "<br/>")

'重定向到财付通支付
reqHandler.doSend()


%>
<br/><a href="<%=reqUrl%>" target="_blank">财付通支付</a>
</body>
</html>