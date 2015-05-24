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
using System.Drawing;
using System.Drawing.Imaging;

class MouseEvent
{
    [DllImport("User32")]
    public static extern int GetCursorPos(out System.Windows.Point p);
}

namespace KinectUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToolBar : Window
    {
        //Linked List로 터치 포인트 구현
        public class TouchPoint
        {
            private double X;               // X좌표
            private double Y;               // Y좌표
            private bool exist;             // 이전 프레임과 존재여부 판별
            private bool click;             // 클릭이벤트 발생 Flag
            private TouchPoint nextPoint;   // 짧은 프레임 내 포인트 삭제 방지
            public int revision;
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
                //this.nextPoint = null;
                T.setX(-10.0);
                T.setY(-10.0);
                this.nextPoint = T.next();
                //T.nextPoint = null;
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

        public ToolBar()
        {
            this.Topmost = true;
            InitializeComponent();

            for (int i = 0; i < 4; i++)
                screenPoint[i] = new System.Windows.Point(0, 0);

            myEllipse = new Ellipse[4];
            for (int i = 0; i < 4; i++)
                myEllipse[i] = new Ellipse();

            head = new TouchPoint(-100.0, -100.0);
            pre_Point1 = new System.Windows.Point(0, 0);
            pre_Point2 = new System.Windows.Point(0, 0);

        }
        
        //init
        public KinectSensor nui;                               // 키넥트 센서 변수
        public SetUp setup;                                    // 옵션창 객체 생성
        PenTool pentool; 
        Process keyProcess;                                    // 키보드 프로세스
        public DepthImagePixel[] depthPixels;                  // 현재 Depth 저장
        public DepthImagePixel[] initDepthPixels;              // 초기 Depth 저장
        public short[,] depthPixel = new short[480, 640];      // 화면 좌우 반전을 위해 2차원 배열로 저장
        public short[,] initdepthPixel = new short[480, 640];  // 화면 좌우 반전을 위해 2차원 배열로 저장
        public WriteableBitmap colorBitmap;                    // 화면의 출력할 Color Map
        public TouchPoint head;                                // TouchPoint Head
        public byte[] VirKey = new byte[256];                  // 키보드
        public System.Windows.Point pre_Point1, pre_Point2;    // TouchPoint 저장변수
        public System.Windows.Point rightPoint;                // 우클릭 인식 좌표
        public System.Windows.Point[] screenPoint = new System.Windows.Point[4];             // SCREEN 4 모퉁이 좌표
        public System.Windows.Point screenLT = new System.Windows.Point(0, 0);               // SCREEN LeftTop 위치
        public System.Windows.Shapes.Rectangle screen;         // SCREEN 사각형
        public Ellipse[] myEllipse;                            // TouchPoint 위치 생성할 원
        public bool start = false;                             // 키넥트센서 시작 정지 
        public bool init = true;                               // 초기 Depth 저장을 위한 변수
        public bool ScreenDraw = false;                        // SCREEN 캘리 변수  
        public bool mouseUse = false;                          // 마우스사용 Flag                         
        public bool rightFlag = true;                          // 우클릭 이벤트 Flag
        public bool gestureFlag = false;                       // 제스쳐 이벤트 Flag
        public bool setupWindow = false;                       // 옵션창 생성 Flag
        public bool penTool_Flag = false;                      // 펜툴창 생성 Flag
        public byte[] colorPixels;                             // Depth값을 Color값으로 변경
        public int moveLimit = 49;                             // TouchPoint 변화보정 변수
        public int screenCnt = 0;                              // SCREEN 모서리 생성 개수
        public int pixelCnt = 0;                               // TouchPoint의 최소 픽셀 개수
        public int pointCnt = 0;                               // TouchPoint 개수
        public int s_Row = 0;                                  // TouchPoint를 만드는 픽셀의 가로합
        public int s_Col = 0;                                  // TouchPoint를 만드는 픽셀의 세로합
        public int mode = 1;                                   // Mode 설정
        public int frameCnt = 0;                               // 프레임 카운트
        public int UserCnt = 50;                               // 사용자 숫자
        public double imageX;                                  // SCREEN Width
        public double imageY;                                  // SCREEN Height
        public double computerX = 1600.0;                      // 화면 해상도 Width
        public double computerY = 900.0;                       // 화면 해상도 Height     

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
        
        // 마우스, 키보드 사용하기위한 함수 
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("user32.dll")]
        static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);     
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_RESTORE = 0xF120;

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
                return;
            }
            screen = new System.Windows.Shapes.Rectangle();
            screen.Stroke = System.Windows.Media.Brushes.Red;
            screen.StrokeThickness = 2;
            screen.Width = 640;
            screen.Height = 480;
            
            GetKeyboardState(VirKey);

            nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            depthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            initDepthPixels = new DepthImagePixel[nui.DepthStream.FramePixelDataLength];
            colorPixels = new byte[nui.DepthStream.FramePixelDataLength * sizeof(int)];
            colorBitmap = new WriteableBitmap(nui.DepthStream.FrameWidth, nui.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            nui.DepthStream.Range = DepthRange.Near;

            nui.DepthFrameReady += nui_DepthFrameReady;
            nui.Start();

            setup = new SetUp();
            setup.Owner = this;
            setup.Show();
            ((App)System.Windows.Application.Current).SetUp.Add(setup);
        
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
                            short subDepth;
                            if (currentDepth > 2000 || initDepth > 2000)
                                subDepth = 0;
                            else
                                subDepth = (short)(initDepth - currentDepth);

                            if (subDepth < 0)
                                subDepth = 0;
                            if (0 <= subDepth && subDepth <= 7)
                            {
                                colorPixels[colorPixelIndex++] = 255;
                                colorPixels[colorPixelIndex++] = 0;
                                colorPixels[colorPixelIndex++] = 0;
                            }
                            else if (7 < subDepth && subDepth <= 14)
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

                            if (pixelCnt > 40 && pixelCnt <400)  // 일정 픽셀이상 Point 인식
                            {
                                /*
                                if (head.next() != null) // 생성된 점이 
                                    refresh(s_Row / pixelCnt, s_Col / pixelCnt);
                                else
                                    add(s_Row / pixelCnt, s_Col / pixelCnt);
                                */
                                refresh(s_Row / pixelCnt, s_Col / pixelCnt);
                            }
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

                    //마우스 이벤트 함수
                    if (mouseUse)
                        mouseEvents();

                    // 사용자 제한수 보다 많은 입력 즉 뎁스값 오류면 옵션창 호출
                    if (pointCnt > UserCnt)
                    {
                        if (!optionFlag)
                        {
                            var uriSource = new Uri(@"images\option_1_hover.png", UriKind.Relative);
                            option.Source = new BitmapImage(uriSource);
                            setup = new SetUp();
                            setup.Owner = this;
                            setup.Show();
                            ((App)System.Windows.Application.Current).SetUp.Add(setup);
                            optionFlag = true;
                        }
                    }
                    colorBitmap.WritePixels(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight), colorPixels, colorBitmap.PixelWidth * sizeof(int), 0);

                }
            }
        }

        string keyboardName = @"C:\Users\tnt_h_000\Desktop\MYTH\q\keyboardTool.exe";
        string filenameStr = "";
        string filename()
        {
            return string.Format("{0}.png", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }

        bool openFlag = true;
        public bool optionFlag = true;
        bool penFlag = false;
        bool keyboardFlag = false;
        bool captureFlag = false;
        bool usermodeFlag = false;

        // 오픈 버튼 이벤트
        private void open_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!openFlag)
            {
                var uriSource = new Uri(@"images\open.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\close.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);
            }
        }
        private void open_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!openFlag)
            {
                var uriSource = new Uri(@"images\open_hover.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\close_hover.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);
            }
        }
        private void open_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!openFlag)
            {

                var uriSource = new Uri(@"images\close_hover.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);

                //create menu
                option.Opacity = 1;
                pen.Opacity = 1;
                keyboard.Opacity = 1;
                capture.Opacity = 1;
                usermode.Opacity = 1;
                quit.Opacity = 1;

                openFlag = !openFlag;
            }
            else
            {

                var uriSource = new Uri(@"images\open_hover.png", UriKind.Relative);
                open.Source = new BitmapImage(uriSource);

                // remove menu
                option.Opacity = 0;
                pen.Opacity = 0;
                keyboard.Opacity = 0;
                capture.Opacity = 0;
                usermode.Opacity = 0;
                quit.Opacity = 0;

                openFlag = !openFlag;

            }
        }

        // 펜 버튼 이벤트
        private void pen_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!penFlag)
            {
                var uriSource = new Uri(@"images\pen.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\pen_1.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
            }
        }
        private void pen_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!penFlag)
            {
                var uriSource = new Uri(@"images\pen_hover.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\pen_1_hover.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
            }
        }
        private void pen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!penFlag)
            {
                var uriSource = new Uri(@"images\pen_1_hover.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
                inkCanvas.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#01898989");
                moveLimit = 3;
                penFlag = true;

            }
            else
            {
                penFlag = false;
                var uriSource = new Uri(@"images\pen.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);
                inkCanvas.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#00898989");
                moveLimit = 49;
                inkCanvas.Strokes.Clear();
                if (pentool != null)
                {
                    pentool.Close();
                    penTool_Flag = false;
                }

            }
        }

        // 키보드 버튼 이벤트
        private void keyboard_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!keyboardFlag)
            {
                var uriSource = new Uri(@"images\keyboard.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\keyboard_1.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
            }
        }
        private void keyboard_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!keyboardFlag)
            {
                var uriSource = new Uri(@"images\keyboard_hover.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\keyboard_1_hover.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
            }
        }
        private void keyboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!keyboardFlag)
            {
                var uriSource = new Uri(@"images\keyboard_1_hover.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
                keyboardFlag = true;

                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "keyboardTool")
                    {
                        SetForegroundWindow(proc.MainWindowHandle);
                        return;
                    }
                }

                keyProcess = new Process();
                keyProcess.StartInfo.FileName = keyboardName;
                keyProcess.Start();
                keyProcess.Dispose();
            }
            else
            {
                var uriSource = new Uri(@"images\keyboard_hover.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
                keyboardFlag = false;
                Process[] processList = Process.GetProcessesByName("keyboardTool");
                processList[0].Kill();
            }
        }

        // 캡쳐 버튼 이벤트
        private void capture_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!captureFlag)
            {
                var uriSource = new Uri(@"images\capture.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\capture_1.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);
            }
        }
        private void capture_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!captureFlag)
            {
                var uriSource = new Uri(@"images\capture_hover.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\capture_1_hover.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);
            }
        }
        private void capture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!captureFlag)
            {
                var uriSource = new Uri(@"images\capture_1_hover.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);

                Bitmap printScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(printScreen);
                graphics.CopyFromScreen(0, 0, 0, 0, printScreen.Size);
                string fn = filename();
                filenameStr = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" + fn;
                printScreen.Save(filenameStr, ImageFormat.Png);
            }
        }


        // 유저모드 버튼 이벤트
        private void usermode_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!usermodeFlag)
            {
                var uriSource = new Uri(@"images\usermode.png", UriKind.Relative);
                usermode.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\usermode_1.png", UriKind.Relative);
                usermode.Source = new BitmapImage(uriSource);
            }
        }
        private void usermode_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!usermodeFlag)
            {
                var uriSource = new Uri(@"images\usermode_hover.png", UriKind.Relative);
                usermode.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\usermode_1_hover.png", UriKind.Relative);
                usermode.Source = new BitmapImage(uriSource);
            }
        }
        private void usermode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!usermodeFlag)
            {
                var uriSource = new Uri(@"images\usermode_1_hover.png", UriKind.Relative);
                usermode.Source = new BitmapImage(uriSource);
                mode = 2;
                usermodeFlag = true;

            }
            else
            {
                usermodeFlag = false;
                var uriSource = new Uri(@"images\usermode.png", UriKind.Relative);
                mode = 1;
                usermode.Source = new BitmapImage(uriSource);

            }
        }

        // 옵션 버튼 이벤트
        private void option_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!optionFlag)
            {
                var uriSource = new Uri(@"images\option.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\option_1.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
        }
        private void option_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (!optionFlag)
            {
                var uriSource = new Uri(@"images\option_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\option_1_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
        }
        private void option_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!optionFlag)
            {
                var uriSource = new Uri(@"images\option_1_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
                setup = new SetUp();
                setup.Owner = this;
                setup.Show();
                ((App)System.Windows.Application.Current).SetUp.Add(setup);
                optionFlag = true;
            }
            else
            {
                var uriSource = new Uri(@"images\option.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
                setup.Close();
                optionFlag = false;
            }

        }

        // 종료 버튼 이벤트
        private void quit_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var uriSource = new Uri(@"images\quit_2.png", UriKind.Relative);
            quit.Source = new BitmapImage(uriSource);
        }
        private void quit_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var uriSource = new Uri(@"images\quit_hover.png", UriKind.Relative);
            quit.Source = new BitmapImage(uriSource);
        }
        private void quit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void Preview_Mouse_Right_Button_Down(object sender, MouseButtonEventArgs e)
        {
            if (penTool_Flag == false)
            {
                penTool_Flag = true;
                pentool = new PenTool();
                pentool.Owner = this;
                System.Windows.Point p;
                p = new System.Windows.Point(0,0);
                MouseEvent.GetCursorPos(out p);
                pentool.Left = p.X;
                pentool.Top = p.Y;
                pentool.Show();
                ((App)System.Windows.Application.Current).PenTool.Add(pentool);
            }

            else
            {
                penTool_Flag = false;
                pentool.Close();
            }
        }

        // TouchPoint Search 함수
        public void setPoint(int i)
        {
            if (i < 0 || i >= 307200)
                return;
            else if (pixelCnt > 400)
                return;
            else if (colorPixels[i * 4 + 1] == 0 && colorPixels[i * 4 + 2] == 255)
            {
                colorPixels[i * 4 + 1] = 1; // 한번 지나간곳은 +로 바꿔서 안들리게함
                pixelCnt++; // 픽셀카운트
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
            int[] minX = new int[2] { 700, 700 };
            int[] minY = new int[2] { 700, 700 };
            int[] maxX = new int[2] { -1, -1 };
            int[] maxY = new int[2] { -1, -1 };
            
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
                screenLT.X = (minX[0] + minX[1]) / 2;
                screenLT.Y = (minY[0] + minY[1]) / 2;
                screen.StrokeThickness = 2;
                Canvas.SetLeft(screen, screenLT.X);
                Canvas.SetTop(screen, screenLT.Y);
                ScreenDraw = false;
                screenCnt = 0;
                for (int i = 0; i < 4; i++)
                {
                    Canvas.SetLeft(myEllipse[i], -100);
                    Canvas.SetTop(myEllipse[i], -100);
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
                if (save)
                {
                    screenPoint[screenCnt].X = head.next().getX();
                    screenPoint[screenCnt].Y = head.next().getY();
                    screenCnt++;
                    Canvas.SetLeft(myEllipse[screenCnt - 1],head.next().getX());
                    Canvas.SetTop(myEllipse[screenCnt - 1], head.next().getY());
                }
                frameCnt = 0;
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
                        rightPoint = new System.Windows.Point(head.next().getX(), head.next().getY());
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
                    if (cur_distance - pre_distance > 900)
                    {
                        keybd_event((byte)Keys.ControlKey, 0, 0, 0);
                        Up(MouseButton.Wheel);
                        Up(MouseButton.Wheel);
                        Up(MouseButton.Wheel);
                        keybd_event((byte)Keys.ControlKey, 0, 0x02, 0);
                    }
                    //축소
                    else if (cur_distance - pre_distance < -900)
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
   
    }
}
