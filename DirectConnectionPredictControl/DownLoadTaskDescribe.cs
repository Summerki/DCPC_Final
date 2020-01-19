using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// 用来描述下载任务的一个类
    /// </summary>
    class DownLoadTaskDescribe
    {
        private string downloadFileName;
        private long downloadFileSize;
        private string localFileFolder;
        private string localFileName;

        /// <summary>
        /// 重写构造函数
        /// </summary>
        /// <param name="downloadFileName">下载文件名称</param>
        /// <param name="downloadFileSize">下载的文件的大小</param>
        /// <param name="localFileFolder">存放的本地文件夹</param>
        /// <param name="localFileName">存放的本地文件名称</param>
        public DownLoadTaskDescribe(string downloadFileName, string downloadFileSize, string localFileFolder, string localFileName)
        {
            this.DownloadFileName = downloadFileName;
            // this.DownloadFileSize = long.Parse(downloadFileSize.Remove(downloadFileSize.IndexOf(','),1));
            this.downloadFileSize = long.Parse(downloadFileSize.Replace(",", ""));
            this.LocalFileFolder = localFileFolder;
            this.LocalFileName = localFileName;
        }

        public string DownloadFileName { get => downloadFileName; set => downloadFileName = value; }
        public long DownloadFileSize { get => downloadFileSize; set => downloadFileSize = value; }
        public string LocalFileFolder { get => localFileFolder; set => localFileFolder = value; }
        public string LocalFileName { get => localFileName; set => localFileName = value; }
    }
}
