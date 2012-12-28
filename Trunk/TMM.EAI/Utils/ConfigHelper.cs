using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TMM.EAI.Utils
{
    public class ConfigHelper
    {
        public static string SqlConnStr;
        /// <summary>
        /// 马甲用户ID列表
        /// </summary>
        public static int[] UserIds;

        public static string SaveFile(FileInfo fi, ref string ext,string rootPath)
        {
            ext = fi.FullName.Substring(fi.FullName.LastIndexOf(".") + 1).ToLower();
            
            string newFileName = Guid.NewGuid().ToString("N");
            
            string savePath = GetStoreDir(rootPath) + newFileName + "." + ext;
            fi.CopyTo(savePath);
            return savePath;
        }

        private static String GetStoreDir(string rootPath)
        {
            string fullPath = string.Empty;
            DateTime n = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder(rootPath);

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
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

            return fullPath;
        }
    }
}
