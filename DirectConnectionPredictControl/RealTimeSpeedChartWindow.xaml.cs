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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DirectConnectionPredictControl.CommenTool;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// RealTimeChartWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    
    public partial class RealTimeSpeedChartWindow : Window
    {
        private ObservableDataSource<Point> speed1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> speed6 = new ObservableDataSource<Point>();
        private int x;
        private Queue<int> queue = new Queue<int>();
        private int xaxis = 0;
        public event closeWindowHandler CloseWindowEvent;
        public RealTimeSpeedChartWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            speedChart.AddLineGraph(speed1, Colors.DodgerBlue, 1.0, "1轴速度");
            speedChart.AddLineGraph(speed2, Colors.DarkOrange, 1.0, "2轴速度");
            speedChart.AddLineGraph(speed3, Colors.LimeGreen, 1.0, "3轴速度");
            speedChart.AddLineGraph(speed4, Colors.Violet, 1.0, "4轴速度");
            speedChart.AddLineGraph(speed5, Colors.Tomato, 1.0, "5轴速度");
            speedChart.AddLineGraph(speed6, Colors.Brown, 1.0, "6轴速度");
            x = 0;
        }

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

        public void UpdateData(MainDevDataContains mainDevData1, SliverDataContainer sliverData2, SliverDataContainer sliverData3, SliverDataContainer sliverData4, SliverDataContainer sliverData5, MainDevDataContains mainDevData6)
        {
            ClearDataSource();
            speed1.AppendAsync(base.Dispatcher, new Point(x, (mainDevData1.SpeedA1Shaft1 + mainDevData1.SpeedA1Shaft2) / 2));
            speed2.AppendAsync(base.Dispatcher, new Point(x, (sliverData2.SpeedShaft1 + sliverData2.SpeedShaft2) / 2));
            speed3.AppendAsync(base.Dispatcher, new Point(x, (sliverData3.SpeedShaft1 + sliverData3.SpeedShaft2) / 2));
            speed4.AppendAsync(base.Dispatcher, new Point(x, (sliverData4.SpeedShaft1 + sliverData4.SpeedShaft2) / 2));
            speed5.AppendAsync(base.Dispatcher, new Point(x, (sliverData5.SpeedShaft1 + sliverData5.SpeedShaft2) / 2));
            speed6.AppendAsync(base.Dispatcher, new Point(x, (mainDevData6.SpeedA1Shaft1 + mainDevData6.SpeedA1Shaft2) / 2));
            if (queue.Count < 60)
            {
                queue.Enqueue(x);
            }
            else
            {
                queue.Dequeue();
                queue.Enqueue(x);
            }
            if (x - 60 > 0)
            {
                xaxis = x - 60;
            }
            else
            {
                xaxis = 0;
            }
            this.Dispatcher.Invoke(() =>
            {
                speedChart.Viewport.Visible = new Rect(xaxis, 0, 60, 180);
            });

            x++;
        }

        private void ClearDataSource()
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

        private void ChartWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "speedChart");
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
