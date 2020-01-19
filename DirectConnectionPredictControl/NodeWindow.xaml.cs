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

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// NodeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NodeWindow : Window
    {
        private int lifeSigNode1;
        private int lifeSigNode2;
        private int lifeSigNode3;
        private int lifeSigNode4;
        private int lifeSigNode5;
        private int lifeSigNode6;

        private int preLifeSigNode1;
        private int preLifeSigNode2;
        private int preLifeSigNode3;
        private int preLifeSigNode4;
        private int preLifeSigNode5;
        private int preLifeSigNode6;

        private bool stateNode1;
        private bool stateNode2;
        private bool stateNode3;
        private bool stateNode4;
        private bool stateNode5;
        private bool stateNode6;

        private delegate void updateUI(bool stateNode1, bool stateNode2, bool stateNode3, bool stateNode4, bool stateNode5, bool stateNode6);
        private updateUI asyncUpdateUI;

        public NodeWindow()
        {
            InitializeComponent();
        }

        public NodeWindow(int lifeSigNode1, int lifeSigNode2, int lifeSigNode3, int lifeSigNode4, int lifeSigNode5, int lifeSigNode6)
        {
            this.lifeSigNode1 = lifeSigNode1;
            this.lifeSigNode2 = lifeSigNode2;
            this.lifeSigNode3 = lifeSigNode3;
            this.lifeSigNode4 = lifeSigNode4;
            this.lifeSigNode5 = lifeSigNode5;
            this.lifeSigNode6 = lifeSigNode6;
        }

        private void Init()
        {

        }

        private void UpdateUIHandler(bool stateNode1, bool stateNode2, bool stateNode3, bool stateNode4, bool stateNode5, bool stateNode6)
        {
            //节点按钮的状态
            node1Btn.Background = stateNode1 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            node2Btn.Background = stateNode2 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            node3Btn.Background = stateNode3 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            node4Btn.Background = stateNode4 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            node5Btn.Background = stateNode5 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            node6Btn.Background = stateNode6 == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

            //标识状态
            node1Tb.Text = stateNode1 == true ? "正常" : "故障";
            node2Tb.Text = stateNode2 == true ? "正常" : "故障";
            node3Tb.Text = stateNode3 == true ? "正常" : "故障";
            node4Tb.Text = stateNode4 == true ? "正常" : "故障";
            node5Tb.Text = stateNode5 == true ? "正常" : "故障";
            node6Tb.Text = stateNode6 == true ? "正常" : "故障";
        }

        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyNodeWindow_Loaded(object sender, RoutedEventArgs e)
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
        private void nodeMinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nodeClose_Click(object sender, RoutedEventArgs e)
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
        /// 获取所有节点的生命信号状态
        /// </summary>
        public void CheckAllNode()
        {
            stateNode1 = CheckNode(lifeSigNode1, preLifeSigNode1);
            stateNode2 = CheckNode(lifeSigNode2, preLifeSigNode2);
            stateNode3 = CheckNode(lifeSigNode3, preLifeSigNode3);
            stateNode4 = CheckNode(lifeSigNode4, preLifeSigNode4);
            stateNode5 = CheckNode(lifeSigNode5, preLifeSigNode5);
            stateNode6 = CheckNode(lifeSigNode6, preLifeSigNode6);
        }

        /// <summary>
        /// 检测单一个节点的生命信号状态
        /// </summary>
        /// <param name="lifeSigNode"></param>
        /// <param name="preLifeSigNode"></param>
        /// <returns></returns>
        public bool CheckNode(int lifeSigNode, int preLifeSigNode)
        {
            bool state = false;
            if (lifeSigNode == preLifeSigNode)
            {
                state = false;
            }
            else
            {
                state = true;
            }
            return state;
        }

        /// <summary>
        /// 更新所有节点的生命信号的状态
        /// </summary>
        /// <param name="lifeSigNode1"></param>
        /// <param name="lifeSigNode2"></param>
        /// <param name="lifeSigNode3"></param>
        /// <param name="lifeSigNode4"></param>
        /// <param name="lifeSigNode5"></param>
        /// <param name="lifeSigNode6"></param>
        public void UpdateNode(int lifeSigNode1, int lifeSigNode2, int lifeSigNode3, int lifeSigNode4, int lifeSigNode5, int lifeSigNode6)
        {
            preLifeSigNode1 = lifeSigNode1;
            preLifeSigNode2 = lifeSigNode2;
            preLifeSigNode3 = lifeSigNode3;
            preLifeSigNode4 = lifeSigNode4;
            preLifeSigNode5 = lifeSigNode5;
            preLifeSigNode6 = lifeSigNode6;
            CheckAllNode();
            if (asyncUpdateUI == null)
            {
                asyncUpdateUI = new updateUI(UpdateUIHandler);
            }
            asyncUpdateUI.Invoke(stateNode1, stateNode2, stateNode3, stateNode4, stateNode5, stateNode6);
        }
    }
}

