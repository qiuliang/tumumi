using System;
using System.Collections;
using Castle.Core;
using System.Collections.Generic;

using TMM.Model;
using TMM.Persistence;
using TMM.Service.Bll.Sys;
using TMM.Service.Bll;

namespace TMM.Service
{
    public class UserService : IServiceEnabledComponent, IInitializable
    {
        private IServiceProvider provider;
        
        

        #region IInitializable Members

        public void Initialize()
        {
            UserInfoBll = new TMM.Service.Bll.User.U_UserInfoBLL();
            MessageBll = new TMM.Service.Bll.User.M_MessageBLL();
            DocInfoBll = new TMM.Service.Bll.Doc.DDocInfoBLL();
            MCatalogBll = new TMM.Service.Bll.Doc.MCatalogBLL();
            MFavoriteBll = new TMM.Service.Bll.Doc.MFavoriteBLL();
            DTagBll = new TMM.Service.Bll.Doc.D_TagBLL();
            MPurchaseBll = new TMM.Service.Bll.User.MPurchaseBLL();
            MAccountBll = new TMM.Service.Bll.User.MAccountBLL();
            TReqDocBll = new TMM.Service.Bll.Doc.TReqDocBLL();
            AccountLogBll = new TMM.Service.Bll.Order.AccountLogBLL();
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
        /// 用户
        /// </summary>
        public Bll.User.U_UserInfoBLL UserInfoBll;
        /// <summary>
        /// 消息
        /// </summary>
        public Bll.User.M_MessageBLL MessageBll;
        /// <summary>
        /// 文档
        /// </summary>
        public Bll.Doc.DDocInfoBLL DocInfoBll;
        /// <summary>
        /// 个人文件夹
        /// </summary>
        public Bll.Doc.MCatalogBLL MCatalogBll;
        /// <summary>
        /// 个人收藏
        /// </summary>
        public Bll.Doc.MFavoriteBLL MFavoriteBll;
        /// <summary>
        /// 标签
        /// </summary>
        public Bll.Doc.D_TagBLL DTagBll;
        /// <summary>
        /// 我的购买
        /// </summary>
        public Bll.User.MPurchaseBLL MPurchaseBll;
        /// <summary>
        /// 个人账户
        /// </summary>
        public Bll.User.MAccountBLL MAccountBll;
        /// <summary>
        /// 账户日志
        /// </summary>
        public Bll.Order.AccountLogBLL AccountLogBll;
        /// <summary>
        /// 悬赏文档
        /// </summary>
        public Bll.Doc.TReqDocBLL TReqDocBll;
    }
}
