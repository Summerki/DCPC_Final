using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    class CanHelper
    {
        //结构体API
        //1.ZLGCAN系列接口卡信息的数据类型。
        public struct VCI_BOARD_INFO   //结构体，包含CAN系列接口卡的设备信息，结构体将在VCI_ReadBoardInfo函数中被填充。
        {
            public UInt16 hw_Version;    //硬件版本号，16进制
            public UInt16 fw_Version;    //固件版本号
            public UInt16 dr_Version;    //驱动程序版本号
            public UInt16 in_Version;    //接口库版本号
            public UInt16 irq_Num;       //板卡中所使用的中断号
            public byte can_Num;         //表示由几路CAN通道
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] //在托管代码和非托管代码封送数据
            public byte[] str_Serial_Num; //此板卡的序列号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] str_hw_Type;  //硬件类型
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Reserved;     //系统保留
        }


        //2.定义CAN信息帧的数据类型。在VCI_Transmit和VCI_Receive函数中被用来传送CAN信息帧。
        unsafe public struct VCI_CAN_OBJ  //使用不安全代码或非托管代码，使用指针变量
        {
            public uint ID;        //报文ID
            public uint TimeStamp; //接收到信息帧时的时间标识，从CAN控制器初始化开始计时
            public byte TimeFlag;  //是否使用时间标识，为1时TimeStamp有效，这两个只在此帧为接收帧时才有意义
            public byte SendType;  //发送帧类型，=0时为正常发送，=1时为单次发送，=2时为自发自收，=3时为单次自发自收，只有为发送帧时才有意义
            public byte RemoteFlag;//是否是远程帧
            public byte ExternFlag;//是否是扩展帧
            public byte DataLen;   //数据长度<=8，Data的长度

            public fixed byte Data[8];//报文的数据，是数组

            public fixed byte Reserved[3];//系统保留

        }


        //3.定义CAN控制器状态的数据类型。
        public struct VCI_CAN_STATUS //CAN控制器状态信息，结构体将在VCI_ReadCanStatus函数中被填充
        {
            public byte ErrInterrupt;//中断记录，读操作会清除
            public byte regMode;     //CAN控制器模式寄存器
            public byte regStatus;   //CAN控制器状态寄存器
            public byte regALCapture;//CAN控制器仲裁丢失寄存器
            public byte regECCapture;//CAN控制器错误寄存器
            public byte regEWLimit; //CAN控制器错误警告限制寄存器
            public byte regRECounter;//CAN控制器接收错误寄存器
            public byte regTECounter;//CAN控制器发送错误寄存器
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Reserved;
        }

        //4.定义错误信息的数据类型。
        public struct VCI_ERR_INFO //用于装载VCI库运行时产生的错误信息。结构体将在VCI_ReadErrInfo函数中被填充
        {
            public UInt32 ErrCode; // 错误码
            public byte Passive_ErrData1; //当产生的错误中有消极错误时表示为消极错误的错误标识数据
            public byte Passive_ErrData2; //三个分别为错误代码捕捉位功能表示，接受错误计数器和发送错误计数器
            public byte Passive_ErrData3;
            public byte ArLost_ErrData; //当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据
        }

        //5.定义初始化CAN的数据类型
        public struct VCI_INIT_CONFIG //定义了初始化CAN的配置，结构体将在VCI_InitCan函数中被填充
        {
            public UInt32 AccCode; //验收码
            public UInt32 AccMask; //屏蔽码
            public UInt32 Reserved;//保留
            public byte Filter;    //滤波方式
            public byte Timing0;   //定时器0 （BTR0）
            public byte Timing1;   //定时器1 （BTR1），这两个定时器用来设置CAN波特率。
            public byte Mode;     //模式
        }


        [DllImport("controlcan.dll")]  //调用名为controlcan的DLL文件，调用过来之后变成静态的，这些函数的返回值都是1表示成功，0表示失败。
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        //VCI_openDevice是用来打开设备的函数。DeviceType为设备类型号，USBCAN为3和4.DeviceIndex为设备索引号，当只有一个USBCAN时，索引号为0，有两个时可以为0或1.Reserved为系统保留，无意义。
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        //此函数用来关闭设备，参数为设备类型号和设备索引号。
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        //此函数用来初始化指定的CAN，参数为设备类型号和索引号。CANindex为第几路CAN，pInitConfig为初始化参数结构，有验收码，屏蔽码，滤波方式，定时器和模式选择的信息。
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        //此函数用来获取设备信息，参数为设备类型号和索引号，pInfo为用来存储设备信息的VCI_BOARD_INFO结构指针
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        //此函数用来获取最后一次错误信息，参数为设备类型号，索引号，第几路CAN和用来存储错误信息的VCI_ERR_INFO结构指针
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);
        //此函数用来获取CAN状态，参数为设备类型号，索引号，第几路CAN和用来存储CAN状态的VCI_CAN_STATUS结构指针
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        //此函数用来获取设备的相应参数，参数为设备类型号，索引号，第几路CAN，参数类型和用来存储参数有关数据缓冲区地址首指针pData，RefType的值一般为1
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        //此函数用来设置设备的相应参数，主要处理不同设备的特定操作，参数为设备类型号，索引号，第几路CAN，参数类型和用来存储参数有关数据缓冲区地址首指针，RefType的值可以为1或2
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        //此函数用来获取指定接收缓冲区中接收到但尚未被读取的帧数，返回尚未被读取的帧数，参数为设备类型号，索引号和第几路CAN
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        //此函数用以清空指定缓冲区，参数为设备类型号，索引号和第几路CAN
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        //此函数用以启动CAN，参数为设备类型号，索引号和第几路CAN
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        //此函数用以复位CAN，参数为设备类型号，索引号和第几路CAN
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);
        //此函数用以返回实际发送的帧数。参数为设备类型号，索引号，第几路CAN，要发送的数据帧数组的首指针和要发送的数据帧数组的长度，返回值为返回实际发送的帧数

        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        //此函数从指定的设备读取数据，参数为设备类型号，索引号，第几路CAN，用来接收的数据帧数组的首指针，用来接收的数据帧数组的长度和等待超时时间，以毫秒为单位
        //返回值为实际读取到的帧数。如果返回值为0xFFFFFFFF,则表示读取数据失败，有错误发生，请调用VCI_ReadErrInfo函数来获取错误码

        private VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[50];
        private UInt32[] m_arrdevtype = new UInt32[20];
        private UInt16 CANInd = 0;   // 第几路CAN
        private static UInt32 con_maxlen = 500;//为接收到的报文的最大条数
        private IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);  //为接收数据分配空间
        private VCI_CAN_OBJ obj = new VCI_CAN_OBJ();
        private VCI_INIT_CONFIG config;
        private VCI_INIT_CONFIG config1;
        private bool isOpen = false;
        private byte boundFirst = 0x01;
        private byte boundSecond = 0x1c;

        public enum DeviceState
        {
            Success,
            Fail,
            IsOpen,
            IsClose
        }

        /// <summary>
        /// 发送一个CAN数据包
        /// </summary>
        /// <param name="data">设备状态</param>
        /// <returns></returns>
        unsafe public DeviceState Send(byte[] data)
        {
            VCI_ClearBuffer(4, 0, CANInd);
            UInt32 con_maxlen = 500;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            VCI_CAN_OBJ sendObj = new VCI_CAN_OBJ
            {
                SendType = 0,
                RemoteFlag = 0,
                ExternFlag = 0,
                ID = 0x51,
                DataLen = 8
            };
            for (int i = 0; i < data.Length; i++)
            {
                sendObj.Data[i] = data[i];
            }
            if (VCI_Transmit(4, 0, CANInd, ref sendObj, 1) == 0)
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }
        #region 何虎，2018-9-24
        /// <summary>
        /// 发送一个CAN数据包
        /// </summary>
        /// <param name="data">设备状态</param>
        /// <returns></returns>
        unsafe public DeviceState SendByID(byte[] data, uint id)
        {
            //if (id != 0x11)
            //{
            //    uint crc = Utils.CRC_GEN(data, 8);
            //    id = id << 21;
            //    id = id | crc;
            //}
            VCI_ClearBuffer(4, 0, CANInd);
            UInt32 con_maxlen = 500;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            VCI_CAN_OBJ sendObj = new VCI_CAN_OBJ
            {
                SendType = 0,
                RemoteFlag = 0,
                ExternFlag = 1, //2018-10-12:将其等于1 意思是改为扩展帧的格式 记录一下
                ID = id,
                DataLen = 8
            };
            for (int i = 0; i < data.Length; i++)
            {
                sendObj.Data[i] = data[i];
            }
            if (VCI_Transmit(4, 0, CANInd, ref sendObj, 1) == 0)
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }
        #endregion
        /// <summary>
        /// can接收数据
        /// </summary>
        /// <returns></returns>
        unsafe public List<CanDTO> Recv()
        {
            UInt32 res = VCI_Receive(4, 0, CANInd, pt, con_maxlen, 100);
            VCI_ClearBuffer(4, 0, CANInd);
            List<CanDTO> list = new List<CanDTO>();
            for (UInt32 i = 0; i < res; i++)
            {
                CanDTO dTO = new CanDTO();
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                //dTO.Time = (int)obj.TimeStamp;
                dTO.Id = obj.ID;
                for (int index = 0; index < 8; index++)
                {
                    dTO.Data[index] = obj.Data[index];
                }
                list.Add(dTO);
            }
            return list;
        }

        public CanHelper()
        {

        }

        /// <summary>
        /// 开启CAN设备
        /// </summary>
        /// <returns>设备状态码</returns>
        public DeviceState Open()
        {   
            if (isOpen == false)
            {
                VCI_ClearBuffer(4, 0, CANInd);
                VCI_CAN_OBJ[] CANObj = new VCI_CAN_OBJ[100];
                if (VCI_OpenDevice((UInt32)4, (UInt32)0, CANInd) == 0)
                {
                    return DeviceState.Fail;
                }
                isOpen = true;
                config = new VCI_INIT_CONFIG
                {
                    AccCode = 0x00000000,
                    AccMask = 0xffffffff,
                    Reserved = 0,
                    Timing0 = boundFirst,
                    Timing1 = boundSecond,
                    Filter = 1,
                    Mode = 0
                };
                return DeviceState.Success;
            }
            else
            {
                return DeviceState.IsOpen;
            }
        }

        /// <summary>
        /// 初始化CAN设备
        /// </summary>
        /// <returns>设备状态码</returns>
        public DeviceState Init()
        {
            if (VCI_InitCAN(4, 0, CANInd, ref config) == 0)         //初始化CAN
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }

        /// <summary>
        /// 开启CAN设备
        /// </summary>
        /// <returns>设别状态码</returns>
        public DeviceState Start()
        {
            if (VCI_StartCAN(4, 0, CANInd) == 0)
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }

        /// <summary>
        /// 关闭CAN设备
        /// </summary>
        /// <returns>设备状态码</returns>
        public DeviceState Close()
        {
            if (isOpen == false)
            {
                return DeviceState.IsClose;
            }
            else
            {
                if (VCI_CloseDevice(4, 0) == 0)
                {
                    isOpen = false;
                    return DeviceState.Success;
                }
                else
                {
                    return DeviceState.Fail;
                }
                
                
            }
        }

        #region 开启CAN 1通道
        private UInt16 CANInd_1 = 1;   // 第1路CAN
        /// <summary>
        /// 开启CAN设备
        /// </summary>
        /// <returns>设备状态码</returns>
        public DeviceState OpenCAN_1()
        {
            if (isOpen == false)
            {
                VCI_ClearBuffer(4, 0, CANInd_1);
                VCI_CAN_OBJ[] CANObj = new VCI_CAN_OBJ[100];
                if (VCI_OpenDevice((UInt32)4, (UInt32)0, CANInd_1) == 0)
                {
                    return DeviceState.Fail;
                }
                isOpen = true;
                config1 = new VCI_INIT_CONFIG
                {
                    AccCode = 0x00000000,
                    AccMask = 0xffffffff,
                    Reserved = 0,
                    Timing0 = boundFirst,
                    Timing1 = boundSecond,
                    Filter = 1,
                    Mode = 0
                };
                return DeviceState.Success;
            }
            else
            {
                return DeviceState.IsOpen;
            }
        }

        /// <summary>
        /// 初始化CAN设备
        /// </summary>
        /// <returns>设备状态码</returns>
        public DeviceState InitCAN_1()
        {
            if (VCI_InitCAN(4, 0, CANInd_1, ref config1) == 0)         //初始化CAN
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }

        /// <summary>
        /// 开启CAN设备
        /// </summary>
        /// <returns>设别状态码</returns>
        public DeviceState StartCAN_1()
        {
            if (VCI_StartCAN(4, 0, CANInd_1) == 0)
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }

        /// <summary>
        /// 发送一个CAN数据包
        /// </summary>
        /// <param name="data">设备状态</param>
        /// <returns></returns>
        unsafe public DeviceState SendByIDCAN_1(byte[] data, uint id)
        {
            //if (id != 0x11)
            //{
            //    uint crc = Utils.CRC_GEN(data, 8);
            //    id = id << 21;
            //    id = id | crc;
            //}
            VCI_ClearBuffer(4, 0, CANInd_1);
            UInt32 con_maxlen = 500;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            VCI_CAN_OBJ sendObj = new VCI_CAN_OBJ
            {
                SendType = 0,
                RemoteFlag = 0,
                ExternFlag = 0, //2018-10-12:将其等于1 意思是改为扩展帧的格式 记录一下
                ID = id,
                DataLen = 8
            };
            for (int i = 0; i < data.Length; i++)
            {
                sendObj.Data[i] = data[i];
            }
            if (VCI_Transmit(4, 0, CANInd_1, ref sendObj, 1) == 0)
            {
                return DeviceState.Fail;
            }
            else
            {
                return DeviceState.Success;
            }
        }
        #endregion
    }
}
