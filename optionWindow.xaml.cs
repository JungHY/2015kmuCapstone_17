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
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

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
        //Linked List로 터치 포인트 구현
        public class TouchPoint
        {
            private double X;              // X좌표
            private double Y;              // Y좌표
            private bool exist;            // 이전 프레임과 존재여부 판별
            private bool click;            // 클릭이벤트 발생 Flag
            public int revision;           // 짧은 프레임 내 포인트 삭제 방지
            private TouchPoint nextPoint;  
            public TouchPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
                this.exist = false;
                this.click = false;
                this.revision = 0;
                nextPoint = null;
            }
            // NextPoint 연결
            public TouchPoint next()
            {
                return nextPoint;
            }
            // Linked List 삽입
            public void insert(TouchPoint T)
            {
                T.nextPoint = this.nextPoint;
                this.nextPoint = T;
                T.setExist(true);
            }
            // Linked List 삭제
            public void removeNext()
            {
                TouchPoint T = next();
                if (T == null)
                    return;
                T.setX(-10.0);
                T.setY(-10.0);
                this.nextPoint = T.next();
            }
            //Private 변수 Get, Set 함수
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

        //init
        KinectSensor nui;                               // 키넥트 센서 변수                                      
        DepthImagePixel[] depthPixels;                  // 현재 Depth 저장
        DepthImagePixel[] initDepthPixels;              // 초기 Depth 저장
        short[,] depthPixel = new short[480, 640];      // 화면 좌우 반전을 위해 2차원 배열로 저장
        short[,] initdepthPixel = new short[480, 640];  // 화면 좌우 반전을 위해 2차원 배열로 저장
        WriteableBitmap colorBitmap;                    // 화면의 출력할 Color Map
        private TouchPoint head;                        // TouchPoint Head
        private byte[] VirKey = new byte[256];          // 키보드
        Point pre_Point1, pre_Point2;                   // TouchPoint 저장변수
        Point rightPoint;                               // 우클릭 인식 좌표
        Point[] screenPoint = new Point[4];             // SCREEN 4 모퉁이 좌표
        Point screenLT = new Point(0, 0);               // SCREEN LeftTop 위치
        Rectangle screen;                               // SCREEN 사각형
        Ellipse[] myEllipse;                            // TouchPoint 위치 생성할 원
        bool start = false;                             // 키넥트센서 시작 정지 
        bool init = true;                               // 초기 Depth 저장을 위한 변수
        bool ScreenDraw = false;                        // SCREEN 캘리 변수  
        bool mouseUse = false;                          // 마우스사용 Flag
        bool rightFlag = true;                          // 우클릭 이벤트 Flag
        bool gestureFlag = false;                       // 제스쳐 이벤트 Flag
        byte[] colorPixels;                             // Depth값을 Color값으로 변경
        int moveLimit = 49;                              // TouchPoint 변화보정 변수
        int screenCnt = 0;                              // SCREEN 모서리 생성 개수
        int pixelCnt = 0;                               // TouchPoint 픽셀 개수
        int pointCnt = 0;                               // TouchPoint 개수
        int s_Row = 0;                                  // TouchPoint를 만드는 픽셀의 가로합
        int s_Col = 0;                                  // TouchPoint를 만드는 픽셀의 세로합
        int mode = 1;                                   // Mode 설정
        int frameCnt = 0;                               // 프레임 카운트
        int UserCnt = 50;                               // 사용자 숫자
        double imageX;                                  // SCREEN Width
        double imageY;                                  // SCREEN Height
        double computerX = 1600.0;                      // 화면 해상도 Width
        double computerY = 900.0;                       // 화면 해상도 Height

        // 마우스, 키보드 사용하기위한 함수 
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("user32.dll")]
        static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public MainWindow()
        {
            InitializeComponent();
            myEllipse = new Ellipse[UserCnt + 1];
            for (int i = 0; i < UserCnt+1; i++)
                myEllipse[i] = new Ellipse();
            for (int i = 0; i < 4; i++)
                screenPoint[i] = new Point(0, 0);

            head = new TouchPoint(-100.0, -100.0);
            pre_Point1 = new Point(0, 0);
            pre_Point2 = new Point(0, 0);

            ModeSelect.Items.Add("1인모드");
            ModeSelect.Items.Add("다인모드");
            return;
        }

        // 마우스이벤트 enum class
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

        // 마우스버튼 enum class
        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            Wheel,
            X,
        }

        // TouchPoint 추가
        public void add(double x, double y)
        {
            TouchPoint T = new TouchPoint(x, y);
            head.insert(T);
            pointCnt++;
            return;
        }

        // TouchPoint 갱신
        public void refresh(double x, double y)
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if ((x - next.getX()) * (x - next.getX()) + (y - next.getY()) * (y - next.getY()) < moveLimit)
                {
                    next.setExist(true);
                    return;
                }
                else if ((x - next.getX()) * (x - next.getX()) + (y - next.getY()) * (y - next.getY()) < 400)
                {
                    next.setX(x);
                    next.setY(y);
                    next.setExist(true);
                    return;
                }
            }
            add(x, y);
        }

        // TouchPoint 전, 현 frame 비교 
        public void initExist()
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
                T.next().setExist(false);
        }

        // 없어진 TouchPoint Linked List에서 삭제
        public void removeNoneExist()
        {
            TouchPoint T;
            for (T = head; T.next() != null; T = T.next())
            {
                TouchPoint next = T.next();
                if (!next.getExist())
                {
                    if (next.revision > 5)
                    {
                        T.next().setclick(false);
                        if(!gestureFlag)
                            Up(MouseButton.Left);
                        rightFlag = true;
                        T.removeNext();
                        T = head;
                        pointCnt--;
                        if (T.next() == null)
                            break;
                        next.revision = 0;
                    }
                    else
                        next.revision++;
                }
            }
            initExist();
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    nui = KinectSensor.KinectSensors[0];
                    break;
                }
            }

            if (null != nui)
            {
                try
                {
                    // Start the sensor!
                    nui.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    nui = null;
                }
            }
            else
            {
                textBox1.Text = "No Ready Kinect Found";
                return;
            }

            //변수 초기화
            screen = new Rectangle();
            screen.Stroke = Brushes.Red;
            screen.StrokeThickness = 2;
            screen.Width = 640;
            screen.Height = 480;
            screenLT.X = 0;
            screenLT.Y = 0;
            Canvas.SetLeft(screen, screenLT.X);
            Canvas.SetTop(screen, screenLT.Y);
            leftCanvas.Children.Add(screen);

            GetKeyboardState(VirKey);


            for (int i = 0; i < UserCnt+1; i++)
            {
                myEllipse[i].Fill = Brushes.Green;
                myEllipse[i].Height = 10;
                myEllipse[i].Width = 10;
                leftCanvas.Children.Add(myEllipse[i]);
            }

            myEllipse[0].Fill = Brushes.White;
            myEllipse[1].Fill = Brushes.Violet;
            myEllipse[2].Fill = Brushes.Yellow;
            myEllipse[3].Fill = Brushes.Pink;
            myEllipse[4].Fill = Brushes.Orange;
            myEllipse[5].Fill = Brushes.LightCyan;

            nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            depthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            initDepthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            colorPixels = new byte[nui.DepthStream.FramePixelDataLength * sizeof(int)];
            colorBitmap = new WriteableBitmap(nui.DepthStream.FrameWidth, nui.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            Video.Source = colorBitmap;

            nui.DepthStream.Range = DepthRange.Near;

            nui.DepthFrameReady += nui_DepthFrameReady;
            nui.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Stop();
            Environment.Exit(0);
        }
        
        private void nui_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null && start)
                {
                    depthImageFrame.CopyDepthImagePixelDataTo(depthPixels);

                    int minDepth = depthImageFrame.MinDepth;
                    int maxDepth = depthImageFrame.MaxDepth;

                    int colorPixelIndex = 0;

                    // Depth값 2차원 배열로 저장
                    for (int i = 0; i < 480; i++)
                    {
                        for (int j = 0; j < 640; j++)
                        {
                            if (init == true)
                                initdepthPixel[i, j] = depthPixels[640 * i + j].Depth;
                            depthPixel[i, j] = depthPixels[640 * i + j].Depth;
                        }
                    }
                    init = false;

                    // 좌우 반전으로 Color 값 출력
                    for (int i = 0; i < 480; i++)
                    {
                        for (int j = 639; j >= 0; j--)
                        {
                            short currentDepth = depthPixel[i, j];
                            short initDepth = initdepthPixel[i, j];
                            short subDepth ;
                            if(currentDepth > 2000 || initDepth >2000)
                                subDepth = 0;
                            else
                                subDepth = (short)(initDepth - currentDepth);

                            if (subDepth < 0)
                                subDepth = 0;
                            if (0 <= subDepth && subDepth <= 10)
                            {
                                colorPixels[colorPixelIndex++] = 255;
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 0;
                            }
                            else if (10 < subDepth && subDepth <= 15)
                            {
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 255;
                            }
                            else
                            {
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 0;
                            }
                            ++colorPixelIndex;

                        }
                    }

                    // Color값으로 TouchPoint Search
                    for (int i = 0; i < depthPixels.Length; ++i)
                    {
                        // Point 최대 개수 설정
                        if (pointCnt > UserCnt)
                            break;

                        // SCREEN 내에서 터치 이벤트 발생
                        if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255 && i % 640 >= screenLT.X && i % 640 <= (screenLT.X + screen.Width) && i / 640 >= screenLT.Y && i / 640 <= (screenLT.Y + screen.Height))
                        {
                            setPoint(i);     // 빨간색을 가진 Pixel을 찾는 재귀 함수
                             
                            if (pixelCnt > 30)  // 일정 픽셀이상 Point 인식
                                refresh(s_Row / pixelCnt, s_Col / pixelCnt);
                            
                            // 변수 초기화
                            s_Row = 0;
                            s_Col = 0;
                            pixelCnt = 0;
                        }
                    }
                    //캘리 화면 크기
                    imageX = screen.Width;
                    imageY = screen.Height;

                    //화면 캘리 함수 호출
                    if (ScreenDraw == true && head.next() != null)
                        drawRectangle();
                    else if (ScreenDraw == true && head.next() == null)
                        frameCnt = 0;

                    //사라진 포인트 삭제
                    removeNoneExist(); 
                    
                    //포인트 위치에 점 생성
                    setEllipse();

                    //마우스 이벤트 함수
                    if(mouseUse)
                        mouseEvents();
                    
                    textBox1.Text = string.Format("{0:0.00}",frameCnt);
                    colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);

                }
            }
        }

        // TouchPoint Search 함수
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

        // 화면 캘리브레이션 함수
        public void drawRectangle()
        {
            if (head.next().next() != null)
                frameCnt = 0;
            else
                frameCnt++;
            int[] minX = new int[2]{700,700};
            int[] minY = new int[2]{700,700};
            int[] maxX = new int[2]{-1,-1};
            int[] maxY = new int[2]{-1,-1};
            // 모서리 4개 생성시 화면 크기 저장 
            if (screenCnt == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (minX[0] > screenPoint[i].X)
                        minX[0] = (int)screenPoint[i].X;
                    if (minY[0] > screenPoint[i].Y)
                        minY[0] = (int)screenPoint[i].Y;
                    if (maxX[0] < screenPoint[i].X)
                        maxX[0] = (int)screenPoint[i].X;
                    if (maxY[0] < screenPoint[i].Y)
                        maxY[0] = (int)screenPoint[i].Y;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (minX[1] > screenPoint[i].X && minX[0] != screenPoint[i].X)
                        minX[1] = (int)screenPoint[i].X;
                    if (minY[1] > screenPoint[i].Y && minY[0] != screenPoint[i].Y)
                        minY[1] = (int)screenPoint[i].Y;
                    if (maxX[1] < screenPoint[i].X && maxX[0] != screenPoint[i].X)
                        maxX[1] = (int)screenPoint[i].X;
                    if (maxY[1] < screenPoint[i].Y && maxY[0] != screenPoint[i].Y)
                        maxY[1] = (int)screenPoint[i].Y;
                }

                screen.Width = (maxX[0] + maxX[1]) / 2 - (minX[0] + minX[1]) / 2;
                screen.Height = (maxY[0] + maxY[1]) / 2 - (minY[0] + minY[1]) / 2;
                screenLT.X = (minX[0] + minX[1])/2;
                screenLT.Y = (minY[0] + minY[1])/2;
                screen.StrokeThickness = 2;
                Canvas.SetLeft(screen, screenLT.X);
                Canvas.SetTop(screen, screenLT.Y);
                ScreenDraw = false;
                screenCnt = 0;
                for (int i = 1; i < 5; i++)
                {
                    Canvas.SetLeft(myEllipse[UserCnt - i], -100);
                    Canvas.SetTop(myEllipse[UserCnt - i], -100);
                }
                mouseUse = true;
            }
            // 모서리 저장
            else if (frameCnt > 20)
            {
                bool save = true; // 같은 위치에 모서리 생성 방지 Flag
                for (int i = 0; i < screenCnt; i++)
                {
                    double distance = (screenPoint[i].X - head.next().getX()) * (screenPoint[i].X - head.next().getX()) + (screenPoint[i].Y - head.next().getY()) * (screenPoint[i].Y - head.next().getY());
                    if (distance < 100)
                    {
                        save = false;
                        break;
                    }
                    save = true;
                }
                if(save)
                {
                    screenPoint[screenCnt].X = head.next().getX();
                    screenPoint[screenCnt].Y = head.next().getY();
                    screenCnt++;
                    Canvas.SetLeft(myEllipse[UserCnt - screenCnt], head.next().getX());
                    Canvas.SetTop(myEllipse[UserCnt - screenCnt], head.next().getY());
                }
                frameCnt = 0;
            }
        }

        // TouchPoint 좌표에 원 이동
        public void setEllipse()
        {
            TouchPoint T;
            int i = 0;
            for (T = head; T.next() != null; T = T.next())
            {
                Canvas.SetLeft(myEllipse[i], T.next().getX());
                Canvas.SetTop(myEllipse[i], T.next().getY());
                i++;
            }
            for (int j = i; j < UserCnt-3; j++)
            {
                Canvas.SetLeft(myEllipse[j], -100);
                Canvas.SetTop(myEllipse[j], -100);
            }
        }

        // Mouse Gesture 이벤트 설정 함수
        public void mouseEvents()
        {
            if (pointCnt == 0)
                gestureFlag = false;
            // Point가 하나일때 일반적 마우스 사용 & 1인 모드
            if (pointCnt == 1 && mode == 1)
            {
                if (!head.next().getclick())
                {
                    MoveAt(head.next());
                    Down(MouseButton.Left);
                    head.next().setclick(true);
                    if (!ScreenDraw)
                    {
                        rightPoint = new Point(head.next().getX(), head.next().getY());
                        frameCnt = 0;
                    }
                }
                else
                {
                    MoveAt(head.next());
                    if ((rightPoint.X - head.next().getX()) * (rightPoint.X - head.next().getX()) + (rightPoint.Y - head.next().getY()) * (rightPoint.Y - head.next().getY()) < 100)
                    {
                        frameCnt++;
                    }
                    else
                    {
                        frameCnt = 0;
                    }
                    if (frameCnt > 20 && rightFlag)
                    {
                        Down(MouseButton.Right);
                        Up(MouseButton.Right); 
                        Up(MouseButton.Left);
                        frameCnt = 0;
                        rightFlag = false;
                    }
                }
            }
            // Point가 둘일때 Touch Gesture 사용 
            else if (pointCnt == 2 && mode == 1)
            {
                moveLimit = 100;
                double seta = 45.0, avg_Seta = 0, seta1 = 0, seta2 = 0;
                double pre_distance = 0, cur_distance;

                Up(MouseButton.Left);
                gestureFlag = true;

                if (!head.next().next().getclick())
                {
                    pre_Point1.X = head.next().getX();
                    pre_Point1.Y = head.next().getY();
                    pre_Point2.X = head.next().next().getX();
                    pre_Point2.Y = head.next().next().getY();
                    head.next().setclick(true);
                    head.next().next().setclick(true);
                }
                else
                {
                    double x1, y1, x2, y2;
                    pre_distance = (pre_Point2.X - pre_Point1.X) * (pre_Point2.X - pre_Point1.X) + (pre_Point2.Y - pre_Point1.Y) * (pre_Point2.Y - pre_Point1.Y);
                    x1 = head.next().getX() - pre_Point1.X;
                    y1 = head.next().getY() - pre_Point1.Y;
                    seta1 = Math.Atan2(-y1, x1) * 180 / Math.PI;
                    x2 = head.next().next().getX() - pre_Point2.X;
                    y2 = head.next().next().getY() - pre_Point2.Y;
                    seta2 = Math.Atan2(-y2, x2) * 180 / Math.PI;

                    // 두각의 차이값을 최대 180도 지정
                    if (Math.Abs(seta1 - seta2) <= 180)
                        seta = Math.Abs(seta1 - seta2);
                    else
                        seta = 360 - Math.Abs(seta1 - seta2);

                    pre_Point1.X = head.next().getX();
                    pre_Point1.Y = head.next().getY();
                    pre_Point2.X = head.next().next().getX();
                    pre_Point2.Y = head.next().next().getY();
                }

                // 스크롤 이벤트
                if (seta <= 30 && seta != 0)
                {
                    //예외처리
                    if (seta1 * seta2 < 0)
                    {
                        if (seta1 < 0)
                            seta1 += 360;
                        else
                            seta2 += 360;
                    }
                    //두점의 평균방향
                    avg_Seta = (seta1 + seta2) / 2.0;
                    //예외처리
                    if (avg_Seta > 180)
                        avg_Seta -= 360;

                    //스크롤 방향


                    //휠 다운
                    if (avg_Seta >= 30 && avg_Seta <= 150)
                    {
                        Down(MouseButton.Wheel);
                        Down(MouseButton.Wheel);
                    }
                    //휠 업
                    else if (avg_Seta <= -30 && avg_Seta >= -150)
                    {
                        Up(MouseButton.Wheel);
                        Up(MouseButton.Wheel);
                    }
                }
                //확대 축소 이벤트
                else if (seta >= 90 || (seta == 0 && (seta1 != 0 || seta2 != 0)))
                {
                    cur_distance = (pre_Point2.X - pre_Point1.X) * (pre_Point2.X - pre_Point1.X) + (pre_Point2.Y - pre_Point1.Y) * (pre_Point2.Y - pre_Point1.Y);
                    //확대
                    if (cur_distance - pre_distance > 400)
                    {
                        keybd_event((byte)Keys.ControlKey, 0, 0, 0);
                        Up(MouseButton.Wheel);
                        Up(MouseButton.Wheel);
                        Up(MouseButton.Wheel);
                        keybd_event((byte)Keys.ControlKey, 0, 0x02, 0);
                    }
                    //축소
                    else if (cur_distance - pre_distance < -400)
                    {
                        keybd_event((byte)Keys.ControlKey, 0, 0, 0);
                        Down(MouseButton.Wheel);
                        Down(MouseButton.Wheel);
                        Down(MouseButton.Wheel);
                        keybd_event((byte)Keys.ControlKey, 0, 0x02, 0);
                    }
                }
                moveLimit = 49;
            }
            // 다인모드
            else if (mode == 2)
            {
                TouchPoint T;
                for (T = head; T.next() != null; T = T.next())
                {
                    if (!T.next().getclick())
                    {
                        MoveAt(T.next());
                        Down(MouseButton.Left);
                        T.next().setclick(true);
                    }
                }
            }

        }

        // 마우스 좌표 이동
        public void MoveAt(TouchPoint T)
        {
            SetCursorPos((int)((T.getX() - screenLT.X) / imageX * computerX), (int)((T.getY() - screenLT.Y) / imageY * computerY));
        }

        // 마우스 Down 이벤트
        public void Down(MouseButton Button)
        {
            MouseEventFlag Flag = new MouseEventFlag();

            switch (Button)
            {
                case MouseButton.Left: Flag = MouseEventFlag.LeftDown; break;
                case MouseButton.Right: Flag = MouseEventFlag.RightDown; break;
                case MouseButton.Middle: Flag = MouseEventFlag.MiddleDown; break;
                case MouseButton.X: Flag = MouseEventFlag.XDown; break;
                case MouseButton.Wheel: Flag = MouseEventFlag.Wheel; break;
            }
            if (Flag == MouseEventFlag.Wheel)
                mouse_event((int)Flag, 0, 0, -30, IntPtr.Zero);

            mouse_event((int)Flag, 0, 0, 0, IntPtr.Zero);
        }

        // 마우스 Up 이벤트
        public void Up(MouseButton Button)
        {
            MouseEventFlag Flag = new MouseEventFlag();

            switch (Button)
            {
                case MouseButton.Left: Flag = MouseEventFlag.LeftUp; break;
                case MouseButton.Right: Flag = MouseEventFlag.RightUp; break;
                case MouseButton.Middle: Flag = MouseEventFlag.MiddleUp; break;
                case MouseButton.X: Flag = MouseEventFlag.XUp; break;
                case MouseButton.Wheel: Flag = MouseEventFlag.Wheel; break;
            }

            if (Flag == MouseEventFlag.Wheel)
                mouse_event((int)Flag, 0, 0, 30, IntPtr.Zero);
            mouse_event((int)Flag, 0, 0, 0, IntPtr.Zero);
        }

        // SCREENSIZE 재설정 함수
        private void ScreenSize_Click(object sender, RoutedEventArgs e)
        {
            ScreenDraw = true;
            screen.StrokeThickness = 0;
            screen.Width = 640;
            screen.Height = 480;
            Canvas.SetLeft(screen, 0);
            Canvas.SetTop(screen, 0);
            screenLT.X = 0;
            screenLT.Y = 0;
            mouseUse = false;
        }
        
        // 초기 Depth값 재설정 함수
        private void DepthInit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 480; i++)
                for (int j = 0; j < 640; j++)
                    initdepthPixel[i, j] = depthPixels[640 * i + j].Depth;
        }

        // Kinect 센서 Start & Stop 함수
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (start)
                start = false;
            else
                start = true;
        }
        // 사용자 모드 설정 함수
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeSelect.SelectedIndex == 0)
                mode = 1;
            else
                mode = 2;
        }
        // 종료 함수
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        

    }
}