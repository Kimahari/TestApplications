// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");


var defaultValueArrayMaps = new Dictionary<Type, object>() {
    { typeof(bool), false },
    { typeof(short), (short)0 },
    { typeof(int), 0 },
    { typeof(long), 0L },
    { typeof(double), 0D },
    { typeof(decimal), 0m },
    { typeof(string), string.Empty }
}!;

DoBulkInsertStuffs();

void DoBulkInsertStuffs() {
    var t = typeof(int);

    var values = new List<object>() { (Int32)2, new NoMapping(), 2 };

    var nullable = typeof(Nullable<>).MakeGenericType(t);
    var data = Activator.CreateInstance(typeof(List<>).MakeGenericType(nullable))!;

    var dd = data as dynamic;

    List<bool?> d = []; ;

    d.Add(null);

    foreach (var value in values) {
        if (value is NoMapping) {
            //HandleMappingObjectValue(data);
            dd.Add(null);
            //dd.Add(GetNoMappingObjectValue(data, value));
        } else if (value is null && defaultValueArrayMaps.TryGetValue(t, out var @null)) {
            dd.Add(@null as dynamic);
        } else if (value is string str && t == typeof(byte[])) {
            var byteValue = Encoding.UTF8.GetBytes(str);
            dd.Add(byteValue as dynamic);
        } else {
            dd.Add(value as dynamic);
        }
    }
}



record NoMapping;

