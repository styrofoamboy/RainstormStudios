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
using RainstormStudios.Collections;

namespace RainstormStudios.IO
{
    /// <summary>
    /// An enumeration containing the possible permission types for the FolderACL classes.
    /// </summary>
    [Flags]
    public enum FolderACLPermissionValue : uint
    {
        None =                  0x00000000,
        Read =                  0x00000002,
        Write =                 0x00000009|Read,
        Modify =                0x00000018|Write,
        Append =                0x00000054|Modify,
        ChangeAttributes =      0x00000162|Modify,
        Delete =                0x00000486|Modify,
        Execute =               0x00001458|Read,
        ReadPermissions =       0x00004374|Read,
        ChangePermissions =     0x00013122,
        TakeOwnership =         0x00039366,
        FullControl =           Append|ChangeAttributes|Delete|Execute|ReadPermissions|ChangePermissions|TakeOwnership
    }
    /// <summary>
    /// Provides a mechanism for storing a file-based ACL for use in Windows or Web applications.  This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class is intended to emulate the Window's ACL system as much as possible, but this class will do nothing if the
    /// user has direct access granted to the file through the actual Windows ACL.  This interface is mostly useful on a
    /// website where documents are stored and provides a way to store that data without the need for a database containing
    /// eveny file name.
    /// </remarks>
    public sealed class FolderACL : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Constants
        // 
        const short
            currentVersionMinor = 1,
            currentVersionMajor = 1;
        const string
            aclFileDefName = "perm.acl";
        //***************************************************************************
        // Private Fields
        // 
        private string
            _fldPath;
        private Guid
            _fldOwner,
            _fldCreator,
            _updUser,
            _curUser;
        private DateTime
            _updDate;
        private int
            _permCnt;
        private long
            _fatSz;
        private short
            _verMinor,
            _verMajor;
        private bool
            _reading,
            _inherits,
            _autosave;
        private FolderACLPermissionEntryCollection
            _permCol;
        private FolderACLFileTable
            _fat;
        private bool
            _disposed;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string FolderPath
        { get { return this._fldPath; } }
        public string ACLFilePath
        { get { return Path.Combine(this._fldPath, aclFileDefName); } }
        public Guid FolderOwner
        { get { return this._fldOwner; } }
        public Guid FolderCreator
        { get { return this._fldCreator; } }
        public Guid LastACLUpdateUser
        { get { return this._updUser; } }
        public DateTime LastACLUpdateTime
        { get { return this._updDate; } }
        public Version ACLFormatVersion
        { get { return new Version(this._verMajor, this._verMinor); } }
        public int PermissionCount
        { get { return this._permCol.Count; } }
        public int FileTableEntryCount
        { get { return this._fat.Count; } }
        public long FileTableSize
        { get { return this._fat.GetFileTableSize(); } }
        public bool AutoSave
        {
            get { return this._autosave; }
            set { this._autosave = value; }
        }
        public bool InheritsParentACL
        {
            get { return this._inherits; }
            set
            {
                this._inherits = value;
                if (this._inherits)
                {
                    // TODO:: Write code to look back up the path one directory for
                    //   a "perm.acl" file to inherit from.
                }
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private FolderACL(Guid folderCreator)
        {
            this._autosave = true;
            this._inherits = true;
            this._fldCreator =
                this._fldOwner =
                this._updUser = folderCreator;
            this._updDate = DateTime.Now;
            this._permCnt = 0;
            this._fatSz = 0;
            this._verMinor = currentVersionMinor;
            this._verMajor = currentVersionMajor;
            this._disposed = false;

            EventHandler clrEvnt = new EventHandler(this.Col_onCleared);
            CollectionEventHandler modEvnt = new CollectionEventHandler(this.Col_onModified);
            this._permCol = new FolderACLPermissionEntryCollection(this);
            this._permCol.Cleared += clrEvnt;
            this._permCol.Inserted += modEvnt;
            this._permCol.Removed += modEvnt;
            this._permCol.Updated += modEvnt;
            this._fat = new FolderACLFileTable(this);
            this._fat.Cleared += clrEvnt;
            this._fat.Inserted += modEvnt;
            this._fat.Removed += modEvnt;
            this._fat.Updated += modEvnt;
        }
        public FolderACL(string folderPath, Guid currentUser)
            : this(currentUser)
        {
            try
            {
                this.SetFolderPath(folderPath);
                this._curUser = currentUser;
                this.ReadACL();
            }
            catch
            { throw; }
        }
        public FolderACL(string folderPath, object currentUserGuid)
            : this(folderPath, new Guid(currentUserGuid.ToString()))
        { }
        ~FolderACL()
        {
            this.Dispose(false);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddFolderPermission(string groupName, FolderACLPermissionValue permissionValues)
        {
            this.AddFolderPermission(groupName, true, permissionValues);
        }
        public void AddFolderPermission(string groupName, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.AddFolderPermission(groupName, true, permissionValues, deny);
        }
        public void AddFolderPermission(Guid userID, FolderACLPermissionValue permissionValues)
        {
            this.AddFolderPermission(userID, false, permissionValues);
        }
        public void AddFolderPermission(Guid userID, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.AddFolderPermission(userID, false, permissionValues, deny);
        }
        public void AddFolderPermission(object permissionID, bool isGroup, FolderACLPermissionValue permissionValues)
        {
            this.AddFolderPermission(permissionValues, isGroup, permissionValues, false);
        }
        public void AddFolderPermission(object permissionID, bool isGroup, FolderACLPermissionValue permissionValues, bool deny)
        {
            this._permCol.AddPermission(permissionID, isGroup, permissionValues, deny, 0);
        }

        public void RemoveFolderPermission(string groupName, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFolderPermission(groupName, true, permissionValues, false);
        }
        public void RemoveFolderPermission(string groupName, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.RemoveFolderPermission(groupName, true, permissionValues, deny);
        }
        public void RemoveFolderPermission(Guid userID, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFolderPermission(userID, false, permissionValues, false);
        }
        public void RemoveFolderPermission(Guid userID, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.RemoveFolderPermission(userID, false, permissionValues, deny);
        }
        public void RemoveFolderPermission(object permissionID, bool isGroup, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFolderPermission(permissionID, isGroup, permissionValues, false);
        }
        public void RemoveFolderPermission(object permissionID, bool isGroup, FolderACLPermissionValue permissionValues, bool deny)
        {
            string SID = string.Format("{0}{1}E{2}", (deny) ? "D" : "A", (isGroup) ? "G" : "U", permissionID.ToString().GetHashCode());
            if (this._permCol.ContainsKey(SID))
            {
                if (this._permCol[SID].PermValues <= permissionValues)
                    this._permCol.RemoveByKey(SID);
                else
                    this._permCol[SID].RemovePermValue(permissionValues);
            }
            else
                throw new ArgumentException("Requested user does not have explicit permissions for this resource.");
        }
        public void RemoveFolderPermission(string SID)
        {
            if (SID.Substring(2, 1).ToUpper() == "I")
                throw new ArgumentException("You cannot delete inherited permissions. Remove the permission from the inheritance source, or turn off inheritance for this ACL.", "SID");

            if (this._permCol.ContainsKey(SID))
                this._permCol.RemoveByKey(SID);
            else
                throw new ArgumentException("Requested SID does not exist with the permissions collection.");
        }

        public void AddFilePermission(string filename, string groupName, FolderACLPermissionValue permissionValues)
        {
            this.AddFilePermission(filename, groupName, true, permissionValues, false);
        }
        public void AddFilePermission(string filename, string groupName, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.AddFilePermission(filename, groupName, true, permissionValues, deny);
        }
        public void AddFilePermission(string filename, Guid userID, FolderACLPermissionValue permissionValues)
        {
            this.AddFilePermission(filename, userID, false, permissionValues, false);
        }
        public void AddFilePermission(string filename, Guid userID, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.AddFilePermission(filename, userID, false, permissionValues, deny);
        }
        public void AddFilePermission(string filename, object permissionID, bool isGroup, FolderACLPermissionValue permissionValues)
        {
            this.AddFilePermission(filename, permissionID, isGroup, permissionValues, false);
        }
        public void AddFilePermission(string filename, object permissionID, bool isGroup, FolderACLPermissionValue permissionValues, bool deny)
        {
            FolderACLFileEntry fe = this._fat[filename];
            if (fe == null)
            {
                fe = new FolderACLFileEntry(filename, 0, 0);
                this._fat.Add(fe);
            }
            fe.PermEntries.AddPermission(permissionID, isGroup, permissionValues, deny, 0);
        }

        public void RemoveFilePermission(string filename, string groupName, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFilePermission(filename, groupName, true, permissionValues, false);
        }
        public void RemoveFilePermission(string filename, string groupName, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.RemoveFilePermission(filename, groupName, true, permissionValues, deny);
        }
        public void RemoveFilePermission(string filename, Guid userID, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFilePermission(filename, userID, false, permissionValues, false);
        }
        public void RemoveFilePermission(string filename, Guid userID, FolderACLPermissionValue permissionValues, bool deny)
        {
            this.RemoveFilePermission(filename, userID, false, permissionValues, deny);
        }
        public void RemoveFilePermission(string filename, object permissionID, bool isGroup, FolderACLPermissionValue permissionValues)
        {
            this.RemoveFilePermission(filename, permissionID, isGroup, permissionValues, false);
        }
        public void RemoveFilePermission(string filename, object permissionID, bool isGroup, FolderACLPermissionValue permissionValues, bool deny)
        {
            FolderACLFileEntry fe = this._fat[filename];
            if (fe == null)
                throw new ArgumentException("Specified filename does not exist in the file table.");

            string SID = string.Format("{0}{1}E{2}", (deny) ? "D" : "A", (isGroup) ? "G" : "U", permissionID.ToString().GetHashCode());
            if (fe.PermEntries.ContainsKey(SID))
                fe.PermEntries[SID].RemovePermValue(permissionValues);
            else
                throw new ArgumentException("Requested user does not have explicit permissions for this resource.");
        }

        public void SaveACL()
        {
            this.SaveACL(false);
        }
        public void SaveACL(bool resetChildren)
        {
            if (this._updUser == Guid.Empty)
                throw new ApplicationException("Update user GUID is empty. Please specify the GUID of the user responsible for updating the ACL before saving.");

            try
            {
                using (FileStream fs = new FileStream(this.ACLFilePath, FileMode.Create, FileAccess.Write))
                using (BinaryWriter br = new BinaryWriter(fs))
                {
                    this.WriteACLHeader(br);
                    this.WriteACLPerms(br);
                    this.WriteFAT(br);
                    this.WriteFATPerms(br);
                }

                string[] subDirs = Directory.GetDirectories(this._fldPath, "*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < subDirs.Length; i++)
                    this.PropogateToChildFolders(subDirs[i], this, resetChildren);
            }
            catch
            { throw; }
        }

        public void SetUpdateUser(object userID)
        {
            this.SetUpdateUser(this.TestForGuid(userID));
        }
        public void SetUpdateUser(Guid userId)
        {
            this._updUser = userId;

            if (this._fldCreator == Guid.Empty)
                this._fldCreator = userId;

            if (this._fldOwner == Guid.Empty)
                this._fldOwner = userId;
        }

        public void TakeOwnership()
        {
            this.SetOwner(this._curUser);
        }

        public FolderACLPermission GetEffectivePermission(object userID, string[] groups)
        {
            return this.GetEffectivePermission(this.TestForGuid(userID), groups);
        }
        public FolderACLPermission GetEffectivePermission(Guid userID, string[] groups)
        {
            FolderACLPermissionValue effVal = this.GetEffectivePermission(this._permCol, userID, groups);
            return new FolderACLPermission(userID, effVal, false);
        }

        public FolderACLPermission GetEffectiveFilePermission(string filename, object userID, string[] groups)
        {
            return this.GetEffectiveFilePermission(filename, this.TestForGuid(userID), groups);
        }
        public FolderACLPermission GetEffectiveFilePermission(string filename, Guid userID, string[] groups)
        {
            FolderACLFileEntry fe = this._fat[filename];

            if (!File.Exists(this.ACLFilePath) || fe == null)
            {
                // If the file doesn't exist in the FileTable, or no ACL file exists
                //   in this folder, then return the folder-level permissions.
                return GetEffectivePermission(userID, groups);
            }
            else
            {
                FolderACLPermissionValue effVals = this.GetEffectivePermission(fe.PermEntries, userID, groups);
                return new FolderACLPermission(userID, effVals);
            }
        }

        public FolderACLPermission[] GetAllPermissions()
        {
            return this._permCol.GetAllPermissions();
        }

        public FolderACLPermissionEntry[] GetAllPermissionEntries()
        {
            return this._permCol.ToArray();
        }
        public FolderACLPermissionEntry[] GetAllPermissionEntries(object permID)
        {
            return this._permCol.GetAllPermEntries(permID);
        }

        public FolderACLPermission[] GetAllFilePermissions(string filename)
        {
            FolderACLFileEntry fe = this._fat[filename];
            if (fe == null)
                // If there's no entry for the file in the FileTable, just return
                //   the folder-level permissions.
                return this._permCol.GetAllPermissions();
            else
                return fe.PermEntries.GetAllPermissions();
        }

        public bool IsPermissionInherited(object permID, FolderACLPermissionValue permValue, bool deny)
        {
            string uSID = string.Format("{0}UE{1}", (deny) ? "D" : "A", permID.ToString().GetHashCode());
            string gSID = string.Format("{0}GE{1}", (deny) ? "D" : "A", permID.ToString().GetHashCode());
            if ((!this._permCol.ContainsKey(uSID) && !this._permCol.ContainsKey(gSID)) || (((this._permCol[uSID].PermValues & permValue) != permValue) && ((this._permCol[gSID].PermValues & permValue) != permValue)))
            {
                // If we didn't find any explicit permissions for this user,
                //   look for any inherited permissions.
                string uSID2 = string.Format("{0}UI{1}", (deny) ? "D" : "A", permID.ToString().GetHashCode());
                string gSID2 = string.Format("{0}GI{2}", (deny) ? "D" : "A", permID.ToString().GetHashCode());
                if (!this._permCol.ContainsKey(uSID2) && !this._permCol.ContainsKey(gSID2))
                    // If the identity (permID) doesn't contain any keys in the
                    //   permissions collection, then the requested permission
                    //   obviously isn't inherited, so we return false.
                    return false;
                else if (this._permCol.ContainsKey(uSID2))
                {
                    // We check for a user-level permission first.
                    return ((this._permCol[uSID2].PermValues & permValue) == permValue);
                }
                else
                {
                    // Then we look for a group permission.
                    return ((this._permCol[gSID2].PermValues & permValue) == permValue);
                }
            }
            else
                // If we hit here, it means with found the permission in either an
                //   explicit user or group permission entry.
                return false;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            if (disposing)
            {
                this._permCol.Clear();
                this._fat.Clear();
            }

            this._disposed = true;
        }
        private Guid TestForGuid(object guidObj)
        {
            Guid tst = Guid.Empty;
            if (guidObj.GetType().Name.ToLower() == "guid")
                tst = (Guid)guidObj;
            else
                try
                { tst = new Guid(guidObj.ToString()); }
                catch { }
            if (tst != Guid.Empty)
                return tst;
            else
                throw new ArgumentException("Argument must be an object of type System.Guid, or a value which can be parsed into a System.Guid object.");
        }

        private void SetFolderPath(string folderPath)
        {
            try
            {
                string tstPath = folderPath;
                if (!Path.IsPathRooted(tstPath))
                    tstPath = Path.GetFullPath(folderPath);

                if (!Directory.Exists(tstPath))
                    throw new DirectoryNotFoundException("The specified directory path was not found.");

                //if (this._br != null)
                //    this._br.Close();
                //if (this._fs != null)
                //    this._fs.Dispose();

                this._fldPath = tstPath;
                //this._fs = new FileStream(this.ACLFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //this._br = new BinaryReader(fs);
            }
            catch
            { throw; }
        }

        private void SetOwner(object userID)
        {
            this.SetOwner(this.TestForGuid(userID));
        }
        private void SetOwner(Guid userID)
        {
            this._fldOwner = userID;
            this.UpdateStats();
        }

        internal void UpdateStats()
        {
            this.SetUpdateUser(this._curUser);
            this._updDate = DateTime.Now;
            if (this._autosave && this._updUser != Guid.Empty)
                this.SaveACL();
        }

        private void ReadACL()
        {
            // If no "perm.acl" file exists in this folder, then just assume we're
            //   creating a new file.
            if (!File.Exists(this.ACLFilePath))
            {
                // If we're creating a new file from scratch, let's check one
                //   folder back for a perm.acl file that we might need to
                //   inherit permissions from.
                string pntFldr = this._fldPath.Substring(0, this._fldPath.LastIndexOf('\\'));
                string pntAcl = Path.Combine(pntFldr, aclFileDefName);
                if (File.Exists(pntAcl))
                {
                    FolderACL pacl = new FolderACL(pntFldr, this._curUser);
                    foreach (FolderACLPermissionEntry pe in pacl.GetAllPermissionEntries())
                        this._permCol.AddPermission(pe.PermissionID, pe.IsGroup, pe.PermValues, pe.IsDeny, (ushort)(pe.InheritenceLevel + 1));
                }
                return;
            }

            try
            {
                this._reading = true;
                using (FileStream fs = new FileStream(this.ACLFilePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    this.ReadACLHeader(br);
                    this.ReadACLPerms(br);
                    this.ReadFAT(br);
                }
            }
            catch
            { throw; }
            finally
            {
                this._reading = false;
            }
        }

        private void ReadACLHeader(BinaryReader br)
        { this.ReadACLHeader(br, false); }
        private void ReadACLHeader(BinaryReader br, bool resetStreamPosition)
        {
            // Record the current stream posiiton.
            long strmPos = br.BaseStream.Position;
            try
            {
                // If the stream's current position isn't at the beginning of the
                //   file, seek to the first byte in the stream.
                if (strmPos != 0)
                    br.BaseStream.Seek(0, SeekOrigin.Begin);

                // Get the file identification string. This should always be "*ACL".
                //   Any other value indicates that this is not the expected file
                //   type.
                string flID = Encoding.ASCII.GetString(br.ReadBytes(4));

                // Get the hash of the folder we're working in and also the ACL
                //   folder hash from the permissions file and compare them.
                // If they don't match, it means this permissions file doesn't
                //   belong to this folder.
                int curFldHash = this._fldPath.GetHashCode();
                int aclFldHash = br.ReadInt32();
                if (curFldHash != aclFldHash)
                    throw new InvalidDataException("Specified folder's ACL belongs to a different folder.");

                // Read the folder owner's GUID.
                this._fldOwner = new Guid(br.ReadBytes(16));

                // Read the folder creator's GUID.
                this._fldCreator = new Guid(br.ReadBytes(16));

                // Read the GUID of the last user to update the ACL and the
                //   DateTime value when that update occured.
                this._updUser = new Guid(br.ReadBytes(16));
                this._updDate = DateTime.FromBinary(br.ReadInt64());

                // Finally, read the FAT size, permission count, version number,
                //   and whether or not this ACL is inheriting from it's parent.
                this._fatSz = br.ReadInt64();
                this._permCnt = br.ReadInt32();
                this._verMajor = br.ReadInt16();
                this._verMinor = br.ReadInt16();
                this._inherits = br.ReadBoolean();
            }
            catch
            { throw; }
            finally
            {
                // Reset the stream position if requested.
                if (resetStreamPosition && br.BaseStream.CanSeek)
                    br.BaseStream.Seek(strmPos, SeekOrigin.Begin);
                else
                    // If we're not restoring the stream position, the caller might
                    //   expect the stream to be at the end of the header when we
                    //   return, so skip the next 47 bytes.
                    br.ReadBytes(47);
            }
        }

        private void ReadACLPerms(BinaryReader br)
        { this.ReadACLPerms(br, false); }
        private void ReadACLPerms(BinaryReader br, bool resetStreamPosition)
        {
            // Record the current stream position.
            long strmPos = br.BaseStream.Position;
            try
            {
                // Make sure there's data beyond the header. This will prevent
                //   throwing an exception if there are no permission entries.
                if (br.BaseStream.Length < 129)
                    return;

                // Move the stream to the first byte after the header, if it's
                //   not already there.
                if (strmPos != 128)
                    br.BaseStream.Seek(128, SeekOrigin.Begin);

                // We have to loop as many times as there are supposed to be
                //   permission entries for this folder in the ACL file.
                for (int i = 0; i < this._permCnt; i++)
                    this._permCol.Add(FolderACL.ReadPermissionEntry(br));
            }
            catch
            { throw; }
            finally
            {
                if (resetStreamPosition && br.BaseStream.CanSeek)
                    br.BaseStream.Seek(strmPos, SeekOrigin.Begin);
            }
        }

        private void ReadFAT(BinaryReader br)
        { this.ReadFAT(br, false); }
        private void ReadFAT(BinaryReader br, bool resetStreamPosition)
        {
            // Record the stream position.
            long strmPos = br.BaseStream.Position;
            long fatBytesRead = 0;
            int hdrSz = (this._permCnt * 74) + 128;
            try
            {
                // Check to make sure we're not at the end of the stream, which
                //   would indicate that there is no FAT data.
                if (hdrSz > br.BaseStream.Length)
                    return;

                // Set the stream position to the end of the header and permission
                //   entries, if it's not already there.
                if (strmPos != hdrSz)
                    br.BaseStream.Seek(hdrSz, SeekOrigin.Begin);

                // Keep looping until we read the entire FAT
                while (fatBytesRead < this._fatSz)
                {
                    // The first 4 (four) bytes indicate the size of the FAT entry.
                    // NOTE: Current format spec signifies that this value is
                    //   unnescessary and will always be the value of bytes 5-8
                    //   plus 12, but the value was left in the specification so
                    //   that future versions can include more info in the FAT
                    //   entry without breaking the existing ACL perser.
                    int entrySz = br.ReadInt32();

                    // Bytes 5-8 indicate the number of bytes used to store the
                    //   filename (2 bytes-per-char).
                    int fnSz = br.ReadInt32();

                    // Now we read the number of bytes designated by the previous
                    //   4 (four) bytes and use the Unicode encoder to convert
                    //   them to a string value.
                    byte[] btFn = br.ReadBytes(fnSz);
                    string fileName = Encoding.Unicode.GetString(btFn);

                    // Then comes a long value (8 bytes) containing the byte position
                    //   where this file's permission entries begin in the ACL file.
                    long filePermPos = br.ReadInt64();

                    // Finally, an Int32 value (4 bytes) containing the number of
                    //   permission entries to read starting at the address found
                    //   in the previous value.
                    int filePermCnt = br.ReadInt32();

                    // Add a new FolderACLFileEntry object to the FolderACL's FAT.
                    FolderACLFileEntry fileEntry = new FolderACLFileEntry(fileName, filePermPos, filePermCnt);
                    this._fat.Add(fileEntry);

                    // Make the new FileEntry load it's permissions.
                    fileEntry.LoadPerms(br, true);

                    // Record the number of bytes just read for this FAT entry.
                    fatBytesRead += entrySz;
                }
            }
            catch
            { throw; }
            finally
            {
                if (resetStreamPosition && br.BaseStream.CanSeek)
                    br.BaseStream.Seek(strmPos, SeekOrigin.Begin);
            }
        }

        private void WriteACLHeader(BinaryWriter br)
        {
            // Write the file identification string.
            br.Write(Encoding.ASCII.GetBytes("*ACL"));

            // Write the hash code of the folder name.
            br.Write(this._fldPath.GetHashCode());

            // Write the folder owner guid. If no owner guid was specified,
            //   then use the current update user's guid.
            br.Write((this._fldOwner != Guid.Empty)
                        ? this._fldOwner.ToByteArray()
                        : this._updUser.ToByteArray());

            // Write the folder creator guid. Again, if no guid was previously
            //   specified, we use the current update user's guid.
            br.Write((this._fldCreator != Guid.Empty)
                        ? this._fldCreator.ToByteArray()
                        : this._updUser.ToByteArray());

            // Write the update user and date/time.
            br.Write(this._updUser.ToByteArray());
            br.Write(this._updDate.ToBinary());

            // Write the FAT size and permission count.
            br.Write(this._fat.GetFileTableSize());
            br.Write(this._permCol.Count);

            // Lastly, write the file version and whether or not this ACL should
            //   inherit permissions from it's parent folder.
            br.Write(this._verMajor);
            br.Write(this._verMinor);
            br.Write(this._inherits);

            // We need to fill the header out to 128 bytes so that it will end
            //   on a DWORD. This also leaves room to add other data to the header
            //   for future format versions.
            br.Write(new byte[47]);
        }

        private void WriteACLPerms(BinaryWriter br)
        {
            // Loop through each permission entry in the collection.
            for (int i = 0; i < this._permCol.Count; i++)
                FolderACL.WritePermissionEntry(br, this._permCol[i]);
        }

        private void WriteFAT(BinaryWriter br)
        {
            // We need to keep track of how much binary data is going to be
            //   reserved for each FileEntry's permission set.
            long permBtCnt = 0;

            // Determine the size of the FAT so we can calculate the offset of the
            //   file permission entries in the stream.
            long fTblSz = this._fat.GetFileTableSize();

            // Loop through each entry in the FileTable and write the entries
            //   to the binary stream.
            for (int i = 0; i < this._fat.Count; i++)
            {
                FolderACLFileEntry fatEntry = this._fat[i];

                // The first 4 (four) bytes determine the total length of the FAT entry.
                br.Write(fatEntry.GetFATEntrySize());

                // We want to convert the entry's filename to a Unicode-base byte array.
                byte[] btFn = Encoding.Unicode.GetBytes(fatEntry.FileName);

                // The next 4 (four) bytes indicate the number of bytes used to store the
                //   filename in Unicode format.
                br.Write(btFn.Length);

                // Write the Unicode bytes for the filename.
                br.Write(btFn);

                // Then comes 8 (eight) bytes specifying the byte position where this
                //   entry's permissions begin in the ACL file.
                long permPos = permBtCnt + fTblSz + (this._permCol.Count * 74) + 128;
                br.Write(permPos);

                // Finally, an Int32 value (4 bytes) containing the number of
                //   permission entries to read starting at the address writen to the
                //   previous 8 bytes.
                br.Write(fatEntry.PermissionCount);

                // Keep track of how many bytes to reserverve for this FileEntry's
                //   permissions.
                permBtCnt += (fatEntry.PermissionCount * 74);
            }
        }

        private void WriteFATPerms(BinaryWriter br)
        {
            // Loop through each entry in the FileTable and write the file's
            //   permissions to the stream.
            for (int f = 0; f < this._fat.Count; f++)
                for (int p = 0; p < this._fat[f].PermEntries.Count; p++)
                    FolderACL.WritePermissionEntry(br, this._fat[f].PermEntries[p]);
        }

        private void PropogateToChildFolders(string folderPath, FolderACL parentACL, bool reset)
        {
            try
            {
                FolderACL acl = new FolderACL(folderPath, this._curUser);
                acl.AutoSave = false;
                if (acl.InheritsParentACL || reset)
                {
                    if (reset)
                        acl._permCol.Clear();
                    else
                        acl.ClearInheritedPerms();

                    for (int i = 0; i < parentACL._permCol.Count; i++)
                    {
                        FolderACLPermissionEntry pe = parentACL._permCol[i];
                        acl._permCol.AddPermission(pe.PermissionID, pe.IsGroup, pe.PermValues, pe.IsDeny, (ushort)(pe.InheritenceLevel + 1));
                    }
                    acl.SaveACL(reset);
                }
            }
            catch
            { throw; }
        }

        private void ClearInheritedPerms()
        {
            try
            {
                List<FolderACLPermissionEntry> pEntryList = new List<FolderACLPermissionEntry>();
                for (int i = 0; i < this._permCol.Count; i++)
                    if (!this._permCol[i].IsInherited)
                    {
                        FolderACLPermissionEntry pe = (FolderACLPermissionEntry)this._permCol[i].Clone();
                        pEntryList.Add(pe);
                    }

                this._permCol.Clear();
                for (int i = 0; i < pEntryList.Count; i++)
                    this._permCol.Add(pEntryList[i]);
            }
            catch
            { throw; }
        }

        private FolderACLPermissionValue GetEffectivePermission(FolderACLPermissionEntryCollection permCol, Guid userID, string[] groups)
        {
            //FolderACLPermissionEntry effPerm = new FolderACLPermissionEntry(userID.ToString(), 0, 2, 2, 0);
            FolderACLPermissionValue effVal = FolderACLPermissionValue.None;
            if (this._permCol.Count < 1 && !File.Exists(this.ACLFilePath))
            {
                // If there is no actual ACL file in existance, just give
                //   everyone "Read" permission.
                effVal |= FolderACLPermissionValue.Read;
            }
            else
            {
                // Compile a list of all permission entry keys which match the
                //   given userID and group list. These will be processed in the
                //   following order:
                //      1. Any permissions for the "Everyone" group.
                //      2. Permissions for the supplied group list.
                //      3. Permissions for the specific user.
                // Add permissions will be processed before Deny permissions for
                //   each of the steps listed above.
                List<string> keys = new List<string>();
                keys.AddRange(permCol.GetGroupEntryList("Everyone"));
                for (int g = 0; g < groups.Length; g++)
                    keys.AddRange(permCol.GetGroupEntryList(groups[g]));

                // Loop through all the collected keys and determine the effectual
                //   permissions.
                for (int i = 0; i < keys.Count; i++)
                {
                    FolderACLPermissionEntry pe = permCol[keys[i]];
                    if (pe.IsDeny)
                        effVal ^= pe.PermValues;
                    else
                        effVal |= pe.PermValues;
                }

                keys.Clear();
                keys.AddRange(permCol.GetUserEntryList(userID));
                // Loop through all the collected keys and determine the effectual
                //   permissions.
                for (int i = 0; i < keys.Count; i++)
                {
                    FolderACLPermissionEntry pe = permCol[keys[i]];
                    if (pe.IsDeny)
                        effVal ^= pe.PermValues;
                    else
                        effVal |= pe.PermValues;
                }
            }

            // The folder owner *always* has the ability to change permissions.
            if (this._fldOwner.ToString() == userID.ToString())
                effVal |= FolderACLPermissionValue.ChangePermissions;

            // Any group named "Administrators" is treated special. This group always
            //   has permission to take ownership of a folder.
            if (Array.IndexOf<string>(groups, "Administrators") > -1)
                effVal |= FolderACLPermissionValue.TakeOwnership;

            return effVal;
        }
        //***************************************************************************
        // Static Methods
        // 
        internal static FolderACLPermission[] GetAllEffectivePerms(FolderACLPermissionEntryCollection permCol, bool user, bool group)
        {
            try
            {
                List<String> procPerms = new List<String>();
                List<FolderACLPermission> finalPerms = new List<FolderACLPermission>();

                for (int i = 0; i < permCol.Count; i++)
                {
                    FolderACLPermissionEntry pe = permCol[i];
                    if ((pe.IsGroup == group || pe.IsGroup != user) && !procPerms.Contains(pe.KeyName))
                    {
                        procPerms.Add(pe.KeyName);
                        FolderACLPermissionEntry effPe = (FolderACLPermissionEntry)pe.Clone();

                        // Look for any Inherited/Explicit Allow perms opposite
                        //   the one we just found.
                        string sSID = string.Format("A{0}{1}{2}", (pe.IsGroup) ? "G" : "U", (pe.IsInherited) ? "E" : "I", pe.SID);
                        if (permCol.ContainsKey(sSID))
                        {
                            procPerms.Add(sSID);
                            effPe.AddPermValue(permCol[sSID].PermValues);
                        }

                        // Look for any Inherited Deny perms with the same SID.
                        string iDenySID = string.Format("D{0}I{1}", (pe.IsGroup) ? "G" : "U", pe.SID);
                        if (permCol.ContainsKey(iDenySID))
                        {
                            procPerms.Add(iDenySID);
                            effPe.RemovePermValue(permCol[iDenySID].PermValues);
                        }

                        // Look for any Explicit Deny perms with the same SID.
                        string eDenySID = string.Format("D{0}E{1}", (pe.IsGroup) ? "G" : "U", pe.SID);
                        if (permCol.ContainsKey(eDenySID))
                        {
                            procPerms.Add(eDenySID);
                            effPe.RemovePermValue(permCol[eDenySID].PermValues);
                        }

                        finalPerms.Add(new FolderACLPermission(pe.PermissionID, pe.PermValues, pe.IsGroup));
                    }
                }
                return finalPerms.ToArray();
            }
            catch
            { throw; }
        }

        internal static void WritePermissionEntry(BinaryWriter br, FolderACLPermissionEntry pEntry)
        {
            try
            {
                // The first two bytes of each perm entry is the entity type.
                br.Write((ushort)pEntry.EntryType);

                // Next two bytes identify Allow/Deny
                br.Write((ushort)pEntry.EntryAction);

                // Bytes 5 & 6 indicate inheritance. Values greater than one indicate
                //   the permission is inherited. As of version 1.1, the actual
                //   integer value indicates how many levels up the directory tree
                //   the permission inherits from.
                br.Write(pEntry.InheritenceLevel);

                // Bytes 7-71 are the entity name. This must be handled differently
                //   for User entities and Group entities.
                if (pEntry.IsGroup)
                {
                    // Group names are stored in unicode with a maximum length of
                    //   32 characters (64 bytes). Group names are "padded" with
                    //   spaces if they are less than 32 characters before being
                    //   written to the binary ACL file.
                    string grpName = pEntry.PermissionID.PadRight(32, ' ');
                    if (grpName.Length > 32)
                        throw new InvalidDataException("Permission name exceeds maximum length of 32 characters.");

                    br.Write(Encoding.Unicode.GetBytes(grpName));
                }
                else
                {
                    // For user-based permissions, we just save the user's GUID
                    //   as a 16-position byte array.
                    string userID = pEntry.PermissionID;
                    // We have to create a byte array 64 bytes long...
                    byte[] btSID = new byte[64];
                    // Initialize all the bytes to their default value...
                    btSID.Initialize();
                    // And copy the GUID bytes into the first 16 positions.
                    Array.Copy(new Guid(userID).ToByteArray(), btSID, 16);
                    br.Write(btSID);
                }

                // The last 4 bytes are the permission bit array. We stored it this
                //   way in the entity class, so we don't need to do anything but
                //   write the value to the stream.
                br.Write((uint)pEntry.PermValues);
            }
            catch
            { throw; }
        }

        internal static FolderACLPermissionEntry ReadPermissionEntry(BinaryReader br)
        {
            // All permission entries are 74 bytes in length.

            // First two bytes define the entity type
            //   0x02 = User entity
            //   0x04 = Group entity
            ushort entityType = br.ReadUInt16();

            // The next two bytes define the entity's action
            //   0x02 = Allow action
            //   0x04 = Deny action
            ushort entityAction = br.ReadUInt16();

            // Bytes 5 & 6 identify the level of inheritence of this permission.
            //   A value of 0 (zero) indicates that this permission is not inherited,
            //   while a value greater than 0 (zero) defines the number of levels up
            //   the directory tree where this permission inherits from.
            ushort inheritLevel = br.ReadUInt16();

            // Bytes 7-71 identify the SID this permission belongs to.
            string sid = string.Empty;
            if (entityType == (ushort)FolderACLPermissionEntry.ACLPermissionEntryType.User)
            {
                sid = new Guid(br.ReadBytes(16)).ToString();
                // There's 48 extra "null" bytes we need to skip.
                br.ReadBytes(48);
            }
            else if (entityType == (ushort)FolderACLPermissionEntry.ACLPermissionEntryType.Group)
            {
                // For group entities, we just read the 64 bytes, run it through the
                //   Unicode decoder, save it to a string and trim any whitespace.
                sid = Encoding.Unicode.GetString(br.ReadBytes(64)).Trim();
            }
            else
                throw new InvalidDataException("Permission entry contained an invalid ACLPermissionEntryType value.");

            // Finally, the last 4 bytes are the actual permission bit array.
            uint permVals = br.ReadUInt32();

            return new FolderACLPermissionEntry(sid, permVals, entityType, entityAction, inheritLevel);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void Col_onCleared(object sender, EventArgs e)
        {
            if (!this._reading)
                this.UpdateStats();
        }

        private void Col_onModified(object sender, CollectionEventArgs e)
        {
            if (!this._reading)
                this.UpdateStats();
        }
        #endregion
    }
    /// <summary>
    /// Stores ACL permission information for a given folder and account object.
    /// </summary>
    public struct FolderACLPermission
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FolderACLPermissionValue
            _permVals;
        private string
            _permName;
        private bool
            _grp;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string PermName
        { get { return this._permName; } }
        public int SID
        { get { return this.PermName.GetHashCode(); } }
        /// <summary>
        /// Gets a <see cref="T:System.Guid"/> value uniquely identify the user whose permission are stored in this instance. If this instance references a group object, the Guid.Empty value is returned.
        /// </summary>
        public Guid UserID
        { get { return (this._grp) ? Guid.Empty : new Guid(this._permName); } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Read" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanRead
        {
            get
            {
                return ((this._permVals & FolderACLPermissionValue.Read) == FolderACLPermissionValue.Read)
                        || this.CanWrite 
                        || this.CanModify 
                        || this.CanAppend
                        || this.CanExecute 
                        || this.CanChangeAttributes;
        } 
        }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Write" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return ((this._permVals & FolderACLPermissionValue.Write) == FolderACLPermissionValue.Write)
                        || this.CanModify
                        || this.CanAppend
                        || this.CanChangeAttributes
                        || this.CanDelete;
            }
        }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Modify" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanModify
        {
            get
            {
                return ((this._permVals & FolderACLPermissionValue.Modify) == FolderACLPermissionValue.Modify)
                        || this.CanDelete
                        || this.CanChangeAttributes
                        || this.CanAppend;
            }
        }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Append" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanAppend
        { get { return (this._permVals & FolderACLPermissionValue.Append) == FolderACLPermissionValue.Append; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Execute" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanExecute
        { get { return (this._permVals & FolderACLPermissionValue.Execute) == FolderACLPermissionValue.Execute; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Delete" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanDelete
        { get { return (this._permVals & FolderACLPermissionValue.Delete) == FolderACLPermissionValue.Delete; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Change Attributes" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanChangeAttributes
        { get { return (this._permVals & FolderACLPermissionValue.ChangeAttributes) == FolderACLPermissionValue.ChangeAttributes; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Read Permissions" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanReadPermissions
        { get { return (this._permVals & FolderACLPermissionValue.ReadPermissions) == FolderACLPermissionValue.ReadPermissions; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Change Permissions" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanChangePermissions
        { get { return (this._permVals & FolderACLPermissionValue.ChangePermissions) == FolderACLPermissionValue.ChangePermissions; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this object contains "Take Ownership" permissions. Otherwise, 'false'.
        /// </summary>
        public bool CanTakeOwnership
        { get { return (this._permVals & FolderACLPermissionValue.TakeOwnership) == FolderACLPermissionValue.TakeOwnership; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FolderACLPermission(object permName, FolderACLPermissionValue permValues, bool isGroup)
        {
            this._permName = permName.ToString();
            this._permVals = permValues;
            this._grp = isGroup;
        }
        internal FolderACLPermission(string groupName, FolderACLPermissionValue permValues)
            : this(groupName, permValues, true)
        { }
        internal FolderACLPermission(Guid userID, FolderACLPermissionValue permValues)
            : this(userID, permValues, false)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Override Methods
        // 
        /// <summary>
        /// Returns a <see cref="T:System.String"/> representation of of the folder permissions stored in this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this._permName + " ");
            if (this.CanRead)
                sb.Append("R");
            if (this.CanWrite)
                sb.Append("W");
            if (this.CanModify)
                sb.Append("M");
            if (this.CanAppend)
                sb.Append("A");
            if (this.CanDelete)
                sb.Append("D");
            if (this.CanExecute)
                sb.Append("E");
            if (this.CanChangeAttributes)
                sb.Append("Ca");
            if (this.CanReadPermissions)
                sb.Append("Rp");
            if (this.CanChangePermissions)
                sb.Append("Cp");
            if (this.CanTakeOwnership)
                sb.Append("T");
            return sb.ToString().Trim();
        }
        #endregion
    }
    public sealed class FolderACLPermissionEntry : ICloneable
    {
        #region NestedTypes
        //***************************************************************************
        // Nested Enums
        // 
        /// <summary>
        /// Defines the type of permission represented by this instance.
        /// </summary>
        public enum ACLPermissionEntryType : ushort
        {
            User = 0x02,
            Group = 0x04
        }
        /// <summary>
        /// Defines the action represented by this instance, either 'allow' or 'deny'.
        /// </summary>
        public enum ACLPermissionEntryAction : ushort
        {
            Allow = 0x02,
            Deny = 0x04
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FolderACLPermissionEntryCollection
            _owner;
        private string
            _permName;
        private FolderACLPermissionValue
            _permVals;
        private ACLPermissionEntryType
            _unitType;
        private ACLPermissionEntryAction
            _unitAction;
        private ushort
            _inheritLvl;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FolderACLPermissionEntryCollection Owner
        { get { return this._owner; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this is permission represent a 'deny' action. Otherwise, 'false'.
        /// </summary>
        public bool IsDeny
        { get { return (this._unitAction == ACLPermissionEntryAction.Deny); } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' if this permission object is for a group object instead of a specific user. Otherwise, 'false'.
        /// </summary>
        public bool IsGroup
        { get { return (this._unitType == ACLPermissionEntryType.Group); } }
        public bool IsInherited
        { get { return (this._inheritLvl > 0); } }
        /// <summary>
        /// Gets a <see cref="T:System.UnsignedInt16"/> value indicating how many levels above this permission's parent object this permission is inherited from.
        /// </summary>
        public ushort InheritenceLevel
        { get { return this._inheritLvl; } }
        /// <summary>
        /// Gets a <see cref="T:System.String"/> value indicating the name of the permission object that this permission is inherited from.
        /// </summary>
        public string InheritsFrom
        {
            get
            {
                if (this._inheritLvl < 1)
                    return string.Empty;
                else
                {
                    object parent = this._owner.Owner;
                    string pType = parent.GetType().Name;

                    string fldPath = "";
                    if (pType == "FolderACL")
                        fldPath = ((FolderACL)parent).FolderPath;
                    else if (pType == "FolderACLFileEntry")
                        fldPath = ((FolderACLFileEntry)parent).FATOwner.Owner.FolderPath;
                    else
                        throw new ApplicationException("This collection does not inherit from a known Type.");

                    string[] pathPcs = fldPath.Split('\\');
                    if (this._inheritLvl > (pathPcs.Length - 1))
                        throw new ApplicationException("Current folder path does not allow for permission's level in inheritance.");

                    if (pathPcs.Length > 1)
                    {
                        string finalPath = "";
                        //for (int i = 0; i < pathPcs.Length - this._inheritLvl; i++)
                        //    finalPath += pathPcs[i] + "\\";
                        for (int i = pathPcs.Length - (this._inheritLvl + 1); i < pathPcs.Length; i++)
                            finalPath += pathPcs[i] + "\\";
                        return finalPath.TrimEnd('\\');
                    }
                    else
                        return fldPath;
                }
            }
        }
        public bool Read
        { get { return (this._permVals & FolderACLPermissionValue.Read) == FolderACLPermissionValue.Read; } }
        public bool Write
        { get { return (this._permVals & FolderACLPermissionValue.Write) == FolderACLPermissionValue.Write; } }
        public bool Modify
        { get { return (this._permVals & FolderACLPermissionValue.Modify) == FolderACLPermissionValue.Modify; } }
        public bool Append
        { get { return (this._permVals & FolderACLPermissionValue.Append) == FolderACLPermissionValue.Append; } }
        public bool Execute
        { get { return (this._permVals & FolderACLPermissionValue.Execute) == FolderACLPermissionValue.Execute; } }
        public bool Delete
        { get { return (this._permVals & FolderACLPermissionValue.Delete) == FolderACLPermissionValue.Delete; } }
        public bool ChangeAttributes
        { get { return (this._permVals & FolderACLPermissionValue.ChangeAttributes) == FolderACLPermissionValue.ChangeAttributes; } }
        public bool ReadPermissions
        { get { return (this._permVals & FolderACLPermissionValue.ReadPermissions) == FolderACLPermissionValue.ReadPermissions; } }
        public bool ChangePermissions
        { get { return (this._permVals & FolderACLPermissionValue.ChangePermissions) == FolderACLPermissionValue.ChangePermissions; } }
        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value indicating 'true' of this instance grants permission to take ownership of its parent object. Otherwise, 'false'.
        /// </summary>
        public bool TakeOwnerShip
        { get { return (this._permVals & FolderACLPermissionValue.TakeOwnership) == FolderACLPermissionValue.TakeOwnership; } }
        /// <summary>
        /// Gets the 'name' of this permission entry instance.
        /// </summary>
        public string PermissionID
        { get { return this._permName; } }
        /// <summary>
        /// Gets a unique identification number for this instance.
        /// </summary>
        public int SID
        { get { return this._permName.GetHashCode(); } }
        /// <summary>
        /// Gets a <see cref="T:System.String"/> value that is used as a collection key by the other FolderACL classes.
        /// </summary>
        public string KeyName
        { get { return ((IsDeny) ? "D" : "A") + ((IsGroup) ? "G" : "U") + ((IsInherited) ? "I" : "E") + this.SID.ToString(); } }
        //***************************************************************************
        // Private Properties
        // 
        internal ACLPermissionEntryType EntryType
        { get { return this._unitType; } }
        internal ACLPermissionEntryAction EntryAction
        { get { return this._unitAction; } }
        internal FolderACLPermissionValue PermValues
        { get { return this._permVals; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FolderACLPermissionEntry(string permID, uint aclPermVals, ushort permUnitType, ushort permUnitAction, ushort inheritLevel)
        {
            this._owner = null;
            this._permName = permID;
            this._permVals = (FolderACLPermissionValue)aclPermVals;
            this._unitType = (ACLPermissionEntryType)permUnitType;
            this._unitAction = (ACLPermissionEntryAction)permUnitAction;
            this._inheritLvl = inheritLevel;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        internal void AddPermValue(FolderACLPermissionValue permValue)
        {
            this._permVals |= permValue;
            this.UpdateParentStats();
        }
        internal void RemovePermValue(FolderACLPermissionValue permValue)
        {
            this._permVals ^= permValue;
            this.UpdateParentStats();
        }
        internal void SetOwner(FolderACLPermissionEntryCollection owner)
        {
            this._owner = owner;
        }
        private void UpdateParentStats()
        {
            object parentOwner = this._owner.Owner;
            if (parentOwner.GetType().Name.ToLower() == "folderacl")
                ((FolderACL)parentOwner).UpdateStats();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates an exact clone of this instance.
        /// </summary>
        /// <returns>A <see cref="T:RainstormStudios.FolderACLPermissionEntry"/> object.</returns>
        public object Clone()
        {
            object peClone = this.MemberwiseClone();
            ((FolderACLPermissionEntry)peClone).SetOwner(null);
            return peClone;
        }
        #endregion
    }
    public sealed class FolderACLPermissionEntryCollection : RainstormStudios.Collections.ObjectCollectionBase<FolderACLPermissionEntry>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object Owner
        { get { return this._owner; } }
        public new FolderACLPermissionEntry this[int index]
        {
            get { return (FolderACLPermissionEntry)base[index]; }
            set { base[index] = value; }
        }
        public new FolderACLPermissionEntry this[string key]
        {
            get { return (FolderACLPermissionEntry)base[key]; }
            set { base[key] = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FolderACLPermissionEntryCollection(object aclOwner)
            : base()
        {
            this._owner = aclOwner;
        }
        private FolderACLPermissionEntryCollection(FolderACLPermissionEntry[] vals, string[] keys, object aclOwner)
            : this(aclOwner)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                string valKey = (keys.Length < i) ? keys[i] : "";
                this.Add(vals[i], valKey);
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(FolderACLPermissionEntry value)
        {
            if (value.Owner == null)
                value.SetOwner(this);
            else if (this.Owner != null && value.Owner != this)
                throw new ArgumentException("Permission entry belongs to another permissions collection.");
            base.Add(value, value.KeyName);
        }
        public void Insert(int index, FolderACLPermissionEntry value)
        {
            if (value.Owner == null)
                value.SetOwner(this);
            else if (this.Owner != null && value.Owner != this)
                throw new ArgumentException("Permission entry belongs to another permissions collection.");
            base.Insert(index, value, value.KeyName);
        }
        public void AddPermission(object permID, bool isGrp, FolderACLPermissionValue permVals, bool deny, ushort inheritLvl)
        {
            try
            {
                string SID = string.Format("{0}{1}{2}{3}", (deny) ? "D" : "A", (isGrp) ? "G" : "U", (inheritLvl > 0) ? "I" : "E", permID.ToString().GetHashCode());
                if (this.ContainsKey(SID) && this[SID].IsInherited == (inheritLvl > 0))
                {
                    // We already have a permission entry for the specified
                    //   user/group, so we just add this permission to that entry.
                    this[SID].AddPermValue(permVals);
                }
                else
                {
                    // We don't have a permission entry yet, so we need to create
                    //   a new one from scratch.
                    this.Add(new FolderACLPermissionEntry(
                        permID.ToString(),
                        (uint)permVals,
                        (ushort)((isGrp)
                                    ? FolderACLPermissionEntry.ACLPermissionEntryType.Group
                                    : FolderACLPermissionEntry.ACLPermissionEntryType.User),
                        (ushort)((deny)
                                    ? FolderACLPermissionEntry.ACLPermissionEntryAction.Deny
                                    : FolderACLPermissionEntry.ACLPermissionEntryAction.Allow),
                        inheritLvl));
                }
            }
            catch
            { throw; }
        }
        public new FolderACLPermissionEntry[] ToArray()
        {
            return this.ToArray(0, this.List.Count);
        }
        public new FolderACLPermissionEntry[] ToArray(int offset, int length)
        {
            return Array.ConvertAll<object, FolderACLPermissionEntry>(base.ToArray(offset, length), new Converter<object, FolderACLPermissionEntry>(this.CastObj));
        }
        public string[] GetUserEntryList(object userID)
        {
            List<string> keyList = new List<string>();
            for (int i = 0; i < this._keys.Count; i++)
            {
                string keyVal = this._keys[i].ToString();
                if (keyVal.Substring(1, 1) == "U" && keyVal.Substring(3) == userID.ToString().GetHashCode().ToString())
                    keyList.Add(this._keys[i].ToString());

            }
            keyList.Sort();
            return keyList.ToArray();
        }
        public string[] GetGroupEntryList(string groupName)
        {
            List<string> keyList = new List<string>();
            for (int i = 0; i < this._keys.Count; i++)
            {
                string keyVal = this._keys[i].ToString();
                if (keyVal.Substring(1, 1) == "G" && keyVal.Substring(3) == groupName.GetHashCode().ToString())
                    keyList.Add(this._keys[i].ToString());
            }
            keyList.Sort();
            return keyList.ToArray();
        }
        public string[] GetAllEntryList(object permID)
        {
            List<string> keyList = new List<string>();
            for (int i = 0; i < this.Count; i++)
                if (this._keys[i].ToString().Substring(3) == permID.ToString().GetHashCode().ToString())
                    keyList.Add(this._keys[i].ToString());
            keyList.Sort();
            return keyList.ToArray();
        }
        public FolderACLPermissionEntry[] GetAllPermEntries(object permID)
        {
            List<FolderACLPermissionEntry> permList = new List<FolderACLPermissionEntry>();
            for (int i = 0; i < this.Count; i++)
                if (this._keys[i].ToString().Substring(3) == permID.ToString().GetHashCode().ToString())
                    permList.Add(this[i]);
            return permList.ToArray();
        }
        public string[] GetAllEntryList(object userID, string[] groups)
        {
            List<string> keyList = new List<string>();
            keyList.AddRange(this.GetUserEntryList(userID));
            for (int i = 0; i < groups.Length; i++)
                keyList.AddRange(this.GetGroupEntryList(groups[i]));
            keyList.Sort();
            return keyList.ToArray();
        }
        public FolderACLPermission[] GetAllPermissions()
        {
            List<FolderACLPermissionValue> valList = new List<FolderACLPermissionValue>();
            List<string> namList = new List<string>();
            List<bool> grpList = new List<bool>();
            List<int> keyList = new List<int>();

            List<string> colKeyList = new List<string>(Array.ConvertAll<object, string>(this._keys.ToArray(), new Converter<object, string>(Convert.ToString)));
            colKeyList.Sort();
            for (int i = 0; i < colKeyList.Count; i++)
            {
                FolderACLPermissionEntry pe = this[colKeyList[i]];
                if (keyList.Contains(pe.SID))
                {
                    if (pe.IsDeny)
                        valList[keyList.IndexOf(pe.SID)] ^= pe.PermValues;
                    else
                        valList[keyList.IndexOf(pe.SID)] |= pe.PermValues;
                }
                else
                {
                    keyList.Add(pe.SID);
                    namList.Add(pe.PermissionID);
                    grpList.Add(pe.IsGroup);
                    valList.Add(pe.PermValues);
                }
            }

            List<FolderACLPermission> permList = new List<FolderACLPermission>();
            for (int i = 0; i < keyList.Count; i++)
                permList.Add(new FolderACLPermission(namList[i], valList[i], grpList[i]));
            return permList.ToArray();
        }
        public void Sort()
        {
            this.Sort(SortDirection.Ascending);
        }
        public void Sort(SortDirection dir)
        {
            FolderACLPermissionEntry[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<String, FolderACLPermissionEntry>(keys, vals);

            this.Clear();
            for (int i = 0; i < keys.Length; i++)
                this.Add(vals[i], keys[i]);
        }
        public FolderACLPermissionEntryCollection SortClone()
        {
            return this.SortClone(SortDirection.Ascending);
        }
        public FolderACLPermissionEntryCollection SortClone(SortDirection dir)
        {
            FolderACLPermissionEntry[] vals = this.ToArray();
            Array.ForEach<FolderACLPermissionEntry>(vals, new Action<FolderACLPermissionEntry>(this.ClearOwner));
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<String, FolderACLPermissionEntry>(keys, vals);
            return new FolderACLPermissionEntryCollection(vals, keys, null);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private FolderACLPermissionEntry CastObj(object value)
        { return (FolderACLPermissionEntry)value; }
        private void ClearOwner(FolderACLPermissionEntry val)
        {
            val.SetOwner(null);
        }
        #endregion
    }
    public sealed class FolderACLFileEntry
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FolderACLPermissionEntryCollection
            _permCol;
        private FolderACLFileTable
            _owner;
        private string
            _flName;
        private long
            _permPos;
        private int
            _permCnt;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FolderACLFileTable FATOwner
        { get { return this._owner; } }
        public string FileName
        { get { return this._flName; } }
        public string FullFilePath
        { get { return Path.Combine(this._owner.Owner.FolderPath, this._flName); } }
        public long PermissionStreamLocation
        { get { return this._permPos; } }
        public int PermissionCount
        { get { return this._permCnt; } }
        //***************************************************************************
        // Private Properties
        // 
        internal FolderACLPermissionEntryCollection PermEntries
        { get { return this._permCol; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private FolderACLFileEntry()
        {
            this._permCol = new FolderACLPermissionEntryCollection(this);
        }
        internal FolderACLFileEntry(string fileName, long streamPos, int permCount)
            : this()
        {
            this._flName = fileName;
            this._permPos = streamPos;
            this._permCnt = permCount;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Internal Methods
        // 
        internal void SetOwner(FolderACLFileTable owner)
        {
            this._owner = owner;
        }
        internal int GetFATEntrySize()
        {
            return (this._flName.Length * 2) + 4 + 4 + 8 + 4;
        }
        internal void LoadPerms()
        {
            string aclFile = this._owner.Owner.ACLFilePath;
            if (!File.Exists(aclFile))
                throw new ApplicationException("Current ACL file could not be found.");

            try
            {
                using (FileStream fs = new FileStream(aclFile, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                    this.LoadPerms(br, false);
            }
            catch
            { throw; }
        }
        internal void LoadPerms(BinaryReader br, bool resetStreamPosition)
        {
            // Record the stream's position.
            long strmPos = br.BaseStream.Position;
            try
            {
                // First, seek to the start of the file's permission group.
                br.BaseStream.Seek(this._permPos, SeekOrigin.Begin);

                // Then, read the number of perms specified in the FileEntry.
                // Remember: each perm is 74 bytes, just like the folder-level permissions.
                for (int i = 0; i < this._permCnt; i++)
                {
                    this._permCol.Add(FolderACL.ReadPermissionEntry(br));
                }
            }
            catch
            { throw; }
            finally
            {
                if (resetStreamPosition && br.BaseStream.CanSeek)
                    br.BaseStream.Seek(strmPos, SeekOrigin.Begin);
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        #endregion
    }
    public sealed class FolderACLFileTable : RainstormStudios.Collections.ObjectCollectionBase<FolderACLFileEntry>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FolderACL
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FolderACL Owner
        { get { return this._owner; } }
        public new FolderACLFileEntry this[int index]
        {
            get { return (FolderACLFileEntry)base[index]; }
            set { base[index] = value; }
        }
        public new FolderACLFileEntry this[string key]
        {
            get
            {
                return (this.ContainsKey(key))
                          ? (FolderACLFileEntry)base[key]
                          : null;
            }
            set { base[key] = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FolderACLFileTable(FolderACL owner)
            : base()
        {
            this._owner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(FolderACLFileEntry value)
        {
            if (value.FATOwner == null)
                value.SetOwner(this);
            else if (value.FATOwner != this)
                throw new ArgumentException("File entry belongs to another FolderACLFileTable object.");
            base.Add(value, value.FileName);
        }
        public long GetFileTableSize()
        {
            int fatSz = 0;
            for (int i = 0; i < this.Count; i++)
                fatSz += this[i].GetFATEntrySize();
            return fatSz;
        }
        #endregion
    }
}
