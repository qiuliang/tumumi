using System;
using System.Collections;

namespace TMM.Core.Common
{
    /// <summary>
    /// 分页的数据模型类
    /// </summary>
    [Serializable]
    public class ListPage
    {
        /// <summary>
        /// 当前页的记录列表
        /// </summary>
        private IList list;

        /// <summary>
        /// 当前页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        private int firstResult;

        /// <summary>
        /// 每一页的记录数
        /// </summary>
        private int maxResults;

        /// <summary>
        /// 全部记录的数量
        /// </summary>
        private int allResults;

        /// <summary>
        /// 上一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        private int prevPage;

        /// <summary>
        /// 下一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        private int nextPage;

        /// <summary>
        /// 最后一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        private int lastPage;

        /// <summary>
        /// 当前页是第几页
        /// </summary>
        private int currPage;

        /// <summary>
        /// 共有多少页
        /// </summary>
        private int pages;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">当前页的记录列表</param>
        /// <param name="firstResult">当前页的第一条记录在全部记录中的索引数，以0为起始索引</param>
        /// <param name="maxResults">每一页的记录数</param>
        /// <param name="allResults">全部记录的数量</param>
        public ListPage(IList list, int firstResult, int maxResults, int allResults)
        {
            this.list = list;
            this.firstResult = firstResult;
            this.maxResults = maxResults;
            this.allResults = allResults;

            this.prevPage = firstResult - maxResults;
            if (this.prevPage < 0)
            {
                this.prevPage = 0;
            }
            if (allResults <= maxResults)
            {
                this.lastPage = 0;
            }
            else
            {
                this.lastPage = allResults % maxResults;
                this.lastPage = (this.lastPage == 0) ? (allResults - maxResults) : (allResults - this.lastPage);
            }
            this.nextPage = firstResult + maxResults;
            if (this.nextPage > this.lastPage)
            {
                this.nextPage = this.lastPage;
            }

            pages = allResults % maxResults;
            if (pages == 0)
                pages = allResults / maxResults;
            else
                pages = allResults / maxResults + 1;

            currPage = (firstResult + maxResults) / maxResults;
        }
        public ListPage( int firstResult, int maxResults, int allResults)
        {
            
            this.firstResult = firstResult;
            this.maxResults = maxResults;
            this.allResults = allResults;

            this.prevPage = firstResult - maxResults;
            if (this.prevPage < 0)
            {
                this.prevPage = 0;
            }
            if (allResults <= maxResults)
            {
                this.lastPage = 0;
            }
            else
            {
                this.lastPage = allResults % maxResults;
                this.lastPage = (this.lastPage == 0) ? (allResults - maxResults) : (allResults - this.lastPage);
            }
            this.nextPage = firstResult + maxResults;
            if (this.nextPage > this.lastPage)
            {
                this.nextPage = this.lastPage;
            }

            pages = allResults % maxResults;
            if (pages == 0)
                pages = allResults / maxResults;
            else
                pages = allResults / maxResults + 1;

            currPage = (firstResult + maxResults) / maxResults;
        }
        /// <summary>
        /// 当前页的记录列表
        /// </summary>
        public IList List
        {
            get { return list; }
        }

        /// <summary>
        /// 当前页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        public int FirstResult
        {
            get { return firstResult; }
        }

        /// <summary>
        /// 每一页的记录数
        /// </summary>
        public int MaxResults
        {
            get { return maxResults; }
        }

        /// <summary>
        /// 全部记录的数量
        /// </summary>
        public int AllResults
        {
            get { return allResults; }
        }

        /// <summary>
        /// 上一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        public int PrevPage
        {
            get { return prevPage; }
        }

        /// <summary>
        /// 下一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        public int NextPage
        {
            get { return nextPage; }
        }

        /// <summary>
        /// 最后一页的第一条记录在全部记录中的索引数，以0为起始索引
        /// </summary>
        public int LastPage
        {
            get { return lastPage; }
        }

        /// <summary>
        /// 当前页是第几页
        /// </summary>
        public int CurrPage
        {
            get { return currPage; }
        }

        /// <summary>
        /// 共有多少页
        /// </summary>
        public int Pages
        {
            get { return pages; }
        }
    }
}
