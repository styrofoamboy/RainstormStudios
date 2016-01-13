using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Controls
{
    public class ADOProviderSelectList : System.Windows.Forms.ComboBox
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ADOProviderSelectList()
            : base()
        {
            this.InitProviderNames(null);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual void InitProviderNames(string[] excludeProviders)
        {
            if (excludeProviders == null)
                excludeProviders = new string[0];

            var providerNames = (from members in typeof(RainstormStudios.Data.AdoProviderType).GetMembers()
                                 where !excludeProviders.Contains(members.Name)
                                 let attrs = members.GetCustomAttributes(true).OfType<System.ComponentModel.DescriptionAttribute>()
                                 where attrs.Count() > 0
                                 select attrs.First().Description);

            foreach (var nm in providerNames)
                this.Items.Add(nm);
        }
        #endregion
    }
}
