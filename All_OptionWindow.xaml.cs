using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectUI
{
    /// <summary>
    /// SetUp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetUp : Window
    {

        public SetUp()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ((ToolBar)(this.Owner)).myEllipse = new Ellipse[4];
            for (int i = 0; i < 4; i++)
                ((ToolBar)(this.Owner)).myEllipse[i] = new Ellipse();
            ((ToolBar)(this.Owner)).screenLT.X = 0;
            ((ToolBar)(this.Owner)).screenLT.Y = 0;
            ((ToolBar)(this.Owner)).screen = new System.Windows.Shapes.Rectangle();
            ((ToolBar)(this.Owner)).screen.Stroke = System.Windows.Media.Brushes.Red;
            ((ToolBar)(this.Owner)).screen.StrokeThickness = 2;
            ((ToolBar)(this.Owner)).screen.Width = 640;
            ((ToolBar)(this.Owner)).screen.Height = 480;
            ((ToolBar)(this.Owner)).mouseUse = false;
            Canvas.SetLeft(((ToolBar)(this.Owner)).screen, ((ToolBar)(this.Owner)).screenLT.X);
            Canvas.SetTop(((ToolBar)(this.Owner)).screen, ((ToolBar)(this.Owner)).screenLT.Y);
            leftCanvas.Children.Add(((ToolBar)(this.Owner)).screen);

            Video.Source = ((ToolBar)(this.Owner)).colorBitmap;
            
            for (int i = 0; i < 4; i++)
            {
                ((ToolBar)(this.Owner)).myEllipse[i].Fill = System.Windows.Media.Brushes.Green;
                ((ToolBar)(this.Owner)).myEllipse[i].Height = 10;
                ((ToolBar)(this.Owner)).myEllipse[i].Width = 10;
                leftCanvas.Children.Add(((ToolBar)(this.Owner)).myEllipse[i]);
                Canvas.SetLeft(((ToolBar)(this.Owner)).myEllipse[i], -100);
                Canvas.SetTop(((ToolBar)(this.Owner)).myEllipse[i], -100);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        // SCREENSIZE 재설정 함수
        private void ScreenSize_Click(object sender, RoutedEventArgs e)
        {
            ((ToolBar)(this.Owner)).ScreenDraw = true;
            ((ToolBar)(this.Owner)).screen.StrokeThickness = 0;
            ((ToolBar)(this.Owner)).screen.Width = 640;
            ((ToolBar)(this.Owner)).screen.Height = 480;
            Canvas.SetLeft(((ToolBar)(this.Owner)).screen, 0);
            Canvas.SetTop(((ToolBar)(this.Owner)).screen, 0);
            ((ToolBar)(this.Owner)).screenLT.X = 0;
            ((ToolBar)(this.Owner)).screenLT.Y = 0;
            ((ToolBar)(this.Owner)).screenCnt = 0;
            for (int i = 0; i < 4; i++)
            {
                Canvas.SetLeft(((ToolBar)(this.Owner)).myEllipse[i], -100);
                Canvas.SetTop(((ToolBar)(this.Owner)).myEllipse[i], -100);
            }
            ((ToolBar)(this.Owner)).mouseUse = false;
        }
        // 초기 Depth값 재설정 함수
        private void DepthInit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 480; i++)
                for (int j = 0; j < 640; j++)
                    ((ToolBar)(this.Owner)).initdepthPixel[i, j] = ((ToolBar)(this.Owner)).depthPixels[640 * i + j].Depth;
        }

        // Kinect 센서 Start & Stop 함수
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (((ToolBar)(this.Owner)).start)
                ((ToolBar)(this.Owner)).start = false;
            else
                ((ToolBar)(this.Owner)).start = true;
        }
        // 종료 버튼 함수
        private void Close_Click(object sender, RoutedEventArgs e)
        {

            var uriSource = new Uri(@"images\option.png", UriKind.Relative);
            ((ToolBar)(this.Owner)).option.Source = new BitmapImage(uriSource);
            ((ToolBar)(this.Owner)).optionFlag = false;
            this.Close();
        }

    }
}
