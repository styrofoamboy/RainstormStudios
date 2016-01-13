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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    public partial class DriveListTreeView : TreeView
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FileInfo[]
            allFiles = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// A FileInfo array object containing all the files in the currently selected directory.
        /// </summary>
        public FileInfo[] Files
        {
            get { return allFiles; }
        }
        public string FullPath
        {
            get
            {
                return (this.SelectedNode != null)
                          ? ParseFullPath(this.SelectedNode.FullPath)
                          : string.Empty;
            }
        }
        public string CurrentDrive
        {
            get { return this.SelectedNode.FullPath.Substring(0, 1) + @":\"; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DriveListTreeView()
            : base()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string ParseFullPath(string pathArg)
        {
            if (pathArg.IndexOf(']') + 2 < pathArg.Length)
                return pathArg.Substring(0, 1) + @":\" + pathArg.Substring(pathArg.IndexOf(']') + 2);
            else
                return pathArg.Substring(0, 1) + @":\";
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public bool SelectPath(string pathValue)
        {
            try
            {
                string[] pcs = pathValue.Split(new string[] { this.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode nd = null;
                string ndKey = "";
                for (int i = 0; i < pcs.Length; i++)
                {
                    ndKey += pcs[i] + "|";
                    TreeNode[] ndFnd = null;
                    if (nd == null)
                        ndFnd = this.Nodes.Find(ndKey.TrimEnd('|'), false);
                    else
                        ndFnd = nd.Nodes.Find(ndKey.TrimEnd('|'), false);
                    if (ndFnd.Length < 1 || nd == ndFnd[0])
                        break;
                    nd = ndFnd[0];
                    nd.Expand();
                }
                if (nd != null)
                    this.SelectedNode = nd;
                else
                    return false;
            }
            catch
            { return false; }
            return true;
        }
        #endregion

        #region Base-Class Overrides
        //***************************************************************************
        // Base Class Overrides
        // 
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            string[] drives = Environment.GetLogicalDrives();
            this.BeginUpdate();
            this.Nodes.Clear();
            int i = 0;
            foreach (string drive in drives)
            {
                DriveInfo drvInfo = new DriveInfo(drive);
                if (drvInfo.IsReady)
                {
                    if (drvInfo.VolumeLabel != null && drvInfo.VolumeLabel != "")
                        this.Nodes.Add(new TreeNode(drive + "    [" + drvInfo.VolumeLabel + "]"));
                    else
                        this.Nodes.Add(new TreeNode(drive + "    [" + drvInfo.DriveType + "]"));
                    DirectoryInfo d = new DirectoryInfo(drive);
                    try
                    {
                        DirectoryInfo[] dirList = d.GetDirectories();
                        foreach (DirectoryInfo dir in dirList)
                        {
                            TreeNode ndDir = new TreeNode(dir.Name);
                            ndDir.Name = drive.TrimEnd('\\') + "|" + dir.Name;
                            this.Nodes[i].Nodes.Add(ndDir);
                        }
                    }
                    catch
                    {
                        this.Nodes[i].Nodes.Add(new TreeNode("Access Denied"));
                    }
                }
                else
                {
                    this.Nodes.Add(new TreeNode(drive + "    [" + drvInfo.DriveType + "]"));
                    this.Nodes[i].Nodes.Add(new TreeNode("Device Not Ready"));
                }
                this.Nodes[i].Name = drive.TrimEnd('\\');
                i++;
            }
            this.EndUpdate();
        }
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
            Cursor.Current = Cursors.WaitCursor;
            this.BeginUpdate();
            DirectoryInfo d = new DirectoryInfo(ParseFullPath(e.Node.FullPath));
            try
            {
                DirectoryInfo[] dirList = d.GetDirectories();
                int i = 0;
                foreach (DirectoryInfo dir in dirList)
                {
                    if (e.Node.Nodes[i].Nodes.Count < 1)
                    {
                        try
                        {
                            DirectoryInfo[] subDirList = dir.GetDirectories();
                            foreach (DirectoryInfo subDir in subDirList)
                            {
                                TreeNode ndDir = new TreeNode(subDir.Name);
                                ndDir.Name = e.Node.Nodes[i].Name + "|" + subDir.Name;
                                e.Node.Nodes[i].Nodes.Add(ndDir);
                            }
                        }
                        catch
                        {
                            e.Node.Nodes[i].Nodes.Add(new TreeNode("Access Denied"));
                        }
                    }
                    i++;
                }
            }
            catch { }
            this.EndUpdate();
            Cursor.Current = Cursors.Default;
        }
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DirectoryInfo d = new DirectoryInfo(ParseFullPath(e.Node.FullPath));
                allFiles = d.GetFiles();
            }
            catch
            {
                allFiles = null;
            }
            Cursor.Current = Cursors.Default;
        }
        #endregion
    }
}
