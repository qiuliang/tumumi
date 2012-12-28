            ╭───────────────────────╮
  ╭────┤           支付宝代码示例结构说明             ├────╮
  │        ╰───────────────────────╯        │
　│                                                                  │
　│     接口名称：支付宝会员通用登录接口（user_authentication）      │
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

user_asp_utf8
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
  ├index.asp┈┈┈┈┈┈┈┈┈┈快速付款入口模板文件
  │
  ├return_url.asp ┈┈┈┈┈┈┈页面跳转同步通知文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.asp、index.asp、return_url.asp

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
功能：备用，目前闲置

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
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




