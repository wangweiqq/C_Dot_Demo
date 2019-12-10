using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
namespace TCPIP
{
    public class TCPClient
    {
        /// <summary>
        /// TCP套接字
        /// </summary>
        private Socket client_socket;
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        private IPAddress ipAddress;
        /// <summary>
        /// 端口号
        /// </summary>
        private int port;
        /// <summary>
        /// 接收缓冲
        /// </summary>
        byte[] buffer = new byte[1024 * 1024 * 3];
        /// <summary>
        /// 接受TCP信息回调函数,byte[]为缓冲区，int为接收长度
        /// </summary>
        public Action<byte[],int> ActReceive = null;
        public TCPClient() {
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        ~TCPClient() {
            if (client_socket != null)
            {
                disconnect();
            }
        }
        /// <summary>
        /// 链接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public bool connect(string ip, int port) {
            this.ipAddress = IPAddress.Parse(ip);
            this.port = port;
            return reconnect();
        }
        /// <summary>
        /// 重连
        /// </summary>
        public bool reconnect() {
            try
            {
                client_socket.Connect(this.ipAddress, this.port);
                Console.WriteLine("连接服务器成功");
            }
            catch {
                Console.WriteLine("连接服务器失败");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 断开链接
        /// </summary>
        public void disconnect() {
            if (client_socket.Connected)
            {
                client_socket.Shutdown(SocketShutdown.Both);
            }
            client_socket.Close();
        }
        /// <summary>
        /// 接收TCP信息，具体内容请注册ActReceive回调
        /// 请把Receive函数放在线程中调用
        /// </summary>
        public void Receive() {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际接收到的有效字节数
                    int len = client_socket.Receive(buffer);
                    if (len == 0)
                    {
                        Console.WriteLine("退出TCPClient 线程");
                        break;
                    }
                    if (ActReceive != null) {
                        ActReceive(buffer, len);
                    }
                    //string str = Encoding.UTF8.GetString(buffer, 0, len);
                    //Console.WriteLine(str);
                }
                catch {
                    Console.WriteLine("异常退出TCPClient 线程");
                    break;
                }
            }
        }
        public bool Send(byte[] buf,int size) {
            try
            {
                client_socket.Send(buf, size, 0);
            }
            catch {
                return false;
            }
            return true;
        }
    }
}
