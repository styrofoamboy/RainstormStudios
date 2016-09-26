using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RainstormStudios.Data
{
    public static class CsvWriter
    {
        /// <summary>
        /// Converts the <see cref="T:System.Data.IEnumerable"/> data source into CSV formatted data and returns that data as a <see cref="T:System.IO.MemoryStream"/> stream.
        /// </summary>
        /// <param name="data">The <see cref="T:System.Linq.IQueryable"/> object containing the data to be parsed.</param>
        /// <param name="columnHeaders">A value of type <see cref="T:System.Boolean"/> indicating true if the resulting CSV file should include column headers. Otherwise, false.</param>
        /// <returns>A <see cref="T:System.IO.MemoryStream"/> containing the resulting CSV file's binary data.</returns>
        public static System.IO.MemoryStream CreateCSV(IEnumerable data, bool columnHeaders = true)
        { return CsvWriter.CreateCSV(data, columnHeaders, new string[0]); }
        /// <summary>
        /// Converts the <see cref="T:System.Data.IEnumerable"/> data source into CSV formatted data and returns that data as a <see cref="T:System.IO.MemoryStream"/> stream.
        /// </summary>
        /// <param name="data">The <see cref="T:System.Linq.IQueryable"/> object containing the data to be parsed.</param>
        /// <param name="skipColumns">A <see cref="T:System.String[]"/> array indicating the names of columns that should not be included in the output.</param>
        /// <param name="columnHeaders">A value of type <see cref="T:System.Boolean"/> indicating true if the resulting CSV file should include column headers. Otherwise, false.</param>
        /// <returns>A <see cref="T:System.IO.MemoryStream"/> containing the resulting CSV file's binary data.</returns>
        public static System.IO.MemoryStream CreateCSV(IEnumerable data, bool columnHeaders, params string[] skipColumns)
        {
            List<int> skipColIdx = new List<int>();
            System.IO.MemoryStream strm = new System.IO.MemoryStream();
            {
                int rowNum = 0;
                foreach (var d in data)
                {
                    Type dType = d.GetType();
                    PropertyInfo[] props = dType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

                    // First, we're going to identify any columns we don't want to output (if any are specified).
                    if (rowNum == 0 && skipColumns.Length > 0)
                        for (int i = 0; i < props.Length; i++)
                            if (skipColumns.Contains(props[i].Name))
                                skipColIdx.Add(i);

                    // Now we're going to create the column headers, if we haven't done that already.
                    if (++rowNum == 1 && columnHeaders)
                    {
                        StringBuilder sbHdr = new StringBuilder();
                        for (int i = 0; i < props.Length; i++)
                            if (!props[i].PropertyType.Name.StartsWith("EntitySet") && !skipColIdx.Contains(i))
                            {
                                string colHdrName = props[i].Name;
                                if (colHdrName.Contains(',') || colHdrName.StartsWith(" ") || colHdrName.EndsWith(" "))
                                    sbHdr.AppendFormat(",\"{0}\"", colHdrName);
                                else
                                    sbHdr.Append("," + props[i].Name);
                            }
                        byte[] bufferHdr = System.Text.Encoding.UTF8.GetBytes(sbHdr.ToString().TrimStart(',') + "\r\n");
                        strm.Write(bufferHdr, 0, bufferHdr.Length);
                    }


                    StringBuilder sbRow = new StringBuilder();
                    for (int i = 0; i < props.Length; i++)
                    {
                        if (props[i].PropertyType.Name.StartsWith("EntitySet") || skipColIdx.Contains(i))
                            continue;

                        sbRow.Append(",");
                        object objfieldVal = props[i].GetValue(d, null);
                        string fieldVal = (objfieldVal != null) ? objfieldVal.ToString() : string.Empty;
                        if (fieldVal.Contains(',') || fieldVal.StartsWith(" ") || fieldVal.EndsWith(" "))
                            sbRow.AppendFormat("\"{0}\"", fieldVal);
                        else
                            sbRow.Append(fieldVal);
                    }

                    byte[] bufferRow = System.Text.Encoding.UTF8.GetBytes(sbRow.ToString().TrimStart(',') + "\r\n");
                    strm.Write(bufferRow, 0, bufferRow.Length);
                }
            }
            return strm;
        }
    }
}
