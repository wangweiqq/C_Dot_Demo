using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LmiScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //析构相机
            LmiScanner scanner = LmiScanner.intance();
            scanner.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //拍照
            LmiScanner scanner = LmiScanner.intance();
            scanner.SoftTrigger();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //序列号初始化相机
            LmiScanner scanner = LmiScanner.intance();
            scanner.InitCamera(51793);
            scanner.UseJob("TEST.job");
            scanner.StartListen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Ip地址初始化相机
            LmiScanner scanner = LmiScanner.intance();
            scanner.InitCamera("192.168.11.13");
            
            scanner.UseJob("TEST.job");
            scanner.StartListen();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //开始监听
            LmiScanner scanner = LmiScanner.intance();
            scanner.StartListen();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //停止监听
            LmiScanner scanner = LmiScanner.intance();
            scanner.StopListen();
        }
        void onDataPicture(Bitmap bmap) {
            pictureBox1.Image = bmap;
            Console.WriteLine("Image width = {0},height = {0}", bmap.Width, bmap.Height);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LmiScanner scanner = LmiScanner.intance();
            scanner.ActGetBit = onDataPicture;
        }
    }
}
