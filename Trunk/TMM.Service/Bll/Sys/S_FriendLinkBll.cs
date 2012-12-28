using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMM.Model;

namespace TMM.Service.Bll.Sys
{
    public class S_FriendLinkBll
    {
        private Dal.Sys.S_FriendLinkDal dal = new TMM.Service.Dal.Sys.S_FriendLinkDal();

        public void Insert(S_FriendLink model)
        {
            dal.Insert(model);
        }

        public IList<S_FriendLink> GetList() {
            return dal.GetList();
        }

        public S_FriendLink Get(int fid) {
            return dal.Get(fid);
        }
        public void Update(S_FriendLink obj) {
            dal.Update(obj);
        }
        public void Delete(int fid) {
            dal.Delete(fid);
        }
    }
}
