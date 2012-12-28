                  ╭───────────────────────╮
  ╭───────┤           支付宝代码示例结构说明             ├───────╮
  │              ╰───────────────────────╯              │
　│                                                                              │
　│     接口名称：支付宝即时到帐批量退款接口（refund_fastpay_by_platform_pwd）   │
　│　   代码版本：3.1                                                            │
  │     开发语言：ASP.NET(C#)                                                    │
  │     版    权：支付宝（中国）网络技术有限公司                                 │
　│     制 作 者：支付宝商户事业部技术支持组                                     │
  │     联系方式：商户服务电话0571-88158090                                      │
  │                                                                              │
  ╰───────────────────────────────────────╯

───────
 代码文件结构
───────

refund_vs2005_utf8
  │
  ├app_code ┈┈┈┈┈┈┈┈┈┈类文件夹
  │  │
  │  ├alipay_config.cs ┈┈┈┈基础信息配置属性类文件
  │  │
  │  ├alipay_function.cs ┈┈┈公用函数类文件
  │  │
  │  ├alipay_notify.cs ┈┈┈┈支付宝通知处理类文件
  │  │
  │  └alipay_refund_service.cs┈支付宝请求处理类文件
  │
  ├log┈┈┈┈┈┈┈┈┈┈┈┈┈日志文件夹
  │
  ├refund.aspx┈┈┈┈┈┈┈┈┈支付宝接口入口文件
  ├refund.aspx.cs ┈┈┈┈┈┈┈支付宝接口入口文件
  │
  ├default.aspx ┈┈┈┈┈┈┈┈退款模板文件
  ├default.aspx.cs┈┈┈┈┈┈┈退款模板文件
  │
  ├notify_url.aspx┈┈┈┈┈┈┈服务器异步通知页面文件
  ├notify_url.aspx.cs ┈┈┈┈┈服务器异步通知页面文件
  │
  ├Web.Config ┈┈┈┈┈┈┈┈┈配置文件（集成时删除）
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.cs、refund.aspx、refund.aspx.cs、notify_url.aspx.cs
统一命名空间为：namespace AlipayClass

default.aspx仅是支付宝提供的付款入口模板文件，可选择使用。
如果商户网站根据业务需求不需要使用，请把refund.aspx作为与商户网站网站相衔接页面。
如果需要使用default.aspx，那么refund.aspx文件无需更改，只需配置好alipay_config.cs文件
拿到default.aspx页面在商户网站中的HTTP路径放置在商户网站中需要的位置，就能直接使用支付宝接口。



─────────
 类文件函数结构
─────────

alipay_function.cs

public static string Build_mysign(Dictionary<string, string> dicArray, string key, string sign_type, string _input_charset)
功能：生成签名结果
输入：Dictionary<string, string>  dicArray 要签名的数组
      string key 安全校验码
      string sign_type 签名类型
      string _input_charset 编码格式
输出：string 签名结果字符串

public static string Create_linkstring(Dictionary<string, string> dicArray)
功能：把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
输入：Dictionary<string, string> dicArray 需要拼接的数组
输出：string 拼接完成以后的字符串

public static Dictionary<string, string> Para_filter(SortedDictionary<string, string> dicArrayPre)
功能：除去数组中的空值和签名参数并以字母a到z的顺序排序
输入：SortedDictionary<string, string> dicArrayPre 过滤前的参数组
输出：Dictionary<string, string>  去掉空值与签名参数后的新签名参数组

public static string Sign(string prestr, string sign_type, string _input_charset)
功能：签名字符串
输入：string prestr 需要签名的字符串
      string sign_type 签名类型
      string _input_charset 编码格式
输出：string 签名结果

public static string Query_timestamp(string partner)
功能：用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
输入：string partner 合作身份者ID
输出：string 时间戳字符串
功能：备用，目前闲置

public static void log_result(string sPath, string sWord)
功能：写日志，方便测试（看网站需求，也可以改成存入数据库）
输入：string sPath 日志的本地绝对路径
      string sWord 要写入日志里的文本内容

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_notify.cs

public AlipayNotify(SortedDictionary<string, string> inputPara, string notify_id, string partner, string key, string input_charset, string sign_type, string transport)
功能：构造函数
      从配置文件中初始化变量
输入：SortedDictionary<string, string> inputPara 通知返回来的参数数组
      string notify_id 验证通知ID
      string partner 合作身份者ID
      string key 安全校验码
      string input_charset 编码格式
      string sign_type 签名类型
      string transport 访问模式

private string Verify(string notify_id)
功能：验证是否是支付宝服务器发来的请求
输入：string notify_id 验证通知ID
输出：string 验证结果

private string Get_Http(string strUrl, int timeout)
功能：获取远程服务器ATN结果
输入：string strUrl 指定URL路径地址
      int timeout 超时时间设置
输出：string 服务器ATN结果字符串

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

alipay_refund_service.cs

public AlipayRefundService(string partner,
            string seller_email,
            string notify_url,
            string refund_date,
            string batch_no,
            string batch_num,
            string detail_data,
            string key,
            string input_charset,
            string sign_type)
功能：构造函数
      从配置文件及入口文件中初始化变量
输入：string partner 合作身份者ID
      string seller_email 签约支付宝账号或卖家支付宝帐户
      string notify_url 交易过程中服务器通知的页面 要用http格式的完整路径，不允许加?id=123这类自定义参数
      string refund_date 退款当天日期，获取当天日期，格式：年[4位]-月[2位]-日[2位] 小时[2位 24小时制]:分[2位]:秒[2位]，如：2007-10-01 13:13:13
      string batch_no 商家网站里的批次号，保证其唯一性，格式：当天日期[8位]+序列号[3至24位]，如：201008010000001
      string batch_num 退款笔数，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
      string detail_data 退款详细数据
      string key 安全检验码
      string input_charset 字符编码格式 目前支持 gbk 或 utf-8
      string sign_type 签名方式 不需修改

public string Build_Form()
功能：构造表单提交HTML
输出：string 表单提交HTML文本

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

notify_url.aspx.cs

public SortedDictionary<string, string> GetRequestPost()
功能：获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
输出：SortedDictionary<string, string> request回来的信息组成的数组

──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




