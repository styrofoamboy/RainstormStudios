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
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public class Db2ConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Db2ConnectionStringBuilder()
            : base()
        { }
        /// <summary>
        /// Initializes a new instance of the Db2ConnectionString Builder.
        /// </summary>
        /// <param name="value">An existing connection string to load into the builder.</param>
        public Db2ConnectionStringBuilder(string value)
            : this()
        {
            foreach (string keyval in value.Split(';'))
                this.Add(keyval.Split('=')[0], keyval.Split('=')[1].Trim('\"', ' ', ';'));
        }
        #endregion
    }
}
