using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类T_ReqDoc 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class TReqDoc
	{
		public TReqDoc()
		{}
		#region Model
		private int _tid;
		private int _userid;
		private string _title;
		private string _description;
		private decimal _price;
		private DateTime _createtime;
		private DateTime _endtime;
		private int _status;
		/// <summary>
		/// 悬赏ID
		/// </summary>
		public int TId
		{
			set{ _tid=value;}
			get{return _tid;}
		}
		/// <summary>
		/// 用户ID
		/// </summary>
		public int UserId
		{
			set{ _userid=value;}
			get{return _userid;}
		}
		/// <summary>
		/// 悬赏标题
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public string Description
		{
			set{ _description=value;}
			get{return _description;}
		}
		/// <summary>
		/// 悬赏金额
		/// </summary>
		public decimal Price
		{
			set{ _price=value;}
			get{return _price;}
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
		/// 截止时间
		/// </summary>
		public DateTime EndTime
		{
			set{ _endtime=value;}
			get{return _endtime;}
		}
		/// <summary>
		/// 状态 1：待审核 2：通过审核
		/// </summary>
		public int Status
		{
			set{ _status=value;}
			get{return _status;}
		}
        public int DocCount { get; set; }
        public U_UserInfo Publisher { get; set; }
		#endregion Model

	}
}

