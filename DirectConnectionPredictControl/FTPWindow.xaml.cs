using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

/*
 * XiaoHu,Summerki:My_FTPDownloader开发日志
 * V1.0:在暑假来学校期间已经测试完毕
 * V2.0:搜索关键词：2018-9-12
 *      1. 修复芯片上登录FTP服务器后列表获取不完整问题
 *      2. 在登陆后的主界面增加一个“刷新”按钮
 *      3. 使用一个鸡贼的方法使得文件大小为 1KB 时下载这个文件进度条相关显示正确
 * V2.1:搜索关键词：2018-9-13
 *      1. 决定将上传按钮，暂停下载按钮，还有右键菜单中的重命名先隐藏掉，因为还没有做出来
 *      2. 阻止了没有选定目标时点击下载按钮时会发生的异常事件
 *      3. 优化在删除事件中删除成功后会弹出一个对话框显示某某文件已经删除成功
 * V2.2:搜索关键词：2018-9-19
 *      1. FTP客户端现在已经集成到DCPC软件之中，集成后出现了一个问题，打开FTP客户端登录成功之后
 *         mainwindow主界面会消失，这是因为我之前的FTP客户端逻辑有问题，所以进行了相应的
 *         修改
 *      2. 改完之后又出现了新的BUG：打开FTP客户端登录成功后主界面卡死在桌面，随后报异常
 *         于是将1.中的close()都改为hide(),现在已经没有问题了
 */

