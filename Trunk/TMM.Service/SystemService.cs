using System;
using System.Collections;
using Castle.Core;
using System.Collections.Generic;

using TMM.Model;
using TMM.Persistence;
using TMM.Service.Bll.Sys;

namespace TMM.Service
{
    public class SystemService : IServiceEnabledComponent, IInitializable
    {
        private IServiceProvider provider;
        
        

        #region IInitializable Members

        public void Initialize()
        {
            SFriendLinkBll = new S_FriendLinkBll();
            SConfigBll = new S_ConfigBll();
            SCatalogBll = new S_CatalogBLL();
        }

        #endregion

        #region IServiceEnabledComponent Members

        public void Service(IServiceProvider provider)
        {
            this.provider = provider;
            
        }
	    public void initService()
	    {
		    
	    }

        #endregion

        /// <summary>
        /// 友情链接
        /// </summary>
        public S_FriendLinkBll SFriendLinkBll;
        /// <summary>
        /// 系统全局配置
        /// </summary>
        public S_ConfigBll SConfigBll;
        /// <summary>
        /// 系统分类
        /// </summary>
        public S_CatalogBLL SCatalogBll;

    }
}
