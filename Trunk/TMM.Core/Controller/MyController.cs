using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using TMM.Model;
using TMM.Service;
using TMM.Core.Helper;
using TMM.Core.Common;
using TMM.Core.Extends;


namespace TMM.Core.Controller
{
    [Filter(ExecuteEnum.BeforeAction,typeof(Filter.MyTmmFilter))]
    [Helper(typeof(UserHelper))]
    [Helper(typeof(FormatHelper))]
    public class MyController : BaseController
    {
        
        public void Index(int first) {
            Redirect("MyDoc.do");
            return;
        }
        /// <summary>
        /// 我的文档
        /// </summary>
        public void MyDoc(int first,string keyword,int? userCataId) {
            PropertyBag["cur_page_index"] = true;
            int rows = 10;
            PropertyBag["cur_page_index"] = true;
            UserService us = Context.GetService<UserService>();
            U_UserInfo logonUser = base.GetUser();
            int count = 0;
            IList<DDocInfo> list = us.DocInfoBll.GetListByUser(logonUser.UserId, out count, first, rows,keyword,userCataId,null);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
            //总文档量
            PropertyBag["allDocCount"] = count;
            //未分类文档总数
            PropertyBag["unCatalogDocCount"] = us.DocInfoBll.GetCataDocCount(logonUser.UserId, 0); 
            //总浏览量
            PropertyBag["allViewCount"] = us.DocInfoBll.GetViewCount(logonUser.UserId);
            //我的文件夹列表
            PropertyBag["folders"] = us.MCatalogBll.GetListByUser(logonUser.UserId, 1);
            if (userCataId.HasValue) {
                if (userCataId.Value != 0)
                {
                    MCatalog mc = us.MCatalogBll.Get(userCataId.Value);
                    PropertyBag["mc"] = mc;
                }
            }

            RenderView("mydoc");
        }
        public void DoMydoc(string opType, string folderName, string docIds, int applyFolderId, int folderType)
        {
            try
            {
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                switch (opType.ToLower())
                {
                    #region 新增文件夹
                    case "addfolder":
                        MCatalog mc = new MCatalog() { 
                            CateText = folderName,
                            CatalogType = folderType,
                            CreateTime = DateTime.Now,
                            DocCount = 0,
                            UserId = u.UserId
                        };
                        us.MCatalogBll.Insert(mc);
                        
                        break;
                    #endregion
                    #region 删除文件夹
                    case "deletefolder":
                        MCatalog dmc = us.MCatalogBll.Get(applyFolderId, u.UserId);
                        if (dmc != null)
                        {
                            us.MCatalogBll.Delete(dmc.CateId);
                            us.DocInfoBll.UpdateUserCatalog(u.UserId, dmc.CateId);
                        }
                        else {
                            throw new Exception("操作失败");
                        }
                        break;
                    #endregion
                    #region 重命名文件夹
                    case "updatefolder":
                        MCatalog dmc2 = us.MCatalogBll.Get(applyFolderId, u.UserId);
                        if (dmc2 != null)
                        {
                            dmc2.CateText = folderName;
                            us.MCatalogBll.Update(dmc2);
                        }
                        else
                        {
                            throw new Exception("操作失败");
                        }
                        break;
                    #endregion
                    #region 多选删除
                    case "deleteall":
                        string[] strDocIds = docIds.Split(',');
                        strDocIds.ToList().ForEach(s => {
                            DDocInfo d = us.DocInfoBll.Get(int.Parse(s), u.UserId);
                            if (d != null)
                            {
                                us.DocInfoBll.Delete(d.DocId);
                                if (d.UserCateId != 0)
                                {
                                    //文件夹文档数量减一
                                    MCatalog dsmc = us.MCatalogBll.Get(d.UserCateId);
                                    dsmc.DocCount -= 1;
                                    us.MCatalogBll.Update(dsmc);
                                }
                            }
                            else
                                throw new Exception("可能删除了不是自己的文档");
                        });
                        break;
                    #endregion
                    #region 移动到文件夹
                    case "moveall":
                        string[] strDocIds2 = docIds.Split(',');
                        strDocIds2.ToList().ForEach(s => {
                            us.DocInfoBll.MoveFolder(int.Parse(s), applyFolderId);
                        });
                        break;
                    #endregion
                    #region 单个删除
                    case "deletesingle":
                        string[] strDocIds3 = docIds.Split(',');
                        strDocIds3.ToList().ForEach(s => {
                            DDocInfo d = us.DocInfoBll.Get(int.Parse(s), u.UserId);
                            if (d != null)
                            {
                                us.DocInfoBll.Delete(d.DocId);
                                if (d.UserCateId != 0) { 
                                    //文件夹文档数量减一
                                    MCatalog dsmc = us.MCatalogBll.Get(d.UserCateId);
                                    dsmc.DocCount -= 1;
                                    us.MCatalogBll.Update(dsmc);
                                }
                            }
                            else
                                throw new Exception("可能删除了不是自己的文档");
                        });
                        break;
                    #endregion
                    #region 移动到收藏夹
                    case "moveallfav":
                        string[] strDocIds4 = docIds.Split(',');
                        strDocIds4.ToList().ForEach(s =>
                        {                            
                            us.MFavoriteBll.MoveFolder(int.Parse(s), applyFolderId);
                        });
                        break;
                    #endregion
                    #region 删除收藏
                    case "deleteallfav":
                        string[] strDocIds5 = docIds.Split(',');
                        strDocIds5.ToList().ForEach(s =>
                        {
                            us.MFavoriteBll.Delete(int.Parse(s));
                        });
                        break;
                    #endregion
                }
                AddSuccess("操作成功");
            }
            catch(Exception ex) {
                Utils.Log4Net.Error(ex);
                AddError("操作失败");
            }
            RedirectToReferrer();
        }
        /// <summary>
        /// 个人中心--编辑文档
        /// </summary>
        /// <param name="docId"></param>
        public void EditDoc(int docId) {
            UserService us = Context.GetService<UserService>();
            U_UserInfo u = base.GetUser();
            DDocInfo doc = us.DocInfoBll.Get(docId, u.UserId);
            if (doc.IsAudit)
                throw new TmmException("该文档已经发布，不能修改");
            PropertyBag["doc"] = doc;
            SystemService ss = Context.GetService<SystemService>();
            IList<S_Catalog> cates = ss.SCatalogBll.GetTopNode();
            PropertyBag["cates"] = cates;
        }

