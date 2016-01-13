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
using System.Web;
using System.Web.UI;

namespace RainstormStudios.Web
{
    [Author("Unfried, Michael")]
    public static class WebExtensionMethods
    {
        /// <summary>
        /// Finds the master page of type 'T' from which this page inherits.  If no master page of the given type is found, a NULL will be returned.
        /// </summary>
        /// <typeparam name="T">The Type-name of a class that inherits from <see cref="T:System.Web.MasterPage"/>.</typeparam>
        /// <param name="pg">The current page.</param>
        /// <returns>A class of the type specified by 'T' from which this page inherits from, if found. Otherwise, NULL.</returns>
        public static T FindMasterOfType<T>(this Page pg)
            where T : MasterPage
        {
            return RainstormStudios.Web.WebHelper.FindMasterOfType<T>(pg);
        }
        /// <summary>
        /// Finds the specified child control and casts it to the specified type, or returns a NULL value if a control that the specified type is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="controlID"></param>
        /// <returns></returns>
        public static T FindControl<T>(this System.Web.UI.Control ctrl, string controlID, bool throwExceptionIfNull = true)
            where T : System.Web.UI.Control
        {
            var childCtrl = (ctrl.FindControl(controlID) as T);
            if (childCtrl == null && throwExceptionIfNull)
                throw new ControlNotFoundException(controlID, ctrl);
            else
                return childCtrl;
        }

        public static T FindControlRecursive<T>(this System.Web.UI.Control parentControl, string id) where T : System.Web.UI.Control
        {
            T ctrl = default(T);

            if (parentControl.ID == id)
                return (T)parentControl;

            foreach (Control c in parentControl.Controls)
            {
                ctrl = c.FindControlRecursive<T>(id);

                if (ctrl != null)
                    break;
            }
            return ctrl;
        }

        public static string[] GetValidatorErrors(this System.Web.UI.Page pg, string validationGroupName = null)
        {
            if (string.IsNullOrEmpty(validationGroupName))
                pg.Validate();
            else
                pg.Validate(validationGroupName);

            if (!pg.IsValid)
            {
                List<IValidator> errored = pg.Validators.Cast<IValidator>().Where(v => !v.IsValid).ToList();
                return errored.Select(v => v.ErrorMessage).ToArray();
            }
            else
                return null;
        }
    }
    public class ControlNotFoundException : Exception
    {
        public readonly string
            ControlID;
        public readonly System.Web.UI.Control
            ParentControl;

        public ControlNotFoundException(string controlID, System.Web.UI.Control parentCtrl)
            : base("Control not found: " + controlID)
        {
            this.ControlID = controlID;
            this.ParentControl = parentCtrl;
        }
    }
}
