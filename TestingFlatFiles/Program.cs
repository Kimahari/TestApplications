// See https://aka.ms/new-console-template for more information
using System.Data;

Console.WriteLine("Hello, World!");

DataTable dt = new();
dt.Clear();
dt.Columns.Add("Name");
dt.Columns.Add("Marks", typeof(int));
for (int i = 0; i < 200; i++) {
    DataRow _ravi = dt.NewRow();
    _ravi["Name"] = $"Value ${i}";
    _ravi["Marks"] = i;
    dt.Rows.Add(_ravi);
}

return;


//var exporter = new FlatFileExporter();
//using var dt = GenerateTable();

//await exporter.ExportToFileFile(dt);

//DataTable GenerateTable() {
//    DataTable dt = new DataTable();
//    dt.Clear();
//    dt.Columns.Add("Name");
//    dt.Columns.Add("Marks", typeof(int));
//    for (int i = 0; i < 200; i++) {
//        DataRow _ravi = dt.NewRow();
//        _ravi["Name"] = $"Value ${i}";
//        _ravi["Marks"] = i;
//        dt.Rows.Add(_ravi);
//    }
//    return dt;
//}