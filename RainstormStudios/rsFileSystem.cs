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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios
{
    public sealed class rsFileSystem
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static string
            TimeStampFormat = "yyyyMMddHHmmss";
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Appends the current date/time (in 'TimeStamp' format) to the given file name.
        /// </summary>
        /// <param name="FileName">A string value containing the file name to append a time stamp to.</param>
        /// <returns></returns>
        public static string AppendTimeStampToFileName(string FileName)
        {
            try
            {
                FileInfo fi = new FileInfo(FileName);
                return fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + "_" + DateTime.Now.ToString(rsFileSystem.TimeStampFormat) + fi.Extension;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Parses through the given file path and confirms that each folder exists, creating any folders which do not exist.
        /// </summary>
        /// <param name="FolderPath">A string value or the file path, including the drive letter, to create.</param>
        /// <returns>A string value containing the path created.</returns>
        public static string CreateFolderPath(string FolderPath)
        {
            if (!Path.IsPathRooted(FolderPath))
                throw new Exception("You must specify a full path including the drive letter.");

            // If the use pased a path ending with a backslash or space, strip it.
            FolderPath.TrimEnd('\\', ' ');

            //DirectoryInfo di = null;
            string[] directories = null;

            try
            {
                directories = FolderPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string curPath = "";
                foreach (string dir in directories)
                {
                    curPath += dir + "\\";
                    if (!Directory.Exists(curPath))
                        Directory.CreateDirectory(curPath);

                    //di = new DirectoryInfo(curPath);
                    //if (!di.Exists)
                    //    di.Create();
                }
                return curPath;
            }
            catch (Exception ex)
            { throw new Exception("Unable to create folder path.", ex); }
            finally
            {
                // Garbage collection
                //di.Delete(false);
                directories = null;
            }
        }
        /// <summary>
        /// Copies all files and subdirectories from one folder to another.
        /// </summary>
        /// <param name="SrcPath">The source path to copy.</param>
        /// <param name="DstPath">The destination path to copy to.</param>
        /// <returns>A bool value indicating success or failure.</returns>
        public static void CopyDirectory(string SrcPath, string DstPath)
        { rsFileSystem.CopyDirectory(SrcPath, DstPath, false, false); }
        /// <summary>
        /// Copies all files and subdirectories from one folder to another.
        /// </summary>
        /// <param name="SrcPath">The source path to copy.</param>
        /// <param name="DstPath">The destination path to copy to.</param>
        /// <param name="Overwrite">Indicates whether you want to overwrite existing files in the destination folder(s).</param>
        /// <returns>A bool value indicating success or failure.</returns>
        public static void CopyDirectory(string srcPath, string dstPath, bool overwrite, bool delSource)
        { rsFileSystem.CopyDirectory(srcPath, dstPath, overwrite, delSource, "*.*", "*"); }
        /// <summary>
        /// Copies all files and folders matching a specific mask from one folder to another.
        /// </summary>
        /// <param name="srcPath">A System.String value containing the fully qualified path of the source folder.</param>
        /// <param name="dstPath">A System.String value containing the fully qualified path of the destination.</param>
        /// <param name="overwrite">A System.Boolean value indicating whether existing files in the desination folder(s) should be overwritten.</param>
        /// <param name="fileMask">A System.String value indicating a mask used for selecting which files to copy (works recursively).</param>
        public static void CopyDirectory(string srcPath, string dstPath, bool overwrite, string fileMask)
        { rsFileSystem.CopyDirectory(srcPath, dstPath, overwrite, false, fileMask, "*"); }
        /// <summary>
        /// Copies all files and folders matching a specific mask from one folder to another.
        /// </summary>
        /// <param name="srcPath">A System.String value containing the fully qualified path of the source folder.</param>
        /// <param name="dstPath">A System.String value containing the fully qualified path of the destination.</param>
        /// <param name="overwrite">A System.Boolean value indicating whether existing files in the desination folder(s) should be overwritten.</param>
        /// <param name="fileMask">A System.String value indicating a mask used for selecting which files to copy (works recursively).</param>
        /// <param name="dirMask">A System.String value indicating a mask used for selecting which folders to copy (works recursively).</param>
        public static void CopyDirectory(string srcPath, string dstPath, bool overwrite, string fileMask, string dirMask)
        { rsFileSystem.CopyDirectory(srcPath, dstPath, overwrite, false, fileMask, dirMask); }
        public static void DeleteDirectory(string delPath)
        {
            try
            {
                // First, we're going to drill all the way down into the subdirectories.
                string[] subDirs = Directory.GetDirectories(delPath);
                for (int i = 0; i < subDirs.Length; i++)
                    rsFileSystem.DeleteDirectory(Path.Combine(delPath, subDirs[i]));

                // If the folder has no sub directories, then kill all the files.
                string[] files = Directory.GetFiles(delPath, "*.*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                    File.Delete(Path.Combine(delPath, files[i]));

                // And finally, delete the folder itself.
                Directory.Delete(delPath);
            }
            catch (Exception ex)
            { throw new Exception("Unable to delete directory.", ex); }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static void CopyDirectory(string SrcPath, string DstPath, bool Overwrite, bool DelSource, string fileMask, string dirMask)
        {
            try
            {
                rsFileSystem.CreateFolderPath(DstPath);

                // First, drill all the way down to the furthest subdirectory.
                string[] subDirs = Directory.GetDirectories(SrcPath, dirMask, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < subDirs.Length; i++)
                {
                    if (!Directory.Exists(Path.Combine(DstPath, subDirs[i])))
                        Directory.CreateDirectory(Path.Combine(DstPath, subDirs[i]));
                    CopyDirectory(Path.Combine(SrcPath, subDirs[i]), Path.Combine(DstPath, subDirs[i]), Overwrite, DelSource, fileMask, dirMask);
                }

                // Then, copy all files in the current directory.
                string[] files = Directory.GetFiles(SrcPath, fileMask, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    string dstFilePath = Path.Combine(DstPath, Path.GetFileName(files[i]));
                    if (!File.Exists(dstFilePath) || Overwrite)
                        File.Copy(files[i], dstFilePath, true);
                }

                // Finally, delete the source path, if specified.
                if (DelSource)
                    rsFileSystem.DeleteDirectory(SrcPath);
            }
            catch (Exception ex)
            { throw new Exception("Unable to copy folder.", ex); }

            #region Depreciated Code
            //DirectoryInfo diSrc = null;
            //DirectoryInfo diDst = null;

            //try
            //{
            //    diSrc = new DirectoryInfo(SrcPath);
            //    diDst = new DirectoryInfo(rsFileSystem.CreateFolderPath(DstPath));

            //    // Copy all files in the initial directory.
            //    FileInfo[] files = diSrc.GetFiles();
            //    foreach (FileInfo fi in files)
            //        File.Copy(fi.FullName, Path.Combine(DstPath, fi.Name), Overwrite);

            //    // Then, parse through each subdirectory.
            //    DirectoryInfo[] subDirs = diSrc.GetDirectories();
            //    foreach (DirectoryInfo dir in subDirs)
            //    {
            //        rsFileSystem.CreateFolderPath(Path.Combine(DstPath, dir.Name));
            //        CopyDirectory(dir.FullName, Path.Combine(DstPath, dir.Name));
            //    }
            //    diSrc.Delete(DelSource);
            //}
            //catch (Exception ex)
            //{ throw new Exception("Unable to copy folder.", ex); }
            //finally
            //{
            //    //if (diSrc != null)
            //    //    diSrc.Delete(false);
            //    //diDst.Delete(false);
            //}
            #endregion
        }
        #endregion
    }
}
