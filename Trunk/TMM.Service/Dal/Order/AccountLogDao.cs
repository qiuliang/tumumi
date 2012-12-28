﻿//==============================================================================
//	CAUTION: This file is generated by IBatisNetGen.DaoImpl.cst at 2010-12-17 22:06:26
//				By xincai.wu
//==============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMM.Model;
using TMM.Persistence;


namespace TMM.Service.Dal.Order {

	/// <summary>
    /// 名称：AccountLogDao 数据层
    /// 开发者：
    /// 创建时间：2011-1-9 11:03:46
    /// 功能描述：
    /// </summary>
    public partial class AccountLogDal {

		/// <summary>
        /// 取得记录数
        /// </summary>
        /// <param name="param">可选参数为：</param>
        /// <returns></returns>
		public int GetCount(Hashtable param) {
			if (param == null)
                param = new Hashtable();
				
			String stmtId = "AccountLog.GetCount";
			return SqlMapper.Instance().QueryForObject<int>(stmtId, param);
		}

		/// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="param">可选参数为：</param>
        /// <param name="orderBy">排序方式:默认为“LogId asc”</param>
        /// <param name="first">起始记录：从0开始</param>
        /// <param name="rows">提取的条数</param>
        /// <returns></returns>
		public IList<AccountLog> GetList(Hashtable param,string orderBy,int first,int rows) 
		{
			if (param == null)
                param = new Hashtable();

            param.Add("Top", first+ rows);
            param.Add("StartRecord", first);

            if (string.IsNullOrEmpty(orderBy))
                param.Add("OrderBy", "LogId DESC");
			else
                param.Add("OrderBy", orderBy);
				
			String stmtId = "AccountLog.GetList";
			return SqlMapper.Instance().QueryForList<AccountLog>(stmtId, param);
		}
		
		/// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
		public AccountLog Get(Int32 logId) {
			String stmtId = "AccountLog.Get";
			return SqlMapper.Instance().QueryForObject<AccountLog>(stmtId, logId);
		}

		/// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回：该条数据的主键Id</returns>
		public int Insert(AccountLog obj) {
			if (obj == null) throw new ArgumentNullException("obj");
			String stmtId = "AccountLog.Insert";
			return SqlMapper.Instance().QueryForObject<int>(stmtId, obj);
		}
		
		/// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回：ture 成功，false 失败</returns>
		public bool Update(AccountLog obj) {
			if (obj == null) throw new ArgumentNullException("obj");
			String stmtId = "AccountLog.Update";
			int result = SqlMapper.Instance().QueryForObject<int>(stmtId, obj);
			return result > 0 ? true : false;
		}
		
		/// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="logId"></param>
        /// <returns>返回：ture 成功，false 失败</returns>
		public bool Delete(Int32 logId) {
			String stmtId = "AccountLog.Delete";
			int result = SqlMapper.Instance().QueryForObject<int>(stmtId, logId);
			return result > 0 ? true : false;
		}

        public decimal SumIncome(int userId) {
            String stmtId = "AccountLog.SumIncome";
            decimal result = SqlMapper.Instance().QueryForObject<decimal>(stmtId, userId);
            return result;
        }
		
	}

}