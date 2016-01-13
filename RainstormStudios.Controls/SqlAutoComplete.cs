using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios
{
    public class SqlAutoComplete
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        System.ComponentModel.IContainer
            _parent;
        System.Windows.Forms.ContextMenuStrip
            _mnuStrip;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SqlAutoComplete(System.ComponentModel.IContainer parent)
        {
            this._parent = parent;
            this._mnuStrip = new System.Windows.Forms.ContextMenuStrip(parent);
            this._mnuStrip.AutoClose = false;
            this._mnuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(_mnuStrip_ItemClicked);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 

        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void _mnuStrip_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
