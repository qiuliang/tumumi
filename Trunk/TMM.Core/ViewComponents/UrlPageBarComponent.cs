using System;
using System.Collections;
using System.Collections.Specialized;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Internal;
using TMM.Core.Common;

namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// 分页条组件
    /// </summary>
    public class UrlPageBarComponent : ViewComponent
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
            
            string content = "<table class=\"page\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
            content += "<tr>";

            string url = Request.RawUrl;
            if (url.Contains(".do"))
            {   
                string linkFormat = "<td><a href=\"{0}?{1}\">{2}</a></td>";
                string currLinkFormat = "<td><a class=\"hover\" href=\"{0}?{1}\">{2}</a></td>";
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
                    content += String.Format(linkFormat, Request.FilePath, queryString, firstPageText);

                    parameters.Set(firstName, Convert.ToString(objectPage.PrevPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    content += String.Format(linkFormat, Request.FilePath, queryString, prevPageText);
                }


                for (int i = start; i <= end; i++)
                {
                    string pageText = Convert.ToString(i);
                    parameters.Set(firstName, Convert.ToString((i - 1) * objectPage.MaxResults));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    if (i == objectPage.CurrPage)
                    {
                        content += String.Format(currLinkFormat, Request.FilePath, queryString, pageText);
                    }
                    else
                    {
                        content += String.Format(linkFormat, Request.FilePath, queryString, pageText);
                    }
                }

                if (objectPage.CurrPage < objectPage.Pages)
                {
                    parameters.Set(firstName, Convert.ToString(objectPage.NextPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    content += String.Format(linkFormat, Request.FilePath, queryString, nextPageText);

                    parameters.Set(firstName, Convert.ToString(objectPage.LastPage));
                    queryString = BuildQueryString(RailsContext.Server, parameters, true);
                    content += String.Format(linkFormat, Request.FilePath, queryString, lastPageText);
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
                        url = url.Substring(0, url.LastIndexOf("_"));
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
                

                string linkFormat = "<td><a href=\"{0}\">{1}</a></td>";
                string currLinkFormat = "<td><a class=\"hover\" href=\"{0}\">{1}</a></td>";
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
                        urlTemp = url + "_" + Convert.ToString(0) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(0);
                    }
                    content += String.Format(linkFormat, urlTemp, firstPageText);


                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "_" + Convert.ToString(objectPage.PrevPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.PrevPage);
                    }

                    content += String.Format(linkFormat, urlTemp, prevPageText);
                }


                for (int i = start; i <= end; i++)
                {
                    urlTemp = url;
                    string pageText = Convert.ToString(i);
                    if (type == 1)
                    {
                        urlTemp = url + "_" + Convert.ToString((i - 1) * objectPage.MaxResults) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString((i - 1) * objectPage.MaxResults);
                    }


                    if (i == objectPage.CurrPage)
                    {
                        content += String.Format(currLinkFormat, urlTemp, pageText);
                    }
                    else
                    {
                        content += String.Format(linkFormat, urlTemp, pageText);
                    }
                }

                if (objectPage.CurrPage < objectPage.Pages)
                {
                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "_" + Convert.ToString(objectPage.NextPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.NextPage);
                    }
                    content += String.Format(linkFormat, urlTemp, nextPageText);

                    urlTemp = url;
                    if (type == 1)
                    {
                        urlTemp = url + "_" + Convert.ToString(objectPage.LastPage) + ".html";
                    }
                    else if (type == 2)
                    {
                        urlTemp = url + "/" + Convert.ToString(objectPage.LastPage);
                    }
                    content += String.Format(linkFormat, urlTemp, lastPageText);
                }
            }
            content += "</tr>";
            content += "</table>";
            
            RenderText(content);
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
