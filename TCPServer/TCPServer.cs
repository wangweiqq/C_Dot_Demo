using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPIP
{
    public class TCPServer
    {
        private TcpListener tcpListener = null;
        private Thread listenThread;
        /// <summary>
        /// TCPClient读取回调函数byte[]数据缓冲区，int：数据长度，int：线程id号对应list Key值
        /// </summary>
        public Action<byte[], int, int> ActRevice = null;
        private Dictionary<int, TcpClient> list;
        public TCPServer(int port) {
            list = new Dictionary<int, TcpClient>();
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }
        ~TCPServer() {
            disListen();
            Console.WriteLine("TCPServer 析构");
        }
        private void ListenForClients()
        {
            this.tcpListener.Start();
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();//create a thread to handle communication
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                list.Add(clientThread.ManagedThreadId, client);
                clientThread.Start(client);
                Console.WriteLine("TCP客户端链接");
            }
        }
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;
            while (true)
            {
                bytesRead = 0;
                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }
                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }        
                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                if (ActRevice != null) {
                    ActRevice(message, bytesRead, Thread.CurrentThread.ManagedThreadId);
                }
            }
            tcpClient.Close();
            list.Remove(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("退出线程");
        }
        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="threadid">线程ID号</param>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public bool Send(int threadid, byte[] buff, int len) {
            NetworkStream clientStream = list[threadid].GetStream();
            try {
                clientStream.Write(buff, 0, len);
            } catch {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        public void disListen() {
            foreach (var v in list) {
                v.Value.Close();
            }
            list.Clear();
            listenThread.Abort();
            tcpListener.Stop();
        }
    }
}
