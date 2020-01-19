using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DirectConnectionPredictControl.CommenTool;

namespace DirectConnectionPredictControl.IO
{
    class FileBuilding
    {
        private string filePath;
        private const string recordName = "record.log";
        private int len;
        private static int LINE_LENGTH = 620;
        public static int CAN_MO_NUM = 66;
        private static Dictionary<int, uint> canIDMap = new Dictionary<int, uint>();
        private static long fileLength;
        private static long MAX_FILE_SIZE = 8 * 1024 * 1024;

        public static long FileLength { get => fileLength; set => fileLength = value; }
        public static int LINE_LENGTH1 { get => LINE_LENGTH; set => LINE_LENGTH = value; }

        static FileBuilding()
        {
            LINE_LENGTH1 = 620;
            CAN_MO_NUM = 66;
            InitID();
        }

        public FileBuilding()
        {
            
        }
        

        public FileBuilding(string filePath)
        {
            this.filePath = filePath;
        }

        public static List<byte[]> GetFileContent(string path)
        {
            List<byte[]> list = new List<byte[]>();
            //FileStream stream = new FileStream(path, FileMode.Open);
            System.IO.FileStream stream = new System.IO.FileStream(path,System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            long lengthToRead = stream.Length;
            fileLength = lengthToRead;
            int num = 0;
            int start = 0;
            while (lengthToRead > 0)
            {
                byte[] buf = new byte[LINE_LENGTH1];
                stream.Position = start;
                if (lengthToRead < LINE_LENGTH1)
                {
                    num = stream.Read(buf, 0, Convert.ToInt32(lengthToRead));
                }
                else
                {
                    num = stream.Read(buf, 0, LINE_LENGTH1);
                }
                list.Add(buf);
                if (num == 0)
                {
                    break;
                }
                start += num;
                lengthToRead -= num;
            }
            stream.Close();
            return list;
        }

        public static List<List<CanDTO>> GetCanList(List<byte[]> bytes, FileSource source)
        {
            List<List<CanDTO>> canList = new List<List<CanDTO>>();
            for (int i = 0; i < bytes.Count; i++)
            {
                List<CanDTO> tempList = new List<CanDTO>();
                //时间解析
                DateTime dateTime;
                string year = Encoding.ASCII.GetString(bytes[i].Skip(0).Take(4).ToArray());
                string month = Encoding.ASCII.GetString(bytes[i].Skip(5).Take(2).ToArray());
                string day = Encoding.ASCII.GetString(bytes[i].Skip(8).Take(2).ToArray());
                string hour = Encoding.ASCII.GetString(bytes[i].Skip(11).Take(2).ToArray());
                string minute = Encoding.ASCII.GetString(bytes[i].Skip(14).Take(2).ToArray());
                string second = Encoding.ASCII.GetString(bytes[i].Skip(17).Take(2).ToArray());
                if (source == FileSource.PC)
                {
                    string millisecond = Encoding.ASCII.GetString(bytes[i].Skip(20).Take(3).ToArray());
                    dateTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), int.Parse(millisecond));
                }
                else
                {
                    dateTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second));
                }
                
                
                //数据解析
                for (int j = 0; j < CAN_MO_NUM; j++)
                {
                    CanDTO can = new CanDTO();
                    
                    uint id = canIDMap[j];
                    byte[] data;
                    if (source == FileSource.PC)
                    {
                        data = bytes[i].Skip(24 + j * 9).Take(8).ToArray();
                    }
                    else
                    {
                        data = bytes[i].Skip(20 + j * 9).Take(8).ToArray();
                    }
                   
                    can.Id = id;
                    can.Time = dateTime;
                    can.Data = data;
                    tempList.Add(can);
                }
                canList.Add(tempList);
            }
            return canList;
        }

        private static void InitID()
        {
            int index = 0;
            canIDMap.Add(index++, 0x2000000);   //0x10
            canIDMap.Add(index++, 0x2200000);   //0x11 第一列
            canIDMap.Add(index++, 0x2400000);   //0x12
            canIDMap.Add(index++, 0x2600000);   //0x13
            canIDMap.Add(index++, 0x2800000);   //0x14
            canIDMap.Add(index++, 0x2a00000);   //0x15
            canIDMap.Add(index++, 0x2c00000);   //0x16
            canIDMap.Add(index++, 0x2e00000);   //0x17
            canIDMap.Add(index++, 0x3000000);   //0x18

            canIDMap.Add(index++, 0x4000000);   //0x20
            canIDMap.Add(index++, 0x4200000);   //0x21 =
            canIDMap.Add(index++, 0x4400000);   //0x22
            canIDMap.Add(index++, 0x4600000);   //0x23
            canIDMap.Add(index++, 0x4800000);   //0x24
            canIDMap.Add(index++, 0x4a00000);   //0x25
            canIDMap.Add(index++, 0x4c00000);   //0x26
            canIDMap.Add(index++, 0x4e00000);   //0x27
            canIDMap.Add(index++, 0x5000000);   //0x28

            canIDMap.Add(index++, 0x6200000);   //0x31=
            canIDMap.Add(index++, 0x6800000);   //0x34
            canIDMap.Add(index++, 0x6a00000);   //0x35
            canIDMap.Add(index++, 0x6c00000);   //0x36
            canIDMap.Add(index++, 0x6e00000);   //0x37
            canIDMap.Add(index++, 0x7000000);   //0x38

            canIDMap.Add(index++, 0x8200000);   //0x41=
            canIDMap.Add(index++, 0x8800000);   //0x44
            canIDMap.Add(index++, 0x8a00000);   //0x45
            canIDMap.Add(index++, 0x8c00000);   //0x46
            canIDMap.Add(index++, 0x8e00000);   //0x47
            canIDMap.Add(index++, 0x9000000);   //0x48

            canIDMap.Add(index++, 0xa200000);   //0x51=
            canIDMap.Add(index++, 0xa800000);   //0x54
            canIDMap.Add(index++, 0xaa00000);   //0x55
            canIDMap.Add(index++, 0xac00000);   //0x56
            canIDMap.Add(index++, 0xae00000);   //0x57
            canIDMap.Add(index++, 0xb000000);   //0x58

            canIDMap.Add(index++, 0xc200000);   //0x61=
            canIDMap.Add(index++, 0xc800000);   //0x64
            canIDMap.Add(index++, 0xca00000);   //0x65
            canIDMap.Add(index++, 0xcc00000);   //0x66
            canIDMap.Add(index++, 0xce00000);   //0x67
            canIDMap.Add(index++, 0xd000000);   //0x68

            canIDMap.Add(index++, 0xe200000);   //0x71
            canIDMap.Add(index++, 0xe400000);   //0x72
            canIDMap.Add(index++, 0xe600000);   //0x73
            canIDMap.Add(index++, 0xe800000);   //0x74
            canIDMap.Add(index++, 0xea00000);   //0x75
            canIDMap.Add(index++, 0xec00000);   //0x76
            canIDMap.Add(index++, 0xee00000);   //0x77
            canIDMap.Add(index++, 0xf000000);   //0x78
            canIDMap.Add(index++, 0xf200000);   //0x79

            canIDMap.Add(index++, 0x10200000);   //0x81
            canIDMap.Add(index++, 0x10400000);   //0x82
            canIDMap.Add(index++, 0x10600000);   //0x83
            canIDMap.Add(index++, 0x10800000);   //0x84
            canIDMap.Add(index++, 0x10a00000);   //0x85
            canIDMap.Add(index++, 0x10c00000);   //0x86
            canIDMap.Add(index++, 0x10e00000);   //0x87
            canIDMap.Add(index++, 0x11000000);   //0x88
            canIDMap.Add(index++, 0x11200000);   //0x89

            canIDMap.Add(index++, 0x14200000);   //0xa1
            canIDMap.Add(index++, 0x14400000);   //0xa2
            canIDMap.Add(index++, 0x14600000);   //0xa3
            canIDMap.Add(index++, 0x14800000);   //0xa4
            canIDMap.Add(index++, 0x14a00000);   //0xa5
            canIDMap.Add(index++, 0x14c00000);   //0xa6
        }

        /// <summary>
        /// 将字节数组写入到指定文件中
        /// </summary>
        /// <param name="data">需要存入的字节数组</param>
        public void WriteFile(byte[] data)
        {
            if (File.Exists(recordName))
            {
                long fileSize = new System.IO.FileInfo(recordName).Length;
                if (fileSize > MAX_FILE_SIZE)
                {
                    string newName = DateTime.Now.ToString("yyyy年MM月dd日 HH-mm-ss") + ".log";
                    File.Copy(recordName, newName);
                    File.Delete(recordName);
                }
            }
            
            BinaryWriter bw = new BinaryWriter(new FileStream(recordName, FileMode.Append, FileAccess.Write));
            try
            {
                
                bw.Write(data);
                bw.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">字符串："8 8 8 8 8"</param>
        public void Record(byte[][] msg)
        {
            DateTime time = DateTime.Now;
            byte[] content = new byte[0];
            for (int i = 0; i < msg.Length; i++)
            {
                content = content.Concat(msg[i]).ToArray();
            }
            string now = time.ToString("yyyy-MM-dd HH:mm:ss.fff") + " ";
            byte[] nowByte = Encoding.ASCII.GetBytes(now);
            byte[] end = Encoding.ASCII.GetBytes("\r\n");
            byte[] final = nowByte.Concat(content).Concat(end).ToArray();
            WriteFile(final);
        }
    }
}
