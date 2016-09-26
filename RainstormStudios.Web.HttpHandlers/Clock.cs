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
    public class Clock : IHttpHandler
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
            Exception exKeep = null;
            string
                strW = context.Request.Params["w"],
                strH = context.Request.Params["h"],
                strFgClr = context.Request.Params["fg"],
                strBgClr = context.Request.Params["bg"],
                strFont = context.Request.Params["fnt"],
                strSize = context.Request.Params["sz"],
                strStyle = context.Request.Params["st"],
                strFormat = context.Request.Params["fmt"],
                strAlign = context.Request.Params["aln"];

            int iW = 0,
                iH = 0;
            Color
                fgClr = Color.Empty,
                bgClr = Color.Empty,
                tdClr = Color.Empty;
            float
                fSZ = 0.9f;
            int iFgClr = 0,
                iBgClr = 0,
                iTdClr = 0;
            FontStyle
                style = FontStyle.Regular;
            int iFntStyle = 0;
            System.Web.UI.WebControls.HorizontalAlign
                align = System.Web.UI.WebControls.HorizontalAlign.NotSet;

            try
            {
                if (!int.TryParse(strW, out iW))
                    context.AddError(new ArgumentException("Specified 'Width' is not a valid integer value."));
                if (!int.TryParse(strH, out iH))
                    context.AddError(new ArgumentException("Specified 'Height' is not a valid integer value."));

                if (string.IsNullOrEmpty(strFormat))
                    strFormat = "hh:MM:ss tt";

                if (!string.IsNullOrEmpty(strFgClr))
                    if (int.TryParse(strFgClr, out iFgClr))
                        fgClr = Color.FromArgb(iFgClr);
                    else
                        fgClr = Color.FromName(strFgClr);
                else
                    fgClr = Color.Black;
                if (fgClr == null || fgClr == Color.Empty)
                    context.AddError(new ArgumentException("Specified foreground color could not be parsed."));

                if (!string.IsNullOrEmpty(strBgClr))
                    if (int.TryParse(strBgClr, out iBgClr))
                        bgClr = Color.FromArgb(iBgClr);
                    else
                        bgClr = Color.FromName(strBgClr);
                else
                    bgClr = Color.White;
                if (bgClr == null || bgClr == Color.Empty)
                    context.AddError(new ArgumentException("Specified background color could not be parsed."));

                if (!float.TryParse(strSize, out fSZ))
                    context.AddError(new ArgumentException("Specified font size is not a valid floating point integer value."));

                // Try and parse the font style value into an integer as a check to
                //   see what kind of value was passed.
                if (int.TryParse(strStyle, out iFntStyle))
                    // If the value is an integer, it's probably a bit array of
                    //   enumeration flags.
                    style = (FontStyle)iFntStyle;
                else
                    // If the value is not an integer, try and parse the enumeration
                    //   value directly.
                    style = (string.IsNullOrEmpty(strStyle))
                        ? FontStyle.Regular
                        : (FontStyle)Enum.Parse(typeof(FontStyle), strStyle, true);

                align = (string.IsNullOrEmpty(strAlign))
                        ? System.Web.UI.WebControls.HorizontalAlign.NotSet
                        : (System.Web.UI.WebControls.HorizontalAlign)Enum.Parse(typeof(System.Web.UI.WebControls.HorizontalAlign), strAlign);
            }
            catch (Exception ex)
            {
                exKeep = ex;
            }

            using (Bitmap bmp = new Bitmap(iW, iH))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (exKeep == null)
                    {
                        g.Clear(bgClr);
                        using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap))
                        {
                            if (align == System.Web.UI.WebControls.HorizontalAlign.Center)
                                format.Alignment = StringAlignment.Center;
                            else if (align == System.Web.UI.WebControls.HorizontalAlign.Right)
                                format.Alignment = StringAlignment.Far;
                            else
                                format.Alignment = StringAlignment.Near;
                            format.LineAlignment = StringAlignment.Center;
                            format.Trimming = StringTrimming.EllipsisCharacter;

                            RectangleF rect = new RectangleF(new PointF(0, 0), new SizeF((float)iW, (float)iH));

                            using (Font font = new Font(strFont, fSZ, style))
                            using (Brush brush = new SolidBrush(fgClr))
                                g.DrawString(DateTime.Now.ToString(strFormat), font, brush, rect, format);
                        }
                    }
                    else
                    {
                        GraphicsUnit gu = g.PageUnit;
                        g.Clear(Color.White);
                        using (Font font = new Font("Tahoma", 6.0f, FontStyle.Regular))
                            g.DrawString(exKeep.Message, font, Brushes.Black, bmp.GetBounds(ref gu));
                    }
                }
                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    bmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] strmData = fs.ToArray();
                    context.Response.OutputStream.Write(strmData, 0, strmData.Length);
                }
            }
        }
        #endregion
    }
}
