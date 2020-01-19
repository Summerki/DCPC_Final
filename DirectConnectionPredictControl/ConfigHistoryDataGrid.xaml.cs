using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectConnectionPredictControl
{
    public static class CommonList
    {
        public static List<string> SelectList = new List<string>();
        public static List<string> analogDataList = new List<string>();
        public static string ss = "ss";
    }

    /// <summary>
    /// ConfigHistoryDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigHistoryDataGrid : Window
    {
        public ConfigHistoryDataGrid()
        {
            InitializeComponent();
            ComboBox1.DataContext = new MainWindowViewModel("AnalogData");
            ComboBox2.DataContext = new MainWindowViewModel("DigitalInput");
            ComboBox3.DataContext = new MainWindowViewModel("DigitalOutput");
            ComboBox4.DataContext = new MainWindowViewModel("FaultData");
            ComboBox5.DataContext = new MainWindowViewModel("AntiskidData");
            
        }

        public delegate void UpdateMainwindowLabel(string labelContent);
        public event UpdateMainwindowLabel updateMainwindowLabel;
        //public static string[] strArray;

        private void showSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            #region 废弃
            //HashSet<string> hs1 = new HashSet<string>(CommonList.SelectList);
            //HashSet<string> hs2 = new HashSet<string>(CommonList.analogDataList);
            //CommonList.SelectList = hs1.ToList();
            //CommonList.analogDataList = hs2.ToList();
            //List<string> Result = CommonList.SelectList.Union(CommonList.analogDataList).ToList<string>();
            //string[] temp = Result.ToArray();

            //for(int i = 0; i < temp.Length; i++)
            //{
            //    MessageBox.Show(temp[i]);
            //}
            //CommonList.SelectList.Clear();
            //CommonList.analogDataList.Clear();
            #endregion
            
            if (updateMainwindowLabel != null)
            {
                string chooseString = ComboBox1.Text + " " + ComboBox2.Text + " " + ComboBox3.Text + " " + ComboBox4.Text + " " + ComboBox5.Text;
                updateMainwindowLabel(chooseString);
            }
        }
    }

    public class MainWindowViewModel : ObservableObject
    {
        private string _selectedText = string.Empty;

        public string SelectedText
        {
            get
            {
                return _selectedText;
            }
            set
            {
                if (_selectedText != value)
                {
                    _selectedText = value;

                    RaisePropertyChanged("SelectedText");
                }
            }
        }

        private ObservableCollection<BookEx> _books;


        public ObservableCollection<BookEx> BookExs
        {
            get
            {
                if (_books == null)
                {
                    _books = new ObservableCollection<BookEx>();

                    _books.CollectionChanged += (sender, e) =>
                    {
                        if (e.OldItems != null)
                        {
                            foreach (BookEx bookEx in e.OldItems)
                            {
                                bookEx.PropertyChanged -= ItemPropertyChanged;
                            }
                        }

                        if (e.NewItems != null)
                        {
                            foreach (BookEx bookEx in e.NewItems)
                            {
                                bookEx.PropertyChanged += ItemPropertyChanged;
                            }
                        }
                    };
                }

                return _books;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                BookEx bookEx = sender as BookEx;

                if (bookEx != null)
                {
                    IEnumerable<BookEx> bookExs = BookExs.Where(b => b.IsChecked == true);


                    StringBuilder builder = new StringBuilder();

                    foreach (BookEx item in bookExs)
                    {

                        //if (item.Book != null)
                        //{
                        //    if (bClear == true)
                        //    {
                        //        CommonList.SelectList.Clear();
                        //        bClear = false;
                        //    }
                        //    CommonList.SelectList.Add(item.Book.Name);
                        //}
                        //if (item.AnalogDataBook != null)
                        //{
                        //    if (bClear == true)
                        //    {
                        //        CommonList.analogDataList.Clear();
                        //        bClear = false;
                        //    }
                        //    CommonList.analogDataList.Add(item.AnalogDataBook.AnalogData);
                        //}

                        if (item.Book != null)
                        {
                            builder.Append(item.Book.Name + " ");
                            //CommonList.mess_1 = CommonList.builder_1 == null ? string.Empty : CommonList.builder_1.ToString();
                        }
                        if (item.AnalogDataClass != null)
                        {
                            builder.Append(item.AnalogDataClass.AnalogData + " ");
                        }
                        if (item.DigitalInputClass != null)
                        {
                            builder.Append(item.DigitalInputClass.DigitalInput + " ");                            
                        }
                        if (item.DigitalOutputClass != null)
                        {
                            builder.Append(item.DigitalOutputClass.DigitalOutput + " ");                           
                        }
                        if (item.FaultDataClass != null)
                        {
                            builder.Append(item.FaultDataClass.FaultData + " ");                            
                        }
                        if (item.AntiskidDataClass != null)
                        {
                            builder.Append(item.AntiskidDataClass.AntiskidData + " ");                           
                        }

                    }

                    SelectedText = builder == null ? string.Empty : builder.ToString();
                    //SelectedText = builder_2 == null ? string.Empty : builder_2.ToString();



                }
            }
        }

        public MainWindowViewModel(string msg)
        {
            if (msg == "AnalogData")
            {
                for(int i = 0; i < HistoryDetail.dataGridHeaderName_1.Count; i++)
                {
                    BookExs.Add(new BookEx(new AnalogDataClass() { AnalogData = HistoryDetail.dataGridHeaderName_1[i] }));
                }
            }
            if (msg == "DigitalInput")
            {
                for (int i = 0; i < HistoryDetail.dataGridHeaderName_2.Count; i++)
                {
                    BookExs.Add(new BookEx(new DigitalInputClass() { DigitalInput = HistoryDetail.dataGridHeaderName_2[i] }));
                }
            }
            if(msg == "DigitalOutput")
            {
                for (int i = 0; i < HistoryDetail.dataGridHeaderName_3.Count; i++)
                {
                    BookExs.Add(new BookEx(new DigitalOutputClass() { DigitalOutput = HistoryDetail.dataGridHeaderName_3[i] }));
                }
            }
            if(msg == "FaultData")
            {
                for (int i = 0; i < HistoryDetail.dataGridHeaderName_4.Count; i++)
                {
                    BookExs.Add(new BookEx(new FaultDataClass() { FaultData = HistoryDetail.dataGridHeaderName_4[i] }));
                }
            }
            if(msg == "AntiskidData")
            {
                for (int i = 0; i < HistoryDetail.dataGridHeaderName_5.Count; i++)
                {
                    BookExs.Add(new BookEx(new AntiskidDataClass() { AntiskidData = HistoryDetail.dataGridHeaderName_5[i] }));
                }
            }

        }


    }

    public class Book
    {
        public string Name { get; set; }
    }

    public class AnalogDataClass
    {
        public string AnalogData { get; set; }
    }

    public class DigitalInputClass
    {
        public string DigitalInput { get; set; }
    }
    public class DigitalOutputClass
    {
        public string DigitalOutput { get; set; }
    }
    public class FaultDataClass
    {
        public string FaultData { get; set; }
    }
    public class AntiskidDataClass
    {
        public string AntiskidData { get; set; }
    }


    public class BookEx : ObservableObject
    {
        public Book Book { get; private set; }
        public AnalogDataClass AnalogDataClass { get; set; }
        public DigitalInputClass DigitalInputClass { get; set; }
        public DigitalOutputClass DigitalOutputClass { get; set; }
        public FaultDataClass FaultDataClass { get; set; }
        public AntiskidDataClass AntiskidDataClass { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;

                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        public BookEx(Book book)
        {
            Book = book;
        }
        public BookEx(AnalogDataClass analogDataClass)
        {
            AnalogDataClass = analogDataClass;
        }
        public BookEx(DigitalInputClass digitalInputClass)
        {
            DigitalInputClass = digitalInputClass;
        }
        public BookEx(DigitalOutputClass digitalOutputClass)
        {
            DigitalOutputClass = digitalOutputClass;
        }
        public BookEx(FaultDataClass faultDataClass)
        {
            FaultDataClass = faultDataClass;
        }
        public BookEx(AntiskidDataClass antiskidDataClass)
        {
            AntiskidDataClass = antiskidDataClass;
        }
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
