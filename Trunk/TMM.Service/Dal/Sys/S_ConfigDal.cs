using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Model;
using TMM.Persistence;

namespace TMM.Service.Dal.Sys
{
    public class S_ConfigDal
    {
       
        public S_Config Get() {
            Hashtable p = new Hashtable();
            p.Add("Top",1);
            return SqlMapper.Get().QueryForObject<S_Config>("S_Config.GetList", p);
        }
        public void Update(S_Config obj)
        {
            SqlMapper.Get().Update("S_Config.Update", obj);
        }
        public void Insert(S_Config obj) {
            SqlMapper.Get().Insert("S_Config.Insert", obj);
        }
    }
}
