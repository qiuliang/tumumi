using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMM.Model;

namespace TMM.Service.Bll.Sys {

	/// <summary>
    /// 名称：SCatalogDao 逻辑层
    /// 创建者：
    /// 创建时间：2010-11-29 19:12:38
    /// 功能描述：
    /// </summary>
    public partial class S_CatalogBLL {

        Dal.Sys.S_CatalogDal dal = new Dal.Sys.S_CatalogDal();

		/// <summary>
        /// 取得记录数
        /// </summary>
        /// <param name="param">可选参数为：</param>
        /// <returns></returns>
		public int GetCount(Hashtable param) {
			return dal.GetCount(param);
		}

		/// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="param">可选参数为：</param>
        /// <param name="orderBy">排序方式:默认为“CatalogId asc”</param>
        /// <param name="first">起始记录：从0开始</param>
        /// <param name="rows">提取的条数</param>
        /// <returns></returns>
		public IList<S_Catalog> GetList(Hashtable param,string orderBy,int? first,int? rows) 
		{
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "OrderId asc";
			return dal.GetList(param, orderBy, first, rows);
		}
		
		/// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
		public S_Catalog Get(Int32 catalogId) {
			return dal.Get(catalogId);
		}

		/// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回：该条数据的主键Id</returns>
		public int Insert(S_Catalog obj) {
			return dal.Insert(obj);
		}
		
		/// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回：ture 成功，false 失败</returns>
		public bool Update(S_Catalog obj) {
			return dal.Update(obj);
		}
		
		/// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns>返回：ture 成功，false 失败</returns>
		public bool Delete(Int32 catalogId) {
			return dal.Delete(catalogId);
		}

        public List<S_Catalog> GetTopNode() { 
            Hashtable p = new Hashtable();
            p.Add("Pid",0);
            IList<S_Catalog> topNodes = GetList(p, null, null, null);
            foreach (S_Catalog c in topNodes) { 
                p.Clear();
                p.Add("Pid",c.CatalogId);
                c.SubCatalog = GetList(p, null, null, null);
                foreach (S_Catalog c3 in c.SubCatalog) {
                    p.Clear();
                    p.Add("Pid",c3.CatalogId);
                    c3.SubCatalog = GetList(p, null, null, null);
                }
            }
            return topNodes.ToList();
        }
        /// <summary>
        /// 根据当前分类ID，往下找两级子分类
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public IList<S_Catalog> GetSubList(int catalogId) 
        {
            Hashtable p = new Hashtable();
            p.Add("Pid",catalogId);
            IList<S_Catalog> sublist = GetList(p, null, null, null);
            sublist.ToList().ForEach(c => {
                p.Clear();
                p.Add("Pid",c.CatalogId);
                c.SubCatalog = GetList(p, null, null, null);
            });
            return sublist;
        }
		
	}

}
