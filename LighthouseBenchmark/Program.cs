using LighthouseBenchmark.Models;
using BenchmarkDotNet.Running;

namespace LighthouseBenchmark
{
    public static class Program
    {
        // Used Image: https://unsplash.com/photos/zXQFq-KDNFs
        private static void Main(string[] args) => BenchmarkRunner.Run<BitmapBenchmarks>();
    }
}
