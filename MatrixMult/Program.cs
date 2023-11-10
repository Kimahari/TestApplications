// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

using MatrixMult;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<MatrixBenchmarks>();