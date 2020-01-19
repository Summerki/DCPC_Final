using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    
    /// <summary>
    /// 从设备的消息类，X=2~6，表示EBCU2~6
    /// </summary>
    public class SliverDataContainer
    {
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
        /// X1车架X制动风缸压力有效
        /// </summary>
        private bool BSRPressureEnable;

        /// <summary>
        /// X1车停放制动缸压力有效
        /// </summary>
        private bool parkBrakePressureEnable;

        /// <summary>
        /// X1车架1制动缸1压力有效
        /// </summary>
        private bool BRKCylinder1Enable;

        /// <summary>
        /// X1车架1制动缸2压力有效
        /// </summary>
        private bool BRKCylinder2Enable;

        /// <summary>
        /// X1车架1空簧1压力有效
        /// </summary>
        private bool airSpringPressureEnable1;

        /// <summary>
        /// X1车架1空簧2压力有效
        /// </summary>
        private bool airSpringPressureEnable2;

        /// <summary>
        /// B1车主风管压力有效（仅B1架1有）
        /// </summary>
        private bool MREPressureEnable;

        /// <summary>
        /// 车架滑行
        /// </summary>
        private bool slip;

        /// <summary>
        /// 紧急制动施加
        /// </summary>
        private bool emergencyBrake;

        /// <summary>
        /// 制动已缓解
        /// </summary>
        private bool brakeRealease;

        /// <summary>
        /// 制动风缸压力低
        /// </summary>
        private bool BSRLow;

        /// <summary>
        /// 停放制动缓解
        /// </summary>
        private bool parkBrakeRealease;

        /// <summary>
        /// 气制动状态	
        /// </summary>
        private bool epState;

        /// <summary>
        /// 载荷信号有效
        /// </summary>
        private bool massSigValid;

        /// <summary>
        /// BCP1制动缸压力
        /// </summary>
        private int BRKCylinderPressure1;

        /// <summary>
        /// BCP2制动缸压力
        /// </summary>
        private int BRKCylinderPressure2;

        /// <summary>
        /// 停放制动缸/总风压力
        /// </summary>
        private int parkPressure;

        /// <summary>
        /// 停放制动缸/总风压力
        /// </summary>
        private int airSpringPressure1;

        /// <summary>
        /// ASP1空气弹簧压力
        /// </summary>
        private int airSpringPressure2;

        /// <summary>
        /// ASP2空气弹簧压力
        /// </summary>
        private int BSRPressure;

        /// <summary>
        /// 本架载荷
        /// </summary>
        private int massValue;

        /// <summary>
        /// 实际空气制动力
        /// </summary>
        private int abForceValue;

        /// <summary>
        /// 空气制动力能力
        /// </summary>
        private int abCapValue;

        /// <summary>
        /// 1轴检测速度
        /// </summary>
        private double speedShaft1;

        /// <summary>
        /// 2轴检测速度
        /// </summary>
        private double speedShaft2;

        /// <summary>
        /// 1轴速度有效
        /// </summary>
        private bool speedShaftEnable1;

        /// <summary>
        /// 2轴速度有效
        /// </summary>
        private bool speedShaftEnable2;

        /// <summary>
        /// 1轴有滑行控制
        /// </summary>
        private bool hasSlipControl1;

        /// <summary>
        /// 2轴有滑行控制
        /// </summary>
        private bool hasSlipControl2;

        /// <summary>
        /// 空气制动有滑行控制
        /// </summary>
        private bool abControlledBySlip;

        /// <summary>
        /// A1车BCP太低
        /// </summary>
        private bool BCPLow;

        /// <summary>
        /// 紧急制动功能异常
        /// </summary>
        private bool emergencyBrakeException;

        /// <summary>
        /// VLD压力实际值
        /// </summary>
        private int vldRealPressure;
        
        /// <summary>
        /// BCP1压力
        /// </summary>
        private int bcp1Pressure;

        /// <summary>
        /// BCP2压力
        /// </summary>
        private int bcp2Pressure;

        /// <summary>
        /// VLD压力设定值
        /// </summary>
        private int vldSetupPressure;

        /// <summary>
        /// 制动风缸压力
        /// </summary>
        private int brakeCylinderPressure;

        /// <summary>
        /// 生命信号
        /// </summary>
        public int LifeSig { get => lifeSig; set => lifeSig = value; }

        /// <summary>
        /// X1车架X制动风缸压力有效
        /// </summary>
        public bool BSRPressureEnable1 { get => BSRPressureEnable; set => BSRPressureEnable = value; }

        /// <summary>
        /// X1车停放制动缸压力有效
        /// </summary>
        public bool ParkBrakePressureEnable { get => parkBrakePressureEnable; set => parkBrakePressureEnable = value; }

        /// <summary>
        /// X1车架1制动缸1压力有效
        /// </summary>
        public bool BRKCylinder1Enable1 { get => BRKCylinder1Enable; set => BRKCylinder1Enable = value; }

        /// <summary>
        /// X1车架1制动缸2压力有效
        /// </summary>
        public bool BRKCylinder2Enable1 { get => BRKCylinder2Enable; set => BRKCylinder2Enable = value; }

        /// <summary>
        /// X1车架1空簧1压力有效
        /// </summary>
        public bool AirSpringPressureEnable1 { get => airSpringPressureEnable1; set => airSpringPressureEnable1 = value; }

        /// <summary>
        /// X1车架1空簧2压力有效
        /// </summary>
        public bool AirSpringPressureEnable2 { get => airSpringPressureEnable2; set => airSpringPressureEnable2 = value; }

        /// <summary>
        /// B1车主风管压力有效
        /// </summary>
        public bool MREPressureEnable1 { get => MREPressureEnable; set => MREPressureEnable = value; }

        /// <summary>
        /// 车架滑行
        /// </summary>
        public bool Slip { get => slip; set => slip = value; }

        /// <summary>
        /// 紧急制动施加
        /// </summary>
        public bool EmergencyBrake { get => emergencyBrake; set => emergencyBrake = value; }

        /// <summary>
        /// 制动已缓解
        /// </summary>
        public bool BrakeRealease { get => brakeRealease; set => brakeRealease = value; }

        /// <summary>
        /// 制动风缸压力低
        /// </summary>
        public bool BSRLow1 { get => BSRLow; set => BSRLow = value; }

        /// <summary>
        /// 停放制动缓解
        /// </summary>
        public bool ParkBrakeRealease { get => parkBrakeRealease; set => parkBrakeRealease = value; }

        /// <summary>
        /// 气制动状态	
        /// </summary>
        public bool EpState { get => epState; set => epState = value; }

        /// <summary>
        /// 载荷信号有效
        /// </summary>
        public bool MassSigValid { get => massSigValid; set => massSigValid = value; }

        /// <summary>
        /// BCP1制动缸压力
        /// </summary>
        public int BRKCylinderPressure11 { get => BRKCylinderPressure1; set => BRKCylinderPressure1 = value; }

        /// <summary>
        /// BCP2制动缸压力
        /// </summary>
        public int BRKCylinderPressure21 { get => BRKCylinderPressure2; set => BRKCylinderPressure2 = value; }

        /// <summary>
        /// 停放制动缸/总风压力
        /// </summary>
        public int ParkPressure { get => parkPressure; set => parkPressure = value; }

        /// <summary>
        /// ASP1空气弹簧压力
        /// </summary>
        public int AirSpringPressure1 { get => airSpringPressure1; set => airSpringPressure1 = value; }

        /// <summary>
        /// ASP2空气弹簧压力
        /// </summary>
        public int AirSpringPressure2 { get => airSpringPressure2; set => airSpringPressure2 = value; }

        /// <summary>
        /// BSRP压力
        /// </summary>
        public int BSRPressure1 { get => BSRPressure; set => BSRPressure = value; }

        /// <summary>
        /// 本架载荷
        /// </summary>
        public int MassValue { get => massValue; set => massValue = value; }

        /// <summary>
        /// 实际空气制动力
        /// </summary>
        public int AbForceValue { get => abForceValue; set => abForceValue = value; }

        /// <summary>
        /// 空气制动力能力
        /// </summary>
        public int AbCapValue { get => abCapValue; set => abCapValue = value; }

        /// <summary>
        /// 1轴检测速度
        /// </summary>
        public double SpeedShaft1 { get => speedShaft1; set => speedShaft1 = value; }

        /// <summary>
        /// 2轴检测速度
        /// </summary>
        public double SpeedShaft2 { get => speedShaft2; set => speedShaft2 = value; }

        /// <summary>
        /// 1轴速度有效
        /// </summary>
        public bool SpeedShaftEnable1 { get => speedShaftEnable1; set => speedShaftEnable1 = value; }

        /// <summary>
        /// 2轴速度有效
        /// </summary>
        public bool SpeedShaftEnable2 { get => speedShaftEnable2; set => speedShaftEnable2 = value; }

        /// <summary>
        /// 空气制动有滑行控制
        /// </summary>
        public bool AbControlledBySlip { get => abControlledBySlip; set => abControlledBySlip = value; }

        /// <summary>
        /// A1车BCP太低
        /// </summary>
        public bool BCPLow1 { get => BCPLow; set => BCPLow = value; }

        /// <summary>
        /// 紧急制动功能异常
        /// </summary>
        public bool EmergencyBrakeException { get => emergencyBrakeException; set => emergencyBrakeException = value; }

        /// <summary>
        /// 1轴有滑行控制
        /// </summary>
        public bool HasSlipControl1 { get => hasSlipControl1; set => hasSlipControl1 = value; }

        /// <summary>
        /// 2轴有滑行控制
        /// </summary>
        public bool HasSlipControl2 { get => hasSlipControl2; set => hasSlipControl2 = value; }

        /// <summary>
        /// VLD压力实际值
        /// </summary>
        public int VldRealPressure { get => vldRealPressure; set => vldRealPressure = value; }

        /// <summary>
        /// BCP1压力
        /// </summary>
        public int Bcp1Pressure { get => bcp1Pressure; set => bcp1Pressure = value; }

        /// <summary>
        /// BCP2压力
        /// </summary>
        public int Bcp2Pressure { get => bcp2Pressure; set => bcp2Pressure = value; }

        /// <summary>
        /// VLD压力设定值
        /// </summary>
        public int VldSetupPressure { get => vldSetupPressure; set => vldSetupPressure = value; }

        /// <summary>
        /// 制动风缸压力
        /// </summary>
        public int BrakeCylinderSourcePressure { get => brakeCylinderPressure; set => brakeCylinderPressure = value; }

        /// <summary>
        /// 紧急制动激活
        /// </summary>
        public bool EmergencyBrakeActive { get; set; }

        /// <summary>
        /// 空气制动施加
        /// </summary>
        public bool AbBrakeActive { get; set; }

        /// <summary>
        /// 气制动状态
        /// </summary>
        public bool AbBrakeSatet { get; set; }

        /// <summary>
        /// 自检目标设定值
        /// </summary>
        public int SelfTestSetup { get; set; }

        public bool SelfTestActive { get; set; }

        public bool SelfTestOver { get; set; }

        public bool SelfTestSuccess { get; set; }

        public bool SelfTestFail { get; set; }

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
        /// 空簧1压力超出范围
        /// </summary>
        public bool AirSpringOverflow_1 { get; set; }

        /// <summary>
        /// 空簧2压力超出范围
        /// </summary>
        public bool AirSpringOverflow_2 { get; set; }

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

        public double X { get; set; }

        /// <summary>
        /// ICAN1故障
        /// </summary>
        public bool ICANFault1 { get; set; }

        /// <summary>
        /// ICAN2故障
        /// </summary>
        public bool ICANFault2 { get; set; }

        /// <summary>
        /// OCAN1通讯故障
        /// </summary>
        public bool OCANFault1 { get; set; }

        /// <summary>
        /// OCAN2通讯故障
        /// </summary>
        public bool OCANFault2 { get; set; }

        /// <summary>
        /// 轴1滑移率
        /// </summary>
        public double SlipRate1 { get; set; }

        /// <summary>
        /// 轴2滑移率
        /// </summary>
        public double SlipRate2 { get; set; }

        /// <summary>
        /// 制动风缸超低
        /// </summary>
        public bool BSSRSuperLow { get; set; }

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
        public bool GateValveState { get; internal set; }
        public bool VCM_MVBConnectionState { get; internal set; }

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

        /// <summary>
        /// 主风管传感器故障
        /// </summary>
        public bool MainPipeSensorFault { get; set; }

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
    }
}