        /// <summary>
        /// 我的收藏
        /// </summary>
        public void MyFav(int first, int? favCataId)
        {
            int rows = 10;
            PropertyBag["cur_page_fav"] = true;
            UserService us = Context.GetService<UserService>();
            U_UserInfo logonUser = base.GetUser();
            int count = 0;
            
            IList<MFavorite> list = us.MFavoriteBll.GetListByUserId(first, rows, out count, logonUser.UserId, favCataId);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
            
            //未分类文档总数
            PropertyBag["unCatalogDocCount"] = us.MFavoriteBll.GetCataDocCount(logonUser.UserId, 0);
            
            //我的文件夹列表
            PropertyBag["folders"] = us.MCatalogBll.GetListByUser(logonUser.UserId, 2);
            if (favCataId.HasValue)
            {
                if (favCataId.Value != 0)
                {
                    MCatalog mc = us.MCatalogBll.Get(favCataId.Value);
                    PropertyBag["mc"] = mc;
                }
            }            
        }

        /// <summary>
        /// 消息
        /// </summary>
        public void Message() {
            PropertyBag["cur_page_msg"] = true;
            RenderView("sysmsg");
        }
        /// <summary>
        /// 系统公告
        /// </summary>
        public void SysMsg(int first) {
            PropertyBag["cur_page_msg"] = true;
            UserService us = Context.GetService<UserService>();
            IList<M_Message> mlist = us.MessageBll.GetSysMessageList(first, 10);

            mlist.ToList().ForEach(m => m.Content = m.Content.Replace(Convert.ToChar(10).ToString(), "<br/>"));
            Hashtable p = new Hashtable();
            p.Add("Mtype", (int)Model.Enums.MessageType.SysMsg);
            int count = us.MessageBll.GetCount(p);
            Common.ListPage lp = new TMM.Core.Common.ListPage((IList)mlist, first, 10, count);
            PropertyBag["listPage"] = lp;
        }
        /// <summary>
        /// 通知
        /// </summary>
        public void SysInform(int first) {
            PropertyBag["cur_page_msg"] = true;
            try {
                int rows = 10;
                UserService us = Context.GetService<UserService>();
                U_UserInfo logonUser = base.GetUser();
                int count = 0;
                IList<M_Message> mlist = us.MessageBll.GetInforms(first, rows, out count, logonUser.UserId);

                ListPage lp = new ListPage((IList)mlist, first, rows, count);
                PropertyBag["listPage"] = lp;
            }
            catch(Exception ex)
            {
                Utils.Log4Net.Error(ex);
            }
        }
        public void DoInform(string mids,string optype,string returnUrl) {
            UserService us = Context.GetService<UserService>();
            string[] messageIds = mids.Split(',');
            switch (optype.ToLower()) { 
                case "deleteall" :                    
                    messageIds.ToList().ForEach(id => us.MessageBll.SetDeleteFlagByReciever(int.Parse(id)));
                    AddSuccess("操作成功");
                    break;
                case "flagall":
                    messageIds.ToList().ForEach(id => us.MessageBll.SetRead(int.Parse(id)));
                    AddSuccess("操作成功");
                    break;
                case "delete":
                    messageIds.ToList().ForEach(id => us.MessageBll.SetDeleteFlagByReciever(int.Parse(id)));
                    AddSuccess("操作成功");
                    break;
                case "deletebysender" :
                    messageIds.ToList().ForEach(id => us.MessageBll.SetDeleteFlagBySender(int.Parse(id)));
                    AddSuccess("操作成功");
                    break;
            }
            if (string.IsNullOrEmpty(returnUrl))
                RedirectToReferrer();
            else
                Redirect(returnUrl);

        }
        /// <summary>
        /// 接收到的消息
        /// </summary>
        public void RecieveMsg(int first) {
            PropertyBag["cur_page_msg"] = true;
            U_UserInfo logonUser = base.GetUser();
            UserService us = Context.GetService<UserService>();
            int count = 0;
            IList<M_Message> mlist = us.MessageBll.GetUserMsgList(first,10,out count,logonUser.UserId);               

            ListPage lp = new ListPage((IList)mlist, first, 10, count);
            PropertyBag["listPage"] = lp;

        }
        /// <summary>
        /// 发送的消息
        /// </summary>
        public void SentMsg(int first) {
            PropertyBag["cur_page_msg"] = true;
            int rows = 10;
            U_UserInfo logonUser = base.GetUser();
            UserService us = Context.GetService<UserService>();
            int count = 0;
            IList<M_Message> mlist = us.MessageBll.GetListByUser(first,rows,out count,logonUser.UserId);               
            //mlist.ToList().ForEach(m => m.Content = m.Content.ReplaceEnterStr());
            ListPage lp = new ListPage((IList)mlist, first, rows, count);
            PropertyBag["listPage"] = lp;
        }
        /// <summary>
        /// 给某人发消息
        /// </summary>
        /// <param name="recieverId"></param>
        public void SendMsg(int recieverId) {
            UserService us = Context.GetService<UserService>();
            PropertyBag["reciever"] = us.UserInfoBll.Get(recieverId);

        }
        /// <summary>
        /// 给系统发消息
        /// </summary>
        public void SendTo(int first) {
            PropertyBag["cur_page_msg"] = true;
            //历史消息
            U_UserInfo logonUser = base.GetUser();
            UserService us = Context.GetService<UserService>();
            int count = 0;
            IList<M_Message> mlist = us.MessageBll.GetListSendToAdmin(
                logonUser.UserId, ConfigHelper.AdminUserId, first, 10,out count);
            mlist.ToList().ForEach(m=>m.Content = m.Content.ReplaceEnterStr());
            ListPage lp = new ListPage((IList)mlist, first, 10, count);
            PropertyBag["listPage"] = lp;
        }
        public void DoSendTo([DataBind("M_Message")]M_Message msg,int recieverId)
        {
            try
            {
                if (string.IsNullOrEmpty(msg.Title))
                {
                    throw new TmmException("标题不能为空");
                }
                if (string.IsNullOrEmpty(msg.Content))
                {
                    throw new TmmException("内容不能为空");
                }
                UserService us = Context.GetService<UserService>();
                msg.CreateTime = DateTime.Now;
                msg.Mtype = (int)Model.Enums.MessageType.Message;
                msg.IsRead = false;
                if (recieverId == 0)
                    msg.RecieverId = ConfigHelper.AdminUserId;
                else
                    msg.RecieverId = recieverId;
                us.MessageBll.Insert(msg);
                AddSuccess("发送成功");
            }
            catch (TmmException te) {
                AddError(te.Message);
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error(ex);
                AddError("系统错误，请重试");
            }
            Flash["msg"] = msg;
            Redirect("/my/SentMsg.do");
        }
        /// <summary>
        ///编辑用户个人信息
        /// </summary>
        public void EditUser() {
            PropertyBag["cur_page_edituser"] = true;
            PropertyBag["userInfo"] = base.GetUser();
            PropertyBag["ct"] = ConfigHelper.CommpanyType;
            PropertyBag["mt"] = ConfigHelper.MajorType;
        }
        /// <summary>
        /// 保存个人资料
        /// </summary>
        public void DoEditUser([DataBind("U_UserInfo")]U_UserInfo u,HttpPostedFile file) 
        {
            try
            {
                U_UserInfo logonUser = base.GetUser();
                //保存头像
                if (file != null && !string.IsNullOrEmpty(file.FileName))
                {
                    string headUrl = SaveHeadIcon(file);
                    if (!string.IsNullOrEmpty(headUrl))
                    {
                        u.HeadIcon = headUrl;
                    }
                }
                else {
                    u.HeadIcon = logonUser.HeadIcon;
                }
                
                u.Password = logonUser.Password;
                u.RegIp = logonUser.RegIp;
                u.RegTime = logonUser.RegTime;

                UserService us = Context.GetService<UserService>();
                us.UserInfoBll.Update(u);
                Session["logonUser"] = u;
                AddSuccess("更新个人资料成功");

            }
            catch (TmmException te) {
                AddError(te.Message);
            }
            catch
            {
                AddError("更新个人资料失败！");
            }
            RedirectToReferrer();
        }
        private string SaveHeadIcon(HttpPostedFile file) {
            string ext = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower();
            string savePath = string.Empty;
            string virPath = string.Empty;
            if (ConfigHelper.allowUploadFileType.Contains(ext))
            {
                if (file.ContentLength > 200 * 1024) {
                    throw new TmmException("上传图片尺寸不能超过200KB");
                }
                string newFileName = Guid.NewGuid().ToString("N");
                savePath = Utils.TmmUtils.GetStoreDir(0,ref virPath) + newFileName + "." + ext;
                file.SaveAs(savePath);
                if (!string.IsNullOrEmpty(virPath))
                {
                    virPath = ConfigHelper.uploadUrl + virPath + newFileName + "." + ext;
                }
            }
            else {
                
                throw new TmmException("上传文件格式只能为" + ConfigHelper.allowUploadFileType.ToStringBySpliter("、"));
            }
            return virPath;
        }

