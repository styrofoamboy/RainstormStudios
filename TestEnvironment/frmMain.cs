using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RainstormStudios.DynamicCodeGeneration;

namespace TestEnvironment
{
    public partial class frmMain : Form
    {
        RainstormStudios.Drawing.CircleF cir = RainstormStudios.Drawing.CircleF.Empty;
        RainstormStudios.Drawing.Charts.Pie.PieChart pc = new RainstormStudios.Drawing.Charts.Pie.PieChart();

        public frmMain()
        {
            InitializeComponent();
            cir = new RainstormStudios.Drawing.CircleF((float)(pictureBox1.Bounds.Right - pictureBox1.Bounds.Left) / 2, (float)(pictureBox1.Bounds.Bottom - pictureBox1.Bounds.Top) / 2, 80.0f);

            RainstormStudios.Drawing.Charts.Pie.PieChartSlice slice1 = new RainstormStudios.Drawing.Charts.Pie.PieChartSlice(20, System.Drawing.Color.Red);
            RainstormStudios.Drawing.Charts.Pie.PieChartSlice slice2 = new RainstormStudios.Drawing.Charts.Pie.PieChartSlice(30, System.Drawing.Color.Green);
            RainstormStudios.Drawing.Charts.Pie.PieChartSlice slice3 = new RainstormStudios.Drawing.Charts.Pie.PieChartSlice(50, System.Drawing.Color.Blue);
            pc.AddElement(slice1);
            pc.AddElement(slice2);
            pc.AddElement(slice3);


            //RainstormStudios.IO.FolderACL acl = new RainstormStudios.IO.FolderACL("C:\\", Guid.NewGuid());
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (cir != RainstormStudios.Drawing.CircleF.Empty)
            {
                //using (System.Drawing.Drawing2D.GraphicsPath path = cir.GetShape(30, System.Drawing.Drawing2D.PathPointType.Line))
                //    e.Graphics.DrawPath(Pens.Black, path);
                //RainstormStudios.Drawing.CircleF.DrawShape(cir, e.Graphics, Pens.Black);
                RainstormStudios.Drawing.CircleF.FillShape(cir, e.Graphics, Brushes.CadetBlue);
            }

            try
            {
                using (Graphics g = this.panel1.CreateGraphics())
                    pc.Draw(g, this.panel1.ClientRectangle);
            }
            catch(Exception ex)
            { }
        }

        //private void button1_DragDrop(object sender, DragEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine(string.Format("pictureBox1_Drop: x={0},y={1}", cir.Center.X, cir.Center.Y));
        //    pictureBox1.Location = new Point(e.X, e.Y);
        //    cir.Center = new PointF((float)(pictureBox1.Bounds.Right - pictureBox1.Bounds.Left) / 2, (float)(pictureBox1.Bounds.Bottom - pictureBox1.Bounds.Top) / 2);
        //}

        //private void button1_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        //{
        //    if (e.EscapePressed)
        //        e.Action = DragAction.Cancel;
        //    else if ((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left)
        //        e.Action = DragAction.Drop;
        //    else
        //        e.Action = DragAction.Continue;

        //    System.Diagnostics.Debug.WriteLine(string.Format("pictureBox1_QueryDrag: a={0},esc={1},ks={2}", e.Action, e.EscapePressed, e.KeyState));
        //}

        //private void button1_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Move;
        //    cir.Center = new PointF((float)e.X, (float)e.Y);
        //}

        //private void button1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Drag-Drop operation started.");
        //        pictureBox1.DoDragDrop(pictureBox1.Image, DragDropEffects.Move);
        //    }
        //}

        //private void frmMain_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Move;
        //    System.Diagnostics.Debug.WriteLine(string.Format("frmMain_DragOver: x={0},y={1},fx={2},data={3}", e.X, e.Y, e.AllowedEffect, string.Join(",", e.Data.GetFormats())));
        //    Point pc = this.PointToClient(new Point(e.X, e.Y));
        //    cir.Center = new PointF((float)pc.X, (float)pc.Y);
        //    this.Invalidate(new Rectangle((int)(cir.Center.X - cir.Radius) - 1, (int)(cir.Center.Y - cir.Radius) - 1, (int)(cir.Radius * 2) + 2, (int)(cir.Radius * 2) + 2));
        //    //this.Invalidate();
        //}

        //private void pictureBox1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine(string.Format("pictureBox1_GiveFeedback: fx={0}", e.Effect));
        //}

        //private void frmMain_DragDrop(object sender, DragEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine(string.Format("frmMain_Drop: x={0},y={1},fx={2},ks={3}", e.X, e.Y, e.Effect, e.KeyState));
        //}
    }
}