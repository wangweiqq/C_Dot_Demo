using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace WpfDrawImage
{
    public class CPoint{
        double x;
        double y;
        double z;

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public double Z
        {
            get
            {
                return z;
            }

            set
            {
                z = value;
            }
        }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>1
    public partial class MainWindow : Window
    {
        XElement root;
        public MainWindow()
        {
            InitializeComponent();
            root = XElement.Load("ConfigDraw.xml");
            //ReDraw();
        }
        object ObjectBindingData(string groupid) {
                switch (groupid)
                {
                    case "id_table1":
                        ObservableCollection<CPoint> list = new ObservableCollection<CPoint>() { new CPoint() { X=2.5,Y=3.6,Z=4.8},
                    new CPoint() { X=3.5,Y=4.6,Z=5.8},
                    new CPoint() { X=2.6,Y=3.7,Z=4.9}};
                        return list;
                    case "id_table2":
                        return "测试矩形框1";
                }
            return null;
        }
        void ClearCanvas() {
            int count = canvas.Children.Count;
            for (int i = count - 1; i > 0; --i) {
                canvas.Children.RemoveAt(i);
            }
        }
        void SetCanvasImage(string imgid) {
            XElement workimg = (from imgtag in root.Elements("WorkImage")
                                where imgtag.Attribute("id").Value == imgid
                                select imgtag).ElementAt(0);
            string imgpath = workimg.Attribute("ImgPath").Value;
            Uri uri = new Uri(System.Environment.CurrentDirectory + imgpath, UriKind.Absolute);
            BitmapImage img = new BitmapImage(uri);
            imgFloor.Source = img;
            imgFloor.Width = img.PixelWidth;
            imgFloor.Height = img.PixelHeight;
        }
        void AppendObject(string imgid) {
            XElement workimg = (from imgtag in root.Elements("WorkImage")
                                where imgtag.Attribute("id").Value == imgid
                                select imgtag).ElementAt(0);
            IEnumerable<XElement> group1 = from g in workimg.Elements("Group")
                                           select g;
            foreach (XElement g in group1) {
                AppendXGroup(g);
            }
        }
        void AppendObject(string imgid, string groupid) {

            XElement workimg = (from imgtag in root.Elements("WorkImage")
                               where imgtag.Attribute("id").Value == imgid
                               select imgtag).ElementAt(0);
            IEnumerable<XElement> group1 = from g in workimg.Elements("Group")
                                           where g.Attribute("id").Value == groupid
                                           select g;
            XElement xgroup = group1.ElementAt(0);
            AppendXGroup(xgroup);
        }
        void AppendXGroup(XElement xgroup) {
            string datactl = xgroup.Attribute("DataControl").Value;
            string groupid = xgroup.Attribute("id").Value;
            IEnumerable<XElement> ctl = xgroup.Elements();
            foreach (XElement item in ctl)
            {
                string str = item.ToString();
                var obj = (FrameworkElement)XamlReader.Parse(str);
                canvas.Children.Add(obj);
                if (obj.Name == datactl)
                {
                    obj.DataContext = ObjectBindingData(groupid);
                }
            }
        }
        void ReDraw()
        {
            //Uri uri = new Uri(System.Environment.CurrentDirectory + "/img/bk.jpg", UriKind.Absolute);
            //BitmapImage img = new BitmapImage(uri);
            //imgFloor.Source = img;
            //imgFloor.Width = img.PixelWidth;
            //imgFloor.Height = img.PixelHeight;

            //ObservableCollection<CPoint> list = new ObservableCollection<CPoint>() { new CPoint() { X=2.5,Y=3.6,Z=4.8},
            //new CPoint() { X=3.5,Y=4.6,Z=5.8},
            //new CPoint() { X=2.6,Y=3.7,Z=4.9}};

            //dataGrid.DataContext = list;
            //Label1.DataContext = list;//"测试矩形框1";
            ////Label1.Text = "测试矩形框1";

            
            //XElement workimg = root.Element("WorkImage");
            //string imgpath = workimg.Attribute("ImgPath").Value;
            //Uri uri = new Uri(System.Environment.CurrentDirectory + imgpath, UriKind.Absolute);
            //BitmapImage img = new BitmapImage(uri);
            //imgFloor.Source = img;
            //imgFloor.Width = img.PixelWidth;
            //imgFloor.Height = img.PixelHeight;

            //IEnumerable<XElement> group1 = from g in workimg.Elements("Group")
            //                               where g.Attribute("id").Value == "id_table1"
            //                               select g;
            //XElement xgroup = group1.ElementAt(0);
            //string datactl = xgroup.Attribute("DataControl").Value;

            //IEnumerable<XElement> ctl = xgroup.Elements();
            //foreach (XElement item in ctl)
            //{
            //    string str = item.ToString();
            //    //StringReader stringReader = new StringReader(str);
            //    //XmlReader reader = XmlReader.Create(stringReader);
            //    //var obj = (FrameworkElement)XamlReader.Load(reader);
            //    var obj = (FrameworkElement)XamlReader.Parse(str);
            //    canvas.Children.Add(obj);
            //    if (obj.Name == datactl)
            //    {
            //        obj.DataContext = list;
            //    }
            //}





        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
            SetCanvasImage("1");
            AppendObject("1", "id_table1");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
            SetCanvasImage("1");
            AppendObject("1", "id_table2");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
            SetCanvasImage("1");
            AppendObject("1");
        }
    }
}
