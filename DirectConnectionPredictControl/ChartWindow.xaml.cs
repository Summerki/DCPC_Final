using DirectConnectionPredictControl.CommenTool;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
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

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartWindow : Window
    {
        private const double X_LENGTH = 120;
        private const float LINE_WEIGHT = 1.0F;
        private const int SPEED_CHART_Y_MAX = 130;
        private const int PRESS_CHART_Y_MAX = 680;
        public event closeWindowHandler CloseWindowEvent;

        //速度组数据
        private ObservableDataSource<Point> speed1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed6 = new ObservableDataSource<Point>();

        private ObservableDataSource<Point> speed7 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed8 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed9 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed10 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed11 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed12 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> refSpeed = new ObservableDataSource<Point>();

        //压力组数据
        private ObservableDataSource<Point> cylinderAir1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> dcu1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> dcu2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> dcu3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> dcu4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> edfadeout = new ObservableDataSource<Point>();
        //private ObservableDataSource<Point> park6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder7 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder8 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder9 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder10 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder11 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder12 = new ObservableDataSource<Point>();

        //其他组
        private ObservableDataSource<Point> airPressure1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> airPressure2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> airPressure3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> airPressure4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> airPressure5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> airPressure6 = new ObservableDataSource<Point>();

        private ObservableDataSource<Point> loadPressure1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> loadPressure2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> loadPressure3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> loadPressure4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> loadPressure5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> loadPressure6 = new ObservableDataSource<Point>();

        //wsp组
        private ObservableDataSource<Point> accValue1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue7 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue8 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue9 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue10 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue11 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> accValue12 = new ObservableDataSource<Point>();

        private double x;
        private Queue<double> queue = new Queue<double>();
        private int xaxis = 0;
        public ChartWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            speedChart.AddLineGraph(speed1, Colors.DodgerBlue, LINE_WEIGHT, "架1轴1速度");
            speedChart.AddLineGraph(speed2, Colors.DarkOrange, LINE_WEIGHT, "架1轴2速度");
            speedChart.AddLineGraph(speed3, Colors.LimeGreen, LINE_WEIGHT, "架2轴1速度");
            speedChart.AddLineGraph(speed4, Colors.Violet, LINE_WEIGHT, "架2轴2速度");
            speedChart.AddLineGraph(speed5, Colors.Tomato, LINE_WEIGHT, "架3轴1速度");
            speedChart.AddLineGraph(speed6, Colors.Brown, LINE_WEIGHT, "架3轴2速度");
            speedChart.AddLineGraph(speed7, Colors.DodgerBlue, LINE_WEIGHT, "架4轴1速度");
            speedChart.AddLineGraph(speed8, Colors.DarkOrange, LINE_WEIGHT, "架4轴2速度");
            speedChart.AddLineGraph(speed9, Colors.LimeGreen, LINE_WEIGHT, "架5轴1速度");
            speedChart.AddLineGraph(speed10, Colors.Violet, LINE_WEIGHT, "架5轴2速度");
            speedChart.AddLineGraph(speed11, Colors.Tomato, LINE_WEIGHT, "架6轴1速度");
            speedChart.AddLineGraph(speed12, Colors.Brown, LINE_WEIGHT, "架6轴2速度");
            speedChart.AddLineGraph(refSpeed, Colors.DarkBlue, LINE_WEIGHT, "参考速度");

            pressureChart.AddLineGraph(cylinder1, Colors.DodgerBlue, LINE_WEIGHT, "1架轴1制动缸");
            pressureChart.AddLineGraph(cylinder2, Colors.DarkOrange, LINE_WEIGHT, "1架轴2制动缸");
            pressureChart.AddLineGraph(cylinder3, Colors.LimeGreen, LINE_WEIGHT, "2架轴1制动缸");
            pressureChart.AddLineGraph(cylinder4, Colors.Violet, LINE_WEIGHT, "2架轴2制动缸");
            pressureChart.AddLineGraph(cylinder5, Colors.Tomato, LINE_WEIGHT, "3架轴1制动缸");
            pressureChart.AddLineGraph(cylinder6, Colors.Brown, LINE_WEIGHT, "3架轴2制动缸");
            pressureChart.AddLineGraph(cylinder7, Colors.DodgerBlue, LINE_WEIGHT, "4架轴1制动缸");
            pressureChart.AddLineGraph(cylinder8, Colors.DarkOrange, LINE_WEIGHT, "4架轴2制动缸");
            pressureChart.AddLineGraph(cylinder9, Colors.LimeGreen, LINE_WEIGHT, "5架轴1制动缸");
            pressureChart.AddLineGraph(cylinder10, Colors.Violet, LINE_WEIGHT, "5架轴2制动缸");
            pressureChart.AddLineGraph(cylinder11, Colors.Tomato, LINE_WEIGHT, "6架轴1制动缸");
            pressureChart.AddLineGraph(cylinder12, Colors.Brown, LINE_WEIGHT, "6架轴2制动缸");

            pressureChart.AddLineGraph(dcu1, Colors.DarkBlue, LINE_WEIGHT, "DCU1电制动力");
            pressureChart.AddLineGraph(dcu2, Colors.Moccasin, LINE_WEIGHT, "DCU2电制动力");
            pressureChart.AddLineGraph(dcu3, Colors.RoyalBlue, LINE_WEIGHT, "DCU3电制动力");
            pressureChart.AddLineGraph(dcu4, Colors.Tan, LINE_WEIGHT, "DCU4电制动力");
            pressureChart.AddLineGraph(refSpeed, Colors.Aqua, LINE_WEIGHT, "参考速度");
            pressureChart.AddLineGraph(edfadeout, Colors.BurlyWood, LINE_WEIGHT, "电制动力淡出");

            //otherChart.AddLineGraph(airPressure1, Colors.DodgerBlue, LINE_WEIGHT, "1架空簧");
            //otherChart.AddLineGraph(airPressure2, Colors.DarkOrange, LINE_WEIGHT, "2架空簧");
            //otherChart.AddLineGraph(airPressure3, Colors.LimeGreen, LINE_WEIGHT, "3架空簧");
            //otherChart.AddLineGraph(airPressure4, Colors.Violet, LINE_WEIGHT, "4架空簧");
            //otherChart.AddLineGraph(airPressure5, Colors.Tomato, LINE_WEIGHT, "5架空簧");
            //otherChart.AddLineGraph(airPressure6, Colors.Brown, LINE_WEIGHT, "6架空簧");

            //otherChart.AddLineGraph(loadPressure1, Colors.DarkBlue, LINE_WEIGHT, "1架载重");
            //otherChart.AddLineGraph(loadPressure1, Colors.Moccasin, LINE_WEIGHT, "2架载重");
            //otherChart.AddLineGraph(loadPressure1, Colors.RoyalBlue, LINE_WEIGHT, "3架载重");
            //otherChart.AddLineGraph(loadPressure1, Colors.Tan, LINE_WEIGHT, "4架载重");
            //otherChart.AddLineGraph(loadPressure1, Colors.Aqua, LINE_WEIGHT, "5架载重");
            //otherChart.AddLineGraph(loadPressure1, Colors.BurlyWood, LINE_WEIGHT, "6架载重");

            wspChart.AddLineGraph(accValue1, Colors.DodgerBlue, LINE_WEIGHT, "架1轴1减速度");
            wspChart.AddLineGraph(accValue2, Colors.DarkOrange, LINE_WEIGHT, "架1轴2减速度");
            wspChart.AddLineGraph(accValue3, Colors.RoyalBlue, LINE_WEIGHT, "架2轴1减速度");
            wspChart.AddLineGraph(accValue4, Colors.Tan, LINE_WEIGHT, "架2轴2减速度");
            wspChart.AddLineGraph(accValue5, Colors.Aqua, LINE_WEIGHT, "架3轴1减速度");
            wspChart.AddLineGraph(accValue6, Colors.BurlyWood, LINE_WEIGHT, "架3轴2减速度");

            wspChart.AddLineGraph(accValue7, Colors.DodgerBlue, LINE_WEIGHT, "架4轴1滑行率");
            wspChart.AddLineGraph(accValue8, Colors.DarkOrange, LINE_WEIGHT, "架4轴2滑行率");
            wspChart.AddLineGraph(accValue9, Colors.RoyalBlue, LINE_WEIGHT, "架5轴1滑行率");
            wspChart.AddLineGraph(accValue10, Colors.Tan, LINE_WEIGHT, "架5轴2滑行率");
            wspChart.AddLineGraph(accValue11, Colors.Aqua, LINE_WEIGHT, "架6轴1滑行率");
            wspChart.AddLineGraph(accValue12, Colors.BurlyWood, LINE_WEIGHT, "架6轴2滑行率");
            speedChart.Legend.LegendLeft = 0;
            pressureChart.Legend.LegendLeft = 0;
            wspChart.Legend.LegendLeft = 0;
            x = 0;
        }

        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            ClearDataSource();
            AppendSpeedSource(container_1, container_2, container_3, container_4, container_5, container_6);
            AppendPressSource(container_1, container_2, container_3, container_4, container_5, container_6);
            //AppendOtherSource(container_1, container_2, container_3, container_4, container_5, container_6);
            AppendWSPSource(container_1, container_2, container_3, container_4, container_5, container_6);
            this.Dispatcher.Invoke(() =>
            {
                UpdateSpeedData(container_1, container_2, container_3, container_4, container_5, container_6);

            });
            this.Dispatcher.Invoke(() =>
            {
                UpdatePressData(container_1, container_2, container_3, container_4, container_5, container_6);
            });

            //this.Dispatcher.Invoke(() =>
            //{
            //    UpdateOtherData(container_1, container_2, container_3, container_4, container_5, container_6);
            //});
            this.Dispatcher.Invoke(() =>
            {
                UpdateWSPData(container_1, container_2, container_3, container_4, container_5, container_6);
            });


            if (queue.Count < X_LENGTH)
            {
                queue.Enqueue(x);
            }
            else
            {
                queue.Dequeue();
                queue.Enqueue(x);
            }
            if (x - X_LENGTH > 0)
            {
                xaxis = (int)(x - X_LENGTH);
            }
            else
            {
                xaxis = 0;
            }
            this.Dispatcher.Invoke(() =>
            {
                speedChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, SPEED_CHART_Y_MAX);
                pressureChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, PRESS_CHART_Y_MAX);
                //otherChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, PRESS_CHART_Y_MAX);
                wspChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, 60);
            });

            x = x + 0.05;
        }

        private void UpdateWSPData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_1_wsp.Content = container_1.AccValue1;
            row_1_column_1_wsp.Content = container_1.SlipRate1;
            row_2_column_1_wsp.Content = container_1.SlipLvl1;
            row_3_column_1_wsp.Content = container_1.AccValue2;
            row_4_column_1_wsp.Content = container_1.SlipRate2;
            row_5_column_1_wsp.Content = container_1.SlipLvl2;
            row_6_column_1_wsp.Content = container_2.AccValue1;
            row_7_column_1_wsp.Content = container_2.SlipRate1;
            row_8_column_1_wsp.Content = container_2.SlipLvl1;
            row_9_column_1_wsp.Content = container_4.AccValue1;
            row_10_column_1_wsp.Content = container_4.SlipRate1;
            row_11_column_1_wsp.Content = container_4.SlipLvl1;
            row_12_column_1_wsp.Content = container_6.AccValue1;
            row_13_column_1_wsp.Content = container_6.SlipRate1;
            row_14_column_1_wsp.Content = container_6.SlipLvl1;
            row_15_column_1_wsp.Content = container_6.AccValue2;
            row_16_column_1_wsp.Content = container_6.SlipRate2;
            row_17_column_1_wsp.Content = container_6.SlipLvl2;

            row_0_column_3_wsp.Content = container_2.AccValue2;
            row_1_column_3_wsp.Content = container_2.SlipRate2;
            row_2_column_3_wsp.Content = container_2.SlipLvl2;
            row_3_column_3_wsp.Content = container_3.AccValue1;
            row_4_column_3_wsp.Content = container_3.SlipRate1;
            row_5_column_3_wsp.Content = container_3.SlipLvl1;
            row_6_column_3_wsp.Content = container_3.AccValue2;
            row_7_column_3_wsp.Content = container_3.SlipRate2;
            row_8_column_3_wsp.Content = container_3.SlipLvl2;
            row_9_column_3_wsp.Content = container_4.AccValue2;
            row_10_column_3_wsp.Content = container_4.SlipRate2;
            row_11_column_3_wsp.Content = container_4.SlipLvl2;
            row_12_column_3_wsp.Content = container_5.AccValue1;
            row_13_column_3_wsp.Content = container_5.SlipRate1;
            row_14_column_3_wsp.Content = container_5.SlipLvl1;
            row_15_column_3_wsp.Content = container_5.AccValue2;
            row_16_column_3_wsp.Content = container_5.SlipRate2;
            row_17_column_3_wsp.Content = container_5.SlipLvl2;

        }

        private void AppendWSPSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            accValue1.AppendAsync(base.Dispatcher, new Point(x, container_1.AccValue1));
            accValue2.AppendAsync(base.Dispatcher, new Point(x, container_1.AccValue2));
            accValue3.AppendAsync(base.Dispatcher, new Point(x, container_2.AccValue1));
            accValue4.AppendAsync(base.Dispatcher, new Point(x, container_2.AccValue2));
            accValue5.AppendAsync(base.Dispatcher, new Point(x, container_3.AccValue1));
            accValue6.AppendAsync(base.Dispatcher, new Point(x, container_3.AccValue2));

            accValue7.AppendAsync(base.Dispatcher, new Point(x, container_4.AccValue1));
            accValue8.AppendAsync(base.Dispatcher, new Point(x, container_4.AccValue2));
            accValue9.AppendAsync(base.Dispatcher, new Point(x, container_5.AccValue1));
            accValue10.AppendAsync(base.Dispatcher, new Point(x, container_5.AccValue2));
            accValue11.AppendAsync(base.Dispatcher, new Point(x, container_6.AccValue1));
            accValue12.AppendAsync(base.Dispatcher, new Point(x, container_6.AccValue2));
        }

        //private void AppendOtherSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        //{
        //    airPressure1.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));
        //    airPressure2.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));
        //    airPressure3.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));
        //    airPressure4.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));
        //    airPressure5.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));
        //    airPressure6.AppendAsync(base.Dispatcher, new Point(x, (container_1.AirSpring1PressureA1Car1 + container_1.AirSpring2PressureA1Car1) / 2));

        //    loadPressure1.AppendAsync(base.Dispatcher, new Point(x, container_1.MassA1));
        //    loadPressure2.AppendAsync(base.Dispatcher, new Point(x, container_2.MassValue));
        //    loadPressure3.AppendAsync(base.Dispatcher, new Point(x, container_3.MassValue));
        //    loadPressure4.AppendAsync(base.Dispatcher, new Point(x, container_4.MassValue));
        //    loadPressure5.AppendAsync(base.Dispatcher, new Point(x, container_5.MassValue));
        //    loadPressure6.AppendAsync(base.Dispatcher, new Point(x, container_6.MassA1));
        //}

        private void AppendPressSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            cylinder1.AppendAsync(base.Dispatcher, new Point(x, (container_1.Bcp1PressureAx1)));
            cylinder2.AppendAsync(base.Dispatcher, new Point(x, (container_1.Bcp2PressureAx2)));
            cylinder3.AppendAsync(base.Dispatcher, new Point(x, (container_2.Bcp1Pressure)));
            cylinder4.AppendAsync(base.Dispatcher, new Point(x, (container_2.Bcp2Pressure)));
            cylinder5.AppendAsync(base.Dispatcher, new Point(x, (container_3.Bcp1Pressure)));
            cylinder6.AppendAsync(base.Dispatcher, new Point(x, (container_3.Bcp2Pressure)));
            cylinder7.AppendAsync(base.Dispatcher, new Point(x, (container_4.Bcp1Pressure)));
            cylinder8.AppendAsync(base.Dispatcher, new Point(x, (container_4.Bcp2Pressure)));
            cylinder9.AppendAsync(base.Dispatcher, new Point(x, (container_5.Bcp1Pressure)));
            cylinder10.AppendAsync(base.Dispatcher, new Point(x, (container_5.Bcp2Pressure)));
            cylinder11.AppendAsync(base.Dispatcher, new Point(x, (container_6.Bcp1PressureAx1)));
            cylinder12.AppendAsync(base.Dispatcher, new Point(x, (container_6.Bcp2PressureAx2)));

            dcu1.AppendAsync(base.Dispatcher, new Point(x, container_1.DcuEbRealValue[0]));
            dcu2.AppendAsync(base.Dispatcher, new Point(x, container_1.DcuEbRealValue[1]));
            dcu3.AppendAsync(base.Dispatcher, new Point(x, container_1.DcuEbRealValue[2]));
            dcu4.AppendAsync(base.Dispatcher, new Point(x, container_1.DcuEbRealValue[3]));
            //ref.AppendAsync(base.Dispatcher, new Point(x, container_5.ParkPressure));
            edfadeout.AppendAsync(base.Dispatcher, new Point(x, (container_1.EdFadeOut == true ? 1 : 0) * 200));
        }

        private void AppendSpeedSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            speed1.AppendAsync(base.Dispatcher, new Point(x, (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2));
            speed2.AppendAsync(base.Dispatcher, new Point(x, (container_2.SpeedShaft1 + container_2.SpeedShaft2) / 2));
            speed3.AppendAsync(base.Dispatcher, new Point(x, (container_3.SpeedShaft1 + container_3.SpeedShaft2) / 2));
            speed4.AppendAsync(base.Dispatcher, new Point(x, (container_4.SpeedShaft1 + container_4.SpeedShaft2) / 2));
            speed5.AppendAsync(base.Dispatcher, new Point(x, (container_5.SpeedShaft1 + container_5.SpeedShaft2) / 2));
            speed6.AppendAsync(base.Dispatcher, new Point(x, (container_6.SpeedA1Shaft1 + container_6.SpeedA1Shaft2) / 2));
            refSpeed.AppendAsync(base.Dispatcher, new Point(x, container_1.RefSpeed));
        }

        //private void UpdateOtherData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        //{
        //    row_0_column_0_Ot.Content = container_1.AirSpring1PressureA1Car1;
        //    row_1_column_0_Ot.Content = container_1.AirSpring2PressureA1Car1;
        //    row_2_column_0_Ot.Content = container_2.AirSpringPressure1;
        //    row_3_column_0_Ot.Content = container_2.AirSpringPressure2;
        //    row_4_column_0_Ot.Content = container_3.AirSpringPressure1;
        //    row_5_column_0_Ot.Content = container_3.AirSpringPressure2;
        //    row_6_column_0_Ot.Content = container_4.AirSpringPressure1;
        //    row_7_column_0_Ot.Content = container_4.AirSpringPressure2;
        //    row_8_column_0_Ot.Content = container_5.AirSpringPressure1;

        //    row_0_column_3_Ot.Content = container_5.AirSpringPressure2;
        //    row_1_column_3_Ot.Content = container_6.AirSpring1PressureA1Car1;
        //    row_2_column_3_Ot.Content = container_6.AirSpring2PressureA1Car1;
        //    row_3_column_3_Ot.Content = container_1.MassA1;
        //    row_4_column_3_Ot.Content = container_2.MassValue;
        //    row_5_column_3_Ot.Content = container_3.MassValue;
        //    row_6_column_3_Ot.Content = container_4.MassValue;
        //    row_7_column_3_Ot.Content = container_5.MassValue;
        //    row_8_column_3_Ot.Content = container_6.MassA1;
        //}

        private void UpdatePressData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_0_Pr.Content = container_1.Bcp1PressureAx1;
            row_1_column_0_Pr.Content = container_1.Bcp2PressureAx2;
            row_2_column_0_Pr.Content = container_2.Bcp1Pressure;
            row_3_column_0_Pr.Content = container_2.Bcp2Pressure;
            row_4_column_0_Pr.Content = container_3.Bcp1Pressure;
            row_5_column_0_Pr.Content = container_3.Bcp2Pressure;
            row_6_column_0_Pr.Content = container_4.Bcp1Pressure;
            row_7_column_0_Pr.Content = container_4.Bcp2Pressure;
            row_8_column_0_Pr.Content = container_1.RefSpeed;
            row_9_column_0_Pr.Content = container_1.EdFadeOut;

            row_0_column_3_Pr.Content = container_5.Bcp1Pressure;
            row_1_column_3_Pr.Content = container_5.Bcp2Pressure;
            row_2_column_3_Pr.Content = container_6.Bcp1PressureAx1;
            row_3_column_3_Pr.Content = container_6.Bcp2PressureAx2;
            row_4_column_3_Pr.Content = container_1.DcuEbRealValue[0];
            row_5_column_3_Pr.Content = container_1.DcuEbRealValue[1];
            row_6_column_3_Pr.Content = container_1.DcuEbRealValue[2];
            row_7_column_3_Pr.Content = container_1.DcuEbRealValue[3];
        }

        private void UpdateSpeedData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_0_Sp.Content = container_1.SpeedA1Shaft1 + " / " + container_1.SpeedA1Shaft2;
            row_1_column_0_Sp.Content = container_2.SpeedShaft1 + " / " + container_2.SpeedShaft2;
            row_2_column_0_Sp.Content = container_3.SpeedShaft1 + " / "+ container_3.SpeedShaft2;
            row_3_column_0_Sp.Content = container_4.SpeedShaft1 + " / " + container_4.SpeedShaft2;
            row_0_column_3_Sp.Content = container_5.SpeedShaft1 + " / "+ container_5.SpeedShaft2;
            row_1_column_3_Sp.Content = container_6.SpeedA1Shaft1 + " / " + container_6.SpeedA1Shaft2;
            row_2_column_3_Sp.Content = container_1.RefSpeed;
        }

        private void ClearDataSource()
        {
            ClearSpeedSource();
            
        }

        private void ClearSpeedSource()
        {
            if (speed1.Collection.Count > 60 * 1000)
            {
                speed1.Collection.Clear();
            }
            if (speed2.Collection.Count > 60 * 1000)
            {
                speed2.Collection.Clear();
            }
            if (speed3.Collection.Count > 60 * 1000)
            {
                speed3.Collection.Clear();
            }
            if (speed4.Collection.Count > 60 * 1000)
            {
                speed4.Collection.Clear();
            }
            if (speed5.Collection.Count > 60 * 1000)
            {
                speed5.Collection.Clear();
            }
            if (speed6.Collection.Count > 60 * 1000)
            {
                speed6.Collection.Clear();
            }
        }

        #region window methods
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
        #endregion

        private void chartWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "chart");
        }

        private void speedChart_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
    }
}
