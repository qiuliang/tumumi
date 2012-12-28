using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类N_News 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class N_News
	{
		public N_News()
		{}
		#region Model
		private int _nid;
		private string _title;
		private string _content;
		private string _catalog;
		/// <summary>
		/// 新闻ID
		/// </summary>
		public int Nid
		{
			set{ _nid=value;}
			get{return _nid;}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 内容
		/// </summary>
		public string Content
		{
			set{ _content=value;}
			get{return _content;}
		}
		/// <summary>
		/// 新闻分类
		/// </summary>
		public string Catalog
		{
			set{ _catalog=value;}
			get{return _catalog;}
		}
		#endregion Model

	}
}