        public void Security() {
            PropertyBag["cur_page_security"] = true;
        }

        public void UpdatePwd(string oldPwd, string newPwd, string newPwdAgain)
        {
            if (Context.Request.HttpMethod.ToLower() == "post")
            {
                try
                {
                    if (string.IsNullOrEmpty(oldPwd))
                    {
                        throw new TmmException("旧密码不能为空");
                    }
                    U_UserInfo lu = base.GetUser();
                    if (oldPwd.ToMd5() == lu.Password)
                    {
                        if (string.IsNullOrEmpty(newPwd))
                            throw new TmmException("新密码不能为空");
                        if (string.IsNullOrEmpty(newPwdAgain))
                            throw new TmmException("确认新密码不能为空");
                        if (newPwd.Length < 6) 
                            throw new TmmException("新密码长度不能小于6位");                        
                        if (newPwd != newPwdAgain) 
                            throw new TmmException("两次密码输入不一致");
                        
                        UserService us = Context.GetService<UserService>();
                        U_UserInfo u = us.UserInfoBll.Get(lu.UserId);
                        u.Password = newPwd.ToMd5();                        
                        us.UserInfoBll.Update(u);
                        Session["logonUser"] = u;
                        AddSuccess("密码修改成功");
                    }
                    else
                    {
                        throw new TmmException("旧密码不正确");                        
                    }
                }
                catch (TmmException te) {
                    AddError(te.Message);                    
                }
                RedirectToReferrer();
            }
            else {
                RedirectToAction("Security");
            }
        }
        /// <summary>
        /// 阅读消息
        /// readType 1:发件人阅读 2:收件人阅读
        /// </summary>
        public void ReadMsg(int mid,int readType) {
            PropertyBag["cur_page_msg"] = true;
            try
            {
                bool viewFlag = true;   //如果是收件人阅读为true，发件人 阅读为false
                U_UserInfo logonUser = base.GetUser();
                UserService us = Context.GetService<UserService>();
                M_Message m = us.MessageBll.Get(mid);
                M_Message recentMsg = null;
                if (readType == 1) {
                    viewFlag = false;
                    if (m.SenderId != logonUser.UserId)
                        throw new TmmException("您不能读取此消息");
                }
                else if (readType == 2)
                {
                    if (m.RecieverId != logonUser.UserId)
                        throw new TmmException("您不能读取此消息");                    
                }
                else {
                    throw new TmmException("不正确的访问路径");
                }
                //最近一条记录
                if (m.RefId.HasValue)
                {
                    //recentMsg = us.MessageBll.Get(m.RefId.Value);
                }
                if (!m.IsRead && viewFlag)
                {
                    m.IsRead = true;
                    us.MessageBll.Update(m);
                }
                m.Content = m.Content.ReplaceEnterStr();
                PropertyBag["msg"] = m;
                //PropertyBag["recentMsg"] = recentMsg;
            }
            catch(TmmException te) {
                
                PropertyBag["error"] = te.Message;
            }
        }
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="mid">被回复的消息id</param>
        public void Reply(int mid) 
        {
            PropertyBag["cur_page_msg"] = true;
            UserService us = Context.GetService<UserService>();
            M_Message msg = us.MessageBll.Get(mid);
            U_UserInfo u = base.GetUser();
            if (msg.RecieverId != u.UserId)
                throw new TmmException("错误代码：001，您未被授权");
            PropertyBag["msg"] = msg;
        }
        public void DoReply([DataBind("M_Message")]M_Message remsg) 
        {
            bool f = true;
            try
            {
                if (string.IsNullOrEmpty(remsg.Title))
                {
                    throw new TmmException("标题不能为空");
                }
                if (string.IsNullOrEmpty(remsg.Content))
                {
                    throw new TmmException("内容不能为空");
                }
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                remsg.CreateTime = DateTime.Now;
                remsg.SenderId = u.UserId;
                remsg.Mtype = (int)Model.Enums.MessageType.Message;
                remsg.IsRead = false;
                remsg.SendDeleteFlag = false;
                remsg.RecieveDeleteFlag = false;

                us.MessageBll.Insert(remsg);
                AddSuccess("发送成功");
            }
            catch (TmmException te)
            {
                f = false;
                AddError(te.Message);
            }
            catch (Exception ex)
            {
                f = false;
                Utils.Log4Net.Error(ex);
                AddError("系统错误，请重试");
            }
            Flash["remsg"] = remsg;
            if (!f)
                RedirectToReferrer();
            else
                Redirect("/my/SentMsg.do");
        }

