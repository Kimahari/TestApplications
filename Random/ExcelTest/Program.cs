using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using OfficeOpenXml;

namespace ExcelTest {

    public class SybrinObjectAccessAnalytic {
        public string Name { get; set; }
        public int AccessedCount { get => 10; }
        public Dictionary<string, int> Analytics { get; set; } = new Dictionary<string, int>();
    }

    public class SybrinObject {

        public Func<string, Task<string>> ResolveNameCallback;
        public int Count { get; set; }
        public Dictionary<string, int> OverviewAnalytics { get; set; } = new Dictionary<string, int>();
        public int AccessedCount { get => AccessAnalytics.Count; }
        public Dictionary<string, SybrinObjectAccessAnalytic> AccessAnalytics { get; set; } = new Dictionary<string, SybrinObjectAccessAnalytic>();
    }


    internal class Program {
        static void Main(string[] args) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var objects = new Dictionary<string, SybrinObject>();
            var appId = Guid.NewGuid().ToString();

            objects["Documents"] = new SybrinObject();
            objects["Documents"].AccessAnalytics = new Dictionary<string, SybrinObjectAccessAnalytic> {
                [appId] = new SybrinObjectAccessAnalytic {
                    Name = "App1",
                    Analytics = new Dictionary<string, int>() {
                        {"JAN",10},
                        {"FEB",10},
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

        public static void CreateSpreadsheetWorkbook(string filepath, Dictionary<string, SybrinObject> data) {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            foreach (var item in data) {
                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() {
                    Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = item.Key
                };

                var cols = GetColumns(item.Value.AccessAnalytics.FirstOrDefault().Value).ToArray();

                var count = 1;

                foreach (var col in cols) {
                    var colName = GetColname(count);
                    count++;
                    Cell cell = InsertCellInWorksheet(colName, 1, worksheetPart);
                    cell.CellValue = new CellValue(col);
                    cell.DataType = new EnumValue<CellValues>(CellValues.String);
                }

                uint rowCount = 2;

                foreach (var row in GetRows(item.Value.AccessAnalytics)) {
                    count = 1;
                    foreach (var rowValue in row) {
                        var colName = GetColname(count);
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

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument?.Close();
        }

        private static string GetColname(int index) {
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
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0) {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            } else {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0) {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
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

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart) {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null) {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>()) {
                if (item.InnerText == text) {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        private static IEnumerable<string> GetColumns(SybrinObjectAccessAnalytic sybrinObjectAccess) {
            yield return "Name";

            foreach (var key in sybrinObjectAccess.Analytics.Keys) {
                yield return key;
            }
        }

        private static IEnumerable<object> GetRowValues(SybrinObjectAccessAnalytic sybrinObjectAccess) {
            yield return sybrinObjectAccess.Name;

            foreach (var value in sybrinObjectAccess.Analytics.Values) {
                yield return value;
            }
        }

        private static IEnumerable<object[]> GetRows(IDictionary<string, SybrinObjectAccessAnalytic> sybrinObjectAccess) {
            foreach (var item in sybrinObjectAccess) {
                var rowData = GetRowValues(item.Value);
                yield return rowData.ToArray();
            }
        }

        private static void ExportAccessAnalytics(ExcelPackage package, string sheetName, Dictionary<string, SybrinObjectAccessAnalytic> accessAnalytics) {
            var cols = GetColumns(accessAnalytics.FirstOrDefault().Value).ToArray();

            package.Workbook.Worksheets.Add(sheetName);
            package.Workbook.Worksheets[sheetName].Cells["A1"].LoadFromArrays(new[] { cols });

            var rows = GetRows(accessAnalytics);
            package.Workbook.Worksheets[sheetName].Cells["A2"].LoadFromArrays(rows);
        }
    }
}
