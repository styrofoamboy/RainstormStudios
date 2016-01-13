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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace RainstormStudios.Web.UI.WebControls
{
    class ServerControlHelper
    {
        public static void AddInlineStyle(HtmlTextWriter writer, string inlineStyle)
        { AddInlineStyle(writer, inlineStyle, true); }
        public static void AddInlineStyle(HtmlTextWriter writer, string inlineStyle, bool suppressExceptions)
        {
            // If the inline style string is NULL, there's nothing to do here.
            if (string.IsNullOrEmpty(inlineStyle))
                return;

            try
            {
                string[] elements = inlineStyle.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < elements.Length; i++)
                {
                    string[] pcs = elements[i].Split(':');
                    if (pcs.Length != 2)
                        if (!suppressExceptions)
                            continue;
                        else
                            throw new Exception("Malformed style attribute.");

                    writer.AddStyleAttribute(pcs[0], pcs[1]);
                }
            }
            catch (Exception ex)
            {
                if (!suppressExceptions)
                    throw new Exception("Unable to apply inline style: " + ex.Message, ex);
            }
        }
    }
}
