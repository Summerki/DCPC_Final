using DirectConnectionPredictControl.CommenTool;
using DirectConnectionPredictControl.IO;
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
    /// CanDownload.xaml 的交互逻辑
    /// </summary>
    public partial class CanDownload : Window
    {
        private CanHelper canHelper;
        private FileBuilding fileBuilding;
        private string fileName;
        private List<byte[]> transData = null;

        private class BoundRate
        {
            public string ID { get; set; }
            public string Number { get; set; }
        }
        public CanDownload()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitCombox();
        }

        /// <summary>
        /// 初始化波特率选择器
        /// </summary>
        private void InitCombox()
        {
            int[] boundRates = { 5, 10, 20, 40, 50, 80, 100, 125, 200, 250, 400, 500, 666, 800, 1000 };
            ObservableCollection<BoundRate> bounds = new ObservableCollection<BoundRate>();
            for (int i = 0; i < boundRates.Length; i++)
            {
                bounds.Add(new BoundRate
                {
                    ID = "1",
                    Number = boundRates[i] + " kbps",
                });
            }
            boundRateCbx.comboBox.ItemsSource = bounds;
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗口d
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancelDownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 选择要打开下载的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "*.hex(二进制文件)|.hex|*.*(所有文件)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                //Console.WriteLine(fileName);
            }
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startDownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (transData == null)
            {
                MessageBox.Show("下载文件为空", "文件读取错误", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            else
            {
                Send();
            }
        }

        /// <summary>
        /// 发送逻辑
        /// </summary>
        private void Send()
        {
            canHelper = new CanHelper();
            for (int i = 0; i < transData.Count; i++)
            {
                canHelper.Send(transData[i]);
            }
        }

        /// <summary>
        /// 窗口拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
