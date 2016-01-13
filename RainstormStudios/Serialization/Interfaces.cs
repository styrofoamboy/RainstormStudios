using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Serialization
{
    public interface ISerializableItem
    {
        bool IsNew { get; }
        bool IsEdited { get; }
        bool IsDeleted { get; }

        string GetSerialized();
        void SetRefKey(string key);

        void Delete();
        void UnDelete();
    }
    public interface ISerializableCollection
    {
        string SerializeCollection();
        void DeserializeCollection(string serializedItems);
    }
}
