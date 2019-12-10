using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace ThreadDemo
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();

        Thread thread;
        Test t1;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            t1 = new Test();
            t1.myAction = CallBack;
            thread = new Thread(new ThreadStart(t1.run));
            thread.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            t1 = null;
            thread.Abort();
            thread.Join();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
        void CallBack(string str) {
            Console.WriteLine(str);
        }
    }
}
