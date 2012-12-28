using System;
using System.Collections.Generic;
namespace TMM.Model
{
	/// <summary>
	/// 实体类S_Catalog 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class S_Catalog
	{
		public S_Catalog()
		{}
		#region Model
		private int _catalogid;
		private string _catalogname;
		private int? _pid;
		private int _orderid;
        private IList<S_Catalog> subCatalog;
		/// <summary>
		/// 分类ID
		/// </summary>
		public int CatalogId
		{
			set{ _catalogid=value;}
			get{return _catalogid;}
		}
		/// <summary>
		/// 分类名称
		/// </summary>
		public string CatalogName
		{
			set{ _catalogname=value;}
			get{return _catalogname;}
		}
		/// <summary>
		/// 父ID
		/// </summary>
		public int? Pid
		{
			set{ _pid=value;}
			get{return _pid;}
		}
		/// <summary>
		/// 排序号
		/// </summary>
		public int OrderId
		{
			set{ _orderid=value;}
			get{return _orderid;}
		}
        /// <summary>
        /// 子分类
        /// </summary>
        public IList<S_Catalog> SubCatalog
        {
            get { return subCatalog; }
            set { subCatalog = value; }
        }
		#endregion Model

	}
}

