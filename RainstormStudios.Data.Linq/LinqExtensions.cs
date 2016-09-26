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
using System.Text;

namespace RainstormStudios.Data.Linq
{
    public static class LinqExtensions
    {
        #region Declarations
        //***************************************************************************
        // Priate Fields
        // 
        private static readonly System.Reflection.BindingFlags
            bindingFlagsPublicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
        private static readonly System.Reflection.BindingFlags
            bindingFlagsPublicStatic = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
        #endregion

        #region Public Methods
        public static bool Exists<T>(this IQueryable<T> src, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return src.Where(predicate).Any();
        }
        public static bool Exists<T>(this IQueryable<T> src, System.Linq.Expressions.Expression<Func<T, int, bool>> predicate)
        {
            return src.Where(predicate).Any();
        }
        /// <summary>
        /// Breaks this generic <see cref="T:System.Linq.IEnumerable"/> data into 'pages' and returns the specified page.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="pageNum">A <see cref="T:System.Int32"/> value indicating the index of the page of data to be returned.</param>
        /// <param name="pageSize">A <see cref="T:System.Int32"/> value indicating how many records each page of data will contain.</param>
        /// <param name="maxPage">A <see cref="T:System.Int32"/> out parameter indicating the maximum number of pages for all data.</param>
        /// <param name="visibleRecords">A <see cref="T:System.Int32"/> out parameter indicating how many records are visible on the requested page of data. This number should always return with the same value as the <paramref name="pageSize"/> parameter, except for the last page of data.</param>
        /// <param name="totalRecords">A <see cref="T:System.Int32"/> out parameter indicating the total number of records found in the source data.</param>
        /// <returns>An <see cref="T:System.Linq.IEnumberable[T]"/> value containing the requested page of data.</returns>
        public static IEnumerable<T> GetPaged<T>(this IEnumerable<T> data, int pageNum, int pageSize, out int maxPage, out int visibleRecords, out int totalRecords)
        {
            return RainstormStudios.Data.Linq.DataHelper.GetPaged<T>(data, pageNum, pageSize, out maxPage, out visibleRecords, out totalRecords);
        }
        /// <summary>
        /// Breaks this <see cref="T:System.Linq.IEnumerable"/> data into 'pages' and returns the specified page.
        /// </summary>
        /// <param name="pageNum">A <see cref="T:System.Int32"/> value indicating the index of the page of data to be returned.</param>
        /// <param name="pageSize">A <see cref="T:System.Int32"/> value indicating how many records each page of data will contain.</param>
        /// <param name="maxPage">A <see cref="T:System.Int32"/> out parameter indicating the maximum number of pages for all data.</param>
        /// <param name="visibleRecords">A <see cref="T:System.Int32"/> out parameter indicating how many records are visible on the requested page of data. This number should always return with the same value as the <paramref name="pageSize"/> parameter, except for the last page of data.</param>
        /// <param name="totalRecords">A <see cref="T:System.Int32"/> out parameter indicating the total number of records found in the source data.</param>
        /// <returns>An <see cref="T:System.Linq.IEnumberable[T]"/> value containing the requested page of data.</returns>
        public static IEnumerable GetPaged(this IEnumerable data, int pageNum, int pageSize, out int maxPage, out int visibleRecords, out int totalRecords)
        {
            return RainstormStudios.Data.Linq.DataHelper.GetPaged(data, pageNum, pageSize, out maxPage, out visibleRecords, out totalRecords);
        }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IEnumerable"/> object based on the parameters specified.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IEnumerable"/> data.</typeparam>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="contains">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be a 'contains', or false for an exact match.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IEnumerable"/> containing the data matching the filter requirements.</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> data, string filterProperty, object filterValue, bool contains = true, bool matchCase = false)
        {
            return RainstormStudios.Data.Linq.DataHelper.FilterResult<T>(data, filterProperty, filterValue, contains, matchCase);
        }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IQueryable"/> object based on the parameters specified and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IQueryable"/> data.</typeparam>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> data, string filterProperty, object filterValue)
        {
            return (IQueryable<T>)Filter((IQueryable)data, filterProperty, filterValue);
        }
        /// <summary>
        /// Filters the data in the generic <see cref="T:System.Linq.IQueryable"/> object based on the parameters specified and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <typeparam name="T">The non-generic data type of the source <see cref="T:System.Linq.IQueryable"/> data.</typeparam>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="op">A value from the <see cref="T:ITCWebToolkit.ComparisonOperator"/> enumeration indicating how the data will be compared.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> data, string filterProperty, object filterValue, Linq.ComparisonOperator op, bool matchCase = false)
        {
            return (IQueryable<T>)Filter((IQueryable)data, filterProperty, filterValue, op, matchCase);
        }
        /// <summary>
        /// Filters the data in the <see cref="T:System.Linq.IQueryable"/> object based on the parameters specifed and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable Filter(this IQueryable data, string filterProperty, object filterValue)
        {
            return RainstormStudios.Data.Linq.DataHelper.FilterResult(data, filterProperty, filterValue);
        }
        /// <summary>
        /// Filters the data in the <see cref="T:System.Linq.IQueryable"/> object based on the parameters specifed and appends those parameters to the object prior to SQL execution.
        /// </summary>
        /// <param name="filterProperty">A <see cref="T:System.String"/> value indicating the name of a public, non-static property member of type 'T' to compare.</param>
        /// <param name="filterValue">A <see cref="T:System.Object"/> value indicating the value(s) to compare against.</param>
        /// <param name="op">A value from the <see cref="T:ITCWebToolkit.ComparisonOperator"/> enumeration indicating how the data will be compared.</param>
        /// <param name="matchCase">A value of type <see cref="T:System.Boolean"/> indicating true if the comparison should be case sensitive. Otherwise, false.</param>
        /// <returns>A value of type <see cref="T:System.Linq.IQueryable"/> containing the data matching the filter requirements.</returns>
        public static IQueryable Filter(this IQueryable data, string filterProperty, object filterValue, Linq.ComparisonOperator op, bool matchCase = false)
        {
            return RainstormStudios.Data.Linq.DataHelper.FilterResult(data, filterProperty, filterValue, op, matchCase);
        }
        /// <summary>
        /// Converts this <see cref="T:System.Collections.IEnumerable"/> data source into CSV formatted data and returns that data as a <see cref="T:System.IO.MemoryStream"/> stream.
        /// </summary>
        /// <param name="columnHeaders">A value of type <see cref="T:System.Boolean"/> indicating true if the resulting CSV file should include column headers. Otherwise, false.</param>
        /// <returns>A <see cref="T:System.IO.MemoryStream"/> containing the resulting CSV file's binary data.</returns>
        public static System.IO.MemoryStream CreateCSV(this IEnumerable data, bool columnHeaders = true)
        {
            return RainstormStudios.Data.CsvWriter.CreateCSV(data, columnHeaders);
        }
        /// <summary>
        /// Converts this <see cref="T:System.Collections.IEnumerable"/> data source into CSV formatted data and returns that data as a <see cref="T:System.IO.MemoryStream"/> stream.
        /// </summary>
        /// <param name="columnHeaders">A value of type <see cref="T:System.Boolean"/> indicating true if the resulting CSV file should include column headers. Otherwise, false.</param>
        /// <param name="skipColumns">An array of <see cref="T:System.String"/> values indicating property names that should not be included in the output file.</param>
        /// <returns>A <see cref="T:System.IO.MemoryStream"/> containing the resulting CSV file's binary data.</returns>
        public static System.IO.MemoryStream CreateCSV(this IEnumerable data, bool columnHeaders, params string[] skipColumns)
        {
            return RainstormStudios.Data.CsvWriter.CreateCSV(data, columnHeaders, skipColumns);
        }
        /// <summary>
        /// Creates a duplicate copy of this Linq2Sql entity, but is completely detached from a database context. NOTE: This method should be considered 'expensive', because it uses reflection during the clone process.
        /// </summary>
        /// <typeparam name="T">A Linq2Sql entity that inherits from <see cref="T:ITCWebToolkit.Linq.ITCEntityBase"/>.</typeparam>
        /// <param name="src">The Linq2Sql entity to copy.</param>
        /// <param name="primaryKeyCols">A list of <see cref="T:System.String"/> values containing the names of properties that contain the primary key values from the database. These properties' values will not be cloned to the new instance.</param>
        /// <returns>A new instance of the entitiy type which contains all the same values, except for any property names listed in the <paramref name="primaryKeyCols"/> parameter.</returns>
        public static T DeepClone<T>(this T src, params string[] primaryKeyCols)
            where T : rsEntityBase, new()
        {
            Type dType = src.GetType();
            System.Reflection.PropertyInfo[] props = dType.GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            // First, we're going to identify columns we don't want to clone.
            List<int> skipColIdx = new List<int>();
            for (int i = 0; i < primaryKeyCols.Length; i++)
                if (primaryKeyCols.Contains(props[i].Name))
                    skipColIdx.Add(i);

            // Create a new instance of the same type as the source.
            T newInstance = new T();
            System.Reflection.PropertyInfo[] setProps = dType.GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            List<string> setPropsNameLookup = new List<string>();
            setPropsNameLookup.AddRange(setProps.Select<System.Reflection.PropertyInfo, string>(s => s.Name));

            // Loop through the properties and clone their data.
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].PropertyType.Name.StartsWith("EntitySet") || skipColIdx.Contains(i))
                    continue;

                object origValue = props[i].GetValue(src, null);
                setProps[setPropsNameLookup.IndexOf(props[i].Name)].SetValue(newInstance, origValue, null);
            }

            return newInstance;
        }
        /// <summary>
        /// Creates a 'shallow' cloned copy of this entity by serializing it, and the returning a deserialized copy.
        /// !!NOTE!!  This may not work the way you expect.  The cloned entity will have no relation to the original
        /// context.  If you use the "InsertOnSubmit", the existing record will be updated.  If you "Add" to a 
        /// queried entity's foreign key collection, then a new record will be created.
        /// </summary>
        public static T Clone<T>(this T src)
            where T : rsEntityBase 
        {
            return Serialization.SerializationHelpers.Clone(src);
        }
        public static byte[] GetSerialized<T>(this T src)
            where T : rsEntityBase
        {
            return Serialization.SerializationHelpers.GetSerialized(src);
        }
        public static T GetDeserialized<T>(this byte[] src)
            where T : rsEntityBase
        {
            return Serialization.SerializationHelpers.GetDeserialized<T>(src);
        }
        /// <summary>
        /// Determines if this Linq2Sql entity is attached to the specified context.
        /// </summary>
        /// <typeparam name="T">A Linq2Sql entity that inherits from the <see cref="T:ITCWebToolkit.Linq.ITCEntityBase"/> class.</typeparam>
        /// <param name="entity">The Linq2Sql entity to test.</param>
        /// <param name="dbContext">An <see cref="T:System.Data.Linq.DataContext"/> Linq2Sql data context.</param>
        /// <returns>A <see cref="T:System.Boolean"/> value indicating true, if the entity is attached. Otherwise, false.</returns>
        public static bool IsAttached<T>(this T entity, System.Data.Linq.DataContext ctx)
            where T : rsEntityBase, new()
        {
            return CheckEntityIsAttached<T>(ctx, entity);
        }
        /// <summary>
        /// Determines if the specified Linq2Sql entity is attached to this context.
        /// </summary>
        /// <typeparam name="T">A Linq2Sql class that inherits from the <see cref="T:ITCWebToolkit.Linq.ITCEntityBase"/> class.</typeparam>
        /// <param name="dbContext">An <see cref="T:System.Data.Linq.DataContext"/> object.</param>
        /// <param name="entity">The Linq2Sql entity to test.</param>
        /// <returns>A <see cref="T:System.Boolean"/> value indicating true, if the entity is attached. Other, false.</returns>
        public static bool IsAttached<T>(this System.Data.Linq.DataContext ctx, T entity)
            where T : rsEntityBase, new()
        {
            return CheckEntityIsAttached<T>(ctx, entity);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static bool CheckEntityIsAttached<T>(System.Data.Linq.DataContext ctx, T entity)
            where T : rsEntityBase, new()
        {
            // This is *SO* much simpler than the old way I was doing it, it's not even funny.
            System.Data.Linq.Table<T> entityTbl = ctx.GetTable<T>();
            return (entityTbl.GetOriginalEntityState(entity) != null);

            #region DEPRECIATED - This old method is a cluster, and has been deemed stupid, thanks to me realizing the context will give me the table for a given entity
            //// NOTE:  This is a fairly expensive method call, since it's performing
            ////   two seperate reflection lookups.

            //// Get the Type of the passed entity.
            //Type entityType = entity.GetType();
            //string entitiyTypeName = entityType.Name;

            //// Generally, the Linq2Sql table names are the 'plural' form of the entity names.
            //string srchName = (entitiyTypeName.EndsWith("y") ? entitiyTypeName.TrimEnd('y') + "ies" : entitiyTypeName + "s");

            //// Try and find the context's property that the passed entity was queried
            ////   from.  This method is *far* from fool-proof.
            //System.Reflection.PropertyInfo piTableProperty = ctx.GetType().GetProperty(srchName, bindingFlagsPublicInstance);
            //if (piTableProperty == null)
            //    throw new MemberAccessException(string.Format("Unable to locate property '{0}' in data context.", srchName));

            //// If we found the table property, we need to get the value of the property, and
            ////   locate the static 'GetOriginalEntityState' method.
            //object ctxTable = piTableProperty.GetValue(ctx, null);
            //Type ctxTableType = ctxTable.GetType();

            //System.Reflection.MethodInfo miGetEntityState = ctxTableType.GetMethod("GetOriginalEntityState", new Type[] { entityType });
            //if (miGetEntityState == null)
            //    throw new MemberAccessException(string.Format("Unable to locate 'GetOriginalEntityState' method in Type '{0}'.", ctxTableType.Name));

            //// If all of that worked out, we just have to call (invoke) the method, and pass
            ////   the supplied entity as a parameter.  If we get a 'null' back, then the entity
            ////   is not attached to the context.
            //object miReturn = miGetEntityState.Invoke(ctxTable, new object[] { entity });

            //return miReturn != null;
            #endregion
        }
        #endregion

    }
}
