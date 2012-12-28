using System.Xml;
using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;

namespace TMM.Persistence
{
    public class SqlMapper
    {
        private static volatile ISqlMapper _mapper = null;

        protected static void Configure(object obj)
        {
            _mapper = null;
        }

        protected static void InitMapper()
        {
            ConfigureHandler handler = new ConfigureHandler(Configure);
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            /*
            XmlDocument sqlMapConfig = Resources.GetEmbeddedResourceAsXmlDocument("MamShare.Persistence.Config.SqlMap.config, MamShare.Persistence");
            _mapper = builder.Configure(sqlMapConfig);
            */
            _mapper = builder.ConfigureAndWatch("SqlMap.config",handler);
        }
        
        public static ISqlMapper Instance()
        {
            if (_mapper == null)
            {
                lock (typeof(SqlMapper))
                {
                    if (_mapper == null) // double-check
                    {
                        InitMapper();
                    }
                }
            }
            return _mapper;
        }

        public static ISqlMapper Get()
        {
            return Instance();
        }

    }
}
