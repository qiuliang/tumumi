using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类M_Bill 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class M_Bill
	{
		public M_Bill()
		{}
		#region Model
		private int _bid;
		private int _userid;
		private DateTime _createtime;
		private int _direct;
		private string _remark;
		private int _status;
		private decimal _price;
		/// <summary>
		/// 账单ID
		/// </summary>
		public int Bid
		{
			set{ _bid=value;}
			get{return _bid;}
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
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 账单方向
		/// </summary>
		public int Direct
		{
			set{ _direct=value;}
			get{return _direct;}
		}
		/// <summary>
		/// 账单备注
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 状态
		/// </summary>
		public int Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 发生金额
		/// </summary>
		public decimal Price
		{
			set{ _price=value;}
			get{return _price;}
		}
		#endregion Model

	}
}

