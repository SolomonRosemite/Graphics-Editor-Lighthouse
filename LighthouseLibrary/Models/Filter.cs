using LighthouseLibrary.Models.Enums;
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Filter : ISerializable
    {
        public int Id { get; }
        public FilterType Type { get; }

        public Filter(int id, FilterType type)
        {
            Id = id;
            Type = type;
        }

        public void ApplyFilter(ref Bitmap bitmap)
        {
            // Todo: Apply Filter to bitmap.
        }

        public Filter(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");
            Type = GetValue<FilterType>("Type");

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Type", Type);
        }
    }
}