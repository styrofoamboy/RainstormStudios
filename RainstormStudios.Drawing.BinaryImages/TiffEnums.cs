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

namespace RainstormStudios.Drawing.BinaryImages
{
    [Author("Unfried, Michael")]
    public enum TiffTagType
    {
        NewSubFileType = 0xfe,
        SubFileType = 0xff,
        ImageWidth = 0x100,
        ImageHeight = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotoMetricInterpretation = 0x106,
        Thresholding = 0x107,
        CellWidth = 0x108,
        CellLength = 0x109,
        FillOrder = 0x10a,
        DocumentName = 0x10d,
        ImageDescription = 0x10e,
        Make = 0x10f,
        Model = 0x110,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCounts = 0x117,
        MinSampleValue = 0x118,
        MaxSampleValue = 0x119,
        YResolution = 0x11a,
        XResolution = 0x11b,
        PlanarConfiguration = 0x11c,
        PageName = 0x11d,
        XPosition = 0x11e,
        YPosition = 0x11f,
        FreeOffsets = 0x120,
        FreeByteCounts = 0x121,
        GreyResponseUnit = 0x122,
        GreyResponseCurves = 0x123,
        Group3Options = 0x125,
        Group4Options = 0x126,
        ResolutionUnit = 0x128,
        PageNumber = 0x129,
        ColorResponseCurves = 0x12d,
        Software = 0x131,
        DateTime = 0x132,
        Artist = 0x13b,
        HostComputer = 0x13c,
        Predictor = 0x13d,
        WhitePoint = 0x13e,
        PrimaryChromaticities = 0x13f,
        ColorMap = 0x141,
        Custom = 0x8000
    }
    [Author("Unfried, Michael")]
    public enum TiffFieldType
    {
        ByteType = 0x01,
        AsciiType = 0x02,
        ShortType = 0x03,
        LongType = 0x04,
        Rational = 0x05
    }
    [Author("Unfried, Michael")]
    public enum TiffCompressMode
    {
        None = 0x01,
        Huffman = 0x02,
        CCITTF3 = 0x03,
        CCITTF4 = 0x04,
        LZW = 0x05,
        PackBits = 0x8005
    }
    //public enum TiffSubFileType
    //{
    //}
    [Author("Unfried, Michael")]
    public enum TiffResolutionUnit
    {
        None = 0x01,
        Inch = 0x02,
        Centimeter = 0x03
    }
    [Author("Unfried, Michael")]
    public enum TiffPhotometricInterpretation
    {
        GreyWhiteNull = 0x00,
        GreyBlackNull = 0x01,
        RGB = 0x02,
        Pallette = 0x03,
        Mask = 0x04
    }
    [Author("Unfried, Michael")]
    public enum TiffPlanarConfiguration
    {
        Sequential = 0x01,
        Planar = 0x02
    }
    [Author("Unfried, Michael")]
    public enum TiffGreyResponseUnit
    {
        Tenths = 0x01,
        Hundreths = 0x02,
        Thousandths = 0x03,
        TenThousandths = 0x04,
        HundredThousandths = 0x05
    }
    [Author("Unfried, Michael")]
    enum TiffTagIDFieldType
    {
        NewSubFileType = 0x04,
        SubFileType = 0x03,
        ImageWidth = 0x04,
        ImageHeight = 0x04,
        BitsPerSample = 0x03,
        Compression = 0x03,
        PhotoMetricInterpretation = 0x03,
        Thresholding = 0x03,
        CellWidth = 0x03,
        CellLength = 0x03,
        FillOrder = 0x03,
        DocumentName = 0x02,
        ImageDescription = 0x02,
        Make = 0x02,
        Model = 0x02,
        StripOffsets = 0x04,
        Orientation = 0x03,
        SamplesPerPixel = 0x03,
        RowsPerStrip = 0x04,
        StripByteCounts = 0x04,
        MinSampleValue = 0x03,
        MaxSampleValue = 0x03,
        YResolution = 0x05,
        XResolution = 0x05,
        PlanarConfiguration = 0x03,
        PageName = 0x02,
        XPosition = 0x05,
        YPosition = 0x05,
        FreeOffsets = 0x04,
        FreeByteCounts = 0x04,
        GreyResponseUnit = 0x03,
        GreyResponseCurves = 0x03,
        Group3Options = 0x04,
        Group4Options = 0x04,
        ResolutionUnit = 0x03,
        PageNumber = 0x03,
        ColorResponseCurves = 0x03,
        Software = 0x02,
        DateTime = 0x02,
        Artist = 0x02,
        HostComputer = 0x02,
        Predictor = 0x03,
        WhitePoint = 0x05,
        PrimaryChromaticities = 0x05,
        ColorMap = 0x03
    }
    [Author("Unfried, Michael")]
    enum TiffTagIDDefValues
    {
        NewSubFileType = 0,
        SubFileType = 0,
        BitsPerSample = 1,
        Compression = 1,
        PhotoMetricInterpretation = 0,
        Thresholding = 1,
        FillOrder = 1,
        Orientation = 1,
        SamplesPerPixel = 1,
        PlanarConfiguration = 1,
        GreyResponseUnit = 2,
        Group3Options = 0,
        Group4Options = 0,
        ResolutionUnit = 2,
        Predictor = 1
    }
}
