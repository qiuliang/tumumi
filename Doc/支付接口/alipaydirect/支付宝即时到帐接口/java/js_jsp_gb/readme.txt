            ╭───────────────────────╮
  ╭────┤           支付宝代码示例结构说明             ├────╮
  │        ╰───────────────────────╯        │
　│                                                                  │
　│     接口名称：支付宝即时到帐接口（create_direct_pay_by_user）    │
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

js_jsp_gb
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
  │  ├alipayto.jsp ┈┈┈┈┈┈┈┈┈支付宝接口入口文件
  │  │
  │  ├index.jsp┈┈┈┈┈┈┈┈┈┈┈快速付款入口模板文件
  │  │
  │  ├notify_url.jsp ┈┈┈┈┈┈┈┈服务器异步通知页面文件
  │  │
  │  └return_url.jsp ┈┈┈┈┈┈┈┈页面跳转同步通知文件
  │
  └readme.txt ┈┈┈┈┈┈┈┈┈使用说明文本

※注意※
需要配置的文件是：alipay_config.jsp、alipayto.jsp
引用包：com.alipay.config.*、com.alipay.util.*

index.jsp仅是支付宝提供的付款入口模板文件，可选择使用。
如果商户网站根据业务需求不需要使用，请把alipayto.jsp作为与商户网站网站相衔接页面。
如果需要使用index.jsp，那么alipayto.jsp文件无需更改，只需配置好alipay_config.java文件
拿到index.jsp页面在商户网站中的HTTP路径放置在商户网站中需要的位置，就能直接使用支付宝接口。

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

public static void LogResult(String sWord)
功能：写日志，方便测试（看网站需求，也可以改成存入数据库）
输入：String sWord 要写入日志里的文本内容

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
	String seller_email,
	String return_url,
	String notify_url,
	String show_url,
	String out_trade_no,
	String subject,
	String body,
	String total_fee,
	String paymethod,
	String defaultbank,
	String encrypt_key,
	String exter_invoke_ip,
	String extra_common_param,
	String buyer_email,
	String royalty_type,
	String royalty_parameters,
	String it_b_pay,
        String input_charset,
        String key,
        String sign_type)
功能：构造表单提交HTML
输入：String partner 合作身份者ID
      String seller_email 签约支付宝账号或卖家支付宝帐户
      String return_url 付完款后跳转的页面 要用 以http开头格式的完整路径，不允许加?id=123这类自定义参数
      String notify_url 交易过程中服务器通知的页面 要用 以http开格式的完整路径，不允许加?id=123这类自定义参数
      String show_url 网站商品的展示地址，不允许加?id=123这类自定义参数
      String out_trade_no 请与贵网站订单系统中的唯一订单号匹配
      String subject 订单名称，显示在支付宝收银台里的“商品名称”里，显示在支付宝的交易管理的“商品名称”的列表里。
      String body 订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
      String total_fee 订单总金额，显示在支付宝收银台里的“应付总额”里
      String paymethod 默认支付方式，四个值可选：bankPay(网银); cartoon(卡通); directPay(余额); CASH(网点支付)
      String defaultbank 默认网银代号，代号列表见club.alipay.com/read.php?tid=8681379
      String encrypt_key 防钓鱼时间戳
      String exter_invoke_ip 买家本地电脑的IP地址
      String extra_common_param 自定义参数，可存放任何内容（除等特殊字符外），不会显示在页面上
      String buyer_email 默认买家支付宝账号
      String royalty_type 提成类型，该值为固定值：10，不需要修改
      String royalty_parameters 提成信息集，与需要结合商户网站自身情况动态获取每笔交易的各分润收款账号、各分润金额、各分润说明。最多只能设置10条
      String it_b_pay 超时时间，不填默认是15天。八个值可选：1h(1小时),2h(2小时),3h(3小时),1d(1天),3d(3天),7d(7天),15d(15天),1c(当天)
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
 如何增加请求参数
──────────

在技术文档的请求参数列表中有诸多请求参数，如果因业务需求等原因要利用这些参数，那么可以按照下面的操作方法来扩充接口功能。

┉┉┉以参数it_b_pay为例┉┉┉


第一步：com.alipay.util文件夹下的AlipayService.java文件

找到BuildForm的构造表单提交HTML函数，增加输入参数“,String it_b_pay”，
在代码“Map sPara = new HashMap();”的下方对签名数组sPara增加元素“sPara.put("it_b_pay", it_b_pay);”

