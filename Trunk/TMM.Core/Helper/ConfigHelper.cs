using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Model;

namespace TMM.Core.Helper
{
    public class ConfigHelper
    {
        /// <summary>
        /// 下载文档需要的key
        /// </summary>
        public static string DownloadKey = System.Configuration.ConfigurationSettings.AppSettings["DownloadKey"];
        /// <summary>
        /// 允许上传的头像文件类型
        /// </summary>
        public static string[] allowUploadFileType = System.Configuration.ConfigurationSettings.AppSettings["AllowUploadFileType"].Split(',');

        /// <summary>
        /// 允许上传的文档类型，是允许上传文档的配置和不需要转换列表的组合
        /// </summary>
        public static string[] docFileType {
            get {
                string[] allowTypes = System.Configuration.ConfigurationSettings.AppSettings["docFileType"].Split(',');
                //string[] notConvertTypes = System.Configuration.ConfigurationSettings.AppSettings["NotConvertDocTypes"].Split(',');
                List<string> tmp = new List<string>();
                tmp.AddRange(allowTypes);
                tmp.AddRange(NotConvertDocTypes);
                return tmp.ToArray();
            }
        }
        /// <summary>
        /// 文档大小限制，单位M
        /// </summary>
        public static int DocSizeLimit = int.Parse( System.Configuration.ConfigurationSettings.AppSettings["DocSizeLimit"]);
        /// <summary>
        /// 上传路径
        /// </summary>
        public static string uploadDir = System.Configuration.ConfigurationSettings.AppSettings["UploadDir"];
        /// <summary>
        /// 上传文档的保存路径
        /// </summary>
        public static string DocUploadDir = System.Configuration.ConfigurationSettings.AppSettings["DocUploadDir"];
        /// <summary>
        /// 上传图片后的访问URL
        /// </summary>
        public static string uploadUrl = System.Configuration.ConfigurationSettings.AppSettings["UploadUrl"];
        /// <summary>
        /// 管理员的用户ID，用于发送系统消息
        /// </summary>
        public static int AdminUserId = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["TmmAdminId"]);
        /// <summary>
        /// 文档图标路径
        /// </summary>
        public static string DocIconPath = System.Configuration.ConfigurationSettings.AppSettings["DocIconPath"];
        /// <summary>
        /// 热度规则
        /// </summary>
        public static string[] HotRule = System.Configuration.ConfigurationSettings.AppSettings["HotRule"].Split(',');
        /// <summary>
        /// 文档类型、描述，对应列表
        /// </summary>
        public static List<DocType> AllDocTypes = new List<DocType>() { 
            new DocType(){ TypeId = 1,TypeName = "doc",TypeDesc = "Microsoft Word" },
            new DocType(){ TypeId = 1,TypeName = "docx",TypeDesc = "Microsoft Word" },

            new DocType(){ TypeId = 2,TypeName = "xls",TypeDesc = "Microsoft Excel" },
            new DocType(){ TypeId = 2,TypeName = "xlsx" ,TypeDesc = "Microsoft Excel" },

            new DocType(){ TypeId = 3,TypeName = "ppt",TypeDesc = "Microsoft PowerPoint"  },
            new DocType(){ TypeId = 3,TypeName = "pptx" ,TypeDesc = "Microsoft PowerPoint"},
            new DocType(){ TypeId = 3,TypeName = "pps" ,TypeDesc = "Microsoft PowerPoint"},
            new DocType(){ TypeId = 3,TypeName = "ppsx",TypeDesc = "Microsoft PowerPoint" },

            new DocType(){ TypeId = 4,TypeName = "pdf",TypeDesc = "PDF" },

            new DocType(){ TypeId = 5,TypeName = "txt" ,TypeDesc = "Text Files"},
            new DocType(){ TypeId = 5,TypeName = "rtf" ,TypeDesc = "Text Files"},

            new DocType(){ TypeId = 6,TypeName = "vsd" ,TypeDesc = "Microsoft Visio"},

            new DocType(){ TypeId = 7,TypeName = "mpp"  ,TypeDesc = "Microsoft Project"}
        };
        /// <summary>
        /// 公司类型
        /// </summary>
        public static Hashtable CommpanyType {
            get {
                string[] cts = System.Configuration.ConfigurationSettings.AppSettings["CompanyType"].Split(',');
                Hashtable ct = new Hashtable();
                int i=1;
                foreach (string s in cts) {
                    ct.Add(i, s);
                    i++;
                }
                return ct;
            }
        }
        /// <summary>
        /// 专业属性
        /// </summary>
        public static Hashtable MajorType
        {
            get
            {
                string[] cts = System.Configuration.ConfigurationSettings.AppSettings["MajorType"].Split(',');
                Hashtable ct = new Hashtable();
                int i = 1;
                foreach (string s in cts)
                {
                    ct.Add(i, s);
                    i++;
                }
                return ct;
            }
        }
        /// <summary>
        /// 免费下载量
        /// </summary>
        public static int FreeDownCount = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["FreeDownCount"]);
        /// <summary>
        /// 最小起兑金额
        /// </summary>
        public static int MinExchange = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["MinExchange"]);
        /// <summary>
        /// 兑换抽成比例
        /// </summary>
        public static decimal ExchangeRatio = decimal.Parse(System.Configuration.ConfigurationSettings.AppSettings["ExchangeRatio"]);
        /// <summary>
        /// 文档转换保存的实际路径转换为URL需要替换的部分字符串
        /// </summary>
        public static string ConvertPathReplaceStr = System.Configuration.ConfigurationSettings.AppSettings["ConvertPathReplaceStr"];
        /// <summary>
        /// HubbleDotNet Connection
        /// </summary>
        public static string HubbleConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Search"].ConnectionString;
        /// <summary>
        /// 发送系统邮件的账号
        /// </summary>
        public static string SysEmailAddr = System.Configuration.ConfigurationSettings.AppSettings["SysEmailAddr"];
        /// <summary>
        /// 系统邮件服务器地址
        /// </summary>
        public static string MailHost = System.Configuration.ConfigurationSettings.AppSettings["MailHost"];
        /// <summary>
        /// 发送系统邮件的账号
        /// </summary>
        public static string MailUserName = System.Configuration.ConfigurationSettings.AppSettings["MailUserName"];
        /// <summary>
        /// 发送系统邮件的密码
        /// </summary>
        public static string MailPassWord = System.Configuration.ConfigurationSettings.AppSettings["MailPassWord"];
        /// <summary>
        /// 回复消息的分隔符
        /// </summary>
        public static string MsgSepChar = System.Configuration.ConfigurationSettings.AppSettings["MsgSepChar"];
        /// <summary>
        /// 上传后不需要转换的文档格式
        /// </summary>
        public static string[] NotConvertDocTypes = System.Configuration.ConfigurationSettings.AppSettings["NotConvertDocTypes"].Split(',');
    }
}
