            ╭───────────────────────╮
  ╭────┤           支付宝代码示例结构说明             ├────╮
  │        ╰───────────────────────╯        │
　│                                                                  │
　│     接口名称：支付宝即时到帐接口（create_direct_pay_by_user）    │
　│　   代码版本：3.1                                                │
  │     开发语言：ASP                                                │
  │     版    权：支付宝（中国）网络技术有限公司                     │
　│     制 作 者：支付宝商户事业部技术支持组                         │
  │     联系方式：商户服务电话0571-88158090                          │
  │                                                                  │
  ╰─────────────────────────────────╯

───────
 代码文件结构
───────

js_asp_gb
  │
  ├class┈┈┈┈┈┈┈┈┈┈┈┈类文件夹
  │  │
  │  ├alipay_function.asp┈┈┈公用函数类文件
  │  │
  │  ├alipay_md5.asp ┈┈┈┈┈MD5类文件
  │  │
  │  ├alipay_notify.asp┈┈┈┈支付宝通知处理类文件
  │  │
  │  └alipay_service.asp ┈┈┈支付宝请求处理类文件
  │
  ├images ┈┈┈┈┈┈┈┈┈┈┈图片、CSS样式文件夹
  │
  ├log┈┈┈┈┈┈┈┈┈┈┈┈┈日志文件夹
  │
  ├alipay_config.asp┈┈┈┈┈┈基础信息配置文件
  │
  ├alipayto.asp ┈┈┈┈┈┈┈┈支付宝接口入口文件
  │
  ├index.asp┈┈┈┈┈┈┈┈┈┈快速付款入口模板文件
  │
  ├notify_url.asp ┈┈┈┈┈┈┈服务器异步通知页面文件
  │
  ├return_url.asp ┈┈┈┈┈┈┈页面跳转同步通知文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.asp、alipayto.asp

index.asp仅是支付宝提供的付款入口模板文件，可选择使用。
如果商户网站根据业务需求不需要使用，请把alipayto.asp作为与商户网站网站相衔接页面。
如果需要使用index.asp，那么alipayto.asp文件无需更改，只需配置好alipay_config.asp文件
拿到index.asp页面在商户网站中的HTTP路径放置在商户网站中需要的位置，就能直接使用支付宝接口。


─────────
 类文件函数结构
─────────

alipay_function.asp

function build_mysign(sArray, key, sign_type,input_charset)
功能：生成签名结果
输入：Array  sArray 要签名的数组
      String key 安全校验码
      String sign_type 签名类型
输出：String 签名结果字符串

function create_linkstring(sArray)
功能：把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
输入：Array  sArray 需要拼接的数组
输出：String 拼接完成以后的字符串

function para_filter(sArray)
功能：除去数组中的空值和签名参数
输入：Array  sArray 签名参数组
输出：Array  去掉空值与签名参数后的新签名参数组

function arg_sort(sArray)
功能：对数组排序
输入：Array  sArray 排序前的数组
输出：Array  排序后的数组

function sign(prestr,sign_type,input_charset)
功能：签名字符串
输入：String prestr 需要签名的字符串
      String sign_type 签名类型
      String input_charset 编码格式
输出：String 签名结果

function query_timestamp(partner)
功能：用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
输入：String partner 合作身份者ID
输出：String 时间戳字符串

function log_result(sWord)
功能：写日志，方便测试（看网站需求，也可以改成存入数据库）
输入：String sWord 要写入日志里的文本内容

function GetDateTime_Format()
功能：获取当前时间
格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
输出：String 当前日期
说明：闲置

function GetDateTime()
功能：获取当前时间
格式：年[4位]月[2位]日[2位]小时[2位 24小时制]分[2位]秒[2位]，如：20071001131313
输出：String 当前日期

Function DelStr(Str)
功能：过滤特殊字符
输入：String Str 要被过滤的字符串
输出：String 已被过滤掉的新字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_md5.asp

Public Function MD5(sMessage,input_charset)
功能：MD5签名
输入：String sMessage 要签名的字符串
      String input_charset 编码格式，utf-8、gbk
输出：String 签名结果

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_notify.asp

function notify_verify()
功能：对notify_url的认证
输出：Bool  验证结果：true/false

function return_verify()
功能：对return_url的认证
输出：Bool  验证结果：true/false

function GetRequestGet()
功能：获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
输出：Array  request回来的信息组成的数组

function GetRequestPost()
功能：获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
输出：Array  request回来的信息组成的数组

function get_http()
功能：获取远程服务器ATN结果
输出：String 服务器ATN结果字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_service.asp

function alipay_service(inputPara)
功能：构造函数
      从配置文件及入口文件中初始化变量
输入：Array  inputPara 需要签名的参数数组

function build_form()
功能：构造表单提交HTML
输出：String 表单提交HTML文本



──────────
 如何增加请求参数
──────────

在技术文档的请求参数列表中有诸多请求参数，如果因业务需求等原因要利用这些参数，那么可以按照下面的操作方法来扩充接口功能。

┉┉┉以参数it_b_pay为例┉┉┉

打开alipayto.asp文件，在注释“以下参数是需要通过下单时的订单数据传入进来获得”与“''''''''''''''''''''''''''''''''''''''''''''''''''''”代码段之间添加以下代码：

'''''''''''''''''''''''''''''''''''''''''''
'扩展功能参数——自定义超时(若要使用，请按照注释要求的格式赋值)
'该功能默认不开通，
'申请开通方式：
'方式一：联系支付宝技术支持申请处理
'方式二：拨打0571-88158090申请
'方式三：提交集成申请（https://b.alipay.com/support/helperApply.htm?action=consultationApply）
it_b_pay = ""
'超时时间，不填默认是15天。设置范围：1m~15d。,-分隔符，~-范围 ， m-分钟，h-小时，d-天，1c-当天（无论何时创建，交易都在0点关闭）
'如：it_b_pay = "1m~1h,2h,3h,1c"
'''''''''''''''''''''''''''''''''''''''''''



在“构造要请求的参数数组，无需改动”注释下方的“数组参数para”中增加数组元素【"it_b_pay="&it_b_pay】

'''''''''''''''''''''''''''''''''''''''''''
para = Array("service=create_direct_pay_by_user","payment_type=1","partner="&partner,"seller_email="&seller_email,"return_url="&return_url,"notify_url="&notify_url,"_input_charset="&input_charset,"show_url="&show_url,"out_trade_no="&out_trade_no,"subject="&subject,"body="&body,"total_fee="&total_fee,"paymethod="&paymethod,"defaultbank="&defaultbank,"anti_phishing_key="&encrypt_key,"exter_invoke_ip="&exter_invoke_ip,"extra_common_param="&extra_common_param,"buyer_email="&buyer_email,"royalty_type="&royalty_type,"royalty_parameters="&royalty_parameters,"it_b_pay="&it_b_pay)
'''''''''''''''''''''''''''''''''''''''''''


──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




