using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类S_FriendLink 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class S_FriendLink
	{
		public S_FriendLink()
		{}
		#region Model
		private int _fid;
		private string _title;
		private string _linkurl;
		private int _orderid;
		/// <summary>
		/// 友情链接ID
		/// </summary>
		public int Fid
		{
			set{ _fid=value;}
			get{return _fid;}
		}
		/// <summary>
		/// 显示文本
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 链接地址
		/// </summary>
		public string LinkUrl
		{
			set{ _linkurl=value;}
			get{return _linkurl;}
		}
		/// <summary>
		/// 排序号
		/// </summary>
		public int OrderId
		{
			set{ _orderid=value;}
			get{return _orderid;}
		}
		#endregion Model

	}
}

