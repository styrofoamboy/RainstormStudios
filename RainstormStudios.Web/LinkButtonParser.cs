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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace RainstormStudios.Web
{
    public class LinkButtonParser
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        const System.Reflection.BindingFlags flags =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.InvokeMethod |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.FlattenHierarchy;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static bool ExecuteLinkCommand(string cmdNm, object cmdArg, object caller, object sender)
        {
            if (string.IsNullOrEmpty(cmdNm))
                return false;
            string args = cmdArg.ToString();
            System.Reflection.MethodInfo mi = LinkButtonParser.ParseLinkCommand(cmdNm, args, caller.GetType());
            if (mi != null)
                if (string.IsNullOrEmpty(args))
                    mi.Invoke(caller, new object[] { (System.Web.UI.Control)sender });
                else
                    mi.Invoke(caller, new object[] { (System.Web.UI.Control)sender, args.Split('|') });
            else
                return false;
            return true;
        }
        public static System.Reflection.MethodInfo ParseLinkCommand(string cmdNm, string cmdArg, Type miType)
        {
            if (!string.IsNullOrEmpty(cmdArg))
                return miType.GetMethod(cmdNm, flags, null, new Type[] { typeof(System.Web.UI.Control), typeof(System.String[]) }, null);
            else
                return miType.GetMethod(cmdNm, flags, null, new Type[] { typeof(System.Web.UI.Control) }, null);
        }
        public static bool ShowFloatingPanel(string targetControlID, object caller, System.Web.UI.WebControls.Panel panMain)
        {
            if (string.IsNullOrEmpty(targetControlID) || caller == null)
                return false;

            Type callerType = caller.GetType();
            System.Reflection.MemberInfo[] mi = callerType.GetMember(targetControlID, System.Reflection.MemberTypes.Field, flags);

            if (mi.Length > 0)
            {
                System.Reflection.FieldInfo fldMbr = (System.Reflection.FieldInfo)mi[0];
                if (fldMbr.FieldType.IsSubclassOf(typeof(System.Web.UI.Control)))
                {
                    try
                    {
                        object objTarget = fldMbr.GetValue(caller);
                        ((System.Web.UI.Control)objTarget).Visible = true;
                        panMain.Enabled = false;
                        return true;
                    }
                    catch { }
                }
            }

            return false;
        }
        public static bool HideFloatingPanel(System.Web.UI.Control sender, System.Web.UI.WebControls.Panel panMain)
        {
            if (sender == null)
                return false;

            Control p = sender.Parent;
            bool found = false;
            while (p != null)
            {
                if (p.GetType().Name == "Panel" && ((Panel)p).CssClass.ToLower() == "floatingpanel")
                {
                    p.Visible = false;
                    found = true;
                    break;
                }
                p = p.Parent;
            }
            if (panMain != null)
                panMain.Enabled = true;
            return found;
        }
        #endregion
    }
}
