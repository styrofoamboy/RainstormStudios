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

namespace RainstormStudios
{
    public struct BinaryByte
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly BinaryByte
            Empty;
        //***************************************************************************
        // Private Fields
        // 
        private byte
            _intVal;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private BinaryByte(byte val)
        { this._intVal = val; }
        private BinaryByte(int val)
            : this((byte)val) { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Base-Class Overrides
        // 
        /// <summary>
        /// Generates a string representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> value used to uniquely identify this instance.</returns>
        public override string ToString()
        {
            return BinaryByte.ToString(this._intVal);
        }
        public override bool Equals(object obj)
        {
            return (
                (obj is BinaryByte && this._intVal == ((BinaryByte)obj)._intVal)
                    ||
                (obj is byte && this._intVal == (byte)obj)
            );
        }
        public override int GetHashCode()
        {
            return this._intVal.GetHashCode();
        }
        //***************************************************************************
        // Static Methods
        // 
        public static RainstormStudios.BinaryByte FromString(string binaryValue)
        {
            if (binaryValue.Length > 8)
                throw new ArgumentException("Specified string value is not a valid binary number representation.");

            for (int i = 0; i < binaryValue.Length; i++)
                if (binaryValue[i] != '0' && binaryValue[i] != '1')
                    throw new ArgumentException("Specified string value has invalid characters for binary representation.");

            byte[] bitMask = new byte[8];
            int retVal;
            for (retVal = 0; retVal < 256; retVal++)
            {
                for (int j = 7; j >= 0; j--)
                {
                    if (bitMask[j] == 0)
                    { bitMask[j] = 1; break; }
                    else
                    { bitMask[j] = 0; }
                }
                if (string.Join("", Array.ConvertAll<byte, string>(bitMask, new Converter<byte, string>(Convert.ToString))) == binaryValue)
                    break;
            }
            return (BinaryByte)retVal;
        }
        public static string ToString(byte binaryValue)
        {
            byte[] bitArray = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < (int)binaryValue; i++)
                for (int j = 7; j >= 0; j--)
                {
                    if (bitArray[j] == 0)
                    { bitArray[j] = 1; break; }
                    else
                    { bitArray[j] = 0; }
                }
            return string.Join("", Array.ConvertAll<byte, string>(bitArray, new Converter<byte, string>(Convert.ToString)));
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public static implicit operator System.Byte(RainstormStudios.BinaryByte b)
        {
            byte newByte = b._intVal;
            return newByte;
        }
        public static implicit operator RainstormStudios.BinaryByte(System.Byte b)
        {
            BinaryByte newVal = new BinaryByte();
            newVal._intVal = b;
            return newVal;
        }
        public static implicit operator RainstormStudios.BinaryByte(System.Int32 i)
        {
            BinaryByte newVal = new BinaryByte();
            newVal._intVal = (byte)i;
            return newVal;
        }
        public static bool operator ==(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return (val1._intVal == val2);
        }
        public static bool operator !=(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return !(val1 == val2);
        }
        public static bool operator >(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return (val1._intVal > val2);
        }
        public static bool operator <(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return (val1._intVal < val2);
        }
        public static bool operator >=(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return (val1._intVal >= val2);
        }
        public static bool operator <=(RainstormStudios.BinaryByte val1, System.Byte val2)
        {
            return (val1._intVal <= val2);
        }
        public static RainstormStudios.BinaryByte op_Assign(byte bVal)
        { return (BinaryByte)bVal; }
        public static RainstormStudios.BinaryByte op_Assign(int iVal)
        { return (BinaryByte)iVal; }
        #endregion
    }
}
