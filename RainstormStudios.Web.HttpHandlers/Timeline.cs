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
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Michael Unfried")]
    public class TimelineHandler : IHttpHandler
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
                strw = req.QueryString["w"],
                strh = req.QueryString["h"],
                strpcnt = req.QueryString["p"],
                strmindate = req.QueryString["mindt"],
                strmaxdate = req.QueryString["maxdt"],
                strstdate = req.QueryString["stdt"],
                streddate = req.QueryString["eddt"],
                strfgclr = req.QueryString["fgclr"],
                strinclr = req.QueryString["inclr"],
                strbgclr = req.QueryString["bgclr"],
                strTdyMark = context.Request.Params["tdy"];

            int w, h, p;
            DateTime
                minDate, maxDate,
                startDate, endDate;
            Color
                fgClr, bgClr, inClr, tdClr;

            if (!int.TryParse(strw, out w))
                throw new ArgumentException("Width is not a valid integer.");
            if (!int.TryParse(strh, out h))
                throw new ArgumentException("Height is not a valid integer.");
            if (!int.TryParse(strpcnt, out p))
                throw new ArgumentException("Percent complete is not a valid integer.");
            if (!DateTime.TryParse(strmindate, out minDate))
                throw new ArgumentException("Minimum date is not a valid datetime value.");
            if (!DateTime.TryParse(strmaxdate, out maxDate))
                throw new ArgumentException("Maximum date is not a valid datetime value.");
            if (!DateTime.TryParse(strstdate, out startDate))
                throw new ArgumentException("Start date is not a valid datetime value.");
            if (!DateTime.TryParse(streddate, out endDate))
                throw new ArgumentException("End date is not a valid datetime value.");

            int fgClrArgb, bgClrArgb, inClrArgb, tdClrArgb;
            if (string.IsNullOrEmpty(strfgclr))
                fgClr = System.Drawing.Color.DarkBlue;
            else
                if (int.TryParse(strfgclr, out fgClrArgb))
                    fgClr = Color.FromArgb(fgClrArgb);
                else
                    fgClr = Color.FromName(strfgclr);
            if (fgClr == Color.Empty)
                throw new ArgumentException("Foreground color did not evaluate to a valid color value.");

            if (string.IsNullOrEmpty(strbgclr))
                bgClr = System.Drawing.Color.White;
            else
                if (int.TryParse(strbgclr, out bgClrArgb))
                    bgClr = Color.FromArgb(bgClrArgb);
                else
                    bgClr = Color.FromName(strbgclr);
            if (bgClr == Color.Empty)
                throw new ArgumentException("Background color did not evaluate to a valid color value.");

            if (string.IsNullOrEmpty(strinclr))
                inClr = System.Drawing.Color.DarkGray;
            else
                if (int.TryParse(strinclr, out inClrArgb))
                    inClr = Color.FromArgb(inClrArgb);
                else
                    inClr = Color.FromName(strinclr);
            if (inClr == Color.Empty)
                throw new ArgumentException("Incomplete color did not evaluate to a valid color value.");

            if (!string.IsNullOrEmpty(strTdyMark))
                if (int.TryParse(strTdyMark, out tdClrArgb))
                    tdClr = Color.FromArgb(tdClrArgb);
                else
                    tdClr = Color.FromName(strTdyMark);
            else
                tdClr = System.Drawing.Color.OrangeRed;
            if (tdClr == null || bgClr == Color.Empty)
                throw new ArgumentException("Specified today mark color could not be parsed.");


            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                TimeSpan tsTotal = maxDate.Subtract(minDate);
                TimeSpan tsStart = startDate.Subtract(minDate);
                TimeSpan tsEnd = endDate.Subtract(minDate);
                int l = (tsStart.Days * w) / tsTotal.Days;
                int r = (tsEnd.Days * w) / tsTotal.Days;

                g.Clear(bgClr);
                using (Brush brush = new SolidBrush(inClr))
                    g.FillRectangle(brush, new Rectangle(l, 0, System.Math.Max(r - l, 1), h));

                using (Brush brush = new SolidBrush(fgClr))
                    g.FillRectangle(brush, new Rectangle(l, 0, (p * System.Math.Max(r - l, 1)) / 100, h));

                // Calculate the mark for "Today"
                if (DateTime.Today > minDate && DateTime.Today < maxDate)
                {
                    TimeSpan tsToday = DateTime.Today.Subtract(minDate);
                    int xToday = (int)System.Math.Round((tsToday.TotalDays * w) / tsTotal.TotalDays);
                    // Make sure the mark is within the bitmap's bounds.
                    xToday = System.Math.Min(System.Math.Max(xToday, 0), bmp.Width);
                    using (Pen pen = new Pen(tdClr))
                        g.DrawLine(pen, new Point(xToday, 0), new Point(xToday, bmp.Height));
                }

                //using (Font f = new Font("Tahoma", (h / 4) * 3, FontStyle.Regular, GraphicsUnit.Pixel))
                //    g.DrawString(string.Format("({0} * ({1} - {2})) / 100", p, r, l), f, Brushes.DarkGoldenrod, new PointF(5.0f, h / 8));

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "image/png";
                    byte[] imgBts = ms.ToArray();
                    context.Response.OutputStream.Write(imgBts, 0, imgBts.Length);
                }
            }
        }
        #endregion
    }
}
