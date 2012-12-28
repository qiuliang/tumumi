<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>支付宝退款入口模板页</title>
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

    <script language="JavaScript">
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
    </script>

</head>
<body>
    <center>
        <form id="alipayment" name="alipayment" action="refund.aspx" method="post" onsubmit="return CheckForm();">
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
