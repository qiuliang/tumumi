using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using NPOI;
using NPOI.HSSF;
using NPOI.POIFS;
using NPOI.Util;

namespace TMM.EAI
{
    public partial class frmCollection : Form
    {
        private string[] docTypes;

        public frmCollection()
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void frmCollection_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            try
            {
                string types = new System.Configuration.AppSettingsReader().GetValue("docFiles", typeof(string)).ToString();
                docTypes = types.Split(',');
            }
            catch
            {
                toolStripStatusLabel1.Text = "配置文件读取错误，请设置正确的文件格式";
            }

        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCollection_Click(object sender, EventArgs e)
        {
            if (tbFolderPath.Text != "")
            {
                this.btnCollection.Enabled = false;
                toolStripStatusLabel1.Text = "正在扫描目录...";
                Thread t = new Thread(new ThreadStart(ScanFolder));
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                MessageBox.Show("请先选择目录");
            }
        }

        private void ScanFolder()
        {
            DirectoryInfo di = new DirectoryInfo(tbFolderPath.Text);
            FileInfo[] files = di.GetFiles();
            toolStripStatusLabel1.Text = string.Format("共找到文件{0}个",files.Length);
            //过滤文件
            List<FileProperty> flist = new List<FileProperty>();
            foreach (var f in files)
            {
                if (docTypes.Contains(f.Extension.Substring(1,f.Extension.Length - 1)))
                {
                    flist.Add(new FileProperty() { 
                        Path = f.FullName,
                        Name = f.Name.Replace(f.Extension,"")
                    });
                }
            }
            //to excel
            ToExcel(flist);
        }

        private void ToExcel(List<FileProperty> flist)
        {
            if (flist.Count == 0)
            {
                toolStripStatusLabel1.Text += "，未找到需要提取的文件";
            }
            else 
            {
                toolStripStatusLabel1.Text += string.Format("，待导入文件{0}个",flist.Count);
                btnCollection.Enabled = true;
                btnCollection.Click -= btnCollection_Click;
                btnCollection.Text = "保存至本地...";
                btnCollection.Click += (o,e) => SaveExcel(flist);
            }
        }

        private void SaveExcel(List<FileProperty> flist)
        {
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = ".xls";
            saveFileDialog1.AutoUpgradeEnabled = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = "正在生成Excel文件";
                string path = saveFileDialog1.FileName;
                NPOI.HSSF.UserModel.HSSFWorkbook workBook = new NPOI.HSSF.UserModel.HSSFWorkbook();
                NPOI.SS.UserModel.Sheet sheet = workBook.CreateSheet("Sheet1");
                NPOI.SS.UserModel.Row row1 = sheet.CreateRow(0);
                row1.CreateCell(0).SetCellValue("Title");
                row1.CreateCell(1).SetCellValue("FilePath");
                row1.CreateCell(2).SetCellValue("Description");
                row1.CreateCell(3).SetCellValue("Tags");
                row1.CreateCell(4).SetCellValue("Price");
                row1.CreateCell(5).SetCellValue("CateId");
                row1.CreateCell(6).SetCellValue("Test");

                for(int i=0;i<flist.Count;i++)
                {
                    NPOI.SS.UserModel.Row r = sheet.CreateRow(i + 1);
                    r.CreateCell(0).SetCellValue(flist[i].Name);
                    r.CreateCell(1).SetCellValue(flist[i].Path);
                }
                
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    workBook.Write(fs);
                }
                toolStripStatusLabel1.Text = "生成Excel成功";
            }
        }

        
    }

    public class FileProperty
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }
}
