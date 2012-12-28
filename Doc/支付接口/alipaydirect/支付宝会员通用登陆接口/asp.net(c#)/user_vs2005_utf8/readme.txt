            ╭───────────────────────╮
  ╭────┤           支付宝代码示例结构说明             ├────╮
  │        ╰───────────────────────╯        │
　│                                                                  │
　│     接口名称：支付宝会员通用登录接口（user_authentication）      │
　│　   代码版本：3.1                                                │
  │     开发语言：ASP.NET(c#)                                        │
  │     版    权：支付宝（中国）网络技术有限公司                     │
　│     制 作 者：支付宝商户事业部技术支持组                         │
  │     联系方式：商户服务电话0571-88158090                          │
  │                                                                  │
  ╰─────────────────────────────────╯

───────
 代码文件结构
───────

user_vs2005_utf8
  │
  ├app_code ┈┈┈┈┈┈┈┈┈┈类文件夹
  │  │
  │  ├alipay_config.cs ┈┈┈┈基础信息配置属性类文件
  │  │
  │  ├alipay_function.cs ┈┈┈公用函数类文件
  │  │
  │  ├alipay_notify.cs ┈┈┈┈支付宝通知处理类文件
  │  │
  │  └alipay_user_service.cs ┈支付宝会员通用登录请求处理类文件
  │
  ├images ┈┈┈┈┈┈┈┈┈┈┈图片、CSS样式文件夹
  │
  ├log┈┈┈┈┈┈┈┈┈┈┈┈┈日志文件夹
  │
  ├default.aspx ┈┈┈┈┈┈┈┈快速付款入口模板文件
  ├default.aspx.cs┈┈┈┈┈┈┈快速付款入口模板文件
  │
  ├return_url.aspx┈┈┈┈┈┈┈页面跳转同步通知文件
  ├return_url.aspx.cs ┈┈┈┈┈页面跳转同步通知文件
  │
  ├Web.Config ┈┈┈┈┈┈┈┈┈配置文件（集成时删除）
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.cs、default.aspx、default.aspx.cs、return_url.aspx、return_url.aspx.cs
统一命名空间为：namespace AlipayClass


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

alipay_user_service.cs

public AlipayService(string partner,
            string return_url,
            string email,
            string key,
            string input_charset,
            string sign_type)
功能：构造函数
      从配置文件及入口文件中初始化变量
输入：string partner 合作身份者ID
      string return_url 登录后跳转的页面 以http开头格式的完整路径，不允许加?id=123这类自定义参数
      string email 支付宝会员登录账号
      string key 安全检验码
      string input_charset 字符编码格式 目前支持 gbk 或 utf-8
      string sign_type 签名方式 不需修改

public string Build_Form()
功能：构造表单提交HTML
输出：string 表单提交HTML文本

┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉┉

return_url.aspx.cs

public SortedDictionary<string, string> GetRequestGet()
功能：获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
输出：SortedDictionary<string, string> request回来的信息组成的数组

──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




