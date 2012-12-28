using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hubble.Framework;
using Hubble.SQLClient;
using Hubble.Analyzer;
using Hubble.Core.Analysis.HighLight;

namespace TMM.Core.Search
{
    public class Index
    {
        #region 属性
        public string TableName { get; set; }
        public string DocType { get; set; }
        #endregion

        private string titleAnalyzerName;   //标题分析器名称
        private string descAnalyzerName; //描述分析器名称

        public ArrayList Search(string keyWords, int pageNo, int pageLen, out int recCount, out long elapsedMilliseconds)
        {
            string connStr = Helper.ConfigHelper.HubbleConnStr;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            DataSet ds;
            using (HubbleConnection conn = new HubbleConnection(connStr))
            {
                #region hubble
                conn.Open();
                GetAnalyzerName(conn, TableName);
                string wordssplitbyspace;
                HubbleCommand matchCmd = new HubbleCommand(conn);
                string matchString = matchCmd.GetKeywordAnalyzerStringFromServer(
                    TableName, "Title", keyWords, int.MaxValue, out wordssplitbyspace);
                HubbleDataAdapter da = new HubbleDataAdapter();
                string sqlTemplate = string.Empty;
                if (string.IsNullOrEmpty(this.DocType))
                    sqlTemplate = "select between @begin to @end * from {0} where IsAudit=1 and ( Description match @matchString or title^2 match @matchString or tags^2 match @matchString ) order by score desc";
                else
                    sqlTemplate = "select between @begin to @end * from {0} where IsAudit=1 and DocType='" + this.DocType + "' and ( Description match @matchString or title^2 match @matchString or tags^2 match @matchString ) order by score desc";
                
                da.SelectCommand = new HubbleCommand( string.Format(sqlTemplate,TableName),conn);
                da.SelectCommand.Parameters.Add("@begin",pageNo * pageLen);
                da.SelectCommand.Parameters.Add("@end", (pageNo+1) * pageLen - 1);
                da.SelectCommand.Parameters.Add("@matchString", matchString);
                

                da.SelectCommand.CacheTimeout = 0;

                ds = new DataSet();
                HubbleCommand cmd = da.SelectCommand;
                ds = cmd.Query(0);

                long[] docids = new long[ds.Tables[0].Rows.Count];

                int i = 0;

                //foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                //{
                //    docids[i++] = (long)row["DocumentId"];
                //}
                #endregion
            }
            recCount = ds.Tables[0].MinimumCapacity;
            ArrayList rList = DataToList(ds, keyWords,true);
            watch.Stop();
            elapsedMilliseconds = watch.ElapsedMilliseconds;
            return rList;
        }

        public ArrayList DataToList(DataSet ds,string keyWords,bool isHighlight)
        {
            ArrayList result = new ArrayList();
            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                Model.DDocInfo doc = new TMM.Model.DDocInfo();
                doc.Title = row["Title"].ToString();
                doc.Description = row["Description"].ToString();
                doc.DocType = row["DocType"].ToString();
                doc.DocId = int.Parse( row["DocumentId"].ToString() );
                doc.UserId = int.Parse( row["UserId"].ToString());
                doc.CreateTime = DateTime.Parse( row["CreateTime"].ToString());
                doc.UpCount = int.Parse(row["UpCount"].ToString());
                doc.ViewCount = int.Parse(row["ViewCount"].ToString());

                if (isHighlight)
                {

                    SimpleHTMLFormatter simpleHTMLFormatter =
                        new SimpleHTMLFormatter("<font color=\"red\">", "</font>");

                    Highlighter titleHighlighter;
                    Highlighter contentHighlighter;

                    if (titleAnalyzerName.Equals("PanGuSegment", StringComparison.CurrentCultureIgnoreCase))
                    {
                        titleHighlighter =
                        new Highlighter(simpleHTMLFormatter, new PanGuAnalyzer());
                    }
                    else if (titleAnalyzerName.Equals("EnglishAnalyzer", StringComparison.CurrentCultureIgnoreCase))
                    {
                        titleHighlighter = new Highlighter(simpleHTMLFormatter, new Hubble.Core.Analysis.EnglishAnalyzer());
                    }
                    else
                    {
                        titleHighlighter = new Highlighter(simpleHTMLFormatter, new Hubble.Core.Analysis.SimpleAnalyzer());
                    }

                    if (descAnalyzerName.Equals("PanGuSegment", StringComparison.CurrentCultureIgnoreCase))
                    {
                        contentHighlighter =
                        new Highlighter(simpleHTMLFormatter, new PanGuAnalyzer());
                    }
                    else if (descAnalyzerName.Equals("EnglishAnalyzer", StringComparison.CurrentCultureIgnoreCase))
                    {
                        contentHighlighter = new Highlighter(simpleHTMLFormatter, new Hubble.Core.Analysis.EnglishAnalyzer());
                    }
                    else
                    {
                        contentHighlighter = new Highlighter(simpleHTMLFormatter, new Hubble.Core.Analysis.SimpleAnalyzer());
                    }

                    titleHighlighter.FragmentSize = 50;
                    contentHighlighter.FragmentSize = 50;


                    doc.SearchSummary = contentHighlighter.GetBestFragment(keyWords, doc.Description);
                    string titleHighlight = titleHighlighter.GetBestFragment(keyWords, doc.Title);

                    if (!string.IsNullOrEmpty(titleHighlight))
                    {
                        doc.Title = titleHighlight;
                    }
                }

                result.Add(doc);
            }
            return result;
        }

