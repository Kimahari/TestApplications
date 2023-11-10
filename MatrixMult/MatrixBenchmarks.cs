using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

namespace MatrixMult;

public class MatrixBenchmarks {

    static int[][] matrix1;
    static int[][] matrix2;

    [GlobalSetup]
    public void Setup() {
        matrix1 = new int[2][];
        matrix1[0] = [1, 2];
        matrix1[1] = [2, 1];


        matrix2 = new int[2][];
        matrix2[0] = [1, 2];
        matrix2[1] = [2, 1];
    }


    [Benchmark]
    public int[][] Multiply() {
        var result = new int[matrix1.Length][];
        for (var i = 0; i < matrix1.Length; i++) {
            result[i] = new int[matrix2[0].Length];
            for (var j = 0; j < matrix2[0].Length; j++) {
                result[i][j] = 0;
                for (var k = 0; k < matrix1[0].Length; k++) {
                    result[i][j] += matrix1[i][k] * matrix2[k][j];
                }
            }
        }
        return result;
    }

    [Benchmark]
    public int[][] MultiplyParallel() {
        var result = new int[matrix1.Length][];
        Parallel.For(0, matrix1.Length, i => {
            result[i] = new int[matrix2[0].Length];
            for (var j = 0; j < matrix2[0].Length; j++) {
                result[i][j] = 0;
                for (var k = 0; k < matrix1[0].Length; k++) {
                    result[i][j] += matrix1[i][k] * matrix2[k][j];
                }
            }
        });
        return result;
    }

    [Benchmark]
    public int[][] MultiplyParallelPartitioner() {
        var result = new int[matrix1.Length][];
        Parallel.ForEach(Partitioner.Create(0, matrix1.Length), range => {
            for (var i = range.Item1; i < range.Item2; i++) {
                result[i] = new int[matrix2[0].Length];
                for (var j = 0; j < matrix2[0].Length; j++) {
                    result[i][j] = 0;
                    for (var k = 0; k < matrix1[0].Length; k++) {
                        result[i][j] += matrix1[i][k] * matrix2[k][j];
                    }
                }
            }
        });
        return result;
    }
}
