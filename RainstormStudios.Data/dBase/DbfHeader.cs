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
using System.Text;
using RainstormStudios;
using RainstormStudios.Collections;

namespace RainstormStudios.Data.dBase
{
    public class DbfHeader
    {
        #region Class Enums
        //***************************************************************************
        // Class Enums
        // 
        public enum dBaseType
        {
            FoxBase = 0x02,
            DbfWithOutMemo = 0x03,
            dBase4WithOutMemo = 0x04,
            dBase5WithOutMemo = 0x05,
            dBase5WithMemo = 0x07,
            VisualFoxPro = 0x30,
            VisualFoxProAutoIncr = 0x31,
            DbvMemoVarSize = 0x43,
            DbfPlusWithMemo = 0x83,
            Dbf3WithMemo = 0x8B,
            dBase4WithSqlTable = 0x8E,
            DbvAndDbtMemo = 0xB3,
            ClipperSIX = 0xE5,
            FoxProWithMemo = 0xF5,
            UnknownFoxPro = 0xFB
        }
        public enum dBaseFieldType
        {
            Unknown = 0x00,
            Timestamp = 0x32,
            Binary = 0x42,
            Character = 0x43,
            Date = 0x44,
            FloatingPoint = 0x46,
            General = 0x47,
            Integer = 0x49,
            Logical = 0x4C,
            Memo = 0x4D,
            Numeric = 0x4E,
            Double = 0x4F,
            Picture = 0x50,
            DateTime = 0x54,
            VariField = 0x56,
            VariantX = 0x57,
            Currency = 0x59,
            AutoIncrement = 0xBB,
            Long = 0xDC
        }
        public enum FoxProCodePage
        {
            None = 0x00,
            DOS_USA_CP437 = 0x01,
            DOS_Multilingual_CP850 = 0x02,
            Windows_ANSI_CP1252 = 0x03,
            Standard_Macintosh = 0x04,
            EE_MSDOS_CP852 = 0x64,
            Nordic_MSDOS_CP865 = 0x65,
            Russian_MSDOS_CP866 = 0x66,
            Icelandic_MSDOS = 0x67,
            Kamenicky_Czech_MSDOS = 0x68,
            Mazovia_Polish_MSDOS = 0x69,
            Greek_MSDOS_437G = 0x6A,
            Turkish_MSDOS = 0x6B,
            Russian_Macintosh = 0x96,
            Eastern_European_Macintosh = 0x97,
            Greek_Macintosh = 0x98,
            Windows_EE_CP1250 = 0xC8,
            Russion_Windows = 0xC9,
            Turkish_Windows = 0xCA,
            Greek_Windows = 0xCB
        }
        public enum FoxProFieldFlags
        {
            SystemColumn = 0x01,
            NullValid = 0x02,
            BinaryColumn = 0x04
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _filename;
        private DbfFieldCollection
            _flds;
        private byte
            _id;
        private DateTime
            _lastUpd;
        private int
            _recCount,
            _dataOffset,
            _recSize;
        private bool
            _cdxStruct,
            _validDbf,
            _parsing = false;
        private DbfTable
            _owner;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public DbfFieldCollection Fields
        { get { return this._flds; } }
        public dBaseType TypeID
        { get { return (dBaseType)this._id; } }
        public DateTime LastUpdate
        { get { return this._lastUpd; } }
        public int RecordCount
        { get { return this._recCount; } }
        public int DataOffset
        { get { return this._dataOffset; } }
        public int RecordSize
        { get { return this._recSize; } }
        public bool CdxStructAttached
        { get { return this._cdxStruct; } }
        public bool IsValid
        { get { return this._validDbf; } }
        public DbfTable Owner
        { get { return this._owner; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbfHeader()
        {
            _flds = new DbfFieldCollection();
        }
        public DbfHeader(string dbfFile)
            : this()
        {
            this.Read(dbfFile);
        }
        protected internal DbfHeader(DbfTable owner)
        {
            this._owner = owner;
            this._owner.RecordAdded += new EventHandler(this.owner_onRecordAdded);
            this._owner.RecordDeleted += new EventHandler(this.owner_onRecordDeleted);
            this._flds = new DbfFieldCollection(this._owner);
            this._flds.Inserted += new CollectionEventHandler(this.flds_onChanged);
            this._flds.Updated += new CollectionEventHandler(this.flds_onChanged);
            this._flds.Removed += new CollectionEventHandler(this.flds_onChanged);
            this._flds.Cleared += new EventHandler(this.flds_onCleared);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Forces a complete re-read of the previous DBF file's header data.
        /// </summary>
        public void Refresh()
        {
            this.Read(this._filename);
        }
        /// <summary>
        /// Clears this object and releases its resources.
        /// </summary>
        public void Dispose()
        {
            _flds.Clear();
            _flds = null;
        }
        /// <summary>
        /// Reads the specified DBF file and parses its header data.
        /// </summary>
        /// <param name="dbfFile">A System.String value containing the filename of the DBF file to parse.</param>
        public void Read(string dbfFile)
        {
            try
            {
                this._parsing = true;
                using (FileStream fs = new FileStream(dbfFile, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // This first byte is the version indentifier.
                    //  03 - DBF with MEMO (FoxBASE+/FoxPro/dBASE III Plus/dBASE IV)
                    //  83 - DBF with MEMO (FoxBASE+/dBASE III Plus)
                    //  8B - dBASE III with MEMO
                    //  F5 - FoxPro with MEMO
                    _id = br.ReadByte();

                    // Bytes 2-4 indicate the last time the DBF was updated (YYMMDD).
                    byte[] lastUpd = br.ReadBytes(3);
                    int year = (Convert.ToInt32(lastUpd[0]) > 100) ? 2000 + (Convert.ToInt32(lastUpd[0]) - 100) : 1900 + Convert.ToInt32(lastUpd[0]);
                    this._lastUpd = new DateTime(year, Convert.ToInt32(lastUpd[1]), Convert.ToInt32(lastUpd[2]));

                    // Bytes 5-8 contain a count of the number of records in the file.
                    //   NOTE:  These values are stored in reverse order.  If the byte
                    //   values are F4 01 00 00, then there would be 1F4h, or 500 decimal,
                    //   records in the table.
                    byte[] recCount = br.ReadBytes(4);
                    Array.Reverse(recCount);
                    string hexCount = "0x" + Hex.ToHex(recCount);
                    this._recCount = Convert.ToInt32(hexCount, 16);

                    // Bytes 9-10 indicate the offset to the start of actual data.
                    //   NOTE:  These values are stored in reverse.  If bytes 9 and 10
                    //   contain 41 02, the first record would begin at 241h, or 577
                    //   decimal bytes from the beginning of the file.
                    byte[] dataOffset = br.ReadBytes(2);
                    Array.Reverse(dataOffset);
                    string hexOffset = "0x" + Hex.ToHex(dataOffset);
                    this._dataOffset = Convert.ToInt32(hexOffset, 16);

                    // Bytes 11-12 indicate the size of each record.
                    //   NOTE:  These values are also stored in reverse format.  The
                    //   number represents the sum of all field sizes plus 1, because
                    //   of the deletion flag.
                    byte[] recSize = br.ReadBytes(2);
                    Array.Reverse(recSize);
                    string hexRecSize = "0x" + Hex.ToHex(recSize);
                    this._recSize = Convert.ToInt32(hexRecSize, 16);

                    // Bytes 13-28 are not used (reserved) and are therefore skipped.
                    //br.ReadBytes(16);

                    // UPDATE 10-21-08 :: Found updated xBase specs defining values
                    //   for these 16 bytes.
                    br.ReadBytes(2);

                    // Byte 15 is the "Incomplete Transaction" flag.
                    byte tran = br.ReadByte();

                    // Byte 16 is the encryption flag.
                    byte encr = br.ReadByte();

                    // Bytes 17-20 are the "Free Record Thread (reserved for LAN only).
                    byte[] freeRec = br.ReadBytes(4);

                    // Bytes 21-28 are reserved for "Multi-User dBase" (dBase III+).
                    byte[] multiUsr = br.ReadBytes(8);

                    // Byte 29 is the compound index flag.  This flag will be 01h if a
                    //   structural CDX file is attached to the database.  Otherwise, 00h.
                    this._cdxStruct = Convert.ToBoolean(br.ReadByte());

                    // Byte 30 is the Language Codepage (only in FoxPro files).
                    byte lang = br.ReadByte();

                    // Bytes 31-32 are also not used.
                    br.ReadBytes(2);

                    // Now we parse the FieldHeaders
                    bool hitTerm = false;
                    while (!hitTerm)
                    {
                        try
                        {
                            byte[] hdr = br.ReadBytes(32);
                            if (hdr[0] != 0x0D)
                                this._flds.Add(ParseField(hdr));
                            else
                                hitTerm = true;
                        }
                        catch (EndOfStreamException)
                        {
                            hitTerm = true;
                        }
                    }
                }
                this._validDbf = true;
                this._filename = dbfFile;
            }
            catch
            { throw; }
            finally
            { this._parsing = false; }
        }
        /// <summary>
        /// Returns this DBF header in it's original raw binary data form.
        /// </summary>
        /// <returns>An Array of type System.Byte[]</returns>
        public byte[] GetDbfHeader()
        {
            try
            {
                ByteCollection bytes = new ByteCollection(32 + this._flds.Count * 32);

                // Keep the same file type ID.
                bytes.Add(this._id);

                // Then we right the last time the DBF was updated.
                byte lstUpYr = (byte)(this._lastUpd.Year - 1900),
                    lstUpMn = (byte)this._lastUpd.Month,
                    lstUpDy = (byte)this._lastUpd.Day;
                bytes.AddRange(lstUpYr, lstUpMn, lstUpDy);

                // Next comes the number of records in the table.
                byte[] recCnt = Hex.GetBytes(Hex.ToHex(this._recCount));
                Array.Reverse(recCnt);
                bytes.AddRange(recCnt);

                // Now, the offset to the actual data.  This is the header data (32 bytes), plus
                //   the field headers (each field header is 32 bytes) plus 1 byte for the
                //   header termination character.
                byte[] datOfst = Hex.GetBytes(Hex.ToHex(this._dataOffset));
                Array.Reverse(datOfst);
                bytes.AddRange(datOfst);

                // Then, the size of each record in the table.
                byte[] recSz = Hex.GetBytes(Hex.ToHex(this._recSize));
                Array.Reverse(recSz);
                bytes.AddRange(recSz);

                // Next, comes 16 "empty" bytes.
                byte[] empBts = new byte[16];
                empBts.Initialize();
                bytes.AddRange(empBts);

                // Then the CDX structure flag.
                bytes.Add(Convert.ToByte(this._cdxStruct));

                // Then three (3) more "empty" bytes.
                empBts = new byte[3];
                empBts.Initialize();
                bytes.AddRange(empBts);

                // Finally, we write the file headers.
                for (int i = 0; i < this._flds.Count; i++)
                    bytes.AddRange(this._flds[0].GetFieldHeader());

                // Don't forget the header terminator character.
                bytes.Add(Convert.ToByte("0x0D"));

                // And... we're done ;)
                return bytes.ToArray();
            }
            catch
            { throw; }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private DbfField ParseField(byte[] value)
        {
            DbfField retVal = new DbfField(
                new byte[] { value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], value[8], value[9], value[10] },
                value[11],
                new byte[] { value[12], value[13] },
                (value[11] == 0x43) ? Convert.ToInt32(Hex.ToHex(new byte[] { value[17], value[16] }), 16) : Convert.ToInt32(Hex.ToHex(value[16]), 16),
                (value[11] == 0x43) ? 0 : Convert.ToInt32(Hex.ToHex(value[17]), 16),
                this._flds.Count);
            if (this._owner != null)
                retVal.Owner = this._owner;
            return retVal;
        }
        private void RefreshRecSz()
        {
            int recLen = 0;
            for (int i = 0; i < this._flds.Count; i++)
                recLen += this._flds[0].FieldLength;
            this._recSize = recLen + 1;
            this._dataOffset = 32 + (this._flds.Count * 32) + 1;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void flds_onChanged(object sender, CollectionEventArgs e)
        {
            if (!this._parsing)
                this.RefreshRecSz();
        }
        private void flds_onCleared(object sender, EventArgs e)
        {
            this._recSize = 0;
        }
        private void owner_onRecordAdded(object sender, EventArgs e)
        {
            this._recCount++;
        }
        private void owner_onRecordDeleted(object sender, EventArgs e)
        {
            this._recCount--;
        }
        #endregion
    }
}
