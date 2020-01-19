using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ParameterSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterSetWindow : Window
    {
       
        //存储comboBox数据的内部类
        private class CalibratedAxle
        {
            public string ID { get; set; }
            public string Number { get; set; }
        }
        public ParameterSetWindow()
        {
            InitializeComponent();
            initComboBox();
        }
        /// <summary>
        /// 初始化comboBox
        /// </summary>
        private void initComboBox()
        {
            int[] axleNo = { 1, 2, 3, 4, 5, 6, 7, 8 };
            ObservableCollection<CalibratedAxle> axleNoData = new ObservableCollection<CalibratedAxle>();
            for (int i = 0; i < axleNo.Length; i++)
            {
                axleNoData.Add(new CalibratedAxle()
                {
                    ID = "1",
                    Number = axleNo[i] + "",
                });
            }
            CalibratedAxleNo.comboBox.ItemsSource = axleNoData;

            int[] wheelSize = { 700, 710, 720, 730, 740, 750, 760, 770, 780, 790, 800 };
            ObservableCollection<CalibratedAxle> wheelSizeData = new ObservableCollection<CalibratedAxle>();
            for (int i = 0; i < wheelSize.Length; i++)
            {
                wheelSizeData.Add(new CalibratedAxle()
                {
                    ID = "1",
                    Number = wheelSize[i] + "",
                });
            }
            WheelSize.comboBox.ItemsSource = wheelSizeData;
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
           //this.CalibratedAxleNo.comboBox.selecte
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
        /// 点击关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}