using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMM.Model.Enums
{
    public enum MessageType { 
        /// <summary>
        /// 系统消息
        /// </summary>
        SysMsg = 1,
        /// <summary>
        /// 通知
        /// </summary>
        Inform,
        /// <summary>
        /// 普通消息
        /// </summary>
        Message
    }

    public enum RegType { 
        Default = 0,
        OAuthQQ = 1,
        OAuthSina = 2
    }
}
