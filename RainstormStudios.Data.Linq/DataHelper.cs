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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace RainstormStudios.Data.Linq
{
    public enum ComparisonOperator : int
    {
        Equals = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5,
        Contains = 6
    }
    public class DataHelper
    {
        private static readonly string[] compOpStr = new string[] { " =", " <>", " >", " >=", " <", " <=", ".ToString().Contains(" };

        #region Declarations
        //*******************************************************************************
        // Private Fields
        // 
        /// <summary>
        /// This indicates the BindingFlags used by the .NET reflection engine to retrieve the property members for comparison in the FilterResult method.
        /// </summary>
        const System.Reflection.BindingFlags
            LinqPropertyBingingFlags = System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Breaks this <see cref="T:System.Linq.IEnumerable<>"/> data into 'pages' and returns the specified page.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="pageNum">A <see cref="T:System.Int32"/> value indicating the index of the page of data to be returned.</param>
        /// <param name="pageSize">A <see cref="T:System.Int32"/> value indicating how many records each page of data will contain.</param>
        /// <param name="maxPage">A <see cref="T:System.Int32"/> out parameter indicating the maximum number of pages for all data.</param>
        /// <param name="visibleRecords">A <see cref="T:System.Int32"/> out parameter indicating how many records are visible on the requested page of data. This number should generally return with the same value as the <paramref name="pageSize"/> parameter, except for the last page of data.</param>
        /// <param name="totalRecords">A <see cref="T:System.Int32"/> out parameter indicating the total number of records found in the source data.</param>
        /// <returns>An <see cref="T:System.Linq.IEnumberable"/> value containing the requested page of data.</returns>
        public static IEnumerable<T> GetPaged<T>(IEnumerable<T> records, int pageNum, int pageSize, out int maxPage, out int visibleRecords, out int totalRecords)
        {
            totalRecords = records.Count();
            maxPage = CalcMaxPage(pageSize, totalRecords);
            var pageRecords = records.Skip(System.Math.Min(pageNum, maxPage) * pageSize).Take(pageSize);
            visibleRecords = pageRecords.Count();
            return pageRecords;
        }
        /// <summary>
        /// Breaks this <see cref="T:System.Collections.IEnumerable"/> data into 'pages' and returns the specified page.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Collections.IEnumerable"/> data.</typeparam>
        /// <param name="pageNum">A <see cref="T:System.Int32"/> value indicating the index of the page of data to be returned.</param>
        /// <param name="pageSize">A <see cref="T:System.Int32"/> value indicating how many records each page of data will contain.</param>
        /// <param name="maxPage">A <see cref="T:System.Int32"/> out parameter indicating the maximum number of pages for all data.</param>
        /// <param name="visibleRecords">A <see cref="T:System.Int32"/> out parameter indicating how many records are visible on the requested page of data. This number should generally return with the same value as the <paramref name="pageSize"/> parameter, except for the last page of data.</param>
        /// <param name="totalRecords">A <see cref="T:System.Int32"/> out parameter indicating the total number of records found in the source data.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumberable"/> value containing the requested page of data.</returns>
        public static IEnumerable GetPaged(IEnumerable records, int pageNum, int pageSize, out int maxPage, out int visibleRecords, out int totalRecords)
        {
            ArrayList pageRecords = new ArrayList();
            IEnumerator eator = records.GetEnumerator();
            int iRecIdx = 0;
            int iStartRec = pageNum * pageSize;
            while (eator.MoveNext())
            {
                iRecIdx++;
                if (iRecIdx > iStartRec && pageRecords.Count <= pageSize)
                    pageRecords.Add(eator.Current);
            }

            totalRecords = iRecIdx;
            visibleRecords = pageRecords.Count;
            maxPage = CalcMaxPage(pageSize, totalRecords);
            return pageRecords;
        }
        /// <summary>
        /// Determines the maximum number of pages required to display data in the specified format.  This method just makes this calculation uniform across all code bases.
        /// </summary>
        /// <param name="pgSz">The size of each page.</param>
        /// <param name="recCnt">The total number of records.</param>
        /// <returns>A <see cref="T:System.Int32"/> value indicating the total number of pages required.</returns>
        public static int CalcMaxPage(double pgSz, double recCnt)
        {
            return System.Math.Max(1, (int)System.Math.Ceiling(recCnt / pgSz));
        }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IQueryable"/> object based on the parameters specified and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IQueryable"/> data.</typeparam>
        /// <param name="dataset">A value of type generic <see cref="T:System.Linq.IQueryable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable<T> FilterResult<T>(IQueryable<T> dataset, string filterProperty, object filterValue)
        { return FilterResult<T>(dataset, filterProperty, filterValue, ComparisonOperator.Equals); }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IQueryable"/> object based on the parameters specified and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IQueryable"/> data.</typeparam>
        /// <param name="dataset">A value of type generic <see cref="T:System.Linq.IQueryable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="op">A value from the <see cref="T:RainstormStudios.Data.Linq.ComparisonOperator"/> enumeration indicating how the data will be compared.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable<T> FilterResult<T>(IQueryable<T> dataset, string filterProperty, object filterValue, ComparisonOperator op, bool matchCase = false)
        { return (IQueryable<T>)FilterResult((IQueryable)dataset, filterProperty, filterValue, op, matchCase); }
        /// <summary>
        /// Filters the data in the <see cref="T:System.Linq.IQueryable"/> object based on the parameters specifed and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <param name="dataset">A value of type <see cref="T:System.Linq.IQueryable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable FilterResult(IQueryable dataset, string filterProperty, object filterValue)
        { return FilterResult(dataset, filterProperty, filterValue, ComparisonOperator.Equals); }
        /// <summary>
        /// Filters the data in the <see cref="T:System.Linq.IQueryable"/> object based on the parameters specifed and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <param name="dataset">A value of type <see cref="T:System.Linq.IQueryable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="op">A value from the <see cref="T:RainstormStudios.Data.Linq.ComparisonOperator"/> enumeration indicating how the data will be compared.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable FilterResult(IQueryable dataset, string filterProperty, object filterValue, ComparisonOperator op, bool matchCase = false)
        {
            if (op == ComparisonOperator.Contains && (!(filterValue is string) && !(filterValue is Array)))
                throw new ArgumentException(string.Format("Contains operation is not valid on member {1}.  Invalid data type: {0}", filterValue.GetType().Name, filterProperty), "filterValue");

            if (op != ComparisonOperator.Contains && op != ComparisonOperator.Equals)
            {
                // This would require a numerical, DateTime, byte or boolean data type.  This is mostly as a failsafe, to get a "cleaner" error message.
                int iVal; DateTime dtVal; byte btVal; bool bVal;
                if (filterValue is string)
                {
                    string fv = (string)filterValue;
                    if (!int.TryParse(fv, out iVal) && !DateTime.TryParse(fv, out dtVal) && !byte.TryParse(fv, out btVal) && !bool.TryParse(fv, out bVal))
                        throw new ArgumentException(string.Format("Unable to convert value to valid data type for requested operation: {0} ({1})", op.ToString(), filterValue.GetType().Name));
                }
            }

            StringBuilder sbPredicate = new StringBuilder();
            if (filterValue is Array && op == ComparisonOperator.Contains)
            {
                sbPredicate.Append("@0.Contains(");
                sbPredicate.Append(filterProperty);
                if (!matchCase)
                    sbPredicate.Append(".ToLower()");
                sbPredicate.Append(")");
            }
            else
            {
                sbPredicate.Append(filterProperty);
                if (filterValue is string && !matchCase)
                    sbPredicate.Append(".ToLower()");
                sbPredicate.Append(compOpStr[(int)op]);
                sbPredicate.Append("@0");
                if (op == ComparisonOperator.Contains)
                    sbPredicate.Append(")");
            }

            string pred = sbPredicate.ToString();

            if ((filterValue is Array) && !matchCase)
            {
                List<object> lst = new List<object>();
                foreach (object obj in ((Array)filterValue))
                {
                    if (obj is string)
                        lst.Add(obj.ToString().ToLower());
                    else
                        lst.Add(obj);
                }
                return dataset.Where(pred, filterValue);
            }

            else if (filterValue is string && !matchCase)
                return dataset.Where(pred, filterValue.ToString());

            else
                return dataset.Where(pred, filterValue);
        }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IEnumerable"/> object based on the parameters specified.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="dataset">A value of type <see cref="T:System.Linq.IEnumerable<>"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data matching the filter requirements.</returns>
        public static IEnumerable<T> FilterResult<T>(IEnumerable<T> dataset, string filterProperty, object filterValue)
        { return DataHelper.FilterResult<T>(dataset, filterProperty, filterValue, true, false); }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IEnumerable"/> object based on the parameters specified.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="dataset">A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="contains">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be a 'contains', or false for an exact match.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data matching the filter requirements.</returns>
        public static IEnumerable<T> FilterResult<T>(IEnumerable<T> dataset, string filterProperty, object filterValue, bool contains)
        { return DataHelper.FilterResult<T>(dataset, filterProperty, filterValue, contains, false); }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IEnumerable"/> object based on the parameters specified.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="dataset">A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data to be filtered.</param>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="contains">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be a 'contains', or false for an exact match.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data matching the filter requirements.</returns>
        public static IEnumerable<T> FilterResult<T>(IEnumerable<T> dataset, string filterProperty, object filterValue, bool contains, bool matchCase)
        {
            if (!(filterValue is string) && contains)
                throw new ArgumentException("You cannot use 'contains' on this data type.", "filterValue");

            Type objectType = typeof(T);
            System.Reflection.PropertyInfo propInfo = objectType.GetProperty(filterProperty, LinqPropertyBingingFlags);
            if (propInfo == null)
                throw new Exception("No property found with name provided in 'filterCol' argument: " + filterProperty);

            // LINQ + Reflection, FTW! :)

            if (!matchCase && (filterValue is string))
                if (contains)
                    return dataset.Where(r => propInfo.GetValue(r, null).ToString().ToLower().Contains(((string)filterValue).ToLower()));
                else
                    return dataset.Where(r => propInfo.GetValue(r, null).ToString().ToLower().Equals(((string)filterValue).ToLower()));
            else
                if (contains)
                    return dataset.Where(r => propInfo.GetValue(r, null).ToString().Contains(filterValue.ToString()));
                else
                    return dataset.Where(r => propInfo.GetValue(r, null).ToString().Equals(filterValue.ToString()));
        }
        /// <summary>
        /// DO NOT USE! This method is not yet functional. A <see cref="T:System.NotImplementedException"/> will be thrown.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="filterProperty"></param>
        /// <param name="filterValue"></param>
        /// <param name="contains"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public static IEnumerable FilterResult(IEnumerable dataset, string filterProperty, string filterValue, bool contains, bool matchCase)
        {
            throw new NotImplementedException();

            //IEnumerator e = dataset.GetEnumerator();
            //object firstRecord = null;
            //while (e.MoveNext())
            //    firstRecord = e.Current;
            //if (firstRecord == null)
            //    return null;

            //Type objectType = firstRecord.GetType();
            //System.Reflection.PropertyInfo propInfo = objectType.GetProperty(filterProperty, LinqPropertyBingingFlags);
            //if (propInfo == null)
            //    throw new Exception("No property found with name provided in 'filterCol' argument: " + filterProperty);

            //if (!matchCase)
            //    if (contains)
            //        return dataset.Where(r => propInfo.GetValue(r, null).ToString().ToLower().Contains(filterValue.ToLower()));
            //    else
            //        return dataset.Where(r => propInfo.GetValue(r, null).ToString().ToLower().Equals(filterValue.ToLower()));
            //else
            //    if (contains)
            //        return dataset.Where(r => propInfo.GetValue(r, null).ToString().Contains(filterValue));
            //    else
            //        return dataset.Where(r => propInfo.GetValue(r, null).ToString().Equals(filterValue));
        }
        #endregion
    }
}
