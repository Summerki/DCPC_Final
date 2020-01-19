using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// FTPLogin.xaml 的交互逻辑
    /// </summary>
    public partial class FTPLogin : Window
    {

        public static FTP ftp { get; set; }
        /// <summary>
        /// 判断LoginBtn按钮的正确与否
        /// </summary>
        

        public   string[] fileListArray { get; set; }

        

        private string ftpRemoteIp;
        private string ftpRemotePort;
        private string ftpRemoteUser;
        private string ftpRemotePass;

        public string FTPRemoteIp { get => remoteIP.Text; set => remoteIP.Text = value; }
        public string FTPRemotePort { get => remotePort.Text; set => remotePort.Text = value; }
        public string FTPRemoteUser { get => loginUser.Text; set => loginUser.Text = value; }
        public string FTPRemotePass { get => loginPassword.Password; set => loginPassword.Password = value; }

       



        public FTPLogin()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            
        }


        






        /// <summary>
        /// FTP下载器登录界面的登录按钮触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            ftp = new FTP(FTPRemoteIp, FTPRemotePort, FTPRemoteUser, FTPRemotePass);
            if (ftp.bConnected == true)
            {
                MessageBox.Show("登录FTP服务器成功！");
                this.Close();
                
                fileListArray =  ftp.Dir(@"\");
                FTPWindow mainWindow_2 = new FTPWindow();
                mainWindow_2.Name = "mainWindow_2";
                mainWindow_2.InitialFtpListViewData(fileListArray);
                mainWindow_2.Show();

                // 上面创建了一个新的MainWindow，名字叫mainWindow_2，下面的代码是将把名字不是mainWindow_2的窗口全都关闭掉
                // 这里卡了我好久...
                int n = Application.Current.Windows.Count;
                for (int a = 0; a < n; a++)
                {
                    for (int i = 0; i < Application.Current.Windows.Count; i++)
                    {
                        string x = Application.Current.Windows[i].Name;
                        //2018-9-19
                        string z = "MyWindow";
                        if ((x != "mainWindow_2") && (x != z)) Application.Current.Windows[i].Hide();
                    }
                }

            }
            else
            {
                MessageBox.Show("登录失败，请重试！");
            }
            

        }

        

        /// <summary>
        /// FTP下载器登录界面的取消按钮触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }
    }
}
