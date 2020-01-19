using System;
using System.Collections.Generic;
using System.Data;
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
using DevExpress.Xpf.Charts;
using DirectConnectionPredictControl.CommenTool;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// SingleChart.xaml 的交互逻辑
    /// </summary>
    public partial class SingleChart : Window
    {

        public event closeWindowHandler CloseWindowEvent;
        private static int X_LENGTH = 60;
        private static int MAX_POINT_COUNT = 1200;
        private DataTable table_1;
        private DataTable table_2;
        private DataTable table_3;
        private DataTable table_4;
        private DataTable table_5;
        private DataTable table_6;
        private ChartToolTipController controller = new ChartToolTipController();

        private double x;

        public SingleChart()
        {
            InitializeComponent();
            Init();
        }

        #region busniss methods

        public static void SetMaxPointCount(int count)
        {
            MAX_POINT_COUNT = count;
        }

        public static void SetXLength(int length)
        {
            X_LENGTH = length;
        }

        private void Init()
        {
            x = 0.0;
        }

        public void SetHistoryModel(HistoryModel history)
        {
            
            table_1 = Utils.ToDataTable(history.Containers_1);
            table_2 = Utils.ToDataTable(history.Containers_2);
            table_3 = Utils.ToDataTable(history.Containers_3);
            table_4 = Utils.ToDataTable(history.Containers_4);
            table_5 = Utils.ToDataTable(history.Containers_5);
            table_6 = Utils.ToDataTable(history.Containers_6);
            ChangeToHistoryStyle();
        }

        public void PaintHistory()
        {
            speedAX1.ArgumentDataMember = "X";
            speedAX1.ValueDataMember = "SpeedA1Shaft1";
            speedAX1.DataSource = table_1;

            speedAX2.ArgumentDataMember = "X";
            speedAX2.ValueDataMember = "SpeedShaft1";
            speedAX2.DataSource = table_2;

            speedAX3.ArgumentDataMember = "X";
            speedAX3.ValueDataMember = "SpeedShaft1";
            speedAX3.DataSource = table_3;
            speedAX4.ArgumentDataMember = "X";
            speedAX4.ValueDataMember = "SpeedShaft1";
            speedAX4.DataSource = table_4;
            speedAX5.ArgumentDataMember = "X";
            speedAX5.ValueDataMember = "SpeedShaft1";
            speedAX5.DataSource = table_5;
            speedAX6.ArgumentDataMember = "X";
            speedAX6.ValueDataMember = "SpeedA1Shaft1";
            speedAX6.DataSource = table_6;

            bcp1AX1.ArgumentDataMember = "X";
            bcp1AX1.ValueDataMember = "Bcp1PressureAx1";
            bcp1AX1.DataSource = table_1;

            bcp2AX1.ArgumentDataMember = "X";
            bcp2AX1.ValueDataMember = "Bcp2PressureAx2";
            bcp2AX1.DataSource = table_1;

            bcp1AX2.ArgumentDataMember = "X";
            bcp1AX2.ValueDataMember = "Bcp1Pressure";
            bcp1AX2.DataSource = table_2;

            bcp2AX2.ArgumentDataMember = "X";
            bcp2AX2.ValueDataMember = "Bcp2Pressure";
            bcp2AX2.DataSource = table_2;

            bcp1AX3.ArgumentDataMember = "X";
            bcp1AX3.ValueDataMember = "Bcp1Pressure";
            bcp1AX3.DataSource = table_3;

            bcp2AX3.ArgumentDataMember = "X";
            bcp2AX3.ValueDataMember = "Bcp2Pressure";
            bcp2AX3.DataSource = table_3;

            bcp1AX4.ArgumentDataMember = "X";
            bcp1AX4.ValueDataMember = "Bcp1Pressure";
            bcp1AX4.DataSource = table_4;

            bcp2AX4.ArgumentDataMember = "X";
            bcp2AX4.ValueDataMember = "Bcp2Pressure";
            bcp2AX4.DataSource = table_4;

            bcp1AX5.ArgumentDataMember = "X";
            bcp1AX5.ValueDataMember = "Bcp1Pressure";
            bcp1AX5.DataSource = table_5;

            bcp2AX5.ArgumentDataMember = "X";
            bcp2AX5.ValueDataMember = "Bcp2Pressure";
            bcp2AX5.DataSource = table_5;

            bcp1AX6.ArgumentDataMember = "X";
            bcp1AX6.ValueDataMember = "Bcp1PressureAx1";
            bcp1AX6.DataSource = table_6;

            bcp2AX6.ArgumentDataMember = "X";
            bcp2AX6.ValueDataMember = "Bcp2PressureAx2";
            bcp2AX6.DataSource = table_6;
            //for (int i = 0; i < history.Count; i++)
            //{
            //    speedAX1.AddPoint(history.X[i], (history.Containers_1[i].SpeedA1Shaft1 + history.Containers_1[i].SpeedA1Shaft2) / 2);
            //    speedAX2.AddPoint(history.X[i], (history.Containers_2[i].SpeedShaft1 + history.Containers_2[i].SpeedShaft2) / 2);
            //    speedAX3.AddPoint(history.X[i], (history.Containers_3[i].SpeedShaft1 + history.Containers_3[i].SpeedShaft2) / 2);
            //    speedAX4.AddPoint(history.X[i], (history.Containers_4[i].SpeedShaft1 + history.Containers_4[i].SpeedShaft2) / 2);
            //    speedAX5.AddPoint(history.X[i], (history.Containers_5[i].SpeedShaft1 + history.Containers_5[i].SpeedShaft2) / 2);
            //    speedAX6.AddPoint(history.X[i], (history.Containers_6[i].SpeedA1Shaft1 + history.Containers_6[i].SpeedA1Shaft2) / 2);

            //    bcp1AX1.AddPoint(history.X[i], history.Containers_1[i].Bcp1PressureAx1);
            //    bcp2AX1.AddPoint(history.X[i], history.Containers_1[i].Bcp2PressureAx2);

            //    bcp1AX2.AddPoint(history.X[i], history.Containers_2[i].Bcp1Pressure);
            //    bcp2AX2.AddPoint(history.X[i], history.Containers_2[i].Bcp2Pressure);

            //    bcp1AX3.AddPoint(history.X[i], history.Containers_3[i].Bcp1Pressure);
            //    bcp2AX3.AddPoint(history.X[i], history.Containers_3[i].Bcp2Pressure);

            //    bcp1AX4.AddPoint(history.X[i], history.Containers_4[i].Bcp1Pressure);
            //    bcp2AX4.AddPoint(history.X[i], history.Containers_4[i].Bcp2Pressure);

            //    bcp1AX5.AddPoint(history.X[i], history.Containers_5[i].Bcp1Pressure);
            //    bcp2AX5.AddPoint(history.X[i], history.Containers_5[i].Bcp2Pressure);

            //    bcp1AX6.AddPoint(history.X[i], history.Containers_6[i].Bcp1PressureAx1);
            //    bcp2AX6.AddPoint(history.X[i], history.Containers_6[i].Bcp2PressureAx2);
            //}
        }

        private void ChangeToHistoryStyle()
        {
            chartDiagramAX1.AxisX.WholeRange.MaxValue = Math.Floor((double)table_1.Rows.Count / 10) + 10;
            chartDiagramAX1.AxisX.VisualRange.MaxValue = Math.Floor((double)table_1.Rows.Count / 10) + 10;
            chartDiagramAX2.AxisX.WholeRange.MaxValue = Math.Floor((double)table_2.Rows.Count / 10) + 10;
            chartDiagramAX2.AxisX.VisualRange.MaxValue = Math.Floor((double)table_2.Rows.Count / 10) + 10;
            chartDiagramAX3.AxisX.WholeRange.MaxValue = Math.Floor((double)table_3.Rows.Count / 10) + 10;
            chartDiagramAX3.AxisX.VisualRange.MaxValue = Math.Floor((double)table_3.Rows.Count / 10) + 10;
            chartDiagramAX4.AxisX.WholeRange.MaxValue = Math.Floor((double)table_4.Rows.Count / 10) + 10;
            chartDiagramAX4.AxisX.VisualRange.MaxValue = Math.Floor((double)table_4.Rows.Count / 10) + 10;
            chartDiagramAX5.AxisX.WholeRange.MaxValue = Math.Floor((double)table_5.Rows.Count / 10) + 10;
            chartDiagramAX5.AxisX.VisualRange.MaxValue = Math.Floor((double)table_5.Rows.Count / 10) + 10;
            chartDiagramAX6.AxisX.WholeRange.MaxValue = Math.Floor((double)table_6.Rows.Count / 10) + 10;
            chartDiagramAX6.AxisX.VisualRange.MaxValue = Math.Floor((double)table_6.Rows.Count / 10) + 10;

            chartDiagramAX1.EnableAxisXNavigation = true;
            chartDiagramAX2.EnableAxisXNavigation = true;
            chartDiagramAX3.EnableAxisXNavigation = true;
            chartDiagramAX4.EnableAxisXNavigation = true;
            chartDiagramAX5.EnableAxisXNavigation = true;
            chartDiagramAX6.EnableAxisXNavigation = true;
        }

        #region 实时曲线绘制方法
        private void SetAxisLimit()
        {
            if (x > X_LENGTH)
            {
                chartDiagramAX1.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX1.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX1.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX1.AxisX.VisualRange.MaxValue = x;

                chartDiagramAX2.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX2.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX2.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX2.AxisX.VisualRange.MaxValue = x;

                chartDiagramAX3.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX3.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX3.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX3.AxisX.VisualRange.MaxValue = x;

                chartDiagramAX4.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX4.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX4.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX4.AxisX.VisualRange.MaxValue = x;

                chartDiagramAX5.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX5.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX5.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX5.AxisX.VisualRange.MaxValue = x;

                chartDiagramAX6.AxisX.WholeRange.MinValue = x - X_LENGTH;
                chartDiagramAX6.AxisX.WholeRange.MaxValue = x;
                chartDiagramAX6.AxisX.VisualRange.MinValue = x - X_LENGTH;
                chartDiagramAX6.AxisX.VisualRange.MaxValue = x;
            }
            

        }

        private void ClearPoint()
        {
            if (bcp1AX1.Points.Count > MAX_POINT_COUNT)
            {
                bcp1AX1.Points.RemoveAt(0);
                bcp2AX1.Points.RemoveAt(0);
                bcp1AX2.Points.RemoveAt(0);
                bcp2AX2.Points.RemoveAt(0);
                bcp1AX3.Points.RemoveAt(0);
                bcp2AX3.Points.RemoveAt(0);
                bcp1AX4.Points.RemoveAt(0);
                bcp2AX4.Points.RemoveAt(0);
                bcp1AX5.Points.RemoveAt(0);
                bcp2AX5.Points.RemoveAt(0);
                bcp1AX6.Points.RemoveAt(0);
                bcp2AX6.Points.RemoveAt(0);
            }
            if (speedAX1.Points.Count > MAX_POINT_COUNT)
            {
                speedAX1.Points.RemoveAt(0);
                speedAX2.Points.RemoveAt(0);
                speedAX3.Points.RemoveAt(0);
                speedAX4.Points.RemoveAt(0);
                speedAX5.Points.RemoveAt(0);
                speedAX6.Points.RemoveAt(0);
            }
            if (refSpeedAX1.Points.Count > MAX_POINT_COUNT)
            {
                refSpeedAX1.Points.RemoveAt(0);
            }

        }

        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.Dispatcher.Invoke(() =>
            {
                ClearPoint();
                SetAxisLimit();
                AddSpeed(container_1, container_2, container_3, container_4, container_5, container_6);
                AddRefSpeed(container_1);
                AddBcp(container_1, container_2, container_3, container_4, container_5, container_6);
                AddLoad(container_1, container_2, container_3, container_4, container_5, container_6);
                UpdateDI(container_1, container_2, container_3, container_4, container_5, container_6);
            });
            x = x + 0.1;
        }

        private void AddLoad(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {

        }

        private void AddBcp(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            bcp1AX1.AddPoint(x, container_1.Bcp1PressureAx1);
            bcp2AX1.AddPoint(x, container_1.Bcp2PressureAx2);

            bcp1AX2.AddPoint(x, container_2.Bcp1Pressure);
            bcp2AX2.AddPoint(x, container_2.Bcp2Pressure);

            bcp1AX3.AddPoint(x, container_3.Bcp1Pressure);
            bcp2AX3.AddPoint(x, container_3.Bcp2Pressure);

            bcp1AX4.AddPoint(x, container_4.Bcp1Pressure);
            bcp2AX4.AddPoint(x, container_4.Bcp2Pressure);

            bcp1AX5.AddPoint(x, container_5.Bcp1Pressure);
            bcp2AX5.AddPoint(x, container_5.Bcp2Pressure);

            bcp1AX6.AddPoint(x, container_6.Bcp1PressureAx1);
            bcp2AX6.AddPoint(x, container_6.Bcp2PressureAx2);
        }

        private void AddRefSpeed(MainDevDataContains container_1)
        {
            refSpeedAX1.AddPoint(x, container_1.RefSpeed);
        }

        private void AddSpeed(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            speedAX1.AddPoint(x, (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2);
            speedAX2.AddPoint(x, (container_2.SpeedShaft1 + container_2.SpeedShaft2) / 2);
            speedAX3.AddPoint(x, (container_3.SpeedShaft1 + container_3.SpeedShaft2) / 2);
            speedAX4.AddPoint(x, (container_4.SpeedShaft1 + container_4.SpeedShaft2) / 2);
            speedAX5.AddPoint(x, (container_5.SpeedShaft1 + container_5.SpeedShaft2) / 2);
            speedAX6.AddPoint(x, (container_6.SpeedA1Shaft1 + container_6.SpeedA1Shaft2) / 2);
            
        }

        private void UpdateDI(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            row_0_column_1_Ax1.Content = (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2;
            row_1_column_1_Ax1.Content = container_1.RefSpeed;
            row_2_column_1_Ax1.Content = container_1.MassA1;
            row_0_column_3_Ax1.Content = container_1.Bcp1PressureAx1;
            row_1_column_3_Ax1.Content = container_1.Bcp2PressureAx2;

            row_0_column_1_Ax2.Content = (container_2.SpeedShaft1 + container_2.SpeedShaft2) / 2;
            row_1_column_1_Ax2.Content = container_1.RefSpeed;
            row_2_column_1_Ax2.Content = container_2.MassValue;
            row_0_column_3_Ax2.Content = container_2.Bcp1Pressure;
            row_1_column_3_Ax2.Content = container_2.Bcp2Pressure;

            row_0_column_1_Ax3.Content = (container_3.SpeedShaft1 + container_2.SpeedShaft2) / 2;
            row_1_column_1_Ax3.Content = container_1.RefSpeed;
            row_2_column_1_Ax3.Content = container_3.MassValue;
            row_0_column_3_Ax3.Content = container_3.Bcp1Pressure;
            row_1_column_3_Ax3.Content = container_3.Bcp2Pressure;

            row_0_column_1_Ax4.Content = (container_4.SpeedShaft1 + container_2.SpeedShaft2) / 2;
            row_1_column_1_Ax4.Content = container_1.RefSpeed;
            row_2_column_1_Ax4.Content = container_4.MassValue;
            row_0_column_3_Ax4.Content = container_4.Bcp1Pressure;
            row_1_column_3_Ax4.Content = container_4.Bcp2Pressure;

            row_0_column_1_Ax5.Content = (container_5.SpeedShaft1 + container_2.SpeedShaft2) / 2;
            row_1_column_1_Ax5.Content = container_1.RefSpeed;
            row_2_column_1_Ax5.Content = container_5.MassValue;
            row_0_column_3_Ax5.Content = container_5.Bcp1Pressure;
            row_1_column_3_Ax5.Content = container_5.Bcp2Pressure;

            row_0_column_1_Ax6.Content = (container_6.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2;
            row_1_column_1_Ax6.Content = container_6.RefSpeed;
            row_2_column_1_Ax6.Content = container_6.MassA1;
            row_0_column_3_Ax6.Content = container_6.Bcp1PressureAx1;
            row_1_column_3_Ax6.Content = container_6.Bcp2PressureAx2;
        }
        #endregion

        #endregion

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

        private void singleChart_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "single");
        }

        private void openFileItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void chartControlAX1_MouseMove(object sender, MouseEventArgs e)
        {
            ChartHitInfo hitInfo = chartControlAX1.CalcHitInfo(e.GetPosition(this));
            StringBuilder builder = new StringBuilder();
            if (hitInfo.SeriesPoint != null)
            {
                builder.Append(hitInfo.SeriesPoint.Argument);
            }
            if (builder.Length > 0)
            {
                
            }
        }
    }
}
