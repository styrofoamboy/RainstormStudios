using System;
using System.Collections.Generic;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Data.Excel
{
    public class CompoundDocumentFile
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ByteCollection
            _msat,
            _sat;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CompoundDocumentFile()
        {

        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public byte[] GetSector(int sid)
        {
            return new byte[512];
        }
        #endregion
    }
}
