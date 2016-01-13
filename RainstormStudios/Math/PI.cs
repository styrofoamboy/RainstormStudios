using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Math
{
    public sealed class PI
    {
        #region Declarations
        //***************************************************************************
        // Public Events
        // 
        public static event FindPiEventHandler
            FindPiComplete;
        //***************************************************************************
        // Event Delegates
        // 
        public delegate void FindPiEventHandler(double pi);
        //***************************************************************************
        // Thread Delegates
        // 
        public delegate double FindPiDelegate(int decnum);
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PI()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public double FindPi(int decimalPlaces)
        {
            return this.DoPICalc(decimalPlaces);
        }
        public void BeginFindPi(int decimalPlaces)
        {
            FindPiDelegate del = new FindPiDelegate(DoPICalc);
            del.BeginInvoke(decimalPlaces, new AsyncCallback(this.FindPiCallback), del);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private double DoPICalc(int decnum)
        {
            double pi = 1;
            double e = 2, o = 1;
            while (Convert.ToString(pi / 2).Length < decnum + 2)
            {
                pi *= (e / o);
                o += 2;
                pi *= (e / o);
                e += 2;
            }
            return pi / 2;
        }
        private void FindPiCallback(IAsyncResult state)
        {
            FindPiDelegate del = (FindPiDelegate)state.AsyncState;
            double retVal = del.EndInvoke(state);
            if (FindPiComplete != null)
                FindPiComplete(retVal);
        }
        #endregion
    }
}
