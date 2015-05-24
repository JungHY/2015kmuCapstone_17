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
using System.Runtime.InteropServices;

namespace KinectUI
{
    
    
    /// <summary>
    /// PenTool.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PenTool : Window
    {

        private byte[] Pixels = new byte[4];

        public PenTool()
        {
            InitializeComponent();
        }

        private Color GetPixelColor(Point CurrentPoint)
        {
            BitmapSource CurrentSource = colorsImage.Source as BitmapSource;

            // 비트맵 내의 좌표 값 계산
            CurrentPoint.X *= CurrentSource.PixelWidth / colorsImage.ActualWidth;
            CurrentPoint.Y *= CurrentSource.PixelHeight / colorsImage.ActualHeight;

            if (CurrentSource.Format == PixelFormats.Bgra32 || CurrentSource.Format == PixelFormats.Bgr32)
            {
                // 32bit stride = (width * bpp + 7) /8
                int Stride = (CurrentSource.PixelWidth * CurrentSource.Format.BitsPerPixel + 7) / 8;
                // 한 픽셀 복사
                CurrentSource.CopyPixels(new Int32Rect((int)CurrentPoint.X, (int)CurrentPoint.Y, 1, 1), Pixels, Stride, 0);

                // 컬러로 변환 후 리턴
                return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
            }
            else
            {
                MessageBox.Show("지원되지 않는 포맷형식");
            }

            return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
        }

        public void SetContent(string content)
        {
            this.Content = content;
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as Image);

            ((ToolBar)(this.Owner)).inkCanvas.DefaultDrawingAttributes.Color = GetPixelColor(point);

        }

        private void btn_AllErase(object sender, RoutedEventArgs e)
        {
            ((ToolBar)(this.Owner)).inkCanvas.Strokes.Clear();
        }

        private void btn_Erase(object sender, RoutedEventArgs e)
        {
            ((ToolBar)(this.Owner)).inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void btn_Pen(object sender, RoutedEventArgs e)
        {
            ((ToolBar)(this.Owner)).inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
    }
}
