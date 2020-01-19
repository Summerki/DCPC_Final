using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// 用来存放关于FTP操作的类
    /// </summary>
    public class FTP
    {
        #region 远程FTP服务器所需要的东西
        /// <summary>
        /// 远程ip
        /// </summary>
        public string strRemoteIp;

        /// <summary>
        /// 远程端口
        /// </summary>
        public string strRemotePort;

        /// <summary>
        /// 远程用户
        /// </summary>
        public string strRemoteUser;

        /// <summary>
        /// 远程密码
        /// </summary>
        public string strRemotePass;

        /// <summary>
        /// 远程服务器的路径
        /// </summary>
        public string strRemotePath;

        /// <summary>
        /// 判断是否连接FTP服务器成功的bool类型变量
        /// </summary>
        public bool bConnected;

        /// <summary>
        /// 判断下载任务是否暂停
        /// </summary>
        public bool bPause;

        #endregion

        #region 从远程服务器获得的东西
        /// <summary>
        /// 获得从FTP服务器发送来的应答信息
        /// </summary>
        public string strMsg;

        /// <summary>
        /// 从获得的FTP服务器的应答信息中获得应答码
        /// </summary>
        public int iReplyCode;

        /// <summary>
        ///  用来进行连接控制的socket
        /// </summary>
        public Socket ControlSccket;

        /// <summary>
        /// 传输模式
        /// </summary>
        public TransferType trType;

        /// <summary>
        /// 传输模式，分为Binary和ASCII
        /// </summary>
        public enum TransferType
        {
            Binary, // 二进制，在进行文件下载上传时会用到
            ASCII, // ASCII，在接受发送命令时用到
        };

        /// <summary>
        /// 接受和发送数据的缓冲区
        /// </summary>
        public static int BLOCK_SIZE = 4096;
        Byte[] buffer = new Byte[BLOCK_SIZE];

        /// <summary>
        /// 默认的编码方式
        /// </summary>
        Encoding ASCII = Encoding.Default;
        #endregion

        #region 构造函数，用于初始化对象

        /// <summary>
        /// 不带参数的构造函数
        /// </summary>
        public FTP()
        {
            strRemoteIp = "";
            strRemotePort = "";
            strRemoteUser = "";
            strRemotePass = "";
            bConnected = false;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="RemoteIp"></param>
        /// <param name="RemotePort"></param>
        /// <param name="RemoteUser"></param>
        /// <param name="RemotePass"></param>
        public FTP(string RemoteIp, string RemotePort, string RemoteUser, string RemotePass)
        {
            strRemoteIp = RemoteIp;
            strRemotePort = RemotePort;
            strRemoteUser = RemoteUser;
            strRemotePass = RemotePass;
            Connect();
        }

        #endregion



        /// <summary>
        /// 建立连接函数
        /// </summary>
        public void Connect()
        {
            ControlSccket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(strRemoteIp), int.Parse(strRemotePort));

            // 尝试连接
            try
            {
                ControlSccket.Connect(endPoint);
            }
            catch (Exception)
            {
                throw new IOException("不能连接到远程的FTP服务器");
            }

            // 若连接成功后，获取FTP返回的应答信息
            iReplyCode = Get_iReplyCode();
            if (iReplyCode != 220)
            {
                DisConnect();
                throw new IOException(strMsg.Substring(4));
            }

            // 登录
            SendCommand("USER " + strRemoteUser);
            if (!(iReplyCode == 331 || iReplyCode == 230))
            {
                CloseSocketControl();//关闭连接
                throw new IOException(strMsg.Substring(4));
            }
            if (iReplyCode != 230)// 开始说不是331或者230就关闭连接，现在逻辑更进一步，再如果不是230，意思就是如果是331的话
            {
                SendCommand("PASS " + strRemotePass);// 返回331那就继续输入密码，输入密码后应该返回230
                if (!(iReplyCode == 230 || iReplyCode == 202))
                {
                    CloseSocketControl();//关闭连接
                    throw new IOException(strMsg.Substring(4));
                }
            }
            bConnected = true;

            //切换远程FTP服务器的显示目录
            chDir(@"\");
        }

        /// <summary>
        /// 向FTP服务器发送PWD命令，取出应答信息中的路径
        /// </summary>
        /// <returns></returns>
        public string showCurrentDir()
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("PWD");
            int index1 = strMsg.IndexOf("\"");
            int index2 = strMsg.IndexOf("\"", index1 + 1, strMsg.Length - index1 - 1);
            string currentPath = strMsg.Substring(index1 + 1, index2 - index1 - 1);
            return currentPath;
        }

        /// <summary>
        /// 发送命令改变远程FTP服务器目录的函数
        /// </summary>
        /// <param name="strDirName">新的远程FTP目录名字</param>
        public void chDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strMsg.Substring(4));
            }
            this.strRemotePath = strDirName;
        }

        public void SendCommand(string strCommand)
        {
            Byte[] cmdBytes = ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
            ControlSccket.Send(cmdBytes, cmdBytes.Length, 0);
            Get_iReplyCode();
        }

        public void DisConnect()
        {
            if (ControlSccket != null)
            {
                SendCommand("QUIT");// 首先退出登录
            }
            CloseSocketControl();
        }

        /// <summary>
        /// 关闭socket连接（用于登录之前）
        /// </summary>
        public void CloseSocketControl()
        {
            if (ControlSccket != null)// 若controlsocket还连接的话
            {
                ControlSccket.Close();
                ControlSccket = null;
            }
            bConnected = false;
        }

        /// <summary>
        /// 返回FTP服务器的应答信息
        /// </summary>
        /// <returns></returns>
        public string Get_strMsg()
        {
            while (true)
            {
                int recvBytes = ControlSccket.Receive(buffer, buffer.Length, 0);// 接受到的字节数
                strMsg += ASCII.GetString(buffer, 0, recvBytes);
                if (recvBytes < buffer.Length)
                {
                    break;
                }
            }
            string[] split_strMsg = strMsg.Split('\n');// 将接受到的FTP服务器应答信息通过 \n 分割，存储到一个字符串数组之中
            if (strMsg.Length > 2)
            {
                strMsg = split_strMsg[split_strMsg.Length - 2];
            }
            else
            {
                strMsg = split_strMsg[0];
            }
            if (!strMsg.Substring(3, 1).Equals(" "))
            {
                return Get_strMsg();
            }
            return strMsg; // 此时返回的是你操作FTP最新的应答信息
        }

        /// <summary>
        /// 从FTP服务器的应答信息中获得应答码
        /// </summary>
        /// <returns></returns>
        public int Get_iReplyCode()
        {
            strMsg = "";// 先将strMsg的内容清空
            strMsg = Get_strMsg(); // 再获得最新的strMsg
            iReplyCode = Int32.Parse(strMsg.Substring(0, 3));
            return iReplyCode;
        }

        #region 和FTP服务器上传，下载相关

        /// <summary>
        /// 创建一个用于数据连接的socket
        /// </summary>
        /// <returns></returns>
        public Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (iReplyCode != 227)
            {
                throw new IOException(strMsg.Substring(4));
            }
            int index1 = strMsg.IndexOf('(');
            int index2 = strMsg.IndexOf(')');
            string ipData = strMsg.Substring(index1 + 1, index2 - index1 - 1);
            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = "";
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                {
                    buf += ch;
                }
                else if (ch != ',')
                {
                    throw new IOException(strMsg);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch
                    {
                        throw new IOException(strMsg);
                    }
                }
            }

            string ipAdress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAdress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("数据socket不能连接到远程FTP服务器");
            }
            return s;
        }

        /// <summary>
        /// 获得远程FTP服务器指定路径下的文件列表
        /// </summary>
        /// <param name="strMask">远程FTP服务器的指定路径</param>
        /// <returns></returns>
        public string[] Dir(string strMask)
        {
            if (!bConnected)
            {
                Connect();
            }
            Socket DataSocket = CreateDataSocket();
            SendCommand("NLST " + strMask);

            // 2018-9-12：这里由于芯片上的FTP服务器发送列表数据和电脑上接收数据的速度相差实在是太大，所以这里让线程休息5ms
            Thread.Sleep(200);

            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226))
            {
                throw new IOException(strMsg.Substring(4));
            }
            strMsg = "";
            while (true)
            {
                int recvBytes = DataSocket.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, recvBytes);
                if (recvBytes < buffer.Length)
                {
                    break;
                }
            }
            string[] split_recvDataSocket = strMsg.Split('\n');
            DataSocket.Close();
            if (iReplyCode != 226)// 226:结束数据连接
            {
                Get_iReplyCode();
                if (iReplyCode != 226)
                {
                    throw new IOException(strMsg.Substring(4));
                }
            }
            return split_recvDataSocket;
        }

        /// <summary>
        /// 从FTP服务器获取指定文件的大小
        /// </summary>
        /// <param name="strFileName">指定的文件名</param>
        /// <returns></returns>
        public long GetFileSize(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + strFileName);
            long lSize = 0;
            if (iReplyCode == 213)
            {
                lSize = Int64.Parse(strMsg.Substring(4));
            }
            else
            {
                throw new IOException(strMsg.Substring(4));
            }
            return lSize;
        }

        /// <summary>
        /// 删除指定的FTP服务器上的文件
        /// </summary>
        public void Delete(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            if (iReplyCode != 250)
            {
                throw new Exception(strMsg.Substring(4));
            }
        }

        /// <summary>
        /// 删除指定FTP服务器上的目录
        /// </summary>
        /// <param name="strDirName">指定目录名</param>
        public void RmDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strMsg.Substring(4));
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="strOldFileName"></param>
        /// <param name="strNewFileName"></param>
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            if (iReplyCode != 350)
            {
                throw new IOException(strMsg.Substring(4));
            }
            //  如果新文件名与原有文件重名,将覆盖原有文件
            SendCommand("RNTO " + strNewFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strMsg.Substring(4));
            }
        }


        #endregion


        #region 传输模式

        /// <summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="ttType"></param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//Binary传输
            }
            else
            {
                SendCommand("TYPE A");// ASCII传输
            }
            if (iReplyCode != 200)
            {
                throw new IOException(strMsg.Substring(4));
            }
            else
            {
                trType = ttType;
            }
        }

        /// <summary>
        /// 获得传输模式
        /// </summary>
        /// <returns></returns>
        public TransferType GetTransferType()
        {
            return trType;
        }


        #endregion

        #region 上传和下载

        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的远程FTP服务器上的文件名</param>
        /// <param name="strFolder">本地目录（不可以以 \ 结束）</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        /// <param name="maxFileNum">要文件的大小</param>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool DownLoadSingleFile(string strRemoteFileName, string strFolder, string strLocalFileName, long maxFileNum, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!bConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);// 设置为二进制传输模式

            // 如果保存在本地文件名是 ""，那么就默认本地文件名就是远程文件名
            if (strLocalFileName.Equals(""))
            {
                strLocalFileName = strRemoteFileName;
            }

            // 如果保存的 本地文件名 不存在，那么直接创建一个
            if (!File.Exists(strRemoteFileName))
            {
                Stream st = File.Create(strLocalFileName);
                st.Close();
            }

            FileStream output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            
            Socket socketData = CreateDataSocket();// 创建一个用于数据连接的 socket

            string path = showCurrentDir();//2018-9-22

            SendCommand("RETR " + strRemoteFileName);
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226 || iReplyCode == 250))
            {
                //throw new IOException(strMsg.Substring(4));
                throw new IOException(strMsg);
            }
            int iBytes;// 接受的字节数
            long nowGetBytes = 0; // 目前获得的字节数
            bool usercancel = false; // 判断用户是否取消
            while (bPause == false)
            {
                iBytes = socketData.Receive(buffer, buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                nowGetBytes += (long)iBytes;
                // 进度
                int percentComplete = (int)(nowGetBytes / maxFileNum * 100);

                // 如果取消下载
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    usercancel = true;
                    break;
                }
                else
                {
                    worker.ReportProgress(percentComplete, nowGetBytes.ToString());
                }
                if (iBytes <= 0)// 接收到的iBytes小于等于0,要么出异常（<0）,要么就是下载完成(=0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                Get_iReplyCode();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    return false;
                    throw new IOException(strMsg.Substring(4));
                }
            }
            return !usercancel;
        }

        /// <summary>
        /// 下载一批文件
        /// </summary>
        /// <param name="strFileNameMask">文件名的匹配字符串</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        public void Get(string strFileNameMask, string strFolder, long maxFileNum, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!bConnected)
            {
                Connect();
            }
            string[] strFiles = Dir(strFileNameMask);
            int i = 1;
            foreach (string strFile in strFiles)
            {
                if (!strFile.Trim().Equals(""))//一般来说strFiles的最后一个元素可能是空字符串
                {
                    i++;
                    worker.ReportProgress(i / strFiles.Length * 100, 0);
                    GetMuiltyFile(strFile, strFolder, strFile.Trim(), maxFileNum, worker, e);
                }
            }
        }

        /// <summary>
        /// 下载一批文件用到的函数
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        public void GetMuiltyFile(string strRemoteFileName, string strFolder, string strLocalFileName, long maxFileNum, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!bConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);// 传输过程使用二进制传输
            if (strLocalFileName.Equals(""))
            {
                strLocalFileName = strRemoteFileName;
            }
            if (!File.Exists(strLocalFileName))// File.Exists:确定指定的文件是否存在
            {
                // ===========================================================关注一下C#中 流/Stream 的用法
                Stream st = File.Create(strLocalFileName);
                st.Close();
            }

            // FileStream为文件提供stream，既支持同步读写也支持异步读写
            FileStream output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);// RETR:从服务器上找回（复制）文件
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226 || iReplyCode == 250))
            {
                throw new IOException(strMsg.Substring(4));
            }
            long nowGetBytes = 0;
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                nowGetBytes += (long)iBytes;
                output.Write(buffer, 0, iBytes);// FileStream.Write:将字节块写入文件流
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                Get_iReplyCode();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strMsg.Substring(4));
                }
            }
        }

        #endregion
    }
}
