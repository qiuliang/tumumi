using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TMM.Model;

namespace TMM.Core.Common
{
    /// <summary>
    /// 异步消息的包装类
    /// </summary>
    public class AsynMessage
    {
        private Queue<M_Message> messages;

        public AsynMessage(Queue<M_Message> msgs)
        {
            TMM.Persistence.SqlMapper.Instance().SessionStore =
                        new IBatisNet.DataMapper.SessionStore.HybridWebThreadSessionStore(TMM.Persistence.SqlMapper.Instance().Id);
            this.messages = msgs;
        }
        public AsynMessage(M_Message msg)
        {
            TMM.Persistence.SqlMapper.Instance().SessionStore =
                        new IBatisNet.DataMapper.SessionStore.HybridWebThreadSessionStore(TMM.Persistence.SqlMapper.Instance().Id);
            this.messages = new Queue<M_Message>();
            this.messages.Enqueue(msg);
        }

        public void Send()
        {
            Thread theThread = new Thread(new ThreadStart(InsertMsg));
            try
            {
                theThread.IsBackground = true;
                theThread.Start();
                         
            }
            catch (Exception ex)
            {
                Utils.Log4Net.Error(ex);
            }
        }

        private void InsertMsg()
        {
            Service.Bll.User.M_MessageBLL bll = new TMM.Service.Bll.User.M_MessageBLL();
            if (messages != null)
            {
                foreach (M_Message msg in messages)
                {
                    bll.Insert(msg);
                    Thread.Sleep(100);
                }
            }            
        }
    }
}
