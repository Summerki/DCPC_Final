using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class HistoryDataModel
    {
        #region column row binding data


        #region 2018-11-27：新增一组自检目标设定值
        public bool ATOHold_1 { get; set; }
        public bool ATOHold_6 { get; set; }
        public bool ATOMode_1 { get; set; }
        public bool ATOMode_6 { get; set; }
        public bool KeepStateBreak_1 { get; set; }
        public bool KeepStateBreak_6 { get; set; }

        public bool ICANFault1_1 { get; set; }
        public bool ICANFault2_1 { get; set; }
        public bool ICANFault3_1 { get; set; }
        public bool ICANFault4_1 { get; set; }
        public bool ICANFault5_1 { get; set; }
        public bool ICANFault6_1 { get; set; }
        public bool ICANFault1_2 { get; set; }
        public bool ICANFault2_2 { get; set; }
        public bool ICANFault3_2 { get; set; }
        public bool ICANFault4_2 { get; set; }
        public bool ICANFault5_2 { get; set; }
        public bool ICANFault6_2 { get; set; }

        public bool OCANFault1_1 { get; set; }
        public bool OCANFault2_1 { get; set; }
        public bool OCANFault3_1 { get; set; }
        public bool OCANFault4_1 { get; set; }
        public bool OCANFault5_1 { get; set; }
        public bool OCANFault6_1 { get; set; }
        public bool OCANFault1_2 { get; set; }
        public bool OCANFault2_2 { get; set; }
        public bool OCANFault3_2 { get; set; }
        public bool OCANFault4_2 { get; set; }
        public bool OCANFault5_2 { get; set; }
        public bool OCANFault6_2 { get; set; }


        public bool CanASPEnable_1 { get; set; }
        public bool CanASPEnable_6 { get; set; }

        public double AccValue1_1 { get; set; }
        public double AccValue2_1 { get; set; }
        public double AccValue3_1 { get; set; }
        public double AccValue4_1 { get; set; }
        public double AccValue5_1 { get; set; }
        public double AccValue6_1 { get; set; }
        public double AccValue1_2 { get; set; }
        public double AccValue2_2 { get; set; }
        public double AccValue3_2 { get; set; }
        public double AccValue4_2 { get; set; }
        public double AccValue5_2 { get; set; }
        public double AccValue6_2 { get; set; }

        public int SlipLv1_1 { get; set; }
        public int SlipLv2_1 { get; set; }
        public int SlipLv3_1 { get; set; }
        public int SlipLv4_1 { get; set; }
        public int SlipLv5_1 { get; set; }
        public int SlipLv6_1 { get; set; }
        public int SlipLv1_2 { get; set; }
        public int SlipLv2_2 { get; set; }
        public int SlipLv3_2 { get; set; }
        public int SlipLv4_2 { get; set; }
        public int SlipLv5_2 { get; set; }
        public int SlipLv6_2 { get; set; }

        public int SelfTestSetup_1 { get; set; }
        public int SelfTestSetup_2 { get; set; }
        public int SelfTestSetup_3 { get; set; }
        public int SelfTestSetup_4 { get; set; }
        public int SelfTestSetup_5 { get; set; }
        public int SelfTestSetup_6 { get; set; }

        public int SoftwareVersionCPU_1 { get; set; }
        public int SoftwareVersionCPU_2 { get; set; }
        public int SoftwareVersionCPU_3 { get; set; }
        public int SoftwareVersionCPU_4 { get; set; }
        public int SoftwareVersionCPU_5 { get; set; }
        public int SoftwareVersionCPU_6 { get; set; }

        public int SoftwareVersionEP_1 { get; set; }
        public int SoftwareVersionEP_2 { get; set; }
        public int SoftwareVersionEP_3 { get; set; }
        public int SoftwareVersionEP_4 { get; set; }
        public int SoftwareVersionEP_5 { get; set; }
        public int SoftwareVersionEP_6 { get; set; }
        #endregion
        public int ID { get; set; }
        public int LifeSig_1 { get; set; }
        public int LifeSig_2 { get; set; }
        public int LifeSig_3 { get; set; }
        public int LifeSig_4 { get; set; }
        public int LifeSig_5 { get; set; }
        public int LifeSig_6 { get; set; }
        public string dateTime { get; set; }
        public string UnixTime { get; set; }
        public double RefSpeed_1 { get; set; }
        public double RefSpeed_6 { get; set; }
        public string Mode_1 { get; set; }
        public string Mode_6 { get; set; }
        public bool BrakeCmd_1 { get; set; }
        public bool BrakeCmd_6 { get; set; }
        public bool DriveCmd_1 { get; set; }
        public bool DriveCmd_6 { get; set; }
        public bool FastBrakeCmd_1 { get; set; }
        public bool FastBrakeCmd_6 { get; set; }
        public bool LazyCmd_1 { get; set; }
        public bool LazyCmd_6 { get; set; }
        public bool EmergencyBrakeCmd_1 { get; set; }
        public bool EmergencyBrakeCmd_6 { get; set; }
        public bool HoldBrakeRealease { get; set; }
        public bool LazyState_1 { get; set; }
        public bool LazyState_6 { get; set; }
        public bool DriveState_1 { set; get; }
        public bool DriveState_6 { set; get; }
        public bool NormalBrakeState_1 { get; set; }
        public bool NormalBrakeState_6 { get; set; }
        public bool EmergencyBrakeState_1 { get; set; }
        public bool EmergencyBrakeState_6 { get; set; }
        public bool ZeroSpeed_1 { get; set; }
        public bool ZeroSpeed_6 { get; set; }
        public int BrakeLevel_1 { get; set; }
        public int BrakeLevel_6 { get; set; }
        public int TrainBrakeForce_1 { get; set; }
        public int TrainBrakeForce_6 { get; set; }
        public double SpeedAx1_1 { get; set; }
        public double SpeedAx2_1 { get; set; }
        public double SpeedAx1_2 { get; set; }
        public double SpeedAx2_2 { get; set; }
        public double SpeedAx1_3 { get; set; }
        public double SpeedAx2_3 { get; set; }
        public double SpeedAx1_4 { get; set; }
        public double SpeedAx2_4 { get; set; }
        public double SpeedAx1_5 { get; set; }
        public double SpeedAx2_5 { get; set; }
        public double SpeedAx1_6 { get; set; }
        public double SpeedAx2_6 { get; set; }
        public bool VCM2MVBState_1 { get; set; }
        public bool VCM2MVBState_2 { get; set; }
        public bool Slip_1 { get; set; }
        public bool Slip_2 { get; set; }
        public bool Slip_3 { get; set; }
        public bool Slip_4 { get; set; }
        public bool Slip_5 { get; set; }
        public bool Slip_6 { get; set; }
        public bool EmergencyBrakeActive_1 { get; set; }
        public bool EmergencyBrakeActive_2 { get; set; }
        public bool EmergencyBrakeActive_3 { get; set; }
        public bool EmergencyBrakeActive_4 { get; set; }
        public bool EmergencyBrakeActive_5 { get; set; }
        public bool EmergencyBrakeActive_6 { get; set; }
        public bool NotZeroSpeed { get; set; }
        public bool AbActive_1 { get; set; }
        public bool AbActive_2 { get; set; }
        public bool AbActive_3 { get; set; }
        public bool AbActive_4 { get; set; }
        public bool AbActive_5 { get; set; }
        public bool AbActive_6 { get; set; }
        public bool BSRLow_1 { get; set; }
        public bool BSRLow_2 { get; set; }
        public bool BSRLow_3 { get; set; }
        public bool BSRLow_4 { get; set; }
        public bool BSRLow_5 { get; set; }
        public bool BSRLow_6 { get; set; }
        public bool ParkBreakRealease_1 { get; set; }
        public bool ParkBreakRealease_2 { get; set; }
        public bool ParkBreakRealease_3 { get; set; }
        public bool ParkBreakRealease_4 { get; set; }
        public bool ParkBreakRealease_5 { get; set; }
        public bool ParkBreakRealease_6 { get; set; }
        public bool AbStatues_1 { get; set; }
        public bool AbStatues_2 { get; set; }
        public bool AbStatues_3 { get; set; }
        public bool AbStatues_4 { get; set; }
        public bool AbStatues_5 { get; set; }
        public bool AbStatues_6 { get; set; }
        public bool MassValid_1 { get; set; }
        public bool MassValid_2 { get; set; }
        public bool MassValid_3 { get; set; }
        public bool MassValid_4 { get; set; }
        public bool MassValid_5 { get; set; }
        public bool MassValid_6 { get; set; }
        public int AbTargetValue_1 { get; set; }
        public int AbTargetValue_2 { get; set; }
        public int AbTargetValue_3 { get; set; }
        public int AbTargetValue_4 { get; set; }
        public int AbTargetValue_5 { get; set; }
        public int AbTargetValue_6 { get; set; }
        public bool SelfInt_1 { get; set; }
        public bool SelfInt_6 { get; set; }
        public bool SelfActive_1 { get; set; }
        public bool SelfActive_6 { get; set; }
        public bool SelfSuccess_1 { get; set; }
        public bool SelfSuccess_6 { get; set; }
        public bool SelfFail_1 { get; set; }
        public bool SelfFail_6 { get; set; }
        public bool UnTest24_1 { get; set; }
        public bool UnTest24_6 { get; set; }
        public bool UnTest26_1 { get; set; }
        public bool UnTest26_6 { get; set; }
        public bool GatewayValveState_1 { get; set; }
        public bool GatewayValveState_6 { get; set; }
        public bool HardDrive_1 { get; set; }
        public bool HardDrive_6 { get; set; }
        public bool HardBrake { get; set; }
        public bool HardFastBrake_1 { get; set; }
        public bool HardFastBrake_6 { get; set; }
        public bool HardEmergencyBrake_1 { get; set; }
        public bool HardEmergencyBrake_6 { get; set; }
        public bool HardEmergencyDrive_1 { get; set; }
        public bool HardEmergencyDrive_6 { get; set; }
        public bool CanUnitTestOn_1 { get; set; }
        public bool CanUnitTestOn_6 { get; set; }
        public bool CanValveActive_1 { get; set; }
        public bool CanValveActive_6 { get; set; }
        public bool CanUnitTestOff_1 { get; set; }
        public bool CanUnitTestOff_6 { get; set; }
        public bool NetDrive_1 { get; set; }
        public bool NetDrive_6 { get; set; }
        public bool NetBrake_1 { get; set; }
        public bool NetBrake_6 { get; set; }
        public bool NetFastBrake { get; set; }
        public bool TowingMode_1 { get; set; }
        public bool TowingMode_6 { get; set; }
        public bool KeepBrakeRelease_1 { get; set; }
        public bool KeepBrakeRelease_6 { get; set; }
        public bool CanTestA_1 { get; set; }
        public bool CanTestA_6 { get; set; }
        public bool CanTestB_1 { get; set; }
        public bool CanTestB_6 { get; set; }
        public bool BrakeLevelActive_1 { get; set; }
        public bool BrakeLevelActive_6 { get; set; }
        public bool SelfTestCmd_1 { get; set; }
        public bool SelfTestCmd_6 { get; set; }
        public bool TrainBrakeEnable_1 { get; set; }
        public bool TrainBrakeEnable_6 { get; set; }
        public bool AbFadeOut_1 { get; set; }
        public bool AbFadeOut_6 { get; set; }
        public bool EDoutB_1 { get; set; }
        public bool EDoutB_6 { get; set; }
        public bool EDoutC_1 { get; set; }
        public bool EDoutC_6 { get; set; }
        public bool WheelInputState { get; set; }
        public int BrakeCylinderSourcePressure_1 { get; set; }
        public int BrakeCylinderSourcePressure_2 { get; set; }
        public int BrakeCylinderSourcePressure_3 { get; set; }
        public int BrakeCylinderSourcePressure_4 { get; set; }
        public int BrakeCylinderSourcePressure_5 { get; set; }
        public int BrakeCylinderSourcePressure_6 { get; set; }
        public int AirSpringPressure1_1 { get; set; }
        public int AirSpringPressure1_2 { get; set; }
        public int AirSpringPressure1_3 { get; set; }
        public int AirSpringPressure1_4 { get; set; }
        public int AirSpringPressure1_5 { get; set; }
        public int AirSpringPressure1_6 { get; set; }
        public int AirSpringPressure2_1 { get; set; }
        public int AirSpringPressure2_2 { get; set; }
        public int AirSpringPressure2_3 { get; set; }
        public int AirSpringPressure2_4 { get; set; }
        public int AirSpringPressure2_5 { get; set; }
        public int AirSpringPressure2_6 { get; set; }
        public int ParkPressure_1 { get; set; }
        public int ParkPressure_2 { get; set; }
        public int ParkPressure_3 { get; set; }
        public int ParkPressure_4 { get; set; }
        public int VldRealPressure_1 { get; set; }
        public int VldRealPressure_2 { get; set; }
        public int VldRealPressure_3 { get; set; }
        public int VldRealPressure_4 { get; set; }
        public int VldRealPressure_5 { get; set; }
        public int VldRealPressure_6 { get; set; }
        public int Bcp1Pressure_1 { get; set; }
        public int Bcp2Pressure_1 { get; set; }
        public int Bcp1Pressure_6 { get; set; }
        public int Bcp2Pressure_6 { get; set; }
        public int Bcp1Pressure_2 { get; set; }
        public int Bcp2Pressure_2 { get; set; }
        public int Bcp1Pressure_3 { get; set; }
        public int Bcp2Pressure_3 { get; set; }
        public int Bcp1Pressure_4 { get; set; }
        public int Bcp2Pressure_4 { get; set; }
        public int Bcp1Pressure_5 { get; set; }
        public int Bcp2Pressure_5 { get; set; }
        public bool BSSRSenorFault_1 { get; set; }
        public bool BSSRSenorFault_2 { get; set; }
        public bool BSSRSenorFault_3 { get; set; }
        public bool BSSRSenorFault_4 { get; set; }
        public bool BSSRSenorFault_5 { get; set; }
        public bool BSSRSenorFault_6 { get; set; }
        public bool AirSpringSenorFault1_1 { get; set; }
        public bool AirSpringSenorFault2_1 { get; set; }
        public bool AirSpringSenorFault1_2 { get; set; }
        public bool AirSpringSenorFault2_2 { get; set; }
        public bool AirSpringSenorFault1_3 { get; set; }
        public bool AirSpringSenorFault2_3 { get; set; }
        public bool AirSpringSenorFault1_4 { get; set; }
        public bool AirSpringSenorFault2_4 { get; set; }
        public bool AirSpringSenorFault1_5 { get; set; }
        public bool AirSpringSenorFault2_5 { get; set; }
        public bool AirSpringSenorFault1_6 { get; set; }
        public bool AirSpringSenorFault2_6 { get; set; }
        public bool ParkCylinderSenorFault_1 { get; set; }
        public bool ParkCylinderSenorFault_2 { get; set; }
        public bool ParkCylinderSenorFault_3 { get; set; }
        public bool ParkCylinderSenorFault_4 { get; set; }
        public bool ParkCylinderSenorFault_5 { get; set; }
        public bool ParkCylinderSenorFault_6 { get; set; }
        public bool VLDSensorFault_1 { get; set; }
        public bool VLDSensorFault_2 { get; set; }
        public bool VLDSensorFault_3 { get; set; }
        public bool VLDSensorFault_4 { get; set; }
        public bool VLDSensorFault_5 { get; set; }
        public bool VLDSensorFault_6 { get; set; }
        public bool BSRSenorFault1_1 { get; set; }
        public bool BSRSenorFault2_1 { get; set; }
        public bool BSRSenorFault1_2 { get; set; }
        public bool BSRSenorFault2_2 { get; set; }
        public bool BSRSenorFault1_3 { get; set; }
        public bool BSRSenorFault2_3 { get; set; }
        public bool BSRSenorFault1_4 { get; set; }
        public bool BSRSenorFault2_4 { get; set; }
        public bool BSRSenorFault1_5 { get; set; }
        public bool BSRSenorFault2_5 { get; set; }
        public bool BSRSenorFault1_6 { get; set; }
        public bool BSRSenorFault2_6 { get; set; }
        public bool AirSpringOverflow1_1 { get; set; }
        public bool AirSpringOverflow2_1 { get; set; }
        public bool AirSpringOverflow1_2 { get; set; }
        public bool AirSpringOverflow2_2 { get; set; }
        public bool AirSpringOverflow1_3 { get; set; }
        public bool AirSpringOverflow2_3 { get; set; }
        public bool AirSpringOverflow1_4 { get; set; }
        public bool AirSpringOverflow2_4 { get; set; }
        public bool AirSpringOverflow1_5 { get; set; }
        public bool AirSpringOverflow2_5 { get; set; }
        public bool AirSpringOverflow1_6 { get; set; }
        public bool AirSpringOverflow2_6 { get; set; }
        public int VldPressureSetup_1 { get; set; }
        public int VldPressureSetup_2 { get; set; }
        public int VldPressureSetup_3 { get; set; }
        public int VldPressureSetup_4 { get; set; }
        public int VldPressureSetup_5 { get; set; }
        public int VldPressureSetup_6 { get; set; }
        public int Mass_1 { get; set; }
        public int Mass_2 { get; set; }
        public int Mass_3 { get; set; }
        public int Mass_4 { get; set; }
        public int Mass_5 { get; set; }
        public int Mass_6 { get; set; }
        public bool BCUFail_Serious_1 { get; set; }
        public bool BCUFail_Middle_1 { get; set; }
        public bool BCUFail_Slight_1 { get; set; }
        public bool BCUFail_Serious_2 { get; set; }
        public bool BCUFail_Middle_2 { get; set; }
        public bool BCUFail_Slight_2 { get; set; }
        public bool BCUFail_Serious_3 { get; set; }
        public bool BCUFail_Middle_3 { get; set; }
        public bool BCUFail_Slight_3 { get; set; }
        public bool BCUFail_Serious_4 { get; set; }
        public bool BCUFail_Middle_4 { get; set; }
        public bool BCUFail_Slight_4 { get; set; }
        public bool BCUFail_Serious_5 { get; set; }
        public bool BCUFail_Middle_5 { get; set; }
        public bool BCUFail_Slight_5 { get; set; }
        public bool BCUFail_Serious_6 { get; set; }
        public bool BCUFail_Middle_6 { get; set; }
        public bool BCUFail_Slight_6 { get; set; }
        public bool EmergencyBrakeFault_1 { get; set; }
        public bool EmergencyBrakeFault_2 { get; set; }
        public bool EmergencyBrakeFault_3 { get; set; }
        public bool EmergencyBrakeFault_4 { get; set; }
        public bool EmergencyBrakeFault_5 { get; set; }
        public bool EmergencyBrakeFault_6 { get; set; }
        public bool SpeedSenorFault1_1 { get; set; }
        public bool SpeedSenorFault2_1 { get; set; }
        public bool SpeedSenorFault1_2 { get; set; }
        public bool SpeedSenorFault2_2 { get; set; }
        public bool SpeedSenorFault1_3 { get; set; }
        public bool SpeedSenorFault2_3 { get; set; }
        public bool SpeedSenorFault1_4 { get; set; }
        public bool SpeedSenorFault2_4 { get; set; }
        public bool SpeedSenorFault1_5 { get; set; }
        public bool SpeedSenorFault2_5 { get; set; }
        public bool SpeedSenorFault1_6 { get; set; }
        public bool SpeedSenorFault2_6 { get; set; }
        public bool WSPFault1_1 { get; set; }
        public bool WSPFault2_1 { get; set; }
        public bool WSPFault1_2 { get; set; }
        public bool WSPFault2_2 { get; set; }
        public bool WSPFault1_3 { get; set; }
        public bool WSPFault2_3 { get; set; }
        public bool WSPFault1_4 { get; set; }
        public bool WSPFault2_4 { get; set; }
        public bool WSPFault1_5 { get; set; }
        public bool WSPFault2_5 { get; set; }
        public bool WSPFault1_6 { get; set; }
        public bool WSPFault2_6 { get; set; }
        public bool CodeConnectorFault_1 { get; set; }
        public bool CodeConnectorFault_2 { get; set; }
        public bool CodeConnectorFault_3 { get; set; }
        public bool CodeConnectorFault_4 { get; set; }
        public bool CodeConnectorFault_5 { get; set; }
        public bool CodeConnectorFault_6 { get; set; }
        public bool AirSpringLimit_1 { get; set; }
        public bool AirSpringLimit_2 { get; set; }
        public bool AirSpringLimit_3 { get; set; }
        public bool AirSpringLimit_4 { get; set; }
        public bool AirSpringLimit_5 { get; set; }
        public bool AirSpringLimit_6 { get; set; }
        public bool BrakeNotRealease_1 { get; set; }
        public bool BrakeNotRealease_2 { get; set; }
        public bool BrakeNotRealease_3 { get; set; }
        public bool BrakeNotRealease_4 { get; set; }
        public bool BrakeNotRealease_5 { get; set; }
        public bool BrakeNotRealease_6 { get; set; }
        public bool BCPLow_1 { get; set; }
        public bool BCPLow_2 { get; set; }
        public bool BCPLow_3 { get; set; }
        public bool BCPLow_4 { get; set; }
        public bool BCPLow_5 { get; set; }
        public bool BCPLow_6 { get; set; }
        public int VCMLifeSig_1 { get; set; }
        public int VCMLifeSig_6 { get; set; }
        public int DCULifeSig1_1 { get; set; }
        public int DCULifeSig1_2 { get; set; }
        public int DCULifeSig1_3 { get; set; }
        public int DCULifeSig1_4 { get; set; }
        public int DCULifeSig6_1 { get; set; }
        public int DCULifeSig6_2 { get; set; }
        public int DCULifeSig6_3 { get; set; }
        public int DCULifeSig6_4 { get; set; }
        public bool DCU_Ed_Ok_1_1 { get; set; }
        public bool DCU_Ed_Ok_1_6 { get; set; }
        public bool DCU_Ed_Fadeout_1_1 { get; set; }
        public bool DCU_Ed_Fadeout_1_6 { get; set; }
        public bool DCU_Ed_Slip_1_1 { get; set; }
        public bool DCU_Ed_Slip_1_6 { get; set; }
        public bool DCU_Ed_Ok_2_1 { get; set; }
        public bool DCU_Ed_Ok_2_6 { get; set; }
        public bool DCU_Ed_Fadeout_2_1 { get; set; }
        public bool DCU_Ed_Fadeout_2_6 { get; set; }
        public bool DCU_Ed_Slip_2_1 { get; set; }
        public bool DCU_Ed_Slip_2_6 { get; set; }
        public bool DCU_Ed_Ok_3_1 { get; set; }
        public bool DCU_Ed_Ok_3_6 { get; set; }
        public bool DCU_Ed_Fadeout_3_1 { get; set; }
        public bool DCU_Ed_Fadeout_3_6 { get; set; }
        public bool DCU_Ed_Slip_3_1 { get; set; }
        public bool DCU_Ed_Slip_3_6 { get; set; }
        public bool DCU_Ed_Ok_4_1 { get; set; }
        public bool DCU_Ed_Ok_4_6 { get; set; }
        public bool DCU_Ed_Fadeout_4_1 { get; set; }
        public bool DCU_Ed_Fadeout_4_6 { get; set; }
        public bool DCU_Ed_Slip_4_1 { get; set; }
        public bool DCU_Ed_Slip_4_6 { get; set; }
        public int DcuEbRealValue1_1 { get; set; }
        public int DcuEbRealValue1_2 { get; set; }
        public int DcuEbRealValue1_3 { get; set; }
        public int DcuEbRealValue1_4 { get; set; }
        public int DcuEbRealValue6_1 { get; set; }
        public int DcuEbRealValue6_2 { get; set; }
        public int DcuEbRealValue6_3 { get; set; }
        public int DcuEbRealValue6_4 { get; set; }
        public int DcuMax1_1 { get; set; }
        public int DcuMax1_2 { get; set; }
        public int DcuMax1_3 { get; set; }
        public int DcuMax1_4 { get; set; }
        public int DcuMax6_1 { get; set; }
        public int DcuMax6_2 { get; set; }
        public int DcuMax6_3 { get; set; }
        public int DcuMax6_4 { get; set; }
        public int AbCapacity1_1 { get; set; }
        public int AbCapacity1_2 { get; set; }
        public int AbCapacity1_3 { get; set; }
        public int AbCapacity1_4 { get; set; }
        public int AbCapacity1_5 { get; set; }
        public int AbCapacity1_6 { get; set; }
        public int AbCapacity6_1 { get; set; }
        public int AbCapacity6_2 { get; set; }
        public int AbCapacity6_3 { get; set; }
        public int AbCapacity6_4 { get; set; }
        public int AbCapacity6_5 { get; set; }
        public int AbCapacity6_6 { get; set; }
        public int AbRealValue1_1 { get; set; }
        public int AbRealValue1_2 { get; set; }
        public int AbRealValue1_3 { get; set; }
        public int AbRealValue1_4 { get; set; }
        public int AbRealValue1_5 { get; set; }
        public int AbRealValue1_6 { get; set; }
        public int AbRealValue6_1 { get; set; }
        public int AbRealValue6_2 { get; set; }
        public int AbRealValue6_3 { get; set; }
        public int AbRealValue6_4 { get; set; }
        public int AbRealValue6_5 { get; set; }
        public int AbRealValue6_6 { get; set; }
        public bool SpeedDetection_1 { get; set; }
        public bool SpeedDetection_2 { get; set; }
        public bool SpeedDetection_3 { get; set; }
        public bool SpeedDetection_4 { get; set; }
        public bool SpeedDetection_5 { get; set; }
        public bool SpeedDetection_6 { get; set; }
        public bool CanBusFail1_1 { get; set; }
        public bool CanBusFail6_1 { get; set; }
        public bool CanBusFail1_2 { get; set; }
        public bool CanBusFail6_2 { get; set; }
        public bool HardDifferent { get; set; }
        public bool Event_High_1 { get; set; }
        public bool Event_High_6 { get; set; }
        public bool Event_Middle_1 { get; set; }
        public bool Event_Middle_6 { get; set; }
        public bool Event_Low_1 { get; set; }
        public bool Event_Low_6 { get; set; }
        public bool CanASPActive { get; set; }
        public bool BCPLowA_1 { get; set; }
        public bool BCPLowA_6 { get; set; }
        public bool BCPLowB_1 { get; set; }
        public bool BCPLowB_6 { get; set; }
        public bool BCPLowC_1 { get; set; }
        public bool BCPLowC_6 { get; set; }
        public int SoftVersion { get; set; }

        /// <summary>
        /// 1架轴1排风故障
        /// </summary>
        public bool exhaustFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1排风故障
        /// </summary>
        public bool exhaustFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1排风故障
        /// </summary>
        public bool exhaustFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1排风故障
        /// </summary>
        public bool exhaustFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1排风故障
        /// </summary>
        public bool exhaustFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1排风故障
        /// </summary>
        public bool exhaustFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2排风故障
        /// </summary>
        public bool exhuastFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2排风故障
        /// </summary>
        public bool exhuastFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2排风故障
        /// </summary>
        public bool exhuastFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2排风故障
        /// </summary>
        public bool exhuastFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2排风故障
        /// </summary>
        public bool exhuastFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2排风故障
        /// </summary>
        public bool exhuastFault6_2 { get; set; }

        /// <summary>
        /// 1架轴1保压故障
        /// </summary>
        public bool keepPressureFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1保压故障
        /// </summary>
        public bool keepPressureFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1保压故障
        /// </summary>
        public bool keepPressureFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1保压故障
        /// </summary>
        public bool keepPressureFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1保压故障
        /// </summary>
        public bool keepPressureFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1保压故障
        /// </summary>
        public bool keepPressureFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2保压故障
        /// </summary>
        public bool keepPressureFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2保压故障
        /// </summary>
        public bool keepPressureFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2保压故障
        /// </summary>
        public bool keepPressureFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2保压故障
        /// </summary>
        public bool keepPressureFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2保压故障
        /// </summary>
        public bool keepPressureFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2保压故障
        /// </summary>
        public bool keepPressureFault6_2 { get; set; }

        /// <summary>
        /// 1架轴1充风故障
        /// </summary>
        public bool chargingFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1充风故障
        /// </summary>
        public bool chargingFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1充风故障
        /// </summary>
        public bool chargingFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1充风故障
        /// </summary>
        public bool chargingFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1充风故障
        /// </summary>
        public bool chargingFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1充风故障
        /// </summary>
        public bool chargingFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2充风故障
        /// </summary>
        public bool chargingFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2充风故障
        /// </summary>
        public bool chargingFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2充风故障
        /// </summary>
        public bool chargingFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2充风故障
        /// </summary>
        public bool chargingFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2充风故障
        /// </summary>
        public bool chargingFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2充风故障
        /// </summary>
        public bool chargingFault6_2 { get; set; }

        /// <summary>
        /// 1架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_1 { get; set; }
        /// <summary>
        /// 2架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_2 { get; set; }
        /// <summary>
        /// 3架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_3 { get; set; }
        /// <summary>
        /// 4架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_4 { get; set; }
        /// <summary>
        /// 5架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_5 { get; set; }
        /// <summary>
        /// 6架连接阀充风故障
        /// </summary>
        public bool connectionValveAirFillingFailure_6 { get; set; }

        /// <summary>
        /// 1架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_1 { get; set; }
        /// <summary>
        /// 2架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_2 { get; set; }
        /// <summary>
        /// 3架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_3 { get; set; }
        /// <summary>
        /// 4架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_4 { get; set; }
        /// <summary>
        /// 5架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_5 { get; set; }
        /// <summary>
        /// 6架连接阀排风故障
        /// </summary>
        public bool connectionValveVentilationFailure_6 { get; set; }

        /// <summary>
        /// 1架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_1 { get; set; }
        /// <summary>
        /// 2架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_2 { get; set; }
        /// <summary>
        /// 3架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_3 { get; set; }
        /// <summary>
        /// 4架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_4 { get; set; }
        /// <summary>
        /// 5架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_5 { get; set; }
        /// <summary>
        /// 6架VLD充风故障
        /// </summary>
        public bool VLDChargingFault_6 { get; set; }

        /// <summary>
        /// 1架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_1 { get; set; }
        /// <summary>
        /// 2架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_2 { get; set; }
        /// <summary>
        /// 3架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_3 { get; set; }
        /// <summary>
        /// 4架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_4 { get; set; }
        /// <summary>
        /// 5架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_5 { get; set; }
        /// <summary>
        /// 6架VLD排风故障
        /// </summary>
        public bool VLDExhuastFault_6 { get; set; }

        /// <summary>
        /// 1架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_1 { get; set; }
        /// <summary>
        /// 2架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_2 { get; set; }
        /// <summary>
        /// 3架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_3 { get; set; }
        /// <summary>
        /// 4架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_4 { get; set; }
        /// <summary>
        /// 5架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_5 { get; set; }
        /// <summary>
        /// 6架VLD保压故障
        /// </summary>
        public bool VLDKeepPressureFault_6 { get; set; }

        /// <summary>
        /// 1架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1WSP排风故障
        /// </summary>
        public bool WSPExhuastFault6_1 { get; set; }
        

        /// <summary>
        /// 1架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1WSP充风故障
        /// </summary>
        public bool WSPChargingFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2排风故障
        /// </summary>
        public bool WSPExhuastFault6_2 { get; set; }

        /// <summary>
        /// 1架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2WSP充风故障
        /// </summary>
        public bool WSPChargingFault6_2 { get; set; }

        /// <summary>
        /// 1架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1WSP连续排风超时故障
        /// </summary>
        public bool WSPContinueExaustAirTimeOutFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure1_2 { get; set; }
        /// <summary>
        /// 2架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure2_2 { get; set; }
        /// <summary>
        /// 3架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure3_2 { get; set; }
        /// <summary>
        /// 4架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure4_2 { get; set; }
        /// <summary>
        /// 5架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure5_2 { get; set; }
        /// <summary>
        /// 6架轴2WSP连续排风故障
        /// </summary>
        public bool WSPContinuousExhaustFailure6_2 { get; set; }

        /// <summary>
        /// 1架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault1_1 { get; set; }
        /// <summary>
        /// 2架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault2_1 { get; set; }
        /// <summary>
        /// 3架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault3_1 { get; set; }
        /// <summary>
        /// 4架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault4_1 { get; set; }
        /// <summary>
        /// 5架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault5_1 { get; set; }
        /// <summary>
        /// 6架轴1WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault6_1 { get; set; }

        /// <summary>
        /// 1架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault1_2 { get; set; }
        /// <summary>
        /// 2架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault2_2 { get; set; }
        /// <summary>
        /// 3架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault3_2 { get; set; }
        /// <summary>
        /// 4架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault4_2 { get; set; }
        /// <summary>
        /// 5架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault5_2 { get; set; }
        /// <summary>
        /// 6架轴2WSP连续保压超时故障
        /// </summary>
        public bool WSPContinueKeepPressureTimeOutFault6_2 { get; set; }

        public int AirBrakeTargetValueBy1_1 { get; set; }
        public int AirBrakeTargetValueBy1_2 { get; set; }
        public int AirBrakeTargetValueBy1_3 { get; set; }
        public int AirBrakeTargetValueBy1_4 { get; set; }
        public int AirBrakeTargetValueBy1_5 { get; set; }
        public int AirBrakeTargetValueBy1_6 { get; set; }
        public int AirBrakeTargetValueBy6_1 { get; set; }
        public int AirBrakeTargetValueBy6_2 { get; set; }
        public int AirBrakeTargetValueBy6_3 { get; set; }
        public int AirBrakeTargetValueBy6_4 { get; set; }
        public int AirBrakeTargetValueBy6_5 { get; set; }
        public int AirBrakeTargetValueBy6_6 { get; set; }

        #endregion
    }
}
