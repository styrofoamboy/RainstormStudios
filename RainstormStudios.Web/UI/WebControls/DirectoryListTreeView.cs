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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    public enum DirectoryListTreeViewRootType : int
    {
        VirtualPath = 0,
        PhysicalPath = 1
    }
    public class DirectoryListTreeView : System.Web.UI.WebControls.TreeView
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _rootPath,
            _folderImg,
            _folderOpenImg;
        private DirectoryListTreeViewRootType
            _rootType;
        private bool
            _showNdImg;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string RootPath
        {
            get { return this._rootPath; }
            set
            {
                this._rootPath = value;
                if (!this.DesignMode && this.Initialized)
                    this.OnInit(EventArgs.Empty);
            }
        }
        public DirectoryListTreeViewRootType RootType
        {
            get { return this._rootType; }
            set
            {
                this._rootType = value;
                if (!this.DesignMode && this.Initialized)
                    this.OnInit(EventArgs.Empty);
            }
        }
        public string SelectedFolderPath
        { get { return this.Page.Server.MapPath(this.SelectedRelativeFolderPath); } }
        public string SelectedRelativeFolderPath
        { get { return (this.SelectedNode == null) ? this._rootPath : Path.Combine(this._rootPath, this.SelectedNode.ValuePath); } }
        public string SelectedValuePath
        {
            get { return (this.SelectedNode != null) ? this.SelectedNode.ValuePath : ""; }
            set
            {
                TreeNode nd = null;
                string[] ndVals = value.Split(this.PathSeparator);
                for (int i = 0; i < ndVals.Length; i++)
                {
                    if (nd == null)
                    {
                        for (int t = 0; t < this.Nodes.Count; t++)
                            if (this.Nodes[t].Value == ndVals[i])
                            {
                                this.Nodes[t].Select();
                                this.Nodes[t].Expand();
                                nd = this.Nodes[t];
                                break;
                            }
                    }
                    else
                        for (int t = 0; t < nd.ChildNodes.Count; t++)
                            if (nd.ChildNodes[t].Value == ndVals[i])
                            {
                                nd.ChildNodes[t].Select();
                                nd.ChildNodes[t].Expand();
                                nd = nd.ChildNodes[t];
                                break;
                            }
                }
            }
        }
        public string FolderClosedImage
        {
            get
            {
                return (string.IsNullOrEmpty(this._folderImg))
                           ? "DirTreeViewImg.axd?imgNm=folder"
                           : this.ResolveUrl(this._folderImg);
            }
            set { this._folderImg = value; }
        }
        public string FolderOpenImage
        {
            get
            {
                return (string.IsNullOrEmpty(this._folderOpenImg))
                            ? "DirTreeViewImg.axd?imgNm=folderOpen"
                            : this.ResolveUrl(this._folderOpenImg);
            }
            set { this._folderOpenImg = value; }
        }
        public bool ShowNodeImages
        {
            get { return this._showNdImg; }
            set
            {
                this._showNdImg = value;
                if (!this.DesignMode && this.Initialized)
                    this.OnInit(EventArgs.Empty);
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ReInit()
        { this.InitBaseNodes(); }
        public void ClearSelectedNode()
        {
            foreach (TreeNode nd in this.Nodes)
                this.ClearSelections(nd);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void PopulateNode(string relPath, TreeNode node)
        {
            if (node != null && node.ChildNodes.Count > 0)
                return;

            string lcPath = string.Empty;
            try
            {
                lcPath = (this.RootType == DirectoryListTreeViewRootType.VirtualPath)
                                    ? this.Page.Server.MapPath(Path.Combine(this.RootPath, relPath))
                                    : Path.Combine(this.RootPath, relPath);
            }
            catch (Exception ex)
            {
                if (ex.Message.EndsWith("is a physical path, but a virtual path was expected."))
                    throw new Exception("You have specified a physical root path, but specified a virtual root path type.", ex);
                else
                    throw;
            }
            if (!Directory.Exists(lcPath))
                return;

            DirectoryInfo di = new DirectoryInfo(lcPath);
            foreach (DirectoryInfo diSub in di.GetDirectories())
            {
                if ((diSub.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (diSub.Attributes & FileAttributes.System) != FileAttributes.System)
                {
                    TreeNode ndSub = new TreeNode();
                    ndSub.Text = diSub.Name;
                    ndSub.Value = diSub.Name;
                    ndSub.SelectAction = TreeNodeSelectAction.SelectExpand;
                    ndSub.PopulateOnDemand = true;
                    ndSub.Expanded = false;
                    if (this.ShowNodeImages)
                        ndSub.ImageUrl = this.FolderClosedImage;

                    if (node == null)
                        this.Nodes.Add(ndSub);
                    else
                        node.ChildNodes.Add(ndSub);
                }
            }
        }
        private void ClearSelections(TreeNode nd)
        {
            nd.Selected = false;
            if (nd.ChildNodes.Count > 0)
                foreach (TreeNode ndSub in nd.ChildNodes)
                    this.ClearSelections(ndSub);
        }
        private void InitBaseNodes()
        {
            this.Nodes.Clear();
            if (string.IsNullOrEmpty(this._rootPath))
                throw new Exception("You must specify the control's root path.");
            string stPath = this._rootPath;

            // Create Root Node
            TreeNode nd = new TreeNode();
            nd.Text = stPath;
            nd.Value = stPath;
            nd.SelectAction = TreeNodeSelectAction.SelectExpand;
            if (this.ShowNodeImages)
                nd.ImageUrl = this.FolderOpenImage;
            nd.Expanded = true;

            this.PopulateNode(stPath, nd);

            this.Nodes.Add(nd);
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DirectoryListTreeView()
        {
            this.CollapseImageUrl = this.FolderOpenImage;
            this.ExpandImageUrl = this.FolderClosedImage;
            this.NoExpandImageUrl = this.FolderClosedImage;
            this.ShowLines = false;
            this.PopulateNodesFromClient = false;
            this.NodeStyle.HorizontalPadding = new Unit(4, UnitType.Pixel);
            this.NodeStyle.VerticalPadding = new Unit(2, UnitType.Pixel);
            this.SelectedNodeStyle.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.ShowNodeImages = true;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Override Handlers
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.InitBaseNodes();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //if (this.Nodes.Count < 1 && !string.IsNullOrEmpty(this._rootPath))
            //    this.InitBaseNodes();
        }
        protected override void OnSelectedNodeChanged(EventArgs e)
        {
            TreeNode nd = this.SelectedNode;
            while (nd.Parent != null)
            {
                nd.Parent.Expand();
                nd = nd.Parent;
            }
            base.OnSelectedNodeChanged(e);
        }
        protected override void OnTreeNodePopulate(System.Web.UI.WebControls.TreeNodeEventArgs e)
        {
            if (e.Node.ChildNodes.Count > 0)
                return;

            this.PopulateNode(e.Node.ValuePath, e.Node);

            if (e.Node.ChildNodes.Count > 0 && this.ShowNodeImages)
                e.Node.ImageUrl = this.FolderOpenImage;

            base.OnTreeNodePopulate(e);
        }
        protected override void OnTreeNodeCollapsed(TreeNodeEventArgs e)
        {
            base.OnTreeNodeCollapsed(e);
            if (this.ShowNodeImages)
                e.Node.ImageUrl = this.FolderClosedImage;
        }
        #endregion
    }
}
