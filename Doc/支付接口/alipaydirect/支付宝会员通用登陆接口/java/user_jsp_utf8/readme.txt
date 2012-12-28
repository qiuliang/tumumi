            ╭───────────────────────╮
  ╭────┤           支付宝代码示例结构说明             ├────╮
  │        ╰───────────────────────╯        │
　│                                                                  │
　│     接口名称：支付宝会员通用登录接口（user_authentication）      │
　│　   代码版本：3.1                                                │
  │     开发语言：JAVA                                               │
  │     版    权：支付宝（中国）网络技术有限公司                     │
　│     制 作 者：支付宝商户事业部技术支持组                         │
  │     联系方式：商户服务电话0571-88158090                          │
  │                                                                  │
  ╰─────────────────────────────────╯

───────
 代码文件结构
───────

user_jsp_utf8
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
  │  │  ├AlipayService.java ┈┈┈┈支付宝请求处理类文件
  │  │  │
  │  │  ├Md5Encrypt.java┈┈┈┈┈┈MD5签名类文件
  │  │  │
  │  │  └UtilDate.java┈┈┈┈┈┈┈自定义订单类文件
  │  │
  │  └filters┈┈┈┈┈┈┈┈┈┈┈┈过滤器文件夹（集成时删除）
  │
  ├WebRoot┈┈┈┈┈┈┈┈┈┈┈┈┈┈页面文件夹
  │  │
  │  ├images ┈┈┈┈┈┈┈┈┈┈┈┈图片、CSS样式文件夹
  │  │
  │  ├index.jsp┈┈┈┈┈┈┈┈┈┈┈快速付款入口模板文件
  │  │
  │  └return_url.jsp ┈┈┈┈┈┈┈┈页面跳转同步通知文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.jsp、index.jsp、return_url.jsp
引用包：com.alipay.config.*、com.alipay.util.*

public static void LogResult(String sWord)
函数中需要设置日志文件创建时所在电脑上的绝对路径。



─────────
 类文件函数结构
─────────

alipay_function.java

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

alipay_notify.java

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

alipay_service.java

public static String BuildForm(String partner,
	String return_url,
	String email,
        String input_charset,
        String key,
        String sign_type)
功能：构造表单提交HTML
输入：String partner 合作身份者ID
      String return_url 登录后跳转的页面 以http开头格式的完整路径，不允许加?id=123这类自定义参数
      String email 支付宝会员登录账号
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

public static String getThree()
功能：产生随机的三位数
输出：String 随机三位数

──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




