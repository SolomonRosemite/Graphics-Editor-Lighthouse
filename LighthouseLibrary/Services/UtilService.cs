﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LighthouseLibrary.Services
{
    public static class UtilService
    {
        private static readonly List<int> Ids = new List<int>();
        private static readonly Random Random = new Random();

        public static int GenerateNewId()
        {
            int value = Random.Next(int.MaxValue);

            if (Ids.Contains(value))
                return GenerateNewId();

            Ids.Add(value);

            return value;
        }

        public static T DeepClone<T>(this T input) where T : ISerializable
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, input);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }
}