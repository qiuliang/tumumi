using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类D_Tag 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class D_Tag
	{
		public D_Tag()
		{}
		#region Model
		private int _tagid;
		private string _tag;
		private int _usecount;
		/// <summary>
		/// 标签ID
		/// </summary>
		public int TagId
		{
			set{ _tagid=value;}
			get{return _tagid;}
		}
		/// <summary>
		/// 标签内容
		/// </summary>
		public string Tag
		{
			set{ _tag=value;}
			get{return _tag;}
		}
		/// <summary>
		/// 使用次数
		/// </summary>
		public int UseCount
		{
			set{ _usecount=value;}
			get{return _usecount;}
		}
        /// <summary>
        /// 权重 1 代表最大 0 代表最小
        /// </summary>
        public decimal TagWeight { get; set; }
		#endregion Model

	}
}

