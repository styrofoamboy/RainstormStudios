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
using System.Data;
using System.Data.Sql;

namespace RainstormStudios.Controls
{
    public class SqlServerComboBox : System.Windows.Forms.ComboBox
    {
        protected override void OnDropDown(EventArgs e)
        {
            if (this.Items.Count < 1 && !this.DesignMode)
            {
                try
                {
                    this.FindForm().UseWaitCursor = true;
                    using (DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources())
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataRow dr = dt.Rows[i];
                            string srvNm = dr[0].ToString();
                            string instNm = dr[1].ToString();
                            this.Items.Add(srvNm + ((!string.IsNullOrEmpty(instNm)) ? "\\" + instNm : ""));
                        }
                    }
                }
                finally
                {
                    this.FindForm().UseWaitCursor = false;
                }
            }

            base.OnDropDown(e);
        }
    }
}