        [SkipFilter]
        public void HomePage(int userId,int first,int? cateId)
        {
            CancelLayout();
            int rows = 10;
            try
            {
                UserService us = Context.GetService<UserService>();
                PropertyBag["homeUser"] = us.UserInfoBll.Get(userId);
                //个人文件夹
                PropertyBag["folders"] = us.MCatalogBll.GetListByUser(userId, 1); 
                //文档总数
                int docCount = 0;                
                //文档列表
                IList<DDocInfo> docList = us.DocInfoBll.GetListByUser(userId,out docCount,first,rows,null,cateId,true);
                ListPage lp = new ListPage((IList)docList,first,rows,docCount);
                PropertyBag["docList"] = lp;
                PropertyBag["allDocCount"] = docCount;
                //总浏览量
                PropertyBag["allViewCount"] = us.DocInfoBll.GetViewCount(userId);
                //收藏文档列表
                int favCount = 0;
                PropertyBag["favList"] = us.MFavoriteBll.GetListByUserId(0, 10, out favCount,userId, null);
                PropertyBag["favCount"] = favCount;
            }
            catch (Exception ex) 
            {
                Utils.Log4Net.Error(ex);
            }
        }
        #region 悬赏
        /// <summary>
        /// 我要悬赏
        /// </summary>
        public void AddReward()
        {
            PropertyBag["cur_page_addreward"] = true;
        }
        public void DoAddReward([DataBind("TReqDoc")]TReqDoc tdoc)
        {
            try
            {
                if (string.IsNullOrEmpty(tdoc.Title))
                    throw new TmmException("标题不能为空");
                if (string.IsNullOrEmpty(tdoc.Description))
                    throw new TmmException("备注不能为空");
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                //余额判断
                decimal ye = us.MAccountBll.GetByUserId(u.UserId).Amount;
                if (ye >= tdoc.Price)
                {
                    tdoc.UserId = u.UserId;
                    tdoc.Title = tdoc.Title.FilterHtml();
                    tdoc.Description = tdoc.Description.FilterHtml();
                    tdoc.Status = 1;
                    tdoc.CreateTime = DateTime.Now;
                    us.TReqDocBll.Insert(tdoc);
                    AddSuccess("发布成功，页面转至您的悬赏文档列表页面");
                    Redirect("MyReward.do");
                    return;
                }
                else
                    throw new TmmException("您的余额不足，请先充值");
            }
            catch (TmmException ex) {
                AddError(ex.Message);
                Flash["model"] = tdoc;
            }
            RedirectToReferrer();
        }

