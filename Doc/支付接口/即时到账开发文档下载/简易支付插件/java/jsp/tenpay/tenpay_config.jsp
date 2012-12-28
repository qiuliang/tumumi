<%@ page language="java" contentType="text/html; charset=GBK"
    pageEncoding="GBK"%>

<%

/**
   财付通即时到帐支付请求示例
   
	*功能：设置财付通帐户有关信息及返回接收路径
	*版本：1.0
	*日期：2010-10-29
	'备注：
	'该部分代码只是提供一个支付案例，方便商户商户使用及测试，可以帮助开发人员快速入门并掌握开发技能；
	'对于具有WEB程序开发背景的技术开发人员，可以另外开发接口。
*/

String spname = "财付通即时到帐测试";                                           //收款单位
String bargainor_id = "1900000109";                                             //财付通商户号
String key = "8934e7d15453e97507ef794cf7b0519d";                              //财付通密钥
String return_url = "http://localhost:8080/tenpay/return_url.jsp";         //交易完成后跳转的URL，需给绝对路径，255字符内，通过该路径直接将支付结果以Get的方式返回
String show_url = "http://localhost:8080/tenpay/show.jsp"; 

/** 
 * 注意：
 * 该案例代码提供的商户号及密钥是财付通使用的测试账户，请更改为商户申请的账户
 * 申请集成财付通，请到如下地址：
 * http://mch.tenpay.com/market/index.shtml
*/

%>
