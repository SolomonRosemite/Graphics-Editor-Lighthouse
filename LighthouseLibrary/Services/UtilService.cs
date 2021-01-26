using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
    }
}