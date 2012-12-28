<?php
/*
	*功能：支付宝会员登录完成后跳转返回的页面（页面跳转同步通知页面）
	*版本：3.1
	*日期：2010-10-22
	'说明：
	'以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
	'该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
	
*/
///////////页面功能说明///////////////
//该页面可在本机电脑测试
//该页面称作“页面跳转同步通知页面”，是由支付宝服务器同步调用
//可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
//建议：
//在商户网站会员数据库中增加一个字段：user_id（支付宝用户唯一ID）
//会员信息的数据表中的唯一ID号是商户网站会员数据表中的外键。货);
///////////////////////////////////

require_once("class/alipay_notify.php");
require_once("alipay_config.php");

//构造通知函数信息
$alipay = new alipay_notify($partner,$key,$sign_type,$_input_charset,$transport);
//计算得出通知验证结果
$verify_result = $alipay->return_verify();

if($verify_result) {//验证成功
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//请在这里加上商户的业务逻辑程序代码
	//——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
    //获取支付宝的通知返回参数
    $user_id           = $_GET['user_id'];		//获取支付宝用户唯一ID号
	
	//判断获取到的user_id的值是否在商户会员数据库中存在（即：是否曾经做过支付宝会员免注册登陆）
	//	若不存在，则程序自动为会员快速注册一个会员，把信息插入商户网站会员数据表中，
	//	且把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登陆信息，如登陆时间、次数、IP等。
	//	若存在，判断该会员在商户网站上的登录状态是否是“已登录”状态
	//		若不是，则把该会员的在商户网站上的登录状态，更改成“已登录”状态。并记录在商家网站会员数据表中记录登陆信息，如登陆时间、次数、IP等。
	//		若是，则不做任何数据库业务逻辑处理。判定该次反馈信息为重复刷新返回链接导致。
	//——请根据您的业务逻辑来编写程序（以上代码仅作参考）——
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
else {
    //验证失败
    //如要调试，请看alipay_notify.php页面的return_verify函数，比对sign和mysign的值是否相等，或者检查$veryfy_result有没有返回true
    echo "fail";
}
?>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>支付宝会员免注册登录返回信息</title>
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
<?
if($verify_result) {
?>
<table align="center" width="350" cellpadding="5" cellspacing="0">
  <tr>
    <td align="center" class="font_title">亲爱的商城会员：<?php echo $_GET['user_id']; ?>：<br />您已经登录成功</td>
  </tr>
</table>
<?
}
else
{
?>
<table align="center" width="350" cellpadding="5" cellspacing="0">
  <tr>
    <td align="center" class="font_title">系统出错，验证失败</td>
  </tr>
</table>
<?
}
?>
</body>
</html>
