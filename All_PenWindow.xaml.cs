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

namespace KinectUI
{
    /// <summary>
    /// PenCanvas2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PenCanvas2 : Window
    {
        public PenTool new_doc1;
        private byte[] Pixels = new byte[4];
        public bool flag = false;

        public PenCanvas2()
        {
            InitializeComponent();
        }
        private void Mouse_Left_Button_Down(object sender, MouseButtonEventArgs e)
        {
            PenTool new_Document = new PenTool();
            new_Document.ShowDialog();

        }
        PenTool pen;

        private void Preview_Mouse_Right_Button_Down(object sender, MouseButtonEventArgs e)
        {
            if (flag == false)
            {
                flag = true;
                pen = new PenTool();
                pen.Owner = this;

                MyPoint p;

                MouseEvent.GetCursorPos(out p);
                pen.Left = p.x;
                pen.Top = p.y;
                pen.Show();
                ((App)Application.Current).PenTool.Add(pen);
            }

            else
            {
                flag = false;
                pen.Close();
            }
        }

        private void Preview_Key_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown(110);
            }
        }

    }
}
