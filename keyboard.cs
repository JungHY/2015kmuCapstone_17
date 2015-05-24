using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace keyboardTool
{
    public partial class Form1 : Form
    {
        bool englishFlag; // false = 한글, true = 영어
        bool shiftFlag;
        bool altFlag;
        bool controlFlag;
        bool capslockFlag;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, ref int extrainfo);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000;
                return param;
            }
        }
        public Form1()
        {
            InitializeComponent();
            englishFlag = false;
            altFlag = false;
            shiftFlag = false;
            controlFlag = false;
        }
        // first lines
        private void esc_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{ESC}");
        }
        private void swung_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xC0;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }



        }
        private void one_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F1}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("1");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("!");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("!");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("1");
                }
            }
        }
        private void two_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F2}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("2");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("@");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("@");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("2");
                }
            }
        }
        private void three_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F3}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("3");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("#");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("#");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("3");
                }
            }
        }
        private void four_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F4}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("4");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("$");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("$");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("4");
                }
            }
        }
        private void five_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F5}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("5");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("{%}");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("{%}");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("5");
                }
            }
        }
        private void six_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F6}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("6");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("{^}");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("{^}");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("6");
                }
            }
        }
        private void seven_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F7}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("7");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("&");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("&");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("7");
                }
            }
        }
        private void eight_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F8}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("8");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("*");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("*");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("8");
                }
            }
        }
        private void nine_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F9}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("9");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("{(}");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("{(}");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("9");
                }
            }
        }
        private void zero_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F10}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("0");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("{)}");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("{)}");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("0");
                }
            }
        }
        private void dash_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F11}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("-");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("_");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("_");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("-");
                }
            }
        }
        private void equal_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                SendKeys.SendWait("{F12}");
                function.CheckState = CheckState.Unchecked;
            }

            else
            {
                if (capslock.Checked)
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("=");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("{+}");
                }
                else
                {
                    if (leftshift.Checked || rightshift.Checked)
                    {
                        SendKeys.SendWait("{+}");
                        if (leftshift.Checked)
                            leftshift.CheckState = CheckState.Unchecked;

                        if (rightshift.Checked)
                            rightshift.CheckState = CheckState.Unchecked;
                    }
                    else
                        SendKeys.SendWait("=");
                }
            }
        }
        private void backspace_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{BACKSPACE}");
        }
        private void home_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x11;
            const int Key_Up = 0x0002;
            int info = 0;
            keybd_event(Key_value, 0, Key_Up, ref info);
        }
        private void pageup_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{PGUP}");
        }

        //second lines
        private void tab_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{TAB}");
        }
        private void q_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x51;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void w_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x57;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void e_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x45;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void r_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x52;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void t_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x54;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void y_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x59;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void u_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x55;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void i_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x49;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void o_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4F;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void p_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x50;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void bigleft_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xDB;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void bigright_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xDD;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void won_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xDD;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void delete_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{DELETE}");
        }
        private void end_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{END}");
        }
        private void pagedown_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{PGDN}");
        }

        //third lines
        private void a_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x41;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void s_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x53;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void d_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x44;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void f_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x46;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void g_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x47;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void h_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x48;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void j_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4A;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void k_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4B;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void l_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4C;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void semicolon_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xBA;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void quote_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0xDE;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void enter_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{ENTER}");
        }
        private void insert_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{INSERT}");
        }
        private void pause_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{BREAK}");
        }

        //fourth lines
        private void z_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x5A;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void x_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x58;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void c_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x43;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void v_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x56;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void b_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x42;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void n_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4E;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void m_Click(object sender, EventArgs e)
        {
            const byte Key_value = 0x4D;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }


        }
        private void comma_Click(object sender, EventArgs e)
        {

            const byte Key_value = 0xBC;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void period_Click(object sender, EventArgs e)
        {

            const byte Key_value = 0xBE;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void slash_Click(object sender, EventArgs e)
        {

            const byte Key_value = 0xBF;
            const int Key_Up = 0x0002;
            int info = 0;

            keybd_event(Key_value, 0, 0, ref info);
            keybd_event(Key_value, 0, Key_Up, ref info);

            if (shiftFlag == true)
            {
                const byte Right_shift = 0xA1;
                const byte Left_shift = 0xA0;
                if (rightshift.Checked == true)
                {
                    keybd_event(Right_shift, 0, Key_Up, ref info);
                    rightshift.Checked = false;
                }
                if (leftshift.Checked == true)
                {
                    keybd_event(Left_shift, 0, Key_Up, ref info);
                    leftshift.Checked = false;
                }
                shiftFlag = false;
            }

            if (altFlag == true)
            {
                const byte Alt = 0x12;
                if (leftalt.Checked == true || rightalt.Checked == true)
                {
                    if (leftalt.Checked == true)
                    {
                        leftalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    if (rightalt.Checked == true)
                    {
                        rightalt.Checked = false;
                        keybd_event(Alt, 0, Key_Up, ref info);
                    }

                    altFlag = false;
                }

            }

            if (controlFlag == true)
            {
                const byte Right_Control = 0xA3;
                const byte Left_Control = 0xA2;

                if (rightcontrol.Checked == true)
                {
                    keybd_event(Right_Control, 0, Key_Up, ref info);
                    rightcontrol.Checked = false;
                }

                if (leftcontrol.Checked == true)
                {
                    keybd_event(Left_Control, 0, Key_Up, ref info);
                    leftcontrol.Checked = false;
                }

                controlFlag = false;
            }

        }
        private void up_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{UP}");
        }
        private void shift_Click(object sender, EventArgs e)
        {
            const Byte rshift = 16;
            const int KEYUP = 0x0002;
            int Info = 0;

            if(shiftFlag == false)
            {
                if (leftshift.Checked == true)
                    rightshift.Checked = true;

                if (rightshift.Checked == true)
                    leftshift.Checked = true;
                shiftFlag = true;
                keybd_event(rshift, 0, 0, ref Info);

            }

            else
            {
                if (leftshift.Checked == false)
                    rightshift.Checked = false;
                if (rightshift.Checked == false)
                    leftshift.Checked = false;
                shiftFlag = false;
                keybd_event(rshift, 0, KEYUP, ref Info);
            }
        }
        private void printscreen_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{PRTSC}");
        }
        private void scrolllock_Click(object sender, EventArgs e)
        {
            //SendKeys.SendWait("{NUMLOCK}");
            const Byte scroll = 145;
            const int KEYUP = 0x0002;
            int Info = 0;

            keybd_event(scroll, 0, 0, ref Info);
            keybd_event(scroll, 0, KEYUP, ref Info);
            // if(scrolllock.Checked)
            //    Control.
        }

        //fifth lines
        private void window_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{WIN}");
        }
        private void hanja_Click(object sender, EventArgs e)
        {
            const Byte hanja = 25;
            const int KEYUP = 0x0002;
            int Info = 0;

            keybd_event(hanja, 0, 0, ref Info);
            keybd_event(hanja, 0, KEYUP, ref Info);
            //englishFlag = !englishFlag;
        }
        private void space_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait(" ");
        }
        private void english_Click(object sender, EventArgs e)
        {
            const Byte hanguel = 21;
            const int KEYUP = 0x0002;
            int Info = 0;

            keybd_event(hanguel, 0, 0, ref Info);
            keybd_event(hanguel, 0, KEYUP, ref Info);
            englishFlag = !englishFlag;

        }
        private void alt_Click(object sender, EventArgs e)
        {

        }
        private void control_Click(object sender, EventArgs e)
        {
            const Byte rcontrol = 0x11;
            const int KEYUP = 0x0002;
            int Info = 0;

            if (controlFlag == false)
            {
                if (rightcontrol.Checked == true)
                    leftcontrol.Checked = true;

                if (leftcontrol.Checked == true)
                    rightcontrol.Checked = true;

                controlFlag = true;
                keybd_event(rcontrol, 0, 0, ref Info);
            }

            else
            {
                if (rightcontrol.Checked == false)
                    leftcontrol.Checked = false;

                if (leftcontrol.Checked == false)
                    rightcontrol.Checked = false;

                controlFlag = false;
                keybd_event(rcontrol, 0, KEYUP, ref Info);
            }



        }
        private void left_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{LEFT}");
        }
        private void down_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{DOWN}");
        }
        private void right_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{RIGHT}");
        }
        private void function_Click(object sender, EventArgs e)
        {
            if (function.Checked)
            {
                one.Text = "F1";
                two.Text = "F2";
                three.Text = "F3";
                four.Text = "F4";
                five.Text = "F5";
                six.Text = "F6";
                seven.Text = "F7";
                eight.Text = "F8";
                nine.Text = "F9";
                zero.Text = "F10";
                dash.Text = "F11";
                equal.Text = "F12";
            }
            else
            {
                one.Text = "! 1";
                two.Text = "@ 2";
                three.Text = "# 3";
                four.Text = "$ 4";
                five.Text = "% 5";
                six.Text = "^ 6";
                seven.Text = "& 7";
                eight.Text = "* 8";
                nine.Text = "( 9";
                zero.Text = ") 0";
                dash.Text = "_ -";
                equal.Text = "+ =";
            }

        }

        private void leftshift_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void function_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (shiftFlag == true)
            {
                const Byte rshift = 16;
                const int KEYUP = 0x0002;
                int Info = 0;

                
                    if (leftshift.Checked == false)
                        rightshift.Checked = false;
                    if (rightshift.Checked == false)
                        leftshift.Checked = false;
                    shiftFlag = false;
                    keybd_event(rshift, 0, KEYUP, ref Info);
                
            }

            if(controlFlag == true)
            {

                const Byte rcontrol = 0x11;
                const int KEYUP = 0x0002;
                int Info = 0;

               
                    if (rightcontrol.Checked == false)
                        leftcontrol.Checked = false;

                    if (leftcontrol.Checked == false)
                        rightcontrol.Checked = false;

                    controlFlag = false;
                    keybd_event(rcontrol, 0, KEYUP, ref Info);
            }


        }


        private void capslock_Click(object sender, EventArgs e)
        {
            const Byte capslock_byte = 0x14;
            const int KEYUP = 0x0002;
            int Info = 0;

            if(capslockFlag == false)
            {
                capslockFlag = true;
                keybd_event(capslock_byte, 0, 0, ref Info);
                keybd_event(capslock_byte, 0, KEYUP, ref Info);
                capslock.Checked = true;
            }

            else
            {
                capslockFlag = false;
                keybd_event(capslock_byte, 0, 0, ref Info);
                keybd_event(capslock_byte, 0, KEYUP, ref Info);
                capslock.Checked = false;
            }
        }
    }
}
