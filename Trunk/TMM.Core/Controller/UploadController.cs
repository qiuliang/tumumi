using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;
using System.Text;
using TMM.Core.Helper;
using TMM.Core.Common;
using TMM.Service;
using TMM.Model;

namespace TMM.Core.Controller
{
    [Filter(ExecuteEnum.BeforeAction, typeof(Filter.MyTmmFilter))]
    [Helper(typeof(Helper.FormatHelper))]
    public class UploadController : BaseController
    {
        public void Default() {
            PropertyBag["SessionId"] = HttpContext.Session.SessionID;
            StringBuilder s = new StringBuilder();
            int i = 0;
            foreach(string t in ConfigHelper.docFileType)
            {
                s.Append(string.Format("*.{0}{1}",t,i == ConfigHelper.docFileType.Length - 1 ? "" : ";" ));
                i++;
            }
            PropertyBag["allowFileType"] = s.ToString();
        }

        public void SaveFile(HttpPostedFile fileData) 
        {            
            CancelLayout();
            CancelView();
            try
            {
                //保存文件
                string ext = string.Empty;
                string savePath = Save(fileData, ref ext);
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                DDocInfo doc = new DDocInfo()
                {
                    UserId = u.UserId,
                    DocType = ext.ToLower(),
                    FileId = Guid.NewGuid(),
                    FileName = fileData.FileName.Replace("." + ext,""),
                    FileLength = fileData.ContentLength,
                    CreateTime = DateTime.Now,
                    PhysicalPath = savePath
                };
                if (ConfigHelper.NotConvertDocTypes.Contains(ext.ToLower())) {
                    doc.Wwk = 1;
                    doc.Wwk2 = 1;
                    doc.Txt = 1;
                }
                us.DocInfoBll.Insert(doc);
                
                Response.StatusCode = 200;
                Response.Write("{code:1,msg:'" + doc.FileId.Value.ToString() + "'}");
            }
            catch (TmmException te) {
                Response.StatusCode = 200;
                Response.Write("{code:0,msg:'" + te.Message + "'}");
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error("上传文档出错：" + ex.Message);
                Response.StatusCode = 500;
                Response.Write("{code:-1,msg:'" + ex.Message + "'}");
            }
        }
        /// <summary>
        /// 保存文件，并返回文件的物理路径
        /// </summary>
        /// <param name="filedata"></param>
        /// <returns></returns>
        private string Save(HttpPostedFile filedata,ref string ext) 
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

        
        public void UploadSuccess(Guid fileId)
        {
            UserService us = Context.GetService<UserService>();
            DDocInfo doc = us.DocInfoBll.GetByFileId(fileId);
            PropertyBag["doc"] = doc;
            SystemService ss = Context.GetService<SystemService>();
            IList<S_Catalog> cates = ss.SCatalogBll.GetTopNode();
            PropertyBag["cates"] = cates;
        }

        public void DoUpdateDoc([DataBind("DDocInfo")]DDocInfo doc)
        {
            try
            {
                UserService us = Context.GetService<UserService>();
                DDocInfo oldDoc = us.DocInfoBll.GetByFileId(doc.FileId.Value);
                if (oldDoc != null)
                {
                    oldDoc.Title = doc.Title;
                    oldDoc.Description = doc.Description;
                    oldDoc.Tags = doc.Tags;
                    oldDoc.CateId = doc.CateId;
                    oldDoc.Price = doc.Price;
                    us.DocInfoBll.Update(oldDoc);
                    //更新标签表
                    if (!string.IsNullOrEmpty(doc.Tags))
                    {
                        string[] tags = doc.Tags.Split(' ');
                        us.DTagBll.UpdateFromDoc(tags, oldDoc.DocId);
                    }

                    Redirect("/my/index.do");
                    return;
                }
                throw new Exception("查询文档为空");
            }
            catch(Exception ex){
                Utils.Log4Net.Error(ex);
                Flash["doc"] = doc;
                AddError("操作失败，系统错误");
                RedirectToReferrer();
            }
        }
        /// <summary>
        /// 我要投稿的文档上传
        /// </summary>
        /// <param name="fileData"></param>
        public void SaveFileForReward(HttpPostedFile fileData)
        {
            CancelLayout();
            CancelView();
            try
            {
                //保存文件
                string ext = string.Empty;
                string savePath = Save(fileData, ref ext);
                UserService us = Context.GetService<UserService>();
                U_UserInfo u = base.GetUser();
                DDocInfo doc = new DDocInfo()
                {
                    UserId = u.UserId,
                    DocType = ext.ToLower(),
                    FileId = Guid.NewGuid(),
                    FileName = fileData.FileName.Replace("." + ext, ""),
                    FileLength = fileData.ContentLength,
                    CreateTime = DateTime.Now,
                    PhysicalPath = savePath,
                    IsTaskDoc = true,
                    IsAudit = false
                };
                us.DocInfoBll.Insert(doc);
                Response.StatusCode = 200;
                Response.Write("{code:1,msg:'" + doc.FileId.Value.ToString() + "'}");
            }
            catch (TmmException te)
            {
                Response.StatusCode = 200;
                Response.Write("{code:0,msg:'" + te.Message + "'}");
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error("上传文档出错：" + ex.Message);
                Response.StatusCode = 500;
                Response.Write("{code:-1,msg:'" + ex.Message + "'}");
            }
        }

        [AccessibleThrough(Verb.Post)]
        public void UploadSuccessReward(Guid fileId,int reqId) {
            DocService ds = Context.GetService<DocService>();
            U_UserInfo u = base.GetUser();
            DDocInfo doc = ds.DDocInfoBll.GetByFileId(fileId);
            //更新悬赏列表的加入文档数量
            TReqDoc trdoc = ds.TReqDocBll.Get(reqId);
            trdoc.DocCount += 1;
            ds.TReqDocBll.Update(trdoc);
            //新增到加入列表
            TJoinDoc jd = new TJoinDoc() { 
                DocId = doc.DocId,
                TId = reqId,
                Title = doc.Title,
                UserId = u.UserId,
                IsWin = false
            };
            ds.TJoinDocBll.Insert(jd);
            //转至我的投稿页面
            AddSuccess("投稿成功");
            Redirect("/my/MyContribute.do");
        }
    }
}
