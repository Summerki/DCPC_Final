using DirectConnectionPredictControl.CommenTool;
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

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// SlaveDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SlaveDetailWindow : Window
    {
        private MainDevDataContains mainDevDataContains;
        private SliverDataContainer sliverDataContainer;
        private string carID;

        private delegate void updateUIDelegate(SliverDataContainer sliverDataContainer);
        public event closeWindowHandler CloseWindowEvent;

        private Thread uiThread;

        public SlaveDetailWindow()
        {
            InitializeComponent();
            InitialFaultListView();
        }
    


        public SlaveDetailWindow(MainDevDataContains mainDevDataContains)
        {
            this.mainDevDataContains = mainDevDataContains;
        }

        public SlaveDetailWindow(SliverDataContainer sliverDataContainer, string carID)
        {
            this.sliverDataContainer = sliverDataContainer;
            this.carID = carID;
            InitializeComponent();
            InitialFaultListView();
            Init();
        }

        /// <summary>
        /// 初始化线程控制器，准备开始线程
        /// </summary>
        private void Init()
        {
            this.titleLbl.Content = "详情-" + carID;
            uiThread = new Thread(UIHandler);
            uiThread.IsBackground = true;
            uiThread.Start();
        }

        private void UIHandler()
        {
            updateUIDelegate updateUI = new updateUIDelegate(UIControl);
            while (true)
            {
                Thread.Sleep(Utils.timeInterval);
                this.Dispatcher.Invoke(updateUI, sliverDataContainer);
            }
        }

        /// <summary>
        /// 更新数据源
        /// </summary>
        /// <param name="sliverDataContainer"></param>
        public void UpdateData(SliverDataContainer sliverDataContainer)
        {
            this.sliverDataContainer = sliverDataContainer;
        }

        /// <summary>
        /// UI呈现
        /// </summary>
        /// <param name="sliverDataContainer"></param>
        public void UIControl(SliverDataContainer sliverDataContainer)
        {

            #region TPDO7 UI

            // 0~1 st. bytes
            t7Byte01Tb.Text = sliverDataContainer.LifeSig.ToString();

            // 2 nd. byte
            t7Byte2Bit0Tb.Text = sliverDataContainer.Slip.ToString();
            t7Byte2Bit1Tb.Text = sliverDataContainer.AbBrakeActive.ToString();
            t7Byte2Bit2Tb.Text = sliverDataContainer.BCPLow1.ToString();
            t7Byte2Bit3Tb.Text = sliverDataContainer.ParkBrakeRealease.ToString();
            t7Byte2Bit4Tb.Text = sliverDataContainer.AbBrakeSatet.ToString();
            t7Byte2Bit5Tb.Text = sliverDataContainer.MassSigValid.ToString();
            t7Byte2Bit6Tb.Text = sliverDataContainer.MREPressureEnable1.ToString();

            // 3 rd. byte
            t7Byte3Bit0Tb.Text = sliverDataContainer.Slip.ToString();
            t7Byte3Bit1Tb.Text = sliverDataContainer.EmergencyBrake.ToString();
            t7Byte3Bit3Tb.Text = sliverDataContainer.BrakeRealease.ToString();
            t7Byte3Bit4Tb.Text = sliverDataContainer.BSRLow1.ToString();
            t7Byte3Bit5Tb.Text = sliverDataContainer.ParkBrakeRealease.ToString();
            t7Byte3Bit6Tb.Text = sliverDataContainer.EpState.ToString();
            t7Byte3Bit7Tb.Text = sliverDataContainer.MassSigValid.ToString();

            // 4~5 th. bytes
            t7Byte45Slider.Value = sliverDataContainer.Bcp1Pressure < 0 ? 0 : sliverDataContainer.Bcp1Pressure;

            // 6~7 th. bytes
            t7Byte67Slider.Value = sliverDataContainer.Bcp2Pressure < 0 ? 0 : sliverDataContainer.Bcp2Pressure;
            #endregion

            #region TPDO8 UI

            // 0~1 st. bytes
            t8Byte01Tb.Text = "停放制动缸/总风压力：" + sliverDataContainer.ParkPressure.ToString() + " kpa";

            // 2~3 rd. bytes
            t8Byte23Slider.Value = sliverDataContainer.BrakeCylinderSourcePressure;

            // 4~5 th. bytes
            t8Byte45Slider.Value = sliverDataContainer.MassValue / 1000.0;

            // 6~7 th. bytes
            t8Byte67Slider.Value = sliverDataContainer.ParkPressure;
            #endregion

            #region TPDO9 UI

            // 0~1 st. bytes
            t9Byte01Slider.Value = sliverDataContainer.VldRealPressure;

            // 2~3 rd. bytes
            t9Byte23Tb.Text = "实际空气制动力：" + sliverDataContainer.AbForceValue.ToString() + " kpa";

            // 4~5 th. bytes
            t9Byte45Tb.Text = "空气制动力能力：" + sliverDataContainer.AbCapValue.ToString() + " kpa";
            #endregion

            #region TPDO10 UI

            // 0~1 st. bytes
            t10Byte01Slider.Value = sliverDataContainer.SpeedShaft1;

            // 2~3 rd. bytes
            t10Byte23Slider.Value = sliverDataContainer.SpeedShaft2;

            // 4 th. byte
            t10Byte4Bit0Tb.Text = sliverDataContainer.SpeedShaftEnable1.ToString();
            t10Byte4Bit1Tb.Text = sliverDataContainer.SpeedShaftEnable2.ToString();
            t10Byte4Bit2Tb.Text = sliverDataContainer.HasSlipControl1.ToString();
            t10Byte4Bit3Tb.Text = sliverDataContainer.HasSlipControl2.ToString();
            t10Byte4Bit4Tb.Text = sliverDataContainer.AbControlledBySlip.ToString();

            // 6 th. byte
            t10Byte7Bit3Tb.Text = sliverDataContainer.EmergencyBrakeException.ToString();
            #endregion

            #region 新增
            vldSetupPressureSlider.Value = sliverDataContainer.VldSetupPressure;

            air1PressureSlider.Value = sliverDataContainer.AirSpringPressure1;

            air2PressureSlider.Value = sliverDataContainer.AirSpringPressure2;
            #endregion
        }


        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySlaveDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3;
            this.Height = y1 * 4 / 5;
        }
        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniumBtn2_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBtn2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 窗口拖动函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// 初始化故障列表
        /// </summary>
        private void InitialFaultListView()
        {
            //给ftpListView加载数据
            List<FaultModel> list = new List<FaultModel>();
            FaultModel fault = new FaultModel() { FaultName = "故障1", FaultType = "类型1", FaultPosition = "位置1" };
            list.Add(fault);
            faultListView2.ItemsSource = list;
        }

        private void MySlaveDetailWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, carID);
        }
    }
}
