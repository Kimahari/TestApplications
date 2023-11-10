// See https://aka.ms/new-console-template for more information
using System.Numerics;
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


void HandleMappingObjectValue(object data) {
    if (data is IList<int> il) il.Add(0);
    if (data is IList<decimal> dl) dl.Add(0);
    if (data is IList<short> sl) sl.Add(0);
    if (data is IList<bool> bl) bl.Add(false);
    if (data is IList<long> ll) ll.Add(0);
    if (data is IList<double> dbl) dbl.Add(0);
    if (data is IList<byte[]> bal) bal.Add(Array.Empty<byte>());

    (data as dynamic).Add(null);
}


DoBulkInsertStuffs();


Type GetGenericTypeForNullable(Type type) {
    if (type == typeof(string)) return type;
    if (type == typeof(byte[])) return type;
    
    var nullable = typeof(Nullable<>).MakeGenericType(type);

    return nullable;
}

void DoBulkInsertStuffs() {
    var t = typeof(int);

    var values = new List<object>() { (Int32)2, new NoMapping(), 2 };

    var nullable = typeof(Nullable<>).MakeGenericType(t);
    var data = Activator.CreateInstance(typeof(List<>).MakeGenericType(nullable))!;

    var dd = data as dynamic;

    List<bool?> d = new(); ;

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

