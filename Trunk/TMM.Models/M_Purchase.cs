using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类M_Purchase 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class MPurchase
	{
		public MPurchase()
		{}
		#region Model
		private int _pid;
		private int _userid;
		private int _docid;
		private string _title;
		private DateTime? _purchasetime;
		private decimal? _price;
        private int _saler;
		/// <summary>
		/// 购买ID
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
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
		/// 文档ID
		/// </summary>
		public int DocId
		{
			set{ _docid=value;}
			get{return _docid;}
		}
		/// <summary>
		/// 文档标题
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 购买时间
		/// </summary>
		public DateTime? PurchaseTime
		{
			set{ _purchasetime=value;}
			get{return _purchasetime;}
		}
		/// <summary>
		/// 消费金额
		/// </summary>
		public decimal? Price
		{
			set{ _price=value;}
			get{return _price;}
		}
		/// <summary>
		/// 上传人
		/// </summary>
		public int Saler
		{
			set{ _saler=value;}
			get{return _saler;}
		}
        /// <summary>
        /// 文档类型
        /// </summary>
        public string DocType { get; set; }
        /// <summary>
        /// 上传用户的信息
        /// </summary>
        public U_UserInfo UploaderInfo { get; set; }
		#endregion Model

	}
}

