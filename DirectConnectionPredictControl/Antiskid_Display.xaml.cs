using System;
using System.Collections.Generic;
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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// Antiskid_Display.xaml 的交互逻辑
    /// </summary>
    public partial class Antiskid_Display : Window // 防滑分析显示界面
    {
        private const double X_LENGTH = 120;
        private const float LINE_WEIGHT = 1.0F;
        private const int SPEED_CHART_Y_MAX = 130;
        private const int PRESS_CHART_Y_MAX = 680;
        private const int Jian_Speed_Y_MAX = 20;
        private const int SlipRate_Y_MAX = 100;  // 2019-13-25：滑移率图表上Y轴最大坐标值100（原本是0-1，现在扩大到100倍）
        public event closeWindowHandler CloseWindowEvent;

        //速度组数据
        private ObservableDataSource<Point> Antiskid_speed1_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed1_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed2_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed2_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed3_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed3_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed4_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed4_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed5_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed5_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed6_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speed6_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_refSpeed = new ObservableDataSource<Point>();
        // 下面是速度差值的定义
        private ObservableDataSource<Point> Antiskid_speedDiff1_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff1_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff2_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff2_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff3_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff3_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff4_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff4_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff5_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff5_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff6_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_speedDiff6_2 = new ObservableDataSource<Point>();

        //减速度，滑行率，滑行等级定义
        private ObservableDataSource<Point> Antiskid_accValue1_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue2_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue3_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue4_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue5_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue6_1 = new ObservableDataSource<Point>();

        private ObservableDataSource<Point> Antiskid_accValue1_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue2_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue3_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue4_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue5_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_accValue6_2 = new ObservableDataSource<Point>();

        //2019-12-25：新加滑移率
        private ObservableDataSource<Point> Antiskid_slipRate1_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate1_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate2_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate2_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate3_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate3_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate4_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate4_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate5_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate5_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate6_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_slipRate6_2 = new ObservableDataSource<Point>();

        // 制动缸组
        private ObservableDataSource<Point> Antiskid_cylinder1_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder2_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder3_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder4_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder5_1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder6_1 = new ObservableDataSource<Point>();

        private ObservableDataSource<Point> Antiskid_cylinder1_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder2_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder3_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder4_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder5_2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> Antiskid_cylinder6_2 = new ObservableDataSource<Point>();

        private double x;
        private Queue<double> queue = new Queue<double>();
        private int xaxis = 0;

        #region overview
        /// <summary>
        /// 数据组
        /// </summary>
        private MainDevDataContains Antiskid_container_1 = new MainDevDataContains();
        private MainDevDataContains Antiskid_container_6 = new MainDevDataContains();
        private SliverDataContainer Antiskid_container_2 = new SliverDataContainer();
        private SliverDataContainer Antiskid_container_3 = new SliverDataContainer();
        private SliverDataContainer Antiskid_container_4 = new SliverDataContainer();
        private SliverDataContainer Antiskid_container_5 = new SliverDataContainer();

        /// <summary>
        /// 线程组
        /// </summary>
        private Thread uiThread;


        private void Overview_Init()
        {
            uiThread = new Thread(UpdateUI);
            uiThread.Start();
        }

        private void UpdateUI()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    //currentTimeLbl.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    UpdateData();
                });
                Thread.Sleep(Utils.timeInterval);
            }
        }

        /// <summary>
        /// 更新最后的数字量的函数
        /// </summary>
        private void UpdateData()
        {
            row_1_column_1_DO.Fill = GetBrush(Antiskid_container_1.WSPFault_1);
            row_1_column_2_DO.Fill = GetBrush(Antiskid_container_2.WSPFault_1);
            row_1_column_3_DO.Fill = GetBrush(Antiskid_container_3.WSPFault_1);
            row_1_column_4_DO.Fill = GetBrush(Antiskid_container_4.WSPFault_1);
            row_1_column_5_DO.Fill = GetBrush(Antiskid_container_5.WSPFault_1);
            row_1_column_6_DO.Fill = GetBrush(Antiskid_container_6.WSPFault_1);

            row_2_column_1_DO.Fill = GetBrush(Antiskid_container_1.WSPFault_2);
            row_2_column_2_DO.Fill = GetBrush(Antiskid_container_2.WSPFault_2);
            row_2_column_3_DO.Fill = GetBrush(Antiskid_container_3.WSPFault_2);
            row_2_column_4_DO.Fill = GetBrush(Antiskid_container_4.WSPFault_2);
            row_2_column_5_DO.Fill = GetBrush(Antiskid_container_5.WSPFault_2);
            row_2_column_6_DO.Fill = GetBrush(Antiskid_container_6.WSPFault_2);

            row_3_column_1_DO.Fill = GetBrush(Antiskid_container_1.SpeedSenorFault_1);
            row_3_column_2_DO.Fill = GetBrush(Antiskid_container_2.SpeedSenorFault_1);
            row_3_column_3_DO.Fill = GetBrush(Antiskid_container_3.SpeedSenorFault_1);
            row_3_column_4_DO.Fill = GetBrush(Antiskid_container_4.SpeedSenorFault_1);
            row_3_column_5_DO.Fill = GetBrush(Antiskid_container_5.SpeedSenorFault_1);
            row_3_column_6_DO.Fill = GetBrush(Antiskid_container_6.SpeedSenorFault_1);

            row_4_column_1_DO.Fill = GetBrush(Antiskid_container_1.SpeedSenorFault_2);
            row_4_column_2_DO.Fill = GetBrush(Antiskid_container_2.SpeedSenorFault_2);
            row_4_column_3_DO.Fill = GetBrush(Antiskid_container_3.SpeedSenorFault_2);
            row_4_column_4_DO.Fill = GetBrush(Antiskid_container_4.SpeedSenorFault_2);
            row_4_column_5_DO.Fill = GetBrush(Antiskid_container_5.SpeedSenorFault_2);
            row_4_column_6_DO.Fill = GetBrush(Antiskid_container_6.SpeedSenorFault_2);

            row_1_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbOK[0]);
            row_2_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbFadeout[0]);
            row_3_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbSlip[0]);

            row_4_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbOK[1]);
            row_5_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbFadeout[1]);
            row_6_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbSlip[1]);

            row_7_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbOK[2]);
            row_8_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbFadeout[2]);
            row_9_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbSlip[2]);

            row_10_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbOK[3]);
            row_11_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbFadeout[3]);
            row_12_column_8_DO.Fill = GetBrush(Antiskid_container_1.DcuEbSlip[3]);
        }

        private SolidColorBrush GetBrush(bool b)
        {
            return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
        }

        #endregion
        public Antiskid_Display()
        {
            InitializeComponent();
            Init();
            Overview_Init();
        }


        private void Init()
        {
            // 速度组，由于速度差是通过计算得来的，所以不在曲线图中显示
            Antiskid_speedChart.AddLineGraph(Antiskid_refSpeed, Colors.DarkBlue, LINE_WEIGHT, "参考速度");

            Antiskid_speedChart.AddLineGraph(Antiskid_speed1_1, Colors.DodgerBlue, LINE_WEIGHT, "1架1轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed2_1, Colors.DarkOrange, LINE_WEIGHT, "2架1轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed3_1, Colors.LimeGreen, LINE_WEIGHT, "3架1轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed4_1, Colors.Violet, LINE_WEIGHT, "4架1轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed5_1, Colors.Tomato, LINE_WEIGHT, "5架1轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed6_1, Colors.Brown, LINE_WEIGHT, "6架1轴速度");

            Antiskid_speedChart.AddLineGraph(Antiskid_speed1_2, Colors.DodgerBlue, LINE_WEIGHT, "1架2轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed2_2, Colors.DarkOrange, LINE_WEIGHT, "2架2轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed3_2, Colors.LimeGreen, LINE_WEIGHT, "3架2轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed4_2, Colors.Violet, LINE_WEIGHT, "4架2轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed5_2, Colors.Tomato, LINE_WEIGHT, "5架2轴速度");
            Antiskid_speedChart.AddLineGraph(Antiskid_speed6_2, Colors.Brown, LINE_WEIGHT, "6架2轴速度");
            //Antiskid_speedChart.AddLineGraph(Antiskid_speedDiff1, Colors.AliceBlue, LINE_WEIGHT, "1架速度差");


            // 减速度，滑行率曲线图，同样滑行等级不在曲线图中展示
            // 减速度
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue1_1, Colors.DodgerBlue, LINE_WEIGHT, "1架1轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue2_1, Colors.DarkOrange, LINE_WEIGHT, "2架1轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue3_1, Colors.LimeGreen, LINE_WEIGHT, "3架1轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue4_1, Colors.Violet, LINE_WEIGHT, "4架1轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue5_1, Colors.Tomato, LINE_WEIGHT, "5架1轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue6_1, Colors.DarkBlue, LINE_WEIGHT, "6架1轴减速度");

            Antiskid_wspChart.AddLineGraph(Antiskid_accValue1_2, Colors.DodgerBlue, LINE_WEIGHT, "1架2轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue2_2, Colors.DarkOrange, LINE_WEIGHT, "2架2轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue3_2, Colors.LimeGreen, LINE_WEIGHT, "3架2轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue4_2, Colors.Violet, LINE_WEIGHT, "4架2轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue5_2, Colors.Tomato, LINE_WEIGHT, "5架2轴减速度");
            Antiskid_wspChart.AddLineGraph(Antiskid_accValue6_2, Colors.DarkBlue, LINE_WEIGHT, "6架2轴减速度");

            // 滑移率：2018-10-11更新：在曲线中不再需要滑移率，先注释掉
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate1, Colors.DodgerBlue, LINE_WEIGHT, "1架滑移率");
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate2, Colors.DarkOrange, LINE_WEIGHT, "2架滑移率");
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate3, Colors.LimeGreen, LINE_WEIGHT, "3架滑移率");
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate4, Colors.Violet, LINE_WEIGHT, "4架滑移率");
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate5, Colors.Tomato, LINE_WEIGHT, "5架滑移率");
            //Antiskid_wspChart.AddLineGraph(Antiskid_slipRate6, Colors.DarkBlue, LINE_WEIGHT, "6架滑移率");

            // 2019-12-25:新增滑移率图表，这里增加滑移率图例
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate1_1, Colors.DodgerBlue, LINE_WEIGHT, "1架1轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate2_1, Colors.DarkOrange, LINE_WEIGHT, "2架1轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate3_1, Colors.LimeGreen, LINE_WEIGHT, "3架1轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate4_1, Colors.Violet, LINE_WEIGHT, "4架1轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate5_1, Colors.Tomato, LINE_WEIGHT, "5架1轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate6_1, Colors.DarkBlue, LINE_WEIGHT, "6架1轴滑移率");

            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate1_2, Colors.DodgerBlue, LINE_WEIGHT, "1架2轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate2_2, Colors.DarkOrange, LINE_WEIGHT, "2架2轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate3_2, Colors.LimeGreen, LINE_WEIGHT, "3架2轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate4_2, Colors.Violet, LINE_WEIGHT, "4架2轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate5_2, Colors.Tomato, LINE_WEIGHT, "5架2轴滑移率");
            Antiskid_SlipRateChart.AddLineGraph(Antiskid_slipRate6_2, Colors.DarkBlue, LINE_WEIGHT, "6架2轴滑移率");

            // 制动缸曲线图
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder1_1, Colors.DodgerBlue, LINE_WEIGHT, "1架1轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder2_1, Colors.DarkOrange, LINE_WEIGHT, "2架1轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder3_1, Colors.LimeGreen, LINE_WEIGHT, "3架1轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder4_1, Colors.Violet, LINE_WEIGHT, "4架1轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder5_1, Colors.Tomato, LINE_WEIGHT, "5架1轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder6_1, Colors.Brown, LINE_WEIGHT, "6架1轴制动缸");

            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder1_2, Colors.DodgerBlue, LINE_WEIGHT, "1架2轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder2_2, Colors.DarkOrange, LINE_WEIGHT, "2架2轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder3_2, Colors.LimeGreen, LINE_WEIGHT, "3架2轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder4_2, Colors.Violet, LINE_WEIGHT, "4架2轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder5_2, Colors.Tomato, LINE_WEIGHT, "5架2轴制动缸");
            Antiskid_pressureChart.AddLineGraph(Antiskid_cylinder6_2, Colors.Brown, LINE_WEIGHT, "6架2轴制动缸");

            x = 0;
        }


        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.Antiskid_container_1 = container_1;
            this.Antiskid_container_2 = container_2;
            this.Antiskid_container_3 = container_3;
            this.Antiskid_container_4 = container_4;
            this.Antiskid_container_5 = container_5;
            this.Antiskid_container_6 = container_6;


            ClearDataSource();
            Antiskid_AppendSpeedSource(container_1, container_2, container_3, container_4, container_5, container_6);
            Antiskid_AppendSlipRateSource(container_1, container_2, container_3, container_4, container_5, container_6);  // 2019-12-25
            Antiskid_AppendPressSource(container_1, container_2, container_3, container_4, container_5, container_6);
            Antiskid_AppendWSPSource(container_1, container_2, container_3, container_4, container_5, container_6);
            
            this.Dispatcher.Invoke(() =>
            {
                Antiskid_UpdateSpeedData(container_1, container_2, container_3, container_4, container_5, container_6);

            });
            this.Dispatcher.Invoke(() =>
            {
                Antiskid_UpdatePressData(container_1, container_2, container_3, container_4, container_5, container_6);
            });
            this.Dispatcher.Invoke(() =>  // 2019-12-25
            {
                Antiskid_UpdateSlipRateData(container_1, container_2, container_3, container_4, container_5, container_6);
            });
            this.Dispatcher.Invoke(() =>
            {
                Antiskid_UpdateWSPData(container_1, container_2, container_3, container_4, container_5, container_6);
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
                Antiskid_speedChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, SPEED_CHART_Y_MAX);
                Antiskid_pressureChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, PRESS_CHART_Y_MAX);
                Antiskid_wspChart.Viewport.Visible = new Rect(xaxis, -10, X_LENGTH, Jian_Speed_Y_MAX);
                Antiskid_SlipRateChart.Viewport.Visible = new Rect(xaxis, 0, X_LENGTH, SlipRate_Y_MAX); // 2019-12-25
            });

            x = x + 1;
        }

        private void ClearDataSource()
        {
            ClearSpeedSource();
            ClearSlipRateSource();
        }

        /// <summary>
        /// 2019-12-25：达到多少数目后清除滑移率点的集合
        /// </summary>
        private void ClearSlipRateSource()
        {
            if(Antiskid_slipRate1_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate1_1.Collection.Clear();
            }
            if (Antiskid_slipRate1_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate1_2.Collection.Clear();
            }
            if (Antiskid_slipRate2_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate2_1.Collection.Clear();
            }
            if (Antiskid_slipRate2_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate2_2.Collection.Clear();
            }
            if (Antiskid_slipRate3_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate3_1.Collection.Clear();
            }
            if (Antiskid_slipRate3_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate3_2.Collection.Clear();
            }
            if (Antiskid_slipRate4_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate4_1.Collection.Clear();
            }
            if (Antiskid_slipRate4_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate4_2.Collection.Clear();
            }
            if (Antiskid_slipRate5_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate5_1.Collection.Clear();
            }
            if (Antiskid_slipRate5_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate5_2.Collection.Clear();
            }
            if (Antiskid_slipRate6_1.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate6_1.Collection.Clear();
            }
            if (Antiskid_slipRate6_2.Collection.Count > 60 * 1000)
            {
                Antiskid_slipRate6_2.Collection.Clear();
            }
        }

        private void ClearSpeedSource()
        {
            if (Antiskid_speed1_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed1_1.Collection.Clear();
            }
            if (Antiskid_speed1_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed1_2.Collection.Clear();
            }
            if (Antiskid_speed2_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed2_1.Collection.Clear();
            }
            if (Antiskid_speed2_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed2_2.Collection.Clear();
            }
            if (Antiskid_speed3_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed3_1.Collection.Clear();
            }
            if (Antiskid_speed3_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed3_2.Collection.Clear();
            }
            if (Antiskid_speed4_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed4_1.Collection.Clear();
            }
            if (Antiskid_speed4_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed4_2.Collection.Clear();
            }
            if (Antiskid_speed5_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed5_1.Collection.Clear();
            }
            if (Antiskid_speed5_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed5_2.Collection.Clear();
            }
            if (Antiskid_speed6_1.Collection.Count > 60 * 1000)
            {
                Antiskid_speed6_1.Collection.Clear();
            }
            if (Antiskid_speed6_2.Collection.Count > 60 * 1000)
            {
                Antiskid_speed6_2.Collection.Clear();
            }
        }

        #region Update Data

        /// <summary>
        /// 2019-12-25：更新滑移率数值数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_UpdateSlipRateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_1_slipRate.Content = container_1.SlipRate1;
            row_0_column_3_slipRate.Content = container_1.SlipRate2;
            row_1_column_1_slipRate.Content = container_2.SlipRate1;
            row_1_column_3_slipRate.Content = container_2.SlipRate2;
            row_2_column_1_slipRate.Content = container_3.SlipRate1;
            row_2_column_3_slipRate.Content = container_3.SlipRate2;
            row_3_column_1_slipRate.Content = container_4.SlipRate1;
            row_3_column_3_slipRate.Content = container_4.SlipRate2;
            row_4_column_1_slipRate.Content = container_5.SlipRate1;
            row_4_column_3_slipRate.Content = container_5.SlipRate2;
            row_5_column_1_slipRate.Content = container_6.SlipRate1;
            row_5_column_3_slipRate.Content = container_6.SlipRate2;
        }


        /// <summary>
        /// 更新减速度、滑移率和滑行等级曲线数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_UpdateWSPData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            //2018-10-8：重新排序减速度、滑移率等界面顺序，增加减速度为12个

            //减速度组
            row_0_column_1_wsp.Content = (container_1.AccValue1) + "/" + (container_1.AccValue2);
            row_0_column_3_wsp.Content = (container_2.AccValue1) + "/" + (container_2.AccValue2);
            row_1_column_1_wsp.Content = (container_3.AccValue1) + "/" + (container_3.AccValue2);
            row_1_column_3_wsp.Content = (container_4.AccValue1) + "/" + (container_4.AccValue2);
            row_2_column_1_wsp.Content = (container_5.AccValue1) + "/" + (container_5.AccValue2);
            row_2_column_3_wsp.Content = (container_6.AccValue1) + "/" + (container_6.AccValue2);

            //滑移率
            row_3_column_1_wsp.Content = container_1.SlipRate1 + "/" + container_1.SlipRate2;
            row_3_column_3_wsp.Content = container_2.SlipRate1 + "/" + container_2.SlipRate2;
            row_4_column_1_wsp.Content = container_3.SlipRate1 + "/" + container_3.SlipRate2;
            row_4_column_3_wsp.Content = container_4.SlipRate1 + "/" + container_4.SlipRate2;
            row_5_column_1_wsp.Content = container_5.SlipRate1 + "/" + container_5.SlipRate2;
            row_5_column_3_wsp.Content = container_6.SlipRate1 + "/" + container_6.SlipRate2;

            //滑行等级
            row_6_column_1_wsp.Content = container_1.SlipLvl1 + "/" + container_1.SlipLvl2;
            row_6_column_3_wsp.Content = container_2.SlipLvl1 + "/" + container_2.SlipLvl2;
            row_7_column_1_wsp.Content = container_3.SlipLvl1 + "/" + container_3.SlipLvl2;
            row_7_column_3_wsp.Content = container_4.SlipLvl1 + "/" + container_4.SlipLvl2;
            row_8_column_1_wsp.Content = container_5.SlipLvl1 + "/" + container_5.SlipLvl2;
            row_8_column_3_wsp.Content = container_6.SlipLvl1 + "/" + container_6.SlipLvl2;
        }

        /// <summary>
        /// 更新速度曲线数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_UpdateSpeedData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            //2018-10-8:速度增加至12个
            row_0_column_0_Sp.Content = (container_1.RefSpeed / 10);
            row_1_column_0_Sp.Content = (container_1.SpeedA1Shaft1 / 10) + "/" + (container_1.SpeedA1Shaft2 / 10);
            row_1_column_3_Sp.Content = (container_2.SpeedShaft1 / 10) + "/" + (container_2.SpeedShaft2 / 10);
            row_2_column_0_Sp.Content = (container_3.SpeedShaft1 / 10) + "/" + (container_3.SpeedShaft2 / 10);
            row_2_column_3_Sp.Content = (container_4.SpeedShaft1 / 10) + "/" + (container_4.SpeedShaft2 / 10);
            row_3_column_0_Sp.Content = (container_5.SpeedShaft1 / 10) + "/" + (container_5.SpeedShaft2 / 10);
            row_3_column_3_Sp.Content = (container_6.SpeedA1Shaft1 / 10) + "/" + (container_6.SpeedA1Shaft2 / 10);

            row_4_column_0_Sp.Content = (container_1.RefSpeed - container_1.SpeedA1Shaft1) / 10 + "/" + (container_1.RefSpeed - container_1.SpeedA1Shaft2) / 10;
            row_4_column_3_Sp.Content = (container_1.RefSpeed - container_2.SpeedShaft1) / 10 + "/" + (container_1.RefSpeed - container_2.SpeedShaft2) / 10;
            row_5_column_0_Sp.Content = (container_1.RefSpeed - container_3.SpeedShaft1) / 10 + "/" + (container_1.RefSpeed - container_3.SpeedShaft2) / 10;
            row_5_column_3_Sp.Content = (container_1.RefSpeed - container_4.SpeedShaft1) / 10 + "/" + (container_1.RefSpeed - container_4.SpeedShaft2) / 10;
            row_6_column_0_Sp.Content = (container_1.RefSpeed - container_5.SpeedShaft1) / 10 + "/" + (container_1.RefSpeed - container_5.SpeedShaft2) / 10;
            row_6_column_3_Sp.Content = (container_1.RefSpeed - container_6.SpeedA1Shaft1) / 10 + "/" + (container_1.RefSpeed - container_6.SpeedA1Shaft2) / 10;
        }

        /// <summary>
        /// 更新制动缸曲线数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_UpdatePressData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_0_Pr.Content = container_1.Bcp1PressureAx1;
            row_0_column_3_Pr.Content = container_1.Bcp2PressureAx2;

            row_1_column_0_Pr.Content = container_2.Bcp1Pressure;
            row_1_column_3_Pr.Content = container_2.Bcp2Pressure;

            row_2_column_0_Pr.Content = container_3.Bcp1Pressure;
            row_2_column_3_Pr.Content = container_3.Bcp2Pressure;

            row_3_column_0_Pr.Content = container_4.Bcp1Pressure;
            row_3_column_3_Pr.Content = container_4.Bcp2Pressure;

            row_4_column_0_Pr.Content = container_5.Bcp1Pressure;
            row_4_column_3_Pr.Content = container_5.Bcp2Pressure;

            row_5_column_0_Pr.Content = container_6.Bcp1PressureAx1;
            row_5_column_3_Pr.Content = container_6.Bcp2PressureAx2;
        }
        #endregion

        #region Append Data
        /// <summary>
        /// 在图表中添加WSP数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_AppendWSPSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            Antiskid_accValue1_1.AppendAsync(base.Dispatcher, new Point(x, container_1.AccValue1));
            Antiskid_accValue2_1.AppendAsync(base.Dispatcher, new Point(x, container_2.AccValue1));
            Antiskid_accValue3_1.AppendAsync(base.Dispatcher, new Point(x, container_3.AccValue1));
            Antiskid_accValue4_1.AppendAsync(base.Dispatcher, new Point(x, container_4.AccValue1));
            Antiskid_accValue5_1.AppendAsync(base.Dispatcher, new Point(x, container_5.AccValue1));
            Antiskid_accValue6_1.AppendAsync(base.Dispatcher, new Point(x, container_6.AccValue1));

            Antiskid_accValue1_2.AppendAsync(base.Dispatcher, new Point(x, container_1.AccValue2));
            Antiskid_accValue2_2.AppendAsync(base.Dispatcher, new Point(x, container_2.AccValue2));
            Antiskid_accValue3_2.AppendAsync(base.Dispatcher, new Point(x, container_3.AccValue2));
            Antiskid_accValue4_2.AppendAsync(base.Dispatcher, new Point(x, container_4.AccValue2));
            Antiskid_accValue5_2.AppendAsync(base.Dispatcher, new Point(x, container_5.AccValue2));
            Antiskid_accValue6_2.AppendAsync(base.Dispatcher, new Point(x, container_6.AccValue2));

            //2018-10-11:滑移率暂时不用显示在曲线上
            //Antiskid_slipRate1.AppendAsync(base.Dispatcher, new Point(x, container_1.SlipRate1));
            //Antiskid_slipRate2.AppendAsync(base.Dispatcher, new Point(x, container_2.SlipRate1));
            //Antiskid_slipRate3.AppendAsync(base.Dispatcher, new Point(x, container_3.SlipRate1));
            //Antiskid_slipRate4.AppendAsync(base.Dispatcher, new Point(x, container_4.SlipRate1));
            //Antiskid_slipRate5.AppendAsync(base.Dispatcher, new Point(x, container_5.SlipRate1));
            //Antiskid_slipRate6.AppendAsync(base.Dispatcher, new Point(x, container_6.SlipRate1));
        }

        /// <summary>
        /// 2019-12-25:向图表中添加滑移率的数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_AppendSlipRateSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            #region 计算各车的滑移率
            if (container_1.GateValveState)
            {
                container_1.SlipRate1 = ((container_1.RefSpeed - container_1.SpeedA1Shaft1) / container_1.RefSpeed) * 100;
                container_1.SlipRate2 = ((container_1.RefSpeed - container_1.SpeedA1Shaft2) / container_1.RefSpeed) * 100;
            } else
            {
                container_1.SlipRate1 = ((container_6.RefSpeed - container_1.SpeedA1Shaft1) / container_6.RefSpeed) * 100;
                container_1.SlipRate2 = ((container_6.RefSpeed - container_1.SpeedA1Shaft2) / container_6.RefSpeed) * 100;
            }

            if (container_2.GateValveState)
            {
                container_2.SlipRate1 = ((container_1.RefSpeed - container_2.SpeedShaft1) / container_1.RefSpeed) * 100;
                container_2.SlipRate2 = ((container_1.RefSpeed - container_2.SpeedShaft1) / container_1.RefSpeed) * 100;
            }
            else
            {
                container_2.SlipRate1 = ((container_6.RefSpeed - container_2.SpeedShaft1) / container_6.RefSpeed) * 100;
                container_2.SlipRate2 = ((container_6.RefSpeed - container_2.SpeedShaft1) / container_6.RefSpeed) * 100;
            }

            if (container_3.GateValveState)
            {
                container_3.SlipRate1 = ((container_1.RefSpeed - container_3.SpeedShaft1) / container_1.RefSpeed) * 100;
                container_3.SlipRate2 = ((container_1.RefSpeed - container_3.SpeedShaft1) / container_1.RefSpeed) * 100;
            }
            else
            {
                container_3.SlipRate1 = ((container_6.RefSpeed - container_3.SpeedShaft1) / container_6.RefSpeed) * 100;
                container_3.SlipRate2 = ((container_6.RefSpeed - container_3.SpeedShaft1) / container_6.RefSpeed) * 100;
            }

            if (container_4.GateValveState)
            {
                container_4.SlipRate1 = ((container_1.RefSpeed - container_4.SpeedShaft1) / container_1.RefSpeed) * 100;
                container_4.SlipRate2 = ((container_1.RefSpeed - container_4.SpeedShaft1) / container_1.RefSpeed) * 100;
            }
            else
            {
                container_4.SlipRate1 = ((container_6.RefSpeed - container_4.SpeedShaft1) / container_6.RefSpeed) * 100;
                container_4.SlipRate2 = ((container_6.RefSpeed - container_4.SpeedShaft1) / container_6.RefSpeed) * 100;
            }

            if (container_5.GateValveState)
            {
                container_5.SlipRate1 = ((container_1.RefSpeed - container_5.SpeedShaft1) / container_1.RefSpeed) * 100;
                container_5.SlipRate2 = ((container_1.RefSpeed - container_5.SpeedShaft1) / container_1.RefSpeed) * 100;
            }
            else
            {
                container_5.SlipRate1 = ((container_6.RefSpeed - container_5.SpeedShaft1) / container_6.RefSpeed) * 100;
                container_5.SlipRate2 = ((container_6.RefSpeed - container_5.SpeedShaft1) / container_6.RefSpeed) * 100;
            }

            if (container_6.GateValveState)
            {
                container_6.SlipRate1 = ((container_1.RefSpeed - container_6.SpeedA1Shaft1) / container_1.RefSpeed) * 100;
                container_6.SlipRate2 = ((container_1.RefSpeed - container_6.SpeedA1Shaft2) / container_1.RefSpeed) * 100;
            }
            else
            {
                container_6.SlipRate1 = ((container_6.RefSpeed - container_6.SpeedA1Shaft1) / container_6.RefSpeed) * 100;
                container_6.SlipRate2 = ((container_6.RefSpeed - container_6.SpeedA1Shaft2) / container_6.RefSpeed) * 100;
            }
            #endregion
            Antiskid_slipRate1_1.AppendAsync(base.Dispatcher, new Point(x, container_1.SlipRate1));
            Antiskid_slipRate1_2.AppendAsync(base.Dispatcher, new Point(x, container_1.SlipRate2));
            Antiskid_slipRate2_1.AppendAsync(base.Dispatcher, new Point(x, container_2.SlipRate1));
            Antiskid_slipRate2_2.AppendAsync(base.Dispatcher, new Point(x, container_2.SlipRate2));
            Antiskid_slipRate3_1.AppendAsync(base.Dispatcher, new Point(x, container_3.SlipRate1));
            Antiskid_slipRate3_2.AppendAsync(base.Dispatcher, new Point(x, container_3.SlipRate2));
            Antiskid_slipRate4_1.AppendAsync(base.Dispatcher, new Point(x, container_4.SlipRate1));
            Antiskid_slipRate4_2.AppendAsync(base.Dispatcher, new Point(x, container_4.SlipRate2));
            Antiskid_slipRate5_1.AppendAsync(base.Dispatcher, new Point(x, container_5.SlipRate1));
            Antiskid_slipRate5_2.AppendAsync(base.Dispatcher, new Point(x, container_5.SlipRate2));
            Antiskid_slipRate6_1.AppendAsync(base.Dispatcher, new Point(x, container_6.SlipRate1));
            Antiskid_slipRate6_2.AppendAsync(base.Dispatcher, new Point(x, container_6.SlipRate2));
        }



        /// <summary>
        /// 在图表中添加压力数据
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_AppendPressSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            //Antiskid_cylinder1.AppendAsync(base.Dispatcher, new Point(x, (container_1.Bcp1PressureAx1 + container_1.Bcp2PressureAx2) / 2));
            //Antiskid_cylinder2.AppendAsync(base.Dispatcher, new Point(x, (container_2.Bcp1Pressure + container_2.Bcp2Pressure) / 2));
            //Antiskid_cylinder3.AppendAsync(base.Dispatcher, new Point(x, (container_3.Bcp1Pressure + container_3.Bcp2Pressure) / 2));
            //Antiskid_cylinder4.AppendAsync(base.Dispatcher, new Point(x, (container_4.Bcp1Pressure + container_4.Bcp2Pressure) / 2));
            //Antiskid_cylinder5.AppendAsync(base.Dispatcher, new Point(x, (container_5.Bcp1Pressure + container_5.Bcp2Pressure) / 2));
            //Antiskid_cylinder6.AppendAsync(base.Dispatcher, new Point(x, (container_6.Bcp1PressureAx1 + container_6.Bcp2PressureAx2) / 2));
            Antiskid_cylinder1_1.AppendAsync(base.Dispatcher, new Point(x, container_1.Bcp1PressureAx1));
            Antiskid_cylinder2_1.AppendAsync(base.Dispatcher, new Point(x, container_2.Bcp1Pressure));
            Antiskid_cylinder3_1.AppendAsync(base.Dispatcher, new Point(x, container_3.Bcp1Pressure));
            Antiskid_cylinder4_1.AppendAsync(base.Dispatcher, new Point(x, container_4.Bcp1Pressure));
            Antiskid_cylinder5_1.AppendAsync(base.Dispatcher, new Point(x, container_5.Bcp1Pressure));
            Antiskid_cylinder6_1.AppendAsync(base.Dispatcher, new Point(x, container_6.Bcp1PressureAx1));

            Antiskid_cylinder1_2.AppendAsync(base.Dispatcher, new Point(x, container_1.Bcp2PressureAx2));
            Antiskid_cylinder2_2.AppendAsync(base.Dispatcher, new Point(x, container_2.Bcp2Pressure));
            Antiskid_cylinder3_2.AppendAsync(base.Dispatcher, new Point(x, container_3.Bcp2Pressure));
            Antiskid_cylinder4_2.AppendAsync(base.Dispatcher, new Point(x, container_4.Bcp2Pressure));
            Antiskid_cylinder5_2.AppendAsync(base.Dispatcher, new Point(x, container_5.Bcp2Pressure));
            Antiskid_cylinder6_2.AppendAsync(base.Dispatcher, new Point(x, container_6.Bcp2PressureAx2));
        }

        /// <summary>
        /// 在图表中添加速度值
        /// </summary>
        /// <param name="container_1"></param>
        /// <param name="container_2"></param>
        /// <param name="container_3"></param>
        /// <param name="container_4"></param>
        /// <param name="container_5"></param>
        /// <param name="container_6"></param>
        private void Antiskid_AppendSpeedSource(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            //Antiskid_speed1.AppendAsync(base.Dispatcher, new Point(x, (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2));
            //Antiskid_speed2.AppendAsync(base.Dispatcher, new Point(x, (container_2.SpeedShaft1 + container_2.SpeedShaft2) / 2));
            //Antiskid_speed3.AppendAsync(base.Dispatcher, new Point(x, (container_3.SpeedShaft1 + container_3.SpeedShaft2) / 2));
            //Antiskid_speed4.AppendAsync(base.Dispatcher, new Point(x, (container_4.SpeedShaft1 + container_4.SpeedShaft2) / 2));
            //Antiskid_speed5.AppendAsync(base.Dispatcher, new Point(x, (container_5.SpeedShaft1 + container_5.SpeedShaft2) / 2));
            //Antiskid_speed6.AppendAsync(base.Dispatcher, new Point(x, (container_6.SpeedA1Shaft1 + container_6.SpeedA1Shaft2) / 2));
            Antiskid_refSpeed.AppendAsync(base.Dispatcher, new Point(x, container_1.RefSpeed / 10));
            Antiskid_speed1_1.AppendAsync(base.Dispatcher, new Point(x, container_1.SpeedA1Shaft1 / 10));
            Antiskid_speed1_2.AppendAsync(base.Dispatcher, new Point(x, container_1.SpeedA1Shaft2 / 10));
            Antiskid_speed2_1.AppendAsync(base.Dispatcher, new Point(x, container_2.SpeedShaft1 / 10));
            Antiskid_speed2_2.AppendAsync(base.Dispatcher, new Point(x, container_2.SpeedShaft2 / 10));
            Antiskid_speed3_1.AppendAsync(base.Dispatcher, new Point(x, container_3.SpeedShaft1 / 10));
            Antiskid_speed3_2.AppendAsync(base.Dispatcher, new Point(x, container_3.SpeedShaft2 / 10));
            Antiskid_speed4_1.AppendAsync(base.Dispatcher, new Point(x, container_4.SpeedShaft1 / 10));
            Antiskid_speed4_2.AppendAsync(base.Dispatcher, new Point(x, container_4.SpeedShaft2 / 10));
            Antiskid_speed5_1.AppendAsync(base.Dispatcher, new Point(x, container_5.SpeedShaft1 / 10));
            Antiskid_speed5_2.AppendAsync(base.Dispatcher, new Point(x, container_5.SpeedShaft2 / 10));
            Antiskid_speed6_1.AppendAsync(base.Dispatcher, new Point(x, container_6.SpeedA1Shaft1 / 10));
            Antiskid_speed6_2.AppendAsync(base.Dispatcher, new Point(x, container_6.SpeedA1Shaft2 / 10));
        }
        #endregion

        private void Antiskid_speedChart_MouseMove(object sender, MouseEventArgs e)
        {

        }

        #region window event
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


        private void Antiskid_Display_Window_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "Antiskid_Display_Window");
        }

        private void Antiskid_SlipRateChart_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