        private void GetAnalyzerName(HubbleConnection conn, string tableName)
        {
            if (titleAnalyzerName != null && descAnalyzerName != null)
            {
                return;
            }

            string sql = string.Format("exec SP_Columns '{0}'", tableName.Replace("'", "''"));

            HubbleCommand cmd = new HubbleCommand(sql, conn);

            foreach (System.Data.DataRow row in cmd.Query().Tables[0].Rows)
            {
                if (row["FieldName"].ToString().Equals("Title", StringComparison.CurrentCultureIgnoreCase))
                {
                    titleAnalyzerName = row["Analyzer"].ToString();
                }

                if (row["FieldName"].ToString().Equals("Description", StringComparison.CurrentCultureIgnoreCase))
                {
                    descAnalyzerName = row["Analyzer"].ToString();
                }
            }

        }
        /// <summary>
        /// 搜索可能喜欢的文档
        /// </summary>  
        /// <param name="pageNo"></param>
        /// <param name="pageLen"></param>
        /// <returns></returns>
        public ArrayList SearchMaybeLike(string title, int pageNo, int pageLen,int docId)
        {
            string connStr = Helper.ConfigHelper.HubbleConnStr;            
            DataSet ds;
            ArrayList rList = new ArrayList();
            using (HubbleConnection conn = new HubbleConnection(connStr))
            {
                #region hubble
                conn.Open();
                GetAnalyzerName(conn, TableName);
                string wordssplitbyspace;
                HubbleCommand matchCmd = new HubbleCommand(conn);

                string matchString = matchCmd.GetKeywordAnalyzerStringFromServer(
                    TableName, "Title", title, int.MaxValue, out wordssplitbyspace);

                HubbleDataAdapter da = new HubbleDataAdapter();
                da.SelectCommand = new HubbleCommand(
                    string.Format(
                        "select between @begin to @end * from {0} where IsAudit=1 and ( Description match @matchString or title^2 match @matchString or tags^2 match @matchString ) "
                        + " and DocumentId<>@docid order by score desc"
                        , TableName),
                        conn);
                da.SelectCommand.Parameters.Add("@begin", pageNo * pageLen);
                da.SelectCommand.Parameters.Add("@end", (pageNo + 1) * pageLen - 1);
                da.SelectCommand.Parameters.Add("@matchString", matchString);
                da.SelectCommand.Parameters.Add("@docid",docId);

                da.SelectCommand.CacheTimeout = 0;

                ds = new DataSet();
                HubbleCommand cmd = da.SelectCommand;
                ds = cmd.Query(0);

                long[] docids = new long[ds.Tables[0].Rows.Count];

                int i = 0;

                foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                {
                    docids[i++] = (long)row["DocId"];
                }

                rList.AddRange(DataToList(ds, title, false));
                
                #endregion
            }
               
            
            return rList;
        }
    }
}
