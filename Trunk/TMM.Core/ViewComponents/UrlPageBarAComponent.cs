using System;
using System.Collections;
using System.Collections.Specialized;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Internal;
using TMM.Core.Common;
using System.Text;

namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// 分页条组件
    /// </summary>
    public class UrlPageBarAComponent : ViewComponent
    {
        protected ListPage objectPage;
        protected string firstName = "first";
        protected int maxPages = 10;
        protected string firstPageText = "&lt;&lt;";
        protected string prevPageText = "&lt;";
        protected string nextPageText = "&gt;";
        protected string lastPageText = "&gt;&gt;";

        [ViewComponentParam(Required = true)]
        public ListPage ObjectPage
        {
            get { return objectPage; }
            set { objectPage = value; }
        }

        [ViewComponentParam]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [ViewComponentParam]
        public int MaxPages
        {
            get { return maxPages; }
            set { maxPages = value; }
        }

        [ViewComponentParam]
        public string FirstPageText
        {
            get { return firstPageText; }
            set { firstPageText = value; }
        }

        [ViewComponentParam]
        public string PrevPageText
        {
            get { return prevPageText; }
            set { prevPageText = value; }
        }

        [ViewComponentParam]
        public string NextPageText
        {
            get { return nextPageText; }
            set { nextPageText = value; }
        }

        [ViewComponentParam]
        public string LastPageText
        {
            get { return lastPageText; }
            set { lastPageText = value; }
        }

        /// <summary>
        /// 是否显示下拉页码跳转
        /// </summary>
        [ViewComponentParam]
        public bool IsDispSelect
        {
            get;
            set;
        }

        /// <summary>
        /// 跳转post的form ID
        /// </summary>
        [ViewComponentParam]
        public string FormId
        {
            get;
            set;
        }

        /// <summary>
        /// 每页记录数
        /// </summary>
        [ViewComponentParam]
        public int MaxRecord
        {
            get;
            set;
        }

        public override void Render()
        {
            if (objectPage.Pages <= 1)
            {
                CancelView();
                return;
            }

            bool flagParam = false;
            int type = 0;
            NameValueCollection parameters = new NameValueCollection(Request.QueryString);
            parameters.Add(Request.Form);

            StringBuilder content = new StringBuilder();

            //string content = "<table class=\"page\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
            //content += "<tr>";

            content.Append("<p class=\"right\">");

            string url = Request.RawUrl;
            if (url.Contains(".do"))
            {   
                string linkFormat = "<a href=\"{0}?{1}\">{2}</a>";
                string currLinkFormat = "<a class=\"hover\" href=\"{0}?{1}\">{2}</a>";
                string queryString;
                int start = 1;
                if (objectPage.CurrPage > maxPages / 2)
                {
                    start = objectPage.CurrPage - maxPages / 2;
                }
                int end = start + maxPages - 1;
                if (end > objectPage.Pages)
                {
                    end = objectPage.Pages;
                }

                if (objectPage.CurrPage > 1)
                {
                    parameters.Set(firstName, Convert.ToString(0));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    //content += String.Format(linkFormat, Request.FilePath, queryString, firstPageText);
                    content.Append(string.Format(linkFormat,Request.FilePath,queryString,firstPageText));

                    parameters.Set(firstName, Convert.ToString(objectPage.PrevPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    //content += String.Format(linkFormat, Request.FilePath, queryString, prevPageText);
                    content.Append(string.Format(linkFormat,Request.FilePath,queryString,prevPageText));
                }


                for (int i = start; i <= end; i++)
                {
                    string pageText = Convert.ToString(i);
                    parameters.Set(firstName, Convert.ToString((i - 1) * objectPage.MaxResults));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    if (i == objectPage.CurrPage)
                    {
                        //content += String.Format(currLinkFormat, Request.FilePath, queryString, pageText);
                        content.Append(string.Format(currLinkFormat,Request.FilePath,queryString,pageText));
                    }
                    else
                    {
                        //content += String.Format(linkFormat, Request.FilePath, queryString, pageText);
                        content.Append(string.Format(linkFormat,Request.FilePath,queryString,pageText));
                    }
                }

                if (objectPage.CurrPage < objectPage.Pages)
                {
                    parameters.Set(firstName, Convert.ToString(objectPage.NextPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    //content += String.Format(linkFormat, Request.FilePath, queryString, nextPageText);
                    content.Append(string.Format(linkFormat,Request.FilePath,queryString,nextPageText));

                    parameters.Set(firstName, Convert.ToString(objectPage.LastPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    //content += String.Format(linkFormat, Request.FilePath, queryString, lastPageText);
                    content.Append(string.Format(linkFormat,Request.FilePath,queryString,lastPageText));
                }

            }
            else
            {
                foreach (string s in parameters.AllKeys)
                {
                    if (s == firstName)
                        flagParam = true;
                }
                if (url.EndsWith(".html"))
                {
                    type = 1;
                    if (flagParam)
                        url = url.Substring(0, url.LastIndexOf("-"));
                    else
                        url = url.Substring(0, url.LastIndexOf(".html"));
                }
                else
                {
                    type = 2;
                    if (url.EndsWith("/"))
                    {
                        if (flagParam)
                        {
                            url = url.Substring(0, url.LastIndexOf("/"));
                            url = url.Substring(0, url.LastIndexOf("/"));
                        }
                        else
                        {
                            url = url.Substring(0, url.LastIndexOf("/"));
                        }
                    }
                    else
                    {
                        if (flagParam)
                        {
                            url = url.Substring(0, url.LastIndexOf("/"));
                        }
                    }
                }
                

                string linkFormat = "<a href=\"{0}\">{1}</a>";
                string currLinkFormat = "<a class=\"hover\" href=\"{0}\">{1}</a>";
                string queryString;
                int start = 1;
                if (objectPage.CurrPage > maxPages / 2)
                {
                    start = objectPage.CurrPage - maxPages / 2;
                }
                int end = start + maxPages - 1;
                if (end > objectPage.Pages)
                {
                    end = objectPage.Pages;
                }

                string urlTemp = url;
                if (objectPage.CurrPage > 1)
                {

                    if (type == 1)
                    {
                        urlTemp = url + "-" + Convert.ToString(0) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(0);
                    }
                    //content += String.Format(linkFormat, urlTemp, firstPageText);
                    content.Append(string.Format(linkFormat,urlTemp,firstPageText));

                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "-" + Convert.ToString(objectPage.PrevPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.PrevPage);
                    }

                    //content += String.Format(linkFormat, urlTemp, prevPageText);
                    content.Append(string.Format(linkFormat,urlTemp,prevPageText));
                }


                for (int i = start; i <= end; i++)
                {
                    urlTemp = url;
                    string pageText = Convert.ToString(i);
                    if (type == 1)
                    {
                        urlTemp = url + "-" + Convert.ToString((i - 1) * objectPage.MaxResults) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString((i - 1) * objectPage.MaxResults);
                    }


                    if (i == objectPage.CurrPage)
                    {
                        //content += String.Format(currLinkFormat, urlTemp, pageText);
                        content.Append(string.Format(currLinkFormat,urlTemp,pageText));
                    }
                    else
                    {
                        //content += String.Format(linkFormat, urlTemp, pageText);
                        content.Append(string.Format(linkFormat,urlTemp,pageText));
                    }
                }

                if (objectPage.CurrPage < objectPage.Pages)
                {
                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "-" + Convert.ToString(objectPage.NextPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.NextPage);
                    }
                    //content += String.Format(linkFormat, urlTemp, nextPageText);
                    content.Append(string.Format(linkFormat,urlTemp,nextPageText));

                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "-" + Convert.ToString(objectPage.LastPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.LastPage);
                    }
                    //content += String.Format(linkFormat, urlTemp, lastPageText);
                    content.Append(string.Format(linkFormat,urlTemp,lastPageText));
                }
            }

            #region 跳转区域
            if (IsDispSelect)
            {
                if (this.MaxRecord == 0)
                    this.MaxRecord = 20;
                //string gp = RailsContext.Params.Get("gotoPage");
                string first = RailsContext.Params.Get("first");

                int tmp = this.objectPage.AllResults % this.MaxRecord;
                int tmp2 = this.objectPage.AllResults / this.MaxRecord;
                int pageCount = tmp == 0 ? tmp2 : tmp2 + 1;
                if (pageCount > 1)
                {
                    content.Append("<span style='float:right;'>跳转到：");
                    content.Append("<select name='gotoPage' onchange='gotoPage(\"" + FormId +  "\",this)'>");
                    for (int i = 0; i < pageCount; i++)
                    {
                        if (!string.IsNullOrEmpty(first) && int.Parse(first) == i * this.MaxRecord)
                        {
                            content.Append(string.Format("<option value='{1}' selected='selected'>{0}</option>", i + 1, i * this.MaxRecord));
                        }
                        else
                        {
                            content.Append(string.Format("<option value='{1}'>{0}</option>", i + 1, i  * this.MaxRecord ));
                        }
                    }
                    content.Append("</select>");
                    content.Append("页</span>");
                }
            }
            #endregion

            content.Append("</p>");
            
            RenderText(content.ToString());
            CancelView();
        }

        private const string amp = "&";
        private const string encodedAmp = "&amp;";
        private static string BuildQueryString(IServerUtility serverUtil, NameValueCollection parameters, bool encodeAmp)
        {
            string queryString = CommonUtils.BuildQueryString(serverUtil, parameters, encodeAmp);
            if (encodeAmp)
            {
                if (queryString.EndsWith(encodedAmp))
                {
                    queryString = queryString.Substring(0, queryString.Length - 5);
                }
            }
            else
            {
                if (queryString.EndsWith(amp))
                {
                    queryString = queryString.Substring(0, queryString.Length - 1);
                }
            }
            return queryString;
        }

        
    }
}
