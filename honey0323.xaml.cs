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
        int f=0;
        //Linked List
        public class TouchPoint                 //포인트를 저장할 객체 클래스
        {
            private double X;                   //포인트의 x좌표
            private double Y;                   //포인트의 y좌표
            private bool exist;                 //같은점인지 판단여부 flag개념, 새로생겨났거나 새로고침여부 확인용
            private TouchPoint nextPoint;       //링크드 리스트를 위한 다음 포인트
            public TouchPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
                this.exist = false;
                nextPoint = null;
            }
            public TouchPoint next()
            {
                return nextPoint;
            }
            public void insert(TouchPoint T)    //현재 head와 head가 가르키고 있는 next포인트 사이에 새로운 포인트 T를 삽입
            {
                T.nextPoint = this.nextPoint;
                this.nextPoint = T;
                T.setExist(true);
            }
            public void removeNext()            //head의 next포인트를 삭제
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
        }

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
            for (int i = 0; i < 6; i++)
            {
                myEllipse[i] = new Ellipse();       //초록색 점 찍기위한 배열
            }
            head = new TouchPoint(-100.0, -100.0);  //최초 head초기화
            return;
        }

        public void add(double x, double y)         //카메라에 새로운 점이 찍혔을 경우 새로운 객체 생성
        {
            TouchPoint T = new TouchPoint(x, y);
            head.insert(T);
            pointCnt++;
            return;
        }
        public void refresh(double x, double y)     //새로고침
        {                                           //input으로 들어온 포인트의 중심점을 뜻하는 (x,y)를 링크드 리스트의 포인트들과 비교하여
            TouchPoint T;                           //이미 존재했던 점이 움직인 것인지 새로 생긴것인지 판별하여 새로생긴 포인트의 경우 add해준다.
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
        public void initExist()             //링크드 리스트의 모든 포인터들의 exist를 false로 초기화
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
                T.next().setExist(false);
        }
        public void removeNoneExist()       //전 프레임과 비교하여 있던점이 사라졌을 경우 링크드 리스트의 포인트를 삭제해준다.
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if (!next.getExist())
                {
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
        Ellipse[] myEllipse = new Ellipse[6];
        bool init = true;
        byte[] colorPixels;
        byte[,] colorMap = new byte[45, 3];
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


            for (int i = 0; i < 6; i++)
            {
                string str = i.ToString();
                myEllipse[i].Name = "Target" + str;
                myEllipse[i].Fill = Brushes.Green;
                myEllipse[i].Height = 10;
                myEllipse[i].Width = 10;
                rightCanvas.Children.Add(myEllipse[i]);
            }

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

                    for (int i = 0; i < depthPixels.Length; ++i)                //depth값에 따라 색깔을 달리해서 출력한다.
                    {                                                           //최초 인식된 벽값과 비교하여 터치가 일어났을 경우 빨간색으로 출력하고
                        short currentDepth = depthPixels[i].Depth;              //최초 인식된 벽값과 같을경우 파란색으로 표시한다.
                        short initDepth = initDepthPixels[i].Depth;
                        byte currentIntensity = (byte)(currentDepth >= minDepth && currentDepth <= maxDepth ? currentDepth : 0);
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
                    for (int i = 0; i < depthPixels.Length; ++i)        //모든 픽셀들에서 검사한다.
                    {
                        if (pointCnt > 5)
                            break;
                        if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255 && i%640 >= 159 && i%640 <= 479 && i/640 >= 119 && i/640 <= 359 )      //필요한 부분만 검사한다.
                        {
                            setPoint(i); //
                            if (pixelCnt > 30 )//&& pixelCnt < 100)         //빨간색 픽셀이 30개 이상 검출된 포인트의 경우
                            {                                               //새로고침 해주거나 최초의 포인트인경우 add해준다.
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

                    removeNoneExist();          //없어진 포인트를 지운다.
                    setEllipse();
                    if (head.next() != null)
                    {
                        textBox1.Text = string.Format("( {0:0.00}, {1:0.00}, {2:0.00}))", head.next().getX(), head.next().getY(),f);
                        if (head.next().next() != null)
                        {
                            textBox2.Text = string.Format("( {0:0.00}, {1:0.00})", head.next().next().getX(), head.next().next().getY());
                            if (head.next().next().next() != null)
                                textBox3.Text = string.Format("( {0:0.00}, {1:0.00})", head.next().next().next().getX(), head.next().next().next().getY());
                            
                        }
                    }

                    colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);
               

                }
            }
        }
        void setPoint(int i)            //i번째 픽셀을 검사해서 빨간색일경우 주변 4방향 픽셀들을 재귀로 검사하여
        {                               //연결된 빨간픽셀들의 중점을 s_Col, s_Row에 저장한다.
            if (i < 0 || i >= 307200)
                return;
            else if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255)
            {
                colorPixels[i * 4 + 1] = 1; // 한번 지나간곳은 Green값을 1로 바꿔서 다시 참조하지 않게 한다.
                pixelCnt++;                 // 일정픽셀이상만 포인트로 인식함
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
                Canvas.SetLeft(myEllipse[j],-10);
                Canvas.SetTop(myEllipse[j], -10);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Stop();
            Environment.Exit(0);
        }
    }
}

