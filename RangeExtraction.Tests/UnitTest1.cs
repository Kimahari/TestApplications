using System.Text;

namespace RangeExtraction.Tests;


public class UnitTest1 {

    [Theory]
    [InlineData(new[] { 1, 2 }, "1,2")]
    [InlineData(new[] { 1 }, "1")]
    [InlineData(new int[] { }, "")]
    [InlineData(null, "")]
    [InlineData(new[] { 1, 2, 3 }, "1-3")]
    [InlineData(new[] { 1, 2, 5 }, "1,2,5")]
    [InlineData(new[] { -6, -3, -2, -1, 0, 1, 3, 4, 5, 7, 8, 9, 10, 11, 14, 15, 17, 18, 19, 20 }, "-6,-3-1,3-5,7-11,14,15,17-20")]
    [InlineData(new[] { 73, 75, 77, 78, 79, 81, 82, 83, 84, 86, 87, 89, 91, 93, 94 }, "73,75,77-79,81-84,86,87,89,91,93,94")]
    public void Test(int[] args, string expected) {
        Assert.Equal(expected, RangeExtraction.Extract(args));
    }
}

public class RangeExtraction {
    public static string Extract(int[] args) {
        if (args is null) return string.Empty;
        if (args.Length == 0) return string.Empty;
        if (args.Length == 1) return args[0].ToString();
        if (args.Length == 2) return $"{args[0]},{args[1]}";

        Console.WriteLine('[' + string.Join(',', args) + ']');

        var result = new StringBuilder();

        var startIndex = 0;
        var lastIndex = 0;
        var lastValue = args[0];

        for (int i = 1; i < args.Length; i++) {
            var distance = args[i] - lastValue;

            if (distance is 0) {
                lastIndex = i;
                continue;
            }

            if (distance is 1 or -1) {
                lastValue = args[i];
                lastIndex = i;
                continue;
            }

            if (lastIndex == 0) {
                result.Append($"{args[startIndex]}");
            } else if (lastIndex - startIndex < 2 && args[startIndex] == args[lastIndex]) {
                if (result.Length > 0) result.Append(",");
                result.Append($"{args[startIndex]}");
            } else if (lastIndex - startIndex < 2) {
                if (result.Length > 0) result.Append(",");
                result.Append($"{args[startIndex]}").Append(",");
                result.Append(args[lastIndex].ToString());
            } else {
                if (result.Length > 0) result.Append(",");
                result.Append($"{args[startIndex]}-{args[lastIndex]}");
            }

            lastValue = args[i];
            startIndex = i;
            lastIndex = i;
        }

        if (result.Length > 0) result.Append(",");


        if (lastIndex == startIndex) {
            result.Append($"{args[startIndex]}");
            return result.ToString();
        }

        if (lastIndex - startIndex < 2 && args[startIndex] == args[lastIndex]) {
            result.Append($"{args[startIndex]}");
            return result.ToString();
        }

        if (lastIndex - startIndex < 2) {
            result.Append($"{args[startIndex]}").Append(",");
            result.Append(args[lastIndex].ToString());
            return result.ToString();
        }

        result.Append($"{args[startIndex]}-{args[lastIndex]}");
        
        return result.ToString();

    }
}