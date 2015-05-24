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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace KinectUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToolBar : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        public const int SW_RESTORE = 9;//window restore
        public const int SW_MINIMIZE = 6; //window minimize

        //option program's path; please modify path
        string exeName = @"C:\Users\ChoiYeojin\Documents\Visual Studio 2010\Projects\q\q\exefiles\Gerim.exe";
        //pen program's path; please modify path
        string optionName = @"C:\Users\ChoiYeojin\Documents\Visual Studio 2010\Projects\q\q\exefiles\righthand.exe";
        //image keyboard's path; please modify path
        string keyboardName = @"C:\Users\ChoiYeojin\Documents\Visual Studio 2010\Projects\q\q\exefiles\keyboardTool.exe";

        //capture image name buffer
        string filenameStr = "";
        //create capture image name
        string filename()
        {
            return string.Format("{0}.png", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }

        //button flags
        bool openFlag = false;
        bool optionFlag = true;
        bool penFlag = false;
        bool peopleFlag = false;
        bool keyboardFlag = false;
        bool captureFlag = false;
        bool first = true;

        public ToolBar()
        {
            InitializeComponent();
            //if you always want to using your tool, Topmost is necessary.
            this.Topmost = true;
        }

        //MouseEnter event : if your mouse is entered in the object, it occurs.
        //MouseLeave event : if your mouse is left at the object, it occurs.
        //MouseLeftButtonDown event : if your mouse left button is downed in the object, it occurs.
        
        //open button
        private void open_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!openFlag)
            {
                //change a button image
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
                quit.Opacity = 1;
                people.Opacity = 1;


                openFlag = !openFlag;

                foreach (var proc in Process.GetProcesses())
                {
                    if (first)
                    {
                        //initialize kinect option. It is necessary to initialize option at first time.
                        Process optionProcess = new Process();
                        optionProcess.StartInfo.FileName = optionName;
                        optionProcess.Start();
                        optionProcess.Dispose();
                        first = false;
                    }
                }
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
                quit.Opacity = 0;
                people.Opacity = 0;

                openFlag = !openFlag;
            }
        }

        //quit button
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
            //quit all programs
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "righthand")
                    proc.Kill();
                else if (proc.ProcessName == "osk")
                    proc.Kill();
                else if (proc.ProcessName == "Gerim")
                    proc.Kill();

                //please enter your codes if you want to kill process in people tab
            }

            //exit tool
            System.Environment.Exit(1);
        }

        //option button
        private void option_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (optionFlag)
            {
                var uriSource = new Uri(@"images\option_1.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\option.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
        }
        private void option_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (optionFlag)
            {
                var uriSource = new Uri(@"images\option_1_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\option_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
            }
        }
        private void option_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!optionFlag)
            {
                var uriSource = new Uri(@"images\option_1_hover.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);
                optionFlag = true;

                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "righthand")
                    {
                        // show window by restore mode
                        ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                        return;
                    }
                }

                // if there is no option program excuted, excute the program.
                Process optionProcess = new Process();
                optionProcess.StartInfo.FileName = optionName;
                optionProcess.Start();
                optionProcess.Dispose();
            }
            else
            {
                optionFlag = false;
                var uriSource = new Uri(@"images\option.png", UriKind.Relative);
                option.Source = new BitmapImage(uriSource);

                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "righthand")
                    {
                        //option program must not quit while this program is running.
                        //so, not kill the process but minimize window.
                        ShowWindow(proc.MainWindowHandle, SW_MINIMIZE);
                        return;
                    }
                }
            }

        }

        //pen button
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
                penFlag = true;

                //excute pen program
                Process penProcess = new Process();
                penProcess.StartInfo.FileName = exeName;
                penProcess.Start();
                penProcess.Dispose();

            }
            else
            {
                penFlag = false;
                var uriSource = new Uri(@"images\pen.png", UriKind.Relative);
                pen.Source = new BitmapImage(uriSource);

                //quit pen program
                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "Gerim")
                    {                        
                        proc.Kill();
                        return;
                    }
                }
            }
        }

        //keyboard button
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

                //excute keyboard program
                Process keyProcess = new Process();
                keyProcess.StartInfo.FileName = keyboardName;
                keyProcess.Start();
                keyProcess.Dispose();
            }
            else
            {
                var uriSource = new Uri(@"images\keyboard_hover.png", UriKind.Relative);
                keyboard.Source = new BitmapImage(uriSource);
                keyboardFlag = false;

                //quit keyboard program
                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "keyboardTool")
                    {
                        proc.Kill();
                        break;
                    }
                }
            }
        }

        //capture button
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
                captureFlag = true;

                // get printscreen image
                Bitmap printScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(printScreen);
                graphics.CopyFromScreen(0, 0, 0, 0, printScreen.Size);

                //create file name
                string fn = filename();

                //get desktop's path
                filenameStr = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" + fn;

                //save printscreen image file by file name created before at desktop's path
                printScreen.Save(filenameStr, ImageFormat.Png);
            }
            else
            {
                var uriSource = new Uri(@"images\capture_hover.png", UriKind.Relative);
                capture.Source = new BitmapImage(uriSource);
                captureFlag = false;
            }
        }

        //people button
        private void people_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!peopleFlag)
            {
                var uriSource = new Uri(@"images\people.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\people_1.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
            }
        }
        private void people_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!peopleFlag)
            {
                var uriSource = new Uri(@"images\people_hover.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
            }
            else
            {
                var uriSource = new Uri(@"images\people_1_hover.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
            }
        }
        private void people_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!peopleFlag)
            {
                var uriSource = new Uri(@"images\people_1_hover.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
                peopleFlag = true;

                // Please Enter your codes like above if you want to excute your program.
            }
            else
            {
                var uriSource = new Uri(@"images\people_hover.png", UriKind.Relative);
                people.Source = new BitmapImage(uriSource);
                peopleFlag = false;

                // Please Enter your codes like above if you want to quit your program.
            }
        }
    }
}
