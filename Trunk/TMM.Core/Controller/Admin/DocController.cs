using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Web;
using System.IO;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Extends;
using TMM.Core.Common;
using TMM.Core.Helper;

namespace TMM.Core.Controller.Admin
{
    [Layout("adminContent")]
    [Helper(typeof(Helper.FormatHelper))]
    [Helper(typeof(Helper.UserHelper))]
    public class DocController : BaseController
    {
        #region 分类
        public void Catalog(int catalogId) 
        {
            SystemService ss = Context.GetService<SystemService>();
            Hashtable p = new Hashtable();
            p.Add("Pid",catalogId);
            IList<S_Catalog> list = ss.SCatalogBll.GetList(p, null, null, null);
            IList<S_Catalog> tree = ss.SCatalogBll.GetTopNode();
            PropertyBag["list"] = list;
            PropertyBag["tree"] = tree;
        }

        
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="catalogId"></param>
        public void DeleteCatalog(int catalogId) {
            SystemService ss = Context.GetService<SystemService>();
            Hashtable p = new Hashtable();
            p.Add("Pid",catalogId);
            IList<S_Catalog> sublist = ss.SCatalogBll.GetList(p,null,null,null);
            if (sublist.Count > 0)
            {
                //确认的view
                PropertyBag["model"] = ss.SCatalogBll.Get(catalogId);
            }
            else {
                ss.SCatalogBll.Delete(catalogId);
                AddSuccess("操作成功");
                RedirectToReferrer();
            }
        }
        /// <summary>
        /// 删除分类及子分类
        /// </summary>
        /// <param name="catalogId"></param>
        public void DeleteCatalog2(int catalogId) {
            try
            {
                SystemService ss = Context.GetService<SystemService>();
                Hashtable p = new Hashtable();
                p.Add("Pid", catalogId);
                IList<S_Catalog> sublist = ss.SCatalogBll.GetSubList(catalogId);
                //先删除两级子分类
                sublist.ToList().ForEach(c =>
                {
                    c.SubCatalog.ToList().ForEach(c1 => ss.SCatalogBll.Delete(c1.CatalogId));
                    ss.SCatalogBll.Delete(c.CatalogId);
                });
                ss.SCatalogBll.Delete(catalogId);
                AddSuccess("操作成功");
            }
            catch {
                AddError("系统错误");
            }
            Redirect("Catalog.do");
            return;
        }

        public void EditCatalog(int catalogId) {
            SystemService ss = Context.GetService<SystemService>();
            PropertyBag["model"] = ss.SCatalogBll.Get(catalogId);
        }

        public void DoCatalog([DataBind("S_Catalog")]S_Catalog cata)
        {
            try
            {
                if (string.IsNullOrEmpty(cata.CatalogName))
                    throw new TmmException("分类名称不能为空");
                SystemService ss = Context.GetService<SystemService>();
                if (cata.CatalogId == 0)
                {
                    ss.SCatalogBll.Insert(cata);

                }
                else
                {
                    ss.SCatalogBll.Update(cata);
                    AddSuccess("操作成功");
                    Redirect("Catalog.do");
                    return;
                }
                AddSuccess("操作成功");
            }
            catch (TmmException te) {
                Flash["model"] = cata;
                AddError(te.Message);
            }
            catch
            {
                AddError("系统错误");
                Flash["model"] = cata;
            }
            RedirectToReferrer();
        }
        #endregion

