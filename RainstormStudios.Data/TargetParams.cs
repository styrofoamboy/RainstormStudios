using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    public class TargetParams
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private DataTargetType
            _type = DataTargetType.None;
        private rsDb
            _dbTarget = null;
        private bool
            _colHeaders,
            _trimVals,
            _csvExcelFormat,
            _csvEscapeChar,
            _xmlSchema,
            _truncateMe,
            _useTS,
            _xlsColNames,
            _xlsImex;
        private string
            _id = string.Empty,
            _filePath = string.Empty,
            _delim = string.Empty,
            _xmlSchemaPath = string.Empty;
        private object 
            _owner;
        private SqlTableParams
            _tblParams = null;
        #endregion
    }
}
