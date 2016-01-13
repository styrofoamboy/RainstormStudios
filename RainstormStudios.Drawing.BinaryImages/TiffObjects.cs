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
using System.Drawing;
using System.Collections;
using System.Text;
using RainstormStudios;
using RainstormStudios.Drawing;
using RainstormStudios.Collections;

namespace RainstormStudios.Drawing.BinaryImages
{
    [Author("Unfried, Michael")]
    public class TiffTag
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private TiffIdf
            _owner;
        private byte[]
            _raw;
        //***************************************************************************
        // Internal Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TiffTagType TagID
        {
            get
            {
                int tagNum = this.TagIDNumber;
                return (tagNum > 32768)
                    ? TiffTagType.Custom
                    : (TiffTagType)tagNum;
            }
        }
        public int TagIDNumber
        {
            get
            {
                byte[] fldType = new byte[] { this._raw[0], this._raw[1] };
                if (this._owner.Owner.LittleEndian)
                    Array.Reverse(fldType);
                return Hex.GetInteger(fldType);
            }
        }
        public TiffFieldType FieldType
        {
            get
            {
                byte[] fldType = new byte[] { this._raw[2], this._raw[3] };
                if (this._owner.Owner.LittleEndian)
                    Array.Reverse(fldType);
                return (TiffFieldType)Hex.GetInteger(fldType);
            }
        }
        public int FieldCount
        {
            get
            {
                byte[] valCount = new byte[] { this._raw[4], this._raw[5], this._raw[6], this._raw[7] };
                if (this._owner.Owner.LittleEndian)
                    Array.Reverse(valCount);
                return Hex.GetInteger(valCount);
            }
        }
        public int Value
        {
            get
            {
                byte[] fldVal = new byte[] { this._raw[8], this._raw[9], this._raw[10], this._raw[11] };
                if (this._owner.Owner.LittleEndian)
                    Array.Reverse(fldVal);
                return Hex.GetInteger(fldVal);
            }
        }
        public bool ValueIsPointer
        { get { return TiffTag.MustBePointer(this.FieldType, this.FieldCount); } }
        public TiffIdf Owner
        { get { return this._owner; } }
        public byte[] RawData
        { get { return this._raw; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal TiffTag(TiffIdf owner, byte[] data)
        {
            this._owner = owner;
            this._raw = data;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public byte[] GetValue()
        {
            ByteCollection retVal = new ByteCollection();

            if (this.ValueIsPointer && this._owner.Owner.GetType().Name == typeof(TiffReader).Name)
            {
                // The tag value contains a pointer to a byte address in the
                //   original file where the literal data is stored.
                #region Read data from file
                FileStream fs = null;
                BinaryReader br = null;

                try
                {
                    // Get a stream and binary reader for the file this tag
                    //   belongs to.
                    fs = new FileStream(((TiffReader)this._owner.Owner).LoadFile, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);

                    // The Value property will return an integer value containing
                    //   the byte offset of the actual data in the file, so the
                    //   first thing we need to do is jump to that byte address.
                    br.BaseStream.Seek(this.Value, SeekOrigin.Begin);

                    // Now the read cursor's in the right spot, so we just have to
                    //   read however many fields of whatever byte-length.
                    for (int i = 0; i < this.FieldCount; i++)
                    {
                        byte[] val;
                        switch (this.FieldType)
                        {
                            case TiffFieldType.ByteType:
                                val = br.ReadBytes(1);
                                break;
                            case TiffFieldType.AsciiType:
                                val = br.ReadBytes(1);
                                break;
                            case TiffFieldType.ShortType:
                                val = br.ReadBytes(2);
                                break;
                            case TiffFieldType.LongType:
                                val = br.ReadBytes(4);
                                break;
                            case TiffFieldType.Rational:
                                // The Rational field type takes a little extra work,
                                //   since it's actually two different long values
                                //   per field entry. The first value represents the
                                //   Numerator, while the second value is the
                                //   Denominator.
                                // NOTE: These values are always in sequencial byte-
                                //   order, so we don't use Array.Reverse().
                                byte[] num = br.ReadBytes(4);
                                byte[] den = br.ReadBytes(4);

                                // We have to initialized the 'val' variable to an
                                //   empty array before we can use the
                                //   Array.Copy() function on it.
                                val = new byte[8];

                                // Now, we just copy the two byte arrays into the
                                //   output array.
                                Array.Copy(num, 0, val, 0, 4);
                                Array.Copy(den, 0, val, 4, 4);
                                break;
                            default:
                                // If we somehow have a different value for the
                                //   Fieldtype (which *shouldn't* be possible),
                                //   populate the 'val' variable with an empty
                                //   array to ensure we don't needlessly throw a
                                //   NullReference exception when we try to check
                                //   the length of the array.
                                val = new byte[0];
                                break;
                        } // End Switch(this.FieldType)

                        // Don't forget to swap the byte orders if we're working with
                        //   a little-endian file (which will be 90% of the time).
                        // NOTE: We don't want to swap the byte-order of Rational
                        //   field types.
                        if (val.Length > 1 && this.FieldType != TiffFieldType.Rational && this._owner.Owner.LittleEndian)
                            Array.Reverse(val);

                        // And, finally, add the value to the collection.
                        if (val.Length > 0)
                            retVal.AddRange(val);

                    } // Next i
                } // End Try
                catch
                {
                    // Just throw the exception back up the call stack so that the
                    //   calling function can deal with it.
                    throw;
                }
                finally
                {
                    // Make sure we release the BinaryReader and FileStream objects.
                    if (br != null)
                        br.Close();
                    if (fs != null)
                        fs.Dispose();
                }
                #endregion
            }
            else if (this.ValueIsPointer)
            {
                // This tag is part of an IDF belonging to a TiffWriter object, so
                //   we need to look for the real data in the TiffWriter's
                //   _metaData field.
                int metaOffset = this.Value;
                for (int i = 0; i < this.FieldCount; i++)
                {
                    // We still need to be concious of what type of data we're
                    //   getting from the binary stream.
                    switch (this.FieldType)
                    {
                        case TiffFieldType.AsciiType:
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + i]);
                            break;
                        case TiffFieldType.ByteType:
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + i]);
                            break;
                        case TiffFieldType.ShortType:
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 2)]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 2) + 1]);
                            break;
                        case TiffFieldType.LongType:
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 4)]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 4) + 1]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 4) + 2]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 4) + 3]);
                            break;
                        case TiffFieldType.Rational:
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8)]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 1]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 2]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 3]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 4]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 5]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 6]);
                            retVal.Add(((TiffWriter)this._owner.Owner)._metaData[metaOffset + (i * 8) + 7]);
                            break;
                    }
                }
            }
            else
            {
                // The tag value contains the literal data, not a pointer, so we
                //   just need to pull the data straight from the binary data
                //   we're already holding.
                #region Read data from tag
                if (this.FieldType == TiffFieldType.AsciiType || this.FieldType == TiffFieldType.ByteType)
                {
                    // We're just retrieving single byte values, so just dump them
                    //   into the ByteCollection in order.
                    for (int i = 0; i < this.FieldCount; i++)
                        retVal.Add(this._raw[i + 8]);
                }
                else if (this.FieldType == TiffFieldType.ShortType)
                {
                    // Here, we read either one or two 2-byte integer values.
                    for (int i = 0; i < this.FieldCount; i++)
                    {
                        byte[] val = new byte[] { this._raw[(i * 2) + 8], this._raw[(i * 2) + 9] };
                        if (this._owner.Owner.LittleEndian)
                            Array.Reverse(val);
                        retVal.AddRange(val);
                    }
                }
                else
                {
                    // If we hit here, we must be pulling a Long value, which means
                    //   we there can only be a single field value.
                    byte[] val = new byte[4];
                    for (int i = 0; i < val.Length; i++)
                        val[i] = this._raw[i + 8];
                    if (this._owner.Owner.LittleEndian)
                        Array.Reverse(val);
                    retVal.AddRange(val);
                }
                #endregion
            }

            return retVal.ToArray();
        }
        #endregion

        #region Non-Public Methods
        //***************************************************************************
        // Internal Methods
        // 
        internal void SetOwner(TiffIdf owner)
        { this._owner = owner; }
        //internal void SetPointer(int addr)
        //{ Array.Copy(Hex.GetBinary((long)addr, this._owner.Owner.LittleEndian), 0, this._raw, 8, 4); }
        //internal void SetValue(byte[] val)
        //{
        //    if (val.Length > 4)
        //    {
        //        this._meta = val;
        //        this.SetPointer(0);
        //    }
        //    else
        //    {
        //        byte[] newVal = new byte[4];
        //        if (val.Length < 4)
        //        {
        //            for (int i = 0; i < newVal.Length; i++)
        //                newVal[i] = ((Math.Abs(i - 4) > val.Length) ? byte.Parse("00") : val[i]);
        //        }
        //        else
        //            Array.Copy(val, newVal, 4);

        //        Array.Copy(newVal, 0, this._raw, 8, 4);
        //    }
        //}
        //***************************************************************************
        // Static Methods
        // 
        internal static bool MustBePointer(TiffFieldType fldType, int fldCount)
        {
            return !((fldType == TiffFieldType.LongType && fldCount < 2)
                || (fldType == TiffFieldType.ShortType && fldCount < 3)
                || ((fldType == TiffFieldType.ByteType || fldType == TiffFieldType.AsciiType) && fldCount < 5));
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class TiffTagCollection : ObjectCollectionBase<TiffTag>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        //
        internal TiffTagCollection()
            : base() { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(TiffTag value)
        { return base.Add(value, ""); }
        public void Add(TiffTag value, string key)
        { base.Add(value, key); }
        public string Insert(int index, TiffTag value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, TiffTag value, string key)
        { base.Insert(index, value, key); }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class TiffIdf
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private TiffStream 
            _owner;
        private TiffTagCollection
            _tags;
        //***************************************************************************
        // Internal Fields
        // 
        internal byte[]
            _img;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TiffStream Owner
        { get { return this._owner; } }
        public TiffTagCollection Tags
        { get { return this._tags; } }
        public int StripCount
        {
            get
            {
                int imgHeight = (int)this.GetTagValues(TiffTagType.ImageHeight)[0];
                int rowsPerStrip = (int)this.GetTagValues(TiffTagType.RowsPerStrip)[0];
                return (imgHeight + rowsPerStrip - 1) / rowsPerStrip;
            }
        }
        public byte[] ImageData
        { get { return this.GetImageData(); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private TiffIdf()
        { this._tags = new TiffTagCollection(); }
        internal TiffIdf(TiffStream owner)
            : this()
        { this._owner = owner; }
        internal TiffIdf(TiffStream owner, byte[] data)
            : this(owner)
        {
            try
            {
                this.GetTags(data);
            }
            catch
            {
                // Just throw any errors back up the call stack so the calling
                //   app can deal with them.
                throw;
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public bool HasTag(TiffTagType tagType)
        { return this._tags.ContainsKey(tagType.ToString()); }
        public TiffTag GetTag(TiffTagType tagType)
        {
            if (!this.HasTag(tagType))
                return null;
            else
                return this._tags[tagType.ToString()];
        }
        public byte[] GetTagData(TiffTagType tagType)
        {
            if (!this.HasTag(tagType))
                return new byte[0];
            else
                return this.GetTag(tagType).GetValue();
        }
        public object[] GetTagValues(TiffTagType tagType)
        {
            if (!this.HasTag(tagType))
                return null;

            // First we have to retrieve the requested tag.
            TiffTag tag = this.GetTag(tagType);
            byte[] raw = tag.GetValue();

            // Prepare a collection to hold the return values.
            ObjectCollection retVal = new ObjectCollection();

            // Next, we have to determine what kind of data we're dealing with.
            switch (tag.FieldType)
            {
                case TiffFieldType.AsciiType:
                    string val = System.Text.ASCIIEncoding.ASCII.GetString(raw);
                    retVal.Add(val);
                    break;
                case TiffFieldType.ByteType:
                    retVal.AddRange(raw);
                    break;
                case TiffFieldType.LongType:
                    for (int i = 0; i < (raw.Length / 4); i++)
                        retVal.Add(Hex.GetInteger(
                            new byte[] {
                                    raw[i * 4],
                                    raw[(i * 4) + 1],
                                    raw[(i * 4) + 2],
                                    raw[(i * 4) + 3] }));
                    break;
                case TiffFieldType.ShortType:
                    for (int i = 0; i < (raw.Length / 2); i++)
                        retVal.Add(Hex.GetInteger(
                            new byte[] {
                                    raw[i * 2],
                                    raw[(i * 2) + 1] }));
                    break;
                case TiffFieldType.Rational:
                    for (int i = 0; i < (raw.Length / 8); i++)
                    {
                        int num = Hex.GetInteger(new byte[] { raw[i * 8], raw[(i * 8) + 1], raw[(i * 8) + 2], raw[(i * 8) + 3] });
                        int den = Hex.GetInteger(new byte[] { raw[(i * 8) + 4], raw[(i * 8) + 5], raw[(i * 8) + 6], raw[(i * 8) + 7] });
                        retVal.Add(num / den);
                    }
                    break;
            }

            // And, finally, return the value(s).
            return retVal.ToArray();
        }
        public byte[] GetStripData(int stripNum)
        {
            byte[] btStrm = null;

            // Now we're ready to read the physical data.
            if (this._owner.GetType().Name == typeof(TiffReader).Name)
            {
                // First, we need to determine the size of this strip. This should
                //   be stored in the StripByteCount tag.
                int stripSz = 0;
                int[] stripSizes = Array.ConvertAll<object, int>(this.GetTagValues(TiffTagType.StripByteCounts), new Converter<object, int>(Convert.ToInt32));
                if (stripNum - 1 < 0 || stripNum - 1 > stripSizes.Length - 1)
                    throw new Exception("The TIFF file does not contain a byte count for this strip.");
                else
                    stripSz = stripSizes[stripNum - 1];

                // Now that we have the strip size, it's time to get the offset.
                int stripOffset = 0;
                int[] stripByteOffsets = Array.ConvertAll<object, int>(this.GetTagValues(TiffTagType.StripOffsets), new Converter<object, int>(Convert.ToInt32));
                if (stripNum - 1 < 0 || stripNum - 1 > stripByteOffsets.Length - 1)
                    throw new Exception("The TIFF file does not contain a byte offset for this strip.");
                else
                    stripOffset = stripByteOffsets[stripNum - 1];

                FileStream fs = null;
                BinaryReader br = null;
                try
                {
                    if (this._owner.GetType().Name != typeof(TiffReader).Name)
                        throw new Exception("This method can only be called with associated with a TiffReader object.");

                    fs = new FileStream(((TiffReader)this._owner).LoadFile, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);
                    br.BaseStream.Seek(stripOffset, SeekOrigin.Begin);
                    btStrm = br.ReadBytes(stripSz);
                }
                catch (Exception ex)
                { throw new Exception("Unable to read binary data from TIFF file.", ex); }
                finally
                {
                    // Don't forget to release the BinaryReader and FileStream.
                    if (br != null)
                        br.Close();
                    if (fs != null)
                        fs.Dispose();
                }
            }
            else
            {
                // This IDF is attached to a TifWriter object, so we need to pull the
                //   binary image data from a Bitmap stored by the owner.
                Bitmap tmp = ((TiffWriter)this._owner)._images[this._owner._idfs.IndexOf(this)];

                // We also need to determine which row to start reading from, based
                //   on the RowsPerStipTag.

            }
            return btStrm;
        }
        public byte[] GetImageData()
        {
            // Before we can create the array that will hold our image data, we first
            //   have to determine how many bytes there are in all the strips.
            int imgLen = 0;
            int[] stripSzs = Array.ConvertAll<object, int>(this.GetTagValues(TiffTagType.StripByteCounts), new Converter<object, int>(Convert.ToInt32));
            foreach (int byteCnt in stripSzs)
                imgLen += byteCnt;

            // Now we can create the array.
            byte[] strm = new byte[imgLen];

            // Now, all we've gotta do is get each strip in the image using the
            //   GetStripData(n) function and let it do all the real work.
            int strmOffset = 0;
            for (int i = 1; i <= this.StripCount; i++)
            {
                // Prepare an array to hold the data for this strip.
                byte[] stripBytes = null;

                try
                {
                    // First, get a strip of image data.
                    stripBytes = this.GetStripData(i);
                }
                catch
                {
                    // If an error is thrown, it means the requested strip
                    //   does not have an entry in the byte count or offset lists.
                    // This appears to be something that some writers do often,
                    //   so just ignore it.
                    stripBytes = new byte[0];
                }

                // Then, we copy the data into the return array.
                Array.Copy(stripBytes, 0, strm, strmOffset, stripBytes.Length);

                // Don't forget to advance the array offset.
                strmOffset += stripBytes.Length;
            }
            this._img = strm;
            return strm;
        }
        public Bitmap GetImage()
        {
            if (this._owner.GetType().Name == typeof(TiffReader).Name)
                return (Bitmap)Bitmap.FromFile(((TiffReader)this._owner).LoadFile);
            else
                return null;

            //int w = (int)((this.HasTag(TiffTagType.ImageWidth)) ?
            //    this.GetTagValues(TiffTagType.ImageWidth)[0] : 0);
            //int h = (int)((this.HasTag(TiffTagType.ImageHeight))
            //    ? this.GetTagValues(TiffTagType.ImageHeight)[0] : 0);
            //int bps = (int)((this.HasTag(TiffTagType.BitsPerSample))
            //    ? this.GetTagValues(TiffTagType.BitsPerSample)[0] : 1);
            //int spp = (int)((this.HasTag(TiffTagType.SamplesPerPixel))
            //    ? this.GetTagValues(TiffTagType.SamplesPerPixel)[0] : 1);
            //int plnr = (int)((this.HasTag(TiffTagType.PlanarConfiguration))
            //    ? this.GetTagValues(TiffTagType.PlanarConfiguration)[0] : 1);
            //byte[] imgData = this.GetImageData();
            //using (Bitmap img = new Bitmap(w, h))
            //{
            //    for (int y = 0; y < h; y++)
            //        for (int x = 0; x < w; x++)
            //        {
            //            Color pClr = Color.FromArgb(
            //                    (int)imgData[y * (x * spp)],
            //                    (int)imgData[(y * (x * spp)) + ((spp > 1) ? 1 : 0)],
            //                    (int)imgData[(y * (x * spp)) + ((spp > 1) ? 2 : 0)]);
            //            img.SetPixel(x, y, pClr);
            //        }
            //    return (Bitmap)img.Clone();
            //}
        }
        #endregion

        #region Non-Public Methods
        //***************************************************************************
        // Private Methods
        // 
        private void GetTags(byte[] data)
        {
            // The first thing we need to do is try and validate the binary stream.
            //   It *should* contain N sets of 12-byte lengths, so it the length
            //   of the data is not a multiple of 12, it's no good.
            if (data.Length % 12 != 0)
                throw new ArgumentException("The provided binary data does not appear to be valid. Binary stream must consist of N sets of 12-byte lengths.", "data");

            // If the data is at least the correct length to be valid, determine the
            //   total number of tags in the stream.
            int tagCnt = data.Length / 12;

            // And, finally, start dumping the stream into TiffTag objects 12 bytes
            //   at a time.
            for (int i = 0; i < tagCnt; i++)
            {
                byte[] val = new byte[12];
                Array.Copy(data, i * 12, val, 0, 12);
                TiffTag newTag = new TiffTag(this, val);

                // We'll store the tags in the collection using the readable name of
                //   the tag as the key. This will make accessing individual tags
                //   later much easier.
                this._tags.Add(newTag, newTag.TagID.ToString());
            }
        }
        //***************************************************************************
        // Internal Methods
        // 
        internal void SetOwner(TiffStream owner)
        { this._owner = owner; }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class TiffIdfCollection : ObjectCollectionBase<TiffIdf>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal TiffIdfCollection()
            : base() { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(TiffIdf value)
        { return base.Add(value, ""); }
        public void Add(TiffIdf value, string key)
        { base.Add(value, key); }
        public string Insert(int index, TiffIdf value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, TiffIdf value, string key)
        { base.Insert(index, value, key); }
        #endregion
    }
    [Author("Unfried, Michael")]
    public abstract class TiffStream
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        protected bool
            _littleEndian;
        //***************************************************************************
        // Internal Fields
        // 
        internal TiffIdfCollection
            _idfs;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public byte[] Header
        {
            get
            {
                return new byte[] { 
            (this._littleEndian) ? byte.Parse("49",System.Globalization.NumberStyles.HexNumber) : byte.Parse("4D",System.Globalization.NumberStyles.HexNumber), 
            (this._littleEndian) ? byte.Parse("49",System.Globalization.NumberStyles.HexNumber) : byte.Parse("4D",System.Globalization.NumberStyles.HexNumber),
            (this._littleEndian) ? byte.Parse("2A",System.Globalization.NumberStyles.HexNumber) : byte.Parse("00",System.Globalization.NumberStyles.HexNumber),
            (this._littleEndian) ? byte.Parse("00",System.Globalization.NumberStyles.HexNumber) : byte.Parse("2A",System.Globalization.NumberStyles.HexNumber) };
            }
        }
        public bool LittleEndian
        { get { return true; } }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class TiffReader : TiffStream
    {
        #region Global Objects
        //***************************************************************************
        // Internal Fields
        // 
        internal string
            LoadFile = string.Empty;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TiffIdfCollection IDFs
        { get { return this._idfs; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public TiffReader()
        {
            this._idfs = new TiffIdfCollection();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        //public void ReadFile(string filename)
        //{
        //    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        //        this.ReadFile(fs);
        //}
        public void ReadFile(string filename)
        {
            this.LoadFile = filename;
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

                // The first four bytes are mostly irrelevant. Byte offsets 0 & 1
                //  *should* always be the same and can contain one of two values:
                //      49 49 = _II_ = Little-Endian byte format
                //      4D 4D = _MM_ = Big-Endian byte format
                // Byte offsets 2-3 will *always* contain the value 2A 00 (assuming
                //   little-endian), which is the Short integer value 42. This is
                //   the "version" number of the file intended only to identify
                //   the file as a TIFF image. According to the Aldus format spec,
                //   this number was chosen for its deep philosophical significance.
                byte[] header = br.ReadBytes(4);
                this._littleEndian = (header[0] == 0x49);
                int verNum = Hex.GetInteger(new byte[] { header[2], header[3] }, this._littleEndian);
                if (verNum != 42)
                    throw new Exception("File does not appear to be a valid TIFF image.");

                // The next four bytes contain the byte offset of the first IDF
                //   (Image Data Folder) in the file.
                int idfOffset = Hex.GetInteger(br.ReadBytes(4), this._littleEndian);

                // A single file can contain multiple IDF's, so we have to keep
                //   reading until we encounter a null pointer ('00 00 00 00')
                //   for the next IDF offset.
                bool eofIDF = false;
                while (!eofIDF)
                {
                    // Seek to the next IDF position.
                    if (!br.BaseStream.CanSeek)
                        throw new Exception("Unable to read data from stream. Stream cannot seek.");
                    br.BaseStream.Seek(idfOffset, SeekOrigin.Begin);

                    // Once we jump to that offset, we find a Short value specifying
                    //   the number of tags in this IDF.
                    int tagCnt = Hex.GetInteger(br.ReadBytes(2), this._littleEndian);

                    // Now we're ready to start grabbing the IDF data stream, which
                    //   consists of 'tagCnt' sets of 12 bytes.
                    ByteCollection idfStrm = new ByteCollection();
                    for (int i = 0; i < tagCnt; i++)
                        idfStrm.AddRange(br.ReadBytes(12));

                    // Once we've gotten the data stream, create a new TiffIdf
                    //   object with it.
                    this._idfs.Add(new TiffIdf(this, idfStrm.ToArray()));

                    // After we've read the IDF's tag data, the next four bytes
                    //   contain a Long value containing a pointer to the next
                    //   IDF in the file ('0' if no more IDF's exist).
                    idfOffset = Hex.GetInteger(br.ReadBytes(4), this._littleEndian);
                    eofIDF = (idfOffset == 0);
                }
            }
            catch (Exception ex)
            { throw new Exception("Unable to load binary TIFF file.", ex); }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Dispose();
            }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class TiffWriter : TiffStream
    {
        #region Global Objects
        //***************************************************************************
        // Internal Fields
        // 
        internal ByteCollection
            _metaData;
        internal BitmapCollection
            _images;
        #endregion

        #region Properties
        //***************************************************************************
        // Internal Properties
        // 
        internal TiffIdfCollection Idfs
        { get { return this._idfs; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public TiffWriter()
        {
            this._idfs = new TiffIdfCollection();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public TiffIdf AddIdf(int width, int height)
        {
            TiffIdf idf = new TiffIdf(this);
            this.SetTag(idf, TiffTagType.ImageWidth, width);
            this.SetTag(idf, TiffTagType.ImageHeight, height);
            this.SetTag(idf, TiffTagType.RowsPerStrip, height);
            this._images.Add(new Bitmap(width, height));
            return idf;
        }
        public void SetTag(TiffIdf idf, TiffTagType tagType, params object[] value)
        { this.SetTag(this._idfs.IndexOf(idf), tagType, value); }
        public void SetTag(int idfNum, TiffTagType tagType, params object[] value)
        {
            if (tagType == TiffTagType.StripOffsets || tagType == TiffTagType.StripByteCounts)
                throw new Exception("The 'StripOffsets' and 'StripByteCounts' tags will be constructed automatically and may not be manually configured.");

            ByteCollection tagBts = new ByteCollection();
            tagBts.AddRange(Hex.GetBinary((short)tagType, this._littleEndian));
            TiffFieldType fldType = (TiffFieldType)Enum.Parse(typeof(TiffTagIDFieldType), tagType.ToString());
            tagBts.AddRange(Hex.GetBinary((short)fldType, this._littleEndian));
            tagBts.AddRange(Hex.GetBinary(value.Length, this._littleEndian));
            if (TiffTag.MustBePointer(fldType, value.Length) || fldType == TiffFieldType.AsciiType)
            {
                // We'll have to add the data to the _metaData ByteCollection,
                //   since it won't fit into the tag itself.  We'll also need
                //   to make a pointer to the data in the tag.
                int metaPointer = this._metaData.Count;
                ByteCollection tmpMetaData = new ByteCollection();

                // We need to process each passed value.  Often there may be only
                //   one, but we still have to be prepared for multiple values.
                for (int i = 0; i < value.Length; i++)
                {
                    try
                    {
                        // First, we need to figure out what kind of data we're deeling with,
                        //   since it will determine what we do with it.
                        switch (fldType)
                        {
                            case TiffFieldType.AsciiType:
                                // ASCII type is easy... we just convert the text to ASCII
                                //   bytes and dump them to the metaData collection.
                                tmpMetaData.AddRange(System.Text.ASCIIEncoding.ASCII.GetBytes(value[i].ToString()));
                                break;
                            case TiffFieldType.ByteType:
                                tmpMetaData.Add((byte)value[i]);
                                break;
                            case TiffFieldType.ShortType:
                                tmpMetaData.AddRange(Hex.GetBinary((short)value[i], this._littleEndian));
                                break;
                            case TiffFieldType.LongType:
                                tmpMetaData.AddRange(Hex.GetBinary((int)value[i], this._littleEndian));
                                break;
                            case TiffFieldType.Rational:
                                string[] vals = ((string)value[i]).Split('/', '|');
                                tmpMetaData.AddRange(Hex.GetBinary(int.Parse(vals[0])));
                                tmpMetaData.AddRange(Hex.GetBinary(int.Parse(vals[1])));
                                break;
                        }
                    }
                    catch
                    {
                        // The most likely reson for ending up here is that we got
                        //   passed some data that didn't match the field type for
                        //   the specified field. Just chunk the error back up the
                        //   stack and let the caller deal with it.
                        throw;
                    }
                    // We don't want to add the values to the actuall metaData
                    //   collection until we're sure we've got it all processes
                    //   and converted. Otherwise, we might end up wtih 'orphaned'
                    //   meta data.
                    this._metaData.AddRange(tmpMetaData.ToArray());
                    tagBts.AddRange(Hex.GetBinary(metaPointer, this._littleEndian));
                }
            }
            else
            {
                // The tag value is small enough to reside inside the tag itself.
                //   This automatically rules out certain data types such as
                //   Rational and ASCII, so we should only be dealing with
                //   Byte, Short, and Long here.
                for (int i = 0; i < value.Length; i++)
                    if (value[i].GetType().Name.ToLower() == "int32")
                        tagBts.AddRange(Hex.GetBinary((short)value[i], this._littleEndian));
                    else if (value[i].GetType().Name.ToLower() == "int64")
                        tagBts.AddRange(Hex.GetBinary((int)value[i], this._littleEndian));
            }
            // If we ended up with less than 12 bytes for the tag data, it means
            //   we're storing a short or byte value that doesn't 'fill' the tag's
            //   value space, so we just need to add 'padding'.
            int btShort=12-tagBts.Count;
            for (int t = 0; t < btShort; t++)
                tagBts.Add(byte.Parse("00"));

            // Now, we're read to add the tag to the IDF.
            this._idfs[idfNum].Tags.Add(new TiffTag(this._idfs[idfNum], tagBts.ToArray()));
        }
        public TiffTag GetTag(TiffIdf idf, TiffTagType tagType)
        { return this.GetTag(this._idfs.IndexOf(idf), tagType); }
        public TiffTag GetTag(int idfNum, TiffTagType tagType)
        {
            return null;
        }
        public void WriteFile(string fileName)
        {
            foreach (TiffIdf idf in this._idfs)
            {
            }
        }
        #endregion
    }
}
