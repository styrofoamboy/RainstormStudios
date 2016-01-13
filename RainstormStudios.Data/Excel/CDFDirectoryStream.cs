using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data.Excel
{
    public class CDFDirectoryStream
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        int
            dId,
            leftChild,
            rightChild,
            rootNode,
            streamSid;
        long
            streamSize;
        byte[]
            uniqueId,
            userFlags;
        DateTime
            created,
            modified;
        string
            dirName;
        StreamType
            dirType;
        NodeColor
            dirColor;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public int DirectoryID
        { get { return this.dId; } set { this.dId = value; } }
        public int LeftChild
        { get { return this.leftChild; } set { this.leftChild = value; } }
        public int RightChild
        { get { return this.rightChild; } set { this.rightChild = value; } }
        public int RootNode
        { get { return this.rootNode; } set { this.rootNode = value; } }
        public int StreamSID
        { get { return this.streamSid; } set { this.streamSid = value; } }
        public long StreamSize
        { get { return this.streamSize; } set { this.streamSize = value; } }
        public byte[] UniqueID
        { get { return this.uniqueId; } set { this.uniqueId = value; } }
        public DateTime TimestampCreated
        { get { return this.created; } set { this.created = value; } }
        public DateTime TimestampModified
        { get { return this.modified; } set { this.modified = value; } }
        public string DirectoryName
        { get { return this.dirName; } set { this.dirName = value; } }
        public StreamType DirectoryType
        { get { return this.dirType; } set { this.dirType = value; } }
        public NodeColor DirectoryNodeColor
        { get { return this.dirColor; } set { this.dirColor = value; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected internal CDFDirectoryStream(int dId, string dirName, byte[] uniqueId, DateTime created, DateTime modified, StreamType type, NodeColor clr, int strmSid, int leftChild, int rightChild, int rootNode, long strmSize)
        {
            this.dId = dId;
            this.dirName = dirName;
            this.uniqueId = uniqueId;
            this.created = created;
            this.modified = modified;
            this.created = created;
            this.modified = modified;
            this.dirType = type;
            this.dirColor = clr;
            this.leftChild = leftChild;
            this.rightChild = rightChild;
            this.rootNode = rootNode;
            this.streamSid = strmSid;
            this.streamSize = strmSize;
        }
        public CDFDirectoryStream(int dId, string dirName)
            : this(dId, dirName, new byte[16], DateTime.MinValue, DateTime.MinValue, StreamType.Empty, NodeColor.Black, -1, -1, -1, 0, 0)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static CDFDirectoryStream ReadStream(byte[] header)
        {
            if (header.Length < 127)
                throw new ArgumentException("Specified byte array does not contain the correct number of indicies to contain a Compound Document File Header.", "header");

            // Grab bytes 64 and 65.  These two bytes define how long
            //   the name of this directory stream is
            byte[] dNameLen = new byte[2];
            Array.Copy(header, 64, dNameLen, 0, 2);
            //Array.Reverse(dNameLen);

            // Once we know how many bytes to read from the direcory name's
            //   byte area, we can grab that many bytes and begin looping
            //   through the captured byte array, parsing each character
            //   two bytes at a time.
            //int dNameLenInt = Convert.ToInt32(Hex.ToHex(dNameLen), 16) - 1;
            int dNameLenInt = Hex.GetInteger(dNameLen, true) - 1;
            //byte[] dName = new byte[dNameLenInt];
            //Array.Copy(header, dName, dNameLenInt);
            //string dNameReal = "";
            //for (int i = 0; i < dNameLenInt; i += 2)
            //{
            //    byte[] nameChar = new byte[] { dName[i], dName[i + 1] };
            //    Array.Reverse(nameChar);
            //    dNameReal += char.Parse(Convert.ToInt32(Hex.ToHex(nameChar), 16).ToString());
            //}
            string dNameReal = System.Text.Encoding.Unicode.GetString(header, 0, dNameLenInt);

            // Byte 66 defines the stream type.
            StreamType dirType = (StreamType)header[66];

            // Byte 67 defines the node color of this entry in the red/black tree.
            NodeColor dirClr = (NodeColor)header[67];

            // Offset 68 contains four bytes defining this entry's left child node.
            byte[] leftChildBytes = new byte[4];
            Array.Copy(header, 68, leftChildBytes, 0, 4);
            //Array.Reverse(leftChildBytes);
            //int leftChild = Convert.ToInt32(Hex.ToHex(leftChildBytes), 16);
            int leftChild = Hex.GetInteger(leftChildBytes, true);

            // Offset 72 contains four bytes defining this entry's right child node.
            byte[] rightChildBytes = new byte[4];
            Array.Copy(header, 72, rightChildBytes, 0, 4);
            //Array.Reverse(rightChildBytes);
            //int rightChild = Convert.ToInt32(Hex.ToHex(rightChildBytes), 16);
            int rightChild = Hex.GetInteger(rightChildBytes, true);

            // 16 Bytes at offset 80 contain a 'unique identifier'.  This value is
            //   often zeros, but we'll grab the value anyway.
            byte[] uniqueId = new byte[16];
            Array.Copy(header, 80, uniqueId, 0, 16);

            // 4 Bytes at offset 96 container "User Flags". They are ignored in this
            //   implementation, since I have no idea what they do.
            byte[] usrFlags = new byte[4];
            Array.Copy(header, 96, usrFlags, 0, 4);

            // Offset 100 contains 8 bytes defining the timestamp of when the
            //   stream was created.
            byte[] createdTimestamp = new byte[8];
            Array.Copy(header, 100, createdTimestamp, 0, 8);
            long createStamp = Hex.GetLong(createdTimestamp, true);

            // Offset 108 contains 8 bytes defining the timestamp of when the
            //   stream was last modified.
            byte[] modifiedTimestamp = new byte[8];
            Array.Copy(header, 108, modifiedTimestamp, 0, 8);
            long modifyStamp = Hex.GetLong(modifiedTimestamp, true);

            // Offset 116 contains (4 bytes) one of three values:
            //   * If this entry refers to a stream - SectorID of the first sector or
            //     short-sector.
            //   * If this is the root storage entry - SectorID of the first sector of
            //     the short-stream container stream
            //   * Or just '0'
            byte[] firstSid = new byte[4];
            Array.Copy(header, 116, firstSid, 0, 4);
            int iFirstSid = Hex.GetInteger(firstSid, true);

            // 4 bytes at offset 120 define the total length of the stream.
            byte[] streamSz = new byte[4];
            Array.Copy(header, 120, streamSz, 0, 4);
            int iStreamSz = Hex.GetInteger(streamSz, true);

            // The last four bytes of the header (124-128) are not used.

            return new CDFDirectoryStream(0, dNameReal, uniqueId, new DateTime(createStamp), new DateTime(modifyStamp), dirType, dirClr, iFirstSid, leftChild, rightChild, 0, iStreamSz);
        }
        #endregion
    }
}