namespace DirectConnectionPredictControl
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FTPWindow : Window
    {
        FTPLogin ftpLogin = new FTPLogin();

        public static int MainWindow_Count = 1;

        //和下载相关的变量
        public BackgroundWorker downloadWorker;
        public string downloadInfo;
        public string downloadFileName;
        public string downloadFileFolder;
        public string localFileName;
        public string downloadFileSize;
        private DownLoadTaskDescribe downloadTask;

        private ManualResetEvent manualReset = new ManualResetEvent(true);


        System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
        public FTPWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            downloadWorker = new BackgroundWorker();
            downloadWorker.WorkerReportsProgress = true;
            downloadWorker.DoWork += DownloadWorker_DoWork;
            downloadWorker.ProgressChanged += DownloadWorker_ProgressChanged;
            downloadWorker.RunWorkerCompleted += DownloadWorker_RunWorkerCompleted;

            //bool类型，指示BackgroundWorker是否支持异步取消操作。
            //当该属性值为True是，将可以成功调用CancelAsync方法，否则将引发InvalidOperationException异常。 
            downloadWorker.WorkerSupportsCancellation = true;



            // 下载功能的实现
            menuItem.Header = "下载";
            //contextMenu.Items.Add(menuItem);
            contextMenu.Items.Insert(0, menuItem);// 插入，使 下载 按钮在第一个，感觉下载按钮就应该放在第一个
            menuItem.Click += MenuItem_Click;


        }
        

        /// <summary>
        /// 右击 下载 按钮的事件，弹出文件选择对话框保存位置和保存文件的重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            bool bDownLoad = true;// 2018-9-13:是否可以进行下载的标志位

            FileInfo fi = FTPListView.SelectedItem as FileInfo;
            try
            {
                downloadFileName = fi.FileName;
                downloadFileSize = fi.FileSize.Split(' ')[0];
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("请选择具体的文件进行下载");
                bDownLoad = false;
            }
            if (bDownLoad == true)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "所有文件（*.*）|*.*";
                sfd.FileName = sfd.FileName + downloadFileName;
                DialogResult dr = sfd.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    localFileName = System.IO.Path.GetFileName(sfd.FileName);
                    downloadFileFolder = System.IO.Path.GetDirectoryName(sfd.FileName);
                    try
                    {
                        string totalSize;
                        long total = 0L;
                        string remotePath = FTPLogin.ftp.showCurrentDir();
                        //FTPLogin.ftp.chDir(remotePath + "/" + downloadFileName);2018-9-28
                        FTPLogin.ftp.chDir(remotePath);
                        string[] currentFileList = FTPLogin.ftp.Dir("");
                        foreach (string item in currentFileList)
                        {
                            if (item.Equals(""))
                            {
                                continue;
                            }
                            string tempFileName = item.Substring(0, item.Length - 1);
                            total += FTPLogin.ftp.GetFileSize(tempFileName) / 1024;
                        }
                        totalSize = string.Format("{0:N0}", total);
                        downloadFileSize = totalSize;
                        downloadTask = new DownLoadTaskDescribe(downloadFileName, downloadFileSize, downloadFileFolder, "");
                        downloadWorker.RunWorkerAsync("multi file");
                        cancelBtn.IsEnabled = true;
                        //SolidColorBrush sc = new SolidColorBrush();
                        //sc.Color = Color.FromRgb(0x36, 0xBA, 0xFE);
                        //cancelBtn.Background = sc;
                    }
                    catch (Exception)
                    {
                        downloadTask = new DownLoadTaskDescribe(downloadFileName, downloadFileSize, downloadFileFolder, localFileName);
                        downloadWorker.RunWorkerAsync("single file");
                        cancelBtn.IsEnabled = true;
                        //SolidColorBrush sc = new SolidColorBrush();
                        //sc.Color = Color.FromRgb(0x36, 0xBA, 0xFE);
                        //cancelBtn.Background = sc;
                    }
                }
            }

        }


        #region 和Backgroundworker类操作有关的函数

        /// <summary>
        /// 点击 下载 选项之后弹出文件选择对话框选择保存位置和文件重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartDownLoadMenuItemWork(object sender, RoutedEventArgs e)
        {
            FileInfo fi = FTPListView.SelectedItem as FileInfo;
            downloadFileName = fi.FileName;
            downloadFileSize = fi.FileSize.Split(' ')[0];
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "所有文件（*.*）|*.*";
            sfd.FileName = sfd.FileName + downloadFileName;
            DialogResult dr = sfd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                localFileName = System.IO.Path.GetFileName(sfd.FileName);
                downloadFileFolder = System.IO.Path.GetDirectoryName(sfd.FileName);
                try
                {
                    string totalSize;
                    long total = 0L;
                    string remotePath = FTPLogin.ftp.showCurrentDir();
                    FTPLogin.ftp.chDir(remotePath + "/" + downloadFileName);

                    string[] currentFileList = FTPLogin.ftp.Dir("");
                    foreach (string item in currentFileList)
                    {
                        if (item.Equals(""))
                        {
                            continue;
                        }
                        string tempFileName = item.Substring(0, item.Length - 1);
                        total += FTPLogin.ftp.GetFileSize(tempFileName) / 1024;
                    }
                    totalSize = string.Format("{0:N0}", total);
                    downloadFileSize = totalSize;
                    downloadTask = new DownLoadTaskDescribe(downloadFileName, downloadFileSize, downloadFileFolder, "");
                    downloadWorker.RunWorkerAsync("multi file");
                    cancelBtn.IsEnabled = true;
                    //SolidColorBrush sc = new SolidColorBrush();
                    //sc.Color = Color.FromRgb(0x36, 0xBA, 0xFE);
                    //cancelBtn.Background = sc;
                }
                catch (Exception)
                {
                    downloadTask = new DownLoadTaskDescribe(downloadFileName, downloadFileSize, downloadFileFolder, localFileName);
                    downloadWorker.RunWorkerAsync("single file");
                    cancelBtn.IsEnabled = true;
                    //SolidColorBrush sc = new SolidColorBrush();
                    //sc.Color = Color.FromRgb(0x36, 0xBA, 0xFE);
                    //cancelBtn.Background = sc;
                }
            }
        }


        /// <summary>
        /// 后台任务完成或者取消时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                progressBar.Value = 0;
                System.Windows.MessageBox.Show("已经取消下载，记得删除下载部分哦~");
            }
            else
            {
                System.Windows.MessageBox.Show("下载成功");
            }

        }

        /// <summary>
        /// 下载任务进度报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //2018-9-12：将显示为1KB的文件报告进度功能单独拿出来，因为之前总是有错
            if (downloadFileSize == "1")
            {
                progressBar.Value = 100;
                progressValue.Text = "100%";
                process.Text = "1 KB/ 1 KB";
            }
            else
            {
                int test = e.ProgressPercentage / 1024;
                progressBar.Value = e.ProgressPercentage / 1024;
                progressValue.Text = e.ProgressPercentage / 1024 + "%";
                if (test >= 100)
                {
                    progressValue.Text = "100%";
                }
                process.Text = string.Format("{0:N0}", decimal.Parse(e.UserState.ToString()) / (1024)) + " KB / " + downloadFileSize + " KB";
                //slider.Value = e.ProgressPercentage;
            }


        }

        /// <summary>
        /// 后台异步下载的任务方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            BackgroundWorker worker = sender as BackgroundWorker;
            object arg = e.Argument;// e.Argument:获取一个值，表示异步操作的参数
            e.Result = DownloadWork(worker, e, downloadTask);
        }

        /// <summary>
        /// 具体的下载方法
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool DownloadWork(BackgroundWorker worker, DoWorkEventArgs e, DownLoadTaskDescribe task)
        {
            if (e.Argument.Equals("single file"))
            {
                return FTPLogin.ftp.DownLoadSingleFile(task.DownloadFileName, task.LocalFileFolder, task.LocalFileName, task.DownloadFileSize, worker, e);
            }
            else if (e.Argument.Equals("multi file"))
            {
                //FTPLogin.ftp.Get("", task.LocalFileFolder, task.DownloadFileSize, worker, e);
                return true;
            }
            else
            {
                //.PutSingleFile(task.DownloadFileName, worker, e);
                return true;
            }
            //return FTPLogin.ftp.DownLoadSingleFile(task.DownloadFileName, task.LocalFileFolder, task.LocalFileName,task.DownloadFileSize,worker,e );   
        }






        #endregion



        /// <summary>
        /// 初始化FTP文件列表数据
        /// </summary>
        /// <param name="fileListArray"></param>
        public void InitialFtpListViewData(string[] fileListArray)
        {
            ObservableCollection<FileInfo> fileInfo = new ObservableCollection<FileInfo>();
            FTPListView.ItemsSource = null;
            for (int i = 0; i < fileListArray.Length; i++)
            {
                if (fileListArray[i].Equals(""))
                {
                    continue;
                }
                string temp = fileListArray[i].Substring(0, fileListArray[i].Length - 1);// 为了去掉 \r

                string fileType;
                string fileSize;
                try
                {
                    fileType = TranslateFileType(System.IO.Path.GetExtension(temp));
                    if (fileType == null)
                    {
                        fileType = "文件夹";
                    }

                }
                catch (Exception)
                {
                    fileType = "文件夹";
                }
                try
                {
                    if (FTPLogin.ftp.GetFileSize(temp) / 1024.0 < 1.0)
                    {
                        fileSize = "1 KB";
                    }
                    else
                    {
                        fileSize = String.Format("{0:N0}", FTPLogin.ftp.GetFileSize(temp) / 1024) + " KB";
                    }
                }
                catch (Exception)
                {
                    fileSize = "";
                }
                fileInfo.Add(new FileInfo()
                {
                    FileName = temp,
                    FileSize = fileSize,
                    FileType = fileType
                });
                FTPListView.ItemsSource = fileInfo;
                //FTPListView.Items.Add(fileInfo);

            }
        }

        /// <summary>
        /// 转换文件类型
        /// </summary>
        /// <param name="originalType"></param>
        /// <returns></returns>
        private string TranslateFileType(string originalType)
        {
            string translateType = "文件";
            switch (originalType)
            {
                case ".txt":
                    translateType = "文本文件";
                    break;
                case ".TXT":
                    translateType = "文本文件";
                    break;
                case ".pdf":
                    translateType = "PDF文件";
                    break;
                case ".doc":
                    translateType = "word文档";
                    break;
                case ".log":
                    translateType = "日志文件";
                    break;
                case ".LOG":
                    translateType = "日志文件";
                    break;
                case ".bin":
                    translateType = "二进制文件";
                    break;
                case "":
                    translateType = "文件夹";
                    break;
                default:
                    translateType = "文件夹";
                    break;
            }
            return translateType;
        }


        private void login_Click(object sender, RoutedEventArgs e)
        {
            ftpLogin = new FTPLogin();
            ftpLogin.Show();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("CSU FTP客户端\n By XiaoHu\nVersion V2.1");
        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 暂停下载 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 2018-9-13：暂时注释掉暂停按钮点击事件
        //private void stopBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((String)stopBtn.Content == "暂停下载")
        //    {
        //        manualReset.Reset();//暂停当前线程的工作，发信号给waitOne方法，阻塞
        //        stopBtn.Content = "继续下载";
        //    }
        //    else
        //    {
        //        manualReset.Set();//继续某个线程的工作
        //        stopBtn.Content = "暂停下载";
        //    }

        //}

        /// <summary>
        /// 取消下载 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            downloadWorker.CancelAsync();// 请求取消的挂起的后台操作
        }

        /// <summary>
        /// 在listview右键中出现的 删除 选项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var items = this.FTPListView.SelectedItems;
            FileInfo fileInfo_items = (FileInfo)items[0];
            string b = System.IO.Path.GetExtension(fileInfo_items.FileName);
            if (b == "")
            {
                //MessageBox.Show("不允许删除文件夹！");
                try
                {
                    FTPLogin.ftp.RmDir(fileInfo_items.FileName);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("文件夹里面还有文件哦~");
                }

                string temp_1 = FTPLogin.ftp.showCurrentDir();
                FTPLogin.ftp.chDir(temp_1);
                string[] newCurrentListArray = FTPLogin.ftp.Dir(temp_1);
                FTPWindow mainWindow = CreateNewMainWindow();
                mainWindow.InitialFtpListViewData_2(newCurrentListArray);
                mainWindow.Show();
                int n = System.Windows.Application.Current.Windows.Count;
                for (int a = 0; a < n; a++)
                {
                    for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                    {
                        string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                        string x = System.Windows.Application.Current.Windows[i].Name;
                        //2018-9-19
                        string z = "MyWindow";
                        if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                    }
                }
            }
            else
            {
                FTPLogin.ftp.Delete(fileInfo_items.FileName);
                System.Windows.MessageBox.Show(fileInfo_items.FileName + " 删除成功！");
                // 删除完后刷新整个FTP listview列表
                string temp_1 = FTPLogin.ftp.showCurrentDir();
                FTPLogin.ftp.chDir(temp_1);
                string[] newCurrentListArray = FTPLogin.ftp.Dir(temp_1);
                FTPWindow mainWindow = CreateNewMainWindow();
                mainWindow.InitialFtpListViewData_2(newCurrentListArray);
                mainWindow.Show();
                int n = System.Windows.Application.Current.Windows.Count;
                for (int a = 0; a < n; a++)
                {
                    for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                    {
                        string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                        string x = System.Windows.Application.Current.Windows[i].Name;
                        //2018-9-19
                        string z = "MyWindow";
                        if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                    }
                }
            }
        }

        /// <summary>
        /// listview使用鼠标左键双击事件，用来双击文件夹进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTPListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("haha");
            var items = this.FTPListView.SelectedItems;

            //MessageBox.Show(items.Count.ToString()); 
            //MessageBox.Show(items.GetType().ToString());
            FileInfo fileInfo_items = (FileInfo)items[0];
            string b = System.IO.Path.GetExtension(fileInfo_items.FileName);


            if (System.IO.Path.GetExtension(fileInfo_items.FileName) == "")// 是文件夹的话
            {
                string[] newFileListArray;
                string temp_1 = FTPLogin.ftp.showCurrentDir();
                //fileListArray = ftp.Dir();
                if (temp_1 == "/")
                {
                    FTPLogin.ftp.chDir(temp_1 + fileInfo_items.FileName);
                    newFileListArray = FTPLogin.ftp.Dir(temp_1 + fileInfo_items.FileName);
                }
                else
                {
                    FTPLogin.ftp.chDir(temp_1 + @"/" + fileInfo_items.FileName);
                    newFileListArray = FTPLogin.ftp.Dir(temp_1 + @"/" + fileInfo_items.FileName);
                }
                //string[] newFileListArray = FTPLogin.ftp.Dir(temp_1  + fileInfo_items.FileName);
                FTPWindow mainWindow = CreateNewMainWindow();
                //MainWindow mainWindow_3 = new MainWindow();
                //mainWindow_3.Name = "mainWindow_3";
                mainWindow.InitialFtpListViewData_2(newFileListArray);
                mainWindow.Show();


                int n = System.Windows.Application.Current.Windows.Count;
                for (int a = 0; a < n; a++)
                {
                    for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                    {
                        string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                        string x = System.Windows.Application.Current.Windows[i].Name;
                        //2018-9-19
                        string z = "MyWindow";
                        if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                    }
                }
            }

        }

        #region 为了双击listview中的项目事件所做的一些函数

        /// <summary>
        /// 每次可以创建一个名字不同的窗口
        /// </summary>
        /// <returns></returns>
        public FTPWindow CreateNewMainWindow()
        {
            string temp = "mianWindow" + MainWindow_Count.ToString();
            FTPWindow mainWindow = new FTPWindow();
            //MainWindow mainWindow = new MainWindow();
            mainWindow.Name = temp;
            MainWindow_Count++;
            return mainWindow;
        }

        /// <summary>
        /// 返回一个字符串中最后出现 \ 的位置函数
        /// </summary>
        /// <returns></returns>
        public int return_LastLocation(string strLocation)
        {
            int int_LastLocation = strLocation.LastIndexOf(@"/");
            return int_LastLocation;
        }

        /// <summary>
        /// 在双击listview中的某一项时新窗口的listview的初始化函数
        /// </summary>
        /// <param name="fileListArray"></param>
        public void InitialFtpListViewData_2(string[] fileListArray)
        {
            ObservableCollection<FileInfo> fileInfo = new ObservableCollection<FileInfo>();
            FTPListView.ItemsSource = null;
            for (int i = 0; i < fileListArray.Length; i++)
            {
                if (fileListArray[i].Equals(""))
                {
                    continue;
                }
                string temp_1 = fileListArray[i].Substring(0, fileListArray[i].Length - 1);// 为了去掉 \r
                int int_LastLocation = return_LastLocation(temp_1);
                string temp = temp_1.Substring(int_LastLocation + 1, temp_1.Length - int_LastLocation - 1);

                string fileType;
                string fileSize;
                try
                {
                    fileType = TranslateFileType(System.IO.Path.GetExtension(temp));
                    if (fileType == null)
                    {
                        fileType = "文件夹";
                    }

                }
                catch (Exception)
                {
                    fileType = "文件夹";
                }
                try
                {
                    if (FTPLogin.ftp.GetFileSize(temp) / 1024.0 < 1.0)
                    {
                        fileSize = "1 KB";
                    }
                    else
                    {
                        fileSize = String.Format("{0:N0}", FTPLogin.ftp.GetFileSize(temp) / 1024) + " KB";
                    }
                }
                catch (Exception)
                {
                    fileSize = "";
                }
                fileInfo.Add(new FileInfo()
                {
                    FileName = temp,
                    FileSize = fileSize,
                    FileType = fileType
                });
                FTPListView.ItemsSource = fileInfo;
                //FTPListView.Items.Add(fileInfo);

            }
        }

        #endregion

        /// <summary>
        /// 主界面点击 返回上层目录 按钮事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormerDir_Click(object sender, RoutedEventArgs e)
        {
            string[] FormerListArray;
            string current_Path;

            //这里是为了解决用户在没有登录的情况下点击 返回上层目录 按钮出现的异常
            try
            {
                current_Path = FTPLogin.ftp.showCurrentDir();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("请先登录FTP服务器！");
                current_Path = null;
            }
            if (current_Path == null)
            {
                return;
            }


            if (current_Path == @"/")
            {
                System.Windows.MessageBox.Show("已经到达FTP服务器的根目录！");
            }
            else
            {
                int index1 = current_Path.LastIndexOf(@"/");

                // 如果查找最后出现 / 的位置是0，说明是在根目录下一层级，那么直接返回根目录即可    如： /xxxx
                if (index1 == 0)
                {
                    FTPLogin.ftp.chDir(@"/");
                    FormerListArray = FTPLogin.ftp.Dir(@"/");
                    FTPWindow mainWindow = CreateNewMainWindow();
                    //MainWindow mainWindow_3 = new MainWindow();
                    //mainWindow_3.Name = "mainWindow_3";
                    mainWindow.InitialFtpListViewData_2(FormerListArray);
                    mainWindow.Show();


                    int n = System.Windows.Application.Current.Windows.Count;
                    for (int a = 0; a < n; a++)
                    {
                        for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                        {
                            string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                            string x = System.Windows.Application.Current.Windows[i].Name;
                            //2018-9-19
                            string z = "MyWindow";
                            if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                        }
                    }
                }
                // 这里就是至少从根目录进去两个层级或以上的情况   如： /xxxx/XXXXX
                else
                {
                    string formerPath = current_Path.Substring(0, index1); // 取出上一层级的目录
                    FTPLogin.ftp.chDir(formerPath);
                    FormerListArray = FTPLogin.ftp.Dir(formerPath);
                    FTPWindow mainWindow = CreateNewMainWindow();
                    //MainWindow mainWindow_3 = new MainWindow();
                    //mainWindow_3.Name = "mainWindow_3";
                    mainWindow.InitialFtpListViewData_2(FormerListArray);
                    mainWindow.Show();


                    int n = System.Windows.Application.Current.Windows.Count;
                    for (int a = 0; a < n; a++)
                    {
                        for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                        {
                            string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                            string x = System.Windows.Application.Current.Windows[i].Name;
                            //2018-9-19
                            string z = "MyWindow";
                            if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                        }
                    }
                }

            }


        }

        /// <summary>
        /// 2018-9-12：主界面点击 刷新 按钮发生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            string[] FormerListArray;
            string current_Path;

            //这里是为了解决用户在没有登录的情况下点击 返回上层目录 按钮出现的异常
            try
            {
                current_Path = FTPLogin.ftp.showCurrentDir();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("请先登录FTP服务器！");
                current_Path = null;
            }
            if (current_Path == null)
            {
                return;
            }


            FTPLogin.ftp.chDir(current_Path);
            FormerListArray = FTPLogin.ftp.Dir(current_Path);
            FTPWindow mainWindow = CreateNewMainWindow();
            //MainWindow mainWindow_3 = new MainWindow();
            //mainWindow_3.Name = "mainWindow_3";
            mainWindow.InitialFtpListViewData_2(FormerListArray);
            mainWindow.Show();


            int n = System.Windows.Application.Current.Windows.Count;
            for (int a = 0; a < n; a++)
            {
                for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
                {
                    string y = "mianWindow" + (MainWindow_Count - 1).ToString();
                    string x = System.Windows.Application.Current.Windows[i].Name;
                    //2018-9-19
                    string z = "MyWindow";
                    if ((x != y) && (x != z)) System.Windows.Application.Current.Windows[i].Hide();
                }
            }
        }

        /// <summary>
        /// 在listview中右键 重命名 选项的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameItem_Click(object sender, RoutedEventArgs e)
        {
            var items = this.FTPListView.SelectedItems;
            FileInfo fileInfo_items = (FileInfo)items[0];
            ReNameWindow r = new ReNameWindow();
            r.Show();
        }


    }
}
