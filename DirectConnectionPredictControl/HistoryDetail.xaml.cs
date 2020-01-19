using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DirectConnectionPredictControl.CommenTool;
using MessageBox = System.Windows.MessageBox;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// HistoryDetail.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDetail : Window
    {
        public event closeWindowHandler CloseWindowEvent;
        private HistoryModel history;
        private CompositeCollection collection = new CompositeCollection();
        private List<HistoryDataModel> dataModelList = new List<HistoryDataModel>();
        private int nowPage = 1;
        private int totalPage;
        private int location;
        private static int LINE_PER_TIME = 30;
        private int id = 0;

        // 2018-11-28
        private System.Windows.Controls.ContextMenu context;
        private System.Windows.Controls.MenuItem analogDataItem;
        private System.Windows.Controls.MenuItem digitalInputItem;
        private System.Windows.Controls.MenuItem digitalOutputItem;
        private System.Windows.Controls.MenuItem faultDataItem;
        private System.Windows.Controls.MenuItem antiskidDataItem;

        // 2018-11-20
        private List<int> IDList = new List<int>();
        private List<int> IDSelectList = new List<int>();
        private List<int> IDSearchResultList = new List<int>();

        private List<string> DateTimeList = new List<string>();
        private List<int> DateTimeSelectList = new List<int>();

        private BackgroundWorker readWorker;


        public HistoryDetail()
        {
            InitializeComponent();
            location = 0;
            iniComboBox();
            Init();
            dataGridHeaderName_1 = GetDataGridHeaderName("本地时间", "4车主风管压力", dataGridHeaderName_1);
            dataGridHeaderName_2 = GetDataGridHeaderName("1架发送制动命令", "6架发送自检命令", dataGridHeaderName_2);
            dataGridHeaderName_3 = GetDataGridHeaderName("1架运行模式", "6架发送C车电制动切除", dataGridHeaderName_3);
            dataGridHeaderName_4 = GetDataGridHeaderName("架1制动风缸传感器故障", "6架轴2WSP连续保压超时故障", dataGridHeaderName_4);
            dataGridHeaderName_5 = GetDataGridHeaderName("1架轴1滑行等级", "6架轴2减速度", dataGridHeaderName_5);
        }

        public static List<string> dataGridHeaderName_1 = new List<string>();
        public static List<string> dataGridHeaderName_2 = new List<string>();
        public static List<string> dataGridHeaderName_3 = new List<string>();
        public static List<string> dataGridHeaderName_4 = new List<string>();
        public static List<string> dataGridHeaderName_5 = new List<string>();
        public List<string> GetDataGridHeaderName(string startString, string endString, List<string> dataGridHeaderName)
        {
            //dataGridHeaderName.Clear();
            //int startLocation = GetColumnNum(historyList, startString);
            //int endLocation = GetColumnNum(historyList, endString);
            //for (int i = startLocation; i < endLocation + 1; i++)
            //{
            //    dataGridHeaderName.Add(historyList.Columns[i].Header.ToString());
            //}
            //return dataGridHeaderName;
            return null;
        }


        /// <summary>
        /// 初始化函数，主要是为了在HistoryDetail窗口加载右键5个选择项和2个窗口进入项
        /// </summary>
        private void Init()
        {
            readWorker = new BackgroundWorker();
            readWorker.DoWork += ReadWorker_DoWork;
            readWorker.WorkerReportsProgress = true;
            readWorker.RunWorkerCompleted += ReadWorker_RunWorkerCompleted;
            readWorker.ProgressChanged += ReadWorker_ProgressChanged;

            context = new System.Windows.Controls.ContextMenu();

            analogDataItem = new System.Windows.Controls.MenuItem();
            analogDataItem.Header = "模拟量数据";
            analogDataItem.IsCheckable = true;
            analogDataItem.IsChecked = true;
            analogDataItem.Click += AnalogDataItem_Click;
            context.Items.Add(analogDataItem);

            digitalInputItem = new System.Windows.Controls.MenuItem();
            digitalInputItem.Header = "数字量输入";
            digitalInputItem.IsCheckable = true;
            digitalInputItem.IsChecked = true;
            digitalInputItem.Click += DigitalInputItem_Click;
            context.Items.Add(digitalInputItem);

            digitalOutputItem = new System.Windows.Controls.MenuItem();
            digitalOutputItem.Header = "数字量输出";
            digitalOutputItem.IsCheckable = true;
            digitalOutputItem.IsChecked = true;
            digitalOutputItem.Click += DigitalOutputItem_Click;
            context.Items.Add(digitalOutputItem);

            faultDataItem = new System.Windows.Controls.MenuItem();
            faultDataItem.Header = "故障数据";
            faultDataItem.IsCheckable = true;
            faultDataItem.IsChecked = true;
            faultDataItem.Click += FaultDataItem_Click;
            context.Items.Add(faultDataItem);

            antiskidDataItem = new System.Windows.Controls.MenuItem();
            antiskidDataItem.Header = "防滑数据";
            antiskidDataItem.IsCheckable = true;
            antiskidDataItem.IsChecked = true;
            antiskidDataItem.Click += AntiskidDataItem_Click;
            context.Items.Add(antiskidDataItem);

            // 添加一条分割线，方便辨认
            Separator sp = new Separator();
            context.Items.Add(sp);

            System.Windows.Controls.MenuItem overviewHisDataItem = new System.Windows.Controls.MenuItem();
            overviewHisDataItem.Header = "历史数据按实时监控界面回放";
            overviewHisDataItem.Click += OverviewHisDataItem_Click;
            context.Items.Add(overviewHisDataItem);

            System.Windows.Controls.MenuItem overviewHisChartItem = new System.Windows.Controls.MenuItem();
            overviewHisChartItem.Header = "历史数据画图显示";
            overviewHisChartItem.Click += OverviewHisChartItem_Click;
            context.Items.Add(overviewHisChartItem);

            this.ContextMenu = context;


        }

        private void ReadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            stateLbl.Content = "正在读取:" + string.Format("{0:N2}", (e.ProgressPercentage / history.Count) * 100) + "%";
            lineHasReadLbl.Content = e.ProgressPercentage;

        }

        private void ReadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stateLbl.Content = "就绪";
            totalPage = history.Count / LINE_PER_TIME;
            totalPageLbl.Content = totalPage;
            byteHasReadLbl.Content = history.FileLength / 1024 + " KB";
            lineHasReadLbl.Content = history.Count;
            GetData(location, location + LINE_PER_TIME);

        }

        private void ReadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            InitMem(history, worker);
        }



        #region HistoryDetail窗口上右键每一项所产生的事件

        #region 右键打开两个窗口的事件
        private void OverviewHisChartItem_Click(object sender, RoutedEventArgs e)
        {
            history.ExportToJson();
            HistoryChartDetail historyChartDetail = new HistoryChartDetail();
            historyChartDetail.Show();
        }

        private void OverviewHisDataItem_Click(object sender, RoutedEventArgs e)
        {
            OverviewWindowHis his = new OverviewWindowHis(history);
            his.Show();
        }
        #endregion

        #region 右键复选框显示数据事件
        /// <summary>
        /// 防滑数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AntiskidDataItem_Click(object sender, RoutedEventArgs e)
        {
            //int antiskidDataItemFir = GetColumnNum(historyList, "1架轴1滑行等级");
            //int antiskidDataItemLast = GetColumnNum(historyList, "6架轴2减速度");
            ////int antiskidDataItemLen = antiskidDataItemLast - antiskidDataItemFir + 1;
            //if (!antiskidDataItem.IsChecked)
            //{
            //    for (int i = antiskidDataItemFir; i < antiskidDataItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    for(int i = antiskidDataItemFir; i < antiskidDataItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Visible;
            //    }
            //}
        }

        /// <summary>
        /// 故障数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaultDataItem_Click(object sender, RoutedEventArgs e)
        {
            //int faultDataItemFir = GetColumnNum(historyList, "架1制动风缸传感器故障");
            //int faultDataItemLast = GetColumnNum(historyList, "6架OCAN2通讯故障");
            ////int faultDataItemLen = faultDataItemLast - faultDataItemFir + 1;
            //if (!faultDataItem.IsChecked)
            //{
            //    for (int i = faultDataItemFir; i < faultDataItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    for(int i = faultDataItemFir; i < faultDataItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Visible;
            //    }
            //}
        }

        /// <summary>
        /// 数字量输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DigitalOutputItem_Click(object sender, RoutedEventArgs e)
        {
            //int digitalOutputItemFir = GetColumnNum(historyList, "1架运行模式");
            //int digitalOutputItemLast = GetColumnNum(historyList, "6架发送C车电制动切除");
            ////int digitalOutputItemLen = digitalOutputItemLast - digitalOutputItemFir + 1;
            //if (!digitalOutputItem.IsChecked)
            //{
            //    for (int i = digitalOutputItemFir; i < digitalOutputItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    for (int i = digitalOutputItemFir; i < digitalOutputItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Visible;
            //    }
            //}
        }

        /// <summary>
        /// 数字量输入 43行   147---- 147 + 43
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DigitalInputItem_Click(object sender, RoutedEventArgs e)
        {
            //int digitalInputItemFir = GetColumnNum(historyList, "1架发送制动命令");
            //int digitalInputItemLast = GetColumnNum(historyList, "6架发送自检命令");
            ////int digitalInputItemLen = digitalInputItemLast - digitalInputItemFir + 1;
            //if (!digitalInputItem.IsChecked)
            //{
            //    for (int i = digitalInputItemFir; i < digitalInputItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    for (int i = digitalInputItemFir; i < digitalInputItemLast + 1; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        /// <summary>
        /// 模拟量数据 147行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalogDataItem_Click(object sender, RoutedEventArgs e)
        {
            //int analogDataItemFir = GetColumnNum(historyList, "本地时间");
            //int analogDataItemLast = GetColumnNum(historyList, "4车主风管压力");
            //int analogDataItemLen = analogDataItemLast - analogDataItemFir + 2;
            //if (!analogDataItem.IsChecked)
            //{
            //    for (int i = analogDataItemFir; i < analogDataItemLen; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    for (int i = analogDataItemFir; i < analogDataItemLen; i++)
            //    {
            //        historyList.Columns[i].Visibility = Visibility.Visible;
            //    }
            //}
        }

        /// <summary>
        /// 根据HistoryList的Header获得相应的为多少列
        /// </summary>
        /// <returns></returns>
        private int GetColumnNum(System.Windows.Controls.DataGrid dataGrid, string header)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (header == dataGrid.Columns[i].Header.ToString())
                {

                    return i;
                }
            }

            return -1;

        }
        #endregion

        #endregion

        #region 初始化搜索下拉框的时分秒
        private void iniComboBox()
        {
            string[] hourArray = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
                                   "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
                                   "21", "22", "23"};
            for (int i = 0; i < hourArray.Length; i++)
            {
                HourComboBox.Items.Add(hourArray[i]);
            }
            string[] MinuteOrSecondArray = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09" };
            for (int j = 0; j < MinuteOrSecondArray.Length; j++)
            {
                MinuteComboBox.Items.Add(MinuteOrSecondArray[j]);
                SecondComboBox.Items.Add(MinuteOrSecondArray[j]);
            }
            string temp;
            for (int k = 10; k < 60; k++)
            {
                temp = k.ToString();
                MinuteComboBox.Items.Add(temp);
                SecondComboBox.Items.Add(temp);
            }

            // 设置时分秒三个combobox的默认值
            HourComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
            SecondComboBox.SelectedIndex = 0;
            // 设置datepicker的默认值
            DateChoose.SelectedDate = DateTime.Now;
        }
        #endregion

        #region bussiness methods

        #region data insert

        private void GetData(int location, int tail)
        {
            // 2018-11-20
            IDList.Clear();
            IDSelectList.Clear();
            DateTimeList.Clear();
            DateTimeSelectList.Clear();


            //2018-10-10
            //if (dataModelList.Count > 0)
            //{
            //dataModelList.Clear();
            historyList.ItemsSource = null;
            historyList.Items.Clear();

            //}
            //for (int i = location; i < LINE_PER_TIME + location; i++)

            historyList.ItemsSource = dataModelList.GetRange(location, LINE_PER_TIME);

            historyList.ScrollIntoView(historyList.Items[0]);

        }


        #endregion

        public void SetHistory(HistoryModel history)
        {
            this.history = history;
            readWorker.RunWorkerAsync();

        }

        private void InitMem(HistoryModel history, BackgroundWorker worker)
        {
            for (int i = 0; i < history.Count; i++)
            {
                //Thread.Sleep(1);
                HistoryDataModel temp = new HistoryDataModel();
                if (i == history.Count)
                {
                    break;
                }
                #region detail
                #region 模拟量数据
                temp.ID = history.ListID[i];
                temp.dateTime = history.Containers_1[i].dateTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                temp.UnixTime = history.Containers_1[i].UnixHour + ":" + history.Containers_1[i].UnixMinute;

                temp.RefSpeed_1 = history.Containers_1[i].RefSpeed;
                temp.RefSpeed_6 = history.Containers_6[i].RefSpeed;

                temp.BrakeLevel_1 = history.Containers_1[i].BrakeLevel;
                temp.BrakeLevel_6 = history.Containers_6[i].BrakeLevel;

                temp.TrainBrakeForce_1 = history.Containers_1[i].TrainBrakeForce;
                temp.TrainBrakeForce_6 = history.Containers_6[i].TrainBrakeForce;

                temp.LifeSig_1 = history.Containers_1[i].LifeSig;
                temp.LifeSig_2 = history.Containers_2[i].LifeSig;
                temp.LifeSig_3 = history.Containers_3[i].LifeSig;
                temp.LifeSig_4 = history.Containers_4[i].LifeSig;
                temp.LifeSig_5 = history.Containers_5[i].LifeSig;
                temp.LifeSig_6 = history.Containers_6[i].LifeSig;

                temp.SpeedAx1_1 = history.Containers_1[i].SpeedA1Shaft1;
                temp.SpeedAx1_2 = history.Containers_2[i].SpeedShaft1;
                temp.SpeedAx1_3 = history.Containers_3[i].SpeedShaft1;
                temp.SpeedAx1_4 = history.Containers_4[i].SpeedShaft1;
                temp.SpeedAx1_5 = history.Containers_5[i].SpeedShaft1;
                temp.SpeedAx1_6 = history.Containers_6[i].SpeedA1Shaft1;

                temp.SpeedAx2_1 = history.Containers_1[i].SpeedA1Shaft2;
                temp.SpeedAx2_2 = history.Containers_2[i].SpeedShaft2;
                temp.SpeedAx2_3 = history.Containers_3[i].SpeedShaft2;
                temp.SpeedAx2_4 = history.Containers_4[i].SpeedShaft2;
                temp.SpeedAx2_5 = history.Containers_5[i].SpeedShaft2;
                temp.SpeedAx2_6 = history.Containers_6[i].SpeedA1Shaft2;

                // 这里制动缸目标值没有解析

                temp.BrakeCylinderSourcePressure_1 = history.Containers_1[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_2 = history.Containers_2[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_3 = history.Containers_3[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_4 = history.Containers_4[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_5 = history.Containers_5[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_6 = history.Containers_6[i].BrakeCylinderSourcePressure;

                temp.AirSpringPressure1_1 = history.Containers_1[i].AirSpring1PressureA1Car1;
                temp.AirSpringPressure1_2 = history.Containers_2[i].AirSpringPressure1;
                temp.AirSpringPressure1_3 = history.Containers_3[i].AirSpringPressure1;
                temp.AirSpringPressure1_4 = history.Containers_4[i].AirSpringPressure1;
                temp.AirSpringPressure1_5 = history.Containers_5[i].AirSpringPressure1;
                temp.AirSpringPressure1_6 = history.Containers_6[i].AirSpring1PressureA1Car1;

                temp.AirSpringPressure2_1 = history.Containers_1[i].AirSpring2PressureA1Car1;
                temp.AirSpringPressure2_2 = history.Containers_2[i].AirSpringPressure2;
                temp.AirSpringPressure2_3 = history.Containers_3[i].AirSpringPressure2;
                temp.AirSpringPressure2_4 = history.Containers_4[i].AirSpringPressure2;
                temp.AirSpringPressure2_5 = history.Containers_5[i].AirSpringPressure2;
                temp.AirSpringPressure2_6 = history.Containers_6[i].AirSpring2PressureA1Car1;

                temp.ParkPressure_1 = history.Containers_1[i].ParkPressureA1;
                temp.ParkPressure_2 = history.Containers_3[i].ParkPressure;
                temp.ParkPressure_3 = history.Containers_5[i].ParkPressure;

                temp.VldRealPressure_1 = history.Containers_1[i].VldRealPressureAx1;
                temp.VldRealPressure_2 = history.Containers_2[i].VldRealPressure;
                temp.VldRealPressure_3 = history.Containers_3[i].VldRealPressure;
                temp.VldRealPressure_4 = history.Containers_4[i].VldRealPressure;
                temp.VldRealPressure_5 = history.Containers_5[i].VldRealPressure;
                temp.VldRealPressure_6 = history.Containers_6[i].VldRealPressureAx1;

                temp.Bcp1Pressure_1 = history.Containers_1[i].Bcp1PressureAx1;
                temp.Bcp1Pressure_2 = history.Containers_2[i].Bcp1Pressure;
                temp.Bcp1Pressure_3 = history.Containers_3[i].Bcp1Pressure;
                temp.Bcp1Pressure_4 = history.Containers_4[i].Bcp1Pressure;
                temp.Bcp1Pressure_5 = history.Containers_5[i].Bcp1Pressure;
                temp.Bcp1Pressure_6 = history.Containers_6[i].Bcp1PressureAx1;

                temp.Bcp2Pressure_1 = history.Containers_1[i].Bcp2PressureAx2;
                temp.Bcp2Pressure_2 = history.Containers_2[i].Bcp2Pressure;
                temp.Bcp2Pressure_3 = history.Containers_3[i].Bcp2Pressure;
                temp.Bcp2Pressure_4 = history.Containers_4[i].Bcp2Pressure;
                temp.Bcp2Pressure_5 = history.Containers_5[i].Bcp2Pressure;
                temp.Bcp2Pressure_6 = history.Containers_6[i].Bcp2PressureAx2;

                temp.VldPressureSetup_1 = history.Containers_1[i].VldPressureSetupAx1;
                temp.VldPressureSetup_2 = history.Containers_2[i].VldSetupPressure;
                temp.VldPressureSetup_3 = history.Containers_3[i].VldSetupPressure;
                temp.VldPressureSetup_4 = history.Containers_4[i].VldSetupPressure;
                temp.VldPressureSetup_5 = history.Containers_5[i].VldSetupPressure;
                temp.VldPressureSetup_6 = history.Containers_6[i].VldPressureSetupAx1;

                temp.Mass_1 = history.Containers_1[i].MassA1;
                temp.Mass_2 = history.Containers_2[i].MassValue;
                temp.Mass_3 = history.Containers_3[i].MassValue;
                temp.Mass_4 = history.Containers_4[i].MassValue;
                temp.Mass_5 = history.Containers_5[i].MassValue;
                temp.Mass_6 = history.Containers_6[i].MassA1;

                temp.SelfTestSetup_1 = history.Containers_1[i].SelfTestSetup;
                temp.SelfTestSetup_2 = history.Containers_2[i].SelfTestSetup;
                temp.SelfTestSetup_3 = history.Containers_3[i].SelfTestSetup;
                temp.SelfTestSetup_4 = history.Containers_4[i].SelfTestSetup;
                temp.SelfTestSetup_5 = history.Containers_5[i].SelfTestSetup;
                temp.SelfTestSetup_6 = history.Containers_6[i].SelfTestSetup;

                temp.VCMLifeSig_1 = history.Containers_1[i].VCMLifeSig;
                temp.VCMLifeSig_6 = history.Containers_6[i].VCMLifeSig;

                temp.DCULifeSig1_1 = history.Containers_1[i].DcuLifeSig[0];
                temp.DCULifeSig6_1 = history.Containers_6[i].DcuLifeSig[0];
                temp.DCULifeSig1_2 = history.Containers_1[i].DcuLifeSig[1];
                temp.DCULifeSig6_2 = history.Containers_6[i].DcuLifeSig[1];
                temp.DCULifeSig1_3 = history.Containers_1[i].DcuLifeSig[2];
                temp.DCULifeSig6_3 = history.Containers_6[i].DcuLifeSig[2];
                temp.DCULifeSig1_4 = history.Containers_1[i].DcuLifeSig[3];
                temp.DCULifeSig6_4 = history.Containers_6[i].DcuLifeSig[3];

                temp.DcuEbRealValue1_1 = history.Containers_1[i].DcuEbRealValue[0];
                temp.DcuEbRealValue6_1 = history.Containers_6[i].DcuEbRealValue[0];
                temp.DcuMax1_1 = history.Containers_1[i].DcuMax[0];
                temp.DcuMax6_1 = history.Containers_6[i].DcuMax[0];
                temp.DcuEbRealValue1_2 = history.Containers_1[i].DcuEbRealValue[1];
                temp.DcuEbRealValue6_2 = history.Containers_6[i].DcuEbRealValue[1];
                temp.DcuMax1_2 = history.Containers_1[i].DcuMax[1];
                temp.DcuMax6_2 = history.Containers_6[i].DcuMax[1];
                temp.DcuEbRealValue1_3 = history.Containers_1[i].DcuEbRealValue[2];
                temp.DcuEbRealValue6_3 = history.Containers_6[i].DcuEbRealValue[2];
                temp.DcuMax1_3 = history.Containers_1[i].DcuMax[2];
                temp.DcuMax6_3 = history.Containers_6[i].DcuMax[2];
                temp.DcuEbRealValue1_4 = history.Containers_1[i].DcuEbRealValue[3];
                temp.DcuEbRealValue6_4 = history.Containers_6[i].DcuEbRealValue[3];
                temp.DcuMax1_4 = history.Containers_1[i].DcuMax[3];
                temp.DcuMax6_4 = history.Containers_6[i].DcuMax[3];

                temp.AbRealValue1_1 = history.Containers_1[i].AbRealValue[0];
                temp.AbRealValue1_2 = history.Containers_1[i].AbRealValue[1];
                temp.AbRealValue1_3 = history.Containers_1[i].AbRealValue[2];
                temp.AbRealValue1_4 = history.Containers_1[i].AbRealValue[3];
                temp.AbRealValue1_5 = history.Containers_1[i].AbRealValue[4];
                temp.AbRealValue1_6 = history.Containers_1[i].AbRealValue[5];

                temp.AbRealValue6_1 = history.Containers_6[i].AbRealValue[0];
                temp.AbRealValue6_2 = history.Containers_6[i].AbRealValue[1];
                temp.AbRealValue6_3 = history.Containers_6[i].AbRealValue[2];
                temp.AbRealValue6_4 = history.Containers_6[i].AbRealValue[3];
                temp.AbRealValue6_5 = history.Containers_6[i].AbRealValue[4];
                temp.AbRealValue6_6 = history.Containers_6[i].AbRealValue[5];

                temp.AbCapacity1_1 = history.Containers_1[i].AbCapacity[0];
                temp.AbCapacity1_2 = history.Containers_1[i].AbCapacity[1];
                temp.AbCapacity1_3 = history.Containers_1[i].AbCapacity[2];
                temp.AbCapacity1_4 = history.Containers_1[i].AbCapacity[3];
                temp.AbCapacity1_5 = history.Containers_1[i].AbCapacity[4];
                temp.AbCapacity1_6 = history.Containers_1[i].AbCapacity[5];

                temp.AbCapacity6_1 = history.Containers_6[i].AbCapacity[0];
                temp.AbCapacity6_2 = history.Containers_6[i].AbCapacity[1];
                temp.AbCapacity6_3 = history.Containers_6[i].AbCapacity[2];
                temp.AbCapacity6_4 = history.Containers_6[i].AbCapacity[3];
                temp.AbCapacity6_5 = history.Containers_6[i].AbCapacity[4];
                temp.AbCapacity6_6 = history.Containers_6[i].AbCapacity[5];

                temp.SoftwareVersionCPU_1 = history.Containers_1[i].SoftwareVersionCPU;
                temp.SoftwareVersionCPU_2 = history.Containers_2[i].SoftwareVersionCPU;
                temp.SoftwareVersionCPU_3 = history.Containers_3[i].SoftwareVersionCPU;
                temp.SoftwareVersionCPU_4 = history.Containers_4[i].SoftwareVersionCPU;
                temp.SoftwareVersionCPU_5 = history.Containers_5[i].SoftwareVersionCPU;
                temp.SoftwareVersionCPU_6 = history.Containers_6[i].SoftwareVersionCPU;

                temp.SoftwareVersionEP_1 = history.Containers_1[1].SoftwareVersionEP;
                temp.SoftwareVersionEP_2 = history.Containers_2[1].SoftwareVersionEP;
                temp.SoftwareVersionEP_3 = history.Containers_3[1].SoftwareVersionEP;
                temp.SoftwareVersionEP_4 = history.Containers_4[1].SoftwareVersionEP;
                temp.SoftwareVersionEP_5 = history.Containers_5[1].SoftwareVersionEP;
                temp.SoftwareVersionEP_6 = history.Containers_6[1].SoftwareVersionEP;

                // 这里还有MVB轮径、存储轮径没有解析

                temp.ParkPressure_4 = history.Containers_4[i].ParkPressure;
                #endregion

                #region 数字量输入
                temp.BrakeCmd_1 = history.Containers_1[i].BrakeCmd;
                temp.BrakeCmd_6 = history.Containers_6[i].BrakeCmd;

                temp.DriveCmd_1 = history.Containers_1[i].DriveCmd;
                temp.DriveCmd_6 = history.Containers_6[i].DriveCmd;

                temp.LazyCmd_1 = history.Containers_1[i].LazyCmd;
                temp.LazyCmd_6 = history.Containers_6[i].LazyCmd;

                temp.FastBrakeCmd_1 = history.Containers_1[i].FastBrakeCmd;
                temp.FastBrakeCmd_6 = history.Containers_6[i].FastBrakeCmd;

                temp.EmergencyBrakeCmd_1 = history.Containers_1[i].EmergencyBrakeCmd;
                temp.EmergencyBrakeCmd_6 = history.Containers_6[i].EmergencyBrakeCmd;

                temp.EmergencyBrakeActive_1 = history.Containers_1[i].EmergencyBrakeActiveA1;
                temp.EmergencyBrakeActive_2 = history.Containers_2[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_3 = history.Containers_3[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_4 = history.Containers_4[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_5 = history.Containers_5[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_6 = history.Containers_6[i].EmergencyBrakeActiveA1;

                temp.AbActive_1 = history.Containers_1[i].AbActive;
                temp.AbActive_2 = history.Containers_2[i].AbBrakeActive;
                temp.AbActive_3 = history.Containers_3[i].AbBrakeActive;
                temp.AbActive_4 = history.Containers_4[i].AbBrakeActive;
                temp.AbActive_5 = history.Containers_5[i].AbBrakeActive;
                temp.AbActive_6 = history.Containers_6[i].AbActive;

                // 这里只有1架的停放制动缓解
                temp.ParkBreakRealease_1 = history.Containers_1[i].ParkBreakRealease;

                temp.HardDrive_1 = history.Containers_1[i].HardDriveCmd;
                temp.HardDrive_6 = history.Containers_6[i].HardDriveCmd;

                temp.HardFastBrake_1 = history.Containers_1[i].HardFastBrakeCmd;
                temp.HardFastBrake_6 = history.Containers_6[i].HardFastBrakeCmd;

                temp.HardEmergencyBrake_1 = history.Containers_1[i].HardEmergencyBrake;
                temp.HardEmergencyBrake_6 = history.Containers_6[i].HardEmergencyBrake;

                temp.HardEmergencyDrive_1 = history.Containers_1[i].HardEmergencyDriveCmd;
                temp.HardEmergencyDrive_6 = history.Containers_6[i].HardEmergencyDriveCmd;

                temp.NetDrive_1 = history.Containers_1[i].NetDriveCmd;
                temp.NetDrive_6 = history.Containers_6[i].NetDriveCmd;

                temp.NetBrake_1 = history.Containers_1[i].NetBrakeCmd;
                temp.NetBrake_6 = history.Containers_6[i].NetBrakeCmd;

                temp.CanTestA_1 = history.Containers_1[i].CanUintSelfTestCmd_A;
                temp.CanTestA_6 = history.Containers_6[i].CanUintSelfTestCmd_A;
                temp.CanTestB_1 = history.Containers_1[i].CanUintSelfTestCmd_B;
                temp.CanTestB_6 = history.Containers_6[i].CanUintSelfTestCmd_B;

                temp.KeepBrakeRelease_1 = history.Containers_1[i].KeepBrakeState;
                temp.KeepBrakeRelease_6 = history.Containers_6[i].KeepBrakeState;

                temp.SelfTestCmd_1 = history.Containers_1[i].SelfTestCmd;
                temp.SelfTestCmd_6 = history.Containers_6[i].SelfTestCmd;
                #endregion

                #region 数字量输出
                temp.Mode_1 = history.Containers_1[i].Mode;
                temp.Mode_6 = history.Containers_6[i].Mode;

                temp.LazyCmd_1 = history.Containers_1[i].LazyCmd;
                temp.LazyCmd_6 = history.Containers_6[i].LazyCmd;

                temp.KeepStateBreak_1 = history.Containers_1[i].KeepBrakeState;
                temp.KeepStateBreak_6 = history.Containers_6[i].KeepBrakeState;

                temp.LazyState_1 = history.Containers_1[i].LazyState;
                temp.LazyState_6 = history.Containers_6[i].LazyState;

                temp.DriveState_1 = history.Containers_1[i].DriveState;
                temp.DriveState_6 = history.Containers_6[i].DriveState;

                temp.NormalBrakeState_1 = history.Containers_1[i].NormalBrakeState;
                temp.NormalBrakeState_6 = history.Containers_6[i].NormalBrakeState;

                temp.EmergencyBrakeState_1 = history.Containers_1[i].EmergencyBrakeState;
                temp.EmergencyBrakeState_6 = history.Containers_6[i].EmergencyBrakeState;

                temp.ZeroSpeed_1 = history.Containers_1[i].ZeroSpeed;
                temp.ZeroSpeed_6 = history.Containers_6[i].ZeroSpeed;

                temp.SelfFail_1 = history.Containers_1[i].SelfTestFail;
                temp.SelfFail_6 = history.Containers_6[i].SelfTestFail;

                temp.UnTest24_1 = history.Containers_1[i].UnSelfTest24;
                temp.UnTest24_6 = history.Containers_6[i].UnSelfTest24;

                temp.UnTest26_1 = history.Containers_1[i].UnSelfTest26;
                temp.UnTest26_6 = history.Containers_6[i].UnSelfTest26;

                temp.GatewayValveState_1 = history.Containers_1[i].GateValveState;
                temp.GatewayValveState_6 = history.Containers_6[i].GateValveState;

                temp.CanUnitTestOn_1 = history.Containers_1[i].CanUnitSelfTestOn;
                temp.CanUnitTestOn_6 = history.Containers_6[i].CanUnitSelfTestOn;

                temp.CanValveActive_1 = history.Containers_1[i].ValveCanEmergencyActive;
                temp.CanValveActive_6 = history.Containers_6[i].ValveCanEmergencyActive;

                temp.CanUnitTestOff_1 = history.Containers_1[i].CanUintSelfTestOver;
                temp.CanUnitTestOff_6 = history.Containers_6[i].CanUintSelfTestOver;

                temp.TowingMode_1 = history.Containers_1[i].TowingMode;
                temp.TowingMode_6 = history.Containers_6[i].TowingMode;

                temp.ATOMode_1 = history.Containers_1[i].ATOMode1;
                temp.ATOMode_6 = history.Containers_6[i].ATOMode1;

                temp.ATOHold_1 = history.Containers_1[i].ATOHold;
                temp.ATOHold_6 = history.Containers_6[i].ATOHold;

                temp.BrakeLevelActive_1 = history.Containers_1[i].BrakeLevelEnable;
                temp.BrakeLevelActive_6 = history.Containers_6[i].BrakeLevelEnable;

                temp.DCU_Ed_Ok_1_1 = history.Containers_1[i].DcuEbOK[0];
                temp.DCU_Ed_Ok_1_6 = history.Containers_6[i].DcuEbOK[0];
                temp.DCU_Ed_Fadeout_1_1 = history.Containers_1[i].DcuEbFadeout[0];
                temp.DCU_Ed_Fadeout_1_6 = history.Containers_6[i].DcuEbFadeout[0];
                temp.DCU_Ed_Slip_1_1 = history.Containers_1[i].DcuEbSlip[0];
                temp.DCU_Ed_Slip_1_6 = history.Containers_6[i].DcuEbSlip[0];
                // 电制动不可用 没有
                temp.DCU_Ed_Ok_2_1 = history.Containers_1[i].DcuEbOK[1];
                temp.DCU_Ed_Ok_2_6 = history.Containers_6[i].DcuEbOK[1];
                temp.DCU_Ed_Fadeout_2_1 = history.Containers_1[i].DcuEbFadeout[1];
                temp.DCU_Ed_Fadeout_2_6 = history.Containers_6[i].DcuEbFadeout[1];
                temp.DCU_Ed_Slip_2_1 = history.Containers_1[i].DcuEbSlip[1];
                temp.DCU_Ed_Slip_2_6 = history.Containers_6[i].DcuEbSlip[1];
                // 电制动不可用 没有
                temp.DCU_Ed_Ok_3_1 = history.Containers_1[i].DcuEbOK[2];
                temp.DCU_Ed_Ok_3_6 = history.Containers_6[i].DcuEbOK[2];
                temp.DCU_Ed_Fadeout_3_1 = history.Containers_1[i].DcuEbFadeout[2];
                temp.DCU_Ed_Fadeout_3_6 = history.Containers_6[i].DcuEbFadeout[2];
                temp.DCU_Ed_Slip_3_1 = history.Containers_1[i].DcuEbSlip[2];
                temp.DCU_Ed_Slip_3_6 = history.Containers_6[i].DcuEbSlip[2];
                // 电制动不可用 没有
                temp.DCU_Ed_Ok_4_1 = history.Containers_1[i].DcuEbOK[3];
                temp.DCU_Ed_Ok_4_6 = history.Containers_6[i].DcuEbOK[3];
                temp.DCU_Ed_Fadeout_4_1 = history.Containers_1[i].DcuEbFadeout[3];
                temp.DCU_Ed_Fadeout_4_6 = history.Containers_6[i].DcuEbFadeout[3];
                temp.DCU_Ed_Slip_4_1 = history.Containers_1[i].DcuEbSlip[3];
                temp.DCU_Ed_Slip_4_6 = history.Containers_6[i].DcuEbSlip[3];
                // 电制动不可用 没有

                temp.VCM2MVBState_1 = history.Containers_1[i].VCM_MVBConnectionState;
                temp.VCM2MVBState_2 = history.Containers_6[i].VCM_MVBConnectionState;

                temp.Slip_1 = history.Containers_1[i].SlipA1;
                temp.Slip_2 = history.Containers_2[i].Slip;
                temp.Slip_3 = history.Containers_3[i].Slip;
                temp.Slip_4 = history.Containers_4[i].Slip;
                temp.Slip_5 = history.Containers_5[i].Slip;
                temp.Slip_6 = history.Containers_6[i].SlipA1;

                temp.AbStatues_1 = history.Containers_1[i].AbStatuesA1;
                temp.AbStatues_2 = history.Containers_2[i].AbBrakeSatet;
                temp.AbStatues_3 = history.Containers_3[i].AbBrakeSatet;
                temp.AbStatues_4 = history.Containers_4[i].AbBrakeSatet;
                temp.AbStatues_5 = history.Containers_5[i].AbBrakeSatet;
                temp.AbStatues_6 = history.Containers_6[i].AbStatuesA1;

                // 风缸压力低 没有

                // 空簧信号有效 没有

                temp.SelfInt_1 = history.Containers_1[i].SelfTestInt;
                temp.SelfInt_6 = history.Containers_6[i].SelfTestInt;
                temp.SelfActive_1 = history.Containers_1[i].SelfTestActive;
                temp.SelfActive_6 = history.Containers_6[i].SelfTestActive;
                temp.SelfSuccess_1 = history.Containers_1[i].SelfTestSuccess;
                temp.SelfSuccess_6 = history.Containers_6[i].SelfTestSuccess;

                temp.AbFadeOut_1 = history.Containers_1[i].EdFadeOut;
                temp.AbFadeOut_6 = history.Containers_6[i].EdFadeOut;

                temp.TrainBrakeEnable_1 = history.Containers_1[i].TrainBrakeEnable;
                temp.TrainBrakeEnable_6 = history.Containers_6[i].TrainBrakeEnable;

                temp.EDoutB_1 = history.Containers_1[i].EdOffB1;
                temp.EDoutB_6 = history.Containers_6[i].EdOffB1;
                temp.EDoutC_1 = history.Containers_1[i].EdOffC1;
                temp.EDoutC_6 = history.Containers_6[i].EdOffC1;

                // 最后还有1、2、3、4、5、6架的自检相关内容(老衲写不动了...
                #endregion

                #region 故障数据
                temp.BSSRSenorFault_1 = history.Containers_1[i].BSSRSenorFault;
                temp.BSSRSenorFault_2 = history.Containers_2[i].BSSRSenorFault;
                temp.BSSRSenorFault_3 = history.Containers_3[i].BSSRSenorFault;
                temp.BSSRSenorFault_4 = history.Containers_4[i].BSSRSenorFault;
                temp.BSSRSenorFault_5 = history.Containers_5[i].BSSRSenorFault;
                temp.BSSRSenorFault_6 = history.Containers_6[i].BSSRSenorFault;

                temp.AirSpringSenorFault1_1 = history.Containers_1[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault1_2 = history.Containers_2[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault1_3 = history.Containers_3[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault1_4 = history.Containers_4[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault1_5 = history.Containers_5[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault1_6 = history.Containers_6[i].AirSpringSenorFault_1;

                temp.AirSpringSenorFault2_1 = history.Containers_1[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault2_2 = history.Containers_2[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault2_3 = history.Containers_3[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault2_4 = history.Containers_4[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault2_5 = history.Containers_5[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault2_6 = history.Containers_6[i].AirSpringSenorFault_2;

                temp.ParkCylinderSenorFault_1 = history.Containers_1[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_3 = history.Containers_3[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_5 = history.Containers_5[i].ParkCylinderSenorFault;

                // 4架主风管传感器故障 没有

                temp.VLDSensorFault_1 = history.Containers_1[i].VLDSensorFault;
                temp.VLDSensorFault_2 = history.Containers_2[i].VLDSensorFault;
                temp.VLDSensorFault_3 = history.Containers_3[i].VLDSensorFault;
                temp.VLDSensorFault_4 = history.Containers_4[i].VLDSensorFault;
                temp.VLDSensorFault_5 = history.Containers_5[i].VLDSensorFault;
                temp.VLDSensorFault_6 = history.Containers_6[i].VLDSensorFault;

                temp.BSRSenorFault1_1 = history.Containers_1[i].BSRSenorFault_1;
                temp.BSRSenorFault1_2 = history.Containers_2[i].BSRSenorFault_1;
                temp.BSRSenorFault1_3 = history.Containers_3[i].BSRSenorFault_1;
                temp.BSRSenorFault1_4 = history.Containers_4[i].BSRSenorFault_1;
                temp.BSRSenorFault1_5 = history.Containers_5[i].BSRSenorFault_1;
                temp.BSRSenorFault1_6 = history.Containers_6[i].BSRSenorFault_1;

                temp.BSRSenorFault2_1 = history.Containers_1[i].BSRSenorFault_2;
                temp.BSRSenorFault2_2 = history.Containers_2[i].BSRSenorFault_2;
                temp.BSRSenorFault2_3 = history.Containers_3[i].BSRSenorFault_2;
                temp.BSRSenorFault2_4 = history.Containers_4[i].BSRSenorFault_2;
                temp.BSRSenorFault2_5 = history.Containers_5[i].BSRSenorFault_2;
                temp.BSRSenorFault2_6 = history.Containers_6[i].BSRSenorFault_2;

                temp.AirSpringOverflow1_1 = history.Containers_1[i].AirSpringOverflow_1;
                temp.AirSpringOverflow1_2 = history.Containers_2[i].AirSpringOverflow_1;
                temp.AirSpringOverflow1_3 = history.Containers_3[i].AirSpringOverflow_1;
                temp.AirSpringOverflow1_4 = history.Containers_4[i].AirSpringOverflow_1;
                temp.AirSpringOverflow1_5 = history.Containers_5[i].AirSpringOverflow_1;
                temp.AirSpringOverflow1_6 = history.Containers_6[i].AirSpringOverflow_1;

                temp.AirSpringOverflow2_1 = history.Containers_1[i].AirSpringOverflow_2;
                temp.AirSpringOverflow2_2 = history.Containers_2[i].AirSpringOverflow_2;
                temp.AirSpringOverflow2_3 = history.Containers_3[i].AirSpringOverflow_2;
                temp.AirSpringOverflow2_4 = history.Containers_4[i].AirSpringOverflow_2;
                temp.AirSpringOverflow2_5 = history.Containers_5[i].AirSpringOverflow_2;
                temp.AirSpringOverflow2_6 = history.Containers_6[i].AirSpringOverflow_2;

                temp.BSRLow_1 = history.Containers_1[i].BSRLowA11;
                temp.BSRLow_2 = history.Containers_2[i].BSRLow1;
                temp.BSRLow_3 = history.Containers_3[i].BSRLow1;
                temp.BSRLow_4 = history.Containers_4[i].BSRLow1;
                temp.BSRLow_5 = history.Containers_5[i].BSRLow1;
                temp.BSRLow_6 = history.Containers_6[i].BSRLowA11;

                temp.BCUFail_Serious_1 = history.Containers_1[i].BCUFail_Serious;
                temp.BCUFail_Serious_2 = history.Containers_2[i].BCUFail_Serious;
                temp.BCUFail_Serious_3 = history.Containers_3[i].BCUFail_Serious;
                temp.BCUFail_Serious_4 = history.Containers_4[i].BCUFail_Serious;
                temp.BCUFail_Serious_5 = history.Containers_5[i].BCUFail_Serious;
                temp.BCUFail_Serious_6 = history.Containers_6[i].BCUFail_Serious;
                temp.BCUFail_Middle_1 = history.Containers_1[i].BCUFail_Middle;
                temp.BCUFail_Middle_2 = history.Containers_2[i].BCUFail_Middle;
                temp.BCUFail_Middle_3 = history.Containers_3[i].BCUFail_Middle;
                temp.BCUFail_Middle_4 = history.Containers_4[i].BCUFail_Middle;
                temp.BCUFail_Middle_5 = history.Containers_5[i].BCUFail_Middle;
                temp.BCUFail_Middle_6 = history.Containers_6[i].BCUFail_Middle;
                temp.BCUFail_Slight_1 = history.Containers_1[i].BCUFail_Slight;
                temp.BCUFail_Slight_2 = history.Containers_2[i].BCUFail_Slight;
                temp.BCUFail_Slight_3 = history.Containers_3[i].BCUFail_Slight;
                temp.BCUFail_Slight_4 = history.Containers_4[i].BCUFail_Slight;
                temp.BCUFail_Slight_5 = history.Containers_5[i].BCUFail_Slight;
                temp.BCUFail_Slight_6 = history.Containers_6[i].BCUFail_Slight;

                temp.EmergencyBrakeFault_1 = history.Containers_1[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_2 = history.Containers_2[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_3 = history.Containers_3[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_4 = history.Containers_4[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_5 = history.Containers_5[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_6 = history.Containers_6[i].EmergencyBrakeFault;

                temp.CanASPEnable_1 = history.Containers_1[i].CanASPEnable;
                temp.CanASPEnable_6 = history.Containers_6[i].CanASPEnable;

                temp.BCPLowA_1 = history.Containers_1[i].BCPLowA;
                temp.BCPLowA_6 = history.Containers_6[i].BCPLowA;
                temp.BCPLowB_1 = history.Containers_1[i].BCPLowB;
                temp.BCPLowB_6 = history.Containers_6[i].BCPLowB;
                temp.BCPLowC_1 = history.Containers_1[i].BCPLowC;
                temp.BCPLowC_6 = history.Containers_6[i].BCPLowC;

                temp.SpeedSenorFault1_1 = history.Containers_1[i].SpeedSenorFault_1;
                temp.SpeedSenorFault1_2 = history.Containers_2[i].SpeedSenorFault_1;
                temp.SpeedSenorFault1_3 = history.Containers_3[i].SpeedSenorFault_1;
                temp.SpeedSenorFault1_4 = history.Containers_4[i].SpeedSenorFault_1;
                temp.SpeedSenorFault1_5 = history.Containers_5[i].SpeedSenorFault_1;
                temp.SpeedSenorFault1_6 = history.Containers_6[i].SpeedSenorFault_1;

                temp.SpeedSenorFault2_1 = history.Containers_1[i].SpeedSenorFault_2;
                temp.SpeedSenorFault2_2 = history.Containers_2[i].SpeedSenorFault_2;
                temp.SpeedSenorFault2_3 = history.Containers_3[i].SpeedSenorFault_2;
                temp.SpeedSenorFault2_4 = history.Containers_4[i].SpeedSenorFault_2;
                temp.SpeedSenorFault2_5 = history.Containers_5[i].SpeedSenorFault_2;
                temp.SpeedSenorFault2_6 = history.Containers_6[i].SpeedSenorFault_2;

                temp.WSPFault1_1 = history.Containers_1[i].WSPFault_1;
                temp.WSPFault1_2 = history.Containers_2[i].WSPFault_1;
                temp.WSPFault1_3 = history.Containers_3[i].WSPFault_1;
                temp.WSPFault1_4 = history.Containers_4[i].WSPFault_1;
                temp.WSPFault1_5 = history.Containers_5[i].WSPFault_1;
                temp.WSPFault1_6 = history.Containers_6[i].WSPFault_1;

                temp.WSPFault2_1 = history.Containers_1[i].WSPFault_2;
                temp.WSPFault2_2 = history.Containers_2[i].WSPFault_2;
                temp.WSPFault2_3 = history.Containers_3[i].WSPFault_2;
                temp.WSPFault2_4 = history.Containers_4[i].WSPFault_2;
                temp.WSPFault2_5 = history.Containers_5[i].WSPFault_2;
                temp.WSPFault2_6 = history.Containers_6[i].WSPFault_2;

                temp.CodeConnectorFault_1 = history.Containers_1[i].CodeConnectorFault;
                temp.CodeConnectorFault_2 = history.Containers_2[i].CodeConnectorFault;
                temp.CodeConnectorFault_3 = history.Containers_3[i].CodeConnectorFault;
                temp.CodeConnectorFault_4 = history.Containers_4[i].CodeConnectorFault;
                temp.CodeConnectorFault_5 = history.Containers_5[i].CodeConnectorFault;
                temp.CodeConnectorFault_6 = history.Containers_6[i].CodeConnectorFault;

                temp.AirSpringLimit_1 = history.Containers_1[i].AirSpringLimit;
                temp.AirSpringLimit_2 = history.Containers_2[i].AirSpringLimit;
                temp.AirSpringLimit_3 = history.Containers_3[i].AirSpringLimit;
                temp.AirSpringLimit_4 = history.Containers_4[i].AirSpringLimit;
                temp.AirSpringLimit_5 = history.Containers_5[i].AirSpringLimit;
                temp.AirSpringLimit_6 = history.Containers_6[i].AirSpringLimit;

                temp.BrakeNotRealease_1 = history.Containers_1[i].BrakeNotRealease;
                temp.BrakeNotRealease_2 = history.Containers_2[i].BrakeNotRealease;
                temp.BrakeNotRealease_3 = history.Containers_3[i].BrakeNotRealease;
                temp.BrakeNotRealease_4 = history.Containers_4[i].BrakeNotRealease;
                temp.BrakeNotRealease_5 = history.Containers_5[i].BrakeNotRealease;
                temp.BrakeNotRealease_6 = history.Containers_6[i].BrakeNotRealease;

                temp.BCPLow_1 = history.Containers_1[i].BCPLowA11;
                temp.BCPLow_2 = history.Containers_2[i].BCPLow1;
                temp.BCPLow_3 = history.Containers_3[i].BCPLow1;
                temp.BCPLow_4 = history.Containers_4[i].BCPLow1;
                temp.BCPLow_5 = history.Containers_5[i].BCPLow1;
                temp.BCPLow_6 = history.Containers_6[i].BCPLowA11;

                temp.SpeedDetection_1 = history.Containers_1[i].SpeedDetection;
                temp.SpeedDetection_6 = history.Containers_6[i].SpeedDetection;

                temp.CanBusFail1_1 = history.Containers_1[i].CanBusFail1;
                temp.CanBusFail6_1 = history.Containers_6[i].CanBusFail1;
                temp.CanBusFail1_2 = history.Containers_1[i].CanBusFail2;
                temp.CanBusFail6_2 = history.Containers_6[i].CanBusFail2;

                // 发送制动指令不一致 没有

                temp.Event_High_1 = history.Containers_1[i].EventHigh;
                temp.Event_High_6 = history.Containers_6[i].EventHigh;
                temp.Event_Middle_1 = history.Containers_1[i].EventMid;
                temp.Event_Middle_6 = history.Containers_6[i].EventMid;
                temp.Event_Low_1 = history.Containers_1[i].EventLow;
                temp.Event_Low_6 = history.Containers_6[i].EventLow;

                temp.ICANFault1_1 = history.Containers_1[i].ICANFault1;
                temp.ICANFault2_1 = history.Containers_2[i].ICANFault1;
                temp.ICANFault3_1 = history.Containers_3[i].ICANFault1;
                temp.ICANFault4_1 = history.Containers_4[i].ICANFault1;
                temp.ICANFault5_1 = history.Containers_5[i].ICANFault1;
                temp.ICANFault6_1 = history.Containers_6[i].ICANFault1;

                temp.ICANFault1_2 = history.Containers_1[i].ICANFault2;
                temp.ICANFault2_2 = history.Containers_2[i].ICANFault2;
                temp.ICANFault3_2 = history.Containers_3[i].ICANFault2;
                temp.ICANFault4_2 = history.Containers_4[i].ICANFault2;
                temp.ICANFault5_2 = history.Containers_5[i].ICANFault2;
                temp.ICANFault6_2 = history.Containers_6[i].ICANFault2;

                temp.OCANFault1_1 = history.Containers_1[i].OCANFault1;
                temp.OCANFault2_1 = history.Containers_2[i].OCANFault1;
                temp.OCANFault3_1 = history.Containers_3[i].OCANFault1;
                temp.OCANFault4_1 = history.Containers_4[i].OCANFault1;
                temp.OCANFault5_1 = history.Containers_5[i].OCANFault1;
                temp.OCANFault6_1 = history.Containers_6[i].OCANFault1;

                temp.OCANFault1_2 = history.Containers_1[i].OCANFault2;
                temp.OCANFault2_2 = history.Containers_2[i].OCANFault2;
                temp.OCANFault3_2 = history.Containers_3[i].OCANFault2;
                temp.OCANFault4_2 = history.Containers_4[i].OCANFault2;
                temp.OCANFault5_2 = history.Containers_5[i].OCANFault2;
                temp.OCANFault6_2 = history.Containers_6[i].OCANFault2;

                // 2018-12-28
                temp.exhaustFault1_1 = history.Containers_1[i].exhaustFault_1;
                temp.exhaustFault2_1 = history.Containers_2[i].exhaustFault_1;
                temp.exhaustFault3_1 = history.Containers_3[i].exhaustFault_1;
                temp.exhaustFault4_1 = history.Containers_4[i].exhaustFault_1;
                temp.exhaustFault5_1 = history.Containers_5[i].exhaustFault_1;
                temp.exhaustFault6_1 = history.Containers_6[i].exhaustFault_1;

                temp.keepPressureFault1_1 = history.Containers_1[i].keepPressureFault_1;
                temp.keepPressureFault2_1 = history.Containers_2[i].keepPressureFault_1;
                temp.keepPressureFault3_1 = history.Containers_3[i].keepPressureFault_1;
                temp.keepPressureFault4_1 = history.Containers_4[i].keepPressureFault_1;
                temp.keepPressureFault5_1 = history.Containers_5[i].keepPressureFault_1;
                temp.keepPressureFault6_1 = history.Containers_6[i].keepPressureFault_1;

                temp.chargingFault1_1 = history.Containers_1[i].chargingFault_1;
                temp.chargingFault2_1 = history.Containers_2[i].chargingFault_1;
                temp.chargingFault3_1 = history.Containers_3[i].chargingFault_1;
                temp.chargingFault4_1 = history.Containers_4[i].chargingFault_1;
                temp.chargingFault5_1 = history.Containers_5[i].chargingFault_1;
                temp.chargingFault6_1 = history.Containers_6[i].chargingFault_1;

                temp.connectionValveAirFillingFailure_1 = history.Containers_1[i].connectionValveAirFillingFailure;
                temp.connectionValveAirFillingFailure_2 = history.Containers_2[i].connectionValveAirFillingFailure;
                temp.connectionValveAirFillingFailure_3 = history.Containers_3[i].connectionValveAirFillingFailure;
                temp.connectionValveAirFillingFailure_4 = history.Containers_4[i].connectionValveAirFillingFailure;
                temp.connectionValveAirFillingFailure_5 = history.Containers_5[i].connectionValveAirFillingFailure;
                temp.connectionValveAirFillingFailure_6 = history.Containers_6[i].connectionValveAirFillingFailure;

                temp.exhuastFault1_2 = history.Containers_1[i].exhuastFault_2;
                temp.exhuastFault2_2 = history.Containers_2[i].exhuastFault_2;
                temp.exhuastFault3_2 = history.Containers_3[i].exhuastFault_2;
                temp.exhuastFault4_2 = history.Containers_4[i].exhuastFault_2;
                temp.exhuastFault5_2 = history.Containers_5[i].exhuastFault_2;
                temp.exhuastFault6_2 = history.Containers_6[i].exhuastFault_2;

                temp.keepPressureFault1_2 = history.Containers_1[i].keepPressureFault_2;
                temp.keepPressureFault2_2 = history.Containers_2[i].keepPressureFault_2;
                temp.keepPressureFault3_2 = history.Containers_3[i].keepPressureFault_2;
                temp.keepPressureFault4_2 = history.Containers_4[i].keepPressureFault_2;
                temp.keepPressureFault5_2 = history.Containers_5[i].keepPressureFault_2;
                temp.keepPressureFault6_2 = history.Containers_6[i].keepPressureFault_2;

                temp.chargingFault1_2 = history.Containers_1[i].chargingFault_2;
                temp.chargingFault2_2 = history.Containers_2[i].chargingFault_2;
                temp.chargingFault3_2 = history.Containers_3[i].chargingFault_2;
                temp.chargingFault4_2 = history.Containers_4[i].chargingFault_2;
                temp.chargingFault5_2 = history.Containers_5[i].chargingFault_2;
                temp.chargingFault6_2 = history.Containers_6[i].chargingFault_2;

                temp.connectionValveVentilationFailure_1 = history.Containers_1[i].connectionValveVentilationFailure;
                temp.connectionValveVentilationFailure_2 = history.Containers_2[i].connectionValveVentilationFailure;
                temp.connectionValveVentilationFailure_3 = history.Containers_3[i].connectionValveVentilationFailure;
                temp.connectionValveVentilationFailure_4 = history.Containers_4[i].connectionValveVentilationFailure;
                temp.connectionValveVentilationFailure_5 = history.Containers_5[i].connectionValveVentilationFailure;
                temp.connectionValveVentilationFailure_6 = history.Containers_6[i].connectionValveVentilationFailure;

                temp.VLDChargingFault_1 = history.Containers_1[i].VLDChargingFault;
                temp.VLDChargingFault_2 = history.Containers_2[i].VLDChargingFault;
                temp.VLDChargingFault_3 = history.Containers_3[i].VLDChargingFault;
                temp.VLDChargingFault_4 = history.Containers_4[i].VLDChargingFault;
                temp.VLDChargingFault_5 = history.Containers_5[i].VLDChargingFault;
                temp.VLDChargingFault_6 = history.Containers_6[i].VLDChargingFault;

                temp.VLDExhuastFault_1 = history.Containers_1[i].VLDExhuastFault;
                temp.VLDExhuastFault_2 = history.Containers_2[i].VLDExhuastFault;
                temp.VLDExhuastFault_3 = history.Containers_3[i].VLDExhuastFault;
                temp.VLDExhuastFault_4 = history.Containers_4[i].VLDExhuastFault;
                temp.VLDExhuastFault_5 = history.Containers_5[i].VLDExhuastFault;
                temp.VLDExhuastFault_6 = history.Containers_6[i].VLDExhuastFault;

                temp.VLDKeepPressureFault_1 = history.Containers_1[i].VLDKeepPressureFault;
                temp.VLDKeepPressureFault_2 = history.Containers_2[i].VLDKeepPressureFault;
                temp.VLDKeepPressureFault_3 = history.Containers_3[i].VLDKeepPressureFault;
                temp.VLDKeepPressureFault_4 = history.Containers_4[i].VLDKeepPressureFault;
                temp.VLDKeepPressureFault_5 = history.Containers_5[i].VLDKeepPressureFault;
                temp.VLDKeepPressureFault_6 = history.Containers_6[i].VLDKeepPressureFault;

                temp.exhaustFault1_1 = history.Containers_1[i].WSPExhuastFault_1;
                temp.exhaustFault2_1 = history.Containers_2[i].WSPExhuastFault_1;
                temp.exhaustFault3_1 = history.Containers_3[i].WSPExhuastFault_1;
                temp.exhaustFault4_1 = history.Containers_4[i].WSPExhuastFault_1;
                temp.exhaustFault5_1 = history.Containers_5[i].WSPExhuastFault_1;
                temp.exhaustFault6_1 = history.Containers_6[i].WSPExhuastFault_1;

                temp.WSPChargingFault1_1 = history.Containers_1[i].WSPChargingFault_1;
                temp.WSPChargingFault2_1 = history.Containers_2[i].WSPChargingFault_1;
                temp.WSPChargingFault3_1 = history.Containers_3[i].WSPChargingFault_1;
                temp.WSPChargingFault4_1 = history.Containers_4[i].WSPChargingFault_1;
                temp.WSPChargingFault5_1 = history.Containers_5[i].WSPChargingFault_1;
                temp.WSPChargingFault6_1 = history.Containers_6[i].WSPChargingFault_1;

                temp.exhuastFault1_2 = history.Containers_1[i].WSPExhuastFault_2;
                temp.exhuastFault2_2 = history.Containers_2[i].WSPExhuastFault_2;
                temp.exhuastFault3_2 = history.Containers_3[i].WSPExhuastFault_2;
                temp.exhuastFault4_2 = history.Containers_4[i].WSPExhuastFault_2;
                temp.exhuastFault5_2 = history.Containers_5[i].WSPExhuastFault_2;
                temp.exhuastFault6_2 = history.Containers_6[i].WSPExhuastFault_2;

                temp.WSPChargingFault1_2 = history.Containers_1[i].WSPChargingFault_2;
                temp.WSPChargingFault2_2 = history.Containers_2[i].WSPChargingFault_2;
                temp.WSPChargingFault3_2 = history.Containers_3[i].WSPChargingFault_2;
                temp.WSPChargingFault4_2 = history.Containers_4[i].WSPChargingFault_2;
                temp.WSPChargingFault5_2 = history.Containers_5[i].WSPChargingFault_2;
                temp.WSPChargingFault6_2 = history.Containers_6[i].WSPChargingFault_2;

                temp.WSPContinueExaustAirTimeOutFault1_1 = history.Containers_1[i].WSPContinueExaustAirTimeOutFault_1;
                temp.WSPContinueExaustAirTimeOutFault2_1 = history.Containers_2[i].WSPContinueExaustAirTimeOutFault_1;
                temp.WSPContinueExaustAirTimeOutFault3_1 = history.Containers_3[i].WSPContinueExaustAirTimeOutFault_1;
                temp.WSPContinueExaustAirTimeOutFault4_1 = history.Containers_4[i].WSPContinueExaustAirTimeOutFault_1;
                temp.WSPContinueExaustAirTimeOutFault5_1 = history.Containers_5[i].WSPContinueExaustAirTimeOutFault_1;
                temp.WSPContinueExaustAirTimeOutFault6_1 = history.Containers_6[i].WSPContinueExaustAirTimeOutFault_1;

                temp.WSPContinuousExhaustFailure1_2 = history.Containers_1[i].WSPContinuousExhaustFailure_2;
                temp.WSPContinuousExhaustFailure2_2 = history.Containers_2[i].WSPContinuousExhaustFailure_2;
                temp.WSPContinuousExhaustFailure3_2 = history.Containers_3[i].WSPContinuousExhaustFailure_2;
                temp.WSPContinuousExhaustFailure4_2 = history.Containers_4[i].WSPContinuousExhaustFailure_2;
                temp.WSPContinuousExhaustFailure5_2 = history.Containers_5[i].WSPContinuousExhaustFailure_2;
                temp.WSPContinuousExhaustFailure6_2 = history.Containers_6[i].WSPContinuousExhaustFailure_2;

                temp.WSPContinueKeepPressureTimeOutFault1_1 = history.Containers_1[i].WSPContinueKeepPressureTimeOutFault_1;
                temp.WSPContinueKeepPressureTimeOutFault2_1 = history.Containers_2[i].WSPContinueKeepPressureTimeOutFault_1;
                temp.WSPContinueKeepPressureTimeOutFault3_1 = history.Containers_3[i].WSPContinueKeepPressureTimeOutFault_1;
                temp.WSPContinueKeepPressureTimeOutFault4_1 = history.Containers_4[i].WSPContinueKeepPressureTimeOutFault_1;
                temp.WSPContinueKeepPressureTimeOutFault5_1 = history.Containers_5[i].WSPContinueKeepPressureTimeOutFault_1;
                temp.WSPContinueKeepPressureTimeOutFault6_1 = history.Containers_6[i].WSPContinueKeepPressureTimeOutFault_1;

                temp.WSPContinueKeepPressureTimeOutFault1_2 = history.Containers_1[i].WSPContinueKeepPressureTimeOutFault_2;
                temp.WSPContinueKeepPressureTimeOutFault2_2 = history.Containers_2[i].WSPContinueKeepPressureTimeOutFault_2;
                temp.WSPContinueKeepPressureTimeOutFault3_2 = history.Containers_3[i].WSPContinueKeepPressureTimeOutFault_2;
                temp.WSPContinueKeepPressureTimeOutFault4_2 = history.Containers_4[i].WSPContinueKeepPressureTimeOutFault_2;
                temp.WSPContinueKeepPressureTimeOutFault5_2 = history.Containers_5[i].WSPContinueKeepPressureTimeOutFault_2;
                temp.WSPContinueKeepPressureTimeOutFault6_2 = history.Containers_6[i].WSPContinueKeepPressureTimeOutFault_2;

                #endregion

                #region 防滑数据
                temp.SlipLv1_1 = history.Containers_1[i].SlipLvl1;
                temp.SlipLv2_1 = history.Containers_2[i].SlipLvl1;
                temp.SlipLv3_1 = history.Containers_3[i].SlipLvl1;
                temp.SlipLv4_1 = history.Containers_4[i].SlipLvl1;
                temp.SlipLv5_1 = history.Containers_5[i].SlipLvl1;
                temp.SlipLv6_1 = history.Containers_6[i].SlipLvl1;

                temp.SlipLv1_2 = history.Containers_1[i].SlipLvl2;
                temp.SlipLv2_2 = history.Containers_2[i].SlipLvl2;
                temp.SlipLv3_2 = history.Containers_3[i].SlipLvl2;
                temp.SlipLv4_2 = history.Containers_4[i].SlipLvl2;
                temp.SlipLv5_2 = history.Containers_5[i].SlipLvl2;
                temp.SlipLv6_2 = history.Containers_6[i].SlipLvl2;

                temp.AccValue1_1 = history.Containers_1[i].AccValue1;
                temp.AccValue2_1 = history.Containers_2[i].AccValue1;
                temp.AccValue3_1 = history.Containers_3[i].AccValue1;
                temp.AccValue4_1 = history.Containers_4[i].AccValue1;
                temp.AccValue5_1 = history.Containers_5[i].AccValue1;
                temp.AccValue6_1 = history.Containers_6[i].AccValue1;

                temp.AccValue1_2 = history.Containers_1[i].AccValue2;
                temp.AccValue2_2 = history.Containers_2[i].AccValue2;
                temp.AccValue3_2 = history.Containers_3[i].AccValue2;
                temp.AccValue4_2 = history.Containers_4[i].AccValue2;
                temp.AccValue5_2 = history.Containers_5[i].AccValue2;
                temp.AccValue6_2 = history.Containers_6[i].AccValue2;
                #endregion

                #region 暂时用不到的数据(先注释
                //temp.HardBrake = history.Containers_1[i].HardBrakeCmd;
                //temp.HoldBrakeRealease = history.Containers_1[i].HoldBrakeRealease;
                //temp.NotZeroSpeed = history.Containers_1[i].NotZeroSpeed;               
                //temp.MassValid_1 = history.Containers_1[i].MassSigValid;
                //temp.MassValid_2 = history.Containers_2[i].MassSigValid;
                //temp.MassValid_3 = history.Containers_3[i].MassSigValid;
                //temp.MassValid_4 = history.Containers_4[i].MassSigValid;
                //temp.MassValid_5 = history.Containers_5[i].MassSigValid;
                //temp.MassValid_6 = history.Containers_6[i].MassSigValid;
                //temp.AbTargetValue_1 = history.Containers_1[i].AbTargetValueAx1;
                //temp.AbTargetValue_2 = history.Containers_1[i].AbTargetValueAx2;
                //temp.AbTargetValue_3 = history.Containers_1[i].AbTargetValueAx3;
                //temp.AbTargetValue_4 = history.Containers_1[i].AbTargetValueAx4;
                //temp.AbTargetValue_5 = history.Containers_1[i].AbTargetValueAx5;
                //temp.AbTargetValue_6 = history.Containers_1[i].AbTargetValueAx6;
                //temp.NetFastBrake = history.Containers_1[i].NetFastBrakeCmd;
                //temp.WheelInputState = history.Containers_1[i].WheelInputState;
                //temp.HardDifferent = history.Containers_1[i].HardDifferent;
                //temp.CanASPActive = history.Containers_1[i].CanASPEnable;                
                //temp.SoftVersion = history.Containers_1[i].SoftVersion;
                #endregion

                #region 2020-1-6:空气制动目标值
                temp.AirBrakeTargetValueBy1_1 = history.Containers_1[i].abTargetValueAx1;
                temp.AirBrakeTargetValueBy1_2 = history.Containers_1[i].abTargetValueAx2;
                temp.AirBrakeTargetValueBy1_3 = history.Containers_1[i].abTargetValueAx3;
                temp.AirBrakeTargetValueBy1_4 = history.Containers_1[i].abTargetValueAx4;
                temp.AirBrakeTargetValueBy1_5 = history.Containers_1[i].abTargetValueAx5;
                temp.AirBrakeTargetValueBy1_6 = history.Containers_1[i].abTargetValueAx6;

                temp.AirBrakeTargetValueBy6_1 = history.Containers_6[i].abTargetValueAx1;
                temp.AirBrakeTargetValueBy6_2 = history.Containers_6[i].abTargetValueAx2;
                temp.AirBrakeTargetValueBy6_3 = history.Containers_6[i].abTargetValueAx3;
                temp.AirBrakeTargetValueBy6_4 = history.Containers_6[i].abTargetValueAx4;
                temp.AirBrakeTargetValueBy6_5 = history.Containers_6[i].abTargetValueAx5;
                temp.AirBrakeTargetValueBy6_6 = history.Containers_6[i].abTargetValueAx6;
                #endregion

                #endregion
                dataModelList.Add(temp);
                worker.ReportProgress(i);
            }
        }

        private void aftEvent(object sender, RoutedEventArgs e)
        {
            if (location + LINE_PER_TIME > history.Count)
            {
                MessageBox.Show("已到达尾页");
            }
            else
            {
                location += LINE_PER_TIME;
                GetData(location, location + LINE_PER_TIME);
                nowPage++;
                nowPageLbl.Content = nowPage;
            }
        }


        private void preEvent(object sender, RoutedEventArgs e)
        {
            if (location == 0)
            {
                MessageBox.Show("已到达首页");
            }
            else
            {
                location -= LINE_PER_TIME;
                GetData(location, location + LINE_PER_TIME);
                nowPage--;
                nowPageLbl.Content = nowPage;
            }
        }


        #endregion

        #region window methods

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
            dataModelList.Clear();
            this.Close();
        }

        private void historyDetail_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "history");
        }

        #endregion

        /// <summary>
        /// 列显示配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnConfigItem_Click(object sender, RoutedEventArgs e)
        {
            // 两个窗口之间相互动作，参考博文：https://blog.csdn.net/empty_android/article/details/79763501
            ConfigHistoryDataGrid configHistoryDataGrid = new ConfigHistoryDataGrid();
            configHistoryDataGrid.Activate();
            configHistoryDataGrid.Show();
            configHistoryDataGrid.updateMainwindowLabel += SubWindow_updateMainwindowLabel;
        }

        private string[] RemoveSelectElement(string msg)
        {
            string[] temp = msg.Split(' ');
            List<string> tempList = new List<string>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != "")
                {
                    tempList.Add(temp[i]);
                }
            }
            return tempList.ToArray();
        }

        private void SubWindow_updateMainwindowLabel(string labelContent)
        {
            //List<int> temp = new List<int>();
            //string[] strArray = RemoveSelectElement(labelContent);
            //for(int i = 0; i < strArray.Length; i++)
            //{
            //    int columnNum = GetColumnNum(historyList, strArray[i]);
            //    temp.Add(columnNum);
            //}
            //for(int i = 1; i < historyList.Columns.Count; i++)
            //{
            //    historyList.Columns[i].Visibility = Visibility.Collapsed;
            //}
            //for(int i = 0; i < temp.Count; i++)
            //{
            //    historyList.Columns[temp[i]].Visibility = Visibility.Visible;
            //}
            //int[] intArray = temp.ToArray();

        }


        private void showAnalog_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void showAnalog_Unchecked(object sender, RoutedEventArgs e)
        {

        }




        /// <summary>
        /// 获取用户选择进行搜索的时分秒
        /// </summary>
        public string GetUserChooseDateTime()
        {
            string YearMonthDayChoose = Convert.ToDateTime(DateChoose.SelectedDate).ToString("yyyy/MM/dd");
            string HourChoose = HourComboBox.SelectedItem.ToString();
            string MinuteChoose = MinuteComboBox.SelectedItem.ToString();
            string SecondChoose = SecondComboBox.SelectedItem.ToString();
            string FullSearchDateTime = YearMonthDayChoose + " " + HourChoose + ":" + MinuteChoose + ":" + SecondChoose;

            object[] temp = history.GetProperties();


            return FullSearchDateTime;


        }
        /// <summary>
        /// 点击历史窗口下搜索按钮会产生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryDeatilSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string SearchDateTime = GetUserChooseDateTime();
            bool bSearch = false;

            for (int i = 0; i < history.Count; i++)
            {
                if (history.Containers_1[i].dateTime.ToString("yyyy/MM/dd HH:mm:ss") == SearchDateTime)
                {
                    DateTimeSelectList.Add(i);// 获得符合搜索条件的所有id值
                    bSearch = true;
                    break;
                }
            }

            if (bSearch == true)
            {
                int[] DateTimeSelectArray = DateTimeSelectList.ToArray();
                JudgeSelectedIndex(DateTimeSelectArray[0] + 1);
                historyList.SelectedIndex = (DateTimeSelectArray[0] % LINE_PER_TIME);
                historyList.UpdateLayout();
                historyList.ScrollIntoView(historyList.SelectedItem);
                historyList.UpdateLayout();
                historyList.ScrollIntoView(historyList.SelectedItem);
            }
            else
            {
                MessageBox.Show("未找到相关搜索结果！");
            }

            DateTimeSelectList.Clear();


            #region 原来的代码
            //string[] DateTimeArray = DateTimeList.ToArray();

            //for (int i = 0; i < DateTimeArray.Length; i++)
            //{
            //    if (DateTimeArray[i] == SearchDateTime)
            //    {
            //        //MessageBox.Show("搜索结果：  " + i + "   行");
            //        DateTimeSelectList.Add(i);
            //        bSearch = true;
            //    }
            //}
            //if (bSearch == false)
            //{
            //    MessageBox.Show("未找到相关搜索结果!");
            //}

            //if (bSearch == true)// 找到了搜索结果
            //{
            //    int[] DateTimeSelectArray = DateTimeSelectList.ToArray();
            //    historyList.SelectedIndex = DateTimeSelectArray[0];


            //    //JudgeSelectedIndex(historyList.SelectedIndex);
            //    historyList.UpdateLayout();
            //    historyList.ScrollIntoView(historyList.SelectedItem);
            //    DateTimeSelectList.Clear();


            //}
            #endregion
        }

        /// <summary>
        /// 判断符合条件的第一个元素是否在本页面，不是的话得翻页
        /// </summary>
        private void JudgeSelectedIndex(int index)
        {
            int SearchPage;
            if ((index % LINE_PER_TIME) == 0)
            {
                SearchPage = index / LINE_PER_TIME;
            }
            else
            {
                SearchPage = (int)(index / LINE_PER_TIME) + 1;
            }

            if (SearchPage == nowPage) { }
            else if (SearchPage > nowPage)
            {
                int PrePage = SearchPage - nowPage; // 向后翻多少页
                location = location + (LINE_PER_TIME * PrePage);
                GetData(location, location + LINE_PER_TIME);
                nowPageLbl.Content = SearchPage;
                nowPage = SearchPage;
            }
            else // 向前翻多少页
            {
                int AfterPage = nowPage - SearchPage;
                location = location - (LINE_PER_TIME * AfterPage);
                GetData(location, location + LINE_PER_TIME);
                nowPageLbl.Content = SearchPage;
                nowPage = SearchPage;
            }
        }


        // 获取search框内输入的关键词
        //public string SearchItem { get => SearchTextBox.Text; set => SearchTextBox.Text = value; }
        //private void IDSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    int SearchID = int.Parse(SearchItem);
        //    bool bSearch = false;
        //    for(int i = 0; i < history.Count; i++)
        //    {
        //        if(history.ListID[i] == SearchID)
        //        {
        //            IDSelectList.Add(i);
        //            bSearch = true;
        //            break;
        //        }
        //    }
        //    if (bSearch == true)
        //    {
        //        int[] IDSelectListArray = IDSelectList.ToArray();
        //        JudgeSelectedIndex(IDSelectListArray[0] + 1);
        //        historyList.SelectedIndex = (IDSelectListArray[0] % LINE_PER_TIME);
        //        historyList.UpdateLayout();
        //        historyList.ScrollIntoView(historyList.SelectedItem);
        //        historyList.UpdateLayout();
        //        historyList.ScrollIntoView(historyList.SelectedItem);
        //    }
        //    else {
        //        MessageBox.Show("未找到相关搜索结果！");
        //    }

        //    IDSelectList.Clear();




        //    #region 原来的代码
        //    //for (int i = 0; i < IDArray.Length; i++)
        //    //{

        //    //    if (IDArray[i] == SearchID)
        //    //    {
        //    //        //MessageBox.Show("搜索结果：  " + i + "   行");
        //    //        IDSelectList.Add(i);
        //    //        IDSearchResultList.Add(i);
        //    //        bSearch = true;
        //    //        break;
        //    //    }
        //    //}
        //    //if (bSearch == false)
        //    //{
        //    //    MessageBox.Show("未找到相关搜索结果!");
        //    //}

        //    //if (bSearch == true)// 找到了搜索结果
        //    //{
        //    //    int[] IDSelectArray = IDSelectList.ToArray();
        //    //    historyList.SelectedIndex = IDSelectArray[0];
        //    //    historyList.UpdateLayout();
        //    //    historyList.ScrollIntoView(historyList.SelectedItem);
        //    //    IDSelectList.Clear();
        //    //}

        //    //// 加在最后的滚动条上的信息
        //    //int[] IDSearchResultArray = IDSearchResultList.ToArray();
        //    //for (int z = 0; z < IDSearchResultArray.Length; z++)
        //    //{
        //    //    int a = z + 1;
        //    //    SearchResultComboBox.Items.Add("搜索结果" + a + ":第" + IDSearchResultArray[z] + "行");
        //    //}
        //    //// 注：由于是每次读取100行数据，我怎么能在toolbar最左侧显示所有的搜索结果？
        //    #endregion
        //}

        private void JSONBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 显示全部列按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAllColumnsBtn_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < historyList.Columns.Count; i++)
            //{
            //    historyList.Columns[i].Visibility = Visibility.Visible;
            //}
        }

        private void exportToJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            history.ExportToJson();
        }
    }
}
