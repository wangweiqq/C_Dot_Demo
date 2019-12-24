using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReDraw();
        }
        void ReDraw() {
            Image bkimg = Image.FromFile("img/bk.jpg");
            Bitmap bmp = new Bitmap(bkimg.Width, bkimg.Height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(bkimg, 0, 0);

            //ReadButConfig.Instance().DrawSmallParts(ref g);
            ReadTableConfig.Instance().DrawParts(ref g);
           
            g.Flush();
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = bmp;
            
            g.Dispose();
            bkimg.Dispose();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("MouseMove ({0},{1})", e.X, e.Y);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("MouseClick ({0},{1})", e.X, e.Y);
            //string str = ReadButConfig.Instance().BtnClick(new Point(e.X,e.Y));
            //if (!string.IsNullOrEmpty(str)) {
            //    Console.WriteLine("{0}被点击.", str);
            //}
        }
        string strPress = "";
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Image bkimg = Image.FromFile("img/bk.jpg");
            //float scalex = ((float)bkimg.Width) / pictureBox1.Width;
            //float scaley = ((float)bkimg.Height) / pictureBox1.Height;
            //Point p;
            //if (scalex <= scaley)
            //{
            //    int offsetx = (int)((pictureBox1.Width - (bkimg.Width / scaley))/2);
            //    p = new Point((int)((e.X - offsetx) * scaley), (int)(e.Y * scaley));
            //}
            //else
            //{
            //    int offsety = (int)((pictureBox1.Height - (bkimg.Height / scalex)) / 2);
            //    p = new Point((int)(e.X  * scalex), (int)((e.Y - offsety) * scalex));
            //}
            ////else {
            ////    p = new Point((int)(e.X * scalex), (int)(e.Y * scalex));
            ////}
            //string str = ReadButConfig.Instance().BtnClick(p);
            //if (!string.IsNullOrEmpty(str))
            //{
            //    strPress = str;
            //    ReadButConfig.Instance().GetSmallPart(str).isPress = true;
            //    //((SmallParts)(ReadConfig.Instance().table[str])).isPress = true;
            //    ReDraw();
            //}
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (!string.IsNullOrEmpty(strPress)) {
            //    ReadButConfig.Instance().GetSmallPart(strPress).isPress = false;
            //    ReDraw();
            //    strPress = "";
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ReadButConfig.Instance();
        }
    }
}
