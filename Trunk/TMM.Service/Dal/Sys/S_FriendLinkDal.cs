using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Model;
using TMM.Persistence;

namespace TMM.Service.Dal.Sys
{
    public class S_FriendLinkDal
    {
        public void Insert(S_FriendLink model) {
            TMM.Persistence.SqlMapper.Get().Insert("S_FriendLink.Insert", model);
        }

        public IList<S_FriendLink> GetList() {
            return SqlMapper.Get().QueryForList<S_FriendLink>("S_FriendLink.GetList", null);
        }

        public S_FriendLink Get(int fid) {
            return SqlMapper.Get().QueryForObject<S_FriendLink>("S_FriendLink.Get", fid);
        }

        public void Update(S_FriendLink obj) {
            SqlMapper.Get().Update("S_FriendLink.Update", obj);
        }
        public void Delete(int fid) {
            SqlMapper.Get().Delete("S_FriendLink.Delete", fid);
        }
    }
}
