using System;
namespace TMM.Model
{
    /// <summary>
    /// 实体类S_Config 。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class S_Config
    {
        public S_Config()
        { }
        #region Model
        private int _id;
        private string _webname;
        private string _keywords;
        private string _description;
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebName
        {
            set { _webname = value; }
            get { return _webname; }
        }
        /// <summary>
        /// SEO关键字
        /// </summary>
        public string Keywords
        {
            set { _keywords = value; }
            get { return _keywords; }
        }
        /// <summary>
        /// SEO描述
        /// </summary>
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }
        public int Id {
            get { return _id; }
            set { _id = value; }
        }

        public string CopyRight { get; set; }
        public string IcpNum { get; set; }
        #endregion Model

    }
}

