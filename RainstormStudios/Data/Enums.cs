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
using System.ComponentModel;
using System.Text;

namespace RainstormStudios.Data
{
    /// <summary>
    /// Specifies which ADO.NET provider to use in database operations.
    /// </summary>
    public enum AdoProviderType
    {
        /// <summary>
        /// Attempt to determine provider type automatically.
        /// </summary>
        [Description("Auto")]
        Auto = -1,
        /// <summary>
        /// Use SQL Driver.
        /// </summary>
        [Description("SQL Provider")]
        SqlProvider = 0,
        /// <summary>
        /// Use OLEDB Driver.
        /// </summary>
        [Description("OLE DB Provider")]
        OleProvider,
        /// <summary>
        /// Use IBM DB2 Driver.
        /// </summary>
        [Description("IBM DB2 Provider")]
        DB2Provider,
        /// <summary>
        /// Use ODBC Driver.
        /// </summary>
        [Description("ODBC Provider")]
        OdbcProvider
    }
    public enum DataTargetType
    {
        None = -1,
        Xml = 0,
        Text,
        Excel,
        Csv,
        Db,
        dBase
    }
    public enum QueryFieldParseOption
    {
        None = 0,
        OriginalName,
        DestinationName
    }
}
