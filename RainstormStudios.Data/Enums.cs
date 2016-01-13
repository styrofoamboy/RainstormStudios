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
using System.Text;

namespace RainstormStudios.Data
{
    /// <summary>
    /// An emuneration which defines which type of flat file should be used.
    /// </summary>
    [Author("Unfried, Michael")]
    public enum FlatFileType
    {
        XmlFile = 0,
        TextFile,
        ExcelFile,
        CSVTextFile,
        FixedWidthFile,
    }
    [Author("Unfried, Michael")]
    public enum FlatFileFormatFlags
    {
        ColumnHeaders = 0x01,
        TrimValues = 0x02,
        XmlWriteSchema = 0x04,
        CsvExcelFormat = 0x08,
        CsvEscapeChars = 0x16,
        XlsColumnNames = 0x32,
        XlsImex = 0x64
    }
    [Author("Unfried, Michael")]
    public enum TextAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }
}
