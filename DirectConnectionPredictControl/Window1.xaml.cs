

using System.Windows;
using System.Windows.Media;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();




















        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            MyGrid.OpacityMask = this.Resources["ClosedBrush"] as LinearGradientBrush;
            System.Windows.Media.Animation.Storyboard std = this.Resources["ClosedStoryboard"] as System.Windows.Media.Animation.Storyboard;
            std.Completed += delegate { this.Close(); };

            std.Begin();
        }
    }
}
