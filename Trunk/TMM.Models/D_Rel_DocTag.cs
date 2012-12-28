using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类D_Rel_DocTag 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class D_Rel_DocTag
	{
		public D_Rel_DocTag()
		{}
		#region Model
		private int _docid;
		private int _tagid;
		/// <summary>
		/// 文档ID
		/// </summary>
		public int DocId
		{
			set{ _docid=value;}
			get{return _docid;}
		}
		/// <summary>
		/// 标签ID
		/// </summary>
		public int TagId
		{
			set{ _tagid=value;}
			get{return _tagid;}
		}
        public int Id { get; set; }
		#endregion Model

	}
}

