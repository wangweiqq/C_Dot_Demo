using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using TCPIP;
namespace TCPClientDemo
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();

        TCPClient client = null;
        Thread thread = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = "192.168.50.111";
            int port = 8888;
            client = new TCPClient();
            bool bl = client.connect(ip, port);
            if (bl) {
                Console.WriteLine("链接成功");
            } else {
                Console.WriteLine("链接失败");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = "Hello World";
            //string str = Encoding.UTF8.GetString(buffer, 0, len);
            byte[] buf = Encoding.ASCII.GetBytes(str);//Encoding.UTF8.GetBytes(str);//Encoding.ASCII.GetBytes(str);
            int len = buf.Length;
            bool bl = client.Send(buf, len);
            if (bl)
            {
                Console.WriteLine("发送成功");
            }
            else
            {
                Console.WriteLine("发送失败");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.ActReceive = Receive;
            thread = new Thread(new ThreadStart(client.Receive));
            thread.Start();
        }
        void Receive(byte[] buff, int len) {
            string str = Encoding.ASCII.GetString(buff, 0, len);//Encoding.UTF8.GetString(buff, 0, len);
            Console.WriteLine(str);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            client.disconnect();
            if (thread != null) {
                //thread.Abort();
                thread.Join();
                thread = null;
            }
        }
    }
}
