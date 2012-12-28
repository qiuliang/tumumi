                  ╭───────────────────────╮
  ╭───────┤           支付宝代码示例结构说明             ├───────╮
  │              ╰───────────────────────╯              │
　│                                                                              │
　│     接口名称：支付宝即时到帐批量退款接口（refund_fastpay_by_platform_pwd）   │
　│　   代码版本：3.1                                                            │
  │     开发语言：PHP                                                            │
  │     版    权：支付宝（中国）网络技术有限公司                                 │
　│     制 作 者：支付宝商户事业部技术支持组                                     │
  │     联系方式：商户服务电话0571-88158090                                      │
  │                                                                              │
  ╰───────────────────────────────────────╯

───────
 代码文件结构
───────

refund_php_gb
  │
  ├class┈┈┈┈┈┈┈┈┈┈┈┈类文件夹
  │  │
  │  ├alipay_function.php┈┈┈公用函数类文件
  │  │
  │  ├alipay_notify.php┈┈┈┈支付宝通知处理类文件
  │  │
  │  └alipay_service.php ┈┈┈支付宝请求处理类文件
  │
  ├log.txt┈┈┈┈┈┈┈┈┈┈┈日志文件
  │
  ├alipay_config.php┈┈┈┈┈┈基础信息配置文件
  │
  ├refund.php ┈┈┈┈┈┈┈┈┈支付宝接口入口文件
  │
  ├index.php┈┈┈┈┈┈┈┈┈┈批量退款模板文件
  │
  ├notify_url.php ┈┈┈┈┈┈┈服务器异步通知页面文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.php、refund.php、notify_url.php

index.php仅是支付宝提供的付款入口模板文件，可选择使用。
如果商户网站根据业务需求不需要使用，请把refund.php作为与商户网站网站相衔接页面。
如果需要使用index.php，那么refund.php文件无需更改，只需配置好alipay_config.php文件
拿到index.php页面在商户网站中的HTTP路径放置在商户网站中需要的位置，就能直接使用支付宝接口。


─────────
 类文件函数结构
─────────

alipay_function.php

function build_mysign($sort_array,$key,$sign_type = "MD5")
功能：生成签名结果
输入：Array  $sort_array 要签名的数组
      String $key 安全校验码
      String $sign_type 签名类型 默认值 MD5
输出：String 签名结果字符串

function create_linkstring($array)
功能：把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
输入：Array  $array 需要拼接的数组
输出：String 拼接完成以后的字符串

function para_filter($parameter)
功能：除去数组中的空值和签名参数
输入：Array  $parameter 签名参数组
输出：Array  去掉空值与签名参数后的新签名参数组

function arg_sort($array)
功能：对数组排序
输入：Array  $array 排序前的数组
输出：Array  排序后的数组

function sign($prestr,$sign_type)
功能：签名字符串
输入：String $prestr 需要签名的字符串
      String $sign_type 签名类型
输出：String 签名结果

function log_result($word)
功能：写日志，方便测试（看网站需求，也可以改成存入数据库）
输入：String $word 要写入日志里的文本内容

function query_timestamp($partner) 
功能：用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
输入：String $partner 合作身份者ID
输出：String 时间戳字符串
说明：备用，目前闲置

function charset_encode($input,$_output_charset ,$_input_charset)
功能：实现多种字符编码方式
输入：String $input 需要编码的字符串
      String $_output_charset 输出的编码格式
      String $_input_charset 输入的编码格式
输出：String 编码后的字符串

function charset_decode($input,$_input_charset ,$_output_charset) 
功能：实现多种字符解码方式
输入：String $input 需要解码的字符串
      String $_output_charset 输出的解码格式
      String $_input_charset 输入的解码格式
输出：String 解码后的字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_notify.php

function alipay_notify($partner,$key,$sign_type,$_input_charset = "GBK",$transport= "https") 
功能：构造函数
      从配置文件中初始化变量
输入：String $partner 合作身份者ID
      String $key 安全校验码
      String $sign_type 签名类型
      String $_input_charset 字符编码格式 默认值 GBK
      String $transport 访问模式 默认值 https

function notify_verify()
功能：对notify_url的认证
输出：Bool  验证结果：true/false

function return_verify()
功能：对return_url的认证
输出：Bool  验证结果：true/false

function get_verify($url,$time_out = "60")
功能：获取远程服务器ATN结果
输入：String $url 指定URL路径地址
      String $time_out 超时计时器 默认值60
输出：String 服务器ATN结果字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_service.php

function alipay_service($parameter,$key,$sign_type)
功能：构造函数
      从配置文件及入口文件中初始化变量
输入：Array  $parameter 需要签名的参数数组
      Array  $key 安全校验码
      Array  $sign_type 签名类型

function build_form()
功能：构造表单提交HTML
输出：String 表单提交HTML文本


──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




