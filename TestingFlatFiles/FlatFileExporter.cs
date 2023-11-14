using System.Data;
using System.Text;

namespace TestingFlatFiles;

internal class FlatFileExporter {
    public async Task ExportToFileFile(DataTable table) {
        if (File.Exists("test.csv")) File.Delete("test.csv");

        using var fileStream = File.Create("test.csv");
        using var writer = new StreamWriter(fileStream);

        await ExportHeader(table, writer);

        foreach (DataRow row in table.Rows) {
            await ExportRow(table, row, writer);
        }
    }

    private static async Task ExportRow(DataTable table, DataRow row, StreamWriter writer) {
        StringBuilder stringBuilder = new();

        var exported = false;

        foreach (DataColumn c in table.Columns) {
            var value = row[c.ColumnName];

            if (exported) stringBuilder.Append(',');
            stringBuilder.Append(row[c.ColumnName]);
            exported = true;
        }

        await writer.WriteLineAsync(stringBuilder.ToString());
    }

    private static async Task ExportHeader(DataTable table, StreamWriter writer) {
        StringBuilder stringBuilder = new();

        var exported = false;

        foreach (DataColumn c in table.Columns) {
            if (exported) stringBuilder.Append(',');
            stringBuilder.Append(c.ColumnName);
            exported = true;
        }

        await writer.WriteLineAsync(stringBuilder.ToString());
    }
}
