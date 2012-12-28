using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TMM.EAI
{
    public class ImportDataInfo
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public decimal Price { get; set; }
        public int ClassId { get; set; }
    }
    public delegate void DataRowImportComplete(DataGridViewRow dr,bool r,string errMsg);

    public interface IDataToImport
    {
        void Import();
    }

    public class ImportXlsData : IDataToImport
    {
        private ImportDataInfo docInfo;
        public ImportXlsData(ImportDataInfo doc)
        {
            this.docInfo = doc;
        }

        public DataGridViewRow CurrentRow { get; set; }
        public event DataRowImportComplete dataRowImportOk;
        public string SavePath { get; set; }

        public void Import()
        {
            try
            {
                string ext = string.Empty;
                long fileLength = 0;
                string fileName = string.Empty;
                string p = CopyFile(this.docInfo.FilePath,ref ext,out fileLength,out fileName);
                fileName = fileName.Replace("." + ext,"");
                TMM.Model.DDocInfo model = new TMM.Model.DDocInfo() { 
                    Title = this.docInfo.Title,
                    Description = this.docInfo.Description,
                    PhysicalPath = p,
                    Price = this.docInfo.Price,
                    Tags = this.docInfo.Tags,
                    CateId = this.docInfo.ClassId,
                    CreateTime = DateTime.Now,
                    DocType = ext,
                    FileId = Guid.NewGuid(),
                    FileLength = (int)fileLength,
                    FileName = fileName,
                    UserId = Utils.ConfigHelper.UserIds[new Random().Next(0, Utils.ConfigHelper.UserIds.Length - 1)]
                };
                InsertSql(model);
                if (dataRowImportOk != null)
                {
                    dataRowImportOk(CurrentRow,true,null);
                }
            }
            catch(Exception ex)
            {
                if (dataRowImportOk != null)
                {
                    dataRowImportOk(CurrentRow, false,ex.Message);
                }
                throw ex;
            }
        }

        private string CopyFile(string filePath,ref string ext,out long length,out string fileName)
        {
            FileInfo f = new FileInfo(filePath);
            length = f.Length;
            fileName = f.Name;
            string p = Utils.ConfigHelper.SaveFile(f, ref ext, this.SavePath);
            return p;
        }
        private void InsertSql(TMM.Model.DDocInfo doc)
        {
            Service.Bll.Doc.DDocInfoBLL bll = new TMM.Service.Bll.Doc.DDocInfoBLL();
            doc.IsMajia = true;
            int docId = bll.Insert(doc);
            InsertTag(doc.Tags, docId);
        }
        private void InsertTag(string tags,int docId)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                Service.Bll.Doc.D_TagBLL bll = new TMM.Service.Bll.Doc.D_TagBLL();
                string[] tagarr = tags.Split(' ');
                bll.UpdateFromDoc(tagarr, docId);
            }
        }

    }
}