        #region 悬赏文档
        public void Reward(int first,string docTitle,int? status)
        {
            DocService ds = Context.GetService<DocService>();
            Hashtable p = new Hashtable();
            if (!string.IsNullOrEmpty(docTitle))
                p.Add("Title", docTitle);
            if(status.HasValue)
                p.Add("Status",status.Value);
            int rows = 20;
            int count = ds.TReqDocBll.GetCount(p);
            IList<TReqDoc> list = ds.TReqDocBll.GetList(p, null, first, rows);
            PropertyBag["lp"] = new ListPage((IList)list, first, rows, count);
        }
        public void DeleteReward(int tid)
        {
            DocService ds = Context.GetService<DocService>();
            TReqDoc tr = ds.TReqDocBll.Get(tid);
            if (tr.Status == 2)
                throw new Exception("悬赏已经发布，不能删除");
            ds.TReqDocBll.Delete(tr.TId);
            AddSuccess("操作成功");
            RedirectToReferrer();
        }
        public void AuditReward(int tid) {
            DocService ds = Context.GetService<DocService>();
            TReqDoc tr = ds.TReqDocBll.Get(tid);
            tr.Status = 2;
            ds.TReqDocBll.Update(tr);
            AddSuccess("操作成功");
            RedirectToReferrer();
        }
        public void Contribute(int first,int? tid,string likeTitle) {
            int rows = 20;
            DocService ds = Context.GetService<DocService>();
            
            Hashtable p = new Hashtable();
            if (tid.HasValue)
            {
                p.Add("TId", tid.Value);
                TReqDoc tr = ds.TReqDocBll.Get(tid.Value);
                PropertyBag["trdoc"] = tr;
            }
            if (!string.IsNullOrEmpty(likeTitle))
                p.Add("LikeTitle",likeTitle);
            int count = ds.TJoinDocBll.GetCount(p);
            IList<TJoinDoc> list = ds.TJoinDocBll.GetList(p, null, first, rows);
            PropertyBag["lp"] = new ListPage((IList)list, first, rows, count);
        }
        /// <summary>
        /// 删除投稿
        /// </summary>
        /// <param name="joinIds"></param>
        public void DeleteContribute(int[] joinIds)
        {
            DocService ds = Context.GetService<DocService>();
            Queue<M_Message> queueMsg = new Queue<M_Message>();
            foreach (int joinId in joinIds)
            {
                TJoinDoc joinDoc = ds.TJoinDocBll.Get(joinId);
                if (joinDoc != null) { 
                    //更新悬赏文档的投稿数量
                    TReqDoc reqDoc = ds.TReqDocBll.Get(joinDoc.TId);
                    if (reqDoc != null) {
                        if (reqDoc.DocCount > 0) {
                            reqDoc.DocCount -= 1;
                            ds.TReqDocBll.Update(reqDoc);
                        }
                    }
                    //删除原始文档
                    ds.DDocInfoBll.Delete(joinDoc.DocId);
                    //删除投稿记录
                    ds.TJoinDocBll.Delete(joinId);
                    //给投稿人发消息
                    M_Message msg = new M_Message()
                    {
                        Content = string.Format("您上传的投稿文档【{0}】因违反相关规定，该文章已被管理员删除", joinDoc.Title),
                        CreateTime = DateTime.Now,
                        IsRead = false,
                        Mtype = (int)Model.Enums.MessageType.Inform,
                        RecieveDeleteFlag = false,
                        RecieverId = joinDoc.UserId,
                        SendDeleteFlag = false,
                        SenderId = Helper.ConfigHelper.AdminUserId,
                        Title = "投稿文档删除通知"
                    };
                    queueMsg.Enqueue(msg);
                    //给悬赏人发消息
                    msg = new M_Message()
                    {
                        Content = string.Format("您收到的投稿文档【{0}】因违反相关规定，该文章已被管理员删除", joinDoc.Title),
                        CreateTime = DateTime.Now,
                        IsRead = false,
                        Mtype = (int)Model.Enums.MessageType.Inform,
                        RecieveDeleteFlag = false,
                        RecieverId = reqDoc.UserId,
                        SendDeleteFlag = false,
                        SenderId = Helper.ConfigHelper.AdminUserId,
                        Title = "投稿文档删除通知"
                    };
                    queueMsg.Enqueue(msg);
                }
            }
            //异步发送消息
            AsynMessage am = new AsynMessage(queueMsg);
            am.Send();

            AddSuccess("操作成功");
            RedirectToReferrer();
        }
        #endregion

