using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DirectConnectionPredictControl.CommenTool;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// OverviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OverviewWindowHis : Window
    {
        /// <summary>
        /// 数据组
        /// </summary>
        private MainDevDataContains container_1 = new MainDevDataContains();
        private MainDevDataContains container_6 = new MainDevDataContains();
        private SliverDataContainer container_2 = new SliverDataContainer();
        private SliverDataContainer container_3 = new SliverDataContainer();
        private SliverDataContainer container_4 = new SliverDataContainer();
        private SliverDataContainer container_5 = new SliverDataContainer();

        private int[] stepArray = new int[] { 1, 5, 10, 20, 50, 100, 200, 500, 1000, 5000 };
        private bool init = false;

        //存储comboBox数据的内部类
        private class Step
        {
            public string ID { get; set; }
            public string Number { get; set; }
        }

        private HistoryModel history;
        private int totalRecord;
        private int nowRecord = 1;
        private int step = 1;

        /// <summary>
        /// 线程组
        /// </summary>
        private Thread uiThread;

        public OverviewWindowHis()
        {
            InitializeComponent();
            Init();
        }

        public OverviewWindowHis(HistoryModel history)
        {
            this.history = history;
            InitializeComponent();

            Init();
            init = true;
        }

        private void Init()
        {
            stepCbx.comboBox.SelectionChanged += StepChanged;
            totalRecord = history.Count;
            ObservableCollection<Step> stepData = new ObservableCollection<Step>();
            for (int i = 0; i < stepArray.Length; i++)
            {
                stepData.Add(new Step()
                {
                    ID = "1",
                    Number = stepArray[i] + "",
                });
            }
            stepCbx.comboBox.ItemsSource = stepData;
            recordPointerLbl.Content = nowRecord + " / " + totalRecord;
            StepTo(1);
            hisProgress.Maximum = totalRecord;
            //ContextMenu contex = new ContextMenu();
            //MenuItem item = new MenuItem();
            //item.Header = "跳转详细数据";
            //item.Click += EatShit;
            //contex.Items.Add(item);
            //this.ContextMenu = contex;
        }

        //private void EatShit(object sender, RoutedEventArgs e)
        //{
        //    HistoryDetail historyDetail = new HistoryDetail();
        //    historyDetail.SetHistory(history);
        //    historyDetail.Show();
        //}

        private void StepChanged(object sender, SelectionChangedEventArgs e)
        {
            step = stepArray[stepCbx.comboBox.SelectedIndex];
        }

        private void StepTo(int step)
        {
            hisProgress.Value = nowRecord;
            recordPointerLbl.Content = nowRecord + " / " + totalRecord + " ( " + string.Format("{0:N2}", ((double)nowRecord / (double)totalRecord) * 100) + " % )";
            container_1 = history.Containers_1[step - 1];
            container_2 = history.Containers_2[step - 1];
            container_3 = history.Containers_3[step - 1];
            container_4 = history.Containers_4[step - 1];
            container_5 = history.Containers_5[step - 1];
            container_6 = history.Containers_6[step - 1];
            currentTimeLbl.Content = history.Containers_1[step - 1].dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UpdateAO();
            UpdateDI();
            UpdateDO();
            UpdateFA();
            UpdateWSP();

            UpdateBrakingDistance(); // 2020-1-7
        }
        #region bussiness method

        // 2020-1-7
        private Queue<double> brakeDistanceQueue = new Queue<double>(); // 创建一个制动距离的队列
        private double brakeDistance = 0; // 初始制动距离为0
        private bool firstGetBrakeCmd = true;  // 判断是否是首次收到制动指令
        private double firstBrakeSpeed = 0; // 首次制动指令时的速度
        private void UpdateBrakingDistance()
        {
            // 利用1车（主车）的网关阀状态来判断此时应该用谁的参考速度
            if (container_1.GateValveState)  // 如果1车的网关阀状态为true，用车1的参考速度
            {
                if (container_1.RefSpeed <= 0.5) // 当1车的参考速度小于等于0.5km/h时才开始计算制动距离
                {
                    if (brakeDistanceQueue.Count > 0)
                    {
                        brakeDistance = 0;
                        for (int i = 0; i < brakeDistanceQueue.Count; i++)
                        {
                            brakeDistance += brakeDistanceQueue.Dequeue() * 0.0000278;  // 累加到制动距离上；100ms=0.0000278h  因为参考速度的单位是km/h
                        }
                        brake_distance.Content = (brakeDistance) + "km";
                        average_reduce_speed.Content = ((firstBrakeSpeed * firstBrakeSpeed) / (2 * brakeDistance)); // 根据公式计算出平均减速度
                    }
                    firstGetBrakeCmd = true;
                }
                // 下面这些是协议里说的 “制动指令”=“制动（硬线）”|“快速制动（硬线）”|“制动（网络）”|“快速制动（网络）”|“紧急制动”
                if (container_1.NetBrakeCmd || container_1.NetFastBrakeCmd || container_1.HardBrakeCmd || container_1.HardFastBrakeCmd || container_1.HardEmergencyBrake)
                {
                    if (firstGetBrakeCmd) // 首次出现制动指令
                    {
                        firstBrakeSpeed = container_1.RefSpeed; // 制动开始时的初速度
                        firstGetBrakeCmd = false; // 再将首次制动置为false
                    }
                    brake_distance.Content = "制动中";
                    brakeDistanceQueue.Enqueue(container_1.RefSpeed); // 当出现以上制动条件时，开始将1车的参考速度入队列
                }
                // 复位
                // 对应协议里的 当V[ref]大于等于5km/h,同时“牵引”有效，无“制动指令”，S值复位。
                if (container_1.RefSpeed >= 5 && !container_1.NetBrakeCmd && !container_1.NetFastBrakeCmd && !container_1.HardBrakeCmd && !container_1.HardFastBrakeCmd && !container_1.HardEmergencyBrake)
                {
                    brake_distance.Content = "0.00";
                    average_reduce_speed.Content = "0.00";
                    firstGetBrakeCmd = true; // 复位代表制动结束，再将首次制动置为true，等待下一次制动
                }
            }
            else  // 此时车6为主车，用车6的参考速度，下面的逻辑只是将1车换成了6车的，同理
            {
                if (container_6.RefSpeed <= 0.5)
                {
                    if (brakeDistanceQueue.Count > 0)
                    {
                        brakeDistance = 0;
                        for (int i = 0; i < brakeDistanceQueue.Count; i++)
                        {
                            brakeDistance += brakeDistanceQueue.Dequeue() * 0.0000278;  // 100ms=0.0000278h  因为参考速度的单位是km/h
                        }
                        brake_distance.Content = (brakeDistance / 1000) + "km";
                        average_reduce_speed.Content = ((firstBrakeSpeed * firstBrakeSpeed) / (2 * brakeDistance)) / 1000 / 1000;
                    }
                    firstGetBrakeCmd = true;
                }
                if (container_6.NetBrakeCmd || container_6.NetFastBrakeCmd || container_6.HardBrakeCmd || container_6.HardFastBrakeCmd || container_6.HardEmergencyBrake)
                {
                    if (firstGetBrakeCmd)
                    {
                        firstBrakeSpeed = container_6.RefSpeed;
                        firstGetBrakeCmd = false;
                    }
                    brake_distance.Content = "制动中";
                    brakeDistanceQueue.Enqueue(container_6.RefSpeed);
                }
                // 复位
                if (container_6.RefSpeed >= 5 && !container_6.NetBrakeCmd && !container_6.NetFastBrakeCmd && !container_6.HardBrakeCmd && !container_6.HardFastBrakeCmd && !container_6.HardEmergencyBrake)
                {
                    brake_distance.Content = "0.00";
                    average_reduce_speed.Content = "0.00";
                    firstGetBrakeCmd = true;
                }
            }
        }
        private void UpdateWSP()
        {
            //row_1_column_1_sli.Content = container_1.SlipLvl1 + " / " + container_1.SlipLvl2;
            //row_1_column_2_sli.Content = container_2.SlipLvl1 + " / " + container_2.SlipLvl2;
            //row_1_column_3_sli.Content = container_3.SlipLvl1 + " / " + container_3.SlipLvl2;
            //row_1_column_4_sli.Content = container_4.SlipLvl1 + " / " + container_4.SlipLvl2;
            //row_1_column_5_sli.Content = container_5.SlipLvl1 + " / " + container_5.SlipLvl2;
            //row_1_column_6_sli.Content = container_6.SlipLvl1 + " / " + container_6.SlipLvl2;
            row_1_column_1_sli.Content = container_1.SlipLvl1;
            row_1_column_2_sli.Content = container_2.SlipLvl1;
            row_1_column_3_sli.Content = container_3.SlipLvl1;
            row_1_column_4_sli.Content = container_4.SlipLvl1;
            row_1_column_5_sli.Content = container_5.SlipLvl1;
            row_1_column_6_sli.Content = container_6.SlipLvl1;

            row_1_column_8_sli.Content = container_1.SlipLvl2;
            row_1_column_9_sli.Content = container_2.SlipLvl2;
            row_1_column_10_sli.Content = container_3.SlipLvl2;
            row_1_column_11_sli.Content = container_4.SlipLvl2;
            row_1_column_12_sli.Content = container_5.SlipLvl2;
            row_1_column_13_sli.Content = container_6.SlipLvl2;

            row_4_column_1_sli.Content = container_1.AccValue1;
            row_4_column_2_sli.Content = container_2.AccValue1;
            row_4_column_3_sli.Content = container_3.AccValue1;
            row_4_column_4_sli.Content = container_4.AccValue1;
            row_4_column_5_sli.Content = container_5.AccValue1;
            row_4_column_6_sli.Content = container_6.AccValue1;

            row_4_column_8_sli.Content = container_1.AccValue2;
            row_4_column_9_sli.Content = container_2.AccValue2;
            row_4_column_10_sli.Content = container_3.AccValue2;
            row_4_column_11_sli.Content = container_4.AccValue2;
            row_4_column_12_sli.Content = container_5.AccValue2;
            row_4_column_13_sli.Content = container_6.AccValue2;

            row_2_column_1_sli.Content = container_1.DcuVolta[0] + " V";
            row_3_column_8_sli.Content = container_1.DcuVolta[3] + " V";
            row_3_column_1_sli.Content = container_1.DcuVolta[1] + " V";
            row_2_column_8_sli.Content = container_1.DcuVolta[2] + " V";

            row_5_column_1_sli.Content = container_1.SpeedSensorError_1;
            row_5_column_2_sli.Content = container_2.SpeedSensorError_1;
            row_5_column_3_sli.Content = container_3.SpeedSensorError_1;
            row_5_column_4_sli.Content = container_4.SpeedSensorError_1;
            row_5_column_5_sli.Content = container_5.SpeedSensorError_1;
            row_5_column_6_sli.Content = container_6.SpeedSensorError_1;

            row_5_column_8_sli.Content = container_1.WSPTargetValue_1 * 3.0;
            row_5_column_9_sli.Content = container_2.WSPTargetValue_1 * 3.0;
            row_5_column_10_sli.Content = container_3.WSPTargetValue_1 * 3.0;
            row_5_column_11_sli.Content = container_4.WSPTargetValue_1 * 3.0;
            row_5_column_12_sli.Content = container_5.WSPTargetValue_1 * 3.0;
            row_5_column_13_sli.Content = container_6.WSPTargetValue_1 * 3.0;

            row_6_column_1_sli.Content = container_1.SpeedSensorError_2;
            row_6_column_2_sli.Content = container_2.SpeedSensorError_2;
            row_6_column_3_sli.Content = container_3.SpeedSensorError_2;
            row_6_column_4_sli.Content = container_4.SpeedSensorError_2;
            row_6_column_5_sli.Content = container_5.SpeedSensorError_2;
            row_6_column_6_sli.Content = container_6.SpeedSensorError_2;

            row_6_column_8_sli.Content = container_1.WSPTargetValue_2 * 3.0;
            row_6_column_9_sli.Content = container_2.WSPTargetValue_2 * 3.0;
            row_6_column_10_sli.Content = container_3.WSPTargetValue_2 * 3.0;
            row_6_column_11_sli.Content = container_4.WSPTargetValue_2 * 3.0;
            row_6_column_12_sli.Content = container_5.WSPTargetValue_2 * 3.0;
            row_6_column_13_sli.Content = container_6.WSPTargetValue_2 * 3.0;

            row_7_column_8_sli.Content = container_1.ConnectValveControl;
            row_7_column_9_sli.Content = container_2.ConnectValveControl;
            row_7_column_10_sli.Content = container_3.ConnectValveControl;
            row_7_column_11_sli.Content = container_4.ConnectValveControl;
            row_7_column_12_sli.Content = container_5.ConnectValveControl;
            row_7_column_13_sli.Content = container_6.ConnectValveControl;

            #region 2020-3-25

            #region 1架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_1.SpeedA1Shaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_1_sli.Content = 0;
                    
                }
                else
                {
                    row_8_column_1_sli.Content = ((container_1.RefSpeed - container_1.SpeedA1Shaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_1.SpeedA1Shaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_8_sli.Content = 0;
                }
                else
                {
                    row_8_column_8_sli.Content = ((container_1.RefSpeed - container_1.SpeedA1Shaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_1.SpeedA1Shaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_1_sli.Content = 0;
                }
                else
                {
                    row_8_column_1_sli.Content = ((container_6.RefSpeed - container_1.SpeedA1Shaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_1.SpeedA1Shaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_8_sli.Content = 0;
                }
                else
                {
                    row_8_column_8_sli.Content = ((container_6.RefSpeed - container_1.SpeedA1Shaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #region 2架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_2.SpeedShaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_2_sli.Content = 0;
                }
                else
                {
                    row_8_column_2_sli.Content = ((container_1.RefSpeed - container_2.SpeedShaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_2.SpeedShaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_9_sli.Content = 0;
                }
                else
                {
                    row_8_column_9_sli.Content = ((container_1.RefSpeed - container_2.SpeedShaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_2.SpeedShaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_2_sli.Content = 0;
                }
                else
                {
                    row_8_column_2_sli.Content = ((container_6.RefSpeed - container_2.SpeedShaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_2.SpeedShaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_9_sli.Content = 0;
                }
                else
                {
                    row_8_column_9_sli.Content = ((container_6.RefSpeed - container_2.SpeedShaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #region 3架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_3.SpeedShaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_3_sli.Content = 0;
                }
                else
                {
                    row_8_column_3_sli.Content = ((container_1.RefSpeed - container_3.SpeedShaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_3.SpeedShaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_10_sli.Content = 0;
                }
                else
                {
                    row_8_column_10_sli.Content = ((container_1.RefSpeed - container_3.SpeedShaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_3.SpeedShaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_3_sli.Content = 0;
                }
                else
                {
                    row_8_column_3_sli.Content = ((container_6.RefSpeed - container_3.SpeedShaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_3.SpeedShaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_10_sli.Content = 0;
                }
                else
                {
                    row_8_column_10_sli.Content = ((container_6.RefSpeed - container_3.SpeedShaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #region 4架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_4.SpeedShaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_4_sli.Content = 0;
                }
                else
                {
                    row_8_column_4_sli.Content = ((container_1.RefSpeed - container_4.SpeedShaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_4.SpeedShaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_11_sli.Content = 0;
                }
                else
                {
                    row_8_column_11_sli.Content = ((container_1.RefSpeed - container_4.SpeedShaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_4.SpeedShaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_4_sli.Content = 0;
                }
                else
                {
                    row_8_column_4_sli.Content = ((container_6.RefSpeed - container_4.SpeedShaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_4.SpeedShaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_11_sli.Content = 0;
                }
                else
                {
                    row_8_column_11_sli.Content = ((container_6.RefSpeed - container_4.SpeedShaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #region 5架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_5.SpeedShaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_5_sli.Content = 0;
                }
                else
                {
                    row_8_column_5_sli.Content = ((container_1.RefSpeed - container_5.SpeedShaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_5.SpeedShaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_12_sli.Content = 0;
                }
                else
                {
                    row_8_column_12_sli.Content = ((container_1.RefSpeed - container_5.SpeedShaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_5.SpeedShaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_5_sli.Content = 0;
                }
                else
                {
                    row_8_column_5_sli.Content = ((container_6.RefSpeed - container_5.SpeedShaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_5.SpeedShaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_12_sli.Content = 0;
                }
                else
                {
                    row_8_column_12_sli.Content = ((container_6.RefSpeed - container_5.SpeedShaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #region 6架1、2轴滑移率

            if (container_1.GateValveState) // 1车为主车
            {
                // 1轴
                if ((container_1.RefSpeed - container_6.SpeedA1Shaft1) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_6_sli.Content = 0;

                }
                else
                {
                    row_8_column_6_sli.Content = ((container_1.RefSpeed - container_6.SpeedA1Shaft1) / container_1.RefSpeed) * 100;
                }

                // 2轴
                if ((container_1.RefSpeed - container_6.SpeedA1Shaft2) < 0 || container_1.RefSpeed < 3)
                {
                    row_8_column_13_sli.Content = 0;
                }
                else
                {
                    row_8_column_13_sli.Content = ((container_1.RefSpeed - container_6.SpeedA1Shaft2) / container_1.RefSpeed) * 100;
                }
            }
            else // 6车为主车
            {
                // 1轴
                if ((container_6.RefSpeed - container_6.SpeedA1Shaft1) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_6_sli.Content = 0;
                }
                else
                {
                    row_8_column_6_sli.Content = ((container_6.RefSpeed - container_6.SpeedA1Shaft1) / container_6.RefSpeed) * 100;
                }

                // 2轴
                if ((container_6.RefSpeed - container_6.SpeedA1Shaft2) < 0 || container_6.RefSpeed < 3)
                {
                    row_8_column_13_sli.Content = 0;
                }
                else
                {
                    row_8_column_13_sli.Content = ((container_6.RefSpeed - container_6.SpeedA1Shaft2) / container_6.RefSpeed) * 100;
                }
            }

            #endregion

            #endregion


        }

        // 2020-1-15
        public static bool LifeSig_1 = false;
        public static bool LifeSig_6 = false;
        /// <summary>
        /// 更新主UI模拟量
        /// </summary>
        private void UpdateAO()
        {
            row_1_column_1.Content = Check_Double(container_1.RefSpeed, 1);
            row_1_column_6.Content = Check_Double(container_6.RefSpeed, 6);
            row_2_column_1.Content = Check_Int(container_1.BrakeLevel, 1);
            row_2_column_6.Content = Check_Int(container_6.BrakeLevel, 6);
            row_3_column_1.Content = Check_Int(container_1.TrainBrakeForce, 1);
            row_3_column_6.Content = Check_Int(container_6.TrainBrakeForce, 6);

            //生命信号
            row_4_column_1.Content = container_1.LifeSig;
            // 2020-1-15
            LifeSig_1 = SaveLifeSig(container_1.LifeSig); // 实际使用中使用两个bool量接受这个函数传来的值
            row_4_column_2.Content = container_2.LifeSig;
            row_4_column_3.Content = container_3.LifeSig;
            row_4_column_4.Content = container_4.LifeSig;
            row_4_column_5.Content = container_5.LifeSig;
            row_4_column_6.Content = container_6.LifeSig;
            // 2020-1-15
            LifeSig_6 = SaveLifeSig_6(container_6.LifeSig);

            //速度信号
            row_5_column_1.Content = string.Format(Utils.formatN1, container_1.SpeedA1Shaft1);
            row_5_column_2.Content = string.Format(Utils.formatN1, container_2.SpeedShaft1);
            row_5_column_3.Content = string.Format(Utils.formatN1, container_3.SpeedShaft1);
            row_5_column_4.Content = string.Format(Utils.formatN1, container_4.SpeedShaft1);
            row_5_column_5.Content = string.Format(Utils.formatN1, container_5.SpeedShaft1);
            row_5_column_6.Content = string.Format(Utils.formatN1, container_6.SpeedA1Shaft1);

            row_6_column_1.Content = string.Format(Utils.formatN1, container_1.SpeedA1Shaft2);
            row_6_column_2.Content = string.Format(Utils.formatN1, container_2.SpeedShaft2);
            row_6_column_3.Content = string.Format(Utils.formatN1, container_3.SpeedShaft2);
            row_6_column_4.Content = string.Format(Utils.formatN1, container_4.SpeedShaft2);
            row_6_column_5.Content = string.Format(Utils.formatN1, container_5.SpeedShaft2);
            row_6_column_6.Content = string.Format(Utils.formatN1, container_6.SpeedA1Shaft2);

            //空气制动目标值
            row_7_column_1.Content = Check(container_1.AbTargetValueAx1, 1) + " / " + Check(container_6.AbTargetValueAx1, 6);
            row_7_column_2.Content = Check(container_1.AbTargetValueAx2, 1) + " / " + Check(container_6.AbTargetValueAx2, 6);
            row_7_column_3.Content = Check(container_1.AbTargetValueAx3, 1) + " / " + Check(container_6.AbTargetValueAx3, 6);
            row_7_column_4.Content = Check(container_1.AbTargetValueAx4, 1) + " / " + Check(container_6.AbTargetValueAx4, 6);
            row_7_column_5.Content = Check(container_1.AbTargetValueAx5, 1) + " / " + Check(container_6.AbTargetValueAx5, 6);
            row_7_column_6.Content = Check(container_1.AbTargetValueAx6, 1) + " / " + Check(container_6.AbTargetValueAx6, 6);

            //1/6架制动风缸压力
            row_8_column_1.Content = container_1.BrakeCylinderSourcePressure;
            row_8_column_2.Content = container_2.BrakeCylinderSourcePressure;
            row_8_column_3.Content = container_3.BrakeCylinderSourcePressure;
            row_8_column_4.Content = container_4.BrakeCylinderSourcePressure;
            row_8_column_5.Content = container_5.BrakeCylinderSourcePressure;
            row_8_column_6.Content = container_6.BrakeCylinderSourcePressure;

            //1/6架空簧压力
            row_9_column_1.Content = container_1.AirSpring1PressureA1Car1;
            row_9_column_2.Content = container_2.AirSpringPressure1;
            row_9_column_3.Content = container_3.AirSpringPressure1;
            row_9_column_4.Content = container_4.AirSpringPressure1;
            row_9_column_5.Content = container_5.AirSpringPressure1;
            row_9_column_6.Content = container_6.AirSpring1PressureA1Car1;

            row_10_column_1.Content = container_1.AirSpring2PressureA1Car1;
            row_10_column_2.Content = container_2.AirSpringPressure2;
            row_10_column_3.Content = container_3.AirSpringPressure2;
            row_10_column_4.Content = container_4.AirSpringPressure2;
            row_10_column_5.Content = container_5.AirSpringPressure2;
            row_10_column_6.Content = container_6.AirSpring2PressureA1Car1;

            //停放压力
            row_11_column_1.Content = container_1.ParkPressureA1;
            row_11_column_3.Content = container_3.ParkPressure;
            row_11_column_5.Content = container_5.ParkPressure;

            //VLD实际
            row_12_column_1.Content = container_1.VldRealPressureAx1;
            row_12_column_2.Content = container_2.VldRealPressure;
            row_12_column_3.Content = container_3.VldRealPressure;
            row_12_column_4.Content = container_4.VldRealPressure;
            row_12_column_5.Content = container_5.VldRealPressure;
            row_12_column_6.Content = container_6.VldRealPressureAx1;

            //1/6架制动缸压力
            row_13_column_1.Content = container_1.Bcp1PressureAx1;
            row_13_column_2.Content = container_2.Bcp1Pressure;
            row_13_column_3.Content = container_3.Bcp1Pressure;
            row_13_column_4.Content = container_4.Bcp1Pressure;
            row_13_column_5.Content = container_5.Bcp1Pressure;
            row_13_column_6.Content = container_6.Bcp1PressureAx1;

            row_14_column_1.Content = container_1.Bcp2PressureAx2;
            row_14_column_2.Content = container_2.Bcp2Pressure;
            row_14_column_3.Content = container_3.Bcp2Pressure;
            row_14_column_4.Content = container_4.Bcp2Pressure;
            row_14_column_5.Content = container_5.Bcp2Pressure;
            row_14_column_6.Content = container_6.Bcp2PressureAx2;

            //VLD设定
            row_15_column_1.Content = container_1.VldPressureSetupAx1;
            row_15_column_2.Content = container_2.VldSetupPressure;
            row_15_column_3.Content = container_3.VldSetupPressure;
            row_15_column_4.Content = container_4.VldSetupPressure;
            row_15_column_5.Content = container_5.VldSetupPressure;
            row_15_column_6.Content = container_6.VldPressureSetupAx1;

            //1/6架载荷
            row_16_column_1.Content = container_1.MassA1;
            row_16_column_2.Content = container_2.MassValue;
            row_16_column_3.Content = container_3.MassValue;
            row_16_column_4.Content = container_4.MassValue;
            row_16_column_5.Content = container_5.MassValue;
            row_16_column_6.Content = container_6.MassA1;

            //架1自检目标设定值
            row_17_column_1.Content = container_1.SelfTestSetup;
            row_17_column_2.Content = container_2.SelfTestSetup;
            row_17_column_3.Content = container_3.SelfTestSetup;
            row_17_column_4.Content = container_4.SelfTestSetup;
            row_17_column_5.Content = container_5.SelfTestSetup;
            row_17_column_6.Content = container_6.SelfTestSetup;

            //VCM & DCU
            row_18_column_1.Content = Check_Int(container_1.VCMLifeSig, 1);
            row_18_column_6.Content = Check_Int(container_6.VCMLifeSig, 6);

            row_19_column_1.Content = Check_Int(container_1.DcuLifeSig[0], 1);
            row_19_column_6.Content = Check_Int(container_6.DcuLifeSig[0], 6);

            row_20_column_1.Content = Check_Int(container_1.DcuLifeSig[1], 1);
            row_20_column_6.Content = Check_Int(container_6.DcuLifeSig[1], 6);

            row_21_column_1.Content = Check_Int(container_1.DcuLifeSig[2], 1);
            row_21_column_6.Content = Check_Int(container_6.DcuLifeSig[2], 6);

            row_22_column_1.Content = Check_Int(container_1.DcuLifeSig[3], 1);
            row_22_column_6.Content = Check_Int(container_6.DcuLifeSig[3], 6);

            row_23_column_1.Content = Check_Int(container_1.DcuEbRealValue[0], 1);
            row_23_column_6.Content = Check_Int(container_6.DcuEbRealValue[0], 6);

            row_24_column_1.Content = Check_Int(container_1.DcuMax[0], 1);
            row_24_column_6.Content = Check_Int(container_6.DcuMax[0], 6);

            row_25_column_1.Content = Check_Int(container_1.DcuEbRealValue[1], 1);
            row_25_column_6.Content = Check_Int(container_6.DcuEbRealValue[1], 6);

            row_26_column_1.Content = Check_Int(container_1.DcuMax[1], 1);
            row_26_column_6.Content = Check_Int(container_6.DcuMax[1], 6);

            row_27_column_1.Content = Check_Int(container_1.DcuEbRealValue[2], 1);
            row_27_column_6.Content = Check_Int(container_6.DcuEbRealValue[2], 6);

            row_28_column_1.Content = Check_Int(container_1.DcuMax[2], 1);
            row_28_column_6.Content = Check_Int(container_6.DcuMax[2], 6);

            row_29_column_1.Content = Check_Int(container_1.DcuEbRealValue[3], 1);
            row_29_column_6.Content = Check_Int(container_6.DcuEbRealValue[3], 6);

            row_30_column_1.Content = Check_Int(container_1.DcuMax[3], 1);
            row_30_column_6.Content = Check_Int(container_6.DcuMax[3], 6);

            //空气制动能力值
            row_31_column_1.Content = Check(container_1.AbCapacity[0], 1) + " / " + Check(container_6.AbCapacity[0], 6);
            row_31_column_2.Content = Check(container_1.AbCapacity[1], 1) + " / " + Check(container_6.AbCapacity[1], 6);
            row_31_column_3.Content = Check(container_1.AbCapacity[2], 1) + " / " + Check(container_6.AbCapacity[2], 6);
            row_31_column_4.Content = Check(container_1.AbCapacity[3], 1) + " / " + Check(container_6.AbCapacity[3], 6);
            row_31_column_5.Content = Check(container_1.AbCapacity[4], 1) + " / " + Check(container_6.AbCapacity[4], 6);
            row_31_column_6.Content = Check(container_1.AbCapacity[5], 1) + " / " + Check(container_6.AbCapacity[5], 6);

            //空气制动实际值
            row_32_column_1.Content = Check(container_1.AbRealValue[0], 1) + " / " + Check(container_6.AbRealValue[0], 6);
            row_32_column_2.Content = Check(container_1.AbRealValue[1], 1) + " / " + Check(container_6.AbRealValue[1], 6);
            row_32_column_3.Content = Check(container_1.AbRealValue[2], 1) + " / " + Check(container_6.AbRealValue[2], 6);
            row_32_column_4.Content = Check(container_1.AbRealValue[3], 1) + " / " + Check(container_6.AbRealValue[3], 6);
            row_32_column_5.Content = Check(container_1.AbRealValue[4], 1) + " / " + Check(container_6.AbRealValue[4], 6);
            row_32_column_6.Content = Check(container_1.AbRealValue[5], 1) + " / " + Check(container_6.AbRealValue[5], 6);

            //软件版本
            row_33_column_1.Content = container_1.SoftwareVersionCPU;
            row_34_column_1.Content = container_1.SoftwareVersionEP;

            row_33_column_2.Content = container_2.SoftwareVersionCPU;
            row_34_column_2.Content = container_2.SoftwareVersionEP;
            row_33_column_3.Content = container_3.SoftwareVersionCPU;
            row_34_column_3.Content = container_3.SoftwareVersionEP;
            row_33_column_4.Content = container_4.SoftwareVersionCPU;
            row_34_column_4.Content = container_4.SoftwareVersionEP;
            row_33_column_5.Content = container_5.SoftwareVersionCPU;
            row_34_column_5.Content = container_5.SoftwareVersionEP;
            row_33_column_6.Content = container_6.SoftwareVersionCPU;
            row_34_column_6.Content = container_6.SoftwareVersionEP;

            row_35_column_4.Content = container_4.ParkPressure;

            //row_34_column_4.Content = container_4.ParkPressure;

            // TC、MP、ME轮径
            row_36_column_1.Content = container_1.WheelSize;
            row_36_column_2.Content = container_2.WheelSize;
            row_36_column_3.Content = container_3.WheelSize;
            row_36_column_4.Content = container_4.WheelSize;
            row_36_column_5.Content = container_5.WheelSize;
            row_36_column_6.Content = container_6.WheelSize;

            row_37_column_1.Content = container_1.Tc1 + " / " + container_6.Tc2;
            row_37_column_2.Content = container_1.Tc1 + " / " + container_6.Tc2;
            row_37_column_3.Content = container_1.Mp1 + " / " + container_6.Mp2;
            row_37_column_4.Content = container_1.Mp1 + " / " + container_6.Mp2;
            row_37_column_5.Content = container_1.M1 + " / " + container_6.M2;
            row_37_column_6.Content = container_1.M1 + " / " + container_6.M2;

            //0xX8 data
            row_38_column_1.Content = "0x" + container_1.SwitchInputChannel1_8.ToString("X");
            row_38_column_2.Content = "0x" + container_2.SwitchInputChannel1_8.ToString("X");
            row_38_column_3.Content = "0x" + container_3.SwitchInputChannel1_8.ToString("X");
            row_38_column_4.Content = "0x" + container_4.SwitchInputChannel1_8.ToString("X");
            row_38_column_5.Content = "0x" + container_5.SwitchInputChannel1_8.ToString("X");
            row_38_column_6.Content = "0x" + container_6.SwitchInputChannel1_8.ToString("X");

            row_39_column_1.Content = "0x" + container_1.DigitalOutputChannel9_16.ToString("X");
            row_39_column_2.Content = "0x" + container_2.DigitalOutputChannel9_16.ToString("X");
            row_39_column_3.Content = "0x" + container_3.DigitalOutputChannel9_16.ToString("X");
            row_39_column_4.Content = "0x" + container_4.DigitalOutputChannel9_16.ToString("X");
            row_39_column_5.Content = "0x" + container_5.DigitalOutputChannel9_16.ToString("X");
            row_39_column_6.Content = "0x" + container_6.DigitalOutputChannel9_16.ToString("X");

            row_40_column_1.Content = "0x" + container_1.DigitalOutputChannel1_8.ToString("X");
            row_40_column_2.Content = "0x" + container_2.DigitalOutputChannel1_8.ToString("X");
            row_40_column_3.Content = "0x" + container_3.DigitalOutputChannel1_8.ToString("X");
            row_40_column_4.Content = "0x" + container_4.DigitalOutputChannel1_8.ToString("X");
            row_40_column_5.Content = "0x" + container_5.DigitalOutputChannel1_8.ToString("X");
            row_40_column_6.Content = "0x" + container_6.DigitalOutputChannel1_8.ToString("X");

            row_41_column_1.Content = "0x" + container_1.OutputOverFlowProtectChannel9_16.ToString("X");
            row_41_column_2.Content = "0x" + container_2.OutputOverFlowProtectChannel9_16.ToString("X");
            row_41_column_3.Content = "0x" + container_3.OutputOverFlowProtectChannel9_16.ToString("X");
            row_41_column_4.Content = "0x" + container_4.OutputOverFlowProtectChannel9_16.ToString("X");
            row_41_column_5.Content = "0x" + container_5.OutputOverFlowProtectChannel9_16.ToString("X");
            row_41_column_6.Content = "0x" + container_6.OutputOverFlowProtectChannel9_16.ToString("X");

            row_42_column_1.Content = "0x" + container_1.OutputOverFlowProtectChannel1_8.ToString("X");
            row_42_column_2.Content = "0x" + container_2.OutputOverFlowProtectChannel1_8.ToString("X");
            row_42_column_3.Content = "0x" + container_3.OutputOverFlowProtectChannel1_8.ToString("X");
            row_42_column_4.Content = "0x" + container_4.OutputOverFlowProtectChannel1_8.ToString("X");
            row_42_column_5.Content = "0x" + container_5.OutputOverFlowProtectChannel1_8.ToString("X");
            row_42_column_6.Content = "0x" + container_6.OutputOverFlowProtectChannel1_8.ToString("X");
        }

        private void UpdateDI()
        {
            //制动命令
            row_1_column_1_DI.Fill = GetBrush(container_1.BrakeCmd, 1);
            row_1_column_6_DI.Fill = GetBrush(container_6.BrakeCmd, 6);

            //牵引命令
            row_2_column_1_DI.Fill = GetBrush(container_1.DriveCmd, 1);
            row_2_column_6_DI.Fill = GetBrush(container_6.DriveCmd, 6);

            //惰行命令
            row_3_column_1_DI.Fill = GetBrush(container_1.LazyCmd, 1);
            row_3_column_6_DI.Fill = GetBrush(container_6.LazyCmd, 6);

            //快速制动命令
            row_4_column_1_DI.Fill = GetBrush(container_1.FastBrakeCmd, 1);
            row_4_column_6_DI.Fill = GetBrush(container_6.FastBrakeCmd, 6);

            //紧急制动命令
            row_5_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd, 1);
            row_5_column_6_DI.Fill = GetBrush(container_6.EmergencyBrakeCmd, 6);

            //紧急制动激活
            row_6_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeActiveA1);
            row_6_column_2_DI.Fill = GetBrush(container_2.EmergencyBrakeActive);
            row_6_column_3_DI.Fill = GetBrush(container_3.EmergencyBrakeActive);
            row_6_column_4_DI.Fill = GetBrush(container_4.EmergencyBrakeActive);
            row_6_column_5_DI.Fill = GetBrush(container_5.EmergencyBrakeActive);
            row_6_column_6_DI.Fill = GetBrush(container_6.EmergencyBrakeActiveA1);

            //空气制动施加
            row_7_column_1_DI.Fill = GetBrush(container_1.AbActive);
            row_7_column_2_DI.Fill = GetBrush(container_2.AbBrakeActive);
            row_7_column_3_DI.Fill = GetBrush(container_3.AbBrakeActive);
            row_7_column_4_DI.Fill = GetBrush(container_4.AbBrakeActive);
            row_7_column_5_DI.Fill = GetBrush(container_5.AbBrakeActive);
            row_7_column_6_DI.Fill = GetBrush(container_6.AbActive);

            //停放制动缓解
            row_8_column_1_DI.Fill = GetBrush(container_1.ParkBreakRealease);
            row_8_column_3_DI.Fill = GetBrush(container_3.ParkBrakeRealease);
            row_8_column_5_DI.Fill = GetBrush(container_5.ParkBrakeRealease);

            //硬线牵引
            row_9_column_1_DI.Fill = GetBrush(container_1.HardDriveCmd, 1);
            row_9_column_6_DI.Fill = GetBrush(container_6.HardDriveCmd, 6);

            //硬线制动
            row_10_column_1_DI.Fill = GetBrush(container_1.HardBrakeCmd, 1);
            row_10_column_6_DI.Fill = GetBrush(container_6.HardBrakeCmd, 6);

            //第二列
            //硬线快速制动
            row_1_column_8_DI.Fill = GetBrush(container_1.HardFastBrakeCmd, 1);
            row_1_column_13_DI.Fill = GetBrush(container_6.HardFastBrakeCmd, 6);

            //硬线紧急制动
            row_2_column_8_DI.Fill = GetBrush(container_1.HardEmergencyBrake, 1);
            row_2_column_13_DI.Fill = GetBrush(container_6.HardEmergencyBrake, 6);

            //硬线紧急牵引
            row_3_column_8_DI.Fill = GetBrush(container_1.HardEmergencyDriveCmd, 1);
            row_3_column_13_DI.Fill = GetBrush(container_6.HardEmergencyDriveCmd, 6);

            //网络牵引
            row_4_column_8_DI.Fill = GetBrush(container_1.NetDriveCmd, 1);
            row_4_column_13_DI.Fill = GetBrush(container_6.NetDriveCmd, 6);


            //网络制动
            row_5_column_8_DI.Fill = GetBrush(container_1.NetBrakeCmd, 1);
            row_5_column_13_DI.Fill = GetBrush(container_6.NetBrakeCmd, 6);

            //保持制动缓解
            row_6_column_8_DI.Fill = GetBrush(container_1.HoldBrakeRealease, 1);
            row_6_column_13_DI.Fill = GetBrush(container_6.HoldBrakeRealease, 6);

            //CAN单元
            row_7_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_A, 1);
            row_8_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_B, 1);

            row_7_column_13_DI.Fill = GetBrush(container_6.CanUintSelfTestCmd_A, 6);
            row_8_column_13_DI.Fill = GetBrush(container_6.CanUintSelfTestCmd_B, 6);

            //自检
            row_9_column_8_DI.Fill = GetBrush(container_1.SelfTestCmd, 1);
            row_9_column_13_DI.Fill = GetBrush(container_6.SelfTestCmd, 6);

            //紧急制动命令
            row_10_column_8_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd, 1);
            row_10_column_13_DI.Fill = GetBrush(container_6.EmergencyBrakeCmd, 6);

            //网络快速制动
            row_11_column_8_DI.Fill = GetBrush(container_1.NetFastBrakeCmd, 1);
            row_11_column_13_DI.Fill = GetBrush(container_6.NetFastBrakeCmd, 6);
        }

        private void UpdateDO()
        {
            row_1_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.NORMAL_MODE), 1);
            row_1_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.NORMAL_MODE), 6);
            row_2_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.EMERGENCY_DRIVE_MODE), 1);
            row_2_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.EMERGENCY_DRIVE_MODE), 6);
            row_3_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.CALLBACK_MODE), 1);
            row_3_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.CALLBACK_MODE), 6);

            row_4_column_1_DO.Fill = GetBrush(container_1.LazyCmd, 1);
            row_4_column_6_DO.Fill = GetBrush(container_6.LazyCmd, 6);

            row_5_column_1_DO.Fill = GetBrush(container_1.KeepBrakeState, 1);
            row_5_column_6_DO.Fill = GetBrush(container_6.KeepBrakeState, 6);

            row_6_column_1_DO.Fill = GetBrush(container_1.LazyState, 1);
            row_6_column_6_DO.Fill = GetBrush(container_6.LazyState, 6);

            row_7_column_1_DO.Fill = GetBrush(container_1.DriveState, 1);
            row_7_column_6_DO.Fill = GetBrush(container_6.DriveState, 6);

            row_8_column_1_DO.Fill = GetBrush(container_1.NormalBrakeState, 1);
            row_8_column_6_DO.Fill = GetBrush(container_6.NormalBrakeState, 6);

            row_9_column_1_DO.Fill = GetBrush(container_1.EmergencyBrakeState, 1);
            row_9_column_6_DO.Fill = GetBrush(container_6.EmergencyBrakeState, 6);

            row_10_column_1_DO.Fill = GetBrush(container_1.ZeroSpeed, 1);
            row_10_column_6_DO.Fill = GetBrush(container_6.ZeroSpeed, 6);

            row_11_column_1_DO.Fill = GetBrush(container_1.SelfTestFail, 1);
            row_11_column_6_DO.Fill = GetBrush(container_6.SelfTestFail, 6);

            row_12_column_1_DO.Fill = GetBrush(container_1.UnSelfTest24, 1);
            row_12_column_6_DO.Fill = GetBrush(container_6.UnSelfTest24, 6);

            row_13_column_1_DO.Fill = GetBrush(container_1.UnSelfTest26, 1);
            row_13_column_6_DO.Fill = GetBrush(container_6.UnSelfTest26, 6);

            row_14_column_1_DO.Fill = GetBrush(container_1.GateValveState);
            row_14_column_6_DO.Fill = GetBrush(container_6.GateValveState);

            row_15_column_1_DO.Fill = GetBrush(container_1.CanUnitSelfTestOn, 1);
            row_15_column_6_DO.Fill = GetBrush(container_6.CanUnitSelfTestOn, 6);

            row_16_column_1_DO.Fill = GetBrush(container_1.ValveCanEmergencyActive, 1);
            row_16_column_6_DO.Fill = GetBrush(container_6.ValveCanEmergencyActive, 6);

            row_17_column_1_DO.Fill = GetBrush(container_1.CanUintSelfTestOver, 1);
            row_17_column_6_DO.Fill = GetBrush(container_6.CanUintSelfTestOver, 6);

            row_18_column_1_DO.Fill = GetBrush(container_1.TowingMode, 1);
            row_18_column_6_DO.Fill = GetBrush(container_6.TowingMode, 6);

            row_19_column_1_DO.Fill = GetBrush(container_1.ATOMode1, 1);
            row_19_column_6_DO.Fill = GetBrush(container_6.ATOMode1, 6);

            row_20_column_1_DO.Fill = GetBrush(container_1.BrakeLevelEnable, 1);
            row_20_column_6_DO.Fill = GetBrush(container_6.BrakeLevelEnable, 6);

            row_21_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[0], 1);
            row_21_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[0], 6);

            row_22_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[0], 1);
            row_22_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[0], 6);

            row_23_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[0], 1);
            row_23_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[0], 6);

            row_24_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[1], 1);
            row_24_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[1], 6);

            row_25_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[1], 1);
            row_25_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[1], 6);

            row_26_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[1], 1);
            row_26_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[1], 6);

            row_27_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[2], 1);
            row_27_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[2], 6);

            row_28_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[2], 1);
            row_28_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[2], 6);

            row_29_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[2], 1);
            row_29_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[2], 6);

            //第二列
            row_1_column_8_DO.Fill = GetBrush(container_1.VCM_MVBConnectionState);
            row_1_column_9_DO.Fill = GetBrush(container_2.VCM_MVBConnectionState);
            row_1_column_10_DO.Fill = GetBrush(container_3.VCM_MVBConnectionState);
            row_1_column_11_DO.Fill = GetBrush(container_4.VCM_MVBConnectionState);
            row_1_column_12_DO.Fill = GetBrush(container_5.VCM_MVBConnectionState);
            row_1_column_13_DO.Fill = GetBrush(container_6.VCM_MVBConnectionState);

            row_2_column_8_DO.Fill = GetBrush(container_1.SlipA1);
            row_2_column_9_DO.Fill = GetBrush(container_2.Slip);
            row_2_column_10_DO.Fill = GetBrush(container_3.Slip);
            row_2_column_11_DO.Fill = GetBrush(container_4.Slip);
            row_2_column_12_DO.Fill = GetBrush(container_5.Slip);
            row_2_column_13_DO.Fill = GetBrush(container_6.SlipA1);

            row_3_column_8_DO.Fill = GetBrush(container_1.NotZeroSpeed, 1);
            row_3_column_13_DO.Fill = GetBrush(container_6.NotZeroSpeed, 6);

            row_4_column_8_DO.Fill = GetBrush(container_1.BSRLowA11);
            row_4_column_9_DO.Fill = GetBrush(container_2.BSRLow1);
            row_4_column_10_DO.Fill = GetBrush(container_3.BSRLow1);
            row_4_column_11_DO.Fill = GetBrush(container_4.BSRLow1);
            row_4_column_12_DO.Fill = GetBrush(container_5.BSRLow1);
            row_4_column_13_DO.Fill = GetBrush(container_6.BSRLowA11);

            row_5_column_8_DO.Fill = GetBrush(container_1.AbStatuesA1);
            row_5_column_9_DO.Fill = GetBrush(container_2.AbBrakeSatet);
            row_5_column_10_DO.Fill = GetBrush(container_3.AbBrakeSatet);
            row_5_column_11_DO.Fill = GetBrush(container_4.AbBrakeSatet);
            row_5_column_12_DO.Fill = GetBrush(container_5.AbBrakeSatet);
            row_5_column_13_DO.Fill = GetBrush(container_6.AbStatuesA1);

            row_6_column_8_DO.Fill = GetBrush(container_1.AirSigValid);
            row_6_column_9_DO.Fill = GetBrush(container_2.AirSigValid);
            row_6_column_10_DO.Fill = GetBrush(container_3.AirSigValid);
            row_6_column_11_DO.Fill = GetBrush(container_4.AirSigValid);
            row_6_column_12_DO.Fill = GetBrush(container_5.AirSigValid);
            row_6_column_13_DO.Fill = GetBrush(container_6.AirSigValid);

            row_7_column_8_DO.Fill = GetBrush(container_1.SelfTestInt, 1);
            row_7_column_13_DO.Fill = GetBrush(container_6.SelfTestInt, 6);

            row_8_column_8_DO.Fill = GetBrush(container_1.SelfTestActive, 1);
            row_8_column_13_DO.Fill = GetBrush(container_6.SelfTestActive, 6);

            row_9_column_8_DO.Fill = GetBrush(container_1.SelfTestSuccess, 1);
            row_9_column_13_DO.Fill = GetBrush(container_6.SelfTestSuccess, 6);

            row_10_column_8_DO.Fill = GetBrush(container_1.EdFadeOut, 1);
            row_10_column_13_DO.Fill = GetBrush(container_6.EdFadeOut, 6);

            row_11_column_8_DO.Fill = GetBrush(container_1.TrainBrakeEnable, 1);
            row_11_column_13_DO.Fill = GetBrush(container_6.TrainBrakeEnable, 6);

            row_12_column_8_DO.Fill = GetBrush(container_1.ZeroSpeed, 1);
            row_12_column_13_DO.Fill = GetBrush(container_6.ZeroSpeed, 6);

            row_13_column_8_DO.Fill = GetBrush(container_1.EdOffB1, 1);
            row_14_column_8_DO.Fill = GetBrush(container_1.EdOffC1, 1);
            row_13_column_13_DO.Fill = GetBrush(container_6.EdOffB1, 6);
            row_14_column_13_DO.Fill = GetBrush(container_6.EdOffC1, 6);

            row_15_column_8_DO.Fill = GetBrush(container_1.WheelInputState, 1);
            row_15_column_13_DO.Fill = GetBrush(container_6.WheelInputState, 6);

            row_16_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestActive);
            row_16_column_9_DO.Fill = GetBrush(container_2.SelfTestActive);
            row_16_column_10_DO.Fill = GetBrush(container_3.SelfTestActive);
            row_16_column_11_DO.Fill = GetBrush(container_4.SelfTestActive);
            row_16_column_12_DO.Fill = GetBrush(container_5.SelfTestActive);
            row_16_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestActive);

            row_17_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestOver);
            row_17_column_9_DO.Fill = GetBrush(container_2.SelfTestOver);
            row_17_column_10_DO.Fill = GetBrush(container_3.SelfTestOver);
            row_17_column_11_DO.Fill = GetBrush(container_4.SelfTestOver);
            row_17_column_12_DO.Fill = GetBrush(container_5.SelfTestOver);
            row_17_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestOver);

            row_18_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestSuccess);
            row_18_column_9_DO.Fill = GetBrush(container_2.SelfTestSuccess);
            row_18_column_10_DO.Fill = GetBrush(container_3.SelfTestSuccess);
            row_18_column_11_DO.Fill = GetBrush(container_4.SelfTestSuccess);
            row_18_column_12_DO.Fill = GetBrush(container_5.SelfTestSuccess);
            row_18_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestSuccess);

            row_19_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestFail);
            row_19_column_9_DO.Fill = GetBrush(container_2.SelfTestFail);
            row_19_column_10_DO.Fill = GetBrush(container_3.SelfTestFail);
            row_19_column_11_DO.Fill = GetBrush(container_4.SelfTestFail);
            row_19_column_12_DO.Fill = GetBrush(container_5.SelfTestFail);
            row_19_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestFail);

            row_20_column_8_DO.Fill = GetBrush(container_1.DcuEbOK[3], 1);
            row_20_column_13_DO.Fill = GetBrush(container_6.DcuEbOK[3], 6);

            row_21_column_8_DO.Fill = GetBrush(container_1.DcuEbFadeout[3], 1);
            row_21_column_13_DO.Fill = GetBrush(container_6.DcuEbFadeout[3], 6);

            row_22_column_8_DO.Fill = GetBrush(container_1.DcuEbSlip[3], 1);
            row_22_column_13_DO.Fill = GetBrush(container_6.DcuEbSlip[3], 6);

            row_23_column_8_DO.Fill = GetBrush(container_1.ATOHold);
            row_23_column_13_DO.Fill = GetBrush(container_6.ATOHold);

            row_24_column_8_DO.Fill = GetBrush(container_1.DcuEbFault[0]);
            row_25_column_8_DO.Fill = GetBrush(container_1.DcuEbFault[1]);
            row_26_column_8_DO.Fill = GetBrush(container_1.DcuEbFault[2]);
            row_27_column_8_DO.Fill = GetBrush(container_1.DcuEbFault[3]);

            row_24_column_13_DO.Fill = GetBrush(container_6.DcuEbFault[0]);
            row_25_column_13_DO.Fill = GetBrush(container_6.DcuEbFault[1]);
            row_26_column_13_DO.Fill = GetBrush(container_6.DcuEbFault[2]);
            row_27_column_13_DO.Fill = GetBrush(container_6.DcuEbFault[3]);
        }

        private void UpdateFA()
        {
            row_1_column_1_FA.Fill = GetBrush(container_1.BSSRSenorFault);
            row_1_column_2_FA.Fill = GetBrush(container_2.BSSRSenorFault);
            row_1_column_3_FA.Fill = GetBrush(container_3.BSSRSenorFault);
            row_1_column_4_FA.Fill = GetBrush(container_4.BSSRSenorFault);
            row_1_column_5_FA.Fill = GetBrush(container_5.BSSRSenorFault);
            row_1_column_6_FA.Fill = GetBrush(container_6.BSSRSenorFault);

            row_2_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_1);
            row_2_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_1);
            row_2_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_1);
            row_2_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_1);
            row_2_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_1);
            row_2_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_1);

            row_3_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_2);
            row_3_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_2);
            row_3_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_2);
            row_3_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_2);
            row_3_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_2);
            row_3_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_2);

            row_4_column_1_FA.Fill = GetBrush(container_1.ParkCylinderSenorFault);
            row_4_column_2_FA.Fill = GetBrush(container_2.ParkCylinderSenorFault);
            row_4_column_3_FA.Fill = GetBrush(container_3.ParkCylinderSenorFault);
            row_4_column_4_FA.Fill = GetBrush(container_4.MainPipeSensorFault);
            row_4_column_5_FA.Fill = GetBrush(container_5.ParkCylinderSenorFault);
            row_4_column_6_FA.Fill = GetBrush(container_6.ParkCylinderSenorFault);

            row_5_column_1_FA.Fill = GetBrush(container_1.VLDSensorFault);
            row_5_column_2_FA.Fill = GetBrush(container_2.VLDSensorFault);
            row_5_column_3_FA.Fill = GetBrush(container_3.VLDSensorFault);
            row_5_column_4_FA.Fill = GetBrush(container_4.VLDSensorFault);
            row_5_column_5_FA.Fill = GetBrush(container_5.VLDSensorFault);
            row_5_column_6_FA.Fill = GetBrush(container_6.VLDSensorFault);

            row_6_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_1);
            row_6_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_1);
            row_6_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_1);
            row_6_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_1);
            row_6_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_1);
            row_6_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_1);

            row_7_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_2);
            row_7_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_2);
            row_7_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_2);
            row_7_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_2);
            row_7_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_2);
            row_7_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_2);

            row_8_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_1);
            row_8_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_1);
            row_8_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_1);
            row_8_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_1);
            row_8_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_1);
            row_8_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_1);

            row_9_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_2);
            row_9_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_2);
            row_9_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_2);
            row_9_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_2);
            row_9_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_2);
            row_9_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_2);

            row_10_column_1_FA.Fill = GetBrush(container_1.BSRLowA11);
            row_10_column_2_FA.Fill = GetBrush(container_2.BSRLow1);
            row_10_column_3_FA.Fill = GetBrush(container_3.BSRLow1);
            row_10_column_4_FA.Fill = GetBrush(container_4.BSRLow1);
            row_10_column_5_FA.Fill = GetBrush(container_5.BSRLow1);
            row_10_column_6_FA.Fill = GetBrush(container_6.BSRLowA11);

            row_11_column_1_FA.Fill = GetBrush(container_1.BCUFail_Serious);
            row_11_column_2_FA.Fill = GetBrush(container_2.BCUFail_Serious);
            row_11_column_3_FA.Fill = GetBrush(container_3.BCUFail_Serious);
            row_11_column_4_FA.Fill = GetBrush(container_4.BCUFail_Serious);
            row_11_column_5_FA.Fill = GetBrush(container_5.BCUFail_Serious);
            row_11_column_6_FA.Fill = GetBrush(container_6.BCUFail_Serious);

            row_12_column_1_FA.Fill = GetBrush(container_1.BCUFail_Middle);
            row_12_column_2_FA.Fill = GetBrush(container_2.BCUFail_Middle);
            row_12_column_3_FA.Fill = GetBrush(container_3.BCUFail_Middle);
            row_12_column_4_FA.Fill = GetBrush(container_4.BCUFail_Middle);
            row_12_column_5_FA.Fill = GetBrush(container_5.BCUFail_Middle);
            row_12_column_6_FA.Fill = GetBrush(container_6.BCUFail_Middle);

            row_13_column_1_FA.Fill = GetBrush(container_1.BCUFail_Slight);
            row_13_column_2_FA.Fill = GetBrush(container_2.BCUFail_Slight);
            row_13_column_3_FA.Fill = GetBrush(container_3.BCUFail_Slight);
            row_13_column_4_FA.Fill = GetBrush(container_4.BCUFail_Slight);
            row_13_column_5_FA.Fill = GetBrush(container_5.BCUFail_Slight);
            row_13_column_6_FA.Fill = GetBrush(container_6.BCUFail_Slight);

            row_14_column_1_FA.Fill = GetBrush(container_1.EmergencyBrakeFault);
            row_14_column_2_FA.Fill = GetBrush(container_2.EmergencyBrakeFault);
            row_14_column_3_FA.Fill = GetBrush(container_3.EmergencyBrakeFault);
            row_14_column_4_FA.Fill = GetBrush(container_4.EmergencyBrakeFault);
            row_14_column_5_FA.Fill = GetBrush(container_5.EmergencyBrakeFault);
            row_14_column_6_FA.Fill = GetBrush(container_6.EmergencyBrakeFault);

            row_15_column_1_FA.Fill = GetBrush(container_1.CanASPEnable, 1);
            row_15_column_6_FA.Fill = GetBrush(container_6.CanASPEnable, 6);

            row_16_column_1_FA.Fill = GetBrush(container_1.BCPLowA, 1);
            row_16_column_6_FA.Fill = GetBrush(container_6.BCPLowA, 6);

            row_17_column_1_FA.Fill = GetBrush(container_1.BCPLowB, 1);
            row_17_column_6_FA.Fill = GetBrush(container_6.BCPLowB, 6);

            row_18_column_1_FA.Fill = GetBrush(container_1.BCPLowC, 1);
            row_18_column_6_FA.Fill = GetBrush(container_6.BCPLowC, 6);

            //第二列
            row_1_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_1);
            row_1_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_1);
            row_1_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_1);
            row_1_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_1);
            row_1_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_1);
            row_1_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_1);

            row_2_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_2);
            row_2_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_2);
            row_2_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_2);
            row_2_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_2);
            row_2_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_2);
            row_2_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_2);

            row_3_column_8_FA.Fill = GetBrush(container_1.WSPFault_1);
            row_3_column_9_FA.Fill = GetBrush(container_2.WSPFault_1);
            row_3_column_10_FA.Fill = GetBrush(container_3.WSPFault_1);
            row_3_column_11_FA.Fill = GetBrush(container_4.WSPFault_1);
            row_3_column_12_FA.Fill = GetBrush(container_5.WSPFault_1);
            row_3_column_13_FA.Fill = GetBrush(container_6.WSPFault_1);

            row_4_column_8_FA.Fill = GetBrush(container_1.WSPFault_2);
            row_4_column_9_FA.Fill = GetBrush(container_2.WSPFault_2);
            row_4_column_10_FA.Fill = GetBrush(container_3.WSPFault_2);
            row_4_column_11_FA.Fill = GetBrush(container_4.WSPFault_2);
            row_4_column_12_FA.Fill = GetBrush(container_5.WSPFault_2);
            row_4_column_13_FA.Fill = GetBrush(container_6.WSPFault_2);

            row_5_column_8_FA.Fill = GetBrush(container_1.CodeConnectorFault);
            row_5_column_9_FA.Fill = GetBrush(container_2.CodeConnectorFault);
            row_5_column_10_FA.Fill = GetBrush(container_3.CodeConnectorFault);
            row_5_column_11_FA.Fill = GetBrush(container_4.CodeConnectorFault);
            row_5_column_12_FA.Fill = GetBrush(container_5.CodeConnectorFault);
            row_5_column_13_FA.Fill = GetBrush(container_6.CodeConnectorFault);

            row_6_column_8_FA.Fill = GetBrush(container_1.AirSpringLimit);
            row_6_column_9_FA.Fill = GetBrush(container_2.AirSpringLimit);
            row_6_column_10_FA.Fill = GetBrush(container_3.AirSpringLimit);
            row_6_column_11_FA.Fill = GetBrush(container_4.AirSpringLimit);
            row_6_column_12_FA.Fill = GetBrush(container_5.AirSpringLimit);
            row_6_column_13_FA.Fill = GetBrush(container_6.AirSpringLimit);

            row_7_column_8_FA.Fill = GetBrush(container_1.BrakeNotRealease);
            row_7_column_9_FA.Fill = GetBrush(container_2.BrakeNotRealease);
            row_7_column_10_FA.Fill = GetBrush(container_3.BrakeNotRealease);
            row_7_column_11_FA.Fill = GetBrush(container_4.BrakeNotRealease);
            row_7_column_12_FA.Fill = GetBrush(container_5.BrakeNotRealease);
            row_7_column_13_FA.Fill = GetBrush(container_6.BrakeNotRealease);

            row_8_column_8_FA.Fill = GetBrush(container_1.BCPLowA11);
            row_8_column_9_FA.Fill = GetBrush(container_2.BCPLow1);
            row_8_column_10_FA.Fill = GetBrush(container_3.BCPLow1);
            row_8_column_11_FA.Fill = GetBrush(container_4.BCPLow1);
            row_8_column_12_FA.Fill = GetBrush(container_5.BCPLow1);
            row_8_column_13_FA.Fill = GetBrush(container_6.BCPLowA11);

            row_9_column_8_FA.Fill = GetBrush(container_1.SpeedDetection);
            row_10_column_8_FA.Fill = GetBrush(container_1.CanBusFail1);
            row_10_column_13_FA.Fill = GetBrush(container_6.CanBusFail1);

            row_10_column_9_FA.Fill = GetBrush(container_2.OCANFault1);
            row_10_column_10_FA.Fill = GetBrush(container_3.OCANFault1);
            row_10_column_11_FA.Fill = GetBrush(container_4.OCANFault1);
            row_10_column_12_FA.Fill = GetBrush(container_5.OCANFault1);

            row_11_column_8_FA.Fill = GetBrush(container_1.CanBusFail2);
            row_11_column_9_FA.Fill = GetBrush(container_2.OCANFault2);
            row_11_column_10_FA.Fill = GetBrush(container_3.OCANFault2);
            row_11_column_11_FA.Fill = GetBrush(container_4.OCANFault2);
            row_11_column_12_FA.Fill = GetBrush(container_5.OCANFault2);
            row_11_column_13_FA.Fill = GetBrush(container_6.CanBusFail2);
            row_12_column_8_FA.Fill = GetBrush(container_1.HardDifferent);
            row_12_column_13_FA.Fill = GetBrush(container_6.HardDifferent);
            row_13_column_8_FA.Fill = GetBrush(container_1.EventHigh);
            row_13_column_13_FA.Fill = GetBrush(container_6.EventHigh);
            row_14_column_8_FA.Fill = GetBrush(container_1.EventMid);
            row_14_column_13_FA.Fill = GetBrush(container_6.EventMid);
            row_15_column_8_FA.Fill = GetBrush(container_1.EventLow);
            row_15_column_13_FA.Fill = GetBrush(container_6.EventLow);



            //row_2_column_1_sli.Content = container_1.

            // ICAN1
            row_16_column_8_FA.Fill = GetBrush(container_1.ICANFault1);
            row_16_column_9_FA.Fill = GetBrush(container_2.ICANFault1);
            row_16_column_10_FA.Fill = GetBrush(container_3.ICANFault1);
            row_16_column_11_FA.Fill = GetBrush(container_4.ICANFault1);
            row_16_column_12_FA.Fill = GetBrush(container_5.ICANFault1);
            row_16_column_13_FA.Fill = GetBrush(container_6.ICANFault1);
            // ICAN2
            row_17_column_8_FA.Fill = GetBrush(container_1.ICANFault2);
            row_17_column_9_FA.Fill = GetBrush(container_2.ICANFault2);
            row_17_column_10_FA.Fill = GetBrush(container_3.ICANFault2);
            row_17_column_11_FA.Fill = GetBrush(container_4.ICANFault2);
            row_17_column_12_FA.Fill = GetBrush(container_5.ICANFault2);
            row_17_column_13_FA.Fill = GetBrush(container_6.ICANFault2);
            // OCAN1
            row_18_column_8_FA.Fill = GetBrush(container_1.OCANFault1);
            row_18_column_9_FA.Fill = GetBrush(container_2.OCANFault1);
            row_18_column_10_FA.Fill = GetBrush(container_3.OCANFault1);
            row_18_column_11_FA.Fill = GetBrush(container_4.OCANFault1);
            row_18_column_12_FA.Fill = GetBrush(container_5.OCANFault1);
            row_18_column_13_FA.Fill = GetBrush(container_6.OCANFault1);
            // OCAN2
            row_19_column_8_FA.Fill = GetBrush(container_1.OCANFault2);
            row_19_column_9_FA.Fill = GetBrush(container_2.OCANFault2);
            row_19_column_10_FA.Fill = GetBrush(container_3.OCANFault2);
            row_19_column_11_FA.Fill = GetBrush(container_4.OCANFault2);
            row_19_column_12_FA.Fill = GetBrush(container_5.OCANFault2);
            row_19_column_13_FA.Fill = GetBrush(container_6.OCANFault2);

            //WSP1轴保压故障
            row_19_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_2_FA.Fill = GetBrush(container_2.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_3_FA.Fill = GetBrush(container_3.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_4_FA.Fill = GetBrush(container_4.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_5_FA.Fill = GetBrush(container_5.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_6_FA.Fill = GetBrush(container_6.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_1);
            row_19_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_1);

            //WSP2轴保压故障
            row_20_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_2_FA.Fill = GetBrush(container_2.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_3_FA.Fill = GetBrush(container_3.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_4_FA.Fill = GetBrush(container_4.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_5_FA.Fill = GetBrush(container_5.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_6_FA.Fill = GetBrush(container_6.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_2);
            row_20_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_2);

            //WSP1轴排风故障
            row_20_column_8_FA.Fill = GetBrush(container_1.WSPContinueExaustAirTimeOutFault_1);
            row_20_column_9_FA.Fill = GetBrush(container_2.WSPContinueExaustAirTimeOutFault_1);
            row_20_column_10_FA.Fill = GetBrush(container_3.WSPContinueExaustAirTimeOutFault_1);
            row_20_column_11_FA.Fill = GetBrush(container_4.WSPContinueExaustAirTimeOutFault_1);
            row_20_column_12_FA.Fill = GetBrush(container_5.WSPContinueExaustAirTimeOutFault_1);
            row_20_column_13_FA.Fill = GetBrush(container_6.WSPContinueExaustAirTimeOutFault_1);

            //WSP2轴排风故障
            row_21_column_8_FA.Fill = GetBrush(container_1.WSPContinueExaustAirTimeOutFault_2);
            row_21_column_9_FA.Fill = GetBrush(container_2.WSPContinueExaustAirTimeOutFault_2);
            row_21_column_10_FA.Fill = GetBrush(container_3.WSPContinueExaustAirTimeOutFault_2);
            row_21_column_11_FA.Fill = GetBrush(container_4.WSPContinueExaustAirTimeOutFault_2);
            row_21_column_12_FA.Fill = GetBrush(container_5.WSPContinueExaustAirTimeOutFault_2);
            row_21_column_13_FA.Fill = GetBrush(container_6.WSPContinueExaustAirTimeOutFault_2);

            //轴1排风故障
            row_21_column_1_FA.Fill = GetBrush(container_1.exhaustFault_1);
            row_21_column_2_FA.Fill = GetBrush(container_2.exhaustFault_1);
            row_21_column_3_FA.Fill = GetBrush(container_3.exhaustFault_1);
            row_21_column_4_FA.Fill = GetBrush(container_4.exhaustFault_1);
            row_21_column_5_FA.Fill = GetBrush(container_5.exhaustFault_1);
            row_21_column_6_FA.Fill = GetBrush(container_6.exhaustFault_1);

            // 轴1保压故障
            row_22_column_1_FA.Fill = GetBrush(container_1.keepPressureFault_1);
            row_22_column_2_FA.Fill = GetBrush(container_2.keepPressureFault_1);
            row_22_column_3_FA.Fill = GetBrush(container_3.keepPressureFault_1);
            row_22_column_4_FA.Fill = GetBrush(container_4.keepPressureFault_1);
            row_22_column_5_FA.Fill = GetBrush(container_5.keepPressureFault_1);
            row_22_column_6_FA.Fill = GetBrush(container_6.keepPressureFault_1);

            // 轴1充风故障
            row_23_column_1_FA.Fill = GetBrush(container_1.chargingFault_1);
            row_23_column_2_FA.Fill = GetBrush(container_2.chargingFault_1);
            row_23_column_3_FA.Fill = GetBrush(container_3.chargingFault_1);
            row_23_column_4_FA.Fill = GetBrush(container_4.chargingFault_1);
            row_23_column_5_FA.Fill = GetBrush(container_5.chargingFault_1);
            row_23_column_6_FA.Fill = GetBrush(container_6.chargingFault_1);

            // 连接阀充风故障
            row_24_column_1_FA.Fill = GetBrush(container_1.connectionValveAirFillingFailure);
            row_24_column_2_FA.Fill = GetBrush(container_2.connectionValveAirFillingFailure);
            row_24_column_3_FA.Fill = GetBrush(container_3.connectionValveAirFillingFailure);
            row_24_column_4_FA.Fill = GetBrush(container_4.connectionValveAirFillingFailure);
            row_24_column_5_FA.Fill = GetBrush(container_5.connectionValveAirFillingFailure);
            row_24_column_6_FA.Fill = GetBrush(container_6.connectionValveAirFillingFailure);

            //轴2排风故障
            row_25_column_1_FA.Fill = GetBrush(container_1.exhuastFault_2);
            row_25_column_2_FA.Fill = GetBrush(container_2.exhuastFault_2);
            row_25_column_3_FA.Fill = GetBrush(container_3.exhuastFault_2);
            row_25_column_4_FA.Fill = GetBrush(container_4.exhuastFault_2);
            row_25_column_5_FA.Fill = GetBrush(container_5.exhuastFault_2);
            row_25_column_6_FA.Fill = GetBrush(container_6.exhuastFault_2);

            // 轴2保压故障
            row_26_column_1_FA.Fill = GetBrush(container_1.keepPressureFault_2);
            row_26_column_2_FA.Fill = GetBrush(container_2.keepPressureFault_2);
            row_26_column_3_FA.Fill = GetBrush(container_3.keepPressureFault_2);
            row_26_column_4_FA.Fill = GetBrush(container_4.keepPressureFault_2);
            row_26_column_5_FA.Fill = GetBrush(container_5.keepPressureFault_2);
            row_26_column_6_FA.Fill = GetBrush(container_6.keepPressureFault_2);

            // 轴2充风故障
            row_27_column_1_FA.Fill = GetBrush(container_1.chargingFault_2);
            row_27_column_2_FA.Fill = GetBrush(container_2.chargingFault_2);
            row_27_column_3_FA.Fill = GetBrush(container_3.chargingFault_2);
            row_27_column_4_FA.Fill = GetBrush(container_4.chargingFault_2);
            row_27_column_5_FA.Fill = GetBrush(container_5.chargingFault_2);
            row_27_column_6_FA.Fill = GetBrush(container_6.chargingFault_2);

            // 连接阀排风故障
            row_28_column_1_FA.Fill = GetBrush(container_1.connectionValveVentilationFailure);
            row_28_column_2_FA.Fill = GetBrush(container_2.connectionValveVentilationFailure);
            row_28_column_3_FA.Fill = GetBrush(container_3.connectionValveVentilationFailure);
            row_28_column_4_FA.Fill = GetBrush(container_4.connectionValveVentilationFailure);
            row_28_column_5_FA.Fill = GetBrush(container_5.connectionValveVentilationFailure);
            row_28_column_6_FA.Fill = GetBrush(container_6.connectionValveVentilationFailure);

            // 轴1WSP连续排风超时故障
            row_29_column_1_FA.Fill = GetBrush(container_1.WSPContinueExaustAirTimeOutFault_1);
            row_29_column_2_FA.Fill = GetBrush(container_2.WSPContinueExaustAirTimeOutFault_1);
            row_29_column_3_FA.Fill = GetBrush(container_3.WSPContinueExaustAirTimeOutFault_1);
            row_29_column_4_FA.Fill = GetBrush(container_4.WSPContinueExaustAirTimeOutFault_1);
            row_29_column_5_FA.Fill = GetBrush(container_5.WSPContinueExaustAirTimeOutFault_1);
            row_29_column_6_FA.Fill = GetBrush(container_6.WSPContinueExaustAirTimeOutFault_1);

            // 轴2WSP连续排风故障
            row_30_column_1_FA.Fill = GetBrush(container_1.WSPContinuousExhaustFailure_2);
            row_30_column_2_FA.Fill = GetBrush(container_2.WSPContinuousExhaustFailure_2);
            row_30_column_3_FA.Fill = GetBrush(container_3.WSPContinuousExhaustFailure_2);
            row_30_column_4_FA.Fill = GetBrush(container_4.WSPContinuousExhaustFailure_2);
            row_30_column_5_FA.Fill = GetBrush(container_5.WSPContinuousExhaustFailure_2);
            row_30_column_6_FA.Fill = GetBrush(container_6.WSPContinuousExhaustFailure_2);

            // 轴1WSP连续保压超时故障
            row_31_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_1);
            row_31_column_2_FA.Fill = GetBrush(container_2.WSPContinueKeepPressureTimeOutFault_1);
            row_31_column_3_FA.Fill = GetBrush(container_3.WSPContinueKeepPressureTimeOutFault_1);
            row_31_column_4_FA.Fill = GetBrush(container_4.WSPContinueKeepPressureTimeOutFault_1);
            row_31_column_5_FA.Fill = GetBrush(container_5.WSPContinueKeepPressureTimeOutFault_1);
            row_31_column_6_FA.Fill = GetBrush(container_6.WSPContinueKeepPressureTimeOutFault_1);

            // 轴2WSP连续保压超时故障
            row_32_column_1_FA.Fill = GetBrush(container_1.WSPContinueKeepPressureTimeOutFault_2);
            row_32_column_2_FA.Fill = GetBrush(container_2.WSPContinueKeepPressureTimeOutFault_2);
            row_32_column_3_FA.Fill = GetBrush(container_3.WSPContinueKeepPressureTimeOutFault_2);
            row_32_column_4_FA.Fill = GetBrush(container_4.WSPContinueKeepPressureTimeOutFault_2);
            row_32_column_5_FA.Fill = GetBrush(container_5.WSPContinueKeepPressureTimeOutFault_2);
            row_32_column_6_FA.Fill = GetBrush(container_6.WSPContinueKeepPressureTimeOutFault_2);

            // VLD充风故障
            row_22_column_8_FA.Fill = GetBrush(container_1.VLDChargingFault);
            row_22_column_9_FA.Fill = GetBrush(container_2.VLDChargingFault);
            row_22_column_10_FA.Fill = GetBrush(container_3.VLDChargingFault);
            row_22_column_11_FA.Fill = GetBrush(container_4.VLDChargingFault);
            row_22_column_12_FA.Fill = GetBrush(container_5.VLDChargingFault);
            row_22_column_13_FA.Fill = GetBrush(container_6.VLDChargingFault);

            // VLD排风故障
            row_23_column_8_FA.Fill = GetBrush(container_1.VLDExhuastFault);
            row_23_column_9_FA.Fill = GetBrush(container_2.VLDExhuastFault);
            row_23_column_10_FA.Fill = GetBrush(container_3.VLDExhuastFault);
            row_23_column_11_FA.Fill = GetBrush(container_4.VLDExhuastFault);
            row_23_column_12_FA.Fill = GetBrush(container_5.VLDExhuastFault);
            row_23_column_13_FA.Fill = GetBrush(container_6.VLDExhuastFault);

            // VLD保压故障
            row_24_column_8_FA.Fill = GetBrush(container_1.VLDKeepPressureFault);
            row_24_column_9_FA.Fill = GetBrush(container_2.VLDKeepPressureFault);
            row_24_column_10_FA.Fill = GetBrush(container_3.VLDKeepPressureFault);
            row_24_column_11_FA.Fill = GetBrush(container_4.VLDKeepPressureFault);
            row_24_column_12_FA.Fill = GetBrush(container_5.VLDKeepPressureFault);
            row_24_column_13_FA.Fill = GetBrush(container_6.VLDKeepPressureFault);

            // 轴1WSP排风故障
            row_25_column_1_FA.Fill = GetBrush(container_1.WSPExhuastFault_1);
            row_25_column_2_FA.Fill = GetBrush(container_2.WSPExhuastFault_1);
            row_25_column_3_FA.Fill = GetBrush(container_3.WSPExhuastFault_1);
            row_25_column_4_FA.Fill = GetBrush(container_4.WSPExhuastFault_1);
            row_25_column_5_FA.Fill = GetBrush(container_5.WSPExhuastFault_1);
            row_25_column_6_FA.Fill = GetBrush(container_6.WSPExhuastFault_1);

            // 轴1WSP充风故障
            row_26_column_1_FA.Fill = GetBrush(container_1.WSPChargingFault_1);
            row_26_column_2_FA.Fill = GetBrush(container_2.WSPChargingFault_1);
            row_26_column_3_FA.Fill = GetBrush(container_3.WSPChargingFault_1);
            row_26_column_4_FA.Fill = GetBrush(container_4.WSPChargingFault_1);
            row_26_column_5_FA.Fill = GetBrush(container_5.WSPChargingFault_1);
            row_26_column_6_FA.Fill = GetBrush(container_6.WSPChargingFault_1);

            // 轴2WSP排风故障
            row_27_column_1_FA.Fill = GetBrush(container_1.WSPExhuastFault_2);
            row_27_column_2_FA.Fill = GetBrush(container_2.WSPExhuastFault_2);
            row_27_column_3_FA.Fill = GetBrush(container_3.WSPExhuastFault_2);
            row_27_column_4_FA.Fill = GetBrush(container_4.WSPExhuastFault_2);
            row_27_column_5_FA.Fill = GetBrush(container_5.WSPExhuastFault_2);
            row_27_column_6_FA.Fill = GetBrush(container_6.WSPExhuastFault_2);

            // 轴2WSP充风故障
            row_28_column_1_FA.Fill = GetBrush(container_1.WSPChargingFault_2);
            row_28_column_2_FA.Fill = GetBrush(container_2.WSPChargingFault_2);
            row_28_column_3_FA.Fill = GetBrush(container_3.WSPChargingFault_2);
            row_28_column_4_FA.Fill = GetBrush(container_4.WSPChargingFault_2);
            row_28_column_5_FA.Fill = GetBrush(container_5.WSPChargingFault_2);
            row_28_column_6_FA.Fill = GetBrush(container_6.WSPChargingFault_2);



        }

        private SolidColorBrush GetBrush(bool b)
        {
            return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
        }

        private static Queue<int> LifeSigQueue_1 = new Queue<int>(); // 创建一个针对1车的用于存储生命信号的队列
        // 2020-1-15
        private bool SaveLifeSig(int LifeSig)
        {
            if (LifeSigQueue_1.Count < 5)  // 限制队列长度最大只能是5个（对应你说的5条记录）
            {
                LifeSigQueue_1.Enqueue(LifeSig); // 当队列长度小于5时候，入队
            }
            else
            {
                foreach (int ele in LifeSigQueue_1) // 当队列已满时，将新来的生命信号与队列中各个元素进行比较
                {
                    if (LifeSig != ele) // 如果发现有不相等的，返回true，代表没问题
                    {
                        LifeSigQueue_1.Dequeue();
                        LifeSigQueue_1.Enqueue(LifeSig);
                        return true;
                    }
                }
                LifeSigQueue_1.Dequeue();
                LifeSigQueue_1.Enqueue(LifeSig);
                return false; // 否则返回false，代表队列中的数据和新来的全都一样                             
            }
            return true;
        }

        private static Queue<int> LifeSigQueue_6 = new Queue<int>(); // 创建一个针对6车的用于存储生命信号的队列
        // 2020-1-15
        private bool SaveLifeSig_6(int LifeSig)
        {
            if (LifeSigQueue_6.Count < 5)  // 限制队列长度最大只能是5个（对应你说的5条记录）
            {
                LifeSigQueue_6.Enqueue(LifeSig); // 当队列长度小于5时候，入队
            }
            else
            {
                foreach (int ele in LifeSigQueue_6) // 当队列已满时，将新来的生命信号与队列中各个元素进行比较
                {
                    if (LifeSig != ele) // 如果发现有不相等的，返回true，代表没问题
                    {
                        LifeSigQueue_6.Dequeue();
                        LifeSigQueue_6.Enqueue(LifeSig);
                        return true;
                    }
                }
                LifeSigQueue_6.Dequeue();
                LifeSigQueue_6.Enqueue(LifeSig);
                return false; // 否则返回false，代表队列中的数据和新来的全都一样                             
            }
            return true;
        }

        private int Check(int value, int ax) // 以Check函数为例
        {
            if (ax == 1) // 如果是1车
            {
                if ((!container_6.GateValveState) && LifeSig_1) // 判断条件为 【非6车网关阀状态 + 新加的函数判断为true】才返回value值
                {
                    return value;
                }
                else
                {
                    return 0; // 否则返回0
                }
            }
            if (ax == 6) // 同理6车
            {
                if ((!container_1.GateValveState) && LifeSig_6)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        private int Check_Int(int value, int ax)
        {
            if (container_1.GateValveState == true && container_6.GateValveState == true && LifeSig_1 && LifeSig_6)
            {
                return value;
            }
            if (ax == 1)
            {
                if ((!container_6.GateValveState) && LifeSig_1)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            if (ax == 6)
            {
                if ((!container_1.GateValveState) && LifeSig_6)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }


        private double Check_Double(double value, int ax)
        {
            if (container_1.GateValveState == true && container_6.GateValveState == true && LifeSig_1 && LifeSig_6)
            {
                return value;
            }
            if (ax == 1)
            {
                if ((!container_6.GateValveState) && LifeSig_1)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            if (ax == 6)
            {
                if ((!container_1.GateValveState) && LifeSig_6)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }



        private SolidColorBrush GetBrush(bool b, int ax)
        {
            if (ax == 1)
            {
                if (!container_6.GateValveState)
                {
                    return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGray);

                }
            }
            else if (ax == 6)
            {
                if (!container_1.GateValveState)
                {
                    return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGray);
                }
            }
            return null;
        }


        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.container_1 = container_1;
            this.container_2 = container_2;
            this.container_3 = container_3;
            this.container_4 = container_4;
            this.container_5 = container_5;
            this.container_6 = container_6;
        }



        #endregion



        #region window method
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3;
            this.Height = y1 * 4 / 5;
        }

        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OverviewWindow_Closed(object sender, EventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// 上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if (nowRecord == 1 || nowRecord - step < 1)
            {
                MessageBox.Show("已经到达顶部");
                return;
            }
            nowRecord -= step;
            StepTo(nowRecord);
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void afterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (nowRecord == totalRecord || nowRecord + step > totalRecord)
            {
                MessageBox.Show("已经到达尾部");
                return;
            }
            nowRecord += step;
            StepTo(nowRecord);
        }

        private void hisProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init == true)
            {
                nowRecord = (int)hisProgress.Value;
                StepTo(nowRecord);
            }

        }
    }
}
