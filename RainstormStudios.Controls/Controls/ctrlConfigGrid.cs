using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Controls
{
    public abstract class ConfigGridParent
    {
        internal enum ConfigGridParentType
        {
            Section,
            Entry
        }

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ConfigGridEntryCollection
            _entryCol = new ConfigGridEntryCollection();
        private ConfigGrid
            _owner;
        private bool
            _exp;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public ConfigGrid Owner
        { get { return this._owner; } }
        public ConfigGridEntryCollection Items
        { get { return this._entryCol; } }
        public bool Expanded
        {
            get { return this._exp; }
            set { this._exp = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        internal abstract ConfigGridParentType ObjType
        { get; }
        #endregion
    }
    public class ConfigGridSection : ConfigGridParent
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _secName;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string SectionName
        {
            get { return this._secName; }
            set { this._secName = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        internal override ConfigGridParent.ConfigGridParentType ObjType
        {
            get { return ConfigGridParentType.Section; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConfigGridSection(string name)
        {
            this._secName = name;
        }
        #endregion
    }
    public class ConfigGridSectionCollection : RainstormStudios.ObjectCollectionBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ConfigGrid
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new ConfigGridSection this[int index]
        {
            get { return (ConfigGridSection)base[index]; }
            set { base[index] = value; }
        }
        public new ConfigGridSection this[string key]
        {
            get { return (ConfigGridSection)base[key]; }
            set { base[key] = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal ConfigGridSectionCollection()
        { }
        internal ConfigGridSectionCollection(ConfigGrid owner)
            : this()
        {
            this._owner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(ConfigGridSection value)
        { return base.Add(value, ""); }
        public string Add(ConfigGridSection value, string key)
        { return base.Add(value, key); }
        public string Insert(int index, ConfigGridSection value)
        { return base.Insert(index, value, ""); }
        public string Insert(int index, ConfigGridSection value, string key)
        { return base.Insert(index, value, key); }
        public new ConfigGridSection[] GetFlatArray()
        { return this.GetFlatArray(0, this.InnerList.Count); }
        public new ConfigGridSection[] GetFlatArray(int offset, int length)
        { return Array.ConvertAll<object, ConfigGridSection>(base.GetFlatArray(offset, length), new Converter<object, ConfigGridSection>(this.CastObj)); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private ConfigGridSection CastObj(object val)
        { return (ConfigGridSection)val; }
        #endregion
    }
    public class ConfigGridEntry : ConfigGridParent
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _entryName,
            _desc;
        private Type
            _valType;
        private bool
            _readOnly;
        private ConfigGridParent
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new ConfigGridParent Owner
        { get { return this._owner; } }
        public string EntryName
        {
            get { return this._entryName; }
            set { this._entryName = value; }
        }
        public string EntryDescription
        {
            get { return this._desc; }
            set { this._desc = value; }
        }
        public Type ValueType
        {
            get { return this._valType; }
            set { this._valType = value; }
        }
        public bool ReadOnly
        {
            get { return this._readOnly; }
            set { this._readOnly = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        internal override ConfigGridParent.ConfigGridParentType ObjType
        {
            get { return ConfigGridParentType.Entry; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConfigGridEntry(string name)
            : this(name, typeof(System.String), false)
        { }
        public ConfigGridEntry(string name, Type valueType)
            : this(name, valueType, false)
        { }
        public ConfigGridEntry(string name, Type valueType, bool readOnly)
        {
            this._entryName = name;
            this._valType = valueType;
            this._readOnly = readOnly;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Internal Methods
        // 
        internal void SetOwner(ConfigGridParent owner)
        { this._owner = owner; }
        #endregion
    }
    public class ConfigGridEntryCollection : RainstormStudios.ObjectCollectionBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ConfigGridSection
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new ConfigGridEntry this[int index]
        {
            get { return (ConfigGridEntry)base[index]; }
            set { base[index] = value; }
        }
        public new ConfigGridEntry this[string key]
        {
            get { return (ConfigGridEntry)base[key]; }
            set { base[key] = value; }
        }
        #endregion
    }
    public class ConfigGrid : System.Windows.Forms.Control
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ConfigGridSectionCollection
            _secs;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public ConfigGridSectionCollection Sections
        { get { return this._secs; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConfigGrid()
        {
            this._secs = new ConfigGridSectionCollection();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(SystemColors.ControlDarkDark);
            int yOffset = 0;
            for (int i = 0; i < this._secs.Count; i++)
            {
                ConfigGridSection sec = this._secs[i];
                Label lblSecHdr = new Label();
                lblSecHdr.Text = sec.SectionName;
                this.Controls.Add(lblSecHdr);
                yOffset += lblSecHdr.Height;

                if (sec.Expanded)
                {
                    // Build the GridView control to edit the data.
                    DataGridView dgv = new DataGridView();
                    DataGridViewColumn colName = new DataGridViewColumn();
                    colName.ReadOnly = true;
                    dgv.Columns.Add(colName);
                    DataGridViewColumn colVal = new DataGridViewColumn();
                    dgv.Columns.Add(colVal);
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    for (int t = 0; t < sec.Items.Count; t++)
                    {
                        ConfigGridEntry ent = sec.Items[t];
                        if (ent.Expanded)
                        {
                        }
                    }

                }
            }
        }
        #endregion
    }
}
