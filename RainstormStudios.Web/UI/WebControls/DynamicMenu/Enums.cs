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

namespace RainstormStudios.Web.UI.WebControls.DynamicMenu
{
    public enum DynamicMenuRenderType : uint
    {
        HoverVertical = 0,
        HoverHorizontal = 1,
        Popout = 2,
        Icon = 3
    }
    public enum DynamicMenuItemControlType : uint
    {
        Command,
        DynamicMenuIconGroup,
        DynamicMenuIcon,
        DynamicMenuPopoutGroup,
        DynamicMenuPopoutItem,
        HorizontalHoverMenu,
        HorizontalHoverMenuItem,
        VerticalHoverMenu,
        VerticalHoverMenuItem
    }
}
