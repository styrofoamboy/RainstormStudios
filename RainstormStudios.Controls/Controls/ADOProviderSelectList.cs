using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Controls
{
    public class ADOProviderSelectList : System.Windows.Forms.ComboBox
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [System.ComponentModel.Browsable(false)
        , System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public RainstormStudios.Data.AdoProviderType SelectedProviderType
        {
            get
            {
                if (!this.DesignMode)
                {
                    var providerName = (from members in typeof(RainstormStudios.Data.AdoProviderType).GetMembers()
                                        let attrs = members.GetCustomAttributes(true).OfType<System.ComponentModel.DescriptionAttribute>().FirstOrDefault()
                                        where attrs != null
                                        && attrs.Description == (string)this.SelectedItem
                                        select members.Name).FirstOrDefault();
                    if (providerName == null)
                        throw new Exception("Selected provider was not found.");

                    RainstormStudios.Data.AdoProviderType providerType = Data.AdoProviderType.Auto;
                    if (!Enum.TryParse<RainstormStudios.Data.AdoProviderType>(providerName, out providerType))
                        throw new Exception("Selected provider was not found.");

                    return providerType;
                }
                else
                    return Data.AdoProviderType.Auto;
            }
            set
            {
                if (!this.DesignMode)
                {
                    var providerDesc = (from members in typeof(RainstormStudios.Data.AdoProviderType).GetMembers()
                                        where members.Name == value.ToString()
                                        let attrs = members.GetCustomAttributes(true).OfType<System.ComponentModel.DescriptionAttribute>().FirstOrDefault()
                                        where attrs != null
                                        select attrs.Description);

                    int selIdx = -1;
                    for (int i = 0; i < this.Items.Count; i++)
                        if (this.Items[i] == providerDesc)
                        { selIdx = i; break; }

                    if (selIdx == -1)
                        throw new Exception("Specified provider was not found in the list.");

                    this.SelectedIndex = selIdx;
                }
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ADOProviderSelectList()
            : base()
        {
            this.InitProviderNames(null);
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
