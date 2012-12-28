using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMM.EAI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new frmMain();
            f.Show();
        }

        private void Login(string host, string dbName,string userId,string pwd)
        {
            string temp = "Data Source={0};Database={1};User ID={2};Password={3};Connection Lifetime=3;Min Pool Size=1;Max Pool Size=50";
            Utils.ConfigHelper.SqlConnStr = string.Format(temp, host, dbName, userId, pwd);
        }
    }
}
