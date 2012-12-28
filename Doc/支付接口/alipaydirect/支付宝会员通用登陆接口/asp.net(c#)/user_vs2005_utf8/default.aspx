<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            <td align="center" class="font_title" colspan="2">
                支付宝会员通用登录</td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:Label ID="lbButton" runat="server"></asp:Label></td>
        </tr>
    </table>
</body>
</html>
