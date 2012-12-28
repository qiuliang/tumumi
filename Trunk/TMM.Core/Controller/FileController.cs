using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using Castle.MonoRail.Framework;
using TMM.Service;
using TMM.Model;
using TMM.Core.Helper;
using TMM.Core.Extends;

namespace TMM.Core.Controller
{
    [Helper(typeof(FormatHelper))]
    [Helper(typeof(UserHelper))]
    [Filter(ExecuteEnum.BeforeAction, typeof(Filter.MyTmmFilter))]
    public class FileController : BaseController
    {
        public void DownloadDoc(int docId,string valkey) 
        {
            CancelLayout();
            CancelView();
            try {
                DocService ds = Context.GetService<DocService>();
                U_UserInfo logonUser = base.GetUser();
                DDocInfo doc = ds.DDocInfoBll.Get(docId);
                FileStream myFile = null;
                BinaryReader br = null;
                try
                {
                    #region 下载文档前需要经过一系列的权限判断
                    #endregion
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

                    //if (Request.Headers["Range"] != null)
                    //{
                    //    Response.StatusCode = 206;
                    //    String[] range = Request.Headers["Range"].Split(new Char[] { '=', '-' });
                    //    startBytes = ConvertUtility.ToInt64(range[1]);
                    //}

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
                    //下载成功，写入一条下载日志
                    if (doc.Price == 0 && doc.UserId != logonUser.UserId)
                    {
                        //先判断是否已经写入过下载日志了
                        bool isExistLog = ds.DownloadLogBll.IsExistDownLog(logonUser.UserId, doc.DocId);
                        if (!isExistLog)
                        {
                            DownloadLog downLog = new DownloadLog()
                            {
                                CreateTime = DateTime.Now,
                                DocId = doc.DocId,
                                UserId = logonUser.UserId
                            };
                            ds.DownloadLogBll.Insert(downLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log4Net.Error("file DownloadFile " + ex.ToString());                    
                    //DoAlert("异常错误 03");
                }
                finally
                {
                    if (br != null) br.Close();
                    if (myFile != null) myFile.Close();
                }
            }
            catch (Exception ex) {
                Utils.Log4Net.Error(ex);
            }
            
        }
        
    }
}
