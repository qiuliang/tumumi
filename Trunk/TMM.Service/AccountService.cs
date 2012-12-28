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
    public class AccountService : IServiceEnabledComponent, IInitializable
    {
        private IServiceProvider provider;
        
        

        #region IInitializable Members

        public void Initialize()
        {
            MPurchaseBll = new TMM.Service.Bll.User.MPurchaseBLL();
            MAccountBll = new TMM.Service.Bll.User.MAccountBLL();
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
        /// ¹ºÂò¼ÇÂ¼
        /// </summary>
        public Bll.User.MPurchaseBLL MPurchaseBll;
        /// <summary>
        /// ÕË»§
        /// </summary>
        public Bll.User.MAccountBLL MAccountBll;
       

    }
}