如：
////////////////////////////////////////////////
	/**
	 * 功能：构造表单提交HTML
	 * @param partner 合作身份者ID
	 * @param seller_email 签约支付宝账号或卖家支付宝帐户
	 * @param return_url 付完款后跳转的页面 要用 以http开头格式的完整路径，不允许加?id=123这类自定义参数
	 * @param notify_url 交易过程中服务器通知的页面 要用 以http开格式的完整路径，不允许加?id=123这类自定义参数
	 * @param show_url 网站商品的展示地址，不允许加?id=123这类自定义参数
	 * @param out_trade_no 请与贵网站订单系统中的唯一订单号匹配
	 * @param subject 订单名称，显示在支付宝收银台里的“商品名称”里，显示在支付宝的交易管理的“商品名称”的列表里。
	 * @param body 订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
	 * @param total_fee 订单总金额，显示在支付宝收银台里的“应付总额”里
	 * @param paymethod 默认支付方式，四个值可选：bankPay(网银); cartoon(卡通); directPay(余额);  CASH(网点支付)
	 * @param defaultbank 默认网银代号，代号列表见club.alipay.com/read.php?tid=8681379
	 * @param encrypt_key 防钓鱼时间戳
	 * @param exter_invoke_ip 买家本地电脑的IP地址
	 * @param extra_common_param 自定义参数，可存放任何内容（除等特殊字符外），不会显示在页面上
	 * @param buyer_email 默认买家支付宝账号
	 * @param royalty_type 提成类型，该值为固定值：10，不需要修改
	 * @param royalty_parameters 提成信息集，与需要结合商户网站自身情况动态获取每笔交易的各分润收款账号、各分润金额、各分润说明。最多只能设置10条
	 * @param it_b_pay 超时时间，不填默认是15天。八个值可选：1h(1小时),2h(2小时),3h(3小时),1d(1天),3d(3天),7d(7天),15d(15天),1c(当天)
	 * @param input_charset 字符编码格式 目前支持 GBK 或 utf-8
	 * @param key 安全校验码
	 * @param sign_type 签名方式 不需修改
	 * @return 表单提交HTML文本
	 */
	public static String BuildForm(String partner,
			String seller_email,
			String return_url,
			String notify_url,
			String show_url,
			String out_trade_no,
			String subject,
			String body,
			String total_fee,
			String paymethod,
			String defaultbank,
			String anti_phishing_key,
			String exter_invoke_ip,
			String extra_common_param,
            		String buyer_email,
			String royalty_type,
			String royalty_parameters,
			String it_b_pay,
            		String input_charset,
            		String key,
			String sign_type){
		Map sPara = new HashMap();
		sPara.put("service","create_direct_pay_by_user");
		sPara.put("payment_type","1");
		sPara.put("partner", partner);
		sPara.put("seller_email", seller_email);
		sPara.put("return_url", return_url);
		sPara.put("notify_url", notify_url);
		sPara.put("_input_charset", input_charset);
		sPara.put("show_url", show_url);
		sPara.put("out_trade_no", out_trade_no);
		sPara.put("subject", subject);
		sPara.put("body", body);
		sPara.put("total_fee", total_fee);
		sPara.put("paymethod", paymethod);
		sPara.put("defaultbank", defaultbank);
		sPara.put("anti_phishing_key", anti_phishing_key);
		sPara.put("exter_invoke_ip", exter_invoke_ip);
		sPara.put("extra_common_param", extra_common_param);
		sPara.put("buyer_email", buyer_email);
		sPara.put("royalty_type", royalty_type);
		sPara.put("royalty_parameters", royalty_parameters);
		sPara.put("it_b_pay", it_b_pay);
		
		Map sParaNew = AlipayFunction.ParaFilter(sPara); //除去数组中的空值和签名参数
		String mysign = AlipayFunction.BuildMysign(sParaNew, key);//生成签名结果
		
		StringBuffer sbHtml = new StringBuffer();
		List keys = new ArrayList(sParaNew.keySet());
		String gateway = "https://www.alipay.com/cooperate/gateway.do?";
		
		//GET方式传递
		sbHtml.append("<form id=\"alipaysubmit\" name=\"alipaysubmit\" action=\"" + gateway + "_input_charset=" + input_charset + "\" method=\"get\">");
		//POST方式传递（GET与POST二必选一）
		//sbHtml.append("<form id=\"alipaysubmit\" name=\"alipaysubmit\" action=\"" + gateway + "_input_charset=" + input_charset + "\" method=\"post\">");
		
		for (int i = 0; i < keys.size(); i++) {
			String name = (String) keys.get(i);
			String value = (String) sParaNew.get(name);
			
			sbHtml.append("<input type=\"hidden\" name=\"" + name + "\" value=\"" + value + "\"/>");
		}
        sbHtml.append("<input type=\"hidden\" name=\"sign\" value=\"" + mysign + "\"/>");
        sbHtml.append("<input type=\"hidden\" name=\"sign_type\" value=\"" + sign_type + "\"/>");
        
        //submit按钮控件请不要含有name属性
        sbHtml.append("<input type=\"submit\" value=\"支付宝确认付款\"></form>");
        
        sbHtml.append("<script>document.forms['alipaysubmit'].submit();</script>");
		
		return sbHtml.toString();
	}
////////////////////////////////////////////////


第二步：打开alipayto.aspx.cs文件

在注释“//以下参数是需要通过下单时的订单数据传入进来获得”与“//////////”代码段之间添加请求参数：

如：
////////////////////////////////////////////////
//扩展功能参数——自定义超时(若要使用，请按照注释要求的格式赋值)
//该功能默认不开通，
//申请开通方式：
//方式一：联系支付宝技术支持申请处理
//方式二：拨打0571-88158090申请
//方式三：提交集成申请（https://b.alipay.com/support/helperApply.htm?action=consultationApply）
String it_b_pay = "";
//超时时间，不填默认是15天。设置范围：1m~15d。,-分隔符，~-范围 ， m-分钟，h-小时，d-天，1c-当天（无论何时创建，交易都在0点关闭）
//如：it_b_pay = "1m~1h,2h,3h,1c";
////////////////////////////////////////////////

在“构造要请求的参数数组，无需改动”注释下方的“String sHtmlText = AlipayService.BuildForm”的括号中增加输入参数“,it_b_pay”

如：
////////////////////////////////
String sHtmlText = AlipayService.BuildForm(partner,seller_email,return_url,notify_url,show_url,out_trade_no,
subject,body,total_fee,paymethod,defaultbank,anti_phishing_key,exter_invoke_ip,extra_common_param,buyer_email,
royalty_type,royalty_parameters,it_b_pay,input_charset,key,sign_type);
////////////////////////////////


──────────
 出现问题，求助方法
──────────

如果在集成支付宝接口时，有疑问或出现问题，可使用下面的链接，提交申请。
https://b.alipay.com/support/helperApply.htm?action=supportHome
我们会有专门的技术支持人员为您处理




