using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Service;
using TMM.Model;

namespace TMM.Core.Helper
{
    public class UserHelper
    {
        public static string GetUserNickName(int userId) {
            string r = string.Empty;
            UserService us = new UserService();
            us.Initialize();
            U_UserInfo u = us.UserInfoBll.Get(userId);
            if (u != null) {
                if (!string.IsNullOrEmpty(u.NickName))
                {
                    r = u.NickName;
                }
                else {
                    r = u.Email;
                }
            }
            return r;
        }
        public static U_UserInfo GetUserById(int userId) {
            UserService us = new UserService();
            us.Initialize();
            U_UserInfo u = us.UserInfoBll.Get(userId);
            return u;
        }

        public static MCatalog GetMyCatalog(int cataId) {
            UserService us = new UserService();
            us.Initialize();
            MCatalog mc = us.MCatalogBll.Get(cataId);
            return mc;
        }

        public static string GetCompanyType(int typeId) {
            foreach (object o in ConfigHelper.CommpanyType.Keys) {
                if (o.ToString() == typeId.ToString()) {
                    return ConfigHelper.CommpanyType[typeId].ToString();
                }
            }
            return "";
        }
        public static string GetMajorType(int typeId)
        {
            foreach (object o in ConfigHelper.MajorType.Keys)
            {
                if (o.ToString() == typeId.ToString())
                {
                    return ConfigHelper.MajorType[typeId].ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 格式化“回复消息”
        /// </summary>
        /// <returns></returns>
        public static string GetReplyMsg(M_Message msg,U_UserInfo sender)
        {
            string m = string.Empty;
            m = string.Format(
                "\r\n\r\n\r\n{3}以下是历史消息{3}\r\n{0}，原发件人：{1}\r\n\r\n{2}",
                msg.CreateTime.ToString(),sender.TmmDispName,msg.Content,
                ConfigHelper.MsgSepChar);
            return m;
        }
        /// <summary>
        /// 获取消息体里分隔符以前的部分
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetRecentMsgStr(string content)
        {
            if (content != null)
            {
                int a = content.IndexOf(ConfigHelper.MsgSepChar);
                if (a > 0)
                {
                    content = content.Substring(0, a);
                }
            }
            return content;
        }
        /// <summary>
        /// 是否特殊格式文档（即不能自动转换flash的文档）
        /// </summary>
        /// <param name="docType"></param>
        /// <returns></returns>
        public static bool IsSpecialType(string docType)
        {
            return ConfigHelper.NotConvertDocTypes.Contains(docType);
        }
    }
}
