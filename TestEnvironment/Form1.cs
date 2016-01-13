using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using RainstormStudios;
using RainstormStudios.Unmanaged;
using RainstormStudios.Drawing.Charts;

namespace TestEnvironment
{
    public partial class Form1 : Form
    {
        PieChart pie = new PieChart();

        public Form1()
        {
            InitializeComponent();
            pie.Use3dEffect = true;
            pie.UseMultisampling = true;
            pie.HighQualityMultiSampling = true;
            pie.TotalValue = 100;
            pie.AngleOfTilt = 25;
            pie.SliceDivideLines = true;
            pie.Slices.Add(new PieChartSlice(20, Color.Red, "Bats", true));
            pie.Slices.Add(new PieChartSlice(40, Color.Blue, "Balls", true));
            pie.Slices.Add(new PieChartSlice(30, Color.Green, "Gloves", true));
            pie.Slices.Add(new PieChartSlice(10, Color.Yellow, "Cards", false));
            //this.pictureBox1.Image = pie.GetImage(pictureBox1.Size);

        }

        //private void cmdRun_Click(object sender, EventArgs e)
        //{
        //    InteropShellExecute shEx = new InteropShellExecute();
        //    shEx.hwnd = Api_User32.GetActiveWindow();
        //    shEx.lpDirectory = System.IO.Path.GetDirectoryName(txtAssembly.Text);
        //    shEx.lpFile = System.IO.Path.GetFileName(txtAssembly.Text);
        //    shEx.lpOperation = "open";
        //    shEx.lpParameters = "c:\\timezoneedit.log";
        //    shEx.lpShowCmd = InteropShellExecute.ShowCommands.SW_NORMAL;
        //    shEx.Execute();
        //}

        //private void cmdCopy_Click(object sender, EventArgs e)
        //{
        //    InteropShellFileOperation shFile = new InteropShellFileOperation();
        //    shFile.lpszProgressTitle = "Now Copyin' Yo Stuff...";
        //    shFile.wFunc = InteropShellFileOperation.FO_Func.FO_COPY;
        //    shFile.pFrom = "c:\\downloads\\*.*";
        //    shFile.pTo = "c:\\temp\\demoCopy\\";
        //    shFile.hwnd = Api_User32.GetActiveWindow();
        //    shFile.fFlags.FOF_FILESONLY = true;
        //    shFile.fFlags.FOF_NORECURSION = true;
        //    shFile.fFlags.FOF_SIMPLEPROGRESS = false;
        //    shFile.fFlags.FOF_RENAMEONCOLLISION = false;
        //    shFile.Execute();
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            pie.Slices[0].DrawSlice(e.Graphics, new Rectangle(10, 10, 50, 50));
        }

        private void advancedButton1_Click(object sender, EventArgs e)
        {
            IntPtr ieFrm = Win32.FindWindow("Untitled - Notepad");
            if (ieFrm != IntPtr.Zero)
            {
                Win32.RedrawWindow(ieFrm);
            }
            else
                MessageBox.Show(this, "Couldn't find IE", "Sorry");

            //RainstormStudios.Unmanaged.Api_User32.EnumWindows(new Api_User32.EnumWindowEventHandler(this.EnumWindowEventHandler_onCallback), 0);
            //this.listBox1.Items.Add(RainstormStudios.Unmanaged.Api_AdvApi32.GetUserSid("sparkenergy\\munfried"));
            //this.listBox1.Items.Add(RainstormStudios.Unmanaged.Api_NetApi32.GetJoinedDomain());
            //RainstormStudios.Unmanaged.Api_NetApi32.NetJoinStatus stat; IntPtr dmn;
            //this.listBox1.Items.Add(RainstormStudios.Unmanaged.Api_NetApi32.NetGetJoinInformation("Utility", out dmn, out stat));
            //this.listBox1.Items.Add(stat);
            //this.listBox1.Items.Add(dmn);
        }

        private bool EnumWindowEventHandler_onCallback(IntPtr hwnd, Int32 lParam)
        {
            this.listBox1.Items.Clear();
            //Api_User32.WindowInfo wInfo = Api_User32.WindowInfo.Empty;
            WindowInfo wInfo = Win32.GetWindowInfo(hwnd);
            {
                this.listBox1.Items.Add("atomWindowType: " + wInfo.atomWindowtype.ToString());
                this.listBox1.Items.Add(string.Format("client: {0}.{1} -> {2}x{3}", wInfo.client.Left, wInfo.client.Top, wInfo.client.Width, wInfo.client.Height));
                this.listBox1.Items.Add("creatorVersion: " + wInfo.creatorVersion.ToString());
                this.listBox1.Items.Add("exStyle: " + wInfo.exStyle.ToString());
                this.listBox1.Items.Add("size: " + wInfo.cbSize.ToString());
                this.listBox1.Items.Add("windowStatus: " + wInfo.windowStatus.ToString());
                this.listBox1.Items.Add("xWindowBorders: " + wInfo.xWindowBorders.ToString());
                this.listBox1.Items.Add("yWindowBorders: " + wInfo.yWindowBorders.ToString());
                //StringBuilder bld = new StringBuilder(50);
                //this.listBox1.Items.Add(RainstormStudios.Unmanaged.Api_User32.GetWindowText(hwnd, bld, 50));
                //this.listBox1.Items.Add(bld);
                this.listBox1.Items.Add(Win32.GetWindowText(hwnd).Trim());
            }
            return true;
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {
            //lblTest1.Text = "Form Drag Leave Event Triggered";
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            lblTest1.Text = "Form Drop Event Triggered";
        }

        private void pictureBox2_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //lblTest1.Text = e.Action.ToString();
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            lblTest1.Text = "Drag Opp Started";
            ((PictureBox)sender).DoDragDrop(sender, DragDropEffects.Copy);
        }