        public void DeleteReward(int reqId) {
            UserService us = Context.GetService<UserService>();
            U_UserInfo u = base.GetUser();
            TReqDoc tdoc = us.TReqDocBll.Get(reqId);
            if (tdoc != null) {
                if (tdoc.UserId == u.UserId) { 
                    //删除
                    us.TReqDocBll.Delete(reqId);
                    AddSuccess("操作成功");
                }
            }
            RedirectToReferrer();
        }
        /// <summary>
        /// 我的悬赏
        /// </summary>
        public void MyReward(int first) {
            int rows = 10;
            PropertyBag["cur_page_addreward"] = true;
            UserService us = Context.GetService<UserService>();
            U_UserInfo u = base.GetUser();
            int count = 0;
            IList<TReqDoc> list = us.TReqDocBll.GetList(u.UserId, out count, first, rows);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        /// <summary>
        /// 我要投稿
        /// </summary>
        public void AddContribute(int reqId) {
            PropertyBag["cur_page_addreward"] = true;
            PropertyBag["SessionId"] = HttpContext.Session.SessionID;
            Service.Bll.Doc.TReqDocBLL tbll = new TMM.Service.Bll.Doc.TReqDocBLL();
            TReqDoc tdoc = tbll.Get(reqId);
            if (tdoc == null)
                throw new Exception("该悬赏文档不存在");
            PropertyBag["tdoc"] = tdoc;
        }
        /// <summary>
        /// 我的投稿
        /// </summary>
        public void MyContribute(int first) {
            PropertyBag["cur_page_addreward"] = true;
            DocService ds = Context.GetService<DocService>();
            U_UserInfo u = base.GetUser();
            int rows = 10;
            int count = 0;
            IList<TJoinDoc> list = ds.TJoinDocBll.GetList(u.UserId, out count, first, rows);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        /// <summary>
        /// 选稿
        /// </summary>
        public void SelectContribute(int first,int reqId) 
        {
            PropertyBag["cur_page_addreward"] = true;
            int rows = 10;
            int count = 0;
            DocService ds = Context.GetService<DocService>();
            U_UserInfo u = base.GetUser();
            TReqDoc trDoc = ds.TReqDocBll.Get(reqId);
            if (trDoc.UserId != u.UserId)
                throw new TmmException("操作错误，您不是该悬赏的所有者");
            IList<TJoinDoc> list = ds.TJoinDocBll.GetListForXg(reqId, out count, first, rows);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        /// <summary>
        /// 设置中标
        /// </summary>
        /// <param name="joinId"></param>
        public void SetZb(int joinId) 
        {
            try
            {
                DocService ds = Context.GetService<DocService>();
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                //投稿对象
                TJoinDoc tj = ds.TJoinDocBll.Get(joinId);
                //是否已经中标了
                if (tj.IsWin)
                    throw new TmmException("此投稿已经设置为中标");
                //悬赏对象
                TReqDoc trDoc = ds.TReqDocBll.Get(tj.TId);
                //余额检测
                decimal ye = us.MAccountBll.GetByUserId(u.UserId).Amount;
                if (ye < trDoc.Price)
                    throw new TmmException("您的余额不足，请先充值");
                //扣除需求方余额
                us.MAccountBll.AccountExpend(u.UserId, trDoc.Price, Utils.TmmUtils.IPAddress(),trDoc.Title);
                //为投稿人增加余额
                us.MAccountBll.AddAmount(tj.UserId, trDoc.Price, Utils.TmmUtils.IPAddress(),trDoc.Title);
                //更改原文档的owner
                DDocInfo doc = ds.DDocInfoBll.Get(tj.DocId);
                doc.UserId = u.UserId;
                doc.IsTaskDoc = false;
                ds.DDocInfoBll.Update(doc);
                PropertyBag["doc"] = doc;
                //更改投稿文档的状态
                tj.IsWin = true;
                tj.WinTime = DateTime.Now;
                ds.TJoinDocBll.Update(tj);
                //发送通知
                M_Message msg = new M_Message() { 
                    Mtype = (int)Model.Enums.MessageType.Inform,
                    SenderId = ConfigHelper.AdminUserId,
                    RecieverId = tj.UserId,
                    Title = "您的投稿被选中",
                    IsRead = false,
                    CreateTime = DateTime.Now,
                    Content = string.Format("您的投稿【{0}】被{1}设置中标，获得收入￥{2}",
                        tj.Title,"<a href='/home/" + u.UserId + ".html' target='_blank'>"+u.TmmDispName+"</a>"
                        , string.Format("{0:N2}", trDoc.Price))
                };
                us.MessageBll.Insert(msg);
            }
            catch (TmmException te)
            {
                AddError(te.Message);
                RedirectToReferrer();
            }
        }
        /// <summary>
        /// 删除投稿
        /// </summary>
        /// <param name="joinId"></param>
        public void DeleteContribute(int joinId)
        {
            try
            {
                DocService ds = Context.GetService<DocService>();
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                //投稿对象
                TJoinDoc tj = ds.TJoinDocBll.Get(joinId);
                if (tj.UserId != u.UserId)
                    throw new TmmException("操作失败，您不是该投稿的所有者");
                if (tj.IsWin)
                    throw new TmmException("操作失败，该投稿已中标");
                //删除文档
                ds.DDocInfoBll.Delete(tj.DocId);
                //删除投稿记录
                ds.TJoinDocBll.Delete(joinId);
                //更新悬赏的投稿数
                TReqDoc trDoc = ds.TReqDocBll.Get(tj.TId);
                trDoc.DocCount -= 1;
                ds.TReqDocBll.Update(trDoc);
            }
            catch (TmmException te)
            {
                AddError(te.Message);
                
            }
            RedirectToReferrer();
        }
        /// <summary>
        /// 发布文档 from 投稿
        /// </summary>
        /// <param name="joinId"></param>
        public void PublishContribute(int joinId)
        {
            try
            {
                DocService ds = Context.GetService<DocService>();
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                //投稿对象
                TJoinDoc tj = ds.TJoinDocBll.Get(joinId);
                if (tj.UserId != u.UserId)
                    throw new TmmException("操作失败，您不是该投稿的所有者");
                if (tj.IsWin)
                    throw new TmmException("操作失败，该投稿已中标");
                //发布文档
                DDocInfo doc = ds.DDocInfoBll.Get(tj.DocId);
                doc.IsAudit = false;
                doc.IsTaskDoc = false;
                ds.DDocInfoBll.Update(doc);
                
                
                //删除投稿记录
                ds.TJoinDocBll.Delete(joinId);
                //更新悬赏的投稿数
                TReqDoc trDoc = ds.TReqDocBll.Get(tj.TId);
                trDoc.DocCount -= 1;
                ds.TReqDocBll.Update(trDoc);

                Redirect("EditDoc.do?docId=" + doc.DocId.ToString());
                return;
            }
            catch (TmmException te)
            {
                AddError(te.Message);

            }
            RedirectToReferrer();
        }
        #endregion

        #region 账户
        /// <summary>
        /// 账单明细
        /// </summary>
        public void AccountDetail(int first, string accWays, DateTime? sd, DateTime? ed)
        {
            int rows = 10;
            int count = 0;
            PropertyBag["cur_page_account"] = true;
            U_UserInfo logonUser = base.GetUser();
            Service.Bll.Order.AccountLogBLL abll = new TMM.Service.Bll.Order.AccountLogBLL();
            PropertyBag["sumIncome"] = abll.SumIncome(logonUser.UserId);
            //可用余额
            Service.Bll.User.MAccountBLL accBll = new TMM.Service.Bll.User.MAccountBLL();
            PropertyBag["acc"] = accBll.GetByUserId(logonUser.UserId);

            int[] accountWays = null;
            if (accWays == "receipt") {
                accountWays = new int[] { (int)AmountWay.In,(int)AmountWay.INCOME,(int)AmountWay.MIn,(int)AmountWay.INCOME_TG };
            }
            if (accWays == "payout") {
                accountWays = new int[] { (int)AmountWay.Out,(int)AmountWay.ROut};
            }
            IList<AccountLog> list = abll.GetList(logonUser.UserId, accountWays, sd, ed, first, rows, out count);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
            PropertyBag["sd"] = sd;
            PropertyBag["ed"] = ed;
        }
        /// <summary>
        /// 充值
        /// </summary>
        public void DoCharge(string returnUrl,decimal total,int docId) {
            PropertyBag["cur_page_account"] = true;

            PropertyBag["returnUrl"] = returnUrl;
            PropertyBag["docId"] = docId;

            OrderService os = Context.GetService<OrderService>();
            DDocInfo doc = os.DDocInfoBll.Get(docId);
            U_UserInfo logonUser = base.GetUser();

            
            //os.TOrderBll.SaveOrder(order);
            //PropertyBag["order"] = order;
            PropertyBag["doc"] = doc;
            PropertyBag["acc"] = os.MAccountBll.GetByUserId(logonUser.UserId);
            PropertyBag["chargeRange"] = ChargeRange(total);
            PropertyBag["total"] = total;
            
        }
        /// <summary>
        /// 兑换
        /// </summary>
        public void Exchange() {
            PropertyBag["cur_page_account"] = true;
            OrderService os = Context.GetService<OrderService>();
            U_UserInfo u = base.GetUser();
            PropertyBag["amount"] = os.MAccountBll.GetByUserId(u.UserId).Amount;
            PropertyBag["minExchange"] = ConfigHelper.MinExchange;
            PropertyBag["ratio"] = ConfigHelper.ExchangeRatio;
            PropertyBag["exAccList"] = os.ExchangeAccountBll.GetList(u.UserId);
        }
        [AccessibleThrough(Verb.Post)]
        public void DoExchange(
            int accountType,string accountName,string accountNo,int amount,int exchangeAccId,
            string bankName,string provinceName,string areaName,int? provinceId,int? areaId
        )
        {
            try
            {
                #region 参数检测
                if (string.IsNullOrEmpty(accountName))
                    throw new TmmException("账户名不能为空");
                if (string.IsNullOrEmpty(accountNo))
                    throw new TmmException("账号不能为空");
                if (amount == 0)
                    throw new TmmException("兑换金额不能为0");
                int toExchange = (int)Helper.FormatHelper.ExchangeValue(amount);
                if (toExchange == 0 || toExchange % ConfigHelper.MinExchange != 0)
                    throw new TmmException("兑换金额只能为" + ConfigHelper.MinExchange.ToString() + "的倍数");

                #endregion
                OrderService os = Context.GetService<OrderService>();
                U_UserInfo u = base.GetUser();
                if (exchangeAccId == 0)
                {
                    //新增兑换账户ID
                    ExchangeAccount ea = new ExchangeAccount()
                    {
                        AccountName = accountName,
                        AccountNo = accountNo,
                        AccountType = accountType,
                        AreaId = areaId,
                        AreaName = areaName,
                        BankName = bankName,
                        ProvinceId = provinceId,
                        ProvinceName = provinceName,
                        CreateTime = DateTime.Now,
                        UserId = u.UserId
                    };
                    exchangeAccId = os.ExchangeAccountBll.Insert(ea);
                }
                else
                {
                   //更新账户信息
                    ExchangeAccount oldEa = os.ExchangeAccountBll.Get(exchangeAccId);
                    oldEa.AccountName = accountName;
                    oldEa.AccountNo = accountNo;
                    oldEa.AreaId = areaId;
                    oldEa.AreaName = areaName;
                    oldEa.ProvinceId = provinceId;
                    oldEa.ProvinceName = provinceName;
                    oldEa.BankName = bankName;
                    os.ExchangeAccountBll.Update(oldEa);
                    //如果因为用户自己更改了兑换账户信息而可能产生的纠纷，管理员在后台处理的时候需要把相关账户信息写进管理员备注
                }
                
                //冻结账户部分余额
                os.MAccountBll.FrozenSomeAmount(amount, u.UserId);
                //生成订单
                decimal orderId = Utils.TmmUtils.GenOrderId();
                TOrderDetail detail = new TOrderDetail()
                {
                    DocId = -2, //-2代表兑换，-1代表直接充值
                    DocTitle = "兑换：" + amount.ToString(),
                    GoodsCount = 1,
                    Price = amount,
                    OrderId = orderId
                };
                TOrder o = new TOrder()
                {
                    CreateTime = DateTime.Now,
                    Email = u.Email,
                    ExchangeAccId = exchangeAccId,
                    Ip = Utils.TmmUtils.IPAddress(),
                    OrderDetails = new List<TOrderDetail>() { detail },
                    OrderId = orderId,
                    OrderType = (int)OrderType.Exchange,
                    PayWay = 0,
                    Remark = accountType.ToString(),    //此处备注字段用户保存是支付宝兑换，还是银行账户兑换
                    Status = (int)OrderStatus.FrozenSomeAmount,
                    Total = amount,
                    UserId = u.UserId,
                    //把生成订单时填写的账户信息写入管理员备注
                    AdminRemark = string.Format("户名：【{0}】，账号：【{2}{1}】",accountName,accountNo,bankName + ",")
                };
                os.TOrderBll.SaveOrder(o);
                Flash["exchangeResult"] = true;
                
            }
            catch (TmmException te) {
                AddError(te.Message);
            }
            Redirect("Exchange.do");
        }
        public void ExchangeList(int first, DateTime? startdate, DateTime? enddate)
        {
            int rows = 10;
            int count = 0;
            PropertyBag["cur_page_account"] = true;
            OrderService os = Context.GetService<OrderService>();
            U_UserInfo u = base.GetUser();
            IList<TOrder> list = os.TOrderBll.GetList(u.UserId, (int)OrderType.Exchange, first, rows, out count,startdate,enddate);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;
        }
        /// <summary>
        /// 我的购买
        /// </summary>
        public void Purchase(int first) {
            string a = Context.UrlReferrer;
            if (!string.IsNullOrEmpty(a)) {
                if (a.ToLower().IndexOf("/my/docharge.do") > -1)
                    PropertyBag["fromPay"] = true;
            }
            int rows = 10;
            PropertyBag["cur_page_purchase"] = true;
            UserService us = Context.GetService<UserService>();
            U_UserInfo u = base.GetUser();
            int count = 0;
            IList<MPurchase> list = us.MPurchaseBll.GetList(u.UserId, first, rows, out count);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["listPage"] = lp;
        }

        private int ChargeRange(decimal total) {
            int r = 10;
            if (total > 10 && total <= 100)
                r = 100;
            else if (total > 100 && total <= 500)
                r = 500;
            else if (total > 500 && total <= 2000)
                r = 2000;
            else if (total > 2000)
                r = 10000;
            return r;
        }
        #endregion

    }
}
