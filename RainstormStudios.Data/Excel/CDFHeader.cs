using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data.Excel
{
    public class CompoundDocumentFileHeader
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        string
            _uid;
        int
            _ver,
            _rev,
            _secSz,
            _ssecSz;

        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public String UniqueID
        { get { return this._uid; } set { this._uid = value; } }
        public int FileVersion
        { get { return this._ver; } set { this._ver = value; } }
        public int FileRevision
        { get { return this._rev; } set { this._rev = value; } }
        public int SectorSize
        { get { return this._secSz; } set { this._secSz = value; } }
        public long RealSectorSize
        { get { return (long)System.Math.Pow(2, (double)this._secSz); } }
        public int ShortSectorSize
        { get { return this._ssecSz; } set { this._ssecSz = value; } }
        public long RealShortSectorSize
        { get { return (long)System.Math.Pow(2, (double)this._ssecSz); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal CompoundDocumentFileHeader()
        {
        }
        #endregion
    }
}
