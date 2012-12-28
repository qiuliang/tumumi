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

using TMM.Model;
using TMM.Service;

namespace TMM.EAI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "正在联系SQL Server...";
            Thread t = new Thread(new ThreadStart(CheckSqlServer));
            t.IsBackground = true;
            t.Start();

            
        }

        private void CheckSqlServer()
        {
            
            try
            {
                TMM.Service.DocService ds = new TMM.Service.DocService();
                ds.Initialize();

                int c = ds.DDocInfoBll.GetCount(null);
                toolStripStatusLabel1.Text = "SQL Server连接成功！";
                //检查马甲用户
                CheckMajiaUser();
            }
            catch
            {
                toolStripStatusLabel1.Text = "SQL Server连接失败，请检查配置文件";
            }
           
        }

        private void CheckMajiaUser()
        {
            toolStripStatusLabel1.Text = "正在检查马甲用户列表...";
            TMM.Service.UserService us = new UserService();
            us.Initialize();
            IList<U_UserInfo> mjList = us.UserInfoBll.GetListMj(null, 0, 1000);
            if (mjList == null || mjList.Count == 0)
            {
                toolStripStatusLabel1.Text = "马甲用户列表为空，请先设置！";
            }
            else 
            {
                toolStripStatusLabel1.Text = "马甲用户检查成功！";
                Utils.ConfigHelper.UserIds = new int[mjList.Count];
                int i = 0;
                mjList.ToList().ForEach(ue => {
                    Utils.ConfigHelper.UserIds[i] = ue.UserId;
                    i++;
                });
                //绑定excel选择事件
                menuItemSelectExcel.Click += new EventHandler(menuItemSelectExcel_Click);
                openFileDialog1.FileOk += new CancelEventHandler(openFileDialog1_FileOk);
            }
        }

        void menuItemSelectExcel_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
        }

        void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string strConnTemp = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Data Source={0};Extended Properties=Excel 8.0;";
            OleDbConnection oConn = new OleDbConnection(string.Format(strConnTemp,openFileDialog1.FileName));
            gvData.Columns.Clear();
            try
            {
                oConn.Open();
                OleDbDataAdapter oda = new OleDbDataAdapter();
                OleDbCommand cmd = new OleDbCommand("select * from [Sheet1$]", oConn);
                oda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                oda.Fill(ds);
                gvData.DataSource = ds.Tables[0];
                FormatGridView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                oConn.Close();
                oConn.Dispose();
            }
        }

        

        private void menuAbout_Click(object sender, EventArgs e)
        {
            string ver = "1.0.26";
            MessageBox.Show("Version : " + ver);
        }
        /// <summary>
        /// 格式化DataGrid
        /// </summary>
        private void FormatGridView()
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Index";
            col.HeaderText = "编号";
            gvData.Columns.Insert(0, col);
            gvData.Columns.Add("colStatus", "状态");

            int i = 1;
            foreach (DataGridViewRow dr in gvData.Rows)
            {
                dr.Cells[0].Value = i;
                i++;
            }
            //设置列宽
            gvData.Columns["Price"].Width = 70;
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFileSavePath.Text))
            {
                MessageBox.Show("请选择保存文件的路径");
                return;
            }

            if (gvData.Rows.Count == 0)
            {
                MessageBox.Show("请先选择数据源文件");
            }
            else
            {
                toolStripProgressBar1.ProgressBar.Visible = true;
                toolStripProgressBar1.ProgressBar.Minimum = 0;
                toolStripProgressBar1.ProgressBar.Maximum = gvData.Rows.Count;
                int i = 0;
                int okCount = 0;
                int errCount = 0;
                foreach (DataGridViewRow dr in gvData.Rows)
                {
                    ImportDataInfo dInfo = DataRowToDataInfo(dr);
                    if (dInfo != null)
                    {
                        try
                        {
                            ImportXlsData import = new ImportXlsData(dInfo);
                            import.dataRowImportOk += new DataRowImportComplete(import_dataRowImportOk);
                            import.CurrentRow = dr;
                            import.SavePath = tbFileSavePath.Text + "\\";
                            import.Import();
                            okCount++;
                            
                        }
                        catch(Exception ex)
                        {
                            errCount++;
                        }
                        toolStripStatusLabel1.Text = string.Format("成功导入{0}个，失败{1}个",okCount,errCount);
                    }
                    i++;
                    toolStripProgressBar1.ProgressBar.Value = i;
                }
                toolStripProgressBar1.ProgressBar.Visible = false;
                toolStripStatusLabel1.Text = toolStripStatusLabel1.Text.Insert(0, "完成导入，");
                btnImport.Enabled = false;
            }
        }

        void import_dataRowImportOk(DataGridViewRow dr,bool r,string errMsg)
        {
            if (r)
            {
                dr.Cells["colStatus"].Value = "OK";
            }
            else
            {
                dr.Cells["colStatus"].Value = "导入失败，" + errMsg;
            }
        }
        private ImportDataInfo DataRowToDataInfo(DataGridViewRow dr)
        {
            ImportDataInfo info;
            try
            {
                info = new ImportDataInfo();
                if (dr.Cells[1] != null)
                {
                    info.Title = dr.Cells[1].Value as string;
                }
                if (dr.Cells[2] != null)
                {
                    info.FilePath = dr.Cells[2].Value as string;
                }
                if (dr.Cells[3] != null)
                {
                    info.Description = dr.Cells[3].Value as string;
                }
                if (dr.Cells[4] != null)
                {
                    info.Tags = dr.Cells[4].Value as string;
                }
                if (dr.Cells["Price"] != null)
                {
                    decimal r = 0;
                    decimal.TryParse(dr.Cells["Price"].FormattedValue as string, out r);
                    info.Price = r;
                }
                
                if (dr.Cells["CateId"] != null)
                {
                    int r = 0;
                    int.TryParse(dr.Cells["CateId"].FormattedValue as string, out r);
                    info.ClassId = r;
                }
                
            }
            catch(Exception ex)
            {
                info = null;
                dr.Cells["colStatus"].Value = "数据源转换错误";
            }
            return info;
        }

        private void btnSetFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFileSavePath.Text = folderBrowserDialog1.SelectedPath;
            }
            
        }

        private void menuCollection_Click(object sender, EventArgs e)
        {
            frmCollection fc = new frmCollection();
            fc.Show();
        }

        private void 批量更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUpdate fr = new frmUpdate();
            fr.Show();
        }

        private void menuItemSelectExcel_Click_1(object sender, EventArgs e)
        {

        }

        
    }
}
