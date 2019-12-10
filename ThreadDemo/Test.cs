using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ThreadDemo
{
    public class Test
    {
        public Test() {
            Name = "这是一个名字";
        }
        ~Test() {
            Console.WriteLine("Test被析构");
        }
        public void run()
        {
            int count = 0;
            lock (this)
            {
                try {
                    while (true) {
                        Thread.Sleep(1000);
                        //Console.WriteLine(Name);
                        string tmp = Name + count;
                        if (myAction != null)
                        {
                            myAction(tmp);
                        }
                    }
                } catch {
                    Console.WriteLine("退出线程");
                }
            }
        }
        private string Name;
        public Action<string> myAction = null;
    }
}
