using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Model;

namespace TMM.Service.Bll.Sys
{
    public class S_ConfigBll
    {
        private Dal.Sys.S_ConfigDal dal = new TMM.Service.Dal.Sys.S_ConfigDal();



        public S_Config Get()
        {
            return dal.Get();
        }
        public void Update(S_Config obj) {
            dal.Update(obj);
        }
        public void Insert(S_Config obj) {
            dal.Insert(obj);
        }
    }
}
