using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using OfficeOpenXml;

namespace ExcelTest; 

public class SomeObjectAccessAnalytic {
    public string Name { get; set; }
    public static int AccessedCount { get => 10; }
    public Dictionary<string, int> Analytics { get; set; } = [];
}

public class SomeObject {

    public Func<string, Task<string>> ResolveNameCallback;
    public int Count { get; set; }
    public Dictionary<string, int> OverviewAnalytics { get; set; } = [];
    public int AccessedCount { get => AccessAnalytics.Count; }
    public Dictionary<string, SomeObjectAccessAnalytic> AccessAnalytics { get; set; } = [];
}


internal class Program {
    static void Main(string[] args) {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var objects = new Dictionary<string, SomeObject>();
        var appId = Guid.NewGuid().ToString();

        objects["Documents"] = new SomeObject {
            AccessAnalytics = new Dictionary<string, SomeObjectAccessAnalytic> {
                [appId] = new SomeObjectAccessAnalytic {
                    Name = "App1",
                    Analytics = new Dictionary<string, int>() {
                    {"JAN",10},
                    {"FEB",10},
                }
                }
            }
        };

        if (File.Exists("MyWorkbook2.xlsx")) File.Delete("MyWorkbook2.xlsx");

        CreateSpreadsheetWorkbook("MyWorkbook2.xlsx", objects);

        if (File.Exists("MyWorkbook.xlsx")) File.Delete("MyWorkbook.xlsx");

        using (var package = new ExcelPackage(new FileInfo("MyWorkbook.xlsx"))) {
            foreach (var sybrinObject in objects) {
                var sheetName = sybrinObject.Key;
                ExportAccessAnalytics(package, sheetName, sybrinObject.Value.AccessAnalytics);

            }

            package.Save();
        }

        Console.WriteLine("Hello World!");
    }

    public static void CreateSpreadsheetWorkbook(string filePath, Dictionary<string, SomeObject> data) {
        // Create a spreadsheet document by supplying the filePath.
        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
            Create(filePath, SpreadsheetDocumentType.Workbook);

        // Add a WorkbookPart to the document.
        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        foreach (var item in data) {
            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new() {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = item.Key
            };

            var cols = GetColumns(item.Value.AccessAnalytics.FirstOrDefault().Value).ToArray();

            var count = 1;

            foreach (var col in cols) {
                var colName = GetColName(count);
                count++;
                Cell cell = InsertCellInWorksheet(colName, 1, worksheetPart);
                cell.CellValue = new CellValue(col);
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }

            uint rowCount = 2;

            foreach (var row in GetRows(item.Value.AccessAnalytics)) {
                count = 1;
                foreach (var rowValue in row) {
                    var colName = GetColName(count);
                    count++;
                    Cell cell = InsertCellInWorksheet(colName, rowCount, worksheetPart);

                    if (rowValue is string str) {
                        cell.CellValue = new CellValue(str);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        continue;
                    }

                    if (rowValue is int i) {
                        cell.CellValue = new CellValue(i);
                        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        continue;
                    }
                }
                
                rowCount++;
            }

            worksheetPart.Worksheet.Save();

            sheets.Append(sheet);
        }

        workbookPart.Workbook.Save();
    }

    private static string GetColName(int index) {
        if (index < 1) {
            throw new ArgumentException("Index should be greater than 0.");
        }

        string column = string.Empty;

        while (index > 0) {
            int remainder = (index - 1) % 26;
            column = (char)('A' + remainder) + column;
            index = (index - 1) / 26;
        }

        return column;
    }

    // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
    // If the cell already exists, returns it. 
    private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart) {
        Worksheet worksheet = worksheetPart.Worksheet;
        SheetData sheetData = worksheet.GetFirstChild<SheetData>();
        string cellReference = columnName + rowIndex;

        // If the worksheet does not contain a row with the specified row index, insert one.
        Row row;
        if (sheetData.Elements<Row>().Any(r => r.RowIndex == rowIndex)) {
            row = sheetData.Elements<Row>().First(r => r.RowIndex == rowIndex);
        } else {
            row = new Row() { RowIndex = rowIndex };
            sheetData.Append(row);
        }

        // If there is not a cell with the specified column name, insert one.  
        if (row.Elements<Cell>().Any(c => c.CellReference.Value == columnName + rowIndex)) {
            return row.Elements<Cell>().First(c => c.CellReference.Value == cellReference);
        } else {
            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell refCell = null;
            foreach (Cell cell in row.Elements<Cell>()) {
                if (cell.CellReference.Value.Length == cellReference.Length) {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0) {
                        refCell = cell;
                        break;
                    }
                }
            }

            Cell newCell = new() { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);

            return newCell;
        }
    }

    private static IEnumerable<string> GetColumns(SomeObjectAccessAnalytic sybrinObjectAccess) {
        yield return "Name";

        foreach (var key in sybrinObjectAccess.Analytics.Keys) {
            yield return key;
        }
    }

    private static IEnumerable<object> GetRowValues(SomeObjectAccessAnalytic sybrinObjectAccess) {
        yield return sybrinObjectAccess.Name;

        foreach (var value in sybrinObjectAccess.Analytics.Values) {
            yield return value;
        }
    }

    private static IEnumerable<object[]> GetRows(IDictionary<string, SomeObjectAccessAnalytic> sybrinObjectAccess) {
        foreach (var item in sybrinObjectAccess) {
            var rowData = GetRowValues(item.Value);
            yield return rowData.ToArray();
        }
    }

    private static void ExportAccessAnalytics(ExcelPackage package, string sheetName, Dictionary<string, SomeObjectAccessAnalytic> accessAnalytics) {
        var cols = GetColumns(accessAnalytics.FirstOrDefault().Value).ToArray();

        package.Workbook.Worksheets.Add(sheetName);
        package.Workbook.Worksheets[sheetName].Cells["A1"].LoadFromArrays(new[] { cols });

        var rows = GetRows(accessAnalytics);
        package.Workbook.Worksheets[sheetName].Cells["A2"].LoadFromArrays(rows);
    }
}
