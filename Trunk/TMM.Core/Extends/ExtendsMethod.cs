using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace TMM.Core.Extends
{
    public static class ExtendsMethod
    {
        /// <summary>
        /// 返回URL解码字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUrlDecode(this string input) {
            return HttpContext.Current.Server.UrlDecode(input);
        }
        public static string ToMd5(this string input) {
            byte[] b = System.Text.Encoding.Default.GetBytes(input);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将字符串数组转换为带符号分隔的字符串
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static string ToStringBySpliter(this string[] inputs,string spliter) {
            StringBuilder s = new StringBuilder();
            inputs.Each(
                (a, i) => s.Append(a + ( i == inputs.Length - 1 ? "" : spliter ) ) 
            );
            return s.ToString();
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T, int> handler) {
            int i = 0;
            foreach (var t in source) {                
                handler(t, i++);
            }
        }
        /// <summary>
        /// 将\r\n替换成br
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceEnterStr(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace(Convert.ToChar(10).ToString(),"<br/>");
        }
        /// <summary>
        /// 过滤字符串中的html
        /// </summary>
        /// <param name="txtString"></param>
        /// <returns></returns>
        public static string FilterHtml(this string txtString)
        {
            string _txtString = string.Empty;
            if (!string.IsNullOrEmpty(txtString))
            {
                _txtString = Regex.Replace(txtString.Trim(), @"<[^>]+>", string.Empty, RegexOptions.IgnoreCase);

                _txtString = Regex.Replace(_txtString, @"<script[\s\s]+</script *>", string.Empty, RegexOptions.IgnoreCase);
                _txtString = Regex.Replace(_txtString, @" href *= *[\s\s]*script *:", string.Empty, RegexOptions.IgnoreCase);
                _txtString = Regex.Replace(_txtString, @" on[\s\s]*=", string.Empty, RegexOptions.IgnoreCase);
                _txtString = Regex.Replace(_txtString, @"<iframe[\s\s]+</iframe *>", string.Empty, RegexOptions.IgnoreCase);
                _txtString = Regex.Replace(_txtString, @"<frameset[\s\s]+</frameset *>", string.Empty, RegexOptions.IgnoreCase);
            }
            return _txtString;
        }
    }
}
