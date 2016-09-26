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
using System.Windows.Forms;
using System.Reflection;

namespace RainstormStudios
{
    /// <summary>
    /// Provides static methods for accessing UI Forms and Controls across thread boundaries.
    /// </summary>
    public sealed class CrossThreadUI
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static readonly BindingFlags
            Binding = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// A bool value indicating 'True' if thread delegates should be executed synchronously. Otherwise, false.
        /// </summary>
        public static bool
            ExecSync = true;
        //***************************************************************************
        // Cross-Thread Delegates
        // 
        private delegate void RefreshControlDelegate(Control ctrl);
        private delegate void SetTextDelegate(Control ctrl, string text);
        private delegate void SetBoolDelegate(Control ctrl, bool visible);
        private delegate void SetIntDelegate(Control ctrl, int value);
        private delegate void SetPropertyDelegate(Control ctrl, string propertyName, object value, object[] args);
        private delegate void ShowMessageBoxDelegate(IWin32Window owner, string msg, string caption, MessageBoxButtons btns, MessageBoxIcon icon, MessageBoxDefaultButton defBtn);
        private delegate object InvokeMethodDelegate(Control ctrl, string methName, object[] args);
        private delegate object GetPropertyInstanceDelegate(Control ctrl, string propertyName);
        private delegate object InvokePropertyMethodDelegate(Control ctrl, string propertyName, string methodName, object[] args);
        private delegate void SetObjectPropertyValueDelegate(object ctrl, string propertyName, object value, object[] args);
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Tells the specified control to invalidate its client area and immediately redraw itself.
        /// </summary>
        /// <param name="ctrl"></param>
        public static void RefreshControl(Control ctrl)
        {
            if (ctrl.InvokeRequired)
            {
                RefreshControlDelegate del = new RefreshControlDelegate(CrossThreadUI.RefreshControl);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del);
                else
                    ctrl.BeginInvoke(del);
            }
            else
                ctrl.Invalidate();
        }
        /// <summary>
        /// Sets the 'Text' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose text property will be set.</param>
        /// <param name="text">The text to be assigned to the property.</param>
        public static void SetText(Control ctrl, string text)
        {
            if (ctrl.InvokeRequired)
            {
                SetTextDelegate del = new SetTextDelegate(CrossThreadUI.SetText);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, text);
                else
                    ctrl.BeginInvoke(del, ctrl, text);
            }
            else
                ctrl.Text = text;
        }
        /// <summary>
        /// Sets the 'Visible' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose visible property will be set.</param>
        /// <param name="visible">The System.Boolean value to assign to the property.</param>
        public static void SetVisible(Control ctrl, bool visible)
        {
            if (ctrl.InvokeRequired)
            {
                SetBoolDelegate del = new SetBoolDelegate(CrossThreadUI.SetVisible);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, visible);
                else
                    ctrl.BeginInvoke(del, ctrl, visible);
            }
            else
                ctrl.Visible = visible;
        }
        /// <summary>
        /// Sets the 'Enabled' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose enabled property will be set.</param>
        /// <param name="enabled">The System.Boolean value to assign to the property.</param>
        public static void SetEnabled(Control ctrl, bool enabled)
        {
            if (ctrl.InvokeRequired)
            {
                SetBoolDelegate del = new SetBoolDelegate(CrossThreadUI.SetEnabled);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, enabled);
                else
                    ctrl.BeginInvoke(del, ctrl, enabled);
            }
            else
                ctrl.Enabled = enabled;
        }
        /// <summary>
        /// Sets the 'Width' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose width property will be set.</param>
        /// <param name="value">The System.Int32 value to assign to the property.</param>
        public static void SetWidth(Control ctrl, int value)
        {
            if (ctrl.InvokeRequired)
            {
                SetIntDelegate del = new SetIntDelegate(CrossThreadUI.SetWidth);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, value);
                else
                    ctrl.BeginInvoke(del, ctrl, value);
            }
            else
                ctrl.Width = value;
        }
        /// <summary>
        /// Sets the 'Height' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose height property will be set.</param>
        /// <param name="value">The System.Int32 value to assign to the property.</param>
        public static void SetHeight(Control ctrl, int value)
        {
            if (ctrl.InvokeRequired)
            {
                SetIntDelegate del = new SetIntDelegate(CrossThreadUI.SetHeight);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, value);
                else
                    ctrl.BeginInvoke(del, ctrl, value);
            }
            else
                ctrl.Height = value;
        }
        /// <summary>
        /// Sets the 'Checked' property of a given control.  An exception will be thrown if the control does not expose a property called 'Checked'.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose checked property will be set.</param>
        /// <param name="value">The Sytem.Boolean value to assign to the property.</param>
        public static void SetChecked(Control ctrl, bool value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "Checked", value); }
            catch { throw; }
        }
        /// <summary>
        /// Sets the 'DataSource' property of a given control.  An exception will be thrown if the control does not expose a property called 'DataSource'.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose DataSource property will be set.</param>
        /// <param name="value">The System.Object value to assign to the property.</param>
        public static void SetDataSource(Control ctrl, object value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "DataSource", value); }
            catch { throw; }
        }
        /// <summary>
        /// Sets the 'Value' property of a given control.  An exception will be thrown if the control does not expose a property called 'Value'.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose Value property will be set.</param>
        /// <param name="value">The System.Object value to assign to the property.</param>
        public static void SetValue(Control ctrl, object value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "Value", value); }
            catch { throw; }
        }
        /// <summary>
        /// Sets a specified property value on a given control.  An exception will be thrown if the control does not expose a property with the specified name.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose property will be set.</param>
        /// <param name="propertyName">The name of the Property member that will receive the new value.</param>
        /// <param name="value">The value to assign to the property.  This object must be properly typed to match the property's signature.</param>
        /// <param name="args">Any optional arguments that must be provided to access this property.  Array objects must be properly typed to match property's signature.</param>
        public static void SetPropertyValue(Control ctrl, string propertyName, object value, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                SetPropertyDelegate del = new SetPropertyDelegate(CrossThreadUI.SetPropertyValue);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, propertyName, value, args);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName, value, args);
            }
            else
            {
                Type ctrlType = ctrl.GetType();
                PropertyInfo pi = ctrlType.GetProperty(propertyName, Binding);

                if (pi != null)
                {
                    // Make sure the object "Value" matches the property's Type.
                    if (!value.GetType().IsAssignableFrom(pi.PropertyType))
                        throw new ArgumentException("Value Type does not match property Type.", "value");

                    try
                    { pi.SetValue(ctrl, ((pi.PropertyType.FullName == "System.String") ? value.ToString() : value), args); }
                    catch { throw; }
                }
                else
                    throw new ArgumentException("Specified control does not expose a '" + propertyName + "' property.", "propertyName");
            }
        }
        /// <summary>
        /// Sets the specified property value on an object that does not inherit from System.Windows.Forms.Control.  An exception will be thrown if the control
        /// does not expose a property with the specified name or it has neither an 'InvokeRequired' property nor an 'Owner' or 'Parent' property with
        /// an 'InvokeRequired' property.
        /// </summary>
        /// <param name="ctrl">The object instance whose property will be set.</param>
        /// <param name="propertyName">The name of the Property member that will receive the new value.</param>
        /// <param name="value">The value to assign to the property.  This object must be properly typed to match the property's signature.</param>
        /// <param name="args">Any optional arguments that must be provided to access this property.  Array objects must be properly typed to match property's signature.</param>
        public static void SetPropertyValue(object ctrl, string propertyName, object value, params object[] args)
        {
            // The first thing we have to do is figure out if we can tell whether or
            //   not we're on the right thread.  If we can't tell that, then this
            //   whole thing is useless.
            bool needsInvoke = false;
            object parent = ctrl;
            // Get the object's Type.
            Type objType = ctrl.GetType();
            PropertyInfo piInvoke = null;

            // Try and see if the passed object has a property called
            //   "InvokeRequired".  Probably not, so...
            if ((piInvoke = objType.GetProperty("InvokeRequired")) != null)
            {
                object retVal = piInvoke.GetValue(ctrl, null);
                needsInvoke = (retVal == null || retVal.GetType().Name != "Boolean") ? false : (bool)retVal;
            }
            else if ((piInvoke = objType.GetProperty("Owner")) != null || (piInvoke = objType.GetProperty("Parent")) != null)
            {
                // If it doesn't have the "InvokeRequired" property, we check to
                //   see if it has a property named "Owner"
                object owner = piInvoke.GetValue(ctrl, null);
                if (owner != null)
                {
                    // If it does, grab the owner and get it's Type.
                    Type ownType = owner.GetType();
                    // Now we'll check to see if the owner has an "InvokeRequired"
                    //   property. This is the way ToolStripMenus and ToolBarMenus
                    //   work.
                    PropertyInfo pOwn = ownType.GetProperty("InvokeRequired");
                    if (pOwn != null)
                    {
                        object retVal = pOwn.GetValue(owner, null);
                        needsInvoke = (retVal == null || retVal.GetType().Name != "Boolean") ? false : (bool)retVal;
                        // We have to remember which object defines our current
                        //   thread context.
                        parent = owner;
                    }
                }
            }
            else
                // If none of that worked, then we can't figure it out, so just throw
                //   an error.  Otherwise we'll end up triggering one by accident or
                //   in an infinate loop.
                throw new ArgumentException("Cannot determine thread context from the given object.");


            // If made it this far, we found some way to determine thread context, so
            //   now this starts to look a little more familiar.
            if (needsInvoke)
            {
                // The 'parent' variable will be a reference to either the passed
                //   object or it's owner, if an "Owner" property was found.  This,
                //   also, is for ToolStripMenus and ToolBarMenus.  Individual
                //   ToolBarButtons and ToolStripMenuItems do not have "Invoke" or
                //   "BeginInvoke" methods.  We have to call their owner's methods.
                Type parType = parent.GetType();
                Type[] paramTypes = new Type[] { typeof(Delegate), typeof(object[]) };
                MethodInfo miInvoke = null;
                // Create the delegate like normal;
                SetObjectPropertyValueDelegate del = new SetObjectPropertyValueDelegate(CrossThreadUI.SetPropertyValue);
                // Then try and find the "Invoke" or "BeginInvoke" method. If we
                //   don't find either, we have to throw an exception.
                if (CrossThreadUI.ExecSync && ((miInvoke = parType.GetMethod("Invoke", paramTypes)) != null))
                    miInvoke.Invoke(parent, new object[] { del, new object[] { ctrl, propertyName, value, args } });
                else if ((miInvoke = parType.GetMethod("BeginInvoke", paramTypes)) != null)
                    miInvoke.Invoke(parent, new object[] { del, new object[] { ctrl, propertyName, value, args } });
                else
                    throw new ArgumentException("Specified object does not expose any default methods for invoking methods on its executing thread.");
            }
            else
            {
                // Now that we've determined thread context and decided we're on the
                //   correct thread, lets set the property value.
                PropertyInfo pi = objType.GetProperty(propertyName, CrossThreadUI.Binding);

                // Make sure the passed object's Type matches the property's Type.
                if (pi.PropertyType != typeof(System.String) && !value.GetType().IsAssignableFrom(pi.PropertyType))
                    throw new ArgumentException("Value Type does not match property Type.", "value");

                if (pi != null)
                    try
                    { pi.SetValue(ctrl, ((pi.PropertyType.FullName == "System.String") ? value.ToString() : value), args); }
                    catch { throw; }
            }
        }
        /// <summary>
        /// Invokes a method member on a given Control instance with proper type-safety.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object on which the method will be invoked.</param>
        /// <param name="methodName">The name of the method that will be invoked.</param>
        /// <param name="args">Any arguments the method requires.  Array objects must be properly typed to match the method's signature.</param>
        /// <returns>An object whose type will match the specified method's signature or null if the method has no return value.</returns>
        public static object InvokeMethod(Control ctrl, string methodName, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                InvokeMethodDelegate del = new InvokeMethodDelegate(CrossThreadUI.InvokeMethod);
                if (CrossThreadUI.ExecSync)
                    return ctrl.Invoke(del, ctrl, methodName, args);
                else
                    ctrl.BeginInvoke(del, ctrl, methodName, args);
            }
            else
            {
                Type ctrlType = ctrl.GetType();

                // Determine the type of each passed argument in order to try and
                //   determine the unique signature for the requested method.
                Type[] paramTypes = new Type[args.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                    paramTypes[i] = args[i].GetType();

                // Now it's time to get a reference to the method.
                MethodInfo mi = ctrlType.GetMethod(methodName, paramTypes);
                if (mi != null)
                {
                    #region DEPRECIATED :: Checking Method Parameters
                    // We don't really need to do this, since we're using the
                    //   provided method arguments as a signature when we
                    //   search for the method.

                    //// Check to make sure the proper number of parameters were passed
                    ////   and that all the types match.
                    //ParameterInfo[] p = mi.GetParameters();
                    //if (p.Length > 0)
                    //{
                    //    // If the 'args' value is null or the lengths don't match,
                    //    //   throw an exception.
                    //    if (args == null || (args.Length != p.Length))
                    //        throw new ArgumentException("Wrong number of arguments for method '" + methodName + "'.", "args");

                    //    // Check to make sure all the parameters are of the correct type.
                    //    for (int i = 0; i < p.Length; i++)
                    //    {
                    //        Type argType = args[i].GetType();
                    //        if (argType.FullName != p[i].ParameterType.FullName && !p[i].ParameterType.IsSubclassOf(argType))
                    //            throw new ArgumentException(string.Format("Given argument is of the wrong type for parameter '{0}'. Expected '{1}' but recieved '{2}' for parameter position {3}.", p[i].Name, p[i].ParameterType.FullName, argType.FullName, p[i].Position), "args");
                    //    }
                    //}
                    #endregion
                    
                    // If we passed all the validation, then call the invoke for the
                    //   MemberInfo object.
                    return mi.Invoke(ctrl, args);
                }
                else
                    throw new ArgumentException("Specified control does not expose a '" + methodName + "' method with the provided parameter types.", "methodName");
            }
            return null;
        }
        /// <summary>
        /// Gets the object instance assigned to the property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control instance whose property will be queried.</param>
        /// <param name="propertyName">The name of the property whose value will be returned.</param>
        /// <returns>An object whose type will match the specified property's signature.</returns>
        public static object GetPropertyInstance(Control ctrl, string propertyName)
        {
            if (ctrl.InvokeRequired)
            {
                GetPropertyInstanceDelegate del = new GetPropertyInstanceDelegate(CrossThreadUI.GetPropertyInstance);
                if (CrossThreadUI.ExecSync)
                    return ctrl.Invoke(del, ctrl, propertyName);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName);
            }
            else
            {
                Type ctrlType = ctrl.GetType();
                PropertyInfo pi = ctrlType.GetProperty(propertyName, CrossThreadUI.Binding);
                if (pi != null)
                {
                    return pi.GetValue(ctrl, null);
                }
                else
                    throw new ArgumentException("Specified control does not expose a '" + propertyName + "' property.");
            }
            return null;
        }
        /// <summary>
        /// Invokes a method on the specified property instance of a given control.
        /// </summary>
        /// <param name="ctrl">The control containing the property returning an object instance.</param>
        /// <param name="propertyName">The name of the property on the control.</param>
        /// <param name="methodName">
        /// The name of the method to be invoked.  This will be queried from the resulting Type of
        /// the instance object returned by the specified property.
        /// </param>
        /// <param name="args">Any arguments the method requires.  Array objects must be properly typed to match the method's signature.</param>
        /// <returns>An object whose type will match the specified method's signature or null if the method has no return value.</returns>
        public static object InvokePropertyMethod(Control ctrl, string propertyName, string methodName, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                InvokePropertyMethodDelegate del = new InvokePropertyMethodDelegate(CrossThreadUI.InvokePropertyMethod);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, propertyName, methodName, args);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName, methodName, args);
            }
            else
            {
                object propVal = CrossThreadUI.GetPropertyInstance(ctrl, propertyName);
                if (propVal != null)
                {
                    Type objType = propVal.GetType();

                    // Determine the type of each passed argument in order to try and
                    //   determine the unique signature for the requested method.
                    Type[] paramTypes = new Type[args.Length];
                    for (int i = 0; i < paramTypes.Length; i++)
                        paramTypes[i] = args[i].GetType();

                    // Now, it's time to get a reference to the method.
                    MethodInfo mi = objType.GetMethod(methodName, paramTypes);
                    if (mi != null)
                    {
                        return mi.Invoke(propVal, args);
                    }
                    else
                        throw new ArgumentException("Specified ('" + propertyName + "') property value does not expose a '" + methodName + "' method with the provided parameter types.");
                }
            }
            return null;
        }
        /// <summary>
        /// Shows a MessageBox properly linked to the given Win32Window owner.
        /// </summary>
        /// <param name="owner">The System.Windows.Forms.IWin32Window object who will act as this message box's parent.</param>
        /// <param name="msg">A System.String value to display as a message on this message box.</param>
        /// <param name="caption">A System.String value to display in the title bar of this message box.</param>
        /// <param name="btns">A System.Windows.Forms.MessageBoxButtons value specifying what buttons are displayed on the message box.</param>
        /// <param name="icon">A System.Windows.Forms.MessageBoxIcon value specifying what icon is displayed on the message box.</param>
        /// <param name="defBtn">A System.Windows.Forms.MessageBoxDefaultButton value specifying which of the message box's buttons should be activated if the user presses the "Enter" key.</param>
        public static void ShowMessageBox(IWin32Window owner, string msg, string caption, MessageBoxButtons btns, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            Type ownerType = owner.GetType();
            PropertyInfo invokeProp = ownerType.GetProperty("InvokeRequired");
            bool invokeReq = false;
            if (invokeProp != null)
                invokeReq = Convert.ToBoolean(invokeProp.GetValue(owner, null));
            if (invokeProp == null || !invokeReq)
            {
                MessageBox.Show(owner, msg, caption, btns, icon, defBtn);
            }
            else
            {
                MethodInfo beginInvokeMeth = ownerType.GetMethod(((CrossThreadUI.ExecSync) ? "Invoke" : "BeginInvoke"), new Type[] { typeof(Delegate), typeof(Object[]) });
                if (beginInvokeMeth != null)
                {
                    ShowMessageBoxDelegate del = new ShowMessageBoxDelegate(CrossThreadUI.ShowMessageBox);
                    beginInvokeMeth.Invoke(owner, new object[] { del, new object[] { owner, msg, caption, btns, icon, defBtn } });
                }
            }
        }
        #endregion
    }
}
