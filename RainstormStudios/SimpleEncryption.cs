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
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios
{
    /// <summary>
    /// Provides a simplified mechanism for performing string encryption and decrption using .NET's RSA cryptography engine.
    /// </summary>
    [Author("Unfried, Michael")]
    public class SimpleEncryption
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        ASCIIEncoding ByteConverter = new ASCIIEncoding();
        byte[]
            _clrBt = null,
            _encBt = null;
        string
            _clrStr,
            _encStr,
            _cspCont;
        bool
            _storeKey = true,
            _oaepPad = true;
        int
            _keyLen;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string KeyContainerName
        {
            get { return _cspCont; }
            set { _cspCont = value; }
        }
        public string UnencryptedString
        {
            get { return _clrStr; }
            set { _clrStr = value; FillClrBytes(); }
        }
        public byte[] UnencryptedBytes
        {
            get { return _clrBt; }
            set { _clrBt = value; MakeClrString(); }
        }
        public string EncryptedString
        {
            get { return _encStr; }
            set { _encStr = value; FillEncBytes(); }
        }
        public byte[] EncryptedBytes
        {
            get { return _encBt; }
            set { _encBt = value; MakeEncString(); }
        }
        public bool StoreKeyInMachineKeyCache
        {
            get { return _storeKey; }
            set { _storeKey = value; }
        }
        public bool UseOAEPPadding
        {
            get { return _oaepPad; }
            set { _oaepPad = value; }
        }
        public int KeyLength
        {
            get { return _keyLen; }
            set { _keyLen = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SimpleEncryption()
        {
            this._clrStr = "";
            this._encStr = "";
            this._cspCont = "";
            this._keyLen = 0;
        }
        public SimpleEncryption(string clrStringValue)
        {
            this._clrStr = clrStringValue;
            this.FillClrBytes();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            if (this._clrBt != null)
                Array.Clear(this._clrBt, 0, this._clrBt.Length);
            if (this._encBt != null)
                Array.Clear(this._encBt, 0, this._encBt.Length);
            this._clrStr = string.Empty;
            this._encStr = string.Empty;
            this._cspCont = string.Empty;
            this._keyLen = 0;
            this._storeKey = false;
            this._oaepPad = false;
            this.ByteConverter = null;
        }
        public string Decrypt()
        {
            if (string.IsNullOrEmpty(this._cspCont))
                throw new Exception("No KeyContainerName to find key in local machine key repository.  You must either define the KeyContainerName or provide the Byte[] value containing the 'KeyBlob' needed for asymetrical decryption.");

            return Decrypt(null);
        }
        public string Decrypt(byte[] keyBlob)
        {
            if (string.IsNullOrEmpty(this._encStr) && (this._encBt == null || this._encBt.Length < 1))
                throw new Exception("No encrypted data given to decrypt.");

            if (this._encBt == null || this._encBt.Length < 1)
                this.FillEncBytes();

            CspParameters cspParams = new CspParameters();
            if (!string.IsNullOrEmpty(this._cspCont))
                cspParams.KeyContainerName = this._cspCont;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams))
            {
                rsa.PersistKeyInCsp = this._storeKey;

                if (keyBlob != null)
                    rsa.ImportCspBlob(keyBlob);

                this._clrBt = rsa.Decrypt(this._encBt, this._oaepPad);
                this.MakeClrString();
                return this._clrStr;
            }
        }
        public byte[] Encrypt()
        {
            if (string.IsNullOrEmpty(this._clrStr) && (this._clrBt == null || this._clrBt.Length < 1))
                throw new Exception("No clean data given to encrypt.");

            if (this._clrBt == null || this._clrBt.Length < 1)
                this.FillClrBytes();

            CspParameters cspParams = new CspParameters();
            if (!string.IsNullOrEmpty(this._cspCont))
                cspParams.KeyContainerName = this._cspCont;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams))
            {
                rsa.PersistKeyInCsp = this._storeKey;

                if (string.IsNullOrEmpty(this._cspCont) && this._keyLen > 0)
                {
                    int keySzInc = 8;
                    if (rsa.LegalKeySizes.Length > 0)
                    {
                        keySzInc = rsa.LegalKeySizes[0].SkipSize;
                        if (this._keyLen < rsa.LegalKeySizes[0].MinSize)
                            this._keyLen = rsa.LegalKeySizes[0].MinSize;
                        else if (this._keyLen > rsa.LegalKeySizes[0].MaxSize)
                            this._keyLen = rsa.LegalKeySizes[0].MaxSize;
                    }
                    rsa.KeySize = (int)(keySzInc * Math.Ceiling((double)this._keyLen / keySzInc));
                }

                this._encBt = rsa.Encrypt(this._clrBt, this._oaepPad);
                this.MakeEncString();
                return rsa.ExportCspBlob(true);
            }
        }
        //***************************************************************************
        // Static Methods
        // 
        public static byte[] GetEncrypted(string value)
        {
            return GetEncrypted(value, true);
        }
        public static byte[] GetEncrypted(string value, bool OAEP)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "aosCore";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams))
            {
                rsa.PersistKeyInCsp = true;
                //byte[] valueClr = Array.ConvertAll(value.ToCharArray(), new Converter<Char, Byte>(Convert.ToByte));
                //return rsa.Encrypt(valueClr, OAEP);
                return rsa.Encrypt(Encoding.Unicode.GetBytes(value), OAEP);
            }
        }
        public static string GetDecrypted(byte[] value)
        {
            return GetDecrypted(value, true);
        }
        public static string GetDecrypted(byte[] value, bool OAEP)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "aosCore";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams))
            {
                rsa.PersistKeyInCsp = true;
                byte[] valueClr = rsa.Decrypt(value, OAEP);
                //string retVal = "";
                //foreach (byte bt in valueClr)
                //    retVal += Convert.ToChar(bt).ToString();
                //return retVal;
                return Encoding.Unicode.GetString(value);
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void FillClrBytes()
        {
            this._clrBt = ByteConverter.GetBytes(this._clrStr);
            //this._clrBt = Array.ConvertAll(this._clrStr.ToCharArray(), new Converter<Char, Byte>(Convert.ToByte));
        }
        private void FillEncBytes()
        {
            this._encBt = ByteConverter.GetBytes(this._encStr);
            //this._encBt = Array.ConvertAll(this._encStr.ToCharArray(), new Converter<Char, Byte>(Convert.ToByte));
        }
        private void MakeClrString()
        {
            this._clrStr = ByteConverter.GetString(this._clrBt);
            //this._clrStr = "";
            //foreach (byte bt in this._clrBt)
            //    this._clrStr += Convert.ToChar(bt).ToString();
        }
        private void MakeEncString()
        {
            this._encStr = ByteConverter.GetString(this._encBt);
            //this._clrStr = "";
            //foreach (byte bt in this._encBt)
            //    this._encStr += Convert.ToChar(bt).ToString();
        }
        #endregion
    }
}
