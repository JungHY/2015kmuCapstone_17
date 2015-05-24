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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;


class MouseEvent
{
    [DllImport("User32")]
    public static extern int GetCursorPos(out MyPoint pt);
}

public struct MyPoint
{
    public int x;
    public int y;
    public MyPoint(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

namespace Gerim
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public Document new_doc1;
        private byte[] Pixels = new byte[4];
        public bool flag = false;

        public MainWindow()
        {
            InitializeComponent();
        }

       

        private void Mouse_Left_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Document new_Document = new Document();
            new_Document.ShowDialog();

        }

        Document doc;

        private void Preview_Mouse_Right_Button_Down(object sender, MouseButtonEventArgs e)
        {
            if (flag == false)
            {
                flag = true;
                doc = new Document();
                doc.Owner = this;

                MyPoint p;

                MouseEvent.GetCursorPos(out p);
                doc.Left = p.x;
                doc.Top = p.y;
                doc.Show();
                ((App)Application.Current).Documents.Add(doc);
            }

            else
            {
                flag = false;
                doc.Close();
            }
        }

        private void Preview_Key_Down(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Application.Current.Shutdown(110);
            }
        }

    }
}
