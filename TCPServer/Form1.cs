using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();
        TCPIP.TCPServer listen = null;
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
            if (listen != null) {
                listen.disListen();
                listen = null;
            }
            listen = new TCPIP.TCPServer(8888);
            listen.ActRevice = Revice;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listen.disListen();
            listen = null;
        }
        private void Revice(byte[] buff, int len, int threadid) {
            Console.WriteLine("接收:");
            string str = Encoding.ASCII.GetString(buff, 0, len);
            Console.WriteLine(str);
            string str2 = "Hello 2";
            byte[] buf = Encoding.ASCII.GetBytes(str2);
            listen.Send(threadid, buf, buf.Length);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listen != null)
            {
                listen.disListen();
                listen = null;
            }
        }
    }
}
