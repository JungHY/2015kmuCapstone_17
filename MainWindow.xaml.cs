using System;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System;

using Microsoft.Kinect;
using System.Windows.Threading;

using System.Diagnostics;

namespace righthand
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public class TouchPoint
        {
            private double X;
            private double Y;
            private TouchPoint nextPoint;
            public TouchPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
                nextPoint = null;
            }
            public TouchPoint next()
            {
                return nextPoint;
            }
            public bool equals(double x, double y)
            {
                if (this.X == x && this.Y == y)
                    return true;
                return false;
            }
            public void insert(TouchPoint T)
            {
                T.nextPoint = this.nextPoint;
                this.nextPoint = T;
                return;
            }
            public void removeNext()
            {
                TouchPoint T = next();
                if (T == null)
                    return;
                this.nextPoint = null;
                T.nextPoint = null;
                return;
            }
            public double getX()
            {
                return this.X;
            }
            public double getY()
            {
                return this.Y;
            }
            public void setX(double x)
            {
                this.X = x;
            }
            public void setY(double y)
            {
                this.Y = y;
            }
        }
        private TouchPoint head;

        int flag = 0;

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 2; i++)
            {
                squar[i] = new Point(-10, 0);
            }
            for (int i = 0; i < 5; i++)
            {
                myEllipse[i] = new Ellipse();
            }
            head = new TouchPoint(-100.0,-100.0);
            return;
        }

        public void add(double x, double y)
        {
            TouchPoint T = new TouchPoint(x, y);
            head.insert(T);
            return;
        }
        public void remove(double x, double y)
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if (next.equals(x, y))
                {
                    T.removeNext();
                    break;
                }
            }
        }
        public void refresh(double x, double y)
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if ((x - next.getX()) * (x - next.getX()) + (y - next.getY()) * (y - next.getY()) < 10000)
                {
                    next.setX(x);
                    next.setY(y);
                }
                else
                    add(x, y);
            }
        }
        //init
        KinectSensor nui = KinectSensor.KinectSensors[0];
        DepthImagePixel[] depthPixels;
        DepthImagePixel[] initDepthPixels;
        WriteableBitmap colorBitmap;
        Ellipse[] myEllipse = new Ellipse[5];
        bool init = true;
        byte[] colorPixels;
        byte[,] colorMap = new byte[45, 3];
        Point squarpoint = new Point(0, 0);
        Point[] touchPoint = new Point[10];
        float sdepth = 0;
        Point[] squar = new Point[4];
        float[] squardepth = new float[4];
        int flog = 0;
        int value = 0;
        int cnt = 0;
        int pixelCnt = 0;
        int pointCnt = 0;
        int s_Row=0;
        int s_Col=0;
        DispatcherTimer timer = new DispatcherTimer();
        int imageX;
        int imageY;
        const int computerX = 1600;
        const int computerY = 900;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.depthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];


            nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            depthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            initDepthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            colorPixels = new byte[nui.DepthStream.FramePixelDataLength * sizeof(int)];
            colorBitmap = new WriteableBitmap(nui.DepthStream.FrameWidth, nui.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            Video.Source = colorBitmap;
            /*
            for ( int i=0;i<15;i++ )
            {
                colorMap[i, 0] = (byte)(Math.Cos( ( i / 15.0 * 90.0 ) * 3.14 / 180.0) * 255.0);//(byte)(initIntensity - currentIntensity);
                colorMap[i, 1] = 0;// (byte)(initIntensity - currentIntensity);
                colorMap[i, 2] = (byte)(Math.Sin((i / 15.0 * 90.0) * 3.14 / 180.0) * 255.0);// (byte)(initIntensity - currentIntensity);
            }*/

            //nui.ColorStream.Enable();
            nui.DepthStream.Enable();

            nui.DepthStream.Range = DepthRange.Near;

            //nui.ColorFrameReady += nui_ColorFrameReady;
            nui.DepthFrameReady += nui_DepthFrameReady;


            nui.Start();
        }
        private void nui_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    depthImageFrame.CopyDepthImagePixelDataTo(depthPixels);
                   

                    int minDepth = depthImageFrame.MinDepth;
                    int maxDepth = depthImageFrame.MaxDepth;
                    if (init == true)
                    {
                        depthImageFrame.CopyDepthImagePixelDataTo(initDepthPixels);
                        
                        init = false;
                    }
                    int colorPixelIndex = 0;

                    for (int i = 0; i < depthPixels.Length; ++i)
                    {
                        short currentDepth = depthPixels[i].Depth;
                        short initDepth = initDepthPixels[i].Depth;
                        byte currentIntensity = (byte)(currentDepth >= minDepth && currentDepth <= maxDepth ? currentDepth : 0);


                        if ((i < 640 || i > 640 * 480 - 480 - 1 || i % 640 == 0 || i % 640 == 640 - 1) != true)
                        {

                            currentDepth = (short)((depthPixels[i - 640 - 1].Depth + depthPixels[i - 640 + 1].Depth
                                                 + depthPixels[i + 640 + 1].Depth + depthPixels[i + 640 + 1].Depth) / 4);

                        }

                        
                        byte initIntensity = (byte)(initDepth >= minDepth && initDepth <= maxDepth ? initDepth : 0);
                        
                        
                        short subDepth = (short)(initDepth - currentDepth );
                        
                        
                        /*
                        if (0 <= subDepth && subDepth < 15)
                        {
                            colorPixels[colorPixelIndex++] = colorMap[subDepth, 0];
                            colorPixels[colorPixelIndex++] = colorMap[subDepth, 1];
                            colorPixels[colorPixelIndex++] = colorMap[subDepth, 2];
                        }
                        else
                        {
                            colorPixels[colorPixelIndex++] = 255;
                            colorPixels[colorPixelIndex++] = 0;
                            colorPixels[colorPixelIndex++] = 0;
                        }
                        */


                        if (subDepth < 0)
                            subDepth = 0;

                        if (0 <= subDepth && subDepth <= 10)//initDepth - currentDepth < 3)//initIntensity - currentIntensity == 0)
                        {
                            colorPixels[colorPixelIndex++] = 255;//(byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 0;// (byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 0;// (byte)(initIntensity - currentIntensity);
                        }
                        else if (10 < subDepth && subDepth <= 15)
                        {
                            colorPixels[colorPixelIndex++] = 0;//(byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 0;// (byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 255;// (byte)(initIntensity - currentIntensity);
                        }
                        else
                        {
                            colorPixels[colorPixelIndex++] = 0;//(byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 0;// (byte)(initIntensity - currentIntensity);
                            colorPixels[colorPixelIndex++] = 0;// (byte)(initIntensity - currentIntensity);
                        }
                        ++colorPixelIndex;
                    }
                    for (int i = 0; i < depthPixels.Length; ++i)
                    {
                        if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255 && i%640 >= 159 && i%640 <= 479 && i/640 >= 119 && i/640 <= 359 )
                        {
                            setPoint(i); // 빨간색을 가진 Pixel을 찾는 재귀 함수
                            if (pixelCnt > 30 && pixelCnt < 100)
                            {
                                if(head.next() != null)
                                    refresh(s_Row / pixelCnt, s_Col / pixelCnt);
                                else
                                    add(s_Row / pixelCnt, s_Col / pixelCnt);
                            }
                            s_Row = 0;
                            s_Col = 0;
                            pixelCnt = 0;
                        }

                    }

                    if (head.next() != null)
                        textBox1.Text = string.Format("( {0:0.00},{1:0.00})", head.next().getX(), head.next().getY());
                    setEllipse();
                    colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);
               

                    /*
                    if ((depth - (rightDistanceZ * 1000) > -60) && (depth - (rightDistanceZ * 1000) < 60) && (value < 0))
                    {
                        if (flog == 0)
                        {
                            squarpoint = rightPoint;
                            sdepth = rightDistanceZ;
                            flog++;
                            timer.Tick += dispatcherTimer_Tick;
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                            timer.Start();
                            textBox5.Text = "SAVING...";
                        }
                        if ((squarpoint.X - rightPoint.X > 5) || (squarpoint.X - rightPoint.X < -5) ||
                            (squarpoint.Y - rightPoint.Y > 5) || (squarpoint.Y - rightPoint.Y < -5))
                        {
                            squarpoint = rightPoint;
                            flog = 0;
                            timer.Stop();
                            timer = null;
                            timer = new DispatcherTimer();
                            textBox5.Text = "NO INPUT";
                            progressBar1.Value = 0;
                            cnt = 0;
                        }

                    }
                    else
                    {
                        squarpoint = rightPoint;
                        flog = 0;
                        timer.Stop();
                        timer = null;
                        timer = new DispatcherTimer();
                        textBox5.Text = "NO INPUT";
                        progressBar1.Value = 0;
                        cnt = 0;
                    }

                    if (value == 0)
                    {
                        if (squar[0].X > squar[1].X)
                        {
                            double tmp;
                            tmp = squar[0].X;
                            squar[0].X = squar[1].X;
                            squar[1].X = tmp;
                        }
                        if (squar[0].Y > squar[1].Y)
                        {
                            double tmp;
                            tmp = squar[0].Y;
                            squar[0].Y = squar[1].Y;
                            squar[1].Y = tmp;
                        }

                    }
                    squar[0].X = 320.0;
                    squar[0].Y = 120.0;
                    squar[1].X = 480.0;
                    squar[1].Y = 240.0;

                    Canvas.SetLeft(screen1, squar[0].X);
                    Canvas.SetTop(screen1, squar[0].Y);
                    Canvas.SetLeft(screen2, squar[1].X);
                    Canvas.SetTop(screen2, squar[1].Y);
                    imageX = (int)(squar[1].X - squar[0].X) + 32;
                    imageY = (int)(squar[1].Y - squar[0].Y) + 18;

                    if ((squar[0].X - 16) < rightPoint.X && (squar[1].X + 16) > rightPoint.X && (squar[0].Y - 9) < rightPoint.Y && (squar[1].Y + 9) > rightPoint.Y)
                        SetCursorPos((int)((rightPoint.X - squar[0].X) / imageX * computerX) + 80, (int)((rightPoint.Y - squar[0].Y) / imageY * computerY) + 45);

                    */

                }
            }
        }
        void setPoint(int i)
        {
            if (i < 0 || i >= 307200)
                return;
            else if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255)
            {
                colorPixels[i * 4 + 1] = 1; // 한번 지나간곳은 +로 바꿔서 안들리게함
                pixelCnt++; // 일정픽셀이상만 포인트로 인식함
                s_Col += i / 640;
                s_Row += i % 640;
            }
            else
                return;
            setPoint(i + 1);
            setPoint(i - 1);
            setPoint(i + 640);
            setPoint(i - 640);
        }
        void dispatcherTimer_Tick(object sender, object e)
        {
            cnt++;
            if (cnt == 10)
            {
                progressBar1.Value += 10;
                squar[value] = squarpoint;
                squardepth[value] = sdepth;
                Canvas.SetLeft(screen1, squar[0].X);
                Canvas.SetTop(screen1, squar[0].Y);
                Canvas.SetLeft(screen2, squar[1].X);
                Canvas.SetTop(screen2, squar[1].Y);
                squarpoint = new Point(-10, 0);
                flog = 0;
                timer.Stop();
                timer = null;
                timer = new DispatcherTimer();
                value++;
                textBox4.Text = "NO INPUT";
                progressBar1.Value = 0;
                cnt = 0;
            }
            progressBar1.Value += 10;

        }
        public void setEllipse()
        {
            TouchPoint T;
            int i=0;
            for (T = head; T.next() != null; T = T.next())
            {
                if (T.next() != null && i==0)
                {
                    Canvas.SetLeft(Target1, T.next().getX());
                    Canvas.SetTop(Target1, T.next().getY());
                }
                if (T.next() != null && i == 1)
                {
                    Canvas.SetLeft(Target1, T.next().getX());
                    Canvas.SetTop(Target1, T.next().getY());
                }
                if (T.next() != null && i == 2)
                {
                    Canvas.SetLeft(Target1, T.next().getX());
                    Canvas.SetTop(Target1, T.next().getY());
                }
                if (T.next() != null && i == 3)
                {
                    Canvas.SetLeft(Target1, T.next().getX());
                    Canvas.SetTop(Target1, T.next().getY());
                }
                if (T.next() != null && i == 4)
                {
                    Canvas.SetLeft(Target1, T.next().getX());
                    Canvas.SetTop(Target1, T.next().getY());
                }

                /*
                myEllipse[i].Fill = System.Windows.Media.Brushes.Green;
                myEllipse[i].Height = 10;
                myEllipse[i].Width = 10;
                Canvas.SetLeft(myEllipse[i], T.getX());
                Canvas.SetTop(myEllipse[i], T.getY());
                */
                i++;
            }
 

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Stop();
            Environment.Exit(0);
        }
    }
}

