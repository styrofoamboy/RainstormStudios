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
    /// <summary>
    /// Provides static methods for converting between Celcuis and Fahrenheit and vice versa.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class Temperature
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>Converts a Celcius temperature to Fahrenheit.</summary>
        /// <param name="celcius">The Celcius temperature to convert.</param>
        /// <returns>A value of type Double</returns>
        public static double CelciusToFahrenheit(double celcius)
        { return (celcius * 1.8) + 32; }
        /// <summary>Converts a Fahrenheit temperature to Celcius.</summary>
        /// <param name="fahrenheit">The Fahrenheit temperature to convert.</param>
        /// <returns>A value of type Double</returns>
        public static double FahrenheitToCelcius(double fahrenheit)
        { return (fahrenheit - 32) * .5556; }
        #endregion
    }
    /// <summary>
    /// Provides static methods for converting linear measurements.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class LinearMeasurementConversion
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly int
            FeetPerMile = 5280,
            FeetPerYard = 3,
            FeetPerFathom = 6;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static double CentimetersToInches(double centimeters)
        { return centimeters * 0.3937; }
        public static double CentimetersToFeet(double feet)
        { return feet * 0.03281; }
        public static double MetersToFeet(double meters)
        { return meters * 3.2808; }
        public static double MetersToMiles(double meters)
        { return meters * 0.0006214; }
        public static double MetersToYards(double meters)
        { return meters * 1.0936; }
        public static double KilometersToMiles(double kilometers)
        { return kilometers * 0.62139; }
        public static double KilometersToMilesNaut(double kilometers)
        { return kilometers * 0.8684; }
        public static double FathomsToFeet(double fathoms)
        { return fathoms * (double)LinearMeasurementConversion.FeetPerFathom; }
        public static double InchesToCentimeters(double inches)
        { return inches * 2.54; }
        public static double FeetToMeters(double feet)
        { return feet * 0.3048; }
        public static double FeetToMiles(double feet)
        { return feet * 0.0001894; }
        public static double FeetToMilesNaut(double feet)
        { return feet * 0.0001645; }
        public static double MilesToFeet(double miles)
        { return miles * (double)LinearMeasurementConversion.FeetPerMile; }
        public static double MilesToKilometers(double miles)
        { return miles * 1.6093; }
        public static double MilesNautToKilometers(double miles)
        { return miles * 1.852; }
        public static double YardsToMeters(double yards)
        { return yards * 0.9144; }
        public static double YardsToMiles(double yards)
        { return yards * 0.0005682; }
        public static double YardsToFeet(double yards)
        { return yards * (double)LinearMeasurementConversion.FeetPerYard; }
        #endregion
    }
    /// <summary>
    /// Provides static methods for converting square measurments of surfaces.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class SurfaceMeasurementConversion
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static double SqKilometersToSqMiles(double sqKilometers)
        { return sqKilometers * 0.3861; }
        public static double SqMetersToSqFeet(double sqMeters)
        { return sqMeters * 10.7639; }
        public static double SqMetersToSqlYards(double sqMeters)
        { return sqMeters * 1.196; }
        public static double SqFeetToSqMeters(double sqFeet)
        { return sqFeet * 0.0929; }
        public static double SqMilesToSqKilometers(double sqMiles)
        { return sqMiles * 2.59; }
        public static double SqYardsToSqMeters(double sqYards)
        { return sqYards * 0.8361; }
        #endregion
    }
    /// <summary>
    /// Provides static methods for converting volumetric measurements.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class VolumeMeasurementConversion
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static double CubicMetersToCubicFeet(double cbMeters)
        { return cbMeters * 35.3145; }
        public static double CubicMetersToCubicYards(double cbMeters)
        { return cbMeters * 1.3079; }
        public static double LitersToGallons(double liters)
        { return liters * 0.2642; }
        public static double LitersToPintsLiquid(double liters)
        { return liters * 2.1134; }
        public static double LitersToQuartsLiquid(double liters)
        { return liters * 1.0567; }
        public static double CubicFeetToCubicMeters(double cbFeet)
        { return cbFeet * 0.0283; }
        public static double CubicYardsToCubicMeters(double cbFeet)
        { return cbFeet * 0.7646; }
        public static double GallonsToLiters(double gallons)
        { return gallons * 3.7853; }
        public static double PintsLiquidToLiters(double pints)
        { return pints * 0.4732; }
        public static double QuartsLiquidToLiters(double quarts)
        { return quarts * 0.9463; }
        #endregion
    }
    /// <summary>
    /// Provides static methods for converting weight measurements.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class WeightMeasurementConversion
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly int
            PoundsPerShortTon = 2000,
            PoundsPerLongTon = 2240,
            PoundsPerMetricTon = 2204;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static double GramsToPounds(double grams)
        { return grams * 0.002205; }
        public static double MetricTonsToPounds(double mTons)
        { return mTons * (double)WeightMeasurementConversion.PoundsPerMetricTon; }
        public static double MetricTonsToTonsLong(double mTons)
        { return mTons * 0.9842; }
        public static double MetricTonsToTonsShort(double mTons)
        { return mTons * 1.1023; }
        public static double TonsLongToMetricTons(double tons)
        { return tons * 1.016; }
        public static double TonLongToMetricTons(double tons)
        { return tons * 0.9072; }
        public static double OuncesToPounds(double ounces)
        { return ounces * 0.00625; }
        public static double PoundsToMetricTons(double pounds)
        { return pounds * 0.0004536; }
        public static double PoundsToTonsLong(double pounds)
        { return pounds * 0.0004464; }
        public static double PoundsToTonsShort(double pounds)
        { return pounds * 0.0005; }
        public static double TonsLongToPounds(double tons)
        { return tons * (double)WeightMeasurementConversion.PoundsPerLongTon; }
        public static double TonsShortToPounds(double tons)
        { return tons * (double)WeightMeasurementConversion.PoundsPerShortTon; }
        #endregion
    }
}
