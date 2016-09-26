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

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    public enum FormElementDisplayType : uint
    {
        Textbox = 1,
        DropdownList = 2,
        RadioButtons = 3,
        CheckBoxes = 4,
        Image = 5,
        Url = 6,
        Message = 7,
        Grid = 8,
        GridCell = 9,
        HiddenField = 10
    }
    [Flags]
    public enum FormElementDisplayOptions
    {
        // Using the "bit-shift" method means I can just use incremental numbers and not
        //   have to figure out the next byte value in the sequence.  The bit-shifted
        //   value is determined at compile time, so introduces no runtime overhead.
        None = 0x00,
        DisplayHorizontal = 1 << 0,
        DisplayVertical = 1 << 1,
        Collapsed = 1 << 2,
        Expanded = 1 << 3,
        Disabled = 1 << 4
    }
}
