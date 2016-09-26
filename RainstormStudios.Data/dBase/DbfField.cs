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
using System.Collections;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Data.dBase
{
    public struct DbfField
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static DbfField
            Empty;
        public string
            FieldNameHex, FieldTypeHex, RecordOffsetHex;
        public int
            FieldLength, DecimalLength, Ordinal;
        public DbfTable
            Owner;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public string FieldName
        {
            get { return new String(Array.ConvertAll<byte, char>(Hex.GetBytes(FieldNameHex), new Converter<byte, char>(Convert.ToChar))).TrimEnd('\0'); }
            set { this.FieldNameHex = Hex.ToHex(Encoding.ASCII.GetBytes(value)); }
        }
        public DbfHeader.dBaseFieldType FieldDataType
        {
            get { return (DbfHeader.dBaseFieldType)Hex.ToByte(FieldTypeHex); }
        }
        public Type FieldSystemDataType
        {
            get { return DbfTable.GetSystemDataType(this); }
        }
        public int RecordOffset
        {
            get { return Convert.ToInt32(RecordOffsetHex, 16); }
        }
        public byte FieldTypeByte
        {
            get { return Hex.ToByte(this.FieldTypeHex); }
        }
        public byte[] FieldNameBytes
        {
            get { int i; return Hex.GetBytes(this.FieldNameHex, out i); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbfField(byte[] fldName, byte fldType, byte[] recOffset, int fldLength, int decLength, int ordinal)
        {
            this.FieldNameHex = Hex.ToHex(fldName);
            this.FieldTypeHex = Hex.ToHex(new byte[] { (!string.IsNullOrEmpty(Enum.GetName(typeof(DbfHeader.dBaseFieldType), fldType)) ? fldType : (byte)0) });
            this.RecordOffsetHex = Hex.ToHex(recOffset);
            this.FieldLength = fldLength;
            this.DecimalLength = decLength;
            this.Ordinal = ordinal;
            this.Owner = null;
        }
        public DbfField(string fldName, DbfHeader.dBaseFieldType fldType, int recOffset, int fldLength, int decLength, int ordinal)
        {
            this.FieldNameHex = Hex.ToHex(Encoding.Unicode.GetBytes(fldName));
            this.FieldTypeHex = Hex.ToHex(new byte[] { (byte)fldType });
            this.RecordOffsetHex = Hex.ToHex(recOffset).PadLeft(8, '0');
            this.FieldLength = fldLength;
            this.DecimalLength = decLength;
            this.Ordinal = ordinal;
            this.Owner = null;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public byte[] GetFieldHeader()
        {
            // Field headers are always 32 bytes.
            byte[] hdr = new byte[32];
            hdr.Initialize();

            // The first 10 bytes (0-9) are the field name.
            byte[] name = this.FieldNameBytes;
            for (int i = 0; i < 10; i++)
                hdr[i] = (i < name.Length) ? name[i] : byte.MinValue;
            // Byte 10 is the field name terminator
            hdr[10] = 0x00;
            // Byte 11 is the field type flag.
            hdr[11] = this.FieldTypeByte;
            // Bytes 12-15 are the field's offset from the beginning
            //   of the record.
            byte[] offst = Hex.GetBytes(this.RecordOffsetHex.PadLeft(8, '0'));
            Array.Reverse(offst);
            for (int i = 0; i < 4; i++)
                hdr[i + 12] = offst[i];
            // Bytes 16 and 17 are the field length and, for numberic fields,
            //   the number of decimal places respectively.
            if (this.FieldDataType == DbfHeader.dBaseFieldType.Numeric)
            {
                hdr[16] = Hex.ToByte(Hex.ToHex(this.FieldLength));
                hdr[17] = Hex.ToByte(Hex.ToHex(this.DecimalLength));
            }
            else
            {
                byte[] len = Hex.GetBytes(Hex.ToHex(this.FieldLength).PadLeft(4, '0'));
                Array.Reverse(len);
                hdr[16] = len[0];
                hdr[17] = len[1];
            }
            // Return the byte stream.
            return hdr;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        public override string ToString()
        {
            return string.Format("{0} ({1})", base.ToString(), this.FieldName);
        }
        #endregion
    }
    public class DbfFieldCollection : ObjectCollection
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        DbfTable _owner;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public new DbfField this[int index]
        {
            get { return (DbfField)List[index]; }
            set { List[index] = value; }
        }
        public new DbfField this[string key]
        {
            get
            {
                for (int i = 0; i < this._keys.Count; i++)
                    if (((string)this._keys[i]) == key)
                        return this[i];
                return DbfField.Empty;
            }
            set
            {
                for (int i = 0; i < this._keys.Count; i++)
                    if (((string)this._keys[i]) == key)
                        List[i] = value;
            }
        }
        public DbfTable Owner
        { get { return this._owner; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbfFieldCollection()
        { }
        protected internal DbfFieldCollection(DbfTable owner)
            : this()
        {
            this._owner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(DbfField value)
        {
            if (this._owner != null)
                value.Owner = this._owner;
            base.Add(value);
        }
        public byte[] GetRawHeader()
        {
            ByteCollection hdr = new ByteCollection();
            foreach (DbfField fld in this.List)
                hdr.AddRange(fld.GetFieldHeader());
            hdr.Add(0x0D);
            return hdr.ToArray();
        }
        #endregion
    }
}
