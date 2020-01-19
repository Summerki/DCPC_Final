using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// 文件系统对象类型
    /// </summary>
    public enum FileSystemObjectType
    {
        Folder, File
    }

    public class FileSystemObjectViewModel : INotifyPropertyChanged
    {
        #region 静态成员

        /// <summary>
        /// 列举当前系统的所有磁盘目录
        /// </summary>
        /// <returns></returns>
        public static FileSystemObjectViewModel GetSystemDrives()
        {
            var top = new FileSystemObjectViewModel();
            top._Children = new ObservableCollection<FileSystemObjectViewModel>();
            foreach (var drv in DriveInfo.GetDrives())
            {
                top._Children.Add(new FileSystemObjectViewModel(drv.RootDirectory.FullName, drv.ToString(), FileSystemObjectType.Folder));
            }
            return top;
        }
        #endregion

        #region 构造函数


        /// <summary>
        /// 初始化一个标准ViewModel
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public FileSystemObjectViewModel(string path, string displayName, FileSystemObjectType type)
        {
            Path = path;
            DisplayName = displayName;
            Type = type;
            isSpecial = false;
            Initialize();
        }

        /// <summary>
        /// 初始化特殊ViewModel节点，用来代表一个文件夹拥有子成员
        /// </summary>
        private FileSystemObjectViewModel()
        {
            isSpecial = true;
        }

        #endregion

        #region 属性和字段
        bool isSpecial;

        public string Path { get; private set; }
        public string DisplayName { get; private set; }

        public FileSystemObjectType Type { get; private set; }
        public bool HasSpecialChild
        {
            get
            {
                return Children != null && Children.Count == 1 && Children[0].isSpecial;
            }
        }

        private ObservableCollection<FileSystemObjectViewModel> _Children;
        public ObservableCollection<FileSystemObjectViewModel> Children
        {
            get
            {
                return _Children;
            }
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnExpanded();
                }
            }
        }

        #endregion

        #region 私有方法


        /// <summary>
        /// 初始化：检查文件夹是否有子成员，有的话加入特殊节点
        /// </summary>
        void Initialize()
        {
            if (Type == FileSystemObjectType.Folder && CheckChildObject())
                AddSpecialChild();
        }

        /// <summary>
        /// 添加特殊特殊节点
        /// </summary>
        void AddSpecialChild()
        {
            _Children = new ObservableCollection<FileSystemObjectViewModel>();
            _Children.Add(new FileSystemObjectViewModel());
        }
        /// <summary>
        /// 移除特树节点
        /// </summary>
        void RemoveSpecialChild()
        {
            _Children.RemoveAt(0);
        }

        /// <summary>
        /// 枚举子文件，注意某些文件夹可能无法访问，所以用try, catch
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSubFiles()
        {
            try
            {
                return Directory.EnumerateFiles(Path);
            }
            catch
            {
                return new string[0];
            }
        }
        /// <summary>
        /// 枚举子文件夹，注意某些文件夹可能无法访问，所以用try, catch
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSubDirs()
        {
            try
            {
                return Directory.EnumerateDirectories(Path);
            }
            catch
            {
                return new string[0];
            }
        }



        #endregion

        #region 派生可改写方法

        /// <summary>
        /// 节点被展开后的操作
        /// </summary>
        protected virtual void OnExpanded()
        {
            //是否有特殊节点
            if (HasSpecialChild)
            {
                //将要展开的节点拥有没有列举的子成员（第一次打开）

                //我们需要移除特殊节点，并将子文件夹加入到Children中
                RemoveSpecialChild();

                foreach (var dir in GetSubDirs())
                    _Children.Add(new FileSystemObjectViewModel(dir, GetFileName(dir), FileSystemObjectType.Folder));
                foreach (var file in GetSubFiles())
                    _Children.Add(new FileSystemObjectViewModel(file, GetFileName(file), FileSystemObjectType.File));
            }
        }


        /// <summary>
        /// 检查当前文件夹是否有子成员（子文件夹或者子文件）
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckChildObject()
        {
            try
            {
                return Directory.EnumerateFileSystemEntries(Path).Any();
            }
            catch
            {
                return false;
            }
        }

        static string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }


        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }

}
