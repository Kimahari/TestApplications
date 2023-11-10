// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;

using CommunityToolkit.HighPerformance.Buffers;

using System.Buffers;

[MemoryDiagnoser]
public class Benchmarks {
    private readonly string string1 = "Hello";
    private readonly string string2 = " World";

    [Benchmark]
    public ReadOnlySpan<char> MergeSpans() {
        var span1 = string1.AsSpan();
        var span2 = string2.AsSpan();

        int length = span1.Length + span2.Length;
        char[] mergedArray = new char[length];
        span1.CopyTo(mergedArray);
        span2.CopyTo(mergedArray.AsSpan(span1.Length));

        return mergedArray;
    }

    [Benchmark]
    public ReadOnlySpan<char> Merge() {
        return string1 + string2;
    }

    [Benchmark]
    public string Merge2() {
        return string.Concat(string1.AsSpan(), string2.AsSpan());
    }

    [Benchmark]
    public ReadOnlySpan<char> MergeSpans2() {
        var span1 = string1.AsSpan();
        var span2 = string2.AsSpan();

        int length = span1.Length + span2.Length;
        char[] mergedArray = new char[length];
        span1.CopyTo(mergedArray);
        span2.CopyTo(mergedArray.AsSpan(span1.Length));

        return StringPool.Shared.GetOrAdd(mergedArray);
    }

    [Benchmark]
    public string MergeSpansWithoutMemoryAllocation() {
        var span1 = string1.AsSpan();
        var span2 = string2.AsSpan();

        return string.Create(span1.Length + span2.Length, (string1, string2), (destination, tuple) => {
            var span1 = tuple.string1.AsSpan();
            var span2 = tuple.string2.AsSpan();

            span1.CopyTo(destination);
            span2.CopyTo(destination[span1.Length..]);
        });
    }

    [Benchmark]
    public string MergeSpansWithStringPool() {
        var span1 = string1.AsSpan();
        var span2 = string2.AsSpan();

        int length = span1.Length + span2.Length;
        char[] mergedArray = ArrayPool<char>.Shared.Rent(length);
        span1.CopyTo(mergedArray);
        span2.CopyTo(mergedArray.AsSpan(span1.Length));

        string result = new string(mergedArray, 0, length);
        ArrayPool<char>.Shared.Return(mergedArray);
        return result;
    }
}