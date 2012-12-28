<?php
/*
 *功能：会员通用登录接口的入口页面，生成请求URL
 *版本：3.1
 *修改日期：2010-11-26
 '说明：
 '以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
 '该代码仅供学习和研究支付宝接口使用，只是提供一个参考。

*/

////////////////////注意/////////////////////////
//如果您在接口集成过程中遇到问题，
//您可以到商户服务中心（https://b.alipay.com/support/helperApply.htm?action=consultationApply），提交申请集成协助，我们会有专业的技术工程师主动联系您协助解决，
//您也可以到支付宝论坛（http://club.alipay.com/read-htm-tid-8681712.html）寻找相关解决方案
/////////////////////////////////////////////////

require_once("alipay_config.php");
require_once("class/alipay_user_service.php");

//选填参数
$email = "";		//会员通用登录时，会员的支付宝账号

/////////////////////////////////////////////////

//构造要请求的参数数组
$parameter = array(
        "service"			=> "user_authentication",	//接口名称，不需要修改

        //获取配置文件(alipay_config.php)中的值
        "partner"			=> $partner,
        "return_url"		=> $return_url,
        "_input_charset"	=> $_input_charset,
		
		//选填参数
		"email"				=> $email
);

//构造请求函数
$alipay = new alipay_user_service($parameter,$key,$sign_type);
$sHtmlText = $alipay->build_form();

?>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
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
    <td align="center"><?php echo $sHtmlText; ?></td>
  </tr>
</table>
</body>
</html>
