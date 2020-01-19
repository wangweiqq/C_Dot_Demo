using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace TestDll
{
    public partial class Form1 : Form
    {
        public delegate void MyAddCallBack(int a);
        public delegate void MyArrayCallBack(string s, IntPtr arr, ref int len);
        [DllImport("MyDll.dll",CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterCallBack(MyAddCallBack funcCallBack);
        [DllImport("MyDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Add(int a, int b);
        [DllImport("MyDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterArrayCallBack(MyArrayCallBack func);
        [DllImport("MyDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetArray(string name);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MyAddCallBack fun = new MyAddCallBack(Add2);
            //RegisterCallBack(fun);
            MyArrayCallBack func2 = new MyArrayCallBack(GetArray);
            RegisterArrayCallBack(func2);
        }

        void Add2(int total) {
            //Console.WriteLine(total.ToString());
            MessageBox.Show(total.ToString());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Add(10, 20);
            GetArray("world");
        }
        void GetArray(string s, IntPtr arr, ref int len) {
            MessageBox.Show(s);
            double[] buff = new double[len];
            Marshal.Copy(arr, buff, 0, len);
            //double[len] d1 = Marshal.PtrToStructure<double[]> (arr);
            for (int i = 0; i < len; ++i)
            {
                double d = buff[i];
                MessageBox.Show(d.ToString());
            }
        }
    }
}
