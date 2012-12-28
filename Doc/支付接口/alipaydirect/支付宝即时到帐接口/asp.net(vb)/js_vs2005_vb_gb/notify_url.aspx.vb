Imports System.IO
Imports System.Net
Imports AlipayClass

'功能：支付宝主动通知调用的页面（服务器异步通知页面）
'版本：3.1
'日期：2010-11-16
'说明：
'以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
'该代码仅供学习和研究支付宝接口使用，只是提供一个参考。

''''''''''''页面功能说明'''''''''''''''''''
'创建该页面文件时，请留心该页面文件中无任何HTML代码及空格。
'该页面不能在本机电脑测试，请到服务器上做测试。请确保外部可以访问该页面。
'该页面调试工具请使用写文本函数log_result，该函数已被默认开启，见alipay_notify.asp中的函数notify_verify
'TRADE_FINISHED(表示交易已经成功结束，通用即时到帐反馈的交易状态成功标志);
'TRADE_SUCCESS(表示交易已经成功结束，高级即时到帐反馈的交易状态成功标志);
'该服务器异步通知页面面主要功能是：对于返回页面（return_url.asp）做补单处理。如果没有收到该页面返回的 success 信息，支付宝会在24小时内按一定的时间策略重发通知
'''''''''''''''''''''''''''''''''''''''''''
Partial Class notify_url
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ''''''''''''''''''''''''以下参数是需要设置的相关配置参数，设置后不会更改的'''''''''''''''''''''''''
        Dim con As alipay_config = New alipay_config()
        con.alipay_config()
        Dim partner As String = con.Partner
        Dim key As String = con.Key
        Dim input_charset As String = con.Input_charset
        Dim sign_type As String = con.Sign_type
        Dim transport As String = con.Transport

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim sArray As String() = GetRequestPost()

        If sArray.Length > 0 Then   '判断是否有带返回参数
            Dim aliNotify As alipay_notify = New alipay_notify()
            aliNotify.alipay_notify(sArray, Request.Form("notify_id"), partner, key, input_charset, sign_type, transport)
            Dim responseTxt As String = aliNotify.ResponseTxt   '获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            Dim sign As String = Request.Form("sign")           '获取支付宝反馈回来的sign结果
            Dim mysign As String = aliNotify.Mysign             '获取通知返回后计算后（验证）的签名结果

            '写日志记录（若要调试，请取消下面两行注释）
            Dim sWord As String = "responseTxt=" & responseTxt & "\n notify_url_log:sign=" & Request.Form("sign") & "&mysign=" & mysign & "\n notify回来的参数：" & aliNotify.PreSignStr
            alipay_function.log_result(Server.MapPath("log/" & DateTime.Now.ToString().Replace(":", "")) & ".txt", sWord)

            '判断responsetTxt是否为ture，生成的签名结果mysign与获得的签名结果sign是否一致
            'responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            'mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关

            If responseTxt = "true" And sign = mysign Then    '验证成功
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '请在这里加上商户的业务逻辑程序代码

                '——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                '获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                Dim trade_no As String = Request.Form("trade_no")          '支付宝交易号
                Dim order_no As String = Request.Form("out_trade_no")      '获取订单号
                Dim total_fee As String = Request.Form("total_fee")        '获取总金额
                Dim subject As String = Request.Form("subject")            '商品名称、订单名称
                Dim body As String = Request.Form("body")                  '商品描述、订单备注、描述
                Dim buyer_email As String = Request.Form("buyer_email")    '买家支付宝账号
                Dim trade_status As String = Request.Form("trade_status")  '交易状态


                If Request.Form("trade_status") = "TRADE_FINISHED" Or Request.Form("trade_status") = "TRADE_SUCCESS" Then
                    '判断该笔订单是否在商户网站中已经做过处理（可参考“集成教程”中“3.4返回数据处理”）
                    '如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                    '如果有做过处理，不执行商户的业务程序

                    Response.Write("success")       '请不要修改或删除
                Else
                    Response.Write("fail")          '请不要修改或删除
                End If
                '——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Else    '验证失败
                Response.Write("fail")              '请不要修改或删除
            End If
        Else
            Response.Write("无通知参数")            '请不要修改或删除
        End If
    End Sub

    '获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
    '输出 String() request回来的信息组成的数组
    Private Function GetRequestPost() As String()
        Dim sArray As String()
        Dim i As Integer
        Dim coll As NameValueCollection
        'Load Form variables into NameValueCollection variable.
        coll = Request.Form
        ' Get names of all forms into a string array
        Dim requestItem As String() = coll.AllKeys

        For i = 0 To requestItem.Length - 1
            ReDim Preserve sArray(i)
            sArray(i) = requestItem(i) & "=" & Request.Form(requestItem(i))
        Next

        GetRequestPost = sArray
    End Function
End Class
