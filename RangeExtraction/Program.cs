// See https://aka.ms/new-console-template for more information
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

//RangeExtraction.Extract(new[] { 1, 2, 3 });
//RangeExtraction.Extract(new[] { -6, -3, -2, -1, 0, 1, 3, 4, 5, 7, 8, 9, 10, 11, 14, 15, 17, 18, 19, 20 });
//RangeExtraction.Extract(new[] { -193, -192, -190, -188, -186, -184, -182, -180, -178, -176, -174, -172, -171, -170, -169, -167, -165, -164, -162, -161, -159, -157, -155, -153, -151, -150, -149, -147, -145, -143, -142, -140, -139, -137, -136, -134, -132, -130, -128, -127, -125, -123, -121, -119, -117, -115, -113, -111, -109, -107, -105, -104, -102, -101, -100, -99, -97, -96, -95, -94, -92, -91, -89, -88, -86, -84, -83, -81, -79, -77, -75, -74, -73, -72, -70, -68, -66, -65, -64, -63, -61, -59, -58, -56, -54, -53, -52, -51, -49, -47, -45, -43, -41, -40, -39, -38, -36, -35, -33 });

//RangeExtraction.Extract(new[] { -158, -157, -156, -155, -154, -152, -150, -149, -147, -146, -144, -143, -141, -139, -138, -136, -135, -133, -132, -130, -129, -128, -127, -125, -124, -123, -122, -121, -119, -117, -116, -115, -113, -112, -110, -108, -107, -106, -105, -103, -102, -101, -100, -99, -97, -95, -94, -93, -92, -91, -90, -89, -88, -87, -86, -85, -84, -83, -81, -80, -78, -77, -76, -75, -73, -72, -71, -70, -68, -66, -64, -63, -62, -60, -58, -57, -55, -54, -53, -51, -50, -48, -47, -46, -45, -44, -42, -40, -38, -37, -35, -34, -33, -31, -30, -29, -27, -26, -24, -22, -20, -18, -17, -16, -14, -13, -12, -10, -8, -6, -5, -4, -2, -1, 1, 3, 5, 7, 8, 9, 10, 12, 14, 16, 18, 20, 21, 22, 23, 25, 26, 28, 29, 31, 33, 35, 37, 39, 41, 43, 44, 46, 48 });
//var data = RangeExtraction.Extract2(new[] { -158, -157, -156, -155, -154, -152, -150, -149, -147, -146, -144, -143, -141, -139, -138, -136, -135, -133, -132, -130, -129, -128, -127, -125, -124, -123, -122, -121, -119, -117, -116, -115, -113, -112, -110, -108, -107, -106, -105, -103, -102, -101, -100, -99, -97, -95, -94, -93, -92, -91, -90, -89, -88, -87, -86, -85, -84, -83, -81, -80, -78, -77, -76, -75, -73, -72, -71, -70, -68, -66, -64, -63, -62, -60, -58, -57, -55, -54, -53, -51, -50, -48, -47, -46, -45, -44, -42, -40, -38, -37, -35, -34, -33, -31, -30, -29, -27, -26, -24, -22, -20, -18, -17, -16, -14, -13, -12, -10, -8, -6, -5, -4, -2, -1, 1, 3, 5, 7, 8, 9, 10, 12, 14, 16, 18, 20, 21, 22, 23, 25, 26, 28, 29, 31, 33, 35, 37, 39, 41, 43, 44, 46, 48 });

//Console.WriteLine(data);
BenchmarkRunner.Run<RangeExtraction>();

[MemoryDiagnoser]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[RPlotExporter]
public class RangeExtraction {

    [Params(new[] { 1, 2, 3 }, new[] { 1, 2, 3, 4 })]
    public int[] Data { get; set; }

    [Benchmark]
    public void Run1() {
        var data = Extract(Data);
    }

    [Benchmark]
    public void Run2() {
        var data = Extract2(Data);
    }

    [Benchmark]
    public void Run3() {
        var data = Extract3(Data);
    }

    public static string Extract(int[] args) {
        if (args.Length <= 2) return string.Join(',', args);

        var span = 0;
        var current = args[0];
        var spanStart = 0;

        StringBuilder sb = new StringBuilder();

        for (int i = 1; i < args.Length; i++) {
            var next = args[i];
            var diff = next - current;

            if (diff == 1) {
                span++;
                current = next;
                continue;
            };
            if (sb.Length > 0) sb.Append(",");

            if (span >= 2) {
                sb.Append(args[spanStart]).Append('-').Append(args[spanStart + span]);
            } else if (span > 0) {
                sb.Append(args[spanStart]).Append(',').Append(args[spanStart + 1]);
            } else sb.Append(current);

            current = next;
            span = 0;
            spanStart = i;
        }

        if (sb.Length > 0) sb.Append(",");

        if (span >= 2) {
            sb.Append(args[spanStart]).Append('-').Append(args[spanStart + span]);
        } else if (span > 0) {
            sb.Append(args[spanStart]).Append(',').Append(args[spanStart + 1]);
        } else {
            sb.Append(current);
        }

        return sb.ToString();  //TODO
    }

    public static string Extract3(int[] args) {
        if (args.Length <= 2)
            return string.Join(',', args);

        var result = new List<string>();
        var spanStart = args[0];
        var spanEnd = args[0];

        for (int i = 1; i < args.Length; i++) {
            if (args[i] == args[i - 1] + 1) {
                spanEnd = args[i];
            } else {
                if (spanEnd == spanStart)
                    result.Add(spanStart.ToString());
                else if (spanEnd == spanStart + 1) {
                    result.Add(spanStart.ToString());
                    result.Add(spanEnd.ToString());
                } else {
                    result.Add($"{spanStart}-{spanEnd}");
                }

                spanStart = args[i];
                spanEnd = args[i];
            }
        }

        if (spanEnd == spanStart)
            result.Add(spanStart.ToString());
        else if (spanEnd == spanStart + 1) {
            result.Add(spanStart.ToString());
            result.Add(spanEnd.ToString());
        } else {
            result.Add($"{spanStart}-{spanEnd}");
        }

        return string.Join(",", result);
    }

    public static string Extract2(int[] args) {
        if (args == null || args.Length == 0)
            return "";

        var result = new StringBuilder();
        var start = args[0];
        var end = args[0];
        var rangeCount = 1;

        for (int i = 1; i < args.Length; i++) {
            if (args[i] == end + 1) {
                end = args[i];
                rangeCount++;
            } else {
                if (rangeCount >= 3) {
                    result.Append($"{start}-{end},");
                } else {
                    for (int j = 0; j < rangeCount; j++) {
                        result.Append($"{start},");
                        start++;
                    }
                }

                start = args[i];
                end = args[i];
                rangeCount = 1;
            }
        }

        // Check for the last range or individual numbers
        if (rangeCount >= 3) {
            result.Append($"{start}-{end}");
        } else {
            for (int j = 0; j < rangeCount; j++) {
                result.Append($"{start},");
                start++;
            }
        }

        // Remove the trailing comma, if any
        if (result.Length > 0 && result[result.Length - 1] == ',')
            result.Length--;

        return result.ToString();
    }
}