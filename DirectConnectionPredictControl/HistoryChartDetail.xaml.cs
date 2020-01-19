using DirectConnectionPredictControl.CommenTool;
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
    /// HistoryChartDetail.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryChartDetail : Window
    {
        public HistoryChartDetail()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            //Utils.htmlPath = Utils.htmlPath.Replace("//", "/");
            string cefHtmlPath = "file:///" + Utils.htmlPath;
            
            bro.Address = cefHtmlPath;
        }
    }
}
