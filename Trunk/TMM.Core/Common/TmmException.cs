using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMM.Core.Common
{
    /// <summary>
    /// 自定义异常类
    /// </summary>
    public class TmmException : ApplicationException
    {
        public TmmException(string friendlyMsg) : base(friendlyMsg){
            friendlyMsg += "TMM Error : ";
        }

        
    }
}
