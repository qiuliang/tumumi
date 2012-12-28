<%@LANGUAGE="VBSCRIPT" CODEPAGE="65001"%>
<%
	'功能：支付宝退款入口模板页
	'版本：3.1
	'日期：2010-12-02
	'说明：
	'以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	'该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML XMLNS:CC>
<HEAD>
<META http-equiv=Content-Type content="text/html; charset=utf-8">
<TITLE>支付宝 - 网上支付 安全快速！</TITLE>
<META content=网上购物/网上支付/安全支付/安全购物/购物，安全/支付,安全/支付宝/安全,支付/安全，购物/支付, 
name=description 在线 付款,收款 网上,贸易 网上贸易.>
<META content=网上购物/网上支付/安全支付/安全购物/购物，安全/支付,安全/支付宝/安全,支付/安全，购物/支付, name=keywords 
在线 付款,收款 网上,贸易 网上贸易.>
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
</HEAD>
<BODY text=#000000 bgColor=#ffffff leftMargin=0 topMargin=4>
<CENTER>
  <BR />
  <FORM name=alipayment onSubmit="return CheckForm();" action=refund.asp 
method=post target="_blank">
    <TABLE cellSpacing=0 cellPadding=0 width=450 border=0>
      <TR>
        <TD class=font_title valign="middle">支付宝退款</TD>
      </TR>
      <TR>
        <TD align="center"><HR width=450 SIZE=2 color="#999999"></TD>
      </TR>
      <tr>
        <td align="center"><TABLE cellSpacing=0 cellPadding=0 width=350 border=0>
            <TR>
              <TD class=form-left>退款笔数：</TD>
              <TD class=form-right><INPUT size=30 name=batch_num maxlength="30"></TD>
            </TR>
            <TR>
              <TD class=form-left>退款详细：</TD>
              <TD class=form-right><INPUT size=30 name=detail_data value="20100801001^0.01^备注说明一"></TD>
            </TR>
            <TR>
              <TD class=form-left></TD>
              <TD class=form-right><input name="pay" id="pay" value="退款" type="submit"></TD>
            </TR>
          </TABLE></td>
      </tr>
    </TABLE>
  </FORM>
</CENTER>
</BODY>
</HTML>
