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
        int f = 0;

        //Linked List
        public class TouchPoint
        {
            private double X;
            private double Y;
            private bool exist;
            private bool click;
            private TouchPoint nextPoint;
            public TouchPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
                this.exist = false;
                this.click = false;
                nextPoint = null;
            }
            public TouchPoint next()
            {
                return nextPoint;
            }
            public void insert(TouchPoint T)
            {
                T.nextPoint = this.nextPoint;
                this.nextPoint = T;
                T.setExist(true);
            }
            public void removeNext()
            {
                TouchPoint T = next();
                if (T == null)
                    return;
                //this.nextPoint = null;
                T.setX(-10.0);
                T.setY(-10.0);
                this.nextPoint = T.next();
                //T.nextPoint = null;
            }
            public double getX()
            {
                return this.X;
            }
            public double getY()
            {
                return this.Y;
            }
            public bool getExist()
            {
                return this.exist;
            }
            public bool getclick()
            {
                return this.click;
            }
            public void setX(double x)
            {
                this.X = x;
            }
            public void setY(double y)
            {
                this.Y = y;
            }
            public void setExist(bool e)
            {
                this.exist = e;
            }
            public void setclick(bool e)
            {
                this.click = e;
            }
        }

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        public MainWindow()
        {
            InitializeComponent();
            /*
            this.rightCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MLBD);
            this.rightCanvas.MouseMove += new MouseEventHandler(Canvas_MM);
            this.rightCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MLBU);
            */
            for (int i = 0; i < 6; i++)
            {
                myEllipse[i] = new Ellipse();
            }
            head = new TouchPoint(-100.0, -100.0);
            return;
        }
        public enum MouseEventFlag : int
        {
            Absolute = 0x8000,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Move = 0x0001,
            RightDown = 0x0008,
            RightUp = 0x0010,
            Wheel = 0x0800,
            XDown = 0x0080,
            XUp = 0x0100,
            HWheel = 0x1000,
        }
        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            X,
        }

        public void add(double x, double y)
        {
            TouchPoint T = new TouchPoint(x, y);
            head.insert(T);
            pointCnt++;
            return;
        }
        public void refresh(double x, double y)
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if ((x - next.getX()) * (x - next.getX()) + (y - next.getY()) * (y - next.getY()) < 400)
                {
                    next.setX(x);
                    next.setY(y);
                    next.setExist(true);
                    return;
                }
            }
            add(x, y);
        }
        public void initExist()
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
                T.next().setExist(false);
        }
        public void removeNoneExist()
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if (!next.getExist())
                {
                    T.next().setclick(false);
                    Up(MouseButton.Left);
                    T.removeNext();
                    T = head;
                    pointCnt--;
                    if (T.next() == null)
                        break;
                }
            }
            initExist();
        }
        //init
        KinectSensor nui = KinectSensor.KinectSensors[0];
        DepthImagePixel[] depthPixels;
        DepthImagePixel[] initDepthPixels;
        WriteableBitmap colorBitmap;
        private TouchPoint head;
        Rectangle screen;
        Line line;
        Ellipse[] myEllipse = new Ellipse[6];
        MouseEventArgs mouse;
        bool init = true;
        byte[] colorPixels;
        byte[,] colorMap = new byte[45, 3];
        int pixelCnt = 0;
        int pointCnt = 0;
        int s_Row = 0;
        int s_Col = 0;
        int imageX;
        int imageY;
        const int computerX = 1600;
        const int computerY = 900;
        bool isDraw;
        double lastX, lastY;

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
            line = new Line();
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;
            rightCanvas.Children.Add(line);

            screen = new Rectangle();
            screen.Stroke = Brushes.White;
            screen.StrokeThickness = 5;
            screen.Width = 330;
            screen.Height = 250;
            Canvas.SetLeft(screen, 154);
            Canvas.SetTop(screen, 114);
            leftCanvas.Children.Add(screen);

            for (int i = 0; i < 6; i++)
            {
                string str = i.ToString();
                myEllipse[i].Name = "Target" + str;
                myEllipse[i].Fill = Brushes.Green;
                myEllipse[i].Height = 10;
                myEllipse[i].Width = 10;
                leftCanvas.Children.Add(myEllipse[i]);
            }
            //rightCanvas.Children.Add();

            myEllipse[0].Fill = Brushes.White;
            myEllipse[1].Fill = Brushes.Violet;
            myEllipse[2].Fill = Brushes.Yellow;
            myEllipse[3].Fill = Brushes.Pink;
            myEllipse[4].Fill = Brushes.Orange;
            myEllipse[5].Fill = Brushes.LightCyan;

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
                        /*
                        for (int i = 0; i < depthPixels.Length; ++i)
                        {
                            if (i % 640 == 0 || i % 640 == 1 || i % 640 == 638 || i % 640 == 639)
                                continue;
                            initDepthPixels[i].Depth = (short)((depthPixels[i - 2].Depth + depthPixels[i - 1].Depth + depthPixels[i].Depth + depthPixels[i + 1].Depth + depthPixels[i + 2].Depth) / 5.0);
                        }*/
                        init = false;
                    }
                    int colorPixelIndex = 0;

                    for (int i = 0; i < depthPixels.Length; ++i)
                    {
                        short currentDepth = depthPixels[i].Depth;
                        short initDepth = initDepthPixels[i].Depth;
                        byte currentIntensity = (byte)(currentDepth >= minDepth && currentDepth <= maxDepth ? currentDepth : 0);
                        byte initIntensity = (byte)(initDepth >= minDepth && initDepth <= maxDepth ? initDepth : 0);
                        short subDepth = (short)(initDepth - currentDepth);


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
                        if (pointCnt > 5)
                            break;
                        if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255 && i % 640 >= 159 && i % 640 <= 479 && i / 640 >= 119 && i / 640 <= 359)
                        {
                            setPoint(i); // 빨간색을 가진 Pixel을 찾는 재귀 함수
                            if (pixelCnt > 30)//&& pixelCnt < 100)
                            {
                                if (head.next() != null)
                                {
                                    refresh(s_Row / pixelCnt, s_Col / pixelCnt);
                                }
                                else
                                    add(s_Row / pixelCnt, s_Col / pixelCnt);
                            }
                            s_Row = 0;
                            s_Col = 0;
                            pixelCnt = 0;
                        }
                    }

                    removeNoneExist();
                    setEllipse();
                    imageX = 320;
                    imageY = 240;
                    mouseEvents();

                    colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);


                }
            }
        }
        public void setPoint(int i)
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
        public void setEllipse()
        {
            TouchPoint T;
            int i = 0;
            f = 0;
            for (T = head; T.next() != null; T = T.next())
            {
                Canvas.SetLeft(myEllipse[i], T.next().getX());
                Canvas.SetTop(myEllipse[i], T.next().getY());
                f++;
                i++;
            }
            for (int j = i; j < 6; j++)
            {
                Canvas.SetLeft(myEllipse[j], -10);
                Canvas.SetTop(myEllipse[j], -10);
            }
        }
        public void mouseEvents()
        {
            // TouchPoint T;
            /*
            if (head.next() != null)
            {
                if (head.next().next() == null)
                    return;
            }*/
            if (head.next() != null)
            {
                if (!head.next().getclick())
                {
                    Canvas_MLBD();
                    MoveAt(head.next());
                    Down(MouseButton.Left);
                    head.next().setclick(true);
                }
                else
                {
                    Canvas_MM();
                    MoveAt(head.next());
                }
            }
            /*

            for (T = head; T.next() != null; T = T.next())
            {
                //SetCursorPos((int)((T.next().getX() - 159) / imageX * computerX) + 80, (int)((T.next().getY() - 119) / imageY * computerY) + 45);
                if (!T.next().getclick())
                {
                    MoveAt(T.next());
                    Down(MouseButton.Left);
                    T.next().setclick(true);
                }
                else
                {
                    MoveAt(T.next());
                }
            }*/
        }
        public void MoveAt(TouchPoint T)
        {
            /*
            MouseEventFlag Flag = MouseEventFlag.Move | MouseEventFlag.Absolute;
            int X = (int)((T.getX()-159)/imageX * computerX );
            int Y = (int)((T.getY()-119)/imageY * computerY );
            mouse_event((int)Flag, X, Y, 0, IntPtr.Zero);
             */
            SetCursorPos((int)((T.getX() - 159) / imageX * computerX) + 80, (int)((T.getY() - 119) / imageY * computerY) + 45);
        }
        public void Down(MouseButton Button)
        {
            MouseEventFlag Flag = new MouseEventFlag();

            switch (Button)
            {
                case MouseButton.Left: Flag = MouseEventFlag.LeftDown; break;
                case MouseButton.Right: Flag = MouseEventFlag.RightDown; break;
                case MouseButton.Middle: Flag = MouseEventFlag.MiddleDown; break;
                case MouseButton.X: Flag = MouseEventFlag.XDown; break;
            }
            Canvas_MLBD();
            mouse_event((int)Flag, 0, 0, 0, IntPtr.Zero);
        }

        public void Up(MouseButton Button)
        {
            MouseEventFlag Flag = new MouseEventFlag();

            switch (Button)
            {
                case MouseButton.Left: Flag = MouseEventFlag.LeftUp; break;
                case MouseButton.Right: Flag = MouseEventFlag.RightUp; break;
                case MouseButton.Middle: Flag = MouseEventFlag.MiddleUp; break;
                case MouseButton.X: Flag = MouseEventFlag.XUp; break;
            }

            mouse_event((int)Flag, 0, 0, 0, IntPtr.Zero);
        }
        private void Canvas_MLBD()
        {
            Line line = new Line();
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;

            double x = ((head.next().getX() - 159) / imageX * computerX) + 80;
            double y = ((head.next().getY() - 119) / imageY * computerY) + 45;
            line.X1 = x;
            line.Y1 = y;
            line.X2 = x;
            line.Y2 = y;

            lastX = x;
            lastY = y;
        }
        private void Canvas_MM()
        {
            Line line = new Line();
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;
            rightCanvas.Children.Add(line);
            double x2 = ((head.next().getX() - 159) / imageX * computerX) + 80;
            double y2 = ((head.next().getY() - 119) / imageY * computerY) + 45;
            line.X1 = lastX;
            line.Y1 = lastY;
            line.X2 = x2;
            line.Y2 = y2;

            lastX = x2;
            lastY = y2;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Stop();
            Environment.Exit(0);
        }
    }
}

