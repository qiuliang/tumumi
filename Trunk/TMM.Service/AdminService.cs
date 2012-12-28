using System;
using System.Collections;
using Castle.Core;
using System.Collections.Generic;

using TMM.Model;
using TMM.Persistence;
using TMM.Service.Bll;
using TMM.Service.Bll.Admin;

namespace TMM.Service
{
    public class AdminService : IServiceEnabledComponent, IInitializable
    {
        private IServiceProvider provider;
        
        

        #region IInitializable Members

        public void Initialize()
        {
            ManageUserBll = new ManageUserBLL();
            MessageBll = new TMM.Service.Bll.User.M_MessageBLL();
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
        /// 管理员账号
        /// </summary>
        public ManageUserBLL ManageUserBll;
        /// <summary>
        /// 消息管理
        /// </summary>
        public Bll.User.M_MessageBLL MessageBll;
       

    }
}
