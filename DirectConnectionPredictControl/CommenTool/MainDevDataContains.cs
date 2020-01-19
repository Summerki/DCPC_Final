using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class MainDevDataContains
    {
        public static string BCU_HIGH_FAIL = "BCU严重故障";
        public static string BCU_MIDDLE_FAIL = "BCU中等故障";
        public static string BCU_LOW_FAIL = "BCU轻微故障";
        public static string EMERGENCY_FAIL = "紧急制动不可用";
        public static string NORMAL_MODE = "正常运行模式";
        public static string EMERGENCY_DRIVE_MODE = "紧急牵引模式";
        public static string CALLBACK_MODE = "回送模式";
        public static string DEFAULT_MODE = "";

        //速度传感器的故障分类
        public static string NORMAL = "正常";
        public static string OPEN_CIRCUIT = "断路";
        public static string SHORT_CIRCUIT = "短路";
        public static string MUTATION = "突变";
        public static string OVER_DIFF_VALUE = "差值过大";


        /// <summary>
        /// 生命信号
        /// </summary>
        private int lifeSig;

        /// <summary>
        /// 运行模式
        /// </summary>
        private string mode = DEFAULT_MODE;

        /// <summary>
        /// 牵引命令
        /// </summary>
        private bool driveCmd;

        /// <summary>
        /// 制动命令
        /// </summary>
        private bool brakeCmd;

        /// <summary>
        /// 惰行命令
        /// </summary>
        private bool lazyCmd;

        /// <summary>
        /// 快速制动命令
        /// </summary>
        private bool fastBrakeCmd;

        /// <summary>
        /// 运输模式
        /// </summary>
        private bool towingMode;

        /// <summary>
        /// 保持制动缓解
        /// </summary>
        private bool holdBrakeRealease;

        /// <summary>
        /// 紧急制动命令
        /// </summary>
        private bool emergencyBrakeCmd;

        /// <summary>
        /// 紧急牵引命令
        /// </summary>
        private bool emergencyDriveCmd;

        /// <summary>
        /// ATO模式
        /// </summary>
        private bool ATOMode;

        /// <summary>
        /// 列车减速度设定值有效
        /// </summary>
        private bool accSetupEnable;

        /// <summary>
        /// 自检开启
        /// </summary>
        private bool selfTestEnable;

        /// <summary>
        /// 电制动淡出
        /// </summary>
        private bool edFadeOut;

        /// <summary>
        /// 整车制动力有效
        /// </summary>
        private bool trainBrakeEnable;

        /// <summary>
        /// 列车减速度设定值
        /// </summary>
        private int accSetupValue;

        /// <summary>
        /// 整车制动力需求
        /// </summary>
        private int brakeForceReq;

        /// <summary>
        /// 2车电制动实际值
        /// </summary>
        private int edRealValue2;

        /// <summary>
        /// 3车电制动实际值
        /// </summary>
        private int edRealValue3;

        /// <summary>
        /// 2车电制动能力值
        /// </summary>
        private int edCapValue2;

        /// <summary>
        /// 3车电制动能力值
        /// </summary>
        private int edCapValue3;

        /// <summary>
        /// 2车电制动可用
        /// </summary>
        private bool edEnable2;

        /// <summary>
        /// 2车电制动有效
        /// </summary>
        private bool edActive2;

        /// <summary>
        /// 2车电制动滑行
        /// </summary>
        private bool edSlip2;

        /// <summary>
        /// 3车电制动可用
        /// </summary>
        private bool edEnable3;

        /// <summary>
        /// 3车电制动有效
        /// </summary>
        private bool edActive3;

        /// <summary>
        /// 3车电制动滑行
        /// </summary>
        private bool edSlip3;

        /// <summary>
        /// A1车BCP过低
        /// </summary>
        private bool BCPLowA1;

        /// <summary>
        /// 紧急制动异常
        /// </summary>
        private bool emergencyBrakeException;

        /// <summary>
        /// CAN内的停放制动全部缓解
        /// </summary>
        private bool canParkBrakeRealease;

        /// <summary>
        /// 速度检测故障
        /// </summary>
        private bool speedDetection;

        /// <summary>
        /// CAN1总线故障
        /// </summary>
        private bool canBusFail1;

        /// <summary>
        /// CAN2总线故障
        /// </summary>
        private bool canBusFail2;

        /// <summary>
        /// 大事件
        /// </summary>
        private bool eventHigh;

        /// <summary>
        /// 中等事件
        /// </summary>
        private bool eventMid;

        /// <summary>
        /// 小事件
        /// </summary>
        private bool eventLow;

        /// <summary>
        /// CAN内的全部ASP值有效
        /// </summary>
        private bool canASPEnable;

        /// <summary>
        /// A1车架1滑行
        /// </summary>
        private bool slipA1;

        /// <summary>
        /// A1车架1紧急制动施加
        /// </summary>
        private bool emergencyBrakeActiveA1;

        /// <summary>
        /// A1车架1制动已缓解
        /// </summary>
        private bool brakeRealeaseA1;

        /// <summary>
        /// A1车架1制动风缸压力低
        /// </summary>
        private bool BSRLowA1;

        /// <summary>
        /// A1车架1气制动状态
        /// </summary>
        private bool abStatuesA1;

        /// <summary>
        /// A1车架1载荷信号有效
        /// </summary>
        private bool massSigValid;

        /// <summary>
        /// A1车架1载荷
        /// </summary>
        private int massA1;

        /// <summary>
        /// 85km/h超速
        /// </summary>
        private bool overSpeed85;

        /// <summary>
        /// 非零速
        /// </summary>
        private bool notZeroSpeed;

        /// <summary>
        /// CAN单元中单个阀紧急激活
        /// </summary>
        private bool valveCanEmergencyActive;

        /// <summary>
        /// 硬线快速制动指令
        /// </summary>
        private bool hardFastBrakeCmd;

        /// <summary>
        /// 硬线制动指令
        /// </summary>
        private bool hardBrakeCmd;

        /// <summary>
        /// 硬线牵引指令
        /// </summary>
        private bool hardDriveCmd;

        /// <summary>
        /// 硬线紧急牵引模式指令
        /// </summary>
        private bool hardEmergencyDriveMode;

        /// <summary>
        /// CAN ID
        /// </summary>
        private bool canID;

        /// <summary>
        /// A1车架1状态和诊断数据有效
        /// </summary>
        private bool dataValidA1Car1;

        /// <summary>
        /// A1车架2状态和诊断数据有效
        /// </summary>
        private bool dataValidA1Car2;

        /// <summary>
        /// B1车架1状态和诊断数据有效
        /// </summary>
        private bool dataValidB1Car1;

        /// <summary>
        /// B1车架2状态和诊断数据有效
        /// </summary>
        private bool dataValidB1Car2;

        /// <summary>
        /// C1车架1状态和诊断数据有效
        /// </summary>
        private bool dataValidC1Car1;

        /// <summary>
        /// C1车架2状态和诊断数据有效
        /// </summary>
        private bool dataValidC1Car2;

        /// <summary>
        /// A1车1轴速度
        /// </summary>
        private double speedA1Shaft1;

        /// <summary>
        /// A1车2轴速度
        /// </summary>
        private double speedA1Shaft2;

        /// <summary>
        /// A1车架1气制动能力
        /// </summary>
        private int epCapA1Car1;

        /// <summary>
        /// A1车架1制动风缸压力
        /// </summary>
        private int BSPPressureA1Car1;

        /// <summary>
        /// A1车架1制动缸1压力
        /// </summary>
        private int BRKCylinder1PressureA1;

        /// <summary>
        /// A1车架1制动缸2压力
        /// </summary>
        private int BRKCylinder2PressureA1;

        /// <summary>
        /// A1车停放制动缸压力
        /// </summary>
        private int parkPressureA1;

        /// <summary>
        /// A1车架1空簧1压力
        /// </summary>
        private int airSpring1PressureA1Car1;

        /// <summary>
        /// A1车架1空簧2压力
        /// </summary>
        private int airSpring2PressureA1Car1;

        /// <summary>
        /// A1车架1制动风缸压力有效
        /// </summary>
        private bool BSRPressureEnableA1Car1;

        /// <summary>
        /// A1车停放制动缸压力有效
        /// </summary>
        private bool parkBrakePressureEnableA1;

        /// <summary>
        /// A1车架1制动缸1压力有效
        /// </summary>
        private bool BRKCylinder1PressureEnableA1Car1;

        /// <summary>
        /// A1车架1制动缸2压力有效
        /// </summary>
        private bool BRKCylinder2PressureEnableA1Car1;

        /// <summary>
        /// A1车架1空簧1压力有效
        /// </summary>
        private bool airSpring1PressureEnableA1Car1;

        /// <summary>
        /// A1车架1空簧2压力有效
        /// </summary>
        private bool airSpring2PressureEnableA1Car1;

        /// <summary>
        /// B1车主风管压力有效
        /// </summary>
        private bool MREPreessureEnableB1;

        /// <summary>
        /// B1车电制动切除
        /// </summary>
        private bool edOffB1;

        /// <summary>
        /// C1车电制动切除
        /// </summary>
        private bool edOffC1;

        /// <summary>
        /// 自检中断
        /// </summary>
        private bool selfTestInt;

        /// <summary>
        /// 自检已激活
        /// </summary>
        private bool selfTestActive;

        /// <summary>
        /// 自检成功
        /// </summary>
        private bool selfTestSuccess;

        /// <summary>
        /// 自检失败
        /// </summary>
        private bool selfTestFail;

        /// <summary>
        /// 24小时未自检
        /// </summary>
        private bool unSelfTest24;

        /// <summary>
        /// 26小时未自检
        /// </summary>
        private bool unSelfTest26;

        /// <summary>
        /// 网关阀状态
        /// </summary>
        private bool gateValveState;

        /// <summary>
        /// A1车1轴速度有效
        /// </summary>
        private bool speedShaftEnable1;

        /// <summary>
        /// A1车2轴速度有效
        /// </summary>
        private bool speedShaftEnable2;

        /// <summary>
        /// 1架空气制动目标值
        /// </summary>
        public int abTargetValueAx1;

        /// <summary>
        /// 2架空气制动目标值
        /// </summary>
        public int abTargetValueAx2;

        /// <summary>
        /// 3架空气制动目标值
        /// </summary>
        public int abTargetValueAx3;

        /// <summary>
        /// 4架空气制动目标值
        /// </summary>
        public int abTargetValueAx4;

        /// <summary>
        /// 5架空气制动目标值
        /// </summary>
        public int abTargetValueAx5;

        /// <summary>
        /// 6架空气制动目标值
        /// </summary>
        public int abTargetValueAx6;

        /// <summary>
        /// 1架VLD压力实际值
        /// </summary>
        private int vldRealPressureAx1;

        /// <summary>
        /// 1架BCP1压力
        /// </summary>
        private int bcp1PressureAx1;

        /// <summary>
        /// 1架BCP2压力
        /// </summary>
        private int bcp2PressureAx2;

        /// <summary>
        /// 1架VLD压力设定值
        /// </summary>
        private int vldPressureSetupAx1;

        /// <summary>
        /// C车主风管压力
        /// </summary>
        private int mbpPressure;

        /// <summary>
        /// 制动风缸压力
        /// </summary>
        private int brakeCylinderSourcePressure;





        private int[] dcuLifeSig = new int[4] { 0, 0, 0, 0 };

        private bool[] dcuEbOK = new bool[4] { false, false, false, false };

        private bool[] dcuEbFadeout = new bool[4] { false, false, false, false };

        private bool[] dcuEbSlip = new bool[4] { false, false, false, false };

        private bool[] dcuEbFault = new bool[4] { false, false, false, false };

        private int[] dcuEbRealValue = new int[4] { 0, 0, 0, 0 };

        private int[] dcuMax = new int[4] { 0, 0, 0, 0 };

        private int[] abCapacity = new int[6] { 0, 0, 0, 0, 0, 0 };

        private int[] abRealValue = new int[6] { 0, 0, 0, 0, 0, 0 };

        private int[] dcuVolta = new int[4] { 0, 0, 0, 0 };

        /// <summary>
        /// 生命信号
        /// </summary>
        public int LifeSig { get => lifeSig; set => lifeSig = value; }

        /// <summary>
        /// 牵引命令
        /// </summary>
        public bool DriveCmd { get => driveCmd; set => driveCmd = value; }

        /// <summary>
        /// 制动命令
        /// </summary>
        public bool BrakeCmd { get => brakeCmd; set => brakeCmd = value; }

        /// <summary>
        /// 快速制动
        /// </summary>
        public bool FastBrakeCmd { get => fastBrakeCmd; set => fastBrakeCmd = value; }

        /// <summary>
        /// 运输模式
        /// </summary>
        public bool TowingMode { get => towingMode; set => towingMode = value; }

        /// <summary>
        /// 保持制动缓解
        /// </summary>
        public bool HoldBrakeRealease { get => holdBrakeRealease; set => holdBrakeRealease = value; }

        /// <summary>
        /// 紧急制动命令
        /// </summary>
        public bool EmergencyBrakeCmd { get => emergencyBrakeCmd; set => emergencyBrakeCmd = value; }

        /// <summary>
        /// 紧急牵引命令
        /// </summary>
        public bool EmergencyDriveCmd { get => emergencyDriveCmd; set => emergencyDriveCmd = value; }

        /// <summary>
        /// ATO模式
        /// </summary>
        public bool ATOMode1 { get => ATOMode; set => ATOMode = value; }

        /// <summary>
        /// 列车减速度设定值有效
        /// </summary>
        public bool AccSetupEnable { get => accSetupEnable; set => accSetupEnable = value; }

        /// <summary>
        /// 自检开启
        /// </summary>
        public bool SelfTestEnable { get => selfTestEnable; set => selfTestEnable = value; }

        /// <summary>
        /// 电制动淡出
        /// </summary>
        public bool EdFadeOut { get => edFadeOut; set => edFadeOut = value; }

        /// <summary>
        /// 整车制动力有效
        /// </summary>
        public bool TrainBrakeEnable { get => trainBrakeEnable; set => trainBrakeEnable = value; }

        /// <summary>
        /// 列车减速度设定值
        /// </summary>
        public int AccSetupValue { get => accSetupValue; set => accSetupValue = value; }

        /// <summary>
        /// 整车制动力需求
        /// </summary>
        public int BrakeForceReq { get => brakeForceReq; set => brakeForceReq = value; }

        /// <summary>
        /// 2车电制动实际值
        /// </summary>
        public int EdRealValue2 { get => edRealValue2; set => edRealValue2 = value; }

        /// <summary>
        /// 3车电制动实际值
        /// </summary>
        public int EdRealValue3 { get => edRealValue3; set => edRealValue3 = value; }

        /// <summary>
        /// 2车电制动能力值
        /// </summary>
        public int EdCapValue2 { get => edCapValue2; set => edCapValue2 = value; }

        /// <summary>
        /// 3车电制动能力值
        /// </summary>
        public int EdCapValue3 { get => edCapValue3; set => edCapValue3 = value; }

        /// <summary>
        /// 2车电制动可用
        /// </summary>
        public bool EdEnable2 { get => edEnable2; set => edEnable2 = value; }


        /// <summary>
        /// 2车电制动有效
        /// </summary>
        public bool EdActive2 { get => edActive2; set => edActive2 = value; }

        /// <summary>
        /// 2车电制动滑行
        /// </summary>
        public bool EdSlip2 { get => edSlip2; set => edSlip2 = value; }

        /// <summary>
        /// 3车电制动可用
        /// </summary>
        public bool EdEnable3 { get => edEnable3; set => edEnable3 = value; }

        /// <summary>
        /// 3车电制动有效
        /// </summary>
        public bool EdActive3 { get => edActive3; set => edActive3 = value; }

        /// <summary>
        /// 3车电制动滑行
        /// </summary>
        public bool EdSlip3 { get => edSlip3; set => edSlip3 = value; }

        /// <summary>
        /// A1车BCP太低
        /// </summary>
        public bool BCPLowA11 { get => BCPLowA1; set => BCPLowA1 = value; }

        /// <summary>
        /// 紧急制动功能异常
        /// </summary>
        public bool EmergencyBrakeException { get => emergencyBrakeException; set => emergencyBrakeException = value; }

        /// <summary>
        /// CAN内的停放制动全部缓解
        /// </summary>
        public bool CanParkBrakeRealease { get => canParkBrakeRealease; set => canParkBrakeRealease = value; }

        /// <summary>
        /// 速度检测故障
        /// </summary>
        public bool SpeedDetection { get => speedDetection; set => speedDetection = value; }

        /// <summary>
        /// CAN1总线故障
        /// </summary>
        public bool CanBusFail1 { get => canBusFail1; set => canBusFail1 = value; }

        /// <summary>
        /// CAN2总线故障
        /// </summary>
        public bool CanBusFail2 { get => canBusFail2; set => canBusFail2 = value; }

        /// <summary>
        /// 大事件
        /// </summary>
        public bool EventHigh { get => eventHigh; set => eventHigh = value; }

        /// <summary>
        /// 中等事件
        /// </summary>
        public bool EventMid { get => eventMid; set => eventMid = value; }

        /// <summary>
        /// 小事件
        /// </summary>
        public bool EventLow { get => eventLow; set => eventLow = value; }

        /// <summary>
        /// CAN内的全部ASP值有效
        /// </summary>
        public bool CanASPEnable { get => canASPEnable; set => canASPEnable = value; }

        /// <summary>
        /// A1车架1滑行
        /// </summary>
        public bool SlipA1 { get => slipA1; set => slipA1 = value; }

        /// <summary>
        /// A1车架1紧急制动施加
        /// </summary>
        public bool EmergencyBrakeActiveA1 { get => emergencyBrakeActiveA1; set => emergencyBrakeActiveA1 = value; }

        /// <summary>
        /// A1车架1制动已缓解
        /// </summary>
        public bool BrakeRealeaseA1 { get => brakeRealeaseA1; set => brakeRealeaseA1 = value; }

        /// <summary>
        /// A1车架1制动风缸压力低
        /// </summary>
        public bool BSRLowA11 { get => BSRLowA1; set => BSRLowA1 = value; }

        /// <summary>
        /// A1车架1气制动状态
        /// </summary>
        public bool AbStatuesA1 { get => abStatuesA1; set => abStatuesA1 = value; }

        /// <summary>
        /// A1车架1载荷信号有效
        /// </summary>
        public bool MassSigValid { get => massSigValid; set => massSigValid = value; }

        /// <summary>
        /// A1车架1载荷高八位
        /// </summary>
        public int MassA1 { get => massA1; set => massA1 = value; }

        /// <summary>
        /// 85km/h超速
        /// </summary>
        public bool OverSpeed85 { get => overSpeed85; set => overSpeed85 = value; }

        /// <summary>
        /// 非零速
        /// </summary>
        public bool NotZeroSpeed { get => notZeroSpeed; set => notZeroSpeed = value; }

        /// <summary>
        /// CAN单元中单个阀紧急激活
        /// </summary>
        public bool ValveCanEmergencyActive { get => valveCanEmergencyActive; set => valveCanEmergencyActive = value; }

        /// <summary>
        /// 硬线快速制动指令
        /// </summary>
        public bool HardFastBrakeCmd { get => hardFastBrakeCmd; set => hardFastBrakeCmd = value; }

        /// <summary>
        /// 硬线制动指令
        /// </summary>
        public bool HardBrakeCmd { get => hardBrakeCmd; set => hardBrakeCmd = value; }

        /// <summary>
        /// 硬线牵引指令
        /// </summary>
        public bool HardDriveCmd { get => hardDriveCmd; set => hardDriveCmd = value; }

        /// <summary>
        /// 硬线紧急牵引模式指令
        /// </summary>
        public bool HardEmergencyDriveMode { get => hardEmergencyDriveMode; set => hardEmergencyDriveMode = value; }

        /// <summary>
        /// 硬线紧急制动
        /// </summary>
        public bool HardEmergencyBrake { get; set; }

        /// <summary>
        /// CAN标识符
        /// </summary>
        public bool CanID { get => canID; set => canID = value; }

        /// <summary>
        /// A1车架1状态和诊断数据有效
        /// </summary>
        public bool DataValidA1Car1 { get => dataValidA1Car1; set => dataValidA1Car1 = value; }

        /// <summary>
        /// A1车架2状态和诊断数据有效
        /// </summary>
        public bool DataValidA1Car2 { get => dataValidA1Car2; set => dataValidA1Car2 = value; }

        /// <summary>
        /// B1车架1状态和诊断数据有效
        /// </summary>
        public bool DataValidB1Car1 { get => dataValidB1Car1; set => dataValidB1Car1 = value; }

        /// <summary>
        /// B1车架2状态和诊断数据有效
        /// </summary>
        public bool DataValidB1Car2 { get => dataValidB1Car2; set => dataValidB1Car2 = value; }

        /// <summary>
        /// C1车架1状态和诊断数据有效
        /// </summary>
        public bool DataValidC1Car1 { get => dataValidC1Car1; set => dataValidC1Car1 = value; }

        /// <summary>
        /// C1车架2状态和诊断数据有效
        /// </summary>
        public bool DataValidC1Car2 { get => dataValidC1Car2; set => dataValidC1Car2 = value; }

        /// <summary>
        /// A1车1轴速度
        /// </summary>
        public double SpeedA1Shaft1 { get => speedA1Shaft1; set => speedA1Shaft1 = value; }

        /// <summary>
        /// A1车2轴速度
        /// </summary>
        public double SpeedA1Shaft2 { get => speedA1Shaft2; set => speedA1Shaft2 = value; }

        /// <summary>
        /// A1车架1气制动能力
        /// </summary>
        public int EpCapA1Car1 { get => epCapA1Car1; set => epCapA1Car1 = value; }

        /// <summary>
        /// A1车架1制动风缸压力
        /// </summary>
        public int BSPPressureA1Car11 { get => BSPPressureA1Car1; set => BSPPressureA1Car1 = value; }

        /// <summary>
        /// A1车架1制动缸1压力
        /// </summary>
        public int BRKCylinder1PressureA11 { get => BRKCylinder1PressureA1; set => BRKCylinder1PressureA1 = value; }

        /// <summary>
        /// A1车架1制动缸2压力
        /// </summary>
        public int BRKCylinder2PressureA11 { get => BRKCylinder2PressureA1; set => BRKCylinder2PressureA1 = value; }

        /// <summary>
        /// A1车停放制动缸压力
        /// </summary>
        public int ParkPressureA1 { get => parkPressureA1; set => parkPressureA1 = value; }

        /// <summary>
        /// A1车架1空簧1压力
        /// </summary>
        public int AirSpring1PressureA1Car1 { get => airSpring1PressureA1Car1; set => airSpring1PressureA1Car1 = value; }

        /// <summary>
        /// A1车架1空簧2压力
        /// </summary>
        public int AirSpring2PressureA1Car1 { get => airSpring2PressureA1Car1; set => airSpring2PressureA1Car1 = value; }

        /// <summary>
        /// A1车架1制动风缸压力
        /// </summary>
        public bool BSRPressureEnableA1Car11 { get => BSRPressureEnableA1Car1; set => BSRPressureEnableA1Car1 = value; }

        /// <summary>
        /// A1车停放制动缸压力有效
        /// </summary>
        public bool ParkBrakePressureEnableA1 { get => parkBrakePressureEnableA1; set => parkBrakePressureEnableA1 = value; }

        /// <summary>
        /// A1车架1制动缸1压力有效
        /// </summary>
        public bool BRKCylinder1PressureEnableA1Car11 { get => BRKCylinder1PressureEnableA1Car1; set => BRKCylinder1PressureEnableA1Car1 = value; }

        /// <summary>
        /// A1车架1制动缸2压力有效
        /// </summary>
        public bool BRKCylinder2PressureEnableA1Car11 { get => BRKCylinder2PressureEnableA1Car1; set => BRKCylinder2PressureEnableA1Car1 = value; }

        /// <summary>
        /// A1车架1空簧1压力有效
        /// </summary>
        public bool AirSpring1PressureEnableA1Car1 { get => airSpring1PressureEnableA1Car1; set => airSpring1PressureEnableA1Car1 = value; }

        /// <summary>
        /// A1车架1空簧2压力有效
        /// </summary>
        public bool AirSpring2PressureEnableA1Car1 { get => airSpring2PressureEnableA1Car1; set => airSpring2PressureEnableA1Car1 = value; }

        /// <summary>
        /// B1车主风管压力有效
        /// </summary>
        public bool MREPreessureEnableB11 { get => MREPreessureEnableB1; set => MREPreessureEnableB1 = value; }

        /// <summary>
        /// B1车电制动切除
        /// </summary>
        public bool EdOffB1 { get => edOffB1; set => edOffB1 = value; }

        /// <summary>
        /// C1车电制动切除
        /// </summary>
        public bool EdOffC1 { get => edOffC1; set => edOffC1 = value; }

        /// <summary>
        /// 自检中断
        /// </summary>
        public bool SelfTestInt { get => selfTestInt; set => selfTestInt = value; }

        /// <summary>
        /// 自检已激活
        /// </summary>
        public bool SelfTestActive { get => selfTestActive; set => selfTestActive = value; }

        /// <summary>
        /// 自检成功
        /// </summary>
        public bool SelfTestSuccess { get => selfTestSuccess; set => selfTestSuccess = value; }

        /// <summary>
        /// 自检失败
        /// </summary>
        public bool SelfTestFail { get => selfTestFail; set => selfTestFail = value; }

        /// <summary>
        /// 24小时未自检
        /// </summary>
        public bool UnSelfTest24 { get => unSelfTest24; set => unSelfTest24 = value; }

        /// <summary>
        /// 26小时未自检
        /// </summary>
        public bool UnSelfTest26 { get => unSelfTest26; set => unSelfTest26 = value; }

        /// <summary>
        /// 网关阀状态
        /// </summary>
        public bool GateValveState { get => gateValveState; set => gateValveState = value; }

        /// <summary>
        /// A1车1轴速度有效
        /// </summary>
        public bool SpeedShaftEnable1 { get => speedShaftEnable1; set => speedShaftEnable1 = value; }

        /// <summary>
        /// A1车2轴速度有效
        /// </summary>
        public bool SpeedShaftEnable2 { get => speedShaftEnable2; set => speedShaftEnable2 = value; }

        /// <summary>
        /// 1架空气制动目标值
        /// </summary>
        public int AbTargetValueAx1 { get => abTargetValueAx1; set => abTargetValueAx1 = value; }

        /// <summary>
        /// 2架空气制动目标值
        /// </summary>
        public int AbTargetValueAx2 { get => abTargetValueAx2; set => abTargetValueAx2 = value; }

        /// <summary>
        /// 3架空气制动目标值
        /// </summary>
        public int AbTargetValueAx3 { get => abTargetValueAx3; set => abTargetValueAx3 = value; }

        /// <summary>
        /// 4架空气制动目标值
        /// </summary>
        public int AbTargetValueAx4 { get => abTargetValueAx4; set => abTargetValueAx4 = value; }

        /// <summary>
        /// 5架空气制动目标值
        /// </summary>
        public int AbTargetValueAx5 { get => abTargetValueAx5; set => abTargetValueAx5 = value; }

        /// <summary>
        /// 6架空气制动目标值
        /// </summary>
        public int AbTargetValueAx6 { get => abTargetValueAx6; set => abTargetValueAx6 = value; }

        /// <summary>
        /// 1架VLD压力实际值
        /// </summary>
        public int VldRealPressureAx1 { get => vldRealPressureAx1; set => vldRealPressureAx1 = value; }

        /// <summary>
        /// 1架BCP1压力
        /// </summary>
        public int Bcp1PressureAx1 { get => bcp1PressureAx1; set => bcp1PressureAx1 = value; }

        /// <summary>
        /// 1架BCP2压力
        /// </summary>
        public int Bcp2PressureAx2 { get => bcp2PressureAx2; set => bcp2PressureAx2 = value; }

        /// <summary>
        /// 1架VLD压力设定值
        /// </summary>
        public int VldPressureSetupAx1 { get => vldPressureSetupAx1; set => vldPressureSetupAx1 = value; }

        /// <summary>
        /// C车主风管压力
        /// </summary>
        public int MbpPressure { get => mbpPressure; set => mbpPressure = value; }

        /// <summary>
        /// 制动风缸压力
        /// </summary>
        public int BrakeCylinderSourcePressure { get => brakeCylinderSourcePressure; set => brakeCylinderSourcePressure = value; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public string Mode { get => mode; set => mode = value; }

        /// <summary>
        /// 惰行命令
        /// </summary>
        public bool LazyCmd { get => lazyCmd; set => lazyCmd = value; }

        /// <summary>
        /// 保持制动状态
        /// </summary>
        public bool KeepBrakeState { get; set; }

        /// <summary>
        /// 惰行状态
        /// </summary>
        public bool LazyState { get; set; }

        /// <summary>
        /// 牵引状态
        /// </summary>
        public bool DriveState { get; set; }

        /// <summary>
        /// 常用制动状态
        /// </summary>
        public bool NormalBrakeState { get; set; }

        /// <summary>
        /// 紧急制动状态
        /// </summary>
        public bool EmergencyBrakeState { get; set; }

        /// <summary>
        /// 制动级位
        /// </summary>
        public int BrakeLevel { get; set; }

        /// <summary>
        /// 整车制动力
        /// </summary>
        public int TrainBrakeForce { get; set; }

        /// <summary>
        /// 空气制动施加
        /// </summary>
        public bool AbActive { get; set; }

        /// <summary>
        /// 停放制动缓解
        /// </summary>
        public bool ParkBreakRealease { get; set; }

        /// <summary>
        /// 硬线紧急牵引
        /// </summary>
        public bool HardEmergencyDriveCmd { get; set; }

        /// <summary>
        /// 网络牵引命令
        /// </summary>
        public bool NetDriveCmd { get; set; }

        /// <summary>
        /// 网络制动命令
        /// </summary>
        public bool NetBrakeCmd { get; set; }

        /// <summary>
        /// 网络快速制动命令
        /// </summary>
        public bool NetFastBrakeCmd { get; set; }

        /// <summary>
        /// 制动级位有效
        /// </summary>
        public bool BrakeLevelEnable { get; set; }

        /// <summary>
        /// 零速
        /// </summary>
        public bool ZeroSpeed { get; set; }

        /// <summary>
        /// 自检命令
        /// </summary>
        public bool SelfTestCmd { get; set; }

        /// <summary>
        /// 轮径输入状态
        /// </summary>
        public bool WheelInputState { get; set; }

        /// <summary>
        /// 传感器故障
        /// </summary>
        public int SensorFail { get; set; }

        /// <summary>
        /// 硬线指令不一致
        /// </summary>
        public bool HardDifferent { get; set; }

        /// <summary>
        /// 空簧1压力超出范围
        /// </summary>
        public bool AirSpringOverflow_1 { get; set; }

        /// <summary>
        /// 空簧2压力超出范围
        /// </summary>
        public bool AirSpringOverflow_2 { get; set; }



        /// <summary>
        /// A车BCP过低
        /// </summary>
        public bool BCPLowA { get; set; }

        /// <summary>
        /// B车BCP过低
        /// </summary>
        public bool BCPLowB { get; set; }

        /// <summary>
        /// C车BCP过低
        /// </summary>
        public bool BCPLowC { get; set; }

        /// <summary>
        /// 软件版本
        /// </summary>
        public int SoftVersion { get; set; }

        /// <summary>
        /// VCM生命信号
        /// </summary>
        public int VCMLifeSig { get; set; }

        /// <summary>
        /// DCU生命信号
        /// </summary>
        public int[] DcuLifeSig { get => dcuLifeSig; set => dcuLifeSig = value; }

        /// <summary>
        /// DCU电制动OK
        /// </summary>
        public bool[] DcuEbOK { get => dcuEbOK; set => dcuEbOK = value; }

        /// <summary>
        /// DCU电制动淡出
        /// </summary>
        public bool[] DcuEbFadeout { get => dcuEbFadeout; set => dcuEbFadeout = value; }

        /// <summary>
        /// DCU电制动滑行
        /// </summary>
        public bool[] DcuEbSlip { get => dcuEbSlip; set => dcuEbSlip = value; }

        /// <summary>
        /// DCU电制动实际值
        /// </summary>
        public int[] DcuEbRealValue { get => dcuEbRealValue; set => dcuEbRealValue = value; }

        /// <summary>
        /// DCU最大电制动可用值
        /// </summary>
        public int[] DcuMax { get => dcuMax; set => dcuMax = value; }

        /// <summary>
        /// 空气制动力能力值
        /// </summary>
        public int[] AbCapacity { get => abCapacity; set => abCapacity = value; }

        /// <summary>
        /// 空气制动实际值
        /// </summary>
        public int[] AbRealValue { get => abRealValue; set => abRealValue = value; }

        /// <summary>
        /// Unix时间 小时
        /// </summary>
        public int UnixHour { get; set; }


        /// <summary>
        /// Unix时间 分钟
        /// </summary>
        public int UnixMinute { get; set; }

        /// <summary>
        /// Unix时间有效
        /// </summary>
        public bool UnixTimeValid { get; set; }



        /// <summary>
        /// 参考速度
        /// </summary>
        public double RefSpeed { get; set; }

        /// <summary>
        /// 自检目标设定值
        /// </summary>
        public int SelfTestSetup { get; set; }

        /// <summary>
        /// can单元A架自检命令
        /// </summary>
        public bool CanUintSelfTestCmd_A { get; set; }

        /// <summary>
        /// can单元B架自检命令
        /// </summary>
        public bool CanUintSelfTestCmd_B { get; set; }

        /// <summary>
        /// CAN单元自检启动
        /// </summary>
        public bool CanUnitSelfTestOn { get; set; }

        /// <summary>
        /// CAN单元自检结束
        /// </summary>
        public bool CanUintSelfTestOver { get; set; }

        /// <summary>
        /// VCM与MVB通信状态
        /// </summary>
        public bool VCM_MVBConnectionState { get; set; }

        /// <summary>
        /// 架1自检激活
        /// </summary>
        public bool Ax1SelfTestActive { get; set; }

        /// <summary>
        /// 架1自检完成
        /// </summary>
        public bool Ax1SelfTestOver { get; set; }

        /// <summary>
        /// 架1自检成功
        /// </summary>
        public bool Ax1SelfTestSuccess { get; set; }

        /// <summary>
        /// 架1自检失败
        /// </summary>
        public bool Ax1SelfTestFail { get; set; }

        /// <summary>
        /// 制动风缸传感器故障
        /// </summary>
        public bool BSSRSenorFault { get; set; }

        /// <summary>
        /// 空簧传感器1故障
        /// </summary>
        public bool AirSpringSenorFault_1 { get; set; }

        /// <summary>
        /// 空簧传感器2故障
        /// </summary>
        public bool AirSpringSenorFault_2 { get; set; }

        /// <summary>
        /// 停放缸传感器故障
        /// </summary>
        public bool ParkCylinderSenorFault { get; set; }

        /// <summary>
        /// VLD传感器故障
        /// </summary>
        public bool VLDSensorFault { get; set; }

        /// <summary>
        /// 制动缸传感器1故障
        /// </summary>
        public bool BSRSenorFault_1 { get; set; }

        /// <summary>
        /// 制动缸传感器2故障
        /// </summary>
        public bool BSRSenorFault_2 { get; set; }

        /// <summary>
        /// BCU严重故障
        /// </summary>
        public bool BCUFail_Serious { get; set; }

        /// <summary>
        /// BCU中等故障
        /// </summary>
        public bool BCUFail_Middle { get; set; }

        /// <summary>
        /// BCU轻微故障
        /// </summary>
        public bool BCUFail_Slight { get; set; }

        /// <summary>
        /// 紧急制动不可用
        /// </summary>
        public bool EmergencyBrakeFault { get; set; }

        /// <summary>
        /// 速度传感器1故障
        /// </summary>
        public bool SpeedSenorFault_1 { get; set; }

        /// <summary>
        /// 速度传感器2故障
        /// </summary>
        public bool SpeedSenorFault_2 { get; set; }

        /// <summary>
        /// WSP故障1
        /// </summary>
        public bool WSPFault_1 { get; set; }

        /// <summary>
        /// WSP故障2
        /// </summary>
        public bool WSPFault_2 { get; set; }

        /// <summary>
        /// 编码插头故障
        /// </summary>
        public bool CodeConnectorFault { get; set; }

        /// <summary>
        /// 制动不缓解故障
        /// </summary>
        public bool BrakeNotRealease { get; set; }

        /// <summary>
        /// 空簧压力超限
        /// </summary>
        public bool AirSpringLimit { get; set; }

        /// <summary>
        /// can单元零速
        /// </summary>
        public bool ZeroSpeedCan { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime dateTime { get; set; }

        /// <summary>
        /// 轴1减速度
        /// </summary>
        public double AccValue1 { get; set; }

        /// <summary>
        /// 轴2减速度
        /// </summary>
        public double AccValue2 { get; set; }

        /// <summary>
        /// 轴1滑行等级
        /// </summary>
        public int SlipLvl1 { get; set; }

        /// <summary>
        /// 轴2滑行等级
        /// </summary>
        public int SlipLvl2 { get; set; }

        /// <summary>
        /// 横坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// ATO保持施加
        /// </summary>
        public bool ATOHold { get; set; }

        /// <summary>
        /// ICAN1故障
        /// </summary>
        public bool ICANFault1 { get; set; }

        /// <summary>
        /// ICAN2故障
        /// </summary>
        public bool ICANFault2 { get; set; }

        /// <summary>
        /// OCAN1故障
        /// </summary>
        public bool OCANFault1 { get; set; }

        /// <summary>
        /// OCAN2故障
        /// </summary>
        public bool OCANFault2 { get; set; }

        /// <summary>
        /// 架1为主时架6的生命信号
        /// </summary>
        public int LifeSigReverse { get; set; }

        /// <summary>
        /// 轴1滑移率
        /// </summary>
        public double SlipRate1 { get; set; }

        /// <summary>
        /// 轴2滑移率
        /// </summary>
        public double SlipRate2 { get; set; }

        /// <summary>
        /// DCU电网电压
        /// </summary>
        public int[] DcuVolta { get => dcuVolta; set => dcuVolta = value; }

        /// <summary>
        /// Tc1车轮径
        /// </summary>
        public int Tc1 { get; set; }

        /// <summary>
        /// Tc2车轮径
        /// </summary>
        public int Tc2 { get; set; }

        public int Mp1 { get; set; }

        public int Mp2 { get; set; }

        public int M1 { get; set; }

        public int M2 { get; set; }

        public bool Tc1Valid { get; set; }

        public bool Tc2Valid { get; set; }

        public bool Mp1Valid { get; set; }

        public bool Mp2Valid { get; set; }

        public bool M1Valid { get; set; }

        public bool M2Valid { get; set; }

        public bool CanWheelInputCondition { get; set; }
        /// <summary>
        /// Ep板电制动切除
        /// </summary>
        public bool EPCutOff { get; set; }

        /// <summary>
        /// 轴1滑行
        /// </summary>
        public bool AxisSlip1 { get; set; }

        /// <summary>
        /// 轴2滑行
        /// </summary>
        public bool AxisSlip2 { get; set; }

        /// <summary>
        /// 轮径存储失败
        /// </summary>
        public bool WheelStoreFail { get; set; }

        /// <summary>
        /// 空簧信号有效
        /// </summary>
        public bool AirSigValid { get; set; }

        /// <summary>
        /// Tc1基准值
        /// </summary>
        public int Tc1Stander { get; set; }

        /// <summary>
        /// Tc2基准值
        /// </summary>
        public int Tc2Stander { get; set; }

        /// <summary>
        /// 确认下载模式
        /// </summary>
        public bool ConfirmDownload { get; set; }

        /// <summary>
        /// CPU面板编码地址
        /// </summary>
        public int CPUAddr { get; set; }

        /// <summary>
        /// 软件版本CPU
        /// </summary>
        public int SoftwareVersionCPU { get; set; }

        /// <summary>
        /// 软件版本EP
        /// </summary>
        public int SoftwareVersionEP { get; set; }

        #region 2018-9-24:新增一系列的变量，用于上位机输入设定这些变量

        /// <summary>
        /// WSP阈值设定有效标志位
        /// </summary>
        public bool bWSPSetting { get; set; }

        /// <summary>
        /// 速度差1阈值
        /// </summary>
        public int SpeedDiff_1_Threshold { get; set; }

        /// <summary>
        /// 速度差2阈值
        /// </summary>
        public int SpeedDiff_2_Threshold { get; set; }

        /// <summary>
        /// 速度差3阈值
        /// </summary>
        public int SpeedDiff_3_Threshold { get; set; }

        /// <summary>
        /// 速度差4阈值
        /// </summary>
        public int SpeedDiff_4_Threshold { get; set; }

        /// <summary>
        /// 速度差5阈值
        /// </summary>
        public int SpeedDiff_5_Threshold { get; set; }

        /// <summary>
        /// 速度差6阈值
        /// </summary>
        public int SpeedDiff_6_Threshold { get; set; }

        /// <summary>
        /// 速度差7阈值
        /// </summary>
        public int SpeedDiff_7_Threshold { get; set; }

        /// <summary>
        /// 速度差8阈值
        /// </summary>
        public int SpeedDiff_8_Threshold { get; set; }

        /// <summary>
        /// 速度差9阈值
        /// </summary>
        public int SpeedDiff_9_Threshold { get; set; }

        /// <summary>
        /// 速度差10阈值
        /// </summary>
        public int SpeedDiff_10_Threshold { get; set; }

        /// <summary>
        /// 速度差11阈值
        /// </summary>
        public int SpeedDiff_11_Threshold { get; set; }

        /// <summary>
        /// 速度差12阈值
        /// </summary>
        public int SpeedDiff_12_Threshold { get; set; }

        /// <summary>
        /// 速度差13阈值
        /// </summary>
        public int SpeedDiff_13_Threshold { get; set; }

        /// <summary>
        /// 速度差14阈值
        /// </summary>
        public int SpeedDiff_14_Threshold { get; set; }

        /// <summary>
        /// 速度差15阈值
        /// </summary>
        public int SpeedDiff_15_Threshold { get; set; }

        /// <summary>
        /// 减速度阈值a1
        /// </summary>
        public int Deceleration_1_Threshold { get; set; }

        /// <summary>
        /// 减速度阈值a2
        /// </summary>
        public int Deceleration_2_Threshold { get; set; }

        /// <summary>
        /// 减速度阈值a3
        /// </summary>
        public int Deceleration_3_Threshold { get; set; }

        /// <summary>
        /// 阶段充风比率[0~100]
        /// </summary>
        public int Air_Charge_Ratio { get; set; }

        /// <summary>
        /// 阶段充风周期[0~10]
        /// </summary>
        public int Air_Filling_Cycle { get; set; }

        /// <summary>
        /// 阶段保压周期[0~10]（第一个）
        /// </summary>
        public int Holding_Cycle_1 { get; set; }

        /// <summary>
        /// 第一次排风比率[0~100]（第一个）
        /// </summary>
        public int First_Exhaust_Ratio_1 { get; set; }

        /// <summary>
        /// 阶段排风比率[0~100]
        /// </summary>
        public int Exhaust_Ratio { get; set; }

        /// <summary>
        /// 阶段排风周期[0~100]
        /// </summary>
        public int Exhaust_Cycle { get; set; }

        /// <summary>
        /// 阶段保压周期[0~10]（第二个）
        /// </summary>
        public int Holding_Cycle_2 { get; set; }

        /// <summary>
        /// 第一次排风比率[0~100]（第二个）
        /// </summary>
        public int First_Exhaust_Ratio_2 { get; set; }

        /// <summary>
        /// 速度大于70km/h百分比阈值
        /// </summary>
        public int Percentage_Threshold { get; set; }
        #endregion

        /// <summary>
        /// 主风管传感器故障
        /// </summary>
        public bool MainPipeSensorFault { get; set; }

        /// <summary>
        /// DCU电制动不可用
        /// </summary>
        public bool[] DcuEbFault { get => dcuEbFault; set => dcuEbFault = value; }

        /// <summary>
        /// 轮径存储值
        /// </summary>
        public double WheelSize { get; set; }

        /// <summary>
        /// 轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault_1 { get; set; }

        /// <summary>
        /// 轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault_2 { get; set; }

        /// <summary>
        /// 轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault_1 { get; set; }

        /// <summary>
        /// 轴2WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault_2 { get; set; }
        /// <summary>
        /// 1轴传感器故障
        /// </summary>
        public string SpeedSensorError_1 = NORMAL;
        /// <summary>
        /// 2轴传感器故障
        /// </summary>
        public string SpeedSensorError_2 = NORMAL;

        /// <summary>
        /// 开关量输入1-8
        /// </summary>
        public byte SwitchInputChannel1_8 { get; set; }

        /// <summary>
        /// 数字量输出9-16
        /// </summary>
        public byte DigitalOutputChannel9_16 { get; set; }

        /// <summary>
        /// 数字量输出1-8
        /// </summary>
        public byte DigitalOutputChannel1_8 { get; set; }

        /// <summary>
        /// 输出过流保护9-16
        /// </summary>
        public byte OutputOverFlowProtectChannel9_16 { get; set; }

        /// <summary>
        /// 输出过流保护1-8
        /// </summary>
        public byte OutputOverFlowProtectChannel1_8 { get; set; }

        /// <summary>
        /// 轴1排风故障
        /// </summary>
        public bool exhaustFault_1 { get; set; }

        /// <summary>
        /// 轴2排风故障
        /// </summary>
        public bool exhuastFault_2 { get; set; }

        /// <summary>
        /// 轴1保压故障
        /// </summary>
        public bool keepPressureFault_1 { get; set; }

        /// <summary>
        /// 轴2保压故障
        /// </summary>
        public bool keepPressureFault_2 { get; set; }

        /// <summary>
        /// 轴1充风故障
        /// </summary>
        public bool chargingFault_1 { get; set; }

        /// <summary>
        /// 轴2充风故障
        /// </summary>
        public bool chargingFault_2 { get; set; }

        /// <summary>
        /// 连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure { get; set; }

        /// <summary>
        /// 连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure { get; set; }

        /// <summary>
        /// VLD充风故障
        /// </summary>
        public bool VLDChargingFault { get; set; }

        /// <summary>
        /// VLD排风故障
        /// </summary>
        public bool VLDExhuastFault { get; set; }

        /// <summary>
        /// VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault { get; set; }

        /// <summary>
        /// 轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault_1 { get; set; }

        /// <summary>
        /// 轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault_1 { get; set; }

        /// <summary>
        /// 轴2排风故障
        /// </summary>
        public bool WSPExhuastFault_2 { get; set; }

        /// <summary>
        /// 轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault_2 { get; set; }

        /// <summary>
        /// 轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure_2 { get; set; }

        /// <summary>
        /// 轴1WSP目标值
        /// </summary>
        public int WSPTargetValue_1 { get; set; }

        /// <summary>
        /// 轴2WSP目标值
        /// </summary>
        public int WSPTargetValue_2 { get; set; }

        /// <summary>
        /// 连接阀控制命令
        /// </summary>
        public bool ConnectValveControl { get; set; }

        //2019-12-25:新加变量
        // 网关阀状态GateValveState
        // 轴1速度SpeedA1Shaft1
        // 轴2速度SpeedA1Shaft2
        // 参考速度RefSpeed
        // 轴1滑移率SlipRate1
        // 轴2滑移率SlipRate2

        // 制动指令NetBrakeCmd
        // 快速制动指令NetFastBrakeCmd
        // 硬线制动指令HardBrakeCmd
        // 硬线快速制动指令HardFastBrakeCmd
        // 硬线紧急制动HardEmergencyBrake
    }
}
