//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Unfried, Michael")]
    public class AnalogClock : System.Web.IHttpHandler
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsReusable
        { get { return false; } }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;

            string
                sW = req.QueryString["w"],
                sH = req.QueryString["h"],
                sBgImg = req.QueryString["bgi"],
                sBgClr = req.QueryString["bgc"],
                sFgClr = req.QueryString["fgc"],
                sShowSecHnd = req.QueryString["sh"],
                sDt = req.QueryString["dt"];

            int iW = 0,
                iH = 0;
            Color
                fgClr = Color.Black,
                bgClr = Color.Empty;

            bool
                bShowSecondHand = false;

            DateTime
                dtNow = DateTime.Now;

            if (sW == null)
                iW = 50;
            else if (!int.TryParse(sW, out iW))
                throw new Exception("Specified width is not a valid integer.");
            if (sH == null)
                iH = 50;
            else if (!int.TryParse(sH, out iH))
                throw new Exception("Specified height is not a valid integer.");

            try
            {
                if (!string.IsNullOrEmpty(sFgClr))
                    fgClr = Hex.GetSystemColor(sFgClr);
            }
            catch (Exception ex)
            { throw new Exception("Unable to parse foreground color: " + ex.Message, ex); }
            try
            {
                if (!string.IsNullOrEmpty(sBgClr))
                    bgClr = Hex.GetSystemColor(sBgClr);
            }
            catch (Exception ex)
            { throw new Exception("Unable to parse background color: " + ex.Message, ex); }

            if (sShowSecHnd != null && !Boolean.TryParse(sShowSecHnd, out bShowSecondHand))
                throw new Exception("Unable to parse show second hand perameter to boolean.");

            if (sDt != null && !DateTime.TryParse(sDt, out dtNow))
                throw new Exception("Unable to parse date/time.");

            int iWc = iW / 2,
                iHc = iH / 2,
                edgeSpc = 2,
                prec = 60;
            Point pntCenter = new Point(iWc, iHc);
            using (Bitmap bmp = new Bitmap(iW, iH, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics gBmp = Graphics.FromImage(bmp))
                {
                    gBmp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    gBmp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gBmp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    gBmp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    if (bgClr != Color.Empty)
                        gBmp.Clear(bgClr);

                    int faceW = (iW - (edgeSpc * 2)),
                        faceH = (iH - (edgeSpc * 2));
                    int numW = ((faceW * 92) / 100) / 2,
                        numH = ((faceH * 92) / 100) / 2;
                    int hourHandW = ((faceW * 60) / 100) / 2,
                        hourHandH = ((faceH * 60) / 100) / 2,
                        minHandW = ((faceW * 85) / 100) / 2,
                        minHandH = ((faceH * 85) / 100) / 2;
                    int dotSzW = (int)((faceW * 1.2) / 100),
                        dotSzH = (int)((faceH * 1.2) / 100);
                    int fntSz = ((iW * 24) / 500);

                    using (Pen fgPen = new Pen(fgClr))
                    {
                        // Draw the circle
                        gBmp.DrawEllipse(fgPen, new Rectangle(edgeSpc, edgeSpc, faceW, faceH));

                        // Daw the dots.
                        int skipDot = 4;
                        for (int t = 0; t < prec; t++)
                        {
                            if (++skipDot == 5)
                            {
                                skipDot = 0;
                                continue;
                            }
                            double a = System.Math.PI * t / (prec / 2);
                            double x = pntCenter.X + numW * System.Math.Sin(a);
                            double y = pntCenter.Y + numH * System.Math.Cos(a);
                            using (SolidBrush dotsBrush = new SolidBrush(fgClr))
                                gBmp.FillEllipse(dotsBrush, new Rectangle((int)(x - (dotSzW / 2)), (int)(y - (dotSzH / 2)), dotSzW, dotSzH));
                            x = y = a = double.MinValue;
                        }
                        skipDot = int.MinValue;

                        // Draw the numbers.
                        for (int t = 0; t > -12; t--)
                        {
                            double a = System.Math.PI * (t + 5) / 6;
                            double x = pntCenter.X + numW * System.Math.Sin(a);
                            double y = pntCenter.Y + numH * System.Math.Cos(a);

                            using (Font font = new Font("Arial", fntSz, FontStyle.Regular, GraphicsUnit.Pixel))
                            using (StringFormat format = new StringFormat())
                            {
                                format.LineAlignment = StringAlignment.Center;
                                format.Alignment = StringAlignment.Center;

                                string numVal = Convert.ToString(System.Math.Abs(t) + 1);
                                SizeF numSz = gBmp.MeasureString(numVal, font);
                                RectangleF numRect = new RectangleF(new PointF((float)x - (numSz.Width / 2), (float)y - (numSz.Height / 2)), numSz);
                                //using (SolidBrush clearBrush = new SolidBrush(Color.White))
                                //    gBmp.FillRectangle(clearBrush, numRect);
                                using (SolidBrush numBrush = new SolidBrush(fgClr))
                                    gBmp.DrawString(numVal, font, numBrush, numRect);
                                numSz = SizeF.Empty;
                                numRect = RectangleF.Empty;
                            }
                            x = y = x = double.MinValue;
                        }

                        // Draw the hands of the clock.
                        double tHour = System.Math.PI * ((dtNow.Hour * -1) + 6) / 6;
                        double tMin = System.Math.PI * ((dtNow.Minute * -1) + 30) / 30;
                        double tSec = System.Math.PI * ((dtNow.Second * -1) + 30) / 30;

                        {
                            // Draw hour hand.
                            double x = pntCenter.X + hourHandW * System.Math.Sin(tHour);
                            double y = pntCenter.Y + hourHandH * System.Math.Cos(tHour);
                            gBmp.DrawLine(fgPen, pntCenter, new Point((int)x, (int)y));
                            x = y = double.MinValue;
                        }
                        {
                            // Draw minute hand.
                            double x = pntCenter.X + minHandW * System.Math.Sin(tMin);
                            double y = pntCenter.Y + minHandH * System.Math.Cos(tMin);
                            gBmp.DrawLine(fgPen, pntCenter, new Point((int)x, (int)y));
                            x = y = double.MinValue;
                        }
                        if (bShowSecondHand)
                        {
                            // Draw second hand.
                            double x = pntCenter.X + minHandW * System.Math.Sin(tSec);
                            double y = pntCenter.Y + minHandH * System.Math.Cos(tSec);
                            using (Pen secHandPen = new Pen(Color.Red))
                                gBmp.DrawLine(secHandPen, pntCenter, new Point((int)x, (int)y));
                            x = y = double.MinValue;
                        }

                        {
                            string curTime = dtNow.ToString("hh:mm:ss");
                            using (Font font = new Font("Arial", fntSz, FontStyle.Regular, GraphicsUnit.Pixel))
                            using (StringFormat format = new StringFormat())
                            {
                                format.Alignment = StringAlignment.Center;
                                format.LineAlignment = StringAlignment.Center;

                                SizeF timeSz = gBmp.MeasureString(curTime, font);
                                RectangleF rectTime = new RectangleF(new PointF(pntCenter.X - (timeSz.Width / 2), pntCenter.Y - (timeSz.Height * 2)), timeSz);
                                using (SolidBrush brush = new SolidBrush(fgClr))
                                    gBmp.DrawString(curTime, font, brush, rectTime, format);
                                timeSz = SizeF.Empty;
                                rectTime = RectangleF.Empty;
                            }
                        }
                        tSec = tMin = tHour = double.MinValue;
                    }
                }

                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    bmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] strmData = fs.ToArray();
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "image/png";
                    context.Response.OutputStream.Write(strmData, 0, strmData.Length);
                    strmData = null;
                }
            }
        }
        #endregion
    }
}
