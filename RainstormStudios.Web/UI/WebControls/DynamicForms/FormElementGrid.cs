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
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    [Author("Unfried, Michael")]
    public class FormElementGrid : FormElementContainerControl
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FormElementGrid(FormElementData data)
            : base(data)
        { }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();

            writer.BeginRender();
            try
            {
                FormElementControl.AddElementStyleAttributes(writer, this._qData);
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                for (int y = 0; y < this.RowCount; y++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    for (int x = 0; x < this.ColumnCount; x++)
                    {
                        FormElementData[] cellData = this._qData.Children.Where(c => c.RowIndex == y && c.ColumnIndex == x).ToArray();

                        if (cellData.Length > 1)
                            // If we found more than one, I don't even want to try and
                            //   figure out how to handle that one right now.
                            throw new Exception(string.Format("Row {0}, Column {1} of the grid contains more than one child cell definition. Cannot render more than one cell in each row/column intersect position.", y + 1, x + 1));

                        else if (cellData.Length < 1)
                        {
                            // If no child cell is found at this intersection, just render
                            //   an empty table cell, so we don't "break" the HTML output.
                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            writer.RenderEndTag();
                        }

                        else if (cellData[0].DisplayType != FormElementDisplayType.GridCell)
                            // If the item we found isn't for a grid cell, that's a problem.
                            //   I'm not coding around that "weirdness".  At least not yet.
                            throw new Exception(string.Format("Cannot render a form element of type {0} directly below a Grid element without a parent GridCell.  Please check your data for correct parent/child hierarchy.", cellData[0].DisplayType));

                        else
                        {
                            // We've got data for a single cell, so create a control.
                            FormElementControl ctrl = FormElementControl.GetControl(cellData[0]);
                            this.Controls.Add(ctrl);
                            ctrl.RenderControl(writer);
                        }
                    }
                    writer.RenderEndTag(); // TR
                }

                writer.RenderEndTag(); // TABLE
            }
            finally
            { writer.EndRender(); }
            base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }
        #endregion
    }
}
