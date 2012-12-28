                  ╭───────────────────────╮
  ╭───────┤           支付宝代码示例结构说明             ├───────╮
  │              ╰───────────────────────╯              │
　│                                                                              │
　│     接口名称：支付宝即时到帐批量退款接口（refund_fastpay_by_platform_pwd）   │
　│　   代码版本：3.1                                                            │
  │     开发语言：JAVA                                                           │
  │     版    权：支付宝（中国）网络技术有限公司                                 │
　│     制 作 者：支付宝商户事业部技术支持组                                     │
  │     联系方式：商户服务电话0571-88158090                                      │
  │                                                                              │
  ╰───────────────────────────────────────╯

───────
 代码文件结构
───────

refund_jsp_utf8
  │
  ├src┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈类文件夹
  │  │
  │  ├com.alipay.config
  │  │  │
  │  │  └AlipayConfig.java┈┈┈┈┈基础信息配置属性类文件
  │  │
  │  ├com.alipay.util
  │  │  │
  │  │  ├AlipayFunction.java┈┈┈┈公用函数类文件
  │  │  │
  │  │  ├AlipayNotify.java┈┈┈┈┈支付宝通知处理类文件
  │  │  │
  │  │  ├AlipayRefundService.java ┈支付宝请求处理类文件
  │  │  │
  │  │  ├Md5Encrypt.java┈┈┈┈┈┈MD5签名类文件
  │  │  │
  │  │  └UtilDate.java┈┈┈┈┈┈┈自定义订单类文件
  │  │
  │  └filters┈┈┈┈┈┈┈┈┈┈┈┈过滤器文件夹（集成时删除）
  │
  ├WebRoot┈┈┈┈┈┈┈┈┈┈┈┈┈┈页面文件夹
  │  │
  │  ├refund.jsp ┈┈┈┈┈┈┈┈┈┈支付宝接口入口文件
  │  │
  │  ├index.jsp┈┈┈┈┈┈┈┈┈┈┈批量退款模板文件
  │  │
  │  └notify_url.jsp ┈┈┈┈┈┈┈┈服务器异步通知页面文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.jsp、refund.jsp、notify_url.jsp
引用包：com.alipay.config.*、com.alipay.util.*

index.jsp仅是支付宝提供的付款入口模板文件，可选择使用。
如果商户网站根据业务需求不需要使用，请把refund.jsp作为与商户网站网站相衔接页面。
如果需要使用index.jsp，那么refund.jsp文件无需更改，只需配置好alipay_config.java文件
拿到index.jsp页面在商户网站中的HTTP路径放置在商户网站中需要的位置，就能直接使用支付宝接口。

public static void LogResult(String sWord)
函数中需要设置日志文件创建时所在电脑上的绝对路径。



─────────
 类文件函数结构
─────────

AlipayFunction.java

public static String BuildMysign(Map sArray, String key)
功能：生成签名结果
输入：Map    sArray 要签名的数组
      String key 安全校验码
输出：String 签名结果字符串

public static Map ParaFilter(Map sArray)
功能：除去数组中的空值和签名参数
输入：Map    sArray 要签名的数组
输出：Map    去掉空值与签名参数后的新签名参数组

public static String CreateLinkString(Map params)
功能：把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
输入：Map    params 需要拼接的数组
输出：String 拼接完成以后的字符串

public static String query_timestamp(String partner)
功能：用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
输入：String partner 合作身份者ID
输出：String 时间戳字符串
功能：备用，目前闲置

public static void LogResult(String sWord)
功能：写日志，方便测试（看网站需求，也可以改成存入数据库）
输入：String sWord 要写入日志里的文本内容
说明：该函数中需要设置日志所在电脑上的绝对路径

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

Md5Encrypt.java

public static String md5(String text)
功能：MD5签名
输入：String sMessage 要签名的字符串
输出：String 签名结果

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

AlipayNotify.java

public static String GetMysign(Map Params, String key)
功能：根据反馈回来的信息，生成签名结果
输入：Map    Params 通知返回来的参数数组
      String key 安全校验码
输出：String 签名结果

public static String Verify(String notify_id)
功能：获取远程服务器ATN结果,验证返回URL
输入：String notify_id 验证通知ID
输出：String 验证结果

public static String CheckUrl(String urlvalue)
功能：获取远程服务器ATN结果
输入：String urlvalue 指定URL路径地址
输出：String 服务器ATN结果字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

AlipayRefundService.java

public static String BuildForm(String partner,
	String seller_email,
	String notify_url,
	String refund_date,
	String batch_no,
	String batch_num,
	String detail_data,
        String input_charset,
        String key,
        String sign_type)
功能：构造表单提交HTML
输入：String partner 合作身份者ID
      String seller_email 签约支付宝账号或卖家支付宝帐户
      String notify_url 交易过程中服务器通知的页面 要用 以http开格式的完整路径，不允许加?id=123这类自定义参数
      String refund_date 退款当天日期，获取当天日期，格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
      String batch_no 商家网站里的批次号，保证其唯一性，格式：当天日期[8位]+序列号[3至24位]，如：201008010000001
      String batch_num 退款笔数，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
      String detail_data 退款详细数据
      String key 安全检验码
      String input_charset 字符编码格式 目前支持 gbk 或 utf-8
      String sign_type 签名方式 不需修改
输出：String 表单提交HTML文本

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

UtilDate.java

public  static String getOrderNum()
功能：自动生出订单号，格式yyyyMMddHHmmss
输出：String 订单号

public  static String getDateFormatter()
功能：获取日期，格式：yyyy-MM-dd HH:mm:ss
输出：String 日期

public static String getDate()
功能：获取日期，格式：yyyyMMdd
输出：String 日期

public static String getTime()
功能：获取当前时间，格式：HHmmss
输出：String 当前时间

public static String getThree()
功能：产生随机的三位数
输出：String 随机三位数

──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




