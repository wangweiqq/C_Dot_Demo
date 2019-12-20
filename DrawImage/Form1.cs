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

            ReadConfig.Instance().DrawSmallParts(ref g);

            //Color c = Color.FromArgb(255, 0, 0);
            //Pen btnPen = new Pen(c, 2);
            //Brush btnBrush = new SolidBrush(Color.Blue);
            //g.FillRectangle(btnBrush, new Rectangle(19, 31, 70, 30));
            //g.DrawRectangle(btnPen, new Rectangle(19, 31, 70, 30));
            //int x = 19 + 70;
            //int y = 31 + 30 / 2;
            //int x2 = 200;
            //int y2 = 118;
            //int x3 = x + (x2 - x)/2;
            ////int y3 = y + (y2 - y)/2;
            //Pen linePen = new Pen(Color.Yellow, 3);
            //Point[] pos = new Point[] { new Point(x,y),new Point(x3,y),new Point(x3,y2),new Point(x2,y2)};
            //g.DrawLines(linePen, pos);
            //Font f = new Font("微软雅黑", 9);
            //Brush fontBrush = new SolidBrush(Color.White);
            //SizeF strSize = g.MeasureString("测试矩形框", f);
            //g.DrawString("测试矩形框", f, fontBrush, new RectangleF(19, 31 + (30 - strSize.Height)/2, 70, 30));


            g.Flush();
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
            string str = ReadConfig.Instance().BtnClick(new Point(e.X,e.Y));
            if (!string.IsNullOrEmpty(str)) {
                Console.WriteLine("{0}被点击.", str);
            }
        }
        string strPress = "";
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            string str = ReadConfig.Instance().BtnClick(new Point(e.X, e.Y));
            if (!string.IsNullOrEmpty(str))
            {
                strPress = str;
                ((SmallParts)(ReadConfig.Instance().table[str])).isPress = true;
                ReDraw();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(strPress)) {
                ((SmallParts)(ReadConfig.Instance().table[strPress])).isPress = false;
                ReDraw();
                strPress = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfig.Instance();
        }
    }
}
