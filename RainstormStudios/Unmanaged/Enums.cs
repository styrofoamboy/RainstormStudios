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

namespace RainstormStudios.Unmanaged
{
    public enum BinaryRasterOperations
    {
        R2_BLACK = 1,           /*  0       */
        R2_NOTMERGEPEN = 2,     /* DPon     */
        R2_MASKNOTPEN = 3,      /* DPna     */
        R2_NOTCOPYPEN = 4,      /* PN       */
        R2_MASKPENNOT = 5,      /* PDna     */
        R2_NOT = 6,             /* Dn       */
        R2_XORPEN = 7,          /* DPx      */
        R2_NOTMASKPEN = 8,      /* DPan     */
        R2_MASKPEN = 9,         /* DPa      */
        R2_NOTXORPEN = 10,      /* DPxn     */
        R2_NOP = 11,            /* D        */
        R2_MERGENOTPEN = 12,    /* DPno     */
        R2_COPYPEN = 13,        /* P        */
        R2_MERGEPENNOT = 14,    /* PDno     */
        R2_MERGEPEN = 15,       /* DPo      */
        R2_WHITE = 16,          /*  1       */
        R2_LAST = 16
    }
    public enum TernaryRasterOperations
    {
        SRCCOPY = 0x00CC0020,       /* dest = source                   */
        SRCPAINT = 0x00EE0086,      /* dest = source OR dest           */
        SRCAND = 0x008800C6,        /* dest = source AND dest          */
        SRCINVERT = 0x00660046,     /* dest = source XOR dest          */
        SRCERASE = 0x00440328,      /* dest = source AND (NOT dest )   */
        NOTSRCCOPY = 0x00330008,    /* dest = (NOT source)             */
        NOTSRCERASE = 0x001100A6,   /* dest = (NOT src) AND (NOT dest) */
        MERGECOPY = 0x00C000CA,     /* dest = (source AND pattern)     */
        MERGEPAINT = 0x00BB0226,    /* dest = (NOT source) OR dest     */
        PATCOPY = 0x00F00021,       /* dest = pattern                  */
        PATPAINT = 0x00FB0A09,      /* dest = DPSnoo                   */
        PATINVERT = 0x005A0049,     /* dest = pattern XOR dest         */
        DSTINVERT = 0x00550009,     /* dest = (NOT dest)               */
        BLACKNESS = 0x00000042,     /* dest = BLACK                    */
        WHITENESS = 0x00FF0062      /* dest = WHITE                    */
    }
    public enum PeekMessageFlags
    {
        PM_NOREMOVE = 0,
        PM_REMOVE = 1,
        PM_NOYIELD = 2
    }
    public enum Win32Messages : long
    {
        CB_GETEDITSEL = 0x00000140,
        CB_LIMITTEXT = 0x00000141,
        CB_SETEDITSEL = 0x00000142,
        CB_ADDSTRING = 0x00000143,
        CB_DELETESTRING = 0x00000144,
        CB_DIR = 0x00000145,
        CB_GETCOUNT = 0x00000146,
        CB_GETCURSEL = 0x00000147,
        CB_GETLBTEXT = 0x00000148,
        CB_GETLBTEXTLEN = 0x00000149,
        CB_INSERTSTRING = 0x0000014A,
        CB_RESETCONTENT = 0x0000014B,
        CB_FINDSTRING = 0x0000014C,
        CB_SELECTSTRING = 0x0000014D,
        CB_SETCURSEL = 0x0000014E,
        CB_SHOWDROPDOWN = 0x0000014F,
        CB_GETITEMDATA = 0x00000150,
        CB_SETITEMDATA = 0x00000151,
        CB_GETDROPPEDCONTROLRECT = 0x00000152,
        CB_SETITEMHEIGHT = 0x00000153,
        CB_GETITEMHEIGHT = 0x00000154,
        CB_SETEXTENDEDUI = 0x00000155,
        CB_GETEXTENDEDUI = 0x00000156,
        CB_GETDROPPEDSTATE = 0x00000157,
        CB_FINDSTRINGEXACT = 0x00000158,
        CB_SETLOCALE = 0x00000159,
        CB_GETLOCALE = 0x0000015A,
        CB_GETTOPINDEX = 0x0000015B,
        CB_SETTOPINDEX = 0x0000015C,
        CB_GETHORIZONTALEXTENT = 0x0000015D,
        CB_SETHORIZONTALEXTENT = 0x0000015E,
        CB_GETDROPPEDWIDTH = 0x0000015F,
        CB_SETDROPPEDWIDTH = 0x00000160,
        CB_INITSTORAGE = 0x00000161,
        CB_MSGMAX = 0x00000162,
        EM_CANUNDO = 0x000000C6,
        EM_EMPTYUNDOBUFFER = 0x000000CD,
        EM_FMTLINES = 0x000000C8,
        EM_FORMATRANGE = (WM_USER + 57),
        EM_GETEVENTMASK = (WM_USER + 59),
        EM_GETFIRSTVISIBLELINE = 0x000000CE,
        EM_GETHANDLE = 0x000000BD,
        EM_GETLINE = 0x000000C4,
        EM_GETLINECOUNT = 0x000000BA,
        EM_GETMODIFY = 0x000000B8,
        EM_GETPASSWORDCHAR = 0x000000D2,
        EM_GETRECT = 0x000000B2,
        EM_GETSEL = 0x000000B0,
        EM_GETTHUMB = 0x000000BE,
        EM_GETWORDBREAKPROC = 0x000000D1,
        EM_LIMITTEXT = 0x000000C5,
        EM_LINEFROMCHAR = 0x000000C9,
        EM_LINEINDEX = 0x000000BB,
        EM_LINELENGTH = 0x000000C1,
        EM_LINESCROLL = 0x000000B6,
        EM_REPLACESEL = 0x000000C2,
        EM_SCROLL = 0x000000B5,
        EM_SCROLLCARET = 0x000000B7,
        EM_SETEVENTMASK = (WM_USER + 69),
        EM_SETHANDLE = 0x000000BC,
        EM_SETMODIFY = 0x000000B9,
        EM_SETPASSWORDCHAR = 0x000000CC,
        EM_SETREADONLY = 0x000000CF,
        EM_SETRECT = 0x000000B3,
        EM_SETRECTNP = 0x000000B4,
        EM_SETSEL = 0x000000B1,
        EM_SETTABSTOPS = 0x000000CB,
        EM_SETTARGETDEVICE = (WM_USER + 72),
        EM_SETWORDBREAKPROC = 0x000000D0,
        EM_UNDO = 0x000000C7,
        HDS_HOTTRACK = 0x00000004,
        HDI_BITMAP = 0x00000010,
        HDI_IMAGE = 0x00000020,
        HDI_ORDER = 0x00000080,
        HDI_FORMAT = 0x00000004,
        HDI_TEXT = 0x00000002,
        HDI_WIDTH = 0x00000001,
        HDI_HEIGHT = HDI_WIDTH,
        HDF_LEFT = 0,
        HDF_RIGHT = 1,
        HDF_IMAGE = 0x00000800,
        HDF_BITMAP_ON_RIGHT = 0x00001000,
        HDF_BITMAP = 0x00002000,
        HDF_STRING = 0x00004000,
        HDM_FIRST = 0x00001200,
        HDM_SETITEM = (HDM_FIRST + 4),
        LB_ADDFILE = 0x00000196,
        LB_ADDSTRING = 0x00000180,
        LB_CTLCODE = 0,
        LB_DELETESTRING = 0x00000182,
        LB_DIR = 0x0000018D,
        LB_ERR = (-1),
        LB_ERRSPACE = (-2),
        LB_FINDSTRING = 0x0000018F,
        LB_FINDSTRINGEXACT = 0x000001A2,
        LB_GETANCHORINDEX = 0x0000019D,
        LB_GETCARETINDEX = 0x0000019F,
        LB_GETCOUNT = 0x0000018B,
        LB_GETCURSEL = 0x00000188,
        LB_GETHORIZONTALEXTENT = 0x00000193,
        LB_GETITEMDATA = 0x00000199,
        LB_GETITEMHEIGHT = 0x000001A1,
        LB_GETITEMRECT = 0x00000198,
        LB_GETLOCALE = 0x000001A6,
        LB_GETSEL = 0x00000187,
        LB_GETSELCOUNT = 0x00000190,
        LB_GETSELITEMS = 0x00000191,
        LB_GETTEXT = 0x00000189,
        LB_GETTEXTLEN = 0x0000018A,
        LB_GETTOPINDEX = 0x0000018E,
        LB_INSERTSTRING = 0x00000181,
        LB_MSGMAX = 0x000001A8,
        LB_OKAY = 0,
        LB_RESETCONTENT = 0x00000184,
        LB_SELECTSTRING = 0x0000018C,
        LB_SELITEMRANGE = 0x0000019B,
        LB_SELITEMRANGEEX = 0x00000183,
        LB_SETANCHORINDEX = 0x0000019C,
        LB_SETCARETINDEX = 0x0000019E,
        LB_SETCOLUMNWIDTH = 0x00000195,
        LB_SETCOUNT = 0x000001A7,
        LB_SETCURSEL = 0x00000186,
        LB_SETHORIZONTALEXTENT = 0x00000194,
        LB_SETITEMDATA = 0x0000019A,
        LB_SETITEMHEIGHT = 0x000001A0,
        LB_SETLOCALE = 0x000001A5,
        LB_SETSEL = 0x00000185,
        LB_SETTABSTOPS = 0x00000192,
        LB_SETTOPINDEX = 0x00000197,
        LBN_DBLCLK = 2,
        LBN_ERRSPACE = (-2),
        LBN_KILLFOCUS = 5,
        LBN_SELCANCEL = 3,
        LBN_SELCHANGE = 1,
        LBN_SETFOCUS = 4,
        LVM_FIRST = 0x00001000,
        LVM_GETHEADER = (LVM_FIRST + 31),
        LVM_GETBKCOLOR = (LVM_FIRST + 0),
        LVM_SETBKCOLOR = (LVM_FIRST + 1),
        LVM_GETIMAGELIST = (LVM_FIRST + 2),
        LVM_SETIMAGELIST = (LVM_FIRST + 3),
        LVM_GETITEMCOUNT = (LVM_FIRST + 4),
        LVM_GETITEMA = (LVM_FIRST + 5),
        LVM_GETITEM = LVM_GETITEMA,
        LVM_SETITEMA = (LVM_FIRST + 6),
        LVM_SETITEM = LVM_SETITEMA,
        LVM_INSERTITEMA = (LVM_FIRST + 7),
        LVM_INSERTITEM = LVM_INSERTITEMA,
        LVM_DELETEITEM = (LVM_FIRST + 8),
        LVM_DELETEALLITEMS = (LVM_FIRST + 9),
        LVM_GETCALLBACKMASK = (LVM_FIRST + 10),
        LVM_SETCALLBACKMASK = (LVM_FIRST + 11),
        LVM_GETNEXTITEM = (LVM_FIRST + 12),
        LVM_FINDITEMA = (LVM_FIRST + 13),
        LVM_FINDITEM = LVM_FINDITEMA,
        LVM_GETITEMRECT = (LVM_FIRST + 14),
        LVM_SETITEMPOSITION = (LVM_FIRST + 15),
        LVM_GETITEMPOSITION = (LVM_FIRST + 16),
        LVM_GETSTRINGWIDTHA = (LVM_FIRST + 17),
        LVM_GETSTRINGWIDTH = LVM_GETSTRINGWIDTHA,
        LVM_HITTEST = (LVM_FIRST + 18),
        LVM_ENSUREVISIBLE = (LVM_FIRST + 19),
        LVM_SCROLL = (LVM_FIRST + 20),
        LVM_REDRAWITEMS = (LVM_FIRST + 21),
        LVM_ARRANGE = (LVM_FIRST + 22),
        LVM_EDITLABELA = (LVM_FIRST + 23),
        LVM_EDITLABEL = LVM_EDITLABELA,
        LVM_GETEDITCONTROL = (LVM_FIRST + 24),
        LVM_GETCOLUMNA = (LVM_FIRST + 25),
        LVM_GETCOLUMN = LVM_GETCOLUMNA,
        LVM_SETCOLUMNA = (LVM_FIRST + 26),
        LVM_SETCOLUMN = LVM_SETCOLUMNA,
        LVM_INSERTCOLUMNA = (LVM_FIRST + 27),
        LVM_INSERTCOLUMN = LVM_INSERTCOLUMNA,
        LVM_DELETECOLUMN = (LVM_FIRST + 28),
        LVM_GETCOLUMNWIDTH = (LVM_FIRST + 29),
        LVM_SETCOLUMNWIDTH = (LVM_FIRST + 30),
        LVM_CREATEDRAGIMAGE = (LVM_FIRST + 33),
        LVM_GETVIEWRECT = (LVM_FIRST + 34),
        LVM_GETTEXTCOLOR = (LVM_FIRST + 35),
        LVM_SETTEXTCOLOR = (LVM_FIRST + 36),
        LVM_GETTEXTBKCOLOR = (LVM_FIRST + 37),
        LVM_SETTEXTBKCOLOR = (LVM_FIRST + 38),
        LVM_GETTOPINDEX = (LVM_FIRST + 39),
        LVM_GETCOUNTPERPAGE = (LVM_FIRST + 40),
        LVM_GETORIGIN = (LVM_FIRST + 41),
        LVM_UPDATE = (LVM_FIRST + 42),
        LVM_SETITEMSTATE = (LVM_FIRST + 43),
        LVM_GETITEMSTATE = (LVM_FIRST + 44),
        LVM_GETITEMTEXTA = (LVM_FIRST + 45),
        LVM_GETITEMTEXT = LVM_GETITEMTEXTA,
        LVM_SETITEMTEXTA = (LVM_FIRST + 46),
        LVM_SETITEMTEXT = LVM_SETITEMTEXTA,
        LVM_SETITEMCOUNT = (LVM_FIRST + 47),
        LVM_SORTITEMS = (LVM_FIRST + 48),
        LVM_SETITEMPOSITION32 = (LVM_FIRST + 49),
        LVM_GETSELECTEDCOUNT = (LVM_FIRST + 50),
        LVM_GETITEMSPACING = (LVM_FIRST + 51),
        LVM_GETISEARCHSTRINGA = (LVM_FIRST + 52),
        LVM_GETISEARCHSTRING = LVM_GETISEARCHSTRINGA,
        LVM_SETICONSPACING = (LVM_FIRST + 53),
        LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54),
        LVM_GETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 55),
        LVM_GETSUBITEMRECT = (LVM_FIRST + 56),
        LVM_SUBITEMHITTEST = (LVM_FIRST + 57),
        LVM_SETCOLUMNORDERARRAY = (LVM_FIRST + 58),
        LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59),
        LVM_SETHOTITEM = (LVM_FIRST + 60),
        LVM_GETHOTITEM = (LVM_FIRST + 61),
        LVM_SETHOTCURSOR = (LVM_FIRST + 62),
        LVM_GETHOTCURSOR = (LVM_FIRST + 63),
        LVM_APPROXIMATEVIEWRECT = (LVM_FIRST + 64),
        LVS_EX_FULLROWSELECT = 0x00000020,
        LVSCW_AUTOSIZE = (-1),
        LVSCW_AUTOSIZE_USEHEADER = (-2),
        WM_ACTIVATE = 0x00000006,
        WM_ACTIVATEAPP = 0x0000001C,
        WM_AFXFIRST = 0x00000360,
        WM_AFXLAST = 0x0000037F,
        WM_APP = 0x00008000,
        WM_APPCOMMAND = 0x00000319,
        WM_ASKCBFORMATNAME = 0x0000030C,
        WM_CANCELJOURNAL = 0x0000004B,
        WM_CANCELMODE = 0x0000001F,
        WM_CAPTURECHANGE = 0x00000215,
        WM_CHANGECBCHAIN = 0x0000030D,
        WM_CHANGEUISTATE = 0x00000127,
        WM_CHAR = 0x00000102,
        WM_CHARTOITEM = 0x0000002F,
        WM_CHILDACTIVATE = 0x00000022,
        WM_CHOOSEFONT_GETLOGFONT = (WM_USER + 1),
        WM_CHOOSEFONT_SETFLAGS = (WM_USER + 102),
        WM_CHOOSEFONT_SETLOGFONT = (WM_USER + 101),
        WM_CLEAR = 0x00000303,
        WM_CLOSE = 0x00000010,
        WM_COMMAND = 0x00000111,
        /// <summary>No longer supported.</summary>
        WM_COMMNOTIFY = 0x00000044,
        WM_COMPACTING = 0x00000041,
        WM_COMPAREITEM = 0x00000039,
        WM_CONTEXTMENU = 0x0000007B,
        WM_CONVERTREQUESTEX = 0x00000108,
        WM_COPY = 0x00000301,
        WM_COPYDATA = 0x0000004A,
        WM_CREATE = 0x00000001,
        WM_CTLCOLOER = 0x00000019,
        WM_CTLCOLORBTN = 0x00000135,
        WM_CTLCOLORDLG = 0x00000136,
        WM_CTLCOLOREDIT = 0x00000133,
        WM_CTLCOLORLISTBOX = 0x00000134,
        WM_CTLCOLORMSGBOX = 0x00000132,
        WM_CTLCOLORSCROLLBAR = 0x00000137,
        WM_CTLCOLORSTATIC = 0x00000138,
        WM_CUT = 0x00000300,
        WM_DDE_FIRST = 0x000003E0,
        WM_DDE_ACK = (WM_DDE_FIRST + 4),
        WM_DDE_ADVISE = (WM_DDE_FIRST + 2),
        WM_DDE_DATA = (WM_DDE_FIRST + 5),
        WM_DDE_EXECUTE = (WM_DDE_FIRST + 8),
        WM_DDE_INITIATE = (WM_DDE_FIRST),
        WM_DDE_LAST = (WM_DDE_FIRST + 8),
        WM_DDE_POKE = (WM_DDE_FIRST + 7),
        WM_DDE_REQUEST = (WM_DDE_FIRST + 6),
        WM_DDE_TERMINATE = (WM_DDE_FIRST + 1),
        WM_DDE_UNADVISE = (WM_DDE_FIRST + 3),
        WM_DEADCHAR = 0x00000103,
        WM_DELETEITEM = 0x0000002D,
        WM_DESTROY = 0x00000002,
        WM_DESTROYCLIPBOARD = 0x00000307,
        WM_DEVICECHANGE = 0x00000219,
        WM_DEVMODECHANGE = 0x0000001B,
        WM_DISPLAYCHANGE = 0x0000007E,
        WM_DRAWCLIPBOARD = 0x00000308,
        WM_DRAWITEM = 0x0000002B,
        WM_DROPFILES = 0x00000233,
        WM_ENABLE = 0x0000000A,
        WM_ENDSESSION = 0x00000016,
        WM_ENTERIDLE = 0x00000121,
        WM_ENTERMENULOOP = 0x00000211,
        WM_ENTERSIZEMOVE = 0x00000231,
        WM_ERASEBKGND = 0x00000014,
        WM_EXITMENULOOP = 0x00000212,
        WM_EXITSIZEMOVE = 0x00000232,
        WM_FONTCHANGE = 0x0000001D,
        WM_GETFONT = 0x00000031,
        WM_GETDLGCODE = 0x00000087,
        WM_GETHOTKEY = 0x00000033,
        WM_GETICON = 0x0000007F,
        WM_GETMINMAXINFO = 0x00000024,
        WM_GETOBJECT = 0x0000003D,
        WM_GETTEXT = 0x0000000D,
        WM_GETTEXTLENGTH = 0x0000000E,
        WM_HANDLEHELDFIRST = 0x00000358,
        WM_HANDLEHELDLAST = 0x0000035F,
        WM_HELP = 0x00000053,
        WM_HOTKEY = 0x00000312,
        WM_HSCROLL = 0x00000114,
        WM_HSCROLLCLIPBOARD = 0x0000030E,
        WM_ICONERASEBKGND = 0x00000027,
        WM_IME_CHAR = 0x00000286,
        WM_IME_COMPOSITION = 0x0000010F,
        WM_IME_COMPOSITIONFULL = 0x00000284,
        WM_IME_CONTROL = 0x00000283,
        WM_IME_ENDCOMPOSITION = 0x0000010E,
        WM_IME_KEYDOWN = 0x00000290,
        WM_IME_KEYLAST = 0x0000010F,
        WM_IME_KEYUP = 0x00000291,
        WM_IME_NOTIFY = 0x00000282,
        WM_IME_REQUEST = 0x00000288,
        WM_IME_SELECT = 0x00000285,
        WM_IME_SETCONTEXT = 0x00000281,
        WM_IME_STARTCOMPOSITION = 0x0000010D,
        WM_INITDIALOG = 0x00000110,
        WM_INITMENU = 0x00000116,
        WM_INITMENUPOPUP = 0x00000117,
        WM_INPUT = 0x000000FF,
        WM_INPUTLANGUAGECHANGE = 0x00000051,
        WM_INPUTLANGUAGECHANGEREQUEST = 0x00000050,
        WM_KEYDOWN = 0x00000100,
        WM_KEYFIRST = 0x00000100,
        WM_KEYLAST = 0x00000108,
        WM_KEYUP = 0x00000101,
        WM_KILLFOCUS = 0x00000008,
        WM_LBUTTONDBLCLK = 0x00000203,
        WM_LBUTTONDOWN = 0x00000201,
        WM_LBUTTONUP = 0x00000202,
        WM_MBUTTONDBLCLK = 0x00000209,
        WM_MBUTTONDOWN = 0x00000207,
        WM_MBUTTONUP = 0x00000208,
        WM_MDIACTIVATE = 0x00000222,
        WM_MDICASCADE = 0x00000227,
        WM_MDICREATE = 0x00000220,
        WM_MDIDESTROY = 0x00000221,
        WM_MDIGETACTIVE = 0x00000229,
        WM_MDIICONARRANGE = 0x00000228,
        WM_MDIMAXIMIZE = 0x00000225,
        WM_MDINEXT = 0x00000224,
        WM_MDIREFRESHMENU = 0x00000234,
        WM_MDIRESTORE = 0x00000223,
        WM_MDISETMENU = 0x00000230,
        WM_MDITILE = 0x00000226,
        WM_MEASUREITEM = 0x0000002C,
        WM_MENUCHAR = 0x00000120,
        WM_MENUCOMMAND = 0x00000126,
        WM_MENUDRAG = 0x00000123,
        WM_MENUGETOBJECT = 0x00000124,
        WM_MENURBUTTONUP = 0x00000122,
        WM_MENUSELECT = 0x0000011F,
        WM_MOUSEACTIVATE = 0x00000021,
        WM_MOUSEFIRST = 0x00000200,
        WM_MOUSEHOVER = 0x000002A1,
        WM_MOUSELAST = 0x0000020D,
        WM_MOUSELEAVE = 0x000002A3,
        WM_MOUSEMOVE = 0x00000200,
        WM_MOUSEWHEEL = 0x0000020A,
        WM_MOVE = 0x00000003,
        WM_MOVING = 0x00000216,
        WM_NCACTIVATE = 0x00000086,
        WM_NCCALCSIZE = 0x00000083,
        WM_NCCREATE = 0x00000081,
        WM_NCDESTROY = 0x00000082,
        WM_NCHITTEST = 0x00000084,
        WM_NCLBUTTONDBLCLK = 0x000000A3,
        WM_NCLBUTTONDOWN = 0x000000A1,
        WM_NCLBUTTONUP = 0x000000A2,
        WM_NCMBUTTONDBLCLK = 0x000000A9,
        WM_NCMBUTTONDOWN = 0x000000A7,
        WM_NCMBUTTONUP = 0x000000A8,
        WM_NCMOUSELEAVE = 0x000002A3,
        WM_NCMOUSEMOVE = 0x000000A0,
        WM_NCPAINT = 0x00000085,
        WM_NCRBUTTONDBLCLK = 0x000000A6,
        WM_NCRBUTTONDOWN = 0x000000A4,
        WM_NCRBUTTONUP = 0x000000A5,
        WM_NCXBUTTONDBLCLK = 0x000000AD,
        WM_NCXBUTTONDOWN = 0x000000AB,
        WM_NCXBUTTONUP = 0x000000AC,
        WM_NEXTDLGCTL = 0x00000028,
        WM_NEXTMENU = 0x00000213,
        WM_NOTIFY = 0x0000004E,
        WM_NOTIFYFORMAT = 0x00000055,
        WM_NULL = 0x00000000,
        /// <summary>No longer supported.</summary>
        WM_OTHERWINDOWCREATED = 0x00000042,
        /// <summary>No longer supported.</summary>
        WM_OTHERWINDOWDESTROYED = 0x00000043,
        WM_PAINT = 0x0000000F,
        WM_PAINTCLIPBOARD = 0x00000309,
        WM_PAINTICON = 0x00000026,
        WM_PALETTECHANGED = 0x00000311,
        WM_PALETTEISCHANGING = 0x00000310,
        WM_PARENTNOTIFY = 0x00000210,
        WM_PASTE = 0x00000302,
        WM_PENWINFIRST = 0x00000380,
        WM_PENWINLAST = 0x0000038F,
        WM_POWER = 0x00000048,
        WM_POWERBROADCAST = 0x00000218,
        WM_PRINT = 0x00000317,
        WM_PRINTCLIENT = 0x00000318,
        WM_PSD_ENVSTAMPRECT = (WM_USER + 5),
        WM_PSD_FULLPAGERECT = (WM_USER + 1),
        WM_PSD_GREEKTEXTRECT = (WM_USER + 4),
        WM_PSD_MARGINRECT = (WM_USER + 3),
        WM_PSD_MINMARGINRECT = (WM_USER + 2),
        WM_PSD_PAGESETUPDLG = (WM_USER),
        WM_PSD_YAFULLPAGERECT = (WM_USER + 6),
        WM_QUERYDRAGICON = 0x00000037,
        WM_QUERYENDSESSION = 0x00000011,
        WM_QUERYNEWPALETTE = 0x0000030F,
        WM_QUERYOPEN = 0x00000013,
        WM_QUERYUISTATE = 0x00000129,
        WM_QUEUESYNC = 0x00000023,
        WM_QUIT = 0x00000012,
        WM_RBUTTONDBLCLK = 0x00000206,
        WM_RBUTTONDOWN = 0x00000204,
        WM_RBUTTONUP = 0x00000205,
        WM_RENDERALLFORMATS = 0x00000306,
        WM_RENDERFORMAT = 0x00000305,
        WM_REFLECT = 0x00002000,
        WM_SETCURSOR = 0x00000020,
        WM_SETFOCUS = 0x00000007,
        WM_SETFONT = 0x00000030,
        WM_SETHOTKEY = 0x00000032,
        WM_SETICON = 0x00000080,
        WM_SETREDRAW = 0x0000000B,
        WM_SETTEXT = 0x0000000C,
        WM_SETTINGCHANGE = WM_WININICHANGE,
        WM_SHOWWINDOW = 0x00000018,
        WM_SIZE = 0x00000005,
        WM_SIZING = 0x00000214,
        WM_SIZECLIPBOARD = 0x0000030B,
        WM_SPOOLERSTATUS = 0x0000002A,
        WM_STYLECHANGED = 0x0000007D,
        WM_STYLECHANGING = 0x0000007C,
        WM_SYNCPAINT = 0x00000088,
        WM_SYSCHAR = 0x00000106,
        WM_SYSCOLORCHANGE = 0x00000015,
        WM_SYSCOMMAND = 0x00000112,
        WM_SYSDEADCHAR = 0x00000107,
        WM_SYSKEYDOWN = 0x00000104,
        WM_SYSKEYUP = 0x00000105,
        WM_TABLET_FIRST = 0x000002C0,
        WM_TABLET_LAST = 0x000002DF,
        WM_TCARD = 0x00000052,
        WM_THEMECHANGED = 0x0000031A,
        WM_TIMECHANGE = 0x0000001E,
        WM_TIMER = 0x00000113,
        WM_UNDO = 0x00000304,
        WM_UNICHAR = 0x00000109,
        WM_UNINITMENUPOPUP = 0x00000125,
        WM_UPDATEUISTATE = 0x00000128,
        WM_USER = 0x00000400,
        WM_USERCHANGED = 0x00000054,
        WM_VKEYTOITEM = 0x0000002E,
        WM_VSCROLL = 0x00000115,
        WM_VSCROLLCLIPBOARD = 0x0000030A,
        WM_WINDOWPOSCHANGED = 0x00000047,
        WM_WINDOWPOSCHANGING = 0x00000046,
        WM_WININICHANGE = 0x0000001A,
        WM_WTSSESSIONCHANGE = 0x000002B1,
        WM_XBUTTONDOWN = 0x0000020B,
        WM_XBUTTONUP = 0x0000020C,
        WM_XBUTTONDBLCLK = 0x0000020D,
        WS_BORDER = 0x800000,
        /// <summary>WS_BORDER Or WS_DLGFRAME</summary>
        WS_CAPTION = 0xC00000,
        WS_CHILD = 0x40000000,
        WS_CHILDWINDOW = (WS_CHILD),
        WS_CLIPCHILDREN = 0x2000000,
        WS_CLIPSIBLINGS = 0x4000000,
        WS_DISABLED = 0x8000000,
        WS_DLGFRAME = 0x400000,
        WS_EX_ACCEPTFILES = 0x10,
        WS_EX_DLGMODALFRAME = 0x1,
        WS_EX_NOPARENTNOTIFY = 0x4,
        WS_EX_TOPMOST = 0x8,
        WS_EX_TRANSPARENT = 0x20,
        WS_GROUP = 0x20000,
        WS_HSCROLL = 0x100000,
        WS_MINIMIZE = 0x20000000,
        WS_ICONIC = WS_MINIMIZE,
        WS_MAXIMIZE = 0x1000000,
        WS_MAXIMIZEBOX = 0x10000,
        WS_MINIMIZEBOX = 0x20000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x40000,
        WS_OVERLAPPED = 0x0,
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_POPUP = 0x80000000,
        WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TABSTOP = 0x00010000,
        WS_TILED = WS_OVERLAPPED,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        WS_VISIBLE = 0x10000000,
        WS_VSCROLL = 0x00200000,
        LBS_DISABLENOSCROLL = 0x00001000,
        LBS_EXTENDEDSEL = 0x00000800,
        LBS_HASSTRINGS = 0x00000040,
        LBS_MULTICOLUMN = 0x00000200,
        LBS_MULTIPLESEL = 0x00000008,
        LBS_NODATA = 0x00002000,
        LBS_NOINTEGRALHEIGHT = 0x00000100,
        LBS_NOREDRAW = 0x00000004,
        LBS_NOTIFY = 0x00000001,
        LBS_OWNERDRAWFIXED = 0x00000010,
        LBS_OWNERDRAWVARIABLE = 0x00000020,
        LBS_SORT = 0x0000002,
        LBS_STANDARD = (LBS_NOTIFY | LBS_SORT | WS_VSCROLL | WS_BORDER),
        LBS_USETABSTOPS = 0x00000080,
        LBS_WANTKEYBOARDINPUT = 0x00000400,
        //LBSELCHSTRING = "commdlg_LBSelChangedNotify",
        TB_ENABLEBUTTON = (WM_USER + 1),
        TB_CHECKBUTTON = (WM_USER + 2),
        TB_PRESSBUTTON = (WM_USER + 3),
        TB_HIDEBUTTON = (WM_USER + 4),
        TB_INDETERMINATE = (WM_USER + 5),
        TB_MARKBUTTON = (WM_USER + 6),
        TB_ISBUTTONENABLED = (WM_USER + 9),
        TB_ISBUTTONCHECKED = (WM_USER + 10),
        TB_ISBUTTONPRESSED = (WM_USER + 11),
        TB_ISBUTTONHIDDEN = (WM_USER + 12),
        TB_ISBUTTONINDETERMINATE = (WM_USER + 13),
        TB_ISBUTTONHIGHLIGHTED = (WM_USER + 14),
        TB_SETSTATE = (WM_USER + 17),
        TB_GETSTATE = (WM_USER + 18),
        TB_ADDBITMAP = (WM_USER + 19),
        TB_ADDBUTTONSA = (WM_USER + 20),
        TB_INSERTBUTTONA = (WM_USER + 21),
        TB_ADDBUTTONS = (WM_USER + 20),
        TB_INSERTBUTTON = (WM_USER + 21),
        TB_DELETEBUTTON = (WM_USER + 22),
        TB_GETBUTTON = (WM_USER + 23),
        TB_BUTTONCOUNT = (WM_USER + 24),
        TB_COMMANDTOINDEX = (WM_USER + 25),
        TB_SAVERESTOREA = (WM_USER + 26),
        TB_SAVERESTOREW = (WM_USER + 76),
        TB_CUSTOMIZE = (WM_USER + 27),
        TB_ADDSTRINGA = (WM_USER + 28),
        TB_ADDSTRINGW = (WM_USER + 77),
        TB_GETITEMRECT = (WM_USER + 29),
        TB_BUTTONSTRUCTSIZE = (WM_USER + 30),
        TB_SETBUTTONSIZE = (WM_USER + 31),
        TB_SETBITMAPSIZE = (WM_USER + 32),
        TB_AUTOSIZE = (WM_USER + 33),
        TB_GETTOOLTIPS = (WM_USER + 35),
        TB_SETTOOLTIPS = (WM_USER + 36),
        TB_SETPARENT = (WM_USER + 37),
        TB_SETROWS = (WM_USER + 39),
        TB_GETROWS = (WM_USER + 40),
        TB_SETCMDID = (WM_USER + 42),
        TB_CHANGEBITMAP = (WM_USER + 43),
        TB_GETBITMAP = (WM_USER + 44),
        TB_GETBUTTONTEXTA = (WM_USER + 45),
        TB_GETBUTTONTEXTW = (WM_USER + 75),
        TB_REPLACEBITMAP = (WM_USER + 46),
        TB_SETINDENT = (WM_USER + 47),
        TB_SETIMAGELIST = (WM_USER + 48),
        TB_GETIMAGELIST = (WM_USER + 49),
        TB_LOADIMAGES = (WM_USER + 50),
        TB_GETRECT = (WM_USER + 51), // wParam is the Cmd instead of index
        TB_SETHOTIMAGELIST = (WM_USER + 52),
        TB_GETHOTIMAGELIST = (WM_USER + 53),
        TB_SETDISABLEDIMAGELIST = (WM_USER + 54),
        TB_GETDISABLEDIMAGELIST = (WM_USER + 55),
        TB_SETSTYLE = (WM_USER + 56),
        TB_GETSTYLE = (WM_USER + 57),
        TB_GETBUTTONSIZE = (WM_USER + 58),
        TB_SETBUTTONWIDTH = (WM_USER + 59),
        TB_SETMAXTEXTROWS = (WM_USER + 60),
        TB_GETTEXTROWS = (WM_USER + 61),
        TBSTYLE_BUTTON = 0x00000000,
        TBSTYLE_SEP = 0x00000001,
        TBSTYLE_CHECK = 0x00000002,
        TBSTYLE_GROUP = 0x00000004,
        TBSTYLE_CHECKGROUP = (TBSTYLE_GROUP | TBSTYLE_CHECK),
        TBSTYLE_DROPDOWN = 0x00000008,
        /// <summary>Automatically calculate the cx of the button.</summary>
        TBSTYLE_AUTOSIZE = 0x00000010,
        /// <summary>If this button should not have accel prefix.</summary>
        TBSTYLE_NOPREFIX = 0x00000020,
        TBSTYLE_TOOLTIPS = 0x00000100,
        TBSTYLE_WRAPABLE = 0x00000200,
        TBSTYLE_ALTDRAG = 0x00000400,
        TBSTYLE_FLAT = 0x00000800,
        TBSTYLE_LIST = 0x00001000,
        TBSTYLE_CUSTOMERASE = 0x00002000,
        TBSTYLE_REGISTERDROP = 0x00004000,
        TBSTYLE_TRANSPARENT = 0x00008000,
        TBSTYLE_EX_DRAWDDARROWS = 0x00000001,
    }
    public enum InputType : int
    {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2
    }
    public enum MOUSEEVENTF : uint
    {
        MOVE = Win32Const.MOUSEEVENTF_MOVE,
        LEFTDOWN = Win32Const.MOUSEEVENTF_LEFTDOWN,
        LEFTUP = Win32Const.MOUSEEVENTF_LEFTUP,
        RIGHTDOWN = Win32Const.MOUSEEVENTF_RIGHTDOWN,
        RIGHTUP = Win32Const.MOUSEEVENTF_RIGHTUP,
        MIDDLEDOWN = Win32Const.MOUSEEVENTF_MIDDLEDOWN,
        MIDDLEUP = Win32Const.MOUSEEVENTF_MIDDLEUP,
        XDOWN = Win32Const.MOUSEEVENTF_XDOWN,
        XUP = Win32Const.MOUSEEVENTF_XUP,
        WHEEL = Win32Const.MOUSEEVENTF_WHEEL,
        VIRTUALDESK = Win32Const.MOUSEEVENTF_VIRTUALDESK,
        ABSOLUTE = Win32Const.MOUSEEVENTF_ABSOLUTE
    }
    public enum KEYEVENTF : uint
    {
        EXTENDEDKEY = Win32Const.KEYEVENTF_EXTENDEDKEY,
        KEYUP = Win32Const.KEYEVENTF_KEYUP,
        UNICODE = Win32Const.KEYEVENTF_UNICODE,
        SCANCODE = Win32Const.KEYEVENTF_SCANCODE
    }
    public enum VK : ushort
    {
        SHIFT = 0x10,
        CONTROL = 0x11,
        MENU = 0x12,
        ESCAPE = 0x1B,
        BACK = 0x08,
        TAB = 0x09,
        RETURN = 0x0D,
        PRIOR = 0x21,
        NEXT = 0x22,
        END = 0x23,
        HOME = 0x24,
        LEFT = 0x25,
        UP = 0x26,
        RIGHT = 0x27,
        DOWN = 0x28,
        SELECT = 0x29,
        PRINT = 0x2A,
        EXECUTE = 0x2B,
        SNAPSHOT = 0x2C,
        INSERT = 0x2D,
        DELETE = 0x2E,
        HELP = 0x2F,
        NUMPAD0 = 0x60,
        NUMPAD1 = 0x61,
        NUMPAD2 = 0x62,
        NUMPAD3 = 0x63,
        NUMPAD4 = 0x64,
        NUMPAD5 = 0x65,
        NUMPAD6 = 0x66,
        NUMPAD7 = 0x67,
        NUMPAD8 = 0x68,
        NUMPAD9 = 0x69,
        MULTIPLY = 0x6A,
        ADD = 0x6B,
        SEPARATOR = 0x6C,
        SUBTRACT = 0x6D,
        DECIMAL = 0x6E,
        DIVIDE = 0x6F,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        OEM_1 = 0xBA,   // ',:' for US
        OEM_PLUS = 0xBB,   // '+' any country
        OEM_COMMA = 0xBC,   // ',' any country
        OEM_MINUS = 0xBD,   // '-' any country
        OEM_PERIOD = 0xBE,   // '.' any country
        OEM_2 = 0xBF,   // '/?' for US
        OEM_3 = 0xC0,   // '`~' for US
        MEDIA_NEXT_TRACK = 0xB0,
        MEDIA_PREV_TRACK = 0xB1,
        MEDIA_STOP = 0xB2,
        MEDIA_PLAY_PAUSE = 0xB3,
        LWIN = 0x5B,
        RWIN = 0x5C
    }
    /// <summary>
    /// Defines the style bits that a window can have
    /// </summary>
    [Flags()]
    public enum WindowStyles : uint
    {
        /// <summary>
        /// A window that has a thin-line border.
        /// </summary>
        WS_BORDER = 0x800000,
        /// <summary>
        /// A window that has a title bar (includes WS_BORDER style.
        /// </summary>
        WS_CAPTION = 0xC00000,
        /// <summary>
        /// A child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
        /// </summary>
        WS_CHILD = 0x40000000,
        /// <summary>
        /// Same as the WS_CHILD style.
        /// </summary>
        WS_CHILDWINDOW = (WS_CHILD),
        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs with the parent window.
        /// </summary>
        WS_CLIPCHILDREN = 0x2000000,
        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message,
        /// the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be
        /// updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing with the client
        /// area of a child window, to draw within the client area of a neighboring child window. */
        /// </summary>
        WS_CLIPSIBLINGS = 0x4000000,
        /// <summary>
        /// A window that is disabled and cannot receive input from the user. To change this, use 'EnableWindow'.
        /// </summary>
        WS_DISABLED = 0x8000000,
        /// <summary>
        /// A window that has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
        /// </summary>
        WS_DLGFRAME = 0x400000,
        /// <summary>
        /// Specifies the first control of a group of controls. The group consists of this first control and all controls
        /// defined after it, up to the next control with the WS_GROUP style. The first control in each group usually has
        /// the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard
        /// focus from one control in the group to the next by using the direction keys. You can turn this style on and off
        /// to change dialog box navigation. To change this style after a window has been created, use 'SetWindowLong'.
        /// </summary>
        WS_GROUP = 0x20000,
        /// <summary>
        /// A window that has a horizontal scroll bar.
        /// </summary>
        WS_HSCROLL = 0x100000,
        /// <summary>
        /// A window that is initially maximized.
        /// </summary>
        WS_MAXIMIZE = 0x1000000,
        /// <summary>
        /// A window that has a maximize button. Cannot combine with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must
        /// also be specified.
        /// </summary>
        WS_MAXIMIZEBOX = 0x10000,
        /// <summary>
        /// A window that is initially minimized.
        /// </summary>
        WS_MINIMIZE = 0x20000000,
        /// <summary>
        /// A window that has a minimize button. Cannot combine with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must
        /// also be specified.
        /// </summary>
        WS_MINIMIZEBOX = 0x20000,
        /// <summary>
        /// An overlapped window. An overlapped window has a title bar and border. Same as the WS_TILED style.
        /// </summary>
        WS_OVERLAPPED = 0x0,
        /// <summary>
        /// A pop-up window. This style cannot be used with the WS_CHILD style.
        /// </summary>
        WS_POPUP = 0x80000000,
        /// <summary>
        /// A window that has a window menu on its title bar. The WS_CAPTION style must also be specified.
        /// </summary>
        WS_SYSMENU = 0x80000,
        /// <summary>
        /// Specifies a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key
        /// changes the keyboard focus to the next control with the WS_TABSTOP style. You can turn this style on and off to
        /// change dialog box navigation. To change this style after a window has been created, us 'SetWindowLong'.
        /// </summary>
        WS_TABSTOP = 0x10000,
        /// <summary>
        /// A window that has a sizing border. Same as the WS_SIZEBOX style.
        /// </summary>
        WS_THICKFRAME = 0x40000,
        /// <summary>
        /// A window that is initially visible. This style can be turned on and off by using 'ShowWindow' or 'SetWindowPos'.
        /// </summary>
        WS_VISIBLE = 0x10000000,
        /// <summary>
        /// A window that has a vertical scroll bar.
        /// </summary>
        WS_VSCROLL = 0x200000,
        /// <summary>
        /// Save as WS_MINIMIZE style.
        /// </summary>
        WS_ICONIC = WS_MINIMIZE,
        /// <summary>
        /// A pop-up window with the WS_BORDER, WS_POPUP, and WS_SYSMENU styles. The WS_CAPTION and WS_POPUPWINDOW styles
        /// must be combined to make the window menu visible.
        /// </summary>
        WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
        /// <summary>
        /// A window that has a sizing border. Same as the WS_THICKFRAME style.
        /// </summary>
        WS_SIZEBOX = WS_THICKFRAME,
        /// <summary>
        /// An overlapped window which has has a title bar and a border. Same as the WS_OVERLAPPED style.
        /// </summary>
        WS_TILED = WS_OVERLAPPED,
        /// <summary>
        /// An overlapped window with the WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX,
        /// and WS_MAXIMIZEBOX styles. Same as the WS_TILEDWINDOW style.
        /// </summary>
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX)
    }
    [Flags()]
    public enum ExtendedWindowStyles : uint
    {
        WS_EX_ACCEPTFILES = 0x10,
        WS_EX_DLGMODALFRAME = 0x1,
        WS_EX_NOPARENTNOTIFY = 0x4,
        WS_EX_TOPMOST = 0x8,
        WS_EX_TRANSPARENT = 0x20,
        WS_EX_TOOLWINDOW = 0x80,
        WS_EX_APPWINDOW = 0x40000
    }
    public enum ShowWindowCmds
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_FORCEMINIMIZE = 11,
        SW_MAX = 11
    }
    [Flags()]
    public enum RedrawWindowFlags : uint
    {
        RDW_INVALIDATE = 0x0001,
        RDW_INTERNALPAINT = 0x0002,
        RDW_ERASE = 0x0004,
        RDW_VALIDATE = 0x0008,
        RDW_NOINTERNALPAINT = 0x0010,
        RDW_NOERASE = 0x0020,
        RDW_NOCHILDREN = 0x0040,
        RDW_UPDATENOW = 0x0100,
        RDW_ERASENAME = 0x0200,
        RDW_FRAME = 0x0400,
        RDW_NOFRAME = 0x0800
    }
    public enum ScrollBarObjectID : uint
    {
        HSCROLL = 0xFFFFFFFA,
        VSCROLL = 0xFFFFFFFB,
        CLIENT = 0xFFFFFFFC
    }
    [Flags()]
    public enum HotkeyModifiers
    {
        MOD_ALT = 0x0001,
        MOD_CONTROL = 0x0002,
        MOD_SHIFT = 0x0004,
        MOD_WIN = 0x0008
    }
}
