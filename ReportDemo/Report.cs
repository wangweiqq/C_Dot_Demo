using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportDemo
{
    public partial class Report : Form
    {
        public Report()
        {
            InitializeComponent();
        }
        ~Report() {
            Console.WriteLine("~Report()");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            
        }
    }
}