        #region 文档列表
        public void Index(
            int first, int? userId, bool? isAudit,
            string likeTitle, bool? isRecommend, string orderBy, bool? isManualConvert, int? cateId,string cateName
            ,bool? isMajia) 
        {
            int rows = 20;
            int count = 0;
                        
            DocService ds = Context.GetService<DocService>();
            UserService userService = Context.GetService<UserService>();

            Hashtable p = new Hashtable();
            //p.Add("IsAudit", isAudit);
            if (isAudit.HasValue)
                p.Add("IsAudit", isAudit.Value);
            if (!string.IsNullOrEmpty(likeTitle))
                p.Add("LikeTitle",likeTitle);
            if (isRecommend.HasValue) {
                if (isRecommend.Value) {
                    p.Add("IsRecommend",isRecommend.Value);
                }
            }
            if (userId.HasValue)
                p.Add("UserId",userId.Value);
            if (isManualConvert.HasValue) {
                if (isManualConvert.Value) {
                    p.Add("DocTypes",Helper.ConfigHelper.NotConvertDocTypes);
                }
            }
            if (cateId.HasValue)
            {
                int[] cateids = Utils.TmmUtils.GetCatalogIds(cateId.Value);
                if (cateids != null && cateids.Length > 0)
                    p.Add("CateIds", cateids);                
            }
            if (isMajia.HasValue) {
                p.Add("IsMajia",isMajia.Value);
            }

            //
            p.Add("IsTaskDoc",false);

            count = ds.DDocInfoBll.GetCount(p);

            if (!string.IsNullOrEmpty(orderBy))
                orderBy += " DESC";
            
            IList<DDocInfo> docs = ds.DDocInfoBll.GetList(p, orderBy, first, rows);
            ListPage lp = new ListPage((IList)docs, first, rows, count);
            PropertyBag["lp"] = lp;
            #region 保存参数
            PropertyBag["IsAudit"] = isAudit;
            PropertyBag["isManualConvert"] = isManualConvert;
            #endregion
        }
        public void DeleteDoc(int[] docIds,string redirectUrl) {
            DocService ds = Context.GetService<DocService>();
            UserService us = Context.GetService<UserService>();
            foreach (int docId in docIds)
            {
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                U_UserInfo u = us.UserInfoBll.Get(doc.UserId);
                if (doc.IsAudit)
                {
                    u.UploadCount -= 1;
                    us.UserInfoBll.Update(u);
                }

                ds.DDocInfoBll.Delete(docId);
            }
            Utils.Log4Net.Error("Refer : " + Context.UrlReferrer);
            base.SuccessInfo();
            RedirectToReferrer2(redirectUrl);
        }
        public void AuditDoc(int[] docIds) {
            DocService ds = Context.GetService<DocService>();
            foreach (int docId in docIds)
            {
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                doc.IsAudit = true;
                ds.DDocInfoBll.Update(doc);
                //增加用户表的UploadCount
                UserService us = Context.GetService<UserService>();
                int uc = ds.DDocInfoBll.GetCountByUser(doc.UserId);
                us.UserInfoBll.UpdateUploadCount(uc, doc.UserId);
            }
            base.SuccessInfo();
            Redirect("/admin/doc/index.do");
        }
        public void CancelAudit(int[] docIds)
        {
            DocService ds = Context.GetService<DocService>();
            foreach (int docId in docIds)
            {
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                doc.IsAudit = false;
                ds.DDocInfoBll.Update(doc);
                //更新用户表的UploadCount
                UserService us = Context.GetService<UserService>();
                int uc = ds.DDocInfoBll.GetCountByUser(doc.UserId);
                us.UserInfoBll.UpdateUploadCount(uc, doc.UserId);
            }
            base.SuccessInfo();
            Redirect("/admin/doc/index.do");
        }
        public void CancelRecommendDoc(int docId) {
            DocService ds = Context.GetService<DocService>();

            DDocInfo doc = ds.DDocInfoBll.Get(docId);
            doc.IsRecommend = false;
            ds.DDocInfoBll.Update(doc);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void RecommendDoc(int docId) {
            DocService ds = Context.GetService<DocService>();

            DDocInfo doc = ds.DDocInfoBll.Get(docId);
            doc.IsRecommend = true;
            ds.DDocInfoBll.Update(doc);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void SetConvert(int docId)
        {
            DocService ds = Context.GetService<DocService>();
            ds.DDocInfoBll.UpdateConvertFlag(docId);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void EditDoc(int docId) {
            DocService ds = Context.GetService<DocService>();
            DDocInfo doc = ds.DDocInfoBll.Get(docId);
            PropertyBag["doc"] = doc;
            SystemService ss = Context.GetService<SystemService>();
            IList<S_Catalog> cates = ss.SCatalogBll.GetTopNode();
            PropertyBag["cates"] = cates;
        }
        [AccessibleThrough(Verb.Post)]
        public void DoEditDoc([DataBind("DDocInfo")]DDocInfo doc,HttpPostedFile file)        
        {
            try
            {
                //DocService us = Context.GetService<DocService>();
                UserService us = Context.GetService<UserService>();
                DDocInfo oldDoc = us.DocInfoBll.Get(doc.DocId);
                if (oldDoc != null)
                {
                    //更新缩略图
                    if (file != null && !string.IsNullOrEmpty(file.FileName))
                    {
                        string newThumbUrl = SaveNewDocThumbnail(file, oldDoc.ThumbnailUrl);
                        if (!string.IsNullOrEmpty(newThumbUrl))
                            oldDoc.ThumbnailUrl = newThumbUrl;
                    }
                    else
                        oldDoc.ThumbnailUrl = doc.ThumbnailUrl;

                    oldDoc.Title = doc.Title;
                    oldDoc.Description = doc.Description;
                    oldDoc.Tags = doc.Tags;
                    oldDoc.CateId = doc.CateId;
                    oldDoc.Price = doc.Price;
                    oldDoc.FlashUrl = doc.FlashUrl;
                    us.DocInfoBll.Update(oldDoc);
                    //更新标签表
                    if (!string.IsNullOrEmpty(oldDoc.Tags) && oldDoc.Tags != doc.Tags)
                    {
                        string[] tags = doc.Tags.Split(' ');
                        us.DTagBll.UpdateFromDoc(tags, oldDoc.DocId);
                    }

                    //异步通知用户文档被管理员更新
                    M_Message msg = new M_Message()
                    {
                        Content = string.Format("您上传的文档“{0}”被管理员更新", doc.Title),
                        CreateTime = DateTime.Now,
                        IsRead = false,
                        Mtype = (int)Model.Enums.MessageType.Inform,
                        RecieverId = oldDoc.UserId,
                        SenderId = Helper.ConfigHelper.AdminUserId,
                        Title = string.Format("您上传的文档“{0}”被管理员更新", doc.Title)
                    };
                    Common.AsynMessage am = new AsynMessage(msg);
                    am.Send();

                    Redirect("/admin/doc/index.do");
                    return;
                }
                throw new Exception("查询文档为空");
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error(ex);                
                RedirectToReferrer();
            }
        }
        public void Comment(int docId,int first)
        {
            int rows = 20;
            int count = 0;
            DocService ds = Context.GetService<DocService>();
            count = ds.DCommentBll.GetCount(docId);
            IList<D_Comment> list = ds.DCommentBll.GetListForAdmin(docId);
            ListPage lp = new ListPage((IList)list, first, rows, count);
            PropertyBag["lp"] = lp;

            //HomeController hc = new HomeController();
            //Hashtable p = new Hashtable();
            //p.Add("DocId", docId);
            //string a = hc.TestSql("D_Comment.GetCount", p);
            //Utils.Log4Net.Error(count.ToString());
        }
        public void DeleteComment(int[] commentIds)
        {
            DocService ds = Context.GetService<DocService>();
            foreach (int cid in commentIds)
            {
                ds.DCommentBll.Delete(cid);
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void SetCommentDisp(int[] commentIds,bool isDisp)
        {
            DocService ds = Context.GetService<DocService>();
            foreach (int cid in commentIds)
            {
                D_Comment dc = ds.DCommentBll.Get(cid);
                if (dc != null)
                {
                    dc.IsDisp = isDisp;
                    ds.DCommentBll.Update(dc);
                }
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void Download(int docId)
        {
            CancelLayout();
            CancelView();
            try
            {
                DocService ds = Context.GetService<DocService>();                
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                FileStream myFile = null;
                BinaryReader br = null;
                try
                {
                    
                    myFile = new FileStream(doc.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    br = new BinaryReader(myFile);


                    Context.Response.AppendHeader("Accept-Ranges", "bytes");
                    HttpContext.Current.Response.Buffer = false;
                    //Response.Buffer = false;
                    Int64 fileLength = myFile.Length;
                    Int64 startBytes = 0;

                    Double pack = 10240; //10K bytes
                    int dlSpeed = 512000;    //下载速度
                    Int32 sleep = (Int32)Math.Floor(1000 * pack / dlSpeed) + 1;                    

                    HttpContext.Current.Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());

                    HttpContext.Current.Response.AddHeader("Connection", "Keep-Alive");
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AddHeader(
                        "Content-Disposition",
                        "attachment;filename=" + HttpUtility.UrlEncode(doc.FileName, System.Text.Encoding.UTF8)
                        + "." + doc.DocType
                        );

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    Int32 maxCount = (Int32)Math.Floor((fileLength - startBytes) / pack) + 1;

                    for (Int32 i = 0; i < maxCount; i++)
                    {
                        if (Response.IsClientConnected)
                        {
                            Response.BinaryWrite(br.ReadBytes(Convert.ToInt32(pack)));
                            System.Threading.Thread.Sleep(sleep);
                        }
                        else
                            break;
                    }                    
                    
                }
                catch (Exception ex)
                {
                    Utils.Log4Net.Error("admin file DownloadFile " + ex.ToString());
                    //DoAlert("异常错误 03");
                }
                finally
                {
                    if (br != null) br.Close();
                    if (myFile != null) myFile.Close();
                }
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error(ex);
            }
        }
        public void UploadReplaceDoc(int originalId)
        { 
        }
        public void DoUploadReplaceDoc(int originalId, HttpPostedFile fileData)
        {
            DocService ds = Context.GetService<DocService>();
            DDocInfo originalDoc = ds.DDocInfoBll.Get(originalId);
            if (originalDoc != null)
            {
                //上传新文档
                string ext = string.Empty;
                string savePath = Save(fileData, ref ext);                
                DDocInfo newDoc = new DDocInfo()
                {
                    UserId = ConfigHelper.AdminUserId,
                    DocType = ext.ToLower(),
                    FileId = Guid.NewGuid(),
                    FileName = fileData.FileName.Replace("." + ext, ""),
                    FileLength = fileData.ContentLength,
                    CreateTime = DateTime.Now,
                    PhysicalPath = savePath
                };                
                int newDocId = ds.DDocInfoBll.Insert(newDoc);
                //更新原来的文档
                originalDoc.ReplaceDocId = newDocId;
                ds.DDocInfoBll.Update(originalDoc);
            }
            base.SuccessInfo();
            RedirectToReferrer();
        }
        public void ReplaceDoc(int docId)
        {
            DocService ds = Context.GetService<DocService>();
            DDocInfo originalDoc = ds.DDocInfoBll.Get(docId);
            DDocInfo replaceDoc = ds.DDocInfoBll.Get(originalDoc.ReplaceDocId.Value);
            originalDoc.ThumbnailUrl = replaceDoc.ThumbnailUrl;
            originalDoc.FlashUrl = replaceDoc.FlashUrl;
            originalDoc.PageCount = replaceDoc.PageCount;
            ds.DDocInfoBll.Update(originalDoc);
            //替换完成后删除文件
            ds.DDocInfoBll.Delete(replaceDoc.DocId);
            //删除物理文件
            File.Delete(replaceDoc.PhysicalPath);
            base.SuccessInfo();
            RedirectToReferrer();
        }
        #endregion

        /// <summary>
        /// 修改封面图片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="oldThumbnailUrl"></param>
        /// <returns></returns>
        private string SaveNewDocThumbnail(HttpPostedFile file,string oldThumbnailUrl)
        {
            string ext = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower();
            string savePath = string.Empty;
            string virPath = string.Empty;
            if (Helper.ConfigHelper.allowUploadFileType.Contains(ext))
            {
                if (file.ContentLength > 200 * 1024)
                {
                    throw new TmmException("上传图片尺寸不能超过200KB");
                }
                if (string.IsNullOrEmpty(oldThumbnailUrl)) 
                {
                    string newFileName = Guid.NewGuid().ToString("N");
                    savePath = Utils.TmmUtils.GetStoreDir(0, ref virPath) + newFileName + "." + ext;
                    oldThumbnailUrl = savePath;
                }                
                file.SaveAs(oldThumbnailUrl);
                //if (!string.IsNullOrEmpty(virPath))
                //{
                //    virPath = Helper.ConfigHelper.uploadUrl + virPath + newFileName + "." + ext;
                //}
            }
            else
            {

                throw new TmmException("上传文件格式只能为" + Helper.ConfigHelper.allowUploadFileType.ToStringBySpliter("、"));
            }
            return oldThumbnailUrl;
        }
        /// <summary>
        /// 保存文件，并返回文件的物理路径
        /// </summary>
        /// <param name="filedata"></param>
        /// <returns></returns>
        private string Save(HttpPostedFile filedata, ref string ext)
        {
            ext = filedata.FileName.Substring(filedata.FileName.LastIndexOf(".") + 1).ToLower();
            if (!ConfigHelper.docFileType.Contains(ext.ToLower()))
                throw new TmmException("文件格式不符合要求");

            if (filedata.ContentLength > ConfigHelper.DocSizeLimit * 1024 * 1024)
                throw new TmmException("文件大小超过限制");

            string newFileName = Guid.NewGuid().ToString("N");
            //string virPath = string.Empty;
            string savePath = Utils.TmmUtils.GetStoreDir() + newFileName + "." + ext;
            filedata.SaveAs(savePath);
            return savePath;
        }

    }
}
