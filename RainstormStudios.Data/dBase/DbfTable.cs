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
using System.Data;
using System.Collections;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Data.dBase
{
    [Author("Unfried, Michael")]
    public class DbfTable : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        string _file;
        DbfHeader _hdr;
        FileInfo _fi;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler RecordAdded;
        public event EventHandler RecordDeleted;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public DbfHeader Header
        { get { return _hdr; } }
        public DbfFieldCollection Fields
        { get { return this._hdr.Fields; } }
        public string FileName
        {
            get { return _file; }
            set { this.LoadFile(value); }
        }
        public string TableName
        { get { return _fi.Name.Substring(0, _fi.Name.Length - _fi.Extension.Length); } }
        public string MemoFile
        { get { return _fi.FullName.Substring(0, _fi.FullName.Length - _fi.Extension.Length) + ".dbt"; } }
        public string IndexFile
        { get { return _fi.FullName.Substring(0, _fi.FullName.Length - _fi.Extension.Length) + ".mdx"; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbfTable()
        {
            this._hdr = new DbfHeader(this);
        }
        public DbfTable(string file)
            : this()
        {
            this.LoadFile(file);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this._fi = null;
            this._file = string.Empty;
            this._hdr.Dispose();
        }
        public void LoadFile(string file)
        {
            this._fi = new FileInfo(file);
            if (!File.Exists(file))
                throw new FileNotFoundException("The specified file was not found.", file);
            FileInfo fi = new FileInfo(file);
            if (fi.Extension.ToUpper() != ".DBF")
                throw new ArgumentException("Only dBASE 'DBF' files are accepted as input.", "file");

            this._file = file;
            this._hdr.Read(this._file);
        }
        public string GetValue(long row, int col)
        {
            //if (this._hdr.Fields[col].FieldDataType == DbfHeader.dBaseFieldType.Memo)
            //    throw new DbfFieldException("Cannot retrieve Memo field data with with GetValue().", this._hdr.Fields[col]);

            using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                // First we move the pointer to the start of the selected row.
                this.MoveStreamToRow(row, fs);

                // Now we're ready to read the data.
                string retVal = (string)this.GetRowData(fs)[col];

                sr.Close();
                fs.Close();

                // Now, we need to "groom" the data.
                if (_hdr.Fields[col].FieldDataType == DbfHeader.dBaseFieldType.Numeric || _hdr.Fields[col].FieldDataType == DbfHeader.dBaseFieldType.Memo || _hdr.Fields[col].FieldDataType == DbfHeader.dBaseFieldType.Integer)
                    retVal = retVal.Trim();

                // And return the value.
                return retVal;
            }
        }
        public DataRow GetRow(int row)
        {
            try
            {
                using (DataTable dtSchema = DbfTable.LoadSchema(this))
                using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.Read))
                using (BinaryReader sr = new BinaryReader(fs))
                {
                    // First we move the pointer to the start of the selected row.
                    this.MoveStreamToRow(row, fs);

                    // Now we're ready to read the data, so prepare
                    //   the DataRow object to return.
                    DataRow retVal = dtSchema.NewRow();

                    // Now we actually read the data.
                    retVal.ItemArray = this.GetRowData(fs);

                    sr.Close();
                    fs.Close();

                    // And return the value.
                    return retVal;
                }
            }
            catch
            { throw; }
        }
        public DataTable GetTable()
        {
            using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.Read))
            {
                // Create a datatable to store all the data we retrieve.
                DataTable dtRet = DbfTable.LoadSchema(this);
                dtRet.TableName = this.TableName;

                // Now we read the actual data into the table.
                // First, we skip the header.
                fs.Read(new byte[_hdr.DataOffset], 0, _hdr.DataOffset);

                // Then, we start reading each row into the DataTable
                for (int t = 0; t < _hdr.RecordCount; t++)
                {
                    DataRow dr = dtRet.NewRow();
                    try
                    { dr.ItemArray = this.GetRowData(fs); }
                    catch (Exception ex)
                    {
                        dr.RowError += ((dr.RowError.Length > 0) ? "  " : "") + ex.Message;
                    }
                    // Add the row to the table.
                    dtRet.Rows.Add(dr);
                }

                // Close the data stream
                fs.Close();

                // Return the DataTable
                return dtRet;
            }
        }
        public byte[] GetMemoField(long row, int col)
        {
            // Make sure the user didn't select a field outside the bounds of the table.
            if (row < 0 || row > _hdr.RecordCount)
                throw new ArgumentOutOfRangeException("Requested row was greater than the length of the table.", "row");
            if (col < 0 || col > _hdr.Fields.Count)
                throw new ArgumentOutOfRangeException("Requested field is outside the bounds of the record.", "col");
            if (_hdr.Fields[col].FieldDataType != DbfHeader.dBaseFieldType.Memo)
                throw new ArgumentException("The selected field is not of type 'Memo'.", "col");

            // We also want to check & make sure the file exists.
            if (!File.Exists(this.MemoFile))
                throw new FileNotFoundException("The table's DBT file could not be found.", this.MemoFile);

            byte[] retData = null;
            using (FileStream fs = new FileStream(this.MemoFile, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // First thing we have to do is determine the offset of the block
                //   we want to read.
                // dBASEIV files can have custom-sized blocks, so we get that from
                //   the header first.
                int blockSize = 0;
                if (_hdr.TypeID == DbfHeader.dBaseType.Dbf3WithMemo)
                {
                    br.ReadBytes(20);
                    byte[] bl = br.ReadBytes(2);
                    Array.Reverse(bl);
                    blockSize = Convert.ToInt32(Hex.ToHex(bl), 16) * 512;
                    br.ReadBytes(490);
                }
                else
                {
                    blockSize = 512;
                    br.ReadBytes(512);
                }
                // Now we can find the actual offset.  We've already skipped the header,
                //   so we don't have to include that in the offset.
                int offset = (Convert.ToInt32(this.GetValue(row, col)) - 1) * blockSize;
                br.ReadBytes(offset);

                // The first 8 bytes of data are header info.
                // The first 4 are reserved and always FF FF 08 00.
                br.ReadBytes(4);

                // The next 4 indicate the size of the memo field
                //   with the usuall reversed bit array.
                byte[] btLen = br.ReadBytes(4);
                Array.Reverse(btLen);
                int fLen = Convert.ToInt32(Hex.ToHex(btLen), 16);

                // The stored length includes the 8 bytes for the header, so we
                //   remove those from the total data length.
                retData = br.ReadBytes(fLen - 8);

                // Now we need to replace all the 0x08 carriage return bytes with the
                //   standard Windows carriage return/line feed bytes, and make sure
                //   the byte array terminates when the 'End of File' byte (0x0A) is
                //   reached.
                ByteCollection bts = new ByteCollection();
                for (int i = 0; i < retData.Length; i++)
                    if (retData[i] == 0x0A && (i < retData.Length - 1 && retData[i + 1] == 0x0A))
                        break;
                    //else if (retData[i] == 0x08)
                    //{
                    //    bts.Add(Convert.ToByte('\r'));
                    //    bts.Add(Convert.ToByte('\n'));
                    //}
                    else
                        bts.Add(retData[i]);

                // And then we pump are 'corrected' byte array back into our variable
                //   to return.
                retData = bts.ToArray();
                //retData = new byte[bts.Count];
                //for (int i = 0; i < retData.Length; i++)
                //    retData[i] = (byte)bts[i];
            }
            return retData;
        }
        public void AppendRecord(Array values)
        {
            this.UpdateRecord(-1, values);
        }
        public bool UpdateChanges(DataTable dt)
        {
            long rowOrdinal = 0;
            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:
                        this.UpdateRecord(-1, dr.ItemArray);
                        break;
                    case DataRowState.Deleted:
                        //this.DeleteRecord(rowOrdinal);
                        break;
                    case DataRowState.Modified:
                        this.UpdateRecord(rowOrdinal, dr.ItemArray);
                        break;
                }
                rowOrdinal++;
            }

            return true;
        }
        public void UpdateRecord(long row, Array values)
        {
            using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.ReadWrite))
            {
                // If row '-1' was passed, it means insert, so move the cursor
                //   to the end of the stream.  Otherwise, move it
                //   to the selected record number.
                if (row == -1)
                    fs.Position = fs.Length;
                else
                    this.MoveStreamToRow(row, fs);

                // Write the row data using the static method.
                DbfTable.WriteDbfRecord(fs, this._hdr.Fields, values);

                // If we added a record, we have to update the header to reflect this.
                DbfTable.UpdateRecordCount(fs, this._hdr.RecordCount + 1);
                this.OnRecordAdded(EventArgs.Empty);

                // And set the last update time.
                DbfTable.SetLastUpdateTime(fs);
            }
        }
        public void UpdateValue(long row, string colName, object value)
        {
            this.UpdateValue(row, this._hdr.Fields[colName].Ordinal, value);
        }
        public void UpdateValue(long row, int col, object value)
        {
            using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.ReadWrite))
            {
                // Move to the start of the selected row.
                this.MoveStreamToRow(row, fs);

                // Move stream to selected field.
                this.MoveStreamToField(col, fs);

                // Write the data using the static method.
                DbfTable.WriteDbfField(fs, this._hdr.Fields[col], value);
            }
        }
        public void UpdateColumn(string colName, Array values)
        {
            this.UpdateColumn(this._hdr.Fields[colName].Ordinal, values);
        }
        public void UpdateColumn(int col, Array values)
        {
            using (FileStream fs = new FileStream(this._file, FileMode.Open, FileAccess.ReadWrite))
            {
                // Move to the start of the first row.
                this.MoveStreamToRow(0, fs);

                // Jump to the start of the field.
                this.MoveStreamToField(col, fs);

                // Then, write each value and then skip the size of a full
                //   record, minus the updated field length, which should
                //   put the Stream's position at the start of that same
                //   field for the next record.
                int recSize = (this._hdr.RecordSize + 1) - this._hdr.Fields[col].FieldLength;
                foreach (object obj in values)
                {
                    DbfTable.WriteDbfField(fs, this._hdr.Fields[col], obj);
                    // If the total numer of bytes read does not match the determined
                    //   number of bytes to skip for each record, it means we hit
                    //   the end of the file.
                    if (fs.Read(new byte[recSize], 0, recSize) != recSize)
                        break;
                }

                // And set the LastUpdateDate value in the header.
                DbfTable.SetLastUpdateTime(fs);
            }
        }
        public void DeleteRecord(long row)
        {
            using (FileStream srcDbf = new FileStream(this._file, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(srcDbf))
            using (FileStream fs = new FileStream(Path.ChangeExtension(this._file, ".tmpDbf"), FileMode.Create, FileAccess.Write))
            {
                // Copy the header data from
                fs.Write(br.ReadBytes(this._hdr.DataOffset - 2), 0, this._hdr.DataOffset - 2);

                // Copy all records prior to the one to be deleted.
                fs.Write(br.ReadBytes((int)((_hdr.RecordSize + 1) * row)), 0, (int)((_hdr.RecordSize + 1) * row));

                // Now, skip the row to be deleted...
                br.ReadBytes(_hdr.RecordSize);

                // ...And write the rest of the file.
                fs.Write(br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position)), 0, (int)(br.BaseStream.Length - br.BaseStream.Position));

                // Don't forget to update the record count in the header.
                DbfTable.UpdateRecordCount(fs, this._hdr.RecordCount - 1);
                this.OnRecordDeleted(EventArgs.Empty);

                // And set the last update time.
                DbfTable.SetLastUpdateTime(fs);
            }
            // Then move the 'tmpDbf' file to replace the existing DBF file.
            File.Move(this._file, Path.ChangeExtension(this._file, ".rsbak_" + DateTime.Now.ToString("yyyyMMddHHmmss")));
            File.Move(Path.ChangeExtension(this._file, ".tmpDbf"), this._file);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Protected Methods
        // 
        /// <summary>
        /// Returns an object array of data values read using the provided BinaryReader object and this instance's DbfHeader object.
        /// </summary>
        /// <param name="sr">An initialized BinaryReader object with the pointer already set to the start of the desired record.</param>
        /// <returns>An array of object values with the same number of dimensions as there are fields in this instances DbfHeader object.</returns>
        protected object[] GetRowData(Stream sr)
        {
            object[] retVal = new object[_hdr.Fields.Count];
            // The first byte of each row is the deletion flag.
            byte delFlag = (byte)sr.ReadByte();
            //if (delFlag > 0) dr.RowState = DataRowState.Deleted;
            //if (delFlag == 0x01) dr.RowError = "Row flagged for deletion.";

            // Then we parse the actual data.
            for (int i = 0; i < _hdr.Fields.Count; i++)
            {
                byte[] val = new byte[_hdr.Fields[i].FieldLength], dec = new byte[_hdr.Fields[i].DecimalLength];
                val.Initialize(); dec.Initialize();
                if (_hdr.Fields[i].FieldDataType == DbfHeader.dBaseFieldType.Numeric && _hdr.Fields[i].DecimalLength > 0)
                {
                    sr.Read(val, 0, _hdr.Fields[i].FieldLength - _hdr.Fields[i].DecimalLength - 1);
                    sr.ReadByte(); ;  // Read the decimal point
                    sr.Read(dec, 0, _hdr.Fields[i].DecimalLength);
                }
                else
                    sr.Read(val, 0, _hdr.Fields[i].FieldLength);

                try
                {
                string sVal = new String(Array.ConvertAll<byte, char>(val, new Converter<byte, char>(Convert.ToChar))).Replace('\0', ' ').Trim();
                    //string sVal = AosConvert.ConcatArray(val).Trim();
                    // Write the data to the row in the proper value format.
                    switch (_hdr.Fields[i].FieldDataType)
                    {
                        case DbfHeader.dBaseFieldType.Character:
                            retVal[i] = new String(Array.ConvertAll<byte, char>(val, new Converter<byte, char>(Convert.ToChar))).TrimEnd();
                            break;
                        case DbfHeader.dBaseFieldType.Date:
                            string sDate = new String(Array.ConvertAll<byte, char>(val, new Converter<byte, char>(Convert.ToChar))).Trim();
                            if (!string.IsNullOrEmpty(sDate) && sDate.Length == 8)
                                //retVal[i] = Convert.ToDateTime(sDate.Substring(4, 2) + "/" + sDate.Substring(6, 2) + "/" + sDate.Substring(0, 4));
                                retVal[i] = DateTime.ParseExact(sDate, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.CurrentInfo);
                            else
                                retVal[i] = DBNull.Value;
                            break;
                        case DbfHeader.dBaseFieldType.DateTime:
                            // First long is number of days since 01/01/4713 BC.
                            // Since the DateTime object won't go back farther than
                            //   Jan 01, 0001 (why would you want to?), we have to
                            //   calc from there and then subtract 4,713 years.
                            if (!string.IsNullOrEmpty(sVal))
                                retVal[i] = DateTime.MinValue
                                    .AddDays(Hex.GetLong(new byte[] { val[0], val[1], val[2], val[3] }, true)).AddYears(-4713)
                                    .AddMilliseconds(Hex.GetLong(new byte[] { val[4], val[5], val[6], val[7] }, true));
                            else
                                retVal[i] = DateTime.MinValue;
                            break;
                        case DbfHeader.dBaseFieldType.Timestamp:
                            // Timestamp is exactly the same as DateTime, except
                            //   that the data is expected to be filled automatically
                            //   when the record is written to the file.
                            if (!string.IsNullOrEmpty(sVal))
                                retVal[i] = DateTime.MinValue
                                    .AddDays(Hex.GetLong(new byte[] { val[0], val[1], val[2], val[3] }, true)).AddYears(-4713)
                                    .AddMilliseconds(Hex.GetLong(new byte[] { val[4], val[5], val[6], val[7] }, true));
                            else
                                retVal[i] = DateTime.MinValue;
                            break;
                        case DbfHeader.dBaseFieldType.Logical:
                            retVal[i] = Convert.ToBoolean(sVal);
                            break;
                        case DbfHeader.dBaseFieldType.Memo:
                            if (!string.IsNullOrEmpty(sVal))
                                retVal[i] = Convert.ToInt32(sVal);
                            break;
                        case DbfHeader.dBaseFieldType.Integer:
                            if (!string.IsNullOrEmpty(sVal))
                                retVal[i] = Hex.GetInteger(val, true);
                            break;
                        case DbfHeader.dBaseFieldType.FloatingPoint:
                            // Floating-point data type values are just stored as
                            //   text, right-aligned, padded with blanks to fill
                            //   the field width.
                            if (!string.IsNullOrEmpty(sVal))
                                retVal[i] = double.Parse(sVal.Trim());
                            break;
                        case DbfHeader.dBaseFieldType.Long:
                            // 4 bytes. Leftmost bit used to indicate
                            //   sign (0 = negative).
                            retVal[i] = Hex.GetLong(val, true);
                            break;
                        case DbfHeader.dBaseFieldType.AutoIncrement:
                            // Same as Long.
                            retVal[i] = Hex.GetLong(val, true);
                            break;
                        case DbfHeader.dBaseFieldType.Numeric:
                            if (sVal.Length > 0)
                            {
                                string sDec = (dec.Length > 0) ? new String(Array.ConvertAll<byte, char>(dec, new Converter<byte, char>(Convert.ToChar))).Replace('\0', ' ').Trim() : "";
                                if (_hdr.Fields[i].DecimalLength > 0 && sDec.Length > 0)
                                    //dr[i] = Convert.ToInt64(sVal + "." + sDec);
                                    retVal[i] = sVal + "." + sDec;
                                else
                                    //dr[i] = Convert.ToInt64(sVal);
                                    retVal[i] = sVal;
                            }
                            //else
                            //    dr[i] = DBNull.Value;
                            break;
                        case DbfHeader.dBaseFieldType.Binary:
                            // Binary fields are almost identical to "Memo" fields.
                            //   Field data contains 10 digits representing a .DBT
                            //   block number.  The number is stored as a string,
                            //   right-aligned and padded with blanks.
                            retVal[i] = sVal;
                            break;
                        case DbfHeader.dBaseFieldType.General:
                            // Also known as "OLE" field type.  General fields are
                            //   exactly the same as Binary fields.  The field type
                            //   difference is just to qualify the type of data
                            //   stored within the field.
                            retVal[i] = sVal;
                            break;
                        default:
                            retVal[i] = sVal;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new DbfFieldException(ex.Message, this._hdr.Fields[i]);
                //    retVal[i] = DBNull.Value;
                //    // If an error occures, note it in the row's error string.
                //    //dr.SetColumnError(dr.Table.Columns[i], ex.Message + ": '" + AosConvert.ConcatArray(val) + "'.");
                //    //Logger.Instance.WriteToLog(new LogMessage(LogMessage.SeverityLevel.Error, "DbfTable", "Error reading row " + t.ToString().PadLeft(Convert.ToString(_hdr.RecordCount + 1).Length, '0') + ", record '" + _hdr.Fields[i].FieldName + "': " + ex.Message.Replace('\n', ' ').Replace('\r', ' ') + " (" + AosString.GetErrMsg(ex).Substring(AosString.GetErrMsg(ex).LastIndexOf(':') + 1) + ")"));
                }
            }
            return retVal;
        }
        /// <summary>
        /// Moves the read position of the given Stream object from the beginning of a DBF file, past the header, to the start of the specified record.
        /// </summary>
        /// <param name="row">The record number at which to place the read position.</param>
        /// <param name="fs">An initialized stream object whose pointer should be moved.</param>
        protected void MoveStreamToRow(long row, Stream fs)
        {
            // Move the Stream object's read cursor past the DBF header,
            //   and then through the records to the selected row.
            //fs.Read(new byte[_hdr.DataOffset], 0, _hdr.DataOffset);
            //fs.Read(new byte[(int)((_hdr.RecordSize + 1) * row)], 0, (int)((_hdr.RecordSize + 1) * row));
            fs.Position = this._hdr.DataOffset + ((this._hdr.RecordSize) * row);
        }
        protected void MoveStreamToField(int ordinal, Stream fs)
        {
            // Skip the Deletion Flag byte.
            //fs.ReadByte();
            // Loop through the fields, moving the Stream object's pointer to the
            //   beginning of each one, until we reach the selected field.
            //for (int i = 0; i < ordinal; i++)
            //    fs.Read(new byte[this._hdr.Fields[i].FieldLength + _hdr.Fields[i].DecimalLength], 0, this._hdr.Fields[i].FieldLength + _hdr.Fields[i].DecimalLength);

            int sPos = 1;
            for (int i = 0; i < ordinal; i++)
                sPos += this._hdr.Fields[i].FieldLength + this._hdr.Fields[i].DecimalLength;
            fs.Position += sPos;
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Returns the System.Type equivalent to the supplied DbfHeader.dBaseFieldType.
        /// </summary>
        /// <param name="fld">A DbfField object with an initialized DbfHeader.dBaseFieldType value.</param>
        /// <returns>An equivalent System.Type object.</returns>
        public static Type GetSystemDataType(DbfField fld)
        {
            return DbfTable.GetSystemDataType(fld.FieldDataType);
        }
        /// <summary>
        /// Retursn the System.Type equivalent to the supplied DbfHeader.dBaseFieldType.
        /// </summary>
        /// <param name="fldType">A DbfHeader.dBaseFieldType value to convert from.</param>
        /// <returns>An equivalent System.Type object.</returns>
        public static Type GetSystemDataType(DbfHeader.dBaseFieldType fldType)
        {
            switch (fldType)
            {
                case DbfHeader.dBaseFieldType.Logical:
                    return typeof(System.Boolean);
                case DbfHeader.dBaseFieldType.Date:
                    return typeof(System.DateTime);
                case DbfHeader.dBaseFieldType.DateTime:
                    return typeof(System.DateTime);
                case DbfHeader.dBaseFieldType.Timestamp:
                    return typeof(System.DateTime);
                case DbfHeader.dBaseFieldType.Memo:
                    return typeof(System.String);
                case DbfHeader.dBaseFieldType.Numeric:
                    return typeof(System.String);
                case DbfHeader.dBaseFieldType.Character:
                    return typeof(System.String);
                case DbfHeader.dBaseFieldType.Unknown:
                    return typeof(System.Object);
                case DbfHeader.dBaseFieldType.Long:
                    return typeof(System.Int64);
                case DbfHeader.dBaseFieldType.Binary:
                    return typeof(System.Int64);
                case DbfHeader.dBaseFieldType.AutoIncrement:
                    return typeof(System.Int64);
                case DbfHeader.dBaseFieldType.Picture:
                    return typeof(System.Byte[]);
                case DbfHeader.dBaseFieldType.FloatingPoint:
                    return typeof(System.Double);
            }
            return typeof(System.String);
        }
        /// <summary>
        /// Writes a full record to a DBF file using the provided Stream object.  This method assumes the Stream object's write position is located at the position to begin writing.  The record's 'DeletionFlag' value will automatically be set to 0x00.
        /// </summary>
        /// <param name="s">An initialized Stream object of a DBF file with the cursor position set to the beginning of record to be written.</param>
        /// <param name="flds">A DbfFieldCollection detailing the structure of the DBF file being writen to.</param>
        /// <param name="values">An Array.Object value containing the values to write into the DBF record.</param>
        public static void WriteDbfRecord(Stream s, DbfFieldCollection flds, Array values)
        {
            // The first byte of every record is the deletion flag.  This method
            //   assumes the given Stream is at the beginning of the record.
            s.WriteByte(0x00);

            // For each DbfField and value pair, write the data to
            //   the given Stream object.
            for (int i = 0; i < System.Math.Min(flds.Count, values.Length); i++)
                DbfTable.WriteDbfField(s, flds[i], values.GetValue(i));
        }
        /// <summary>
        /// Writes a single field of data to a DBF file using the provided Stream object.  This method assumes the Stream object's write position is located at the position to begin writing.
        /// </summary>
        /// <param name="s">An initialized Stream object of a DBF file with the cursor position set to the beginning of the field to be written.</param>
        /// <param name="fld">A DbfField object detailing the structure of the field to be written to.</param>
        /// <param name="value">A System.Object value whose string representation will be written to the Stream.</param>
        public static void WriteDbfField(Stream s, DbfField fld, object value)
        {
            // Save the Stream's current position and the size of the field.
            long sPos = s.Position, sLen = fld.FieldLength + fld.DecimalLength;

            // Get the string value of the passed Object 'value' to be written.
            string val = string.Empty;
            if (value != null)
                val = (value.GetType().Name == "DateTime") ? ((DateTime)value).ToString("yyyyMMdd") : value.ToString();

            // Create a byte array from the string representation of 'value'.
            byte[] buffer = Encoding.ASCII.GetBytes(val.Substring(0, System.Math.Min(fld.FieldLength, val.Length)).PadRight(fld.FieldLength, ' '));

            // If the passed Stream object is a FileStream object,
            //   then lock the bytes we're about to write to.
            if (s.GetType().FullName == "System.IO.FileStream") ((FileStream)s).Lock(sPos, sLen);

            // Write the byte buffer.  If an error occurs, just throw it back
            //   to the calling code, but don't forget to unlock the stream.
            try
            {
                s.Write(buffer, 0, buffer.Length);
                DbfTable.SetLastUpdateTime(s);
            }
            catch
            { throw; }
            finally
            {
                if (s.GetType().FullName == "System.IO.FileStream") ((FileStream)s).Unlock(sPos, sLen);
                s.Position = sPos;
            }
        }
        public static DbfTable WriteDbf(DataTable dt, string filename, DbfHeader dbStruct)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                return DbfTable.WriteDbf(dt, fs, dbStruct);
        }
        public static DbfTable WriteDbf(DataTable dt, FileStream fs, DbfHeader dbStruct)
        {
            // This string contains all the system data types that map to a
            //   particular DBF data type.  This saves from crazy-long
            //   "if" statements.
            string numericType = "Int16,Int32,Int64,Float,Double,Decimal";

            // This DbfField collection stores the info about the fields
            //   in the DBF file we're creating.
            DbfFieldCollection flds = new DbfFieldCollection();

            #region Write DBF Header
            // The first byte is the version identifier.
            // Here, we're specifying dBase IV.
            fs.WriteByte((byte)DbfHeader.dBaseType.dBase4WithOutMemo);
            // The next three bytes are the date that the file
            //   was last modified.
            DateTime dtNow = DateTime.Now;
            fs.Write(new byte[] { (byte)(dtNow.Year - 1900), (byte)dtNow.Month, (byte)dtNow.Day }, 0, 3);
            // The next four bytes contain a count of the number of records
            //   in the file.  NOTE:  these values are stored in reverse order.
            byte[] recCnt = Hex.GetBytes(Hex.ToHex(dt.Rows.Count).PadLeft(8, '0'));
            Array.Reverse(recCnt);
            fs.Write(recCnt, 0,4);
            // The next two bytes indicate the offset to the start of the
            //   actual data. NOTE: these values are stored in reverse  order.
            // The base header takes 32 bytes and each field header takes an
            //   additional 32 bytes.
            byte[] hdrSize = Hex.GetBytes(Hex.ToHex(32 + (dt.Columns.Count * 32)).PadLeft(4, '0'));
            Array.Reverse(hdrSize);
            fs.Write(hdrSize,0,2);
            // The next two bytes identify the size of each record. This number
            //   represents the sum of all field lengths + 1, due to the first
            //   byte of each record being a deletion flag. NOTE: these values
            //   are stored in reverse order.
            int recLen = 0;
            foreach (DataColumn dc in dt.Columns)
                recLen += (dc.MaxLength > 0) ? dc.MaxLength : 100;
            byte[] recSize = Hex.GetBytes(Hex.ToHex(recLen + 1).PadLeft(4, '0'));
            Array.Reverse(recSize);
            fs.Write(recSize,0,2);
            // The next sixteen bytes are not used, so we just fill the
            //   space with zeros.
            byte[] nullBytes = new byte[16]; nullBytes.Initialize();
            fs.Write(nullBytes,0,16);
            // The next byte is the compound index flag. This flag should be set to
            //   0x01h if a stuctural CDX file is attached to the database.
            // At present, I don't know how to write or read these, so I will
            //   hardcode this to 0x00h.
            fs.WriteByte(0x00);
            // The next three bytes are also not used, so fill with zeros.
            nullBytes = new byte[3]; nullBytes.Initialize();
            fs.Write(nullBytes,0,3);
            #endregion

            #region Write Field Headers
            // Now we parse and write the header for each column.
            int fldOffset = 0;
            if (dbStruct != null)
            {
                flds = dbStruct.Fields;
                byte[] fldsHdr = dbStruct.Fields.GetRawHeader();
                fs.Write(fldsHdr, 0, fldsHdr.Length);
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    DbfHeader.dBaseFieldType fldType;
                    if (dc.DataType.Name == "DateTime")
                        fldType = DbfHeader.dBaseFieldType.Date;
                    else if (numericType.Contains(dc.DataType.Name))
                        fldType = DbfHeader.dBaseFieldType.Numeric;
                    else if (dc.DataType.Name == "Boolean")
                        fldType = DbfHeader.dBaseFieldType.Logical;
                    else
                        fldType = DbfHeader.dBaseFieldType.Character;

                    string fldName = dc.ColumnName.Substring(0, (dc.ColumnName.Length <= 10) ? dc.ColumnName.Length : 10);
                    //int fldSize = (dc.MaxLength > 0 || dc.DataType.Name != "String") ? dc.MaxLength : 100;
                    int fldSize = 50;
                    if (dc.DataType.Name == "DateTime")
                        fldSize = 8;
                    else if (dc.MaxLength > 0)
                        fldSize = dc.MaxLength;

                    flds.Add(new DbfField(fldName, fldType, fldOffset, fldSize, 0, dc.Ordinal));
                    fldOffset += fldSize;
                }
                byte[] fldsHdr = flds.GetRawHeader();
                fs.Write(fldsHdr, 0, fldsHdr.Length);
            }
            #endregion

            #region Write DBF Data
            foreach (DataRow dr in dt.Rows)
            {
                // The first byte of every record is the deletion flag.
                fs.WriteByte(0x00);
                // Convert each value to a byte array & add it to the buffer.
                for (int i = 0; i < dbStruct.Fields.Count; i++)
                {
                    byte[] record = new byte[0];
                    if (flds[i].FieldDataType == DbfHeader.dBaseFieldType.Date)
                    {
                        try
                        { record = Array.ConvertAll<Char, Byte>(((DateTime)dr[i]).ToString("yyyyMMdd").ToCharArray(), new Converter<Char, Byte>(Convert.ToByte)); }
                        catch (InvalidCastException)
                        { record = Array.ConvertAll<Char, Byte>("        ".ToCharArray(), new Converter<Char, Byte>(Convert.ToByte)); }
                    }
                    else if (flds[i].FieldDataType == DbfHeader.dBaseFieldType.Memo)
                    {
                        record = Array.ConvertAll<Char, Byte>("00000000".ToCharArray(), new Converter<char, byte>(Convert.ToByte));
                    }
                    else
                        record = Array.ConvertAll<Char, Byte>(dr[i].ToString().PadRight(flds[i].FieldLength, ' ').ToCharArray(), new Converter<Char, Byte>(Convert.ToByte));
                    fs.Write(record, 0, record.Length);
                }
            }
            // Write the EOF markers.
            fs.WriteByte(0x1A); fs.WriteByte(0x1A);
            #endregion

            return new DbfTable();
        }
        public static void UpdateRecordCount(Stream s, int recordCount)
        {
            s.Position = 4;
            byte[] recCnt = Hex.GetBytes(Hex.ToHex(recordCount));
            Array.Reverse(recCnt);
            s.Write(recCnt, 0, 4);
        }
        public static void SetLastUpdateTime(Stream s)
        { DbfTable.SetLastUpdateTime(s, DateTime.Now); }
        public static void SetLastUpdateTime(Stream s, DateTime value)
        {
            s.Position = 1;
            byte[] updTime = new byte[3];
            updTime[0] = (byte)(value.Year - 1900);
            updTime[1] = (byte)(value.Month);
            updTime[2] = (byte)(value.Day);
            s.Write(updTime, 0, 3);
        }
        public static DataTable LoadSchema(DbfTable dbf)
        {
            return DbfTable.LoadSchema(dbf.Header);
        }
        public static DataTable LoadSchema(DbfHeader dbf)
        {
            DataTable dtRet = new DataTable();
            // Add the DBF fields to the DataTable
            foreach (DbfField fld in dbf.Fields)
            {
                DataColumn dc = new DataColumn(fld.FieldName.Trim());
                switch (fld.FieldDataType)
                {
                    case DbfHeader.dBaseFieldType.Character:
                        dc.DataType = typeof(System.String);
                        dc.MaxLength = fld.FieldLength;
                        break;
                    case DbfHeader.dBaseFieldType.Date:
                        dc.DataType = typeof(System.DateTime);
                        break;
                    case DbfHeader.dBaseFieldType.Logical:
                        dc.DataType = typeof(System.Boolean);
                        break;
                    case DbfHeader.dBaseFieldType.Memo:
                        dc.DataType = typeof(System.Int32);
                        break;
                    case DbfHeader.dBaseFieldType.Numeric:
                        dc.DataType = typeof(System.String);
                        break;
                }
                dtRet.Columns.Add(dc);
            }
            return dtRet;
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnRecordAdded(EventArgs e)
        {
            if (this.RecordAdded != null)
                this.RecordAdded.Invoke(this, e);
        }
        protected void OnRecordDeleted(EventArgs e)
        {
            if (this.RecordDeleted != null)
                this.RecordDeleted.Invoke(this, e);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DbfFieldException : Exception
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        DbfField _fld;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public DbfTable Owner
        { get { return this._fld.Owner; } }
        public string FieldName
        { get { return this._fld.FieldName; } }
        public int Ordinal
        { get { return this._fld.Ordinal; } }
        public DbfHeader.dBaseFieldType FieldType
        { get { return this._fld.FieldDataType; } }
        public System.Type SystemType
        { get { return this._fld.FieldSystemDataType; } }
        public int Length
        { get { return this._fld.FieldLength; } }
        public int DecimalLength
        { get { return this._fld.DecimalLength; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbfFieldException(DbfField field)
            : this("An error occured retrieving data from the specified field.", field)
        { }
        public DbfFieldException(string msg, DbfField field)
            : base(msg)
        {
            this._fld = field;
        }
        #endregion
    }
}
