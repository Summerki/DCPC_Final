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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using DirectConnectionPredictControl.IO;
using System.Windows.Media.Animation;
using System.Threading;
using DirectConnectionPredictControl.CommenTool;
using System.Diagnostics;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace DirectConnectionPredictControl
{
    public enum FormatType
    {
        REAL_TIME,
        HISTORY
    }

    public enum FormatCommand
    {
        OK,
        WAIT,
        IGNORE
    }

    public enum ConnectType
    {
        ETH,
        CAN
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] recvData;
        private delegate void updateUI(MainDevDataContains mainDevDataContains);
        private Thread updateUIHandler;

        private Thread updateChartWindowHandler;

        private Thread recvThread;
        private Thread recordThread;
        private CanHelper canHelper;

        private CanHelper canHelper_1;

        private ConnectType connectType = ConnectType.CAN;
        private int recordFreq = 16;
        private CanDTO dTO = null;
        private string msg = "";
        private byte[][] oriData;
        private List<uint> idList = new List<uint>();

        //数据组
        private MainDevDataContains container_1;
        private SliverDataContainer container_2;
        private SliverDataContainer container_3;
        private SliverDataContainer container_4;
        private SliverDataContainer container_5;
        private MainDevDataContains container_6;




        //历史数据组
        private HistoryModel history;

        private MainDevDataContains data_1 = new MainDevDataContains();
        private SliverDataContainer data_2 = new SliverDataContainer();
        private SliverDataContainer data_3 = new SliverDataContainer();
        private SliverDataContainer data_4 = new SliverDataContainer();
        private SliverDataContainer data_5 = new SliverDataContainer();
        private MainDevDataContains data_6 = new MainDevDataContains();

        //窗口组
        private DetailWindow detailWindowCar1;
        private SlaveDetailWindow slaveDetailWindowCar2;
        private SlaveDetailWindow slaveDetailWindowCar3;
        private SlaveDetailWindow slaveDetailWindowCar4;
        private SlaveDetailWindow slaveDetailWindowCar5;
        private DetailWindow slaveDetailWindowCar6;
        private RealTimeSpeedChartWindow speedChartWindow;
        private RealTimePressureChartWindow pressureChartWindow;
        private RealTimeOtherWindow otherWindow;
        private OverviewWindow overviewWindow;
        private ChartWindow chartWindow;
        //private SingleChart singleChartWindow;
        //2018-9-23:新增一个防滑数据显示窗口
        private Antiskid_Display antiskid_Display_Window;
        //2018-9-24:新增一个防滑数据设置窗口
        private Antiskid_Setting antiskid_Setting_Window;

        private TChartDisplay tChartDisplay;

        private double index = 0.0;
        
        //测试组
        private Thread testThread;

        private UserDateTime userDateTime;
        public MainWindow()
        {
            InitializeComponent();
            byEthItem.IsChecked = false;
            byCanItem.IsChecked = true;
            //用代码调动StoryBoard
            Storyboard storyBoard = (Storyboard)MyWindow.Resources["open"];
            if (storyBoard != null)
            {
                storyBoard.Begin();
            }
            Init();
            //Test();
            Utils.getAbsoluteHtmlPath(@"./Charts/test.html");
            
        }

        #region 测试用
        /// <summary>
        /// 测试用例
        /// </summary>
        private void Test()
        {
            container_1 = new MainDevDataContains();
            container_2 = new SliverDataContainer();
            container_3 = new SliverDataContainer();
            container_4 = new SliverDataContainer();
            container_5 = new SliverDataContainer();
            container_6 = new MainDevDataContains();
            history = new HistoryModel();
            //Export("123");
            ClearMsg();
            testThread = new Thread(TestHandler);
            recordThread = new Thread(RecordHandler);
            testThread.Start();
            recordThread.Start();
            byte[] helpFTPInit = { 0x05, 0x05, 0x0a, 0x0a, 0x05, 0x05, 0x0a, 0x0a };
            //canHelper.SendByID(helpFTPInit, 0x11);
        }
        #endregion

        private void RecordHandler()// 做文件记录
        {
            FileBuilding building = new FileBuilding();
            while (true)
            {
                Thread.Sleep(100);
                if (dTO != null)
                {
                    building.Record(oriData);
                }
            }
        }

        /// <summary>
        /// 测试线程
        /// 
        /// </summary>
        private void TestHandler()
        {
            Random random = new Random();
            Random listRandom = new Random();
            while (true)
            {
                Thread.Sleep(Utils.timeInterval);
                int speedValue = random.Next(120);
                int accSetup = random.Next(100);
                int air = random.Next(1000);
                int index = random.Next(0, 60);
                // 2018-10-12:增加一个测试的减速度值试一试
                int Jian_Speed_1 = random.Next(10);
                dTO = new CanDTO();
                dTO.Data = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x39 };
                dTO.Id = idList[index] << 21;
                dTO.Time = DateTime.Now;
                FormatData(dTO, FormatType.REAL_TIME, FormatCommand.IGNORE);
                container_1.SpeedA1Shaft1 = speedValue;
                container_1.SpeedA1Shaft2 = speedValue;
                container_1.RefSpeed = accSetup;
                container_1.Bcp1PressureAx1 = 400;
                container_1.Bcp2PressureAx2 = 420;
                container_1.MassA1 = 200;
                container_2.SpeedShaft1 = speedValue;
                container_2.SpeedShaft2 = speedValue;
                container_2.Bcp1Pressure = air / 2;
                container_3.Bcp1Pressure = air / 3;
                container_2.MassValue = air / 2;
                container_2.AbBrakeActive = true;
                container_1.BrakeCmd = true;
                container_3.AccValue1 = -Jian_Speed_1;
                updateUIMethod(container_1);
                updateChartWindowMethod(container_1);
            }
        }
        

        private void Init()
        {
            ClearMsg();
            container_1 = new MainDevDataContains();
            container_2 = new SliverDataContainer();
            container_3 = new SliverDataContainer();
            container_4 = new SliverDataContainer();
            container_5 = new SliverDataContainer();
            container_6 = new MainDevDataContains();
            history = new HistoryModel();
            userDateTime = new UserDateTime()
            {
                Year = 2018,
                Month = 3,
                Day = 10,
                Hour = 0,
                Minute = 0,
                Second = 0
            };
            if (connectType == ConnectType.ETH)
            {

            }
            if (connectType == ConnectType.CAN)
            {
                canHelper = new CanHelper();

                canHelper_1 = new CanHelper();

                recvThread = new Thread(RecvDataAsyncByCan);
                recvThread.IsBackground = true;
                recvThread.Start();
                updateUIHandler = new Thread(UpdateUIHandlerMethod);

                updateChartWindowHandler = new Thread(UpdateChartHandlerMethod);
                updateChartWindowHandler.IsBackground = true;

                updateUIHandler.IsBackground = true;
                recordThread = new Thread(RecordHandler);
                recordThread.IsBackground = true;
                updateUIHandler.Start();
                recordThread.Start();
                updateChartWindowHandler.Start();

            }
            
        }

        /// <summary>
        /// 初始化所有本地CAN消息记录对象
        /// </summary>
        private void ClearMsg()
        {
            oriData = new byte[66][];
            for (uint i = 0; i < 66; i++)
            {
                oriData[i] = new byte[9];
                for (uint j = 0; j < 8; j++)
                {
                    oriData[i][j] = 0x00;
                }
                oriData[i][8] = (byte)' ';
            }
            for (uint i = 0x10; i <= 0x18; i++)
            {
                idList.Add(i);
            }
            for (uint i = 0x20; i <= 0x28; i++)
            {
                idList.Add(i);
            }
            idList.Add(0x31);
            for (uint i = 0x34; i <= 0x38; i++)
            {
                idList.Add(i);
            }
            idList.Add(0x41);
            for (uint i = 0x44; i <= 0x48; i++)
            {
                idList.Add(i);
            }
            idList.Add(0x51);
            for (uint i = 0x54; i <= 0x58; i++)
            {
                idList.Add(i);
            }
            idList.Add(0x61);
            for (uint i = 0x64; i <= 0x68; i++)
            {
                idList.Add(i);
            }
            for (uint i = 0x71; i <= 0x79; i++)
            {
                idList.Add(i);
            }
            for (uint i = 0x81; i <= 0x89; i++)
            {
                idList.Add(i);
            }
            for (uint i = 0xa1; i <= 0xa6; i++)
            {
                idList.Add(i);
            }
        }

        private void UpdateUIHandlerMethod()
        {
            updateUI update = new updateUI(updateUIMethod);
            while (true)
            {
                Thread.Sleep(Utils.timeInterval);
                update.Invoke(container_1);
            }
        }

        private void UpdateChartHandlerMethod()
        {
            updateUI update = new updateUI(updateChartWindowMethod);
            while (true)
            {
                Thread.Sleep(50);
                update.Invoke(container_1);
            }
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
        

        public void OpenFile()
        {
            string fileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.RestoreDirectory = true;
            FileSource sourceType = FileSource.PC;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                history = new HistoryModel();
                double x = 0.0;
                fileName = ofd.FileName;
                if (System.IO.Path.GetFileName(fileName).Length > 15 || System.IO.Path.GetFileName(fileName) == "record.log")
                {
                    sourceType = FileSource.PC;
                    FileBuilding.LINE_LENGTH1 = 620;
                }
                else
                {
                    sourceType = FileSource.TERMINAL;
                    FileBuilding.LINE_LENGTH1 = 616;
                }
                List<byte[]> content = FileBuilding.GetFileContent(fileName);
                List<List<CanDTO>> canList = FileBuilding.GetCanList(content, sourceType);
                history.FileLength = FileBuilding.FileLength;

                
                for (int i = 0; i < canList.Count; i++)
                {
                    history.ListID.Add(i + 1);// 2018-11-23
                    int count = FileBuilding.CAN_MO_NUM;
                    history.Count = canList.Count;
                    history.X.Add(x);
                    x += 0.1;
                    for (int j = 0; j < canList[i].Count; j++)
                    {
                        if (--count == 0)
                        {
                            FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.OK);
                            data_1 = new MainDevDataContains();
                            data_2 = new SliverDataContainer();
                            data_3 = new SliverDataContainer();
                            data_4 = new SliverDataContainer();
                            data_5 = new SliverDataContainer();
                            data_6 = new MainDevDataContains();
                        }
                        else
                        {
                            FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.WAIT);
                        }
                        
                    }
                    index += 0.1;
                }
                HistoryDetail historyDetail = new HistoryDetail();
                historyDetail.SetHistory(history);
                historyDetail.Show();

                //SingleChart historyChart = new SingleChart();

                //historyChart.Show();
                //historyChart.SetHistoryModel(history);
                //historyChart.PaintHistory();

                //OverviewWindowHis his = new OverviewWindowHis(history);
                //his.Show();
            }
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
        /// 最大化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maximun_Click(object sender, RoutedEventArgs e) 
        {
            if (this.WindowState == WindowState.Maximized)
            {

                this.WindowState = WindowState.Normal;
                this.MainDashboard.dashboard.Width = 250;
                this.MainDashboard.dashboard.Height = 250;
                this.MainDashboard.speedtext.FontSize = 12;
                this.MainDashboard.speed.FontSize = 40;
                this.MainDashboard.Kmphtext.FontSize = 12;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 80);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.MainDashboard.dashboard.Width = 350;
                this.MainDashboard.dashboard.Height = 350;
                this.MainDashboard.speedtext.FontSize = 16;
                this.MainDashboard.speed.FontSize = 48;
                this.MainDashboard.Kmphtext.FontSize = 16;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 110);
            }
           
        }

        /// <summary>
        /// 菜单栏-文件点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        //点击关闭按钮执行完退出动画后执行
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
                System.Windows.Controls.ToolBar toolBar = sender as System.Windows.Controls.ToolBar;
                  var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
             if (overflowGrid != null)
              {
                     overflowGrid.Visibility = Visibility.Collapsed;
              }
          var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
          {
                mainPanelBorder.Margin = new Thickness(0);
                   }
        }
        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3 ;
            this.Height = y1 * 4 / 5;
            //Thread recvThread = new Thread(RecvDataAsync);
            //recvThread.Start();
        }

        /// <summary>
        /// 异步方式接收，16毫秒一次
        /// </summary>
        unsafe private void RecvDataAsyncByCan()
        {

            //canHelper.OpenCAN_1();
            //canHelper.InitCAN_1();
            //canHelper.StartCAN_1();
            canHelper.Open();
            canHelper.Init();
            canHelper.Start();
            canHelper.StartCAN_1();

            byte[] helpFTPInit = { 0x05, 0x05, 0x0a, 0x0a, 0x05, 0x05, 0x0a, 0x0a };
            canHelper.SendByID(helpFTPInit, 0x11);
            while (true)
            {
                Thread.Sleep(16);
                List<CanDTO> list = canHelper.Recv();
                if (list.Count <= 0)
                {
                    continue;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    dTO = list[i];
                    FormatData(dTO, FormatType.REAL_TIME, FormatCommand.IGNORE);
                }
                
            }
        }

        private void CheckZero(byte[] data, int index)
        {
            if ((data[index] & 0x80) == 0x80)
            {
                data[index] = 0;
                data[index + 1] = 0;
            }
        }

        /// <summary>
        /// 从CAN id获取下标值
        /// </summary>
        /// <param name="canID"></param>
        /// <returns></returns>
        private int getIndex(uint canID)
        {
            int index = 0;
            index = idList.IndexOf(canID);
            return index;
        }

        /// <summary>
        /// 格式化接收的数据至类中
        /// </summary>
        /// <param name="recvData"></param>
        private void FormatData(CanDTO dTO, FormatType type, FormatCommand command)
        {
            //设置can数据指针
            
            int point = 0;

            byte[] recvData = dTO.Data;
            DateTime recvTime = dTO.Time;

            //判断数据来源
            uint canID = dTO.Id;
            canID = dTO.Id >> 21;

            

            uint canIdHigh = (canID & 0xf0) >> 4;
            uint canIdLow = canID & 0x0f;

            if (type == FormatType.REAL_TIME)
            {
                FormateRealTime(recvData, canIdHigh, canIdLow, FormatType.REAL_TIME, point);
                int location = getIndex(canID);
                if(location < 0)
                {
                    return;
                }
                for (int i = 0; i < 8; i++)
                {
                    oriData[location][i] = recvData[i];
                }
                oriData[location][8] = (byte)' ';
            }
            if (type == FormatType.HISTORY)
            {
                FormateHistory(recvTime, recvData, canIdHigh, canIdLow, FormatType.HISTORY, command);
            }
        }


        private void FormateRealTime(byte[] recvData, uint canIdHigh, uint canIdLow, FormatType type, int point = 0)
        {
            #region 解析CAN数据包中的8个字节，根据CAN ID来决定字段含义
            switch (canIdHigh)
            {
                case 1:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_1.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_1.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_1.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_1.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_1.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_1.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_1.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_1.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_1.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_1.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_1.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_1.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_1.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_1.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_1.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_1.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_1.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_1.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_1.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_1.BrakeLevel = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            #endregion

                            #region byte6~7
                            container_1.TrainBrakeForce = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_1.LifeSig = recvData[point];

                            //container_1.SlipLvl1 = recvData[point + 1] & 0x0f;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_1.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_1.SlipLvl1 = SlipLv1;
                            }
                            //container_1.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_1.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_1.SlipLvl2 = SlipLv2;
                            }
                            container_1.SlipLvl2 >>= 4;
                            container_1.SpeedA1Shaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_1.SpeedA1Shaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;
                            //container_1.AccValue1 = recvData[point + 6] / 10.0;
                            //container_1.AccValue2 = recvData[point + 6] / 10.0;

                            //代表最高位为1
                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_1.AccValue1 = -(temp/10.0);
                            }
                            else
                            {
                                container_1.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_1.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_1.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_1.AbTargetValueAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.AbTargetValueAx2 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.AbTargetValueAx3 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_1.AbTargetValueAx4 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);
                            

                            
                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_1.AbTargetValueAx5 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.AbTargetValueAx6 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_1.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_1.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_1.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.NetFastBrakeCmd = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_1.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_1.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_1.RefSpeed = Utils.PositiveToNegative(recvData[6], recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_1.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.AirSpring1PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.AirSpring2PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_1.ParkPressureA1 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_1.VldRealPressureAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_1.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_1.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_1.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_1.VldPressureSetupAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.MassA1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_1.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_1.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;

                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_1.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);

                            container_1.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.WSPTargetValue_1 = recvData[4];
                            container_1.WSPTargetValue_2 = recvData[5];

                            container_1.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_1.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            //container_1.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion
                        case 8:
                            container_1.SwitchInputChannel1_8 = recvData[0];
                            container_1.DigitalOutputChannel9_16 = recvData[1];
                            container_1.DigitalOutputChannel1_8 = recvData[2];
                            container_1.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_1.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_1.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_1.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_1.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_1.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_1.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_1.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;

                    }
                    #endregion
                    break;

                case 3:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_2.LifeSig = recvData[point];

                            //container_2.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //container_2.SlipLvl2 = recvData[point + 1] & 0xf0;                           
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_2.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_2.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_2.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_2.SlipLvl2 = SlipLv2;
                            }
                            container_2.SlipLvl2 >>= 4;
                            container_2.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_2.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //container_2.AccValue1 = recvData[point + 6] / 10.0;
                            //container_2.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_2.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_2.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_2.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_2.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_2.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_2.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_2.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_2.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_2.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            container_2.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            container_2.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_2.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_2.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_2.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_2.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_2.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            container_2.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_2.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_2.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_2.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_2.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_2.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_2.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_2.WSPTargetValue_1 = recvData[4];
                            container_2.WSPTargetValue_2 = recvData[5];

                            container_2.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_2.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            container_2.SwitchInputChannel1_8 = recvData[0];
                            container_2.DigitalOutputChannel9_16 = recvData[1];
                            container_2.DigitalOutputChannel1_8 = recvData[2];
                            container_2.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_2.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_2.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_2.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_2.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_2.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_2.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_2.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_2.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_2.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_2.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_2.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_2.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 4:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_3.LifeSig = recvData[point];

                            //container_3.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //container_3.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_3.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_3.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_3.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_3.SlipLvl2 = SlipLv2;
                            }
                            container_3.SlipLvl2 >>= 4;
                            container_3.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_3.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //container_3.AccValue1 = recvData[point + 6] / 10.0;
                            //container_3.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_3.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_3.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_3.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_3.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_3.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_3.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_3.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_3.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_3.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            container_3.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            container_3.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_3.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_3.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_3.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_3.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_3.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            container_3.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_3.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_3.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_3.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_3.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_3.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_3.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_3.WSPTargetValue_1 = recvData[4];
                            container_3.WSPTargetValue_2 = recvData[5];

                            container_3.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_3.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            container_3.SwitchInputChannel1_8 = recvData[0];
                            container_3.DigitalOutputChannel9_16 = recvData[1];
                            container_3.DigitalOutputChannel1_8 = recvData[2];
                            container_3.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_3.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_3.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_3.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_3.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_3.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_3.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_3.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_3.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_3.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_3.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_3.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_3.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 5:
                    #region 从设备4车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_4.LifeSig = recvData[point];

                            //container_4.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //container_4.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_4.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_4.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_4.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_4.SlipLvl2 = SlipLv2;
                            }
                            container_4.SlipLvl2 >>= 4;
                            container_4.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_4.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //container_4.AccValue1 = recvData[point + 6] / 10.0;
                            //container_4.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_4.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_4.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_4.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_4.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_4.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_4.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_4.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_4.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_4.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            container_4.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            container_4.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_4.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.MainPipeSensorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_4.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_4.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_4.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_4.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            container_4.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_4.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_4.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_4.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_4.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_4.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_4.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_4.WSPTargetValue_1 = recvData[4];
                            container_4.WSPTargetValue_2 = recvData[5];

                            container_4.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_4.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            container_4.SwitchInputChannel1_8 = recvData[0];
                            container_4.DigitalOutputChannel9_16 = recvData[1];
                            container_4.DigitalOutputChannel1_8 = recvData[2];
                            container_4.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_4.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_4.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_4.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_4.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_4.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_4.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_4.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_4.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_4.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_4.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_4.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_4.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 6:
                    #region 从设备5车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_5.LifeSig = recvData[point];

                            //container_5.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //container_5.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_5.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_5.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_5.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_5.SlipLvl2 = SlipLv2;
                            }
                            container_5.SlipLvl2 >>= 4;
                            container_5.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_5.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //container_5.AccValue1 = recvData[point + 6] / 10.0;
                            //container_5.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_5.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_5.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_5.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_5.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_5.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_5.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_5.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_5.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_5.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            container_5.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            container_5.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_5.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_5.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_5.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_5.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_5.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            container_5.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_5.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_5.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_5.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_5.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_5.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_5.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_5.WSPTargetValue_1 = recvData[4];
                            container_5.WSPTargetValue_2 = recvData[5];

                            container_5.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_5.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            container_5.SwitchInputChannel1_8 = recvData[0];
                            container_5.DigitalOutputChannel9_16 = recvData[1];
                            container_5.DigitalOutputChannel1_8 = recvData[2];
                            container_5.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_5.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_5.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_5.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_5.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_5.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_5.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_5.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_5.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_5.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_5.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_5.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_5.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 2:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_6.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_6.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_6.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_6.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_6.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_6.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_6.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_6.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_6.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_6.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_6.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_6.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_6.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_6.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_6.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_6.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_6.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_6.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_6.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_6.BrakeLevel = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            #endregion

                            #region byte6~7
                            container_6.TrainBrakeForce = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_6.LifeSig = recvData[point];

                            //container_6.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //container_6.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                container_6.SlipLvl1 = -temp;
                            }
                            else
                            {
                                container_6.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                container_6.SlipLvl2 = -temp;
                            }
                            else
                            {
                                container_6.SlipLvl2 = SlipLv2;
                            }
                            container_6.SlipLvl2 >>= 4;
                            container_6.SpeedA1Shaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            container_6.SpeedA1Shaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //container_6.AccValue1 = recvData[point + 6] / 10.0;
                            //container_6.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_6.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_6.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_6.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_6.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_6.AbTargetValueAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_6.AbTargetValueAx2 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.AbTargetValueAx3 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_6.AbTargetValueAx4 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_6.AbTargetValueAx5 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_6.AbTargetValueAx6 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_6.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_6.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_6.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.NetFastBrakeCmd = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_6.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_6.RefSpeed = Utils.PositiveToNegative(recvData[6], recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_6.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_6.AirSpring1PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.AirSpring2PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            container_6.ParkPressureA1 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_6.VldRealPressureAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_6.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_6.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_6.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_6.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;

                            
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_6.VldPressureSetupAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_6.MassA1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_6.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_6.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_6.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);

                            container_6.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.WSPTargetValue_1 = recvData[4];
                            container_6.WSPTargetValue_2 = recvData[5];

                            container_6.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_6.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            //container_6.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion
                        case 8:
                            container_6.SwitchInputChannel1_8 = recvData[0];
                            container_6.DigitalOutputChannel9_16 = recvData[1];
                            container_6.DigitalOutputChannel1_8 = recvData[2];
                            container_6.OutputOverFlowProtectChannel9_16 = recvData[3];
                            container_6.OutputOverFlowProtectChannel1_8 = recvData[4];

                            container_6.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_6.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_6.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_6.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_6.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_6.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 7:
                    #region 1车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_1.VCMLifeSig = recvData[1];
                            container_1.DcuLifeSig[0] = recvData[2];
                            container_1.DcuLifeSig[1] = recvData[3];
                            

                            container_1.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbFault[0] = (recvData[4] & 0x08) == 0x08 ? true : false;

                            container_1.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.DcuEbFault[1] = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_1.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbFault[3] = (recvData[5] & 0x08) == 0x08 ? true : false;

                            container_1.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_1.DcuEbFault[3] = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_1.DcuLifeSig[2] = recvData[6];
                            container_1.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_1.DcuEbRealValue[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.DcuMax[0] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.DcuEbRealValue[1] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.DcuMax[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_1.DcuEbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.DcuMax[2] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.DcuEbRealValue[3] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.DcuMax[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_1.AbCapacity[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.AbCapacity[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.AbCapacity[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.AbCapacity[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_1.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_1.AbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.AbRealValue[3] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.AbRealValue[4] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.AbRealValue[5] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_1.DcuVolta[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.DcuVolta[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.DcuVolta[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.DcuVolta[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_1.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_1.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_1.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_1.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_1.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_1.UnixHour = recvData[2] * 256 + recvData[3];
                            container_1.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_1.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;
                            
                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_1.Tc1 = recvData[0] * 256 + recvData[1];
                            container_1.Mp1 = recvData[2] * 256 + recvData[3];
                            container_1.M1 = recvData[4] * 256 + recvData[5];
                            container_1.Tc1Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.Mp1Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.M1Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 8:
                    #region 2车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_6.VCMLifeSig = recvData[1];
                            container_6.DcuLifeSig[0] = recvData[2];
                            container_6.DcuLifeSig[1] = recvData[3];

                            container_6.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbFault[0] = (recvData[4] & 0x08) == 0x08 ? true : false;

                            container_6.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.DcuEbFault[1] = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_6.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbFault[2] = (recvData[5] & 0x08) == 0x08 ? true : false;

                            container_6.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_6.DcuEbFault[3] = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_6.DcuLifeSig[2] = recvData[6];
                            container_6.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_6.DcuEbRealValue[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.DcuMax[0] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.DcuEbRealValue[1] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.DcuMax[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_6.DcuEbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.DcuMax[2] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.DcuEbRealValue[3] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.DcuMax[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_6.AbCapacity[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.AbCapacity[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.AbCapacity[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.AbCapacity[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_6.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_6.AbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.AbRealValue[3] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.AbRealValue[4] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.AbRealValue[5] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_6.DcuVolta[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.DcuVolta[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.DcuVolta[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.DcuVolta[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_6.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_6.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_6.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_6.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_6.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_6.UnixHour = recvData[2] * 256 + recvData[3];
                            container_6.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_6.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_6.Tc1 = recvData[0] * 256 + recvData[1];
                            container_6.Mp1 = recvData[2] * 256 + recvData[3];
                            container_6.M1 = recvData[4] * 256 + recvData[5];
                            container_6.Tc2Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.Mp2Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.M2Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 0x0a:
                    switch (canIdLow)
                    {
                        case 1:
                            container_1.WheelSize = recvData[0] * 256 + recvData[1];
                            container_1.ConfirmDownload = recvData[4] == 0xAA;
                            container_1.CPUAddr = recvData[5];
                            container_1.SoftwareVersionCPU = recvData[7];
                            container_1.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorMain(ref container_1, recvData);
                            break;
                        case 4:
                            container_3.WheelSize = recvData[0] * 256 + recvData[1];
                            container_3.CPUAddr = recvData[5];
                            container_3.SoftwareVersionCPU = recvData[7];
                            container_3.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref container_3, recvData);
                            break;
                        case 5:
                            container_4.WheelSize = recvData[0] * 256 + recvData[1];
                            container_4.CPUAddr = recvData[5];
                            container_4.SoftwareVersionCPU = recvData[7];
                            container_4.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref container_4, recvData);
                            break;
                        case 6:
                            container_5.WheelSize = recvData[0] * 256 + recvData[1];
                            container_5.CPUAddr = recvData[5];
                            container_5.SoftwareVersionCPU = recvData[7];
                            container_5.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref container_5, recvData);
                            break;
                        case 2:
                            container_6.WheelSize = recvData[0] * 256 + recvData[1];
                            container_6.CPUAddr = recvData[5];
                            container_6.SoftwareVersionCPU = recvData[7];
                            container_6.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorMain(ref container_6, recvData);
                            break;
                        case 3:
                            container_2.WheelSize = recvData[0] * 256 + recvData[1];
                            container_2.ConfirmDownload = recvData[4] == 0xAA;
                            container_2.CPUAddr = recvData[5];
                            container_2.SoftwareVersionCPU = recvData[7];
                            container_2.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref container_2, recvData);
                            break;
                        default:
                            break;
                    }
                    break;

            }
            #endregion
        }

        private void FormateHistory(DateTime recvTime, byte[] recvData, uint canIdHigh, uint canIdLow, FormatType type, FormatCommand command, int point = 0)
        {

            #region 解析CAN数据包中的8个字节，根据CAN ID来决定字段含义
            switch (canIdHigh)
            {
                case 1:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                data_1.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                data_1.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                data_1.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            data_1.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            data_1.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            data_1.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            data_1.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            data_1.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            data_1.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            data_1.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            data_1.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            data_1.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            data_1.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            data_1.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            data_1.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            data_1.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            data_1.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_1.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_1.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_1.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_1.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            data_1.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            data_1.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            data_1.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            data_1.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            data_1.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            data_1.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            data_1.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            data_1.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            data_1.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            data_1.BrakeLevel = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            #endregion

                            #region byte6~7
                            data_1.TrainBrakeForce = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            data_1.LifeSig = recvData[point];

                            //data_1.SlipLvl1 = recvData[point + 1] & 0x0f;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_1.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_1.SlipLvl1 = SlipLv1;
                            }
                            //data_1.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_1.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_1.SlipLvl2 = SlipLv2;
                            }
                            data_1.SlipLvl2 >>= 4;
                            data_1.SpeedA1Shaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_1.SpeedA1Shaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;
                            //data_1.AccValue1 = recvData[point + 6] / 10.0;
                            //data_1.AccValue2 = recvData[point + 6] / 10.0;

                            //代表最高位为1
                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_1.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_1.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_1.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_1.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            data_1.AbTargetValueAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_1.AbTargetValueAx2 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_1.AbTargetValueAx3 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_1.AbTargetValueAx4 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            data_1.AbTargetValueAx5 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_1.AbTargetValueAx6 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_1.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_1.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_1.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_1.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_1.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_1.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_1.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_1.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_1.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            data_1.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            data_1.NetFastBrakeCmd = (recvData[4] & 0x04) == 0x04 ? true : false;
                            data_1.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            data_1.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            data_1.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            data_1.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            data_1.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            data_1.RefSpeed = Utils.PositiveToNegative(recvData[6], recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            data_1.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_1.AirSpring1PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_1.AirSpring2PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_1.ParkPressureA1 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            data_1.VldRealPressureAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_1.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_1.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_1.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_1.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_1.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_1.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_1.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_1.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_1.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_1.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_1.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_1.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_1.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_1.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            data_1.VldPressureSetupAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_1.MassA1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_1.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_1.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_1.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_1.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_1.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_1.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_1.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_1.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_1.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_1.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_1.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_1.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_1.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_1.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;

                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            data_1.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);

                            data_1.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_1.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_1.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_1.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_1.WSPTargetValue_1 = recvData[4];
                            data_1.WSPTargetValue_2 = recvData[5];

                            data_1.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_1.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_1.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_1.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_1.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_1.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_1.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_1.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_1.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_1.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_1.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            //data_1.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_1.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_1.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_1.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion
                        case 8:
                            data_1.SwitchInputChannel1_8 = recvData[0];
                            data_1.DigitalOutputChannel9_16 = recvData[1];
                            data_1.DigitalOutputChannel1_8 = recvData[2];
                            data_1.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_1.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_1.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_1.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_1.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_1.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_1.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_1.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_1.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_1.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_1.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_1.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_1.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_1.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_1.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_1.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_1.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_1.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_1.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_1.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_1.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;

                    }
                    #endregion
                    break;

                case 3:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            data_2.LifeSig = recvData[point];

                            //data_2.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //data_2.SlipLvl2 = recvData[point + 1] & 0xf0;                           
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_2.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_2.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_2.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_2.SlipLvl2 = SlipLv2;
                            }
                            data_2.SlipLvl2 >>= 4;
                            data_2.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_2.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //data_2.AccValue1 = recvData[point + 6] / 10.0;
                            //data_2.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_2.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_2.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_2.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_2.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            data_2.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_2.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_2.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_2.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            data_2.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            data_2.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            data_2.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_2.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_2.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_2.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_2.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_2.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_2.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_2.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_2.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_2.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_2.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_2.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            data_2.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_2.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            data_2.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_2.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_2.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_2.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_2.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_2.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            data_2.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_2.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_2.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_2.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_2.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_2.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_2.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_2.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            data_2.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_2.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_2.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_2.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_2.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_2.WSPTargetValue_1 = recvData[4];
                            data_2.WSPTargetValue_2 = recvData[5];

                            data_2.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_2.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_2.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_2.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_2.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_2.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_2.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_2.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_2.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_2.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_2.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_2.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_2.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_2.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            data_2.SwitchInputChannel1_8 = recvData[0];
                            data_2.DigitalOutputChannel9_16 = recvData[1];
                            data_2.DigitalOutputChannel1_8 = recvData[2];
                            data_2.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_2.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_2.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_2.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_2.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_2.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_2.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_2.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_2.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_2.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_2.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_2.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_2.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_2.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_2.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_2.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_2.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_2.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_2.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_2.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_2.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 4:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            data_3.LifeSig = recvData[point];

                            //data_3.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //data_3.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_3.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_3.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_3.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_3.SlipLvl2 = SlipLv2;
                            }
                            data_3.SlipLvl2 >>= 4;
                            data_3.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_3.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //data_3.AccValue1 = recvData[point + 6] / 10.0;
                            //data_3.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_3.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_3.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_3.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_3.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            data_3.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_3.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_3.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_3.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            data_3.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            data_3.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            data_3.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_3.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_3.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_3.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_3.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_3.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_3.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_3.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_3.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_3.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_3.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_3.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_3.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            data_3.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_3.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            data_3.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_3.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_3.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_3.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_3.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_3.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            data_3.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_3.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_3.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_3.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_3.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_3.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_3.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_3.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            data_3.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_3.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_3.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_3.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_3.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_3.WSPTargetValue_1 = recvData[4];
                            data_3.WSPTargetValue_2 = recvData[5];

                            data_3.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_3.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_3.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_3.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_3.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_3.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_3.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_3.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_3.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_3.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_3.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_3.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_3.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_3.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            data_3.SwitchInputChannel1_8 = recvData[0];
                            data_3.DigitalOutputChannel9_16 = recvData[1];
                            data_3.DigitalOutputChannel1_8 = recvData[2];
                            data_3.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_3.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_3.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_3.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_3.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_3.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_3.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_3.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_3.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_3.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_3.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_3.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_3.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_3.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_3.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_3.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_3.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_3.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_3.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_3.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_3.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 5:
                    #region 从设备4车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            data_4.LifeSig = recvData[point];

                            //data_4.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //data_4.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_4.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_4.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_4.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_4.SlipLvl2 = SlipLv2;
                            }
                            data_4.SlipLvl2 >>= 4;
                            data_4.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_4.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //data_4.AccValue1 = recvData[point + 6] / 10.0;
                            //data_4.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_4.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_4.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_4.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_4.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            data_4.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_4.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_4.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_4.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            data_4.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            data_4.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            data_4.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_4.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_4.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_4.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_4.MainPipeSensorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_4.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_4.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_4.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_4.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_4.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_4.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_4.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_4.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            data_4.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_4.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            data_4.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_4.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_4.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_4.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_4.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_4.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            data_4.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_4.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_4.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_4.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_4.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_4.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_4.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_4.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            data_4.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_4.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_4.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_4.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_4.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_4.WSPTargetValue_1 = recvData[4];
                            data_4.WSPTargetValue_2 = recvData[5];

                            data_4.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_4.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_4.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_4.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_4.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_4.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_4.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_4.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_4.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_4.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_4.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_4.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_4.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_4.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            data_4.SwitchInputChannel1_8 = recvData[0];
                            data_4.DigitalOutputChannel9_16 = recvData[1];
                            data_4.DigitalOutputChannel1_8 = recvData[2];
                            data_4.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_4.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_4.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_4.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_4.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_4.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_4.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_4.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_4.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_4.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_4.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_4.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_4.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_4.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_4.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_4.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_4.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_4.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_4.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_4.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_4.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 6:
                    #region 从设备5车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            data_5.LifeSig = recvData[point];

                            //data_5.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //data_5.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_5.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_5.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_5.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_5.SlipLvl2 = SlipLv2;
                            }
                            data_5.SlipLvl2 >>= 4;
                            data_5.SpeedShaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_5.SpeedShaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //data_5.AccValue1 = recvData[point + 6] / 10.0;
                            //data_5.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_5.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_5.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_5.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_5.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            data_5.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_5.AirSpringPressure1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_5.AirSpringPressure2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_5.ParkPressure = Utils.PositiveToNegative(recvData[6], recvData[7]);

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            data_5.VldRealPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            CheckZero(recvData, point + 2);
                            data_5.Bcp1Pressure = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            CheckZero(recvData, point + 4);
                            data_5.Bcp2Pressure = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_5.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_5.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_5.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_5.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_5.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_5.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_5.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_5.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_5.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_5.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_5.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_5.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            data_5.VldSetupPressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_5.MassValue = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);

                            data_5.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_5.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_5.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_5.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_5.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_5.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            data_5.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_5.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_5.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_5.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_5.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_5.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_5.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_5.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            data_5.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_5.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_5.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_5.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_5.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_5.WSPTargetValue_1 = recvData[4];
                            data_5.WSPTargetValue_2 = recvData[5];

                            data_5.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_5.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_5.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_5.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_5.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_5.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_5.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_5.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_5.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_5.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_5.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_5.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_5.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_5.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                        #endregion
                        case 8:
                            data_5.SwitchInputChannel1_8 = recvData[0];
                            data_5.DigitalOutputChannel9_16 = recvData[1];
                            data_5.DigitalOutputChannel1_8 = recvData[2];
                            data_5.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_5.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_5.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_5.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_5.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_5.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_5.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_5.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_5.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_5.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_5.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_5.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_5.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_5.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_5.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_5.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_5.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_5.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_5.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_5.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_5.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 2:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                data_6.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                data_6.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                data_6.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            data_6.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            data_6.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            data_6.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            data_6.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            data_6.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            data_6.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            data_6.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            data_6.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            data_6.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            data_6.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            data_6.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            data_6.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            data_6.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            data_6.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_6.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_6.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_6.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_6.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            data_6.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            data_6.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            data_6.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            data_6.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            data_6.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            data_6.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            data_6.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            data_6.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            data_6.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            data_6.BrakeLevel = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            #endregion

                            #region byte6~7
                            data_6.TrainBrakeForce = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            data_6.LifeSig = recvData[point];

                            //data_6.SlipLvl1 = recvData[point + 1] & 0x0f;
                            //data_6.SlipLvl2 = recvData[point + 1] & 0xf0;
                            int SlipLv1 = recvData[point + 1] & 0x0f;
                            if ((SlipLv1 & 0x08) == 0x08)// 低四位的最高位为1的情况，即为负数的情况下
                            {
                                int temp = (SlipLv1 & 0x07);// 先将低四位的最高位置0
                                data_6.SlipLvl1 = -temp;
                            }
                            else
                            {
                                data_6.SlipLvl1 = SlipLv1;
                            }
                            int SlipLv2 = recvData[point + 1] & 0xf0;
                            if ((SlipLv2 & 0x80) == 0x80)// 高四位的最高位是1（负数）
                            {
                                int temp = (SlipLv2 & 0x70);
                                data_6.SlipLvl2 = -temp;
                            }
                            else
                            {
                                data_6.SlipLvl2 = SlipLv2;
                            }
                            data_6.SlipLvl2 >>= 4;
                            data_6.SpeedA1Shaft1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]) / 10.0;
                            data_6.SpeedA1Shaft2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]) / 10.0;

                            //data_6.AccValue1 = recvData[point + 6] / 10.0;
                            //data_6.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                data_6.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                data_6.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                data_6.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                data_6.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            data_6.AbTargetValueAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_6.AbTargetValueAx2 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_6.AbTargetValueAx3 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_6.AbTargetValueAx4 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            data_6.AbTargetValueAx5 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_6.AbTargetValueAx6 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_6.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_6.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_6.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_6.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_6.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_6.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_6.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_6.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_6.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            data_6.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            data_6.NetFastBrakeCmd = (recvData[4] & 0x40) == 0x40 ? true : false;
                            data_6.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            data_6.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            data_6.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            data_6.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            data_6.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            data_6.RefSpeed = Utils.PositiveToNegative(recvData[6], recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            data_6.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_6.AirSpring1PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_6.AirSpring2PressureA1Car1 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);
                            data_6.ParkPressureA1 = Utils.PositiveToNegative(recvData[point + 6], recvData[point + 7]);
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            data_6.VldRealPressureAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_6.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_6.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            data_6.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_6.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_6.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_6.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_6.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_6.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_6.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_6.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            data_6.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_6.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_6.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_6.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;


                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            data_6.VldPressureSetupAx1 = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            data_6.MassA1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            data_6.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_6.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_6.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_6.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_6.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_6.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_6.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_6.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_6.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_6.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            data_6.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_6.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_6.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_6.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            data_6.SelfTestSetup = Utils.PositiveToNegative(recvData[0], recvData[1]);

                            data_6.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            data_6.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            data_6.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            data_6.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            data_6.WSPTargetValue_1 = recvData[4];
                            data_6.WSPTargetValue_2 = recvData[5];

                            data_6.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_6.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_6.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_6.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            data_6.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_6.ConnectValveControl = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_6.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_6.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_6.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_6.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_6.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            //data_6.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            data_6.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            data_6.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            data_6.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion
                        case 8:
                            data_6.SwitchInputChannel1_8 = recvData[0];
                            data_6.DigitalOutputChannel9_16 = recvData[1];
                            data_6.DigitalOutputChannel1_8 = recvData[2];
                            data_6.OutputOverFlowProtectChannel9_16 = recvData[3];
                            data_6.OutputOverFlowProtectChannel1_8 = recvData[4];

                            data_6.exhaustFault_1 = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_6.keepPressureFault_1 = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_6.chargingFault_1 = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_6.connectionValveAirFillingFailure = (recvData[5] & 0x08) == 0x08 ? true : false;
                            data_6.exhuastFault_2 = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_6.keepPressureFault_2 = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_6.chargingFault_2 = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_6.connectionValveVentilationFailure = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_6.VLDChargingFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_6.VLDExhuastFault = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_6.VLDKeepPressureFault = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_6.WSPExhuastFault_1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            data_6.WSPChargingFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            data_6.WSPExhuastFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            data_6.WSPChargingFault_2 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            data_6.WSPContinueExaustAirTimeOutFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            data_6.WSPContinuousExhaustFailure_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            data_6.WSPContinueKeepPressureTimeOutFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            data_6.WSPContinueKeepPressureTimeOutFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                    }
                    #endregion
                    break;

                case 7:
                    #region 1车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            data_1.VCMLifeSig = recvData[1];
                            data_1.DcuLifeSig[0] = recvData[2];
                            data_1.DcuLifeSig[1] = recvData[3];


                            data_1.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            data_1.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            data_1.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            data_1.DcuEbFault[0] = (recvData[4] & 0x08) == 0x08 ? true : false;

                            data_1.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            data_1.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            data_1.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            data_1.DcuEbFault[1] = (recvData[4] & 0x80) == 0x80 ? true : false;

                            data_1.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_1.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_1.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_1.DcuEbFault[3] = (recvData[5] & 0x08) == 0x08 ? true : false;

                            data_1.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_1.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_1.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_1.DcuEbFault[3] = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_1.DcuLifeSig[2] = recvData[6];
                            data_1.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            data_1.DcuEbRealValue[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.DcuMax[0] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.DcuEbRealValue[1] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.DcuMax[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            data_1.DcuEbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.DcuMax[2] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.DcuEbRealValue[3] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.DcuMax[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            data_1.AbCapacity[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.AbCapacity[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.AbCapacity[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.AbCapacity[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            data_1.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            data_1.AbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.AbRealValue[3] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.AbRealValue[4] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.AbRealValue[5] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            data_1.DcuVolta[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_1.DcuVolta[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_1.DcuVolta[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_1.DcuVolta[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            data_1.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            data_1.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            data_1.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            data_1.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            data_1.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            data_1.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            data_1.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            data_1.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            data_1.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            data_1.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            data_1.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            data_1.UnixHour = recvData[2] * 256 + recvData[3];
                            data_1.UnixMinute = recvData[4] * 256 + recvData[5];
                            data_1.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            data_1.Tc1 = recvData[0] * 256 + recvData[1];
                            data_1.Mp1 = recvData[2] * 256 + recvData[3];
                            data_1.M1 = recvData[4] * 256 + recvData[5];
                            data_1.Tc1Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_1.Mp1Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_1.M1Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_1.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 8:
                    #region 2车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            data_6.VCMLifeSig = recvData[1];
                            data_6.DcuLifeSig[0] = recvData[2];
                            data_6.DcuLifeSig[1] = recvData[3];

                            data_6.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            data_6.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            data_6.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            data_6.DcuEbFault[0] = (recvData[4] & 0x08) == 0x08 ? true : false;

                            data_6.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            data_6.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            data_6.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            data_6.DcuEbFault[1] = (recvData[4] & 0x80) == 0x80 ? true : false;

                            data_6.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            data_6.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            data_6.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            data_6.DcuEbFault[2] = (recvData[5] & 0x08) == 0x08 ? true : false;

                            data_6.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            data_6.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            data_6.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;
                            data_6.DcuEbFault[3] = (recvData[5] & 0x80) == 0x80 ? true : false;

                            data_6.DcuLifeSig[2] = recvData[6];
                            data_6.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            data_6.DcuEbRealValue[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.DcuMax[0] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.DcuEbRealValue[1] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.DcuMax[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            data_6.DcuEbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.DcuMax[2] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.DcuEbRealValue[3] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.DcuMax[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            data_6.AbCapacity[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.AbCapacity[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.AbCapacity[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.AbCapacity[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            data_6.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            data_6.AbRealValue[2] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.AbRealValue[3] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.AbRealValue[4] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.AbRealValue[5] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            data_6.DcuVolta[0] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            data_6.DcuVolta[1] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            data_6.DcuVolta[2] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            data_6.DcuVolta[3] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            data_6.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            data_6.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            data_6.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            data_6.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            data_6.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            data_6.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            data_6.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            data_6.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            data_6.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            data_6.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            data_6.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            data_6.UnixHour = recvData[2] * 256 + recvData[3];
                            data_6.UnixMinute = recvData[4] * 256 + recvData[5];
                            data_6.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            data_6.Tc1 = recvData[0] * 256 + recvData[1];
                            data_6.Mp1 = recvData[2] * 256 + recvData[3];
                            data_6.M1 = recvData[4] * 256 + recvData[5];
                            data_6.Tc2Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            data_6.Mp2Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            data_6.M2Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            data_6.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 0x0a:
                    switch (canIdLow)
                    {
                        case 1:
                            data_1.WheelSize = recvData[0] * 256 + recvData[1];
                            data_1.ConfirmDownload = recvData[4] == 0xAA;
                            data_1.CPUAddr = recvData[5];
                            data_1.SoftwareVersionCPU = recvData[7];
                            data_1.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorMain(ref data_1, recvData);
                            break;
                        case 4:
                            data_3.WheelSize = recvData[0] * 256 + recvData[1];
                            data_3.CPUAddr = recvData[5];
                            data_3.SoftwareVersionCPU = recvData[7];
                            data_3.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref data_3, recvData);
                            break;
                        case 5:
                            data_4.WheelSize = recvData[0] * 256 + recvData[1];
                            data_4.CPUAddr = recvData[5];
                            data_4.SoftwareVersionCPU = recvData[7];
                            data_4.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref data_4, recvData);
                            break;
                        case 6:
                            data_5.WheelSize = recvData[0] * 256 + recvData[1];
                            data_5.CPUAddr = recvData[5];
                            data_5.SoftwareVersionCPU = recvData[7];
                            data_5.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref data_5, recvData);
                            break;
                        case 2:
                            data_6.WheelSize = recvData[0] * 256 + recvData[1];
                            data_6.CPUAddr = recvData[5];
                            data_6.SoftwareVersionCPU = recvData[7];
                            data_6.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorMain(ref data_6, recvData);
                            break;
                        case 3:
                            data_2.WheelSize = recvData[0] * 256 + recvData[1];
                            data_2.ConfirmDownload = recvData[4] == 0xAA;
                            data_2.CPUAddr = recvData[5];
                            data_2.SoftwareVersionCPU = recvData[7];
                            data_2.SoftwareVersionEP = recvData[6];

                            Utils.speedSensorErrorSliver(ref data_2, recvData);
                            break;
                        default:
                            break;
                    }
                    break;

            }
            #endregion

            if (type == FormatType.HISTORY && command == FormatCommand.OK)
            {
                // datatime
                data_1.dateTime =recvTime;

                history.Containers_1.Add(data_1);
                history.Containers_2.Add(data_2);
                history.Containers_3.Add(data_3);
                history.Containers_4.Add(data_4);
                history.Containers_5.Add(data_5);
                history.Containers_6.Add(data_6);

           
            }
        }


        #region 更新所有已实例化的窗口的UI
        /// <summary>
        /// 更新主UI
        /// </summary>
        /// <param name="mainDevDataContains">需要向全体窗口通知的数据DTO</param>
        private void updateUIMethod(MainDevDataContains mainDevDataContains)
        {
            //MainDashboard.slider.Value = (mainDevDataContainers.SpeedA1Shaft1 + mainDevDataContainers.SpeedA1Shaft2) / 2;
            this.Dispatcher.Invoke(new Action(() => {
                MainDashboard.slider.Value = (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2;
            }));
            if(detailWindowCar1 != null)
            {
                detailWindowCar1.UpdateData(container_1);
            }
            if(slaveDetailWindowCar2 != null)
            {
                slaveDetailWindowCar2.UpdateData(container_2);
            }
            if (slaveDetailWindowCar3 != null)
            {
                slaveDetailWindowCar3.UpdateData(container_3);
            }
            if (slaveDetailWindowCar4 != null)
            {
                slaveDetailWindowCar4.UpdateData(container_4);
            }
            if (slaveDetailWindowCar5 != null)
            {
                slaveDetailWindowCar5.UpdateData(container_5);
            }
            if (slaveDetailWindowCar6 != null)
            {
                slaveDetailWindowCar6.UpdateData(container_6);
            }
            if (speedChartWindow != null)
            {
                speedChartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (pressureChartWindow != null)
            {
                pressureChartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (otherWindow != null)
            {
                otherWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (overviewWindow != null)
            {
                overviewWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            //if (chartWindow != null)
            //{
            //    chartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            //}
            //if (antiskid_Display_Window != null)
            //{
            //    antiskid_Display_Window.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            //}

            if (antiskid_Setting_Window != null)
            {
                antiskid_Setting_Window.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
        }

        /// <summary>
        /// 更新chart图表方法
        /// </summary>
        private void updateChartWindowMethod(MainDevDataContains mainDevDataContains)
        {
            //this.Dispatcher.Invoke(new Action(() => {
            //    MainDashboard.slider.Value = (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2;
            //}));
            if (antiskid_Display_Window != null)
            {
                antiskid_Display_Window.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (chartWindow != null)
            {
                chartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if(tChartDisplay != null)
            {
                tChartDisplay.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
        }
        #endregion

        #region 按键事件处理器，根据事件发出控件的 Name 属性来决定动作
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(trainHeaderFirstBtn))
            {
                if (detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(container_1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(trainHeaderSecondBtn))
            {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(container_2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(trainMiddleFirstBtn))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(container_3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(trainMiddleSecondBtn))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(container_4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(trainTailFirstBtn))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(container_5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(trainTailSecondBtn))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(container_6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(car1View))
            {
                if (detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(container_1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(car2View))
            {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(container_2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(car3View))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(container_3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(car4View))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(container_4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(car5View))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(container_5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(car6View))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(container_6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(nodeViewItem))
            {
                NodeWindow nodeWindow = new NodeWindow(container_1.LifeSig, container_2.LifeSig, container_3.LifeSig, container_4.LifeSig, container_5.LifeSig, container_6.LifeSig);
                nodeWindow.Show();
            }
            else if (sender.Equals(wheelDiaItem))
            {
                ParameterSetWindow parameterSetWindow = new ParameterSetWindow();
                parameterSetWindow.ShowDialog();
            }
            else if (sender.Equals(uploadItem))
            {
                DownloadExe();
            }
            else if (sender.Equals(closeBtn))
            {
                CloseDevice();
                App.Current.Shutdown();
            }
            else if (sender.Equals(showSpeedChartItem))
            {
                //显示实时曲线窗体
                if (speedChartWindow == null)
                {
                    speedChartWindow = new RealTimeSpeedChartWindow();
                    speedChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                speedChartWindow.Show();
            }
            else if (sender.Equals(showPressureChartItem))
            {
                if (pressureChartWindow == null)
                {
                    pressureChartWindow = new RealTimePressureChartWindow();
                    pressureChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                pressureChartWindow.Show();
            }
            else if (sender.Equals(showOtherChartItem))
            {
                if (otherWindow == null)
                {
                    otherWindow = new RealTimeOtherWindow();
                    otherWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                otherWindow.Show();
            }
            else if (sender.Equals(OverViewItem))
            {
                if (overviewWindow == null)
                {
                    overviewWindow = new OverviewWindow();
                    overviewWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                overviewWindow.Show();
            }
            else if (sender.Equals(chartViewItem))
            {
                if (chartWindow == null)
                {
                    chartWindow = new ChartWindow();
                    chartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                chartWindow.Show();
            }
            else if (sender.Equals(Antiskid_Display_Item))
            {
                if (antiskid_Display_Window == null)
                {
                    antiskid_Display_Window = new Antiskid_Display();
                    antiskid_Display_Window.CloseWindowEvent += OtherWindowClosedHandler;
                }
                antiskid_Display_Window.Show();
            }
            else if (sender.Equals(Antiskid_Setting_Item))
            {
                if (antiskid_Setting_Window == null)
                {
                    antiskid_Setting_Window = new Antiskid_Setting();
                    antiskid_Setting_Window.CloseWindowEvent += OtherWindowClosedHandler;
                }
                antiskid_Setting_Window.Show();
            }
        }

        private void CloseDevice()
        {
            //CanHelper.DeviceState res = canHelper.Close();
        }

        private void OtherWindowClosedHandler(bool winState, string name)
        {
            if ("EBCU1".Equals(name))
            {
                detailWindowCar1 = null;
            }
            else if ("EBCU2".Equals(name))
            {
                slaveDetailWindowCar2 = null;
            }
            else if ("EBCU3".Equals(name))
            {
                slaveDetailWindowCar3 = null;
            }
            else if ("EBCU4".Equals(name))
            {
                slaveDetailWindowCar4 = null;
            }
            else if ("EBCU5".Equals(name))
            {
                slaveDetailWindowCar5 = null;
            }
            else if ("EBCU6".Equals(name))
            {
                slaveDetailWindowCar6 = null;
            }
            else if ("speedChart".Equals(name))
            {
                speedChartWindow = null;
            }
            else if ("pressureChart".Equals(name))
            {
                pressureChartWindow = null;
            }
            else if ("otherChart".Equals(name))
            {
                otherWindow = null;
            }
            else if ("overview".Equals(name))
            {
                overviewWindow = null;
            }
            else if ("chart".Equals(name))
            {
                chartWindow = null;
            }
            else if ("Antiskid_Display_Window".Equals(name))
            {
                antiskid_Display_Window = null;
            }
            else if ("Antiskid_Setting_Window".Equals(name))
            {
                antiskid_Setting_Window = null;
            }
        }
        #endregion

        /// <summary>
        /// 打开外部下载程序
        /// </summary>
        private void DownloadExe()
        {
            //string path = ConfigurationManager.AppSettings["text"];
            //if (path == null || path == "")
            //{
            //    System.Windows.Forms.MessageBox.Show("外部程序路径配置错误" + path, "exe error!");
            //    return;
            //}
            //System.Windows.Forms.MessageBox.Show(path + "", "exe error!");
            if (recvThread.ThreadState == System.Threading.ThreadState.Running)
            {
                canHelper.Close();
                recvThread.Suspend();
            }
            Process.Start(@"CANJieMianMFC.exe");
        }

        /// <summary>
        /// 选择以太网为连接方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void byEthItem_Checked(object sender, RoutedEventArgs e)
        {
            byCanItem.IsChecked = false;
            connectType = ConnectType.ETH;
        }

        /// <summary>
        /// 选择USB-CAN为连接方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void byCanItem_Checked(object sender, RoutedEventArgs e)
        {
            byEthItem.IsChecked = false;
            connectType = ConnectType.CAN;
        }

        private void RecordFreq(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(frequent16))
            {
                recordFreq = 16;
            }
            else if (sender.Equals(frequent32))
            {
                recordFreq = 32;
            }
            else if (sender.Equals(frequent64))
            {
                recordFreq = 64;
            }
            else if (sender.Equals(frequent128))
            {
                recordFreq = 128;
            }
            else if (sender.Equals(frequent256))
            {
                recordFreq = 256;
            }
            else if (sender.Equals(frequent512))
            {
                recordFreq = 512;
            }
            else if (sender.Equals(frequent1024))
            {
                recordFreq = 1024;
            }
        }

        private void resumeItem_Click(object sender, RoutedEventArgs e)
        {
            if (recvThread.ThreadState == System.Threading.ThreadState.Suspended)
            {
                canHelper.Open();
                canHelper.Init();
                canHelper.Start();
                recvThread.Resume();
            }
        }

        private void ftpDownloadItem_Click(object sender, RoutedEventArgs e)
        {
            FTPWindow fTPWindow = new FTPWindow();
            //fTPWindow.ShowDialog();
            //2018-9-19
            fTPWindow.Show();
            
        }





        /// <summary>
        /// 导出为excel 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportXls_Click(object sender, RoutedEventArgs e)
        {
            //导出为xml
            txt_to_xml();
            //string fileName;
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.CheckFileExists = true;
            //sfd.CheckPathExists = true;
            //sfd.RestoreDirectory = true;
            //if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    double x = 0.0;
            //    fileName = sfd.FileName;
            //}
        }

        private void Export(string fileName)
        {
            IList<string> header = Utils.getXml("header.xml", "root");
            
        }


        public void OpenFile_1(string fileName)
        {                
            double x = 0.0;
                
            List<byte[]> content = FileBuilding.GetFileContent(fileName);
            List<List<CanDTO>> canList = FileBuilding.GetCanList(content, FileSource.PC);
            history.FileLength = FileBuilding.FileLength;
            for (int i = 0; i < canList.Count; i++)
            {
                int count = FileBuilding.CAN_MO_NUM;
                history.Count = canList.Count;
                history.X.Add(x);
                x += 0.1;
                for (int j = 0; j < canList[i].Count; j++)
                {
                    if (--count == 0)
                    {
                        FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.OK);
                    }
                    else
                    {
                        FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.WAIT);
                    }

                }
                index += 0.1;
            }
            HistoryDetail historyDetail = new HistoryDetail();
            historyDetail.SetHistory(history);
            historyDetail.Hide();
            //SingleChart historyChart = new SingleChart();

            //historyChart.Show();
            //historyChart.SetHistoryModel(history);
            //historyChart.PaintHistory();
            
        }

        private void Set_EBCU1_Array()
        {
            
        }
        

        /// <summary>
        /// 将指定的txt文件首先转成xml格式
        /// </summary>
        private void txt_to_xml()
        {
            Microsoft.Win32.OpenFileDialog openfile = new Microsoft.Win32.OpenFileDialog();
            openfile.DefaultExt = ".LOG";
            openfile.Filter = "Text Document (.log)|*.LOG";
            bool? openfile_result = openfile.ShowDialog();
            if (openfile_result == true)
            {
                StreamReader readFile = new StreamReader(openfile.FileName);               
            }
            else
            {
                return;
            }

            string[] lines = File.ReadAllLines(openfile.FileName);

            OpenFile_1(openfile.FileName);

            // 对应xml中的name属性
            List<string> info = new List<string>();
            info.Add("时间");

            XmlDocument header_xml = new XmlDocument();
            header_xml.Load("header.xml");
            XmlNode header_root = header_xml.SelectSingleNode("root");
            XmlNodeList header_list = header_root.ChildNodes;

            object[,] objData_Header = new object[1, header_list.Count];
            foreach (var item in header_list)
            {
                XmlElement header_item = (XmlElement)item;
                string header_content = header_item.InnerText;
                info.Add(header_content);
            }

            string[] info_Array = info.ToArray();

            // 下面构建xml文档
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<?xml version='1.0' encoding='UTF-8'?>").Append(Environment.NewLine);
            sb.Append("<data>").Append(Environment.NewLine);
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    sb.Append("\t" + "<row>").Append(Environment.NewLine);
                
            //    for (int j = 0; j < info_Array.Length; j++)
            //    {
                    
            //        sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
            //        sb.Append("\t\t\t" + "<name>" + info_Array[j] + "</name>").Append(Environment.NewLine);
            //        sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].dateTime + "</value>").Append(Environment.NewLine);
            //        sb.Append("\t\t" + "</column>").Append(Environment.NewLine);
            //    }
                
            //    sb.Append("\t" + "</row>").Append(Environment.NewLine);
            //}
            for(int i = 0; i < lines.Length; i++)
            {
                sb.Append("\t" + "<row>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<EBCU>" + "1" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[0] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].dateTime + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "1" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "2" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_2[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);


                sb.Append("\t\t" + "<EBCU>" + "3" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_3[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "4" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_4[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "5" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_5[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "6" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_6[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);



                sb.Append("\t" + "</row>").Append(Environment.NewLine);
            }
            sb.Append("</data>");

            string filePath = openfile.FileName;
            string filePath_1 = System.IO.Path.GetDirectoryName(filePath);
            string file_Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string txt_to_xml_road = filePath_1 + "\\" + file_Name + ".xml";
            FileStream fs = new FileStream(txt_to_xml_road, FileMode.Create);

            //获得字节数组
            byte[] data = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sb.ToString());

            //开始写入
            fs.Write(data, 0, data.Length);

            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
            System.Windows.MessageBox.Show("xml创建完成！");
        }

        private void TChartItem_Click(object sender, RoutedEventArgs e)
        {
            tChartDisplay = new TChartDisplay();
            tChartDisplay.Show();
        }

        private void fileZileDownloadItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@".\FileZilla-3.37.0\filezilla.exe");
        }
    }

}
