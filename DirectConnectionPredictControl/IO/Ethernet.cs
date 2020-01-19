using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DirectConnectionPredictControl.IO
{
    class Ethernet
    {
        private UdpClient udpClient;
        private TcpClient tcpClient;
        private string hostIP;
        private string localIP;
        private int port;
        private static Ethernet instance;
        private IPEndPoint remoteIpEnd;
        private BinaryWriter binaryWriter;
        private BinaryReader binaryReader;
        private NetworkStream networkStream;

        private Ethernet(string hostIP, int port)
        {
            this.hostIP = hostIP;
            this.port = port;
            this.localIP = GetLocalIPv4();
            remoteIpEnd = new IPEndPoint(IPAddress.Parse(hostIP), port);

            tcpClient = new TcpClient();
            Connect();
            networkStream = tcpClient.GetStream();

            binaryWriter = new BinaryWriter(networkStream);
        }

        public void Connect()
        {
            tcpClient.Connect(remoteIpEnd);
        }

        public static Ethernet GetInstance(string hostIP, int port)
        {
            if (instance == null)
            {
                instance = new Ethernet(hostIP, port);
                return instance;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 同以太网中通过TCP获取数据
        /// </summary>
        /// <returns></returns>
        public byte[] Recv()
        {
            try
            {
                binaryReader = new BinaryReader(networkStream);
                byte[] recvData = binaryReader.ReadBytes(32);
                if (recvData.Length > 0)
                {
                    return recvData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 向以太网发送数据（TCP）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            try
            {
                binaryWriter.Write(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ipList = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }
        
    }
}
