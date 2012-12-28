using System;
namespace TMM.Model
{
	/// <summary>
	/// 实体类D_FileInfo 。(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class D_FileInfo
	{
		public D_FileInfo()
		{}
		#region Model
		private Guid _fid;
		private int? _filetype;
		private string _filename;
		private int _ownerid;
		private string _filepath;
		private DateTime _createtime;
		/// <summary>
		/// 文件ID
		/// </summary>
		public Guid Fid
		{
			set{ _fid=value;}
			get{return _fid;}
		}
		/// <summary>
		/// 文件类型
		/// </summary>
		public int? FileType
		{
			set{ _filetype=value;}
			get{return _filetype;}
		}
		/// <summary>
		/// 文件原始名称
		/// </summary>
		public string FileName
		{
			set{ _filename=value;}
			get{return _filename;}
		}
		/// <summary>
		/// 所有者ID
		/// </summary>
		public int OwnerId
		{
			set{ _ownerid=value;}
			get{return _ownerid;}
		}
		/// <summary>
		/// 文件路径
		/// </summary>
		public string FilePath
		{
			set{ _filepath=value;}
			get{return _filepath;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		#endregion Model

	}
}

