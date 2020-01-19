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
    /// RealTimePressureChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimePressureChartWindow : Window
    {
        private ObservableDataSource<Point> cylinderAir1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinderAir6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> park6 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder1 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder2 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder3 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder4 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder5 = new ObservableDataSource<Point>();
        private ObservableDataSource<Point> cylinder6 = new ObservableDataSource<Point>();

        private int x;
        private Queue<int> queue = new Queue<int>();
        public event closeWindowHandler CloseWindowEvent;
        private int xaxis = 0;
        public RealTimePressureChartWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            cylinderAirChart.AddLineGraph(cylinder1, Colors.DodgerBlue, 1.0, "1轴制动风缸压力");
            cylinderAirChart.AddLineGraph(cylinder2, Colors.DarkOrange, 1.0, "2轴制动风缸压力");
            cylinderAirChart.AddLineGraph(cylinder3, Colors.LimeGreen, 1.0, "3轴制动风缸压力");
            cylinderAirChart.AddLineGraph(cylinder4, Colors.Violet, 1.0, "4轴制动风缸压力");
            cylinderAirChart.AddLineGraph(cylinder5, Colors.Tomato, 1.0, "5轴制动风缸压力");
            cylinderAirChart.AddLineGraph(cylinder6, Colors.Brown, 1.0, "6轴制动风缸压力");

            parkChart.AddLineGraph(park1, Colors.DodgerBlue, 1.0, "1轴停放缸压力");
            parkChart.AddLineGraph(park2, Colors.DarkOrange, 1.0, "2轴停放缸压力");
            parkChart.AddLineGraph(park3, Colors.LimeGreen, 1.0, "3轴停放缸压力");
            parkChart.AddLineGraph(park4, Colors.Violet, 1.0, "4轴停放缸压力");
            parkChart.AddLineGraph(park5, Colors.Tomato, 1.0, "5轴停放缸压力");
            parkChart.AddLineGraph(park6, Colors.Brown, 1.0, "6轴停放缸压力");

            cylinderChart.AddLineGraph(cylinder1, Colors.DodgerBlue, 1.0, "1轴制动缸压力");
            cylinderChart.AddLineGraph(cylinder2, Colors.DarkOrange, 1.0, "2轴制动缸压力");
            cylinderChart.AddLineGraph(cylinder3, Colors.LimeGreen, 1.0, "3轴制动缸压力");
            cylinderChart.AddLineGraph(cylinder4, Colors.Violet, 1.0, "4轴制动缸压力");
            cylinderChart.AddLineGraph(cylinder5, Colors.Tomato, 1.0, "5轴制动缸压力");
            cylinderChart.AddLineGraph(cylinder6, Colors.Brown, 1.0, "6轴制动缸压力");

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

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PressureChart_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(false, "pressureChart");
        }

        public void UpdateData(MainDevDataContains mainDevData1, SliverDataContainer sliverData2, SliverDataContainer sliverData3, SliverDataContainer sliverData4, SliverDataContainer sliverData5, MainDevDataContains mainDevData6)
        {
            cylinderAir1.AppendAsync(base.Dispatcher, new Point(x, mainDevData1.BrakeCylinderSourcePressure));
            cylinderAir2.AppendAsync(base.Dispatcher, new Point(x, sliverData2.BrakeCylinderSourcePressure));
            cylinderAir3.AppendAsync(base.Dispatcher, new Point(x, sliverData3.BrakeCylinderSourcePressure));
            cylinderAir4.AppendAsync(base.Dispatcher, new Point(x, sliverData4.BrakeCylinderSourcePressure));
            cylinderAir5.AppendAsync(base.Dispatcher, new Point(x, sliverData5.BrakeCylinderSourcePressure));
            cylinderAir6.AppendAsync(base.Dispatcher, new Point(x, mainDevData6.BrakeCylinderSourcePressure));
            park1.AppendAsync(base.Dispatcher, new Point(x, mainDevData1.ParkPressureA1));
            park2.AppendAsync(base.Dispatcher, new Point(x, sliverData2.ParkPressure));
            park3.AppendAsync(base.Dispatcher, new Point(x, sliverData3.ParkPressure));
            park4.AppendAsync(base.Dispatcher, new Point(x, sliverData4.ParkPressure));
            park5.AppendAsync(base.Dispatcher, new Point(x, sliverData5.ParkPressure));
            park6.AppendAsync(base.Dispatcher, new Point(x, mainDevData6.ParkPressureA1));
            cylinder1.AppendAsync(base.Dispatcher, new Point(x, (mainDevData1.BRKCylinder1PressureA11 + mainDevData1.BRKCylinder2PressureA11) / 2));
            cylinder2.AppendAsync(base.Dispatcher, new Point(x, (sliverData2.BRKCylinderPressure11 + sliverData2.BRKCylinderPressure21) / 2));
            cylinder3.AppendAsync(base.Dispatcher, new Point(x, (sliverData3.BRKCylinderPressure11 + sliverData3.BRKCylinderPressure21) / 2));
            cylinder4.AppendAsync(base.Dispatcher, new Point(x, (sliverData4.BRKCylinderPressure11 + sliverData4.BRKCylinderPressure21) / 2));
            cylinder5.AppendAsync(base.Dispatcher, new Point(x, (sliverData5.BRKCylinderPressure11 + sliverData5.BRKCylinderPressure21) / 2));
            cylinder6.AppendAsync(base.Dispatcher, new Point(x, (mainDevData6.BRKCylinder1PressureA11 + mainDevData6.BRKCylinder2PressureA11) / 2));
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
                cylinderAirChart.Viewport.Visible = new Rect(xaxis, 0, 60, 1260);
                parkChart.Viewport.Visible = new Rect(xaxis, 0, 60, 1260);
                cylinderChart.Viewport.Visible = new Rect(xaxis, 0, 60, 1260);
            });

            x++;
        }
    }
}
