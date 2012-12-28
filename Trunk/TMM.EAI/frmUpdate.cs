using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace TMM.EAI
{
    public partial class frmUpdate : Form
    {
        private const string TitleSplit = "【题　名】";
        private const string TagsSplit = "【关键词】";
        private const string IntroSplit = "【文　摘】";

        public frmUpdate()
        {
            InitializeComponent();
            //过滤文件
            openFileDialog1.Filter = "文本文件(*.txt)|";
            //注册打开文件事件
            openFileDialog1.FileOk += (a, b) => { 
                textBox1.Text = openFileDialog1.FileName;
                btnAnalyze.Visible = true;
            };
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            btnUpdate2.Visible = false;
            progressBar1.Visible = false;

            //
            //DataTable dt = new DataTable();
            //dt.Columns.Add("aa");
            //for (var i = 0; i < 100; i++)
            //{
                
            //    dt.Rows.Add(i);
            //}
            //dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();            
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            btnAnalyze.Enabled = false;
            lblAnalyInfo.Text = "请稍候，正在分析...";
            

            Thread t = new Thread(a => {
                var list = new List<UpdateModel>();
                //载入txt文件
                var fs = new FileInfo(openFileDialog1.FileName).OpenRead();
                StreamReader sr = new StreamReader(fs);
                var i = 0;
                while (sr.Peek() > -1)
                {
                    var l = sr.ReadLine();
                    if (!string.IsNullOrEmpty(l))
                    {
                        var si = l.IndexOf(TitleSplit);
                        if (si > -1)
                        {
                            var up = new UpdateModel();
                            up.Title = l.Substring(TitleSplit.Length, l.Length - TitleSplit.Length);
                            //tags
                            var tags = sr.ReadLine();
                            if (tags.IndexOf(TagsSplit) > -1)
                            {
                                up.Tags = tags.Substring(TagsSplit.Length, tags.Length - TagsSplit.Length);
                            }
                            //intro
                            var intro = sr.ReadLine();
                            if (intro.IndexOf(IntroSplit) > -1)
                            {
                                up.Intro = intro.Substring(IntroSplit.Length, intro.Length - IntroSplit.Length);
                            }
                            list.Add(up);
                            i++;
                            lblAnalyInfo.Text = string.Format("正在分析第{0}条记录",i);
                        }
                    }
                }
                btnAnalyze.Enabled = true;
                lblAnalyInfo.Text = string.Format("分析结束，共发现{0}条待更新记录", list.Count);

                //在子线程中调用主线程invoke，解决GridView垂直下拉假死的问题
                this.Invoke(new Action(() => {
                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.Rows.Clear();
                        BindGrid(list);
                        btnUpdate2.Visible = true;
                        this.progressBar1.Visible = true;
                    })
                );
            });
            t.Start();

            //btnUpdate2.Visible = true;
            //
            
        }

        private void BindGrid(IList<UpdateModel> source)
        {
            
            //dataGridView1.DataSource = source;
            var col = new DataGridViewTextBoxColumn();
            col.HeaderText = "标题";
            col.Width = 250;
            dataGridView1.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "标签";
            col.Width = 200;
            dataGridView1.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "摘要";
            col.Width = 200;
            dataGridView1.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "状态";
            col.Width = 90;
            dataGridView1.Columns.Add(col);

            foreach (var item in source)
            {
                var row = new DataGridViewRow();

                var cell = new DataGridViewTextBoxCell();
                cell.Value = item.Title;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = item.Tags;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = item.Intro;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
            }
            //dataGridView1.
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    btnUpdate2.Visible = true;
                    this.progressBar1.Visible = true;
                }));
            }
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            btnUpdate2.Enabled = false;
            var i = 0;
            progressBar1.Maximum = dataGridView1.Rows.Count;
            Thread t = new Thread(new ThreadStart(() => {
                TMM.Service.DocService bll = new TMM.Service.DocService();
                bll.Initialize();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    Thread.Sleep(10);
                    //执行数据库操作
                    string err = string.Empty;
                    UpdateDoc(row,bll,ref err);
                    i++;
                    progressBar1.Value = i;
                }
                btnUpdate2.Enabled = true;
                MessageBox.Show("批量操作更新完成");
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void UpdateDoc(DataGridViewRow dr,TMM.Service.DocService bll,ref string errMsg)
        {
            if (dr.Cells[0].Value == null)
                return;

            UpdateModel um = new UpdateModel();
            um.Title = dr.Cells[0].Value.ToString();
            um.Tags = dr.Cells[1].Value.ToString();
            um.Intro = dr.Cells[2].Value.ToString();

            
            
            var p = new Hashtable();
            p.Add("Title",um.Title);
            try
            {
                var docList = bll.DDocInfoBll.GetList(p, null, 0, 1);
                if (docList != null)
                {
                    var doc = docList.FirstOrDefault();
                    if (doc == null)
                        throw new Exception(string.Format("未找到标题为【{0}】的记录",um.Title));

                    doc.Tags = um.Tags;
                    doc.Description = um.Intro;
                    //更新doc
                    bll.DDocInfoBll.Update(doc);
                    //更新tag
                    if (!string.IsNullOrEmpty(doc.Tags))
                    {
                        var tags = doc.Tags.Split(' ');
                        foreach (var item in tags)
                        {
                            var tagModel = bll.DTagBll.Get(item);
                            if (tagModel != null)
                            {
                                tagModel.UseCount += 1;
                                bll.DTagBll.Update(tagModel);
                            }
                            else
                            {
                                tagModel = new TMM.Model.D_Tag();
                                tagModel.Tag = item;
                                tagModel.UseCount = 1;
                                bll.DTagBll.Insert(tagModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            dr.Cells[3].Value = string.IsNullOrEmpty(errMsg) ? "Success" : errMsg;
            
        }
    }

    public class UpdateModel
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Intro { get; set; }
    }
}
