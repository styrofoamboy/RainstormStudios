using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RainstormStudios.Data
{
    public sealed class SqlHelper
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static SqlConnection GetOpenConnection(string connString)
        { return SqlHelper.GetOpenConnection(connString, 5, 2000); }
        public static SqlConnection GetOpenConnection(string connString, int retryCnt, int retryDelay)
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            int attemptCnt = 0;
            while (conn.State != ConnectionState.Open && attemptCnt++ < retryCnt)
            {
                System.Threading.Thread.Sleep(retryDelay);
                conn.Open();
            }
            if (conn.State != ConnectionState.Open)
                throw new Exception("Unable to open connection to database using specified connection string.");
            return conn;
        }
        #endregion
    }
}
