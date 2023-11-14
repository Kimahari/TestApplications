using System.Data;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace TestingFlatFiles; 

[MemoryDiagnoser]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class BenchMarks {
    private DataTable? dt;
    private MemoryStream? s;
    private StreamWriter? sWriter;
    private bool firstCall;
    private bool firstCall2;
    private bool firstCall3;
    private bool firstCall4;

    [GlobalSetup]
    public void Setup() {

        this.dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks", typeof(int));
        for (int i = 0; i < 20_000; i++) {
            DataRow? _ravi = dt.NewRow();
            _ravi["Name"] = $"Value ${i}";
            _ravi["Marks"] = i;
            dt.Rows.Add(_ravi);
        }
    }

    [IterationSetup]
    public void BenchMarksSetup() {
        this.s = new MemoryStream();
        this.sWriter = new StreamWriter(s);
    }

    [IterationCleanup]
    public void BenchMarksCleanup() {
        this.sWriter?.Dispose();
        this.s?.Dispose();
    }

    const char comma = ',';
    const char newLine = '\n';
    const string dataStr = "String Data";
    const int dataInt = 69;

    [Benchmark]
    public void ExportTableConst() {
        if (firstCall == false) {
            firstCall = true;
            Console.WriteLine("// First call");
            Thread.Sleep(1000);
        }

        foreach (DataRow row in this.dt!.Rows) {
            ExportRow1(this.dt, row, sWriter!);
        }

        Thread.Sleep(100);
    }

    [Benchmark]
    public void RowExporterMarkConst() {
        if (firstCall2 == false) {
            firstCall2 = true;
            Console.WriteLine("// First call");
            Thread.Sleep(1000);
        }

        var table = this.dt!;
        var row = dt!.Rows[0];

        ExportRow1(table, row, sWriter!);

        Thread.Sleep(100);
    }

    [Benchmark]
    public void RowExporterMarkStringBuilder() {
        if (firstCall3 == false) {
            firstCall3 = true;
            Console.WriteLine("// First call");
            Thread.Sleep(1000);
        }

        var table = this.dt;
        var row = dt!.Rows[0];

        ExportRow2(table!, row, sWriter!);

        Thread.Sleep(100);
    }

    [Benchmark]
    public void ExportTableStringBuilder() {
        if (firstCall4 == false) {
            firstCall4 = true;
            Console.WriteLine("// First call");
            Thread.Sleep(1000);
        }

        foreach (DataRow row in this.dt!.Rows) {
            ExportRow2(this.dt, row, sWriter!);
        }

        Thread.Sleep(100);
    }

    private static void ExportRow2(DataTable table, DataRow row, StreamWriter sWriter) {
        var exported = false;

        foreach (DataColumn c in table.Columns) {
            if (exported) sWriter.WriteLine(comma);

            if (dataInt is int number) {
                sWriter.Write(number);
            }

            if (dataStr is string str) {
                sWriter.Write(str.AsSpan());
            }

            if (!exported) exported = true;
        }

        sWriter.WriteLine();
    }

    private static void ExportRow1(DataTable table, DataRow row, StreamWriter sWriter) {
        StringBuilder? stringBuilder = new();

        var exported = false;

        foreach (DataColumn c in table.Columns) {
            var value = row[c.ColumnName];

            if (exported) stringBuilder.Append(comma);
            stringBuilder.Append(row[c.ColumnName]);
            exported = true;
        }

        sWriter.WriteLineAsync(stringBuilder.ToString());
    }

    [GlobalCleanup]
    public void Cleanup() {
        dt?.Dispose();
    }
}
