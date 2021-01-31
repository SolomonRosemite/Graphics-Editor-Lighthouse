using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Filter : ISerializable
    {
        public int Id { get; }

        public Filter(int id)
        {
            Id = id;
        }

        public void ApplyFilter(ref Bitmap bitmap)
        {
            // Todo: Apply Filter to bitmap.
        }

        public Filter(SerializationInfo info, StreamingContext _)
        {
            Id = GetValue<int>("Id");

            T GetValue<T>(string name)
            {
                return (T)info.GetValue(name, typeof(T));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
        }
    }
}