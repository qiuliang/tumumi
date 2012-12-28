using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类D_Comment 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class D_Comment
	{
		public D_Comment()
		{}
		#region Model
		private int _commentid;
		private int _docid;
		private string _content;
		private DateTime _createtime;
		private int _userid;
		/// <summary>
		/// 评论ID
		/// </summary>
		public int CommentId
		{
			set{ _commentid=value;}
			get{return _commentid;}
		}
		/// <summary>
		/// 文档ID
		/// </summary>
		public int DocId
		{
			set{ _docid=value;}
			get{return _docid;}
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
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 评论人
		/// </summary>
		public int UserId
		{
			set{ _userid=value;}
			get{return _userid;}
		}
        public bool IsDisp { get; set; }

        public U_UserInfo UserInfo { get; set; }
		#endregion Model

	}
}

