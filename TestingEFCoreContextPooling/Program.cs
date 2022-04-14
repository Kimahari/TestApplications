using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var ss = ManualConfig.Create(DefaultConfig.Instance);

BenchmarkRunner.Run<Benchy>(ss);