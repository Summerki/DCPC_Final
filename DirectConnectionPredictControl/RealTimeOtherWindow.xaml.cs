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
    /// RealTimeOtherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeOtherWindow : Window
    {

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

        private int x = 0;
        private Queue<int> queue = new Queue<int>();
        public event closeWindowHandler CloseWindowEvent;
        private int xaxis = 0;

        public RealTimeOtherWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            airPressreChart.AddLineGraph(airPressure1, Colors.DodgerBlue, 1.0, "1轴空簧压力");
            airPressreChart.AddLineGraph(airPressure2, Colors.DarkOrange, 1.0, "2轴空簧压力");
            airPressreChart.AddLineGraph(airPressure3, Colors.LimeGreen, 1.0, "3轴空簧压力");
            airPressreChart.AddLineGraph(airPressure4, Colors.Violet, 1.0, "4轴空簧压力");
            airPressreChart.AddLineGraph(airPressure5, Colors.Tomato, 1.0, "5轴空簧压力");
            airPressreChart.AddLineGraph(airPressure6, Colors.Brown, 1.0, "6轴空簧压力");

            loadPressureChart.AddLineGraph(loadPressure1, Colors.DodgerBlue, 1.0, "1轴载荷");
            loadPressureChart.AddLineGraph(loadPressure2, Colors.DarkOrange, 1.0, "2轴载荷");
            loadPressureChart.AddLineGraph(loadPressure3, Colors.LimeGreen, 1.0, "3轴载荷");
            loadPressureChart.AddLineGraph(loadPressure4, Colors.Violet, 1.0, "4轴载荷");
            loadPressureChart.AddLineGraph(loadPressure5, Colors.Tomato, 1.0, "5轴载荷");
            loadPressureChart.AddLineGraph(loadPressure6, Colors.Brown, 1.0, "6轴载荷");

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

        public void UpdateData(MainDevDataContains mainDevData1, SliverDataContainer sliverData2, SliverDataContainer sliverData3, SliverDataContainer sliverData4, SliverDataContainer sliverData5, MainDevDataContains mainDevData6)
        {
            airPressure1.AppendAsync(base.Dispatcher, new Point(x, (mainDevData1.AirSpring1PressureA1Car1 + mainDevData1.AirSpring2PressureA1Car1) / 2));
            airPressure2.AppendAsync(base.Dispatcher, new Point(x, (sliverData2.AirSpringPressure1 + sliverData2.AirSpringPressure2) / 2));
            airPressure3.AppendAsync(base.Dispatcher, new Point(x, (sliverData3.AirSpringPressure1 + sliverData3.AirSpringPressure2) / 2));
            airPressure4.AppendAsync(base.Dispatcher, new Point(x, (sliverData4.AirSpringPressure1 + sliverData4.AirSpringPressure2) / 2));
            airPressure5.AppendAsync(base.Dispatcher, new Point(x, (sliverData5.AirSpringPressure1 + sliverData5.AirSpringPressure2) / 2));
            airPressure6.AppendAsync(base.Dispatcher, new Point(x, (mainDevData6.AirSpring1PressureA1Car1 + mainDevData6.AirSpring2PressureA1Car1) / 2));

            loadPressure1.AppendAsync(base.Dispatcher, new Point(x, mainDevData1.MassA1));
            loadPressure2.AppendAsync(base.Dispatcher, new Point(x, sliverData2.MassValue));
            loadPressure3.AppendAsync(base.Dispatcher, new Point(x, sliverData3.MassValue));
            loadPressure4.AppendAsync(base.Dispatcher, new Point(x, sliverData4.MassValue));
            loadPressure5.AppendAsync(base.Dispatcher, new Point(x, sliverData5.MassValue));
            loadPressure6.AppendAsync(base.Dispatcher, new Point(x, mainDevData6.MassA1));
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
                airPressreChart.Viewport.Visible = new Rect(xaxis, 0, 60, 1260);
                loadPressureChart.Viewport.Visible = new Rect(xaxis, 0, 60, 1260);
            });
            x++;
        }

        private void ChartWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(false, "otherChart");
        }
    }
}
