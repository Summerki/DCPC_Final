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

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// 全局委托
    /// </summary>
    /// <param name="winState"></param>
    public delegate void closeWindowHandler(bool winState, String name);

    /// <summary>
    /// DetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetailWindow : Window
    {
        private MainDevDataContains mainDevDataContains;
        private SliverDataContainer sliverDataContainer;
        private Thread uiThread;
        private string nodeName;
        private int id;

        private delegate void updateUIDelegate(MainDevDataContains mainDevDataContains);
        public event closeWindowHandler CloseWindowEvent;    
        public DetailWindow()
        {
            InitializeComponent();
            InitialFaultListView();
        }

        /// <summary>
        /// 作为主节点车辆，使用主节点数据类
        /// </summary>
        /// <param name="mainDevDataContains"></param>
        public DetailWindow(MainDevDataContains mainDevDataContains)
        {
            this.mainDevDataContains = mainDevDataContains;
            InitializeComponent();
            InitialFaultListView();
            Init();
        }

        public DetailWindow(MainDevDataContains mainDevDataContains, string nodeName)
        {
            this.mainDevDataContains = mainDevDataContains;
            this.nodeName = nodeName;
            id = int.Parse(nodeName.Last().ToString());
            InitializeComponent();
            InitialFaultListView();
            Init();
        }

        public DetailWindow(MainDevDataContains mainDevDataContains, int id)
        {

        }

        /// <summary>
        /// 初始化线程控制器，准备开始线程
        /// </summary>
        private void Init()
        {
            this.titleLbl.Content = "详情-" + nodeName;
            uiThread = new Thread(UIHandler);
            uiThread.IsBackground = true;
            uiThread.Start();
        }

        private void UIHandler() 
        {
            updateUIDelegate updateUIDelegateImpl = new updateUIDelegate(UIControl);
            while (true)
            {
                Thread.Sleep(Utils.timeInterval);
                this.Dispatcher.Invoke(updateUIDelegateImpl, mainDevDataContains);
            }
        }

        /// <summary>
        /// 作为从节点，使用从节点数据类
        /// </summary>
        /// <param name="sliverDataContainer"></param>
        public DetailWindow(SliverDataContainer sliverDataContainer)
        {
            this.sliverDataContainer = sliverDataContainer;
        }

        /// <summary>
        /// 接收主窗口的通知更改数据
        /// </summary>
        /// <param name="mainDevDataContains"></param>
        public void UpdateData(MainDevDataContains mainDevDataContains)
        {
            this.mainDevDataContains = mainDevDataContains;
        }

        /// <summary>
        /// 实现数据与UI的绑定
        /// </summary>
        public void UIControl(MainDevDataContains mainDevDataContains)
        {
            #region TPDO1 UI
            // 0~1 st. byte
            t1Byte01Tb.Text = mainDevDataContains.LifeSig.ToString();

            // 2 nd. byte
            runModeTb.Text = mainDevDataContains.Mode;
            t1Byte2Bit1Tb.Text = mainDevDataContains.BrakeCmd.ToString();
            t1Byte2Bit0Tb.Text = mainDevDataContains.DriveCmd.ToString();
            lasyCmdTb.Text = mainDevDataContains.LazyCmd.ToString();
            t1Byte2Bit3Tb.Text = mainDevDataContains.FastBrakeCmd.ToString();
            t1Byte2Bit5Tb.Text = mainDevDataContains.EmergencyBrakeCmd.ToString();
            t1Byte2Bit4Tb.Text = mainDevDataContains.HoldBrakeRealease.ToString();
            t1Byte2Bit2Tb.Text = mainDevDataContains.LazyState.ToString();
            t1Byte2Bit6Tb.Text = mainDevDataContains.DriveState.ToString();
            t1Byte2Bit7Tb.Text = mainDevDataContains.NormalBrakeState.ToString();

            // 3 rd. byte
            t1Byte3Bit0Tb.Text = mainDevDataContains.EmergencyBrakeState.ToString();
            t1Byte3Bit1Tb.Text = mainDevDataContains.BrakeLevel.ToString();
            t1Byte3Bit2Tb.Text = mainDevDataContains.TrainBrakeForce.ToString() + " kpa";
            t1Byte3Bit3Tb.Text = mainDevDataContains.SlipA1.ToString();

            // 4~5 th. bytes
            t1Byte45Slider.Value = mainDevDataContains.BrakeCylinderSourcePressure;

            // 6~7 th. bytes
            t1Byte67Tb.Text = "1架空气制动目标值：" + mainDevDataContains.AbTargetValueAx1.ToString() + " kpa";
            #endregion

            #region TPDO2 UI

            // 0~1 nd. bytes
            t2Byte01Slider.Value = mainDevDataContains.ParkPressureA1;

            // 2~3 rd. bytes
            t2Byte23Slider.Value = mainDevDataContains.VldRealPressureAx1;

            // 4~5 th. bytes
            t2Byte45Tb.Text = "2架空气制动目标值：" + mainDevDataContains.AbTargetValueAx2.ToString() + " kpa";

            // 6~7 th. bytes
            t2Byte67Tb.Text = "3架空气制动目标值：" + mainDevDataContains.AbTargetValueAx3.ToString() + " kpa";
            #endregion

            #region TPDO3 UI

            // 0 st. byte
            t3Byte0Bit0Tb.Text = mainDevDataContains.EmergencyBrakeActiveA1.ToString();
            t3Byte0Bit1Tb.Text = mainDevDataContains.NotZeroSpeed.ToString();
            t3Byte0Bit2Tb.Text = mainDevDataContains.AbActive.ToString();
            t3Byte0Bit4Tb.Text = mainDevDataContains.BCPLowA11.ToString();
            t3Byte0Bit5Tb.Text = mainDevDataContains.ParkBreakRealease.ToString();
            t3Byte0Bit6Tb.Text = mainDevDataContains.AbStatuesA1.ToString();

            // 1 st. byte
            t3Byte1Bit1Tb.Text = mainDevDataContains.SelfTestInt.ToString();
            t3Byte1Bit2Tb.Text = mainDevDataContains.SelfTestActive.ToString();
            t3Byte1Bit3Tb.Text = mainDevDataContains.SelfTestSuccess.ToString();
            t3Byte1Bit4Tb.Text = mainDevDataContains.UnSelfTest24.ToString();
            t3Byte1Bit5Tb.Text = mainDevDataContains.UnSelfTest26.ToString();
            t3Byte1Bit6Tb.Text = mainDevDataContains.GateValveState.ToString();
            t3Byte1Bit0Tb.Text = mainDevDataContains.MassSigValid.ToString();
            t3Byte1Bit7Tb.Text = mainDevDataContains.HardDriveCmd.ToString();

            // 2 nd. byte
            t3Byte2Bit0Tb.Text = mainDevDataContains.HardBrakeCmd.ToString();
            t3Byte2Bit1Tb.Text = mainDevDataContains.HardFastBrakeCmd.ToString();
            t3Byte2Bit2Tb.Text = mainDevDataContains.HardEmergencyDriveMode.ToString();
            t3Byte2Bit3Tb.Text = mainDevDataContains.HardEmergencyDriveCmd.ToString();

            // 3 rd. byte
            t3Byte3Bit0Tb.Text = mainDevDataContains.ValveCanEmergencyActive.ToString();

            // 4 th. byte
            t3Byte4Bit0Tb.Text = mainDevDataContains.NetDriveCmd.ToString();
            t3Byte4Bit1Tb.Text = mainDevDataContains.NetBrakeCmd.ToString();
            t3Byte4Bit2Tb.Text = mainDevDataContains.NetFastBrakeCmd.ToString();
            t3Byte4Bit4Tb.Text = mainDevDataContains.TowingMode.ToString();
            t3Byte4Bit5Tb.Text = mainDevDataContains.HoldBrakeRealease.ToString();
            t3Byte4Bit6Tb.Text = mainDevDataContains.ATOMode1.ToString();
            t3Byte4Bit7Tb.Text = mainDevDataContains.BrakeLevelEnable.ToString();

            // 5 th. byte
            t3Byte5Bit0Tb.Text = mainDevDataContains.SelfTestCmd.ToString();
            t3Byte5Bit1Tb.Text = mainDevDataContains.EdFadeOut.ToString();
            t3Byte5Bit3Tb.Text = mainDevDataContains.TrainBrakeEnable.ToString();
            t3Byte5Bit4Tb.Text = mainDevDataContains.ZeroSpeed.ToString();
            t3Byte5Bit6Tb.Text = mainDevDataContains.EdOffB1.ToString();
            t3Byte5Bit7Tb.Text = mainDevDataContains.EdOffC1.ToString();

            // 6~7 th. bytes
            t3Byte67Slider.Value = mainDevDataContains.MassA1 / 1000.0;
            #endregion

            #region TPDO4 UI

            // 0 th. byte
            t4Byte0Bit0Tb.Text = mainDevDataContains.WheelInputState.ToString();
            t4Byte0Bit1Tb.Text = mainDevDataContains.SoftVersion.ToString();
            t4Byte0Bit2Tb.Text = mainDevDataContains.VCMLifeSig.ToString();
 
            t4Byte0Bit5Tb.Text = mainDevDataContains.SpeedDetection.ToString();
            t4Byte0Bit6Tb.Text = mainDevDataContains.CanBusFail1.ToString();
            t4Byte0Bit7Tb.Text = mainDevDataContains.CanBusFail2.ToString();

            // 1 st. byte
            t4Byte1Bit0Tb.Text = mainDevDataContains.HardDifferent.ToString();
            t4Byte1Bit1Tb.Text = mainDevDataContains.EventHigh.ToString();
            t4Byte1Bit2Tb.Text = mainDevDataContains.EventMid.ToString();
            t4Byte1Bit3Tb.Text = mainDevDataContains.EventLow.ToString();
            t4Byte1Bit4Tb.Text = mainDevDataContains.CanASPEnable.ToString();

            // 2~3 rd. bytes
            t4Byte23Slider.Value = mainDevDataContains.SpeedA1Shaft1;

            // 4~5 th. bytes
            t4Byte45Slider.Value = mainDevDataContains.SpeedA1Shaft2;

            // 6~7 th. bytes
            t4Byte67Tb.Text = "4架空气制动目标值：" + mainDevDataContains.AbTargetValueAx4.ToString() + " kpa";
            #endregion

            #region TPDO5 UI

            // 0~1 st. bytes
            t5Byte01Slider.Value = mainDevDataContains.Bcp1PressureAx1;

            // 2~3 rd. bytes
            t5Byte23Slider.Value = mainDevDataContains.Bcp2PressureAx2;

            // 4~5 th. bytes
            t5Byte45Slider.Value = mainDevDataContains.VldPressureSetupAx1;

            // 6~7 th. bytes
            t5Byte67Tb.Text = "5架空气制动目标值：" + mainDevDataContains.AbTargetValueAx5.ToString() + " kpa";
            #endregion

            #region TPDO6 UI

            // 0~1 st. bytes
            t6Byte01Slider.Value = mainDevDataContains.AirSpring1PressureA1Car1;

            // 2~3 rd. bytes
            t6Byte23Slider.Value = mainDevDataContains.AirSpring2PressureA1Car1;
            
            switch (id)
            {
                case 0:
                    t6Byte4Bit6Tb.Text = mainDevDataContains.BCPLowA.ToString();
                    break;
                case 2:
                    t6Byte4Bit6Tb.Text = mainDevDataContains.BCPLowB.ToString();
                    break;
                case 4:
                    t6Byte4Bit6Tb.Text = mainDevDataContains.BCPLowC.ToString();
                    break;
                default:
                    break;
            }
            

            // 5 th. byte
            t6Byte5Bit0Tb.Text = mainDevDataContains.AbCapacity[id - 1].ToString();
            t6Byte5Bit1Tb.Text = mainDevDataContains.AbRealValue[id - 1].ToString();

            // 6 th. byte
            t6Byte6Bit0Tb.Text = mainDevDataContains.UnixHour.ToString() + " : " + mainDevDataContains.UnixMinute.ToString();
            t6Byte6Bit1Tb.Text = mainDevDataContains.UnixTimeValid.ToString();
            t6Byte6Bit2Tb.Text = mainDevDataContains.SelfTestSuccess.ToString();
            t6Byte6Bit3Tb.Text = mainDevDataContains.SelfTestFail.ToString();
            t6Byte6Bit4Tb.Text = mainDevDataContains.UnSelfTest24.ToString();
            t6Byte6Bit5Tb.Text = mainDevDataContains.UnSelfTest26.ToString();
            t6Byte6Bit6Tb.Text = mainDevDataContains.GateValveState.ToString();

            // 7 th. byte
            t6Byte7Bit0Tb.Text = mainDevDataContains.SpeedShaftEnable1.ToString();
            t6Byte7Bit1Tb.Text = mainDevDataContains.SpeedShaftEnable2.ToString();
            #endregion

            abTargetAx6.Text = "6架空气制动目标值：" +  mainDevDataContains.AbTargetValueAx6.ToString() + " kpa";

        }

        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyDetailWindow_Loaded(object sender, RoutedEventArgs e)
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
        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 窗口拖动函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
            faultListView.ItemsSource = list;
        }

        private void MyDetailWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, nodeName);
        }
    }
}