        private void pictureBox2_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //lblTest1.Text = e.Effect.ToString();
            //IntPtr hwnd = RainstormStudios.Unmanaged.Api_User32.WindowFromPoint(new Point(Control.MousePosition.X, Control.MousePosition.Y));
            IntPtr hwnd = Win32.GetWindowFromPoint(Control.MousePosition.X, Control.MousePosition.Y);
            //StringBuilder bld = new StringBuilder(50);
            //RainstormStudios.Unmanaged.Api_User32.GetClassName(hwnd, bld, 50);
            //lblTest1.Text = bld.ToString();
            lblTest1.Text = Win32.GetClassName(hwnd).Trim();
            hwnd = IntPtr.Zero;
        }

        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            lblTest1.Text = "Pic Drop Event Triggered";
            //IntPtr hwnd = RainstormStudios.Unmanaged.Api_User32.WindowFromPoint(new Point(e.X, e.Y));
            IntPtr hwnd = Win32.GetWindowFromPoint(e.X, e.Y);
            //StringBuilder bld = new StringBuilder(50);
            //this.listBox1.Items.Add(RainstormStudios.Unmanaged.Api_User32.GetWindowText(hwnd, bld, 50));
            //this.listBox1.Items.Add(bld);
            this.listBox1.Items.Add(Win32.GetWindowText(hwnd).Trim());
            hwnd = IntPtr.Zero;
            e.Effect = DragDropEffects.None;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void advancedButton2_Click(object sender, EventArgs e)
        {
            //IntPtr ieFrm = Win32.FindWindow("PGPCmdLineGuide.pdf - Adobe Reader");
            IntPtr ieFrm = Win32.FindWindow("Untitled - Notepad");
            if (ieFrm != IntPtr.Zero)
            {
                IntPtr ieScr = Win32.FindWindowEx(ieFrm, "edit");
                if (ieScr != IntPtr.Zero)
                {
                    Win32.RedrawWindow(ieScr, RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_NOERASE);
                    Rectangle bnds = Win32.GetWindowRect(ieScr);
                    Rectangle clBnds = Win32.GetClientRect(ieScr);
                    using (DeviceContext dc = DeviceContext.GetWindow(ieScr))
                    {
                        dc.ManagedGraphics.FillRectangle(SystemBrushes.HotTrack, clBnds);
                        using (Font font = new Font("Tahoma", 9.0f, FontStyle.Underline))
                            dc.ManagedGraphics.DrawString(string.Format("{0} --> {1}", ieFrm, ieScr), font, SystemBrushes.ControlText, 10.0f, 10.0f);
                        //Win32.SetMousePosition(bnds.Left + 5, bnds.Top + 5);
                    }
                }
                else
                    MessageBox.Show(this, "Couldn't find scroll bar", "Sorry");
            }
            else
                MessageBox.Show(this, "Couldn't find IE", "Sorry");
            //IntPtr dc = IntPtr.Zero;
            //try
            //{
            //    IntPtr hwndExplorer = Api_User32.FindWindow("notepad", "Untitled - Notepad");
            //    if (hwndExplorer != IntPtr.Zero)
            //    {
            //        IntPtr hwndClock = Api_User32.FindWindowEx(hwndExplorer, IntPtr.Zero, "edit", "");
            //        if (hwndClock != IntPtr.Zero)
            //        {
            //            dc = Api_Gdi32.CreateDC(IntPtr.Zero, Screen.PrimaryScreen.DeviceName, hwndClock, IntPtr.Zero);
            //            using (Graphics g = Graphics.FromHdc(dc))
            //            {
            //                Api_User32.Rect bounds = new Api_User32.Rect();
            //                Api_User32.GetWindowRect(hwndClock, ref bounds);
            //                g.FillRectangle(SystemBrushes.HotTrack, new Rectangle(bounds.left, bounds.top, bounds.right - bounds.left, bounds.bottom - bounds.top));
            //                Api_User32.SetFocus(hwndClock);
            //                Api_User32.INPUT[] inp = new Api_User32.INPUT[1];
            //                inp[0].type = Api_User32.INPUT_KEYBOARD;
            //                inp[0].ki.wScan = 0;
            //                inp[0].ki.time = 0;
            //                inp[0].ki.dwFlags = 0;
            //                inp[0].ki.dwExtraInfo = Api_User32.GetMessageExtraInfo();
            //                inp[0].ki.wVk = (ushort)Api_User32.VK.NUMPAD1;
            //                Api_User32.SendInput((uint)inp.Length, inp, System.Runtime.InteropServices.Marshal.SizeOf(inp[0]));
            //                inp[0].ki.dwFlags = Api_User32.KEYEVENTF_KEYUP;
            //                Api_User32.SendInput((uint)inp.Length, inp, System.Runtime.InteropServices.Marshal.SizeOf(inp[0]));
            //            }
            //        }
            //        else
            //            MessageBox.Show(this, "WTF?! Notepad doesn't have an Edit component????", "Sorry");
            //    }
            //    else
            //        MessageBox.Show(this, "Couldn't find 'Notepad'.", "Sorry");
            //}
            //catch (Exception ex)
            //{ MessageBox.Show(this, "Something bombed, dude:\n\n" + ex.ToString(), "Damn!"); }
            //finally
            //{ if (dc != IntPtr.Zero)Api_Gdi32.DeleteDC(dc); }
        }

        private void cmdSrchText_Click(object sender, EventArgs e)
        {
            //this.rtfRandomText.SetBackColor(this.txtFindWhat.Text, Color.Red, false, true);
            this.advancedButton3.Enabled = !this.advancedButton3.Enabled;
        }
    }
}