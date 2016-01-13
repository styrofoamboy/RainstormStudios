using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Serialization
{
    public enum SerializablePropertyHandling
    {
        Standard,
        ParentEntity
    }
    public class SerializePropertyAttribute : Attribute
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        public SerializablePropertyHandling
            HandlingType { get; private set; }
        #endregion

        #region Constructors
        //***************************************************************************
        // Constructors
        // 
        public SerializePropertyAttribute()
        {
            this.HandlingType = SerializablePropertyHandling.Standard;
        }
        public SerializePropertyAttribute(SerializablePropertyHandling handling)
            : this()
        {
            this.HandlingType = handling;
        }
        #endregion
    }
}
