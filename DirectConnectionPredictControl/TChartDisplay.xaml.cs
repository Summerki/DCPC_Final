using DirectConnectionPredictControl.CommenTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Steema.TeeChart.WPF;
using System.Threading;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// TChartDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class TChartDisplay : Window
    {

        // 数据组
        private MainDevDataContains Container1_TChart = new MainDevDataContains();
        private MainDevDataContains Container6_TChart = new MainDevDataContains();
        private SliverDataContainer Container2_TChart = new SliverDataContainer();
        private SliverDataContainer Container3_TChart = new SliverDataContainer();
        private SliverDataContainer Container4_TChart = new SliverDataContainer();
        private SliverDataContainer Container5_TChart = new SliverDataContainer();
        
        public TChartDisplay()
        {
            InitializeComponent();
            Init();

            AddLines();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                // 减速度
                accValueTChart.Series[0].Add(DateTime.Now, Container1_TChart.AccValue1);
                accValueTChart.Series[1].Add(DateTime.Now, Container1_TChart.AccValue2);
                accValueTChart.Series[2].Add(DateTime.Now, Container2_TChart.AccValue1);
                accValueTChart.Series[3].Add(DateTime.Now, Container2_TChart.AccValue2);
                accValueTChart.Series[4].Add(DateTime.Now, Container3_TChart.AccValue1);
                accValueTChart.Series[5].Add(DateTime.Now, Container3_TChart.AccValue2);
                accValueTChart.Series[6].Add(DateTime.Now, Container4_TChart.AccValue1);
                accValueTChart.Series[7].Add(DateTime.Now, Container4_TChart.AccValue2);
                accValueTChart.Series[8].Add(DateTime.Now, Container5_TChart.AccValue1);
                accValueTChart.Series[9].Add(DateTime.Now, Container5_TChart.AccValue2);
                accValueTChart.Series[10].Add(DateTime.Now, Container6_TChart.AccValue1);
                accValueTChart.Series[11].Add(DateTime.Now, Container6_TChart.AccValue2);

                // 参考速度
                refSpeedTChart.Series[0].Add(DateTime.Now, Container1_TChart.RefSpeed);
                refSpeedTChart.Series[1].Add(DateTime.Now, Container6_TChart.RefSpeed);

                // 紧急硬线
                emergencyHardTChart.Series[0].Add(DateTime.Now, BoolToInt(Container1_TChart.HardEmergencyBrake));
                emergencyHardTChart.Series[1].Add(DateTime.Now, BoolToInt(Container6_TChart.HardEmergencyBrake));

                // 制动硬线和制动网络
                brakeHardTChart.Series[0].Add(DateTime.Now, BoolToInt(Container1_TChart.HardBrakeCmd));
                brakeHardTChart.Series[1].Add(DateTime.Now, BoolToInt(Container6_TChart.HardBrakeCmd));
                brakeHardTChart.Series[2].Add(DateTime.Now, BoolToInt(Container1_TChart.NetBrakeCmd));
                brakeHardTChart.Series[3].Add(DateTime.Now, BoolToInt(Container6_TChart.NetBrakeCmd));


                // 快制硬线和快制网络
                fastHardTChart.Series[0].Add(DateTime.Now, BoolToInt(Container1_TChart.HardFastBrakeCmd));
                fastHardTChart.Series[1].Add(DateTime.Now, BoolToInt(Container6_TChart.HardFastBrakeCmd));
                fastHardTChart.Series[2].Add(DateTime.Now, BoolToInt(Container1_TChart.NetFastBrakeCmd));
                fastHardTChart.Series[3].Add(DateTime.Now, BoolToInt(Container6_TChart.NetFastBrakeCmd));

                // 整车制动力有效
                enableTrainBrakeForceTChart.Series[0].Add(DateTime.Now, BoolToInt(Container1_TChart.TrainBrakeEnable));
                enableTrainBrakeForceTChart.Series[1].Add(DateTime.Now, BoolToInt(Container6_TChart.TrainBrakeEnable));

                // 整车制动力
                trainBrakeForceTChart.Series[0].Add(DateTime.Now, Container1_TChart.TrainBrakeForce);
                trainBrakeForceTChart.Series[1].Add(DateTime.Now, Container6_TChart.TrainBrakeForce);

                // 制动级位
                brakeLevelTChart.Series[0].Add(DateTime.Now, Container1_TChart.BrakeLevel);
                brakeLevelTChart.Series[1].Add(DateTime.Now, Container6_TChart.BrakeLevel);

                // 制动缸压力
                pressureTChart.Series[0].Add(DateTime.Now, Container1_TChart.Bcp1PressureAx1);
                pressureTChart.Series[1].Add(DateTime.Now, Container2_TChart.Bcp1Pressure);
                pressureTChart.Series[2].Add(DateTime.Now, Container3_TChart.Bcp1Pressure);
                pressureTChart.Series[3].Add(DateTime.Now, Container4_TChart.Bcp1Pressure);
                pressureTChart.Series[4].Add(DateTime.Now, Container5_TChart.Bcp1Pressure);
                pressureTChart.Series[5].Add(DateTime.Now, Container6_TChart.Bcp1PressureAx1);
                pressureTChart.Series[6].Add(DateTime.Now, Container1_TChart.Bcp2PressureAx2);
                pressureTChart.Series[7].Add(DateTime.Now, Container2_TChart.Bcp2Pressure);
                pressureTChart.Series[8].Add(DateTime.Now, Container3_TChart.Bcp2Pressure);
                pressureTChart.Series[9].Add(DateTime.Now, Container4_TChart.Bcp2Pressure);
                pressureTChart.Series[10].Add(DateTime.Now, Container5_TChart.Bcp2Pressure);
                pressureTChart.Series[11].Add(DateTime.Now, Container6_TChart.Bcp2PressureAx2);

                // 电制动淡出
                dcuEbFadeoutTChart.Series[0].Add(DateTime.Now, BoolToInt(Container1_TChart.EdFadeOut));
                dcuEbFadeoutTChart.Series[1].Add(DateTime.Now, BoolToInt(Container6_TChart.EdFadeOut));

                // 电制动实际值
                dcu1EbRealValueTChart.Series[0].Add(DateTime.Now, Container1_TChart.DcuEbRealValue[0]);
                dcu1EbRealValueTChart.Series[1].Add(DateTime.Now, Container6_TChart.DcuEbRealValue[0]);
                dcu1EbRealValueTChart.Series[2].Add(DateTime.Now, Container1_TChart.DcuEbRealValue[1]);
                dcu1EbRealValueTChart.Series[3].Add(DateTime.Now, Container6_TChart.DcuEbRealValue[1]);
                dcu1EbRealValueTChart.Series[4].Add(DateTime.Now, Container1_TChart.DcuEbRealValue[2]);
                dcu1EbRealValueTChart.Series[5].Add(DateTime.Now, Container6_TChart.DcuEbRealValue[2]);
                dcu1EbRealValueTChart.Series[6].Add(DateTime.Now, Container1_TChart.DcuEbRealValue[3]);
                dcu1EbRealValueTChart.Series[7].Add(DateTime.Now, Container6_TChart.DcuEbRealValue[3]);

            }));
        }


        private int BoolToInt(bool temp)
        {
            return temp ? 1 : 0;
        }

        private void Init()
        {
            #region accValueTChart初始化
            accValueTChart.Aspect.View3D = false;//控件3D效果
            accValueTChart.Legend.CheckBoxes = true;//是否需要勾选
            accValueTChart.Legend.Visible = true;//直线标题集合是否显示
            accValueTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            accValueTChart.Header.Text = "accValueTChart";//Tchart窗体标题
            accValueTChart.Axes.Left.Title.Text = "减速度m/s²";//左侧标题
            accValueTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            accValueTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //accValueTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            accValueTChart.Axes.Left.SetMinMax(-1, 1);
            #endregion

            #region refSpeedTChart初始化
            refSpeedTChart.Aspect.View3D = false;//控件3D效果
            refSpeedTChart.Legend.CheckBoxes = true;//是否需要勾选
            refSpeedTChart.Legend.Visible = true;//直线标题集合是否显示
            refSpeedTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            refSpeedTChart.Header.Text = "refSpeedTChart";//Tchart窗体标题
            refSpeedTChart.Axes.Left.Title.Text = "参考速度km/h";//左侧标题
            refSpeedTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            refSpeedTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //refSpeedTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            refSpeedTChart.Axes.Left.SetMinMax(0, 90);


            // 以下是光标入口
            //slipRateCursorTool = new CursorTool(accValueTChart.Chart);
            //slipRateCursorTool.Active = true;
            //slipRateCursorTool.FollowMouse = true;
            //slipRateCursorTool.Style = CursorToolStyles.Both;
            #endregion

            #region emergencyHardTChart初始化
            emergencyHardTChart.Aspect.View3D = false;//控件3D效果
            emergencyHardTChart.Legend.CheckBoxes = true;//是否需要勾选
            emergencyHardTChart.Legend.Visible = true;//直线标题集合是否显示
            emergencyHardTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            emergencyHardTChart.Header.Text = "emergencyHardTChart";//Tchart窗体标题
            emergencyHardTChart.Axes.Left.Title.Text = "紧急硬线";//左侧标题
            emergencyHardTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            emergencyHardTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //emergencyHardTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            emergencyHardTChart.Axes.Left.SetMinMax(0, 1);
            #endregion

            #region brakeHardTChart初始化
            brakeHardTChart.Aspect.View3D = false;//控件3D效果
            brakeHardTChart.Legend.CheckBoxes = true;//是否需要勾选
            brakeHardTChart.Legend.Visible = true;//直线标题集合是否显示
            brakeHardTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            brakeHardTChart.Header.Text = "brakeHardTChart";//Tchart窗体标题
            brakeHardTChart.Axes.Left.Title.Text = "制动硬线和制动网络";//左侧标题
            brakeHardTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            brakeHardTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //brakeHardTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            brakeHardTChart.Axes.Left.SetMinMax(0, 1);
            #endregion

            #region fastHardTChart初始化
            fastHardTChart.Aspect.View3D = false;//控件3D效果
            fastHardTChart.Legend.CheckBoxes = true;//是否需要勾选
            fastHardTChart.Legend.Visible = true;//直线标题集合是否显示
            fastHardTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            fastHardTChart.Header.Text = "fastHardTChart";//Tchart窗体标题
            fastHardTChart.Axes.Left.Title.Text = "快制硬线和快制网络";//左侧标题
            fastHardTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            fastHardTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //fastHardTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            fastHardTChart.Axes.Left.SetMinMax(0, 1);
            #endregion

            #region enableTrainBrakeForceTChart初始化
            enableTrainBrakeForceTChart.Aspect.View3D = false;//控件3D效果
            enableTrainBrakeForceTChart.Legend.CheckBoxes = true;//是否需要勾选
            enableTrainBrakeForceTChart.Legend.Visible = true;//直线标题集合是否显示
            enableTrainBrakeForceTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            enableTrainBrakeForceTChart.Header.Text = "enableTrainBrakeForceTChart";//Tchart窗体标题
            enableTrainBrakeForceTChart.Axes.Left.Title.Text = "整车制动力有效";//左侧标题
            enableTrainBrakeForceTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            enableTrainBrakeForceTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //enableTrainBrakeForceTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            enableTrainBrakeForceTChart.Axes.Left.SetMinMax(0, 1);
            #endregion

            #region trainBrakeForceTChart初始化
            trainBrakeForceTChart.Aspect.View3D = false;//控件3D效果
            trainBrakeForceTChart.Legend.CheckBoxes = true;//是否需要勾选
            trainBrakeForceTChart.Legend.Visible = true;//直线标题集合是否显示
            trainBrakeForceTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            trainBrakeForceTChart.Header.Text = "trainBrakeForceTChart";//Tchart窗体标题
            trainBrakeForceTChart.Axes.Left.Title.Text = "整车制动力N";//左侧标题
            trainBrakeForceTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            trainBrakeForceTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //trainBrakeForceTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            trainBrakeForceTChart.Axes.Left.SetMinMax(0, 4000);
            #endregion

            #region brakeLevelTChart初始化
            brakeLevelTChart.Aspect.View3D = false;//控件3D效果
            brakeLevelTChart.Legend.CheckBoxes = true;//是否需要勾选
            brakeLevelTChart.Legend.Visible = true;//直线标题集合是否显示
            brakeLevelTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            brakeLevelTChart.Header.Text = "brakeLevelTChart";//Tchart窗体标题
            brakeLevelTChart.Axes.Left.Title.Text = "制动级位";//左侧标题
            brakeLevelTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            brakeLevelTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //brakeLevelTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            brakeLevelTChart.Axes.Left.SetMinMax(0, 10000);
            #endregion

            #region pressureTChart初始化
            pressureTChart.Aspect.View3D = false;//控件3D效果
            pressureTChart.Legend.CheckBoxes = true;//是否需要勾选
            pressureTChart.Legend.Visible = true;//直线标题集合是否显示
            pressureTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            pressureTChart.Header.Text = "pressureTChart";//Tchart窗体标题
            pressureTChart.Axes.Left.Title.Text = "制动缸压力KPa";//左侧标题
            pressureTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            pressureTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //pressureTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            pressureTChart.Axes.Left.SetMinMax(0, 600);
            #endregion

            #region dcuEbFadeoutTChart初始化
            dcuEbFadeoutTChart.Aspect.View3D = false;//控件3D效果
            dcuEbFadeoutTChart.Legend.CheckBoxes = true;//是否需要勾选
            dcuEbFadeoutTChart.Legend.Visible = true;//直线标题集合是否显示
            dcuEbFadeoutTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            dcuEbFadeoutTChart.Header.Text = "dcuEbFadeoutTChart";//Tchart窗体标题
            dcuEbFadeoutTChart.Axes.Left.Title.Text = "电制动淡出";//左侧标题
            dcuEbFadeoutTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            dcuEbFadeoutTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //dcuEbFadeoutTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            dcuEbFadeoutTChart.Axes.Left.SetMinMax(0, 1);
            #endregion

            #region dcu1EbRealValueTChart初始化
            dcu1EbRealValueTChart.Aspect.View3D = false;//控件3D效果
            dcu1EbRealValueTChart.Legend.CheckBoxes = true;//是否需要勾选
            dcu1EbRealValueTChart.Legend.Visible = true;//直线标题集合是否显示
            dcu1EbRealValueTChart.Legend.Alignment = LegendAlignments.Right;//直接标题右边显示
            dcu1EbRealValueTChart.Header.Text = "dcu1EbRealValueTChart";//Tchart窗体标题
            dcu1EbRealValueTChart.Axes.Left.Title.Text = "电制动实际值";//左侧标题
            dcu1EbRealValueTChart.Axes.Bottom.Title.Text = "时间";//底部标题
            dcu1EbRealValueTChart.Axes.Bottom.Labels.DateTimeFormat = "HH:mm:ss"; // 将x轴坐标格式化为自己想要的格式
            //dcu1EbRealValueTChart.Axes.Bottom.Labels.Angle = 45; // 将X轴的刻度显示旋转90°
            dcu1EbRealValueTChart.Axes.Left.SetMinMax(0, 800);
            #endregion
        }

        private void AddLines()
        {
            // new出来减速度
            Steema.TeeChart.WPF.Styles.Line accValue1_1 = new Steema.TeeChart.WPF.Styles.Line();//1架1轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue1_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue2_1 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue2_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue3_1 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue3_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue4_1 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue4_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue5_1 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue5_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue6_1 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            Steema.TeeChart.WPF.Styles.Line accValue6_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴减速度
            // new 出来参考速度
            Steema.TeeChart.WPF.Styles.Line refSpeed1 = new Steema.TeeChart.WPF.Styles.Line();//1架参考速度
            Steema.TeeChart.WPF.Styles.Line refSpeed6 = new Steema.TeeChart.WPF.Styles.Line();//6架参考速度

            // new 出来紧急硬线
            Steema.TeeChart.WPF.Styles.Line emergencyHard1 = new Steema.TeeChart.WPF.Styles.Line();//紧急硬线_1
            Steema.TeeChart.WPF.Styles.Line emergencyHard6 = new Steema.TeeChart.WPF.Styles.Line();//紧急硬线_6

            // new 出来制动硬线和制动网络
            Steema.TeeChart.WPF.Styles.Line brakeHard1 = new Steema.TeeChart.WPF.Styles.Line();//制动硬线_1
            Steema.TeeChart.WPF.Styles.Line brakeHard6 = new Steema.TeeChart.WPF.Styles.Line();//制动硬线_6
            Steema.TeeChart.WPF.Styles.Line brakeInternet1 = new Steema.TeeChart.WPF.Styles.Line();//制动网络_1
            Steema.TeeChart.WPF.Styles.Line brakeInternet6 = new Steema.TeeChart.WPF.Styles.Line();//制动网络_6

            // new 出来快制硬线和快制网络
            Steema.TeeChart.WPF.Styles.Line fastBrakeHard1 = new Steema.TeeChart.WPF.Styles.Line();//快制硬线_1
            Steema.TeeChart.WPF.Styles.Line fastBrakeHard6 = new Steema.TeeChart.WPF.Styles.Line();//快制硬线_6
            Steema.TeeChart.WPF.Styles.Line fastBrakeInternet1 = new Steema.TeeChart.WPF.Styles.Line();//快制网络_1
            Steema.TeeChart.WPF.Styles.Line fastBrakeInternet6 = new Steema.TeeChart.WPF.Styles.Line();//快制网络_6

            // new 出来整车制动力有效
            Steema.TeeChart.WPF.Styles.Line enableTrainBrakeForce1 = new Steema.TeeChart.WPF.Styles.Line();//整车制动力有效_1
            Steema.TeeChart.WPF.Styles.Line enableTrainBrakeForce6 = new Steema.TeeChart.WPF.Styles.Line();//整车制动力有效_6

            // 整车制动力
            Steema.TeeChart.WPF.Styles.Line trainBrakeForce1 = new Steema.TeeChart.WPF.Styles.Line();//整车制动力_1
            Steema.TeeChart.WPF.Styles.Line trainBrakeForce6 = new Steema.TeeChart.WPF.Styles.Line();//整车制动力_6

            // 制动级位
            Steema.TeeChart.WPF.Styles.Line brakeLevel1 = new Steema.TeeChart.WPF.Styles.Line();//制动级位1
            Steema.TeeChart.WPF.Styles.Line brakeLevel6 = new Steema.TeeChart.WPF.Styles.Line();//制动级位6

            // 制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure1_1 = new Steema.TeeChart.WPF.Styles.Line();//1架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure2_1 = new Steema.TeeChart.WPF.Styles.Line();//2架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure3_1 = new Steema.TeeChart.WPF.Styles.Line();//3架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure4_1 = new Steema.TeeChart.WPF.Styles.Line();//4架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure5_1 = new Steema.TeeChart.WPF.Styles.Line();//5架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure6_1 = new Steema.TeeChart.WPF.Styles.Line();//6架1轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure1_2 = new Steema.TeeChart.WPF.Styles.Line();//1架2轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure2_2 = new Steema.TeeChart.WPF.Styles.Line();//2架2轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure3_2 = new Steema.TeeChart.WPF.Styles.Line();//3架2轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure4_2 = new Steema.TeeChart.WPF.Styles.Line();//4架2轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure5_2 = new Steema.TeeChart.WPF.Styles.Line();//5架2轴制动缸
            Steema.TeeChart.WPF.Styles.Line antiskidPressure6_2 = new Steema.TeeChart.WPF.Styles.Line();//6架2轴制动缸

            // 电制动淡出
            Steema.TeeChart.WPF.Styles.Line dcuEbFadeout1 = new Steema.TeeChart.WPF.Styles.Line();//电制动淡出_1
            Steema.TeeChart.WPF.Styles.Line dcuEbFadeout6 = new Steema.TeeChart.WPF.Styles.Line();//电制动淡出_6

            // 电制动实际值
            Steema.TeeChart.WPF.Styles.Line dcu1EbRealValue1 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_B1_1
            Steema.TeeChart.WPF.Styles.Line dcu1EbRealValue6 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_B1_6
            Steema.TeeChart.WPF.Styles.Line dcu2EbRealValue1 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_C1_1
            Steema.TeeChart.WPF.Styles.Line dcu2EbRealValue6 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_C1_6
            Steema.TeeChart.WPF.Styles.Line dcu3EbRealValue1 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_B2_1
            Steema.TeeChart.WPF.Styles.Line dcu3EbRealValue6 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_B2_6
            Steema.TeeChart.WPF.Styles.Line dcu4EbRealValue1 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_C2_1
            Steema.TeeChart.WPF.Styles.Line dcu4EbRealValue6 = new Steema.TeeChart.WPF.Styles.Line();//DCU1电制力_C2_6


            #region 设置减速度样式
            accValue1_1.Title = "1架1轴减速度";//标题
            accValue1_1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            accValue1_1.ShowInLegend = true;//是否显示直线标题

            accValue1_2.Title = "1架2轴减速度";//标题
            accValue1_2.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//直线颜色
            accValue1_2.ShowInLegend = true;//是否显示直线标题

            accValue2_1.Title = "2架1轴减速度";//标题
            accValue2_1.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//直线颜色
            accValue2_1.ShowInLegend = true;//是否显示直线标题

            accValue2_2.Title = "2架2轴减速度";//标题
            accValue2_2.Color = System.Windows.Media.Color.FromRgb(255, 255, 0);//直线颜色
            accValue2_2.ShowInLegend = true;//是否显示直线标题

            accValue3_1.Title = "3架1轴减速度";//标题
            accValue3_1.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//直线颜色
            accValue3_1.ShowInLegend = true;//是否显示直线标题

            accValue3_2.Title = "3架2轴减速度";//标题
            accValue3_2.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            accValue3_2.ShowInLegend = true;//是否显示直线标题

            accValue4_1.Title = "4架1轴减速度";//标题
            accValue4_1.Color = System.Windows.Media.Color.FromRgb(100, 0, 0);//直线颜色
            accValue4_1.ShowInLegend = true;//是否显示直线标题

            accValue4_2.Title = "4架2轴减速度";//标题
            accValue4_2.Color = System.Windows.Media.Color.FromRgb(0, 100, 0);//直线颜色
            accValue4_2.ShowInLegend = true;//是否显示直线标题

            accValue5_1.Title = "5架1轴减速度";//标题
            accValue5_1.Color = System.Windows.Media.Color.FromRgb(0, 0, 100);//直线颜色
            accValue5_1.ShowInLegend = true;//是否显示直线标题

            accValue5_2.Title = "5架2轴减速度";//标题
            accValue5_2.Color = System.Windows.Media.Color.FromRgb(100, 100, 0);//直线颜色
            accValue5_2.ShowInLegend = true;//是否显示直线标题

            accValue6_1.Title = "6架1轴减速度";//标题
            accValue6_1.Color = System.Windows.Media.Color.FromRgb(100, 0, 100);//直线颜色
            accValue6_1.ShowInLegend = true;//是否显示直线标题

            accValue6_2.Title = "6架2轴减速度";//标题
            accValue6_2.Color = System.Windows.Media.Color.FromRgb(0, 100, 100);//直线颜色
            accValue6_2.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置参考速度样式
            refSpeed1.Title = "1架参考速度";//标题
            refSpeed1.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//直线颜色
            refSpeed1.ShowInLegend = true;//是否显示直线标题

            refSpeed6.Title = "6架参考速度";//标题
            refSpeed6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            refSpeed6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置紧急硬线样式
            emergencyHard1.Title = "紧急硬线_1";//标题
            emergencyHard1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            emergencyHard1.ShowInLegend = true;//是否显示直线标题

            emergencyHard6.Title = "紧急硬线_6";//标题
            emergencyHard6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            emergencyHard6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置制动硬线和制动网络的样式
            brakeHard1.Title = "制动硬线_1";//标题
            brakeHard1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            brakeHard1.ShowInLegend = true;//是否显示直线标题

            brakeHard6.Title = "制动硬线_6";//标题
            brakeHard6.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//直线颜色
            brakeHard6.ShowInLegend = true;//是否显示直线标题

            brakeInternet1.Title = "制动网络_1";//标题
            brakeInternet1.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//直线颜色
            brakeInternet1.ShowInLegend = true;//是否显示直线标题

            brakeInternet6.Title = "制动网络_6";//标题
            brakeInternet6.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//直线颜色
            brakeInternet6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置快制硬线和快制网络样式
            fastBrakeHard1.Title = "快制硬线_6";//标题
            fastBrakeHard1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            fastBrakeHard1.ShowInLegend = true;//是否显示直线标题

            fastBrakeHard6.Title = "快制硬线_6";//标题
            fastBrakeHard6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            fastBrakeHard6.ShowInLegend = true;//是否显示直线标题

            fastBrakeInternet1.Title = "快制网络_1";//标题
            fastBrakeInternet1.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//直线颜色
            fastBrakeInternet1.ShowInLegend = true;//是否显示直线标题

            fastBrakeInternet6.Title = "快制网络_6";//标题
            fastBrakeInternet6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            fastBrakeInternet6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置整车制动有效样式
            enableTrainBrakeForce1.Title = "整车制动力有效_1";//标题
            enableTrainBrakeForce1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            enableTrainBrakeForce1.ShowInLegend = true;//是否显示直线标题

            enableTrainBrakeForce6.Title = "整车制动力有效_6";//标题
            enableTrainBrakeForce6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            enableTrainBrakeForce6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置整车制动力样式
            trainBrakeForce1.Title = "整车制动力_1";//标题
            trainBrakeForce1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            trainBrakeForce1.ShowInLegend = true;//是否显示直线标题

            trainBrakeForce6.Title = "整车制动力_6";//标题
            trainBrakeForce6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            trainBrakeForce6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置制动级位样式
            brakeLevel1.Title = "制动级位1";//标题
            brakeLevel1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            brakeLevel1.ShowInLegend = true;//是否显示直线标题

            brakeLevel6.Title = "制动级位6";//标题
            brakeLevel6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            brakeLevel6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置制动缸压力曲线样式
            antiskidPressure1_1.Title = "1架1轴制动缸";//标题
            antiskidPressure1_1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            antiskidPressure1_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure2_1.Title = "2架1轴制动缸";//标题
            antiskidPressure2_1.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//直线颜色
            antiskidPressure2_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure3_1.Title = "3架1轴制动缸";//标题
            antiskidPressure3_1.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//直线颜色
            antiskidPressure3_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure4_1.Title = "4架1轴制动缸";//标题
            antiskidPressure4_1.Color = System.Windows.Media.Color.FromRgb(255, 255, 0);//直线颜色
            antiskidPressure4_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure5_1.Title = "5架1轴制动缸";//标题
            antiskidPressure5_1.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            antiskidPressure5_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure6_1.Title = "6架1轴制动缸";//标题
            antiskidPressure6_1.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//直线颜色
            antiskidPressure6_1.ShowInLegend = true;//是否显示直线标题

            antiskidPressure1_2.Title = "1架2轴制动缸";//标题
            antiskidPressure1_2.Color = System.Windows.Media.Color.FromRgb(100, 0, 0);//直线颜色
            antiskidPressure1_2.ShowInLegend = true;//是否显示直线标题

            antiskidPressure2_2.Title = "2架2轴制动缸";//标题
            antiskidPressure2_2.Color = System.Windows.Media.Color.FromRgb(0, 100, 0);//直线颜色
            antiskidPressure2_2.ShowInLegend = true;//是否显示直线标题

            antiskidPressure3_2.Title = "3架2轴制动缸";//标题
            antiskidPressure3_2.Color = System.Windows.Media.Color.FromRgb(0, 0, 100);//直线颜色
            antiskidPressure3_2.ShowInLegend = true;//是否显示直线标题

            antiskidPressure4_2.Title = "4架2轴制动缸";//标题
            antiskidPressure4_2.Color = System.Windows.Media.Color.FromRgb(100, 100, 0);//直线颜色
            antiskidPressure4_2.ShowInLegend = true;//是否显示直线标题

            antiskidPressure5_2.Title = "5架2轴制动缸";//标题
            antiskidPressure5_2.Color = System.Windows.Media.Color.FromRgb(100, 0, 100);//直线颜色
            antiskidPressure5_2.ShowInLegend = true;//是否显示直线标题

            antiskidPressure6_2.Title = "6架2轴制动缸";//标题
            antiskidPressure6_2.Color = System.Windows.Media.Color.FromRgb(0, 100, 100);//直线颜色
            antiskidPressure6_2.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置电制动淡出曲线样式
            dcuEbFadeout1.Title = "电制动淡出_1";//标题
            dcuEbFadeout1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            dcuEbFadeout1.ShowInLegend = true;//是否显示直线标题

            dcuEbFadeout6.Title = "电制动淡出_6";//标题
            dcuEbFadeout6.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            dcuEbFadeout6.ShowInLegend = true;//是否显示直线标题
            #endregion

            #region 设置电制动实际值曲线的样式
            dcu1EbRealValue1.Title = "DCU1电制动实际值-1";//标题
            dcu1EbRealValue1.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            dcu1EbRealValue1.ShowInLegend = true;//是否显示直线标题

            dcu1EbRealValue6.Title = "DCU1电制动实际值-6";//标题
            dcu1EbRealValue6.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//直线颜色
            dcu1EbRealValue6.ShowInLegend = true;//是否显示直线标题

            dcu2EbRealValue1.Title = "DCU2电制动实际值-1";//标题
            dcu2EbRealValue1.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//直线颜色
            dcu2EbRealValue1.ShowInLegend = true;//是否显示直线标题

            dcu2EbRealValue6.Title = "DCU2电制动实际值-6";//标题
            dcu2EbRealValue6.Color = System.Windows.Media.Color.FromRgb(255, 255, 0);//直线颜色
            dcu2EbRealValue6.ShowInLegend = true;//是否显示直线标题

            dcu3EbRealValue1.Title = "DCU3电制动实际值-1";//标题
            dcu3EbRealValue1.Color = System.Windows.Media.Color.FromRgb(255, 0, 255);//直线颜色
            dcu3EbRealValue1.ShowInLegend = true;//是否显示直线标题

            dcu3EbRealValue6.Title = "DCU3电制动实际值-6";//标题
            dcu3EbRealValue6.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//直线颜色
            dcu3EbRealValue6.ShowInLegend = true;//是否显示直线标题

            dcu4EbRealValue1.Title = "DCU4电制动实际值-1";//标题
            dcu4EbRealValue1.Color = System.Windows.Media.Color.FromRgb(100, 0, 0);//直线颜色
            dcu4EbRealValue1.ShowInLegend = true;//是否显示直线标题

            dcu4EbRealValue6.Title = "DCU4电制动实际值-6";//标题
            dcu4EbRealValue6.Color = System.Windows.Media.Color.FromRgb(0, 100, 0);//直线颜色
            dcu4EbRealValue6.ShowInLegend = true;//是否显示直线标题
            #endregion

            accValueTChart.Series.Add(accValue1_1);
            accValueTChart.Series.Add(accValue1_2);
            accValueTChart.Series.Add(accValue2_1);
            accValueTChart.Series.Add(accValue2_2);
            accValueTChart.Series.Add(accValue3_1);
            accValueTChart.Series.Add(accValue3_2);
            accValueTChart.Series.Add(accValue4_1);
            accValueTChart.Series.Add(accValue4_2);
            accValueTChart.Series.Add(accValue5_1);
            accValueTChart.Series.Add(accValue5_2);
            accValueTChart.Series.Add(accValue6_1);
            accValueTChart.Series.Add(accValue6_2);

            refSpeedTChart.Series.Add(refSpeed1);
            refSpeedTChart.Series.Add(refSpeed6);

            emergencyHardTChart.Series.Add(emergencyHard1);
            emergencyHardTChart.Series.Add(emergencyHard6);

            brakeHardTChart.Series.Add(brakeHard1);
            brakeHardTChart.Series.Add(brakeHard6);
            brakeHardTChart.Series.Add(brakeInternet1);
            brakeHardTChart.Series.Add(brakeInternet6);

            fastHardTChart.Series.Add(fastBrakeHard1);
            fastHardTChart.Series.Add(fastBrakeHard6);
            fastHardTChart.Series.Add(fastBrakeInternet1);
            fastHardTChart.Series.Add(fastBrakeInternet6);

            enableTrainBrakeForceTChart.Series.Add(enableTrainBrakeForce1);
            enableTrainBrakeForceTChart.Series.Add(enableTrainBrakeForce6);

            trainBrakeForceTChart.Series.Add(trainBrakeForce1);
            trainBrakeForceTChart.Series.Add(trainBrakeForce6);

            brakeLevelTChart.Series.Add(brakeLevel1);
            brakeLevelTChart.Series.Add(brakeLevel6);

            pressureTChart.Series.Add(antiskidPressure1_1);
            pressureTChart.Series.Add(antiskidPressure2_1);
            pressureTChart.Series.Add(antiskidPressure3_1);
            pressureTChart.Series.Add(antiskidPressure4_1);
            pressureTChart.Series.Add(antiskidPressure5_1);
            pressureTChart.Series.Add(antiskidPressure6_1);
            pressureTChart.Series.Add(antiskidPressure1_2);
            pressureTChart.Series.Add(antiskidPressure2_2);
            pressureTChart.Series.Add(antiskidPressure3_2);
            pressureTChart.Series.Add(antiskidPressure4_2);
            pressureTChart.Series.Add(antiskidPressure5_2);
            pressureTChart.Series.Add(antiskidPressure6_2);

            dcuEbFadeoutTChart.Series.Add(dcuEbFadeout1);
            dcuEbFadeoutTChart.Series.Add(dcuEbFadeout6);

            dcu1EbRealValueTChart.Series.Add(dcu1EbRealValue1);
            dcu1EbRealValueTChart.Series.Add(dcu1EbRealValue6);
            dcu1EbRealValueTChart.Series.Add(dcu2EbRealValue1);
            dcu1EbRealValueTChart.Series.Add(dcu2EbRealValue6);
            dcu1EbRealValueTChart.Series.Add(dcu3EbRealValue1);
            dcu1EbRealValueTChart.Series.Add(dcu3EbRealValue6);
            dcu1EbRealValueTChart.Series.Add(dcu4EbRealValue1);
            dcu1EbRealValueTChart.Series.Add(dcu4EbRealValue6);
        }

        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.Container1_TChart = container_1;
            this.Container2_TChart = container_2;
            this.Container3_TChart = container_3;
            this.Container4_TChart = container_4;
            this.Container5_TChart = container_5;
            this.Container6_TChart = container_6;
        }

    }
}
