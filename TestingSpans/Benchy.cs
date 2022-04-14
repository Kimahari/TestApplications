using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class Benchy {
    private const string DateAsString = "01122021";


    [Benchmark]
    public (int day, int month, int year) GetDateSubstring() {
        var dayAsString = DateAsString[..2];
        var monthAsString = DateAsString[2..4];
        var yearAsString = DateAsString[4..];


        var day = int.Parse(dayAsString);
        var month = int.Parse(monthAsString);
        var year = int.Parse(yearAsString);

        return (day, month, year);
    }

    [Benchmark]
    public (int day, int month, int year) GetDateSpan() {
        return GetDateSpanImp(DateAsString);
    }

    private (int day, int month, int year) GetDateSpanImp(ReadOnlySpan<char> dateAsString) {

        var dayAsString = dateAsString[..2];
        var monthAsString = dateAsString[2..4];
        var yearAsString = dateAsString[4..];


        var day = int.Parse(dayAsString);
        var month = int.Parse(monthAsString);
        var year = int.Parse(yearAsString);

        return (day, month, year);
    }
}