using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace DirectConnectionPredictControl.CommenTool
{
    public enum FileSource
    {
        /// <summary>
        /// 来源于PC端记录的日志文件
        /// </summary>
        PC,
        /// <summary>
        /// 来源于维护终端板子的日志文件
        /// </summary>
        TERMINAL
    }
    class Utils
    {
        public static string htmlPath;
        public static void getAbsoluteHtmlPath(string relativePath)

        {
            //string fileName = Path.GetFileName(relativePath);
            htmlPath = Path.GetFullPath(relativePath);
        }
        public static int timeInterval = 100; // 2020-5-20：修改每个页面的刷新时间
        public static string formatN1 = "{0:N1}";
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static short PositiveToNegative(byte high, byte low)
        {
            short res = 0;
            if ((high & 0x80) == 0x80)
            {
                res = (short)(high * 256 + low);
                res = (short)(res - 1);
                res = (short)-(~res);
            }
            else
            {
                res = (short)(high * 256 + low);
            }
            return res;
        }

        /// <summary>
        /// 读取xml中的列头数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="colunmName"></param>
        /// <returns></returns>
        public static IList<string> getXml(string fileName, string colunmName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            XmlNode root = document.SelectSingleNode(colunmName);
            XmlNodeList list = root.ChildNodes;
            IList<string> header = new List<string>();
            foreach (var item in list)
            {
                XmlNode node = (XmlNode)item;
                header.Add(node.InnerText);
            }
            return header;
        }

        public static void speedSensorErrorMain(ref MainDevDataContains mainDevDataContains, Byte[] recvData)
        {
            switch (recvData[2] & 0x0f)
            {
                case 0x00:
                    mainDevDataContains.SpeedSensorError_1 = MainDevDataContains.NORMAL;
                    break;
                case 0x01:
                    mainDevDataContains.SpeedSensorError_1 = MainDevDataContains.OPEN_CIRCUIT;
                    break;
                case 0x02:
                    mainDevDataContains.SpeedSensorError_1 = MainDevDataContains.SHORT_CIRCUIT;
                    break;
                case 0x04:
                    mainDevDataContains.SpeedSensorError_1 = MainDevDataContains.MUTATION;
                    break;
                case 0x08:
                    mainDevDataContains.SpeedSensorError_1 = MainDevDataContains.OVER_DIFF_VALUE;
                    break;
            }
            switch ((recvData[2] & 0xf0) >> 4)
            {
                case 0x00:
                    mainDevDataContains.SpeedSensorError_2 = MainDevDataContains.NORMAL;
                    break;
                case 0x01:
                    mainDevDataContains.SpeedSensorError_2 = MainDevDataContains.OPEN_CIRCUIT;
                    break;
                case 0x02:
                    mainDevDataContains.SpeedSensorError_2 = MainDevDataContains.SHORT_CIRCUIT;
                    break;
                case 0x04:
                    mainDevDataContains.SpeedSensorError_2 = MainDevDataContains.MUTATION;
                    break;
                case 0x08:
                    mainDevDataContains.SpeedSensorError_2 = MainDevDataContains.OVER_DIFF_VALUE;
                    break;
            }
        }
        public static void speedSensorErrorSliver(ref SliverDataContainer sliverDataContainer, Byte[] recvData)
        {
            switch (recvData[2] & 0x0f)
            {
                case 0x00:
                    sliverDataContainer.SpeedSensorError_1 = SliverDataContainer.NORMAL;
                    break;
                case 0x01:
                    sliverDataContainer.SpeedSensorError_1 = SliverDataContainer.OPEN_CIRCUIT;
                    break;
                case 0x02:
                    sliverDataContainer.SpeedSensorError_1 = SliverDataContainer.SHORT_CIRCUIT;
                    break;
                case 0x04:
                    sliverDataContainer.SpeedSensorError_1 = SliverDataContainer.MUTATION;
                    break;
                case 0x08:
                    sliverDataContainer.SpeedSensorError_1 = SliverDataContainer.OVER_DIFF_VALUE;
                    break;
            }
            switch ((recvData[2] & 0xf0) >> 4)
            {
                case 0x00:
                    sliverDataContainer.SpeedSensorError_2 = MainDevDataContains.NORMAL;
                    break;
                case 0x01:
                    sliverDataContainer.SpeedSensorError_2 = MainDevDataContains.OPEN_CIRCUIT;
                    break;
                case 0x02:
                    sliverDataContainer.SpeedSensorError_2 = MainDevDataContains.SHORT_CIRCUIT;
                    break;
                case 0x04:
                    sliverDataContainer.SpeedSensorError_2 = MainDevDataContains.MUTATION;
                    break;
                case 0x08:
                    sliverDataContainer.SpeedSensorError_2 = MainDevDataContains.OVER_DIFF_VALUE;
                    break;
            }
        }

        public static uint CRC_GEN(byte[] data, int len)
        {
            uint m_CRC_result = 0xffff;
            len = 8;
            m_CRC_result = crc_gen_cal_by_byte(0xffff, data, len);
            return m_CRC_result;
        }

        private static uint crc_gen_cal_by_byte(uint crc_reg_init, byte[] data, int len)
        {
            uint crc = crc_reg_init;
            int i = 0;
            while (len-- != 0)
            {
                uint high = (uint)(crc / 256);
                crc <<= 8;
                crc ^= crc_gen_ta_8[high ^ data[i++]];
            }
            return crc;
        }

        private static uint[] crc_gen_ta_8 ={
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
            0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
            0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
            0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
            0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
            0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
            0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
            0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
            0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
            0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
            0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
            0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
            0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
            0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
            0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
            0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
            0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
            0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
            0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
        };

    }
    
}
