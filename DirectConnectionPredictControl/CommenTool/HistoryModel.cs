using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class HistoryModel
    {
        public List<MainDevDataContains> Containers_1 { get; set; }
        public List<SliverDataContainer> Containers_2 { get; set; }
        public List<SliverDataContainer> Containers_3 { get; set; }
        public List<SliverDataContainer> Containers_4 { get; set; }
        public List<SliverDataContainer> Containers_5 { get; set; }
        public List<MainDevDataContains> Containers_6 { get; set; }

        public List<int> ListID { get; set; }
        public int Count { get; set; }
        public List<double> X { get; set; }
        public long FileLength { get; set; }

        public HistoryModel()
        {
            Containers_1 = new List<MainDevDataContains>();
            Containers_2 = new List<SliverDataContainer>();
            Containers_3 = new List<SliverDataContainer>();
            Containers_4 = new List<SliverDataContainer>();
            Containers_5 = new List<SliverDataContainer>();
            Containers_6 = new List<MainDevDataContains>();
            Count = 0;
            X = new List<double>();
            ListID = new List<int>();            
        }


        /// <summary>
        /// 读取指定的txt文件，返回里面内容的字符串数组
        /// </summary>
        /// <returns></returns>
        public string[] ReadTxtFiles(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string line = sr.ReadLine();
            string[] temp = line.Split('\t');
            return temp;
        }

        /// <summary>
        /// 将历史文件导出为json文件
        /// </summary>
        public void ExportToJson()
        {
            //string[] analogDataArray = ReadTxtFiles("./1架模拟量数据.txt");
            //string[] analogData_2Array = ReadTxtFiles("./2架模拟量数据.txt");
            //string[] analogData_3Array = ReadTxtFiles("./3架模拟量数据.txt");
            //string[] analogData_4Array = ReadTxtFiles("./4架模拟量数据.txt");
            //string[] analogData_5Array = ReadTxtFiles("./5架模拟量数据.txt");
            //string[] analogData_6Array = ReadTxtFiles("./6架模拟量数据.txt");
            //string[] antiskidDataArray = ReadTxtFiles("./防滑数据.txt");
            JsonData jsonData = new JsonData();
            JsonData temp = new JsonData();
            //jsonData["Select"] = new JsonData();
            //for(int i = 0; i < intArray.Length; i++)
            //{
            //    string tempStr = "name" + (i + 1);
            //    temp[tempStr] = intArray[i];
            //}
            //jsonData["Select"].Add(temp);

            jsonData["Container_1"] = new JsonData();
            for (int i = 0; i < Containers_1.Count; i++)
            {
                temp = new JsonData();
                temp["DateTime"] = Containers_1[i].dateTime.ToString("yyyy/MM/dd HH:mm:ss");
                temp["UnixTime"] = Containers_1[i].UnixHour + ":" + Containers_1[i].UnixMinute;
                temp["RefSpeed"] = Containers_1[i].RefSpeed;
                temp["BrakeLevel"] = Containers_1[i].BrakeLevel;
                temp["TrainBrakeForce"] = Containers_1[i].TrainBrakeForce;
                temp["LifeSig"] = Containers_1[i].LifeSig;
                temp["SpeedA1Shaft1"] = Containers_1[i].SpeedA1Shaft1;
                temp["SpeedA1Shaft2"] = Containers_1[i].SpeedA1Shaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_1[i].BrakeCylinderSourcePressure;
                temp["AirSpring1PressureA1Car1"] = Containers_1[i].AirSpring1PressureA1Car1;
                temp["AirSpring2PressureA1Car1"] = Containers_1[i].AirSpring2PressureA1Car1;
                temp["ParkPressureA1"] = Containers_1[i].ParkPressureA1;
                temp["VldRealPressureAx1"] = Containers_1[i].VldRealPressureAx1;
                temp["Bcp1PressureAx1"] = Containers_1[i].Bcp1PressureAx1;
                temp["Bcp2PressureAx2"] = Containers_1[i].Bcp2PressureAx2;
                temp["VldPressureSetupAx1"] = Containers_1[i].VldPressureSetupAx1;
                temp["MassA1"] = Containers_1[i].MassA1;
                temp["SelfTestSetup"] = Containers_1[i].SelfTestSetup;
                temp["VCMLifeSig"] = Containers_1[i].VCMLifeSig;
                temp["DcuLifeSig0"] = Containers_1[i].DcuLifeSig[0];
                temp["DcuLifeSig1"] = Containers_1[i].DcuLifeSig[1];
                temp["DcuLifeSig2"] = Containers_1[i].DcuLifeSig[2];
                temp["DcuLifeSig3"] = Containers_1[i].DcuLifeSig[3];
                temp["DcuEbRealValue0"] = Containers_1[i].DcuEbRealValue[0];
                temp["DcuMax0"] = Containers_1[i].DcuMax[0];
                temp["DcuEbRealValue1"] = Containers_1[i].DcuEbRealValue[1];
                temp["DcuMax1"] = Containers_1[i].DcuMax[1];
                temp["DcuEbRealValue2"] = Containers_1[i].DcuEbRealValue[2];
                temp["DcuMax2"] = Containers_1[i].DcuMax[2];
                temp["DcuEbRealValue3"] = Containers_1[i].DcuEbRealValue[3];
                temp["DcuMax3"] = Containers_1[i].DcuMax[3];
                temp["AbRealValue0"] = Containers_1[i].AbRealValue[0];
                temp["AbRealValue1"] = Containers_1[i].AbRealValue[1];
                temp["AbRealValue2"] = Containers_1[i].AbRealValue[2];
                temp["AbRealValue3"] = Containers_1[i].AbRealValue[3];
                temp["AbRealValue4"] = Containers_1[i].AbRealValue[4];
                temp["AbRealValue5"] = Containers_1[i].AbRealValue[5];
                temp["AbCapacity0"] = Containers_1[i].AbCapacity[0];
                temp["AbCapacity1"] = Containers_1[i].AbCapacity[1];
                temp["AbCapacity2"] = Containers_1[i].AbCapacity[2];
                temp["AbCapacity3"] = Containers_1[i].AbCapacity[3];
                temp["AbCapacity4"] = Containers_1[i].AbCapacity[4];
                temp["AbCapacity5"] = Containers_1[i].AbCapacity[5];
                temp["SoftwareVersionCPU"] = Containers_1[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_1[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_1[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_1[i].SlipLvl2;
                temp["AccValue1"] = Containers_1[i].AccValue1;
                temp["AccValue2"] = Containers_1[i].AccValue2;

                temp["HardEmergencyBrake"] = Containers_1[i].HardEmergencyBrake;
                temp["HardBrakeCmd"] = Containers_1[i].HardBrakeCmd;
                temp["NetBrakeCmd"] = Containers_1[i].NetBrakeCmd;
                temp["HardFastBrakeCmd"] = Containers_1[i].HardFastBrakeCmd;
                temp["NetFastBrakeCmd"] = Containers_1[i].NetFastBrakeCmd;

                temp["TrainBrakeEnable"] = Containers_1[i].TrainBrakeEnable;

                temp["EdFadeOut"] = Containers_1[i].EdFadeOut;

                jsonData["Container_1"].Add(temp);
            }

            jsonData["Container_2"] = new JsonData();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp = new JsonData();
                temp["LifeSig"] = Containers_2[i].LifeSig;
                temp["SpeedShaft1"] = Containers_2[i].SpeedShaft1;
                temp["SpeedShaft2"] = Containers_2[i].SpeedShaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_2[i].BrakeCylinderSourcePressure;
                temp["AirSpringPressure1"] = Containers_2[i].AirSpringPressure1;
                temp["AirSpringPressure2"] = Containers_2[i].AirSpringPressure2;
                temp["VldRealPressure"] = Containers_2[i].VldRealPressure;
                temp["Bcp1Pressure"] = Containers_2[i].Bcp1Pressure;
                temp["Bcp2Pressure"] = Containers_2[i].Bcp2Pressure;
                temp["VldSetupPressure"] = Containers_2[i].VldSetupPressure;
                temp["MassValue"] = Containers_2[i].MassValue;
                temp["SelfTestSetup"] = Containers_2[i].SelfTestSetup;
                temp["SoftwareVersionCPU"] = Containers_2[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_2[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_2[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_2[i].SlipLvl2;
                temp["AccValue1"] = Containers_2[i].AccValue1;
                temp["AccValue2"] = Containers_2[i].AccValue2;

                jsonData["Container_2"].Add(temp);
            }

            jsonData["Container_3"] = new JsonData();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp = new JsonData();
                temp["LifeSig"] = Containers_3[i].LifeSig;
                temp["SpeedShaft1"] = Containers_3[i].SpeedShaft1;
                temp["SpeedShaft2"] = Containers_3[i].SpeedShaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_3[i].BrakeCylinderSourcePressure;
                temp["AirSpringPressure1"] = Containers_3[i].AirSpringPressure1;
                temp["AirSpringPressure2"] = Containers_3[i].AirSpringPressure2;
                temp["ParkPressure"] = Containers_3[i].ParkPressure;
                temp["VldRealPressure"] = Containers_3[i].VldRealPressure;
                temp["Bcp1Pressure"] = Containers_3[i].Bcp1Pressure;
                temp["Bcp2Pressure"] = Containers_3[i].Bcp2Pressure;
                temp["VldSetupPressure"] = Containers_3[i].VldSetupPressure;
                temp["MassValue"] = Containers_3[i].MassValue;
                temp["SelfTestSetup"] = Containers_3[i].SelfTestSetup;
                temp["SoftwareVersionCPU"] = Containers_3[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_3[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_3[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_3[i].SlipLvl2;
                temp["AccValue1"] = Containers_3[i].AccValue1;
                temp["AccValue2"] = Containers_3[i].AccValue2;

                jsonData["Container_3"].Add(temp);
            }

            jsonData["Container_4"] = new JsonData();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp = new JsonData();
                temp["LifeSig"] = Containers_4[i].LifeSig;
                temp["SpeedShaft1"] = Containers_4[i].SpeedShaft1;
                temp["SpeedShaft2"] = Containers_4[i].SpeedShaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_4[i].BrakeCylinderSourcePressure;
                temp["AirSpringPressure1"] = Containers_4[i].AirSpringPressure1;
                temp["AirSpringPressure2"] = Containers_4[i].AirSpringPressure2;
                temp["VldRealPressure"] = Containers_4[i].VldRealPressure;
                temp["Bcp1Pressure"] = Containers_4[i].Bcp1Pressure;
                temp["Bcp2Pressure"] = Containers_4[i].Bcp2Pressure;
                temp["VldSetupPressure"] = Containers_4[i].VldSetupPressure;
                temp["MassValue"] = Containers_4[i].MassValue;
                temp["SelfTestSetup"] = Containers_4[i].SelfTestSetup;
                temp["SoftwareVersionCPU"] = Containers_4[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_4[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_4[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_4[i].SlipLvl2;
                temp["AccValue1"] = Containers_4[i].AccValue1;
                temp["AccValue2"] = Containers_4[i].AccValue2;

                jsonData["Container_4"].Add(temp);

            }

            jsonData["Container_5"] = new JsonData();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp = new JsonData();
                temp["LifeSig"] = Containers_5[i].LifeSig;
                temp["SpeedShaft1"] = Containers_5[i].SpeedShaft1;
                temp["SpeedShaft2"] = Containers_5[i].SpeedShaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_5[i].BrakeCylinderSourcePressure;
                temp["AirSpringPressure1"] = Containers_5[i].AirSpringPressure1;
                temp["AirSpringPressure2"] = Containers_5[i].AirSpringPressure2;
                temp["ParkPressure"] = Containers_5[i].ParkPressure;
                temp["VldRealPressure"] = Containers_5[i].VldRealPressure;
                temp["Bcp1Pressure"] = Containers_5[i].Bcp1Pressure;
                temp["Bcp2Pressure"] = Containers_5[i].Bcp2Pressure;
                temp["VldSetupPressure"] = Containers_5[i].VldSetupPressure;
                temp["MassValue"] = Containers_5[i].MassValue;
                temp["SelfTestSetup"] = Containers_5[i].SelfTestSetup;
                temp["SoftwareVersionCPU"] = Containers_5[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_5[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_5[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_5[i].SlipLvl2;
                temp["AccValue1"] = Containers_5[i].AccValue1;
                temp["AccValue2"] = Containers_5[i].AccValue2;

                jsonData["Container_5"].Add(temp);
            }

            jsonData["Container_6"] = new JsonData();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp = new JsonData();
                temp["RefSpeed"] = Containers_6[i].RefSpeed;
                temp["BrakeLevel"] = Containers_6[i].BrakeLevel;
                temp["TrainBrakeForce"] = Containers_6[i].TrainBrakeForce;
                temp["LifeSig"] = Containers_6[i].LifeSig;
                temp["SpeedA1Shaft1"] = Containers_6[i].SpeedA1Shaft1;
                temp["SpeedA1Shaft2"] = Containers_6[i].SpeedA1Shaft2;
                temp["BrakeCylinderSourcePressure"] = Containers_6[i].BrakeCylinderSourcePressure;
                temp["AirSpring1PressureA1Car1"] = Containers_6[i].AirSpring1PressureA1Car1;
                temp["AirSpring2PressureA1Car1"] = Containers_6[i].AirSpring2PressureA1Car1;
                temp["VldRealPressureAx1"] = Containers_6[i].VldRealPressureAx1;
                temp["Bcp1PressureAx1"] = Containers_6[i].Bcp1PressureAx1;
                temp["Bcp2PressureAx2"] = Containers_6[i].Bcp2PressureAx2;
                temp["VldPressureSetupAx1"] = Containers_6[i].VldPressureSetupAx1;
                temp["MassA1"] = Containers_6[i].MassA1;
                temp["SelfTestSetup"] = Containers_6[i].SelfTestSetup;
                temp["VCMLifeSig"] = Containers_6[i].VCMLifeSig;
                temp["DcuLifeSig0"] = Containers_6[i].DcuLifeSig[0];
                temp["DcuLifeSig1"] = Containers_6[i].DcuLifeSig[1];
                temp["DcuLifeSig2"] = Containers_6[i].DcuLifeSig[2];
                temp["DcuLifeSig3"] = Containers_6[i].DcuLifeSig[3];
                temp["DcuEbRealValue0"] = Containers_6[i].DcuEbRealValue[0];
                temp["DcuMax0"] = Containers_6[i].DcuMax[0];
                temp["DcuEbRealValue1"] = Containers_6[i].DcuEbRealValue[1];
                temp["DcuMax1"] = Containers_6[i].DcuMax[1];
                temp["DcuEbRealValue2"] = Containers_6[i].DcuEbRealValue[2];
                temp["DcuMax2"] = Containers_6[i].DcuMax[2];
                temp["DcuEbRealValue3"] = Containers_6[i].DcuEbRealValue[3];
                temp["DcuMax3"] = Containers_6[i].DcuMax[3];
                temp["AbRealValue0"] = Containers_6[i].AbRealValue[0];
                temp["AbRealValue1"] = Containers_6[i].AbRealValue[1];
                temp["AbRealValue2"] = Containers_6[i].AbRealValue[2];
                temp["AbRealValue3"] = Containers_6[i].AbRealValue[3];
                temp["AbRealValue4"] = Containers_6[i].AbRealValue[4];
                temp["AbRealValue5"] = Containers_6[i].AbRealValue[5];
                temp["AbCapacity0"] = Containers_6[i].AbCapacity[0];
                temp["AbCapacity1"] = Containers_6[i].AbCapacity[1];
                temp["AbCapacity2"] = Containers_6[i].AbCapacity[2];
                temp["AbCapacity3"] = Containers_6[i].AbCapacity[3];
                temp["AbCapacity4"] = Containers_6[i].AbCapacity[4];
                temp["AbCapacity5"] = Containers_6[i].AbCapacity[5];
                temp["SoftwareVersionCPU"] = Containers_6[i].SoftwareVersionCPU;
                temp["SoftwareVersionEP"] = Containers_6[1].SoftwareVersionEP;

                temp["SlipLvl1"] = Containers_6[i].SlipLvl1;
                temp["SlipLvl2"] = Containers_6[i].SlipLvl2;
                temp["AccValue1"] = Containers_6[i].AccValue1;
                temp["AccValue2"] = Containers_6[i].AccValue2;

                temp["HardEmergencyBrake"] = Containers_6[i].HardEmergencyBrake;
                temp["HardBrakeCmd"] = Containers_6[i].HardBrakeCmd;
                temp["NetBrakeCmd"] = Containers_6[i].NetBrakeCmd;
                temp["HardFastBrakeCmd"] = Containers_6[i].HardFastBrakeCmd;
                temp["NetFastBrakeCmd"] = Containers_6[i].NetFastBrakeCmd;

                temp["TrainBrakeEnable"] = Containers_6[i].TrainBrakeEnable;

                temp["EdFadeOut"] = Containers_6[i].EdFadeOut;

                jsonData["Container_6"].Add(temp);
            }

            string json = jsonData.ToJson();
            string path = "./Test.json";

            StreamWriter sw = new StreamWriter(path, false, Encoding.Default, 8192);
            sw.Write(json);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public object[] GetProperties()
        {
            List<object> temp = new List<object>();
            MainDevDataContains mainContainer = new MainDevDataContains();
            Type mainContainerType = mainContainer.GetType();
            foreach(System.Reflection.PropertyInfo p in mainContainerType.GetProperties())
            {
                temp.Add(p.Name);
            }
            return temp.ToArray();
        }

        #region 合成数组

        #region 滑行等级_1
        public string[] GetSlip1_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_4()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }
        #endregion
             
        #region 滑行等级_2
        public string[] GetSlip2_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6
[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }
        #endregion

        #region 各车轴一减速度
        public string[] GetAcc1_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        #endregion

        #region 各车轴二减速度
        public string[] GetAcc2_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }


        #endregion
        #endregion


    }
}
