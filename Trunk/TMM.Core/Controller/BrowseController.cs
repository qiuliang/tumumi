using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Helper;
using TMM.Core.Extends;

namespace TMM.Core.Controller
{
    [Helper(typeof(FormatHelper))]
    [Helper(typeof(UserHelper))]
    public class BrowseController : BaseController
    {
        /// <summary>
        /// 浏览列表页
        /// </summary>
        /// <param name="catalogId">分类ID</param>
        /// <param name="viewType">0：列表视图 1：大图视图</param>
        /// <param name="pageRange">页码范围 0：全部 1:1-8 2:9-100 3:100以上</param>
        /// <param name="docType">文档格式 0：全部 1：word 2：powerpoint 3：excel 4：pdf 5：文本格式 6：其他</param>
        /// <param name="orderNum">排序 0：浏览数 1：评论数 2：顶数 3：收藏数 4：最新上传</param>
        /// <param name="first">页码</param>
        public void Index(int catalogId,int viewType,int pageRange,int docType,int orderNum,int first) 
        {
            int rows = 10;
            string strOrderBy = string.Empty;
            try
            {
                DocService ds = Context.GetService<DocService>();
                Hashtable p = new Hashtable();
                //分类列表
                PropertyBag["catalogs"] = ds.SCatalogBll.GetTopNode();
                //精品推荐
                PropertyBag["recommendList"] = ds.DDocInfoBll.GetRecommendList(0,7);
                //热门文档
                PropertyBag["hotList"] = ds.DDocInfoBll.GetHotList(0, 10);
                //文档列表
                p.Add("IsAudit",true);
                if (catalogId != 0)
                {
                    int[] cateids = Utils.TmmUtils.GetSubCatalogIds(catalogId);
                    if (cateids != null && cateids.Length > 0)
                        p.Add("CateIds", cateids);
                    else
                        p.Add("CateId", catalogId);
                }
                if (docType != 0) {
                    string[] DocTypes = GetDocTypes(docType);
                    p.Add("DocTypes",DocTypes);
                }
                switch (orderNum)
                {
                    case 0 :
                        strOrderBy = "ViewCount DESC";
                        break;
                    case 1:
                        strOrderBy = "CommentCount DESC";
                        break;
                    case 2:
                        strOrderBy = "UpCount DESC";
                        break;
                    case 3:
                        strOrderBy = "FavCount DESC";
                        break;
                    case 4:
                        strOrderBy = "CreateTime DESC";
                        break;
                }
                int count = ds.DDocInfoBll.GetCount(p);
                //如果超过20页，则只显示20页内容
                count = count > rows * 20 ? rows * 20 : count;
                IList<DDocInfo> list = ds.DDocInfoBll.GetList(p,strOrderBy,first,rows);
                Common.ListPage lp = new TMM.Core.Common.ListPage((IList)list, first, rows, count);
                PropertyBag["listPage"] = lp;
                //参数
                PropertyBag["catalogId"] = catalogId;
                PropertyBag["viewType"] = viewType;
                PropertyBag["pageRange"] = pageRange;
                PropertyBag["docType"] = docType;
                PropertyBag["orderNum"] = orderNum;
                PropertyBag["first"] = first;
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex);
            }
        }

        private string[] GetDocTypes(int typeId) {
            List<string> types = new List<string>();
            var p = from t in ConfigHelper.AllDocTypes
                    where t.TypeId == typeId
                    select t;
            p.ToList().ForEach(d => types.Add(d.TypeName));
            return types.ToArray();
        }
        

        [Layout("doc")]
        public void Default(int docId)
        {
            UserService us = Context.GetService<UserService>();
            SystemService ss = Context.GetService<SystemService>();
            DDocInfo doc = us.DocInfoBll.Get(docId);
            doc.Description = doc.Description.ReplaceEnterStr();
            PropertyBag["doc"] = doc;
            if (!doc.IsAudit && !doc.IsTaskDoc)
            {
                if (!(Context.UrlReferrer.IndexOf("/admin/doc/index.do") > -1)) //如果从管理页面过来，不抛异常
                {
                    throw new Common.TmmException("该文档不存在");
                }
            }

            try
            {
                
                
                //该用户其他文档
                IList<DDocInfo> otherList = us.DocInfoBll.GetListByUser(doc.UserId, 0, 9, doc.DocId);
                PropertyBag["otherList"] = otherList;
                //文档分类
                if (doc.CateId.HasValue)
                {
                    S_Catalog c3 = ss.SCatalogBll.Get(doc.CateId.Value);
                    S_Catalog c2 = null;
                    S_Catalog c1 = null;
                    if (c3 != null && c3.Pid.HasValue)
                    {
                        c2 = ss.SCatalogBll.Get(c3.Pid.Value);
                        if (c2.Pid.HasValue)
                        {
                            c1 = ss.SCatalogBll.Get(c2.Pid.Value);
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    if (c3 != null)
                    {
                        sb.Append(string.Format("<a href=\"/list-{1}-0-0-0-0-0.html\">{0}</a>", c3.CatalogName,c3.CatalogId));
                        if (c2 != null)
                        {
                            sb.Append(string.Format("&nbsp;--&nbsp;<a href=\"/list-{1}-0-0-0-0-0.html\">{0}</a>", c2.CatalogName,c2.CatalogId));
                            if (c1 != null)
                            {
                                sb.Append(string.Format("&nbsp;--&nbsp;<a href=\"/list-{1}-0-0-0-0-0.html\">{0}</a>", c1.CatalogName,c1.CatalogId));
                            }
                        }
                    }
                    PropertyBag["catalog"] = sb.ToString();
                }
                //标签
                if (!string.IsNullOrEmpty(doc.Tags))
                {
                    string[] tags = doc.Tags.Split(' ');
                    List<D_Tag> tagList = new List<D_Tag>();
                    tags.ToList().ForEach(s =>
                    {
                        if (!string.IsNullOrEmpty(s.Trim()))
                        {
                            D_Tag dt = us.DTagBll.Get(s);
                            if (dt != null)
                            {
                                tagList.Add(dt);
                            }
                        }

                    });
                    PropertyBag["tagList"] = tagList;
                }
                //相关文档
                IList<DDocInfo> recommandList = us.DocInfoBll.GetRelativeList(docId);
                if (recommandList == null)
                {
                    PropertyBag["relativeDocs"] = us.DocInfoBll.GetRecommendList(0, 25);
                    PropertyBag["recommend"] = true;
                }
                else
                {
                    PropertyBag["relativeDocs"] = recommandList;
                }
                //浏览量
                us.DocInfoBll.UpdateViewCount(docId);
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        
        public void GotoDown(int docId) {
            PropertyBag["docId"] = docId;            
        }
        
    }
}
