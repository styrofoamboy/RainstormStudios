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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Web.Pop3
{
    public class Pop3MailClient : Pop3MimeClientBase, IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        //***************************************************************************
        // Event Declarations
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Pop3MailClient(string serverName, string userName, byte[] encryptedPassword, byte[] decryptBlob)
            : this(serverName, 110, userName, encryptedPassword, decryptBlob)
        { }
        public Pop3MailClient(string serverName, int port, string userName, byte[] encryptedPassword, byte[] decrpytBlob)
            : this(serverName, port, userName, RainstormStudios.Encryption.rsaEncryption.GetDecrypted(encryptedPassword, decrpytBlob))
        { }
        public Pop3MailClient(string serverName, string userName, string password)
            : this(serverName, 110, userName, password)
        { }
        public Pop3MailClient(string serverName, int port, string userName, string password)
            : base(serverName, port, false, userName, password)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            base.Disconnect();
        }
        public Pop3.RxMailMessage[] GetMessages()
        {
            List<Pop3.RxMailMessage> returnValue = new List<RxMailMessage>();
            List<int> mailIDs = null;
            if (!this.GetEmailIdList(out mailIDs))
                throw new ApplicationException("Unable to retrieve list of message ID's");

            for (int i = 0; i < mailIDs.Count; i++)
            {
                Pop3.RxMailMessage msg = null;
                if (this.GetEmail(mailIDs[i], out msg))
                {
                    msg.MessageId = mailIDs[i].ToString();
                    returnValue.Add(msg);
                }
            }
            return returnValue.ToArray();
        }
        public bool ClearMailbox()
        {
            List<int> mailIDs = null;
            if (!this.GetEmailIdList(out mailIDs))
                throw new ApplicationException("Unable to retrieve list of message ID's");

            bool[] result = new bool[mailIDs.Count];
            for (int i = 0; i < mailIDs.Count; i++)
                result[i] = this.DeleteEmail(mailIDs[i]);

            return Array.TrueForAll<bool>(result, new Predicate<bool>(IsTrue));
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private bool IsTrue(bool value)
        { return value == true; }
        #endregion
    }
}
