using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Net;
using System.Net.Mail;
using TMM.Model;
using TMM.Core.Helper;
using TMM.Core.Extends;

namespace TMM.Core.Utils
{
    public class TmmUtils
    {
        /// <summary>
        /// 根据用户信息生成加密cookie验证字符串
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static string GenLoginFlagStr(U_UserInfo u){
            StringBuilder s = new StringBuilder();
            s.Append(u.UserId.ToString());
            s.Append(u.Email);
            s.Append(u.Password);
            return s.ToString().ToMd5();
        }
        /// <summary>
        /// 检查cookies登录信息是否正确
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static U_UserInfo CheckLoginCookie(int userId, string flag)
        {
            Service.Bll.User.U_UserInfoBLL bll = new TMM.Service.Bll.User.U_UserInfoBLL();
            U_UserInfo u = bll.Get(userId);

            if (u == null)
            {
                return null;
            }

            string nowFlag = GenLoginFlagStr(u);

            if (flag != nowFlag)
                return null;
            return u;
        }

        public static String GetStoreDir(int type,ref string virtualPath)
        {
            string fullPath = string.Empty;
            DateTime n = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder(ConfigHelper.uploadDir);
                if (type > 0)
                    sb.Append(type.ToString() + "\\");

                sb.Append(n.ToString("yyyy"))  //year
                  .Append('\\')
                  .Append(n.ToString("MM"))  //month
                  .Append('\\')
                  .Append(n.ToString("dd"))  //day
                  .Append('\\')
                  .Append(n.ToString("HH"))  //hour
                  .Append('\\');

                fullPath = sb.ToString();

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                virtualPath = fullPath.Replace(ConfigHelper.uploadDir, "").Replace("\\","/");
            }
            catch (Exception ex)
            {
                Log4Net.Error("StoreDir  " + ex.ToString());
            }

            return fullPath;
        }
        /// <summary>
        /// 生成文档保存的物理路径
        /// </summary>
        /// <returns></returns>
        public static String GetStoreDir()
        {
            string fullPath = string.Empty;
            DateTime n = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder(ConfigHelper.DocUploadDir);                

                sb.Append(n.ToString("yyyy"))  //year
                  .Append('\\')
                  .Append(n.ToString("MM"))  //month
                  .Append('\\')
                  .Append(n.ToString("dd"))  //day
                  .Append('\\')
                  .Append(n.ToString("HH"))  //hour
                  .Append('\\');

                fullPath = sb.ToString();

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                
            }
            catch (Exception ex)
            {
                Log4Net.Error("StoreDir  " + ex.ToString());
            }

            return fullPath;
        }

        public static string GetDocDownValKey(int userId,int docId,string fileId) {
            string valkey = string.Empty;
            valkey = (userId.ToString() + ConfigHelper.DownloadKey + docId.ToString() + fileId).ToMd5();
            return valkey;
        }
        /// <summary>
        /// 获取真实IP地址
        /// </summary>
        /// <returns></returns>
        public static string IPAddress()
        {
            string userIP;
            // HttpRequest Request = HttpContext.Current.Request;
            HttpRequest Request = HttpContext.Current.Request; // ForumContext.Current.Context.Request;
            // 如果使用代理，获取真实IP
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                userIP = Request.ServerVariables["REMOTE_ADDR"];
            else
                userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (userIP == null || userIP == "")
                userIP = Request.UserHostAddress;
            return userIP;
        }
        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns></returns>
        public static decimal GenOrderId() {
           return decimal.Parse(DateTime.Now.ToString("yyyyMMddHHmmss") + Utils.MyRandom.Next(0, 9999).ToString().PadLeft(4, '0'));
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        public static void SendEmail(string recieveAddr,string subject,string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(ConfigHelper.SysEmailAddr);
                mail.To.Add(new MailAddress(recieveAddr));
                mail.Subject = subject;
                mail.Body = body;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigHelper.MailHost;
                smtp.Credentials = new NetworkCredential(ConfigHelper.MailUserName, ConfigHelper.MailPassWord);
                smtp.Send(mail);

                mail.Dispose();
            }
            catch (Exception ex)
            {
                Log4Net.Error(ex);
            }

        }

        //根据某一分类ID，获取两级子分类的ID
        public static int[] GetSubCatalogIds(int catalogId)
        {
            TMM.Service.DocService ds = new TMM.Service.DocService();
            ds.Initialize();

            List<int> l = new List<int>();
            IList<S_Catalog> c1 = ds.SCatalogBll.GetSubList(catalogId);
            c1.ToList().ForEach(s =>
            {
                l.Add(s.CatalogId);
                s.SubCatalog.ToList().ForEach(ss => l.Add(ss.CatalogId));
            });
            if (l.Count > 0)
            {
                return l.ToArray();
            }
            else
                return null;
        }
        /// <summary>
        /// 根据某一分类ID，获取两级子分类的ID，如果没有子分类，则返回传入分类ID本身
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static int[] GetCatalogIds(int catalogId)
        {
            TMM.Service.DocService ds = new TMM.Service.DocService();
            ds.Initialize();

            List<int> l = new List<int>();
            l.Add(catalogId);

            IList<S_Catalog> c1 = ds.SCatalogBll.GetSubList(catalogId);
            c1.ToList().ForEach(s =>
            {
                l.Add(s.CatalogId);
                s.SubCatalog.ToList().ForEach(ss => l.Add(ss.CatalogId));
            });
            if (l.Count > 0)
            {
                return l.ToArray();
            }
            else
                return null;
        }

        public static string Post(string Url, string RequestString, int TimeoutSeconds)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            if (TimeoutSeconds > 0)
            {
                request.Timeout = 1000 * TimeoutSeconds;
            }
            request.Method = "POST";
            request.AllowAutoRedirect = true;

            byte[] data = System.Text.Encoding.GetEncoding("GBK").GetBytes(RequestString);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "";
            Stream outstream = request.GetRequestStream();

            outstream.Write(data, 0, data.Length);
            outstream.Close();

            HttpWebResponse hwrp = (HttpWebResponse)request.GetResponse();
            string ContentType = hwrp.Headers.Get("Content-Type");

            StreamReader sr = null;
            if (string.IsNullOrEmpty(ContentType))
                sr = new StreamReader(hwrp.GetResponseStream());
            else
            {
                if (ContentType.IndexOf("text/html") > -1)
                {
                    sr = new StreamReader(hwrp.GetResponseStream(), System.Text.Encoding.GetEncoding("GBK"));
                }
                else
                {
                    sr = new StreamReader(new GZipStream(hwrp.GetResponseStream(), CompressionMode.Decompress), System.Text.Encoding.GetEncoding("GBK"));
                }
            }

            return sr.ReadToEnd();
        }
    }
}
