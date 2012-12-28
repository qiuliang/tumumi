<%
	/*
	 功能：支付宝退款入口模板页
	 *版本：3.1
	 *日期：2010-12-02
	 *说明：
	 *以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	 *该代码仅供学习和研究支付宝接口使用，只是提供一个参考。

	 */
%>
<%@ page language="java" contentType="text/html; charset=GBK"
	pageEncoding="GBK"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=GBK">
		<title>支付宝批量退款接口</title>
		<style type="text/css">
			.form-left{
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
		</style>
		<SCRIPT language=JavaScript>
		function CheckForm()
		{
			if (document.alipayment.batch_num.value.length == 0) {
				alert("请输入退款笔数.");
				document.alipayment.batch_num.focus();
				return false;
			}
			if (document.alipayment.detail_data.value.length == 0) {
				alert("请输入退款详细.");
				document.alipayment.detail_data.focus();
				return false;
			}
		
		}  
		</SCRIPT>
	</head>
<body>
    <center>
        <form id="alipayment" name="alipayment" action="refund.jsp" method="post" onsubmit="return CheckForm();">
            <table cellspacing="0" cellpadding="0" width="450" border="0">
                <tr>
                    <td class="font_title" valign="middle" height="40">
                        支付宝退款</td>
                </tr>
                <tr>
                    <td align="center">
                        <hr width="450" size="2" color="#999999">
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellspacing="0" cellpadding="0" width="350" border="0">
                            <tr>
                                <td class="form-left">
                                    退款笔数：</td>
                                <td>
                                    <input size="30" name="batch_num" maxlength="30"></td>
                            </tr>
                            <tr>
                                <td class="form-left">
                                    退款详细：</td>
                                <td>
                                    <input size="30" name="detail_data" value="20100801001^0.01^备注说明一"></td>
                            </tr>
                            <tr>
                                <td class="form-left">
                                </td>
                                <td>
                                    <input name="pay" id="pay" value="退款" type="submit"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </form>
    </center>
	</body>
</html>
