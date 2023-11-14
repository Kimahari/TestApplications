// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Reflection.Emit;

Console.WriteLine("Hello, World!");

Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));
Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));
Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));
Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));
Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));
Console.WriteLine(ILMapper.Map<B>(new A { Id = 1, Name = "Test" }));

Console.ReadLine();

public class ILMapper {
    public static Dictionary<(Type, Type), MethodInfo> _cache = [];
    public static TDestination Map<TDestination>(object source) {
        var key = (source.GetType(), typeof(TDestination));
        var (sourceType, destinationType) = key;

        if (!_cache.TryGetValue(key, out var methodInfo)) {
            _cache[key] = methodInfo = CreateMapperMathod(sourceType, destinationType);
        }

        return (TDestination)methodInfo.Invoke(null, [source]);
    }

    static MethodInfo CreateMapperMathod(Type sourceType, Type destinationType) {
        AssemblyName aName = new("DynamicAssemblyExample");
        AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name!);

        var typeBuilder = mb.DefineType("Mapper", TypeAttributes.NotPublic);
        var mapMethod = typeBuilder.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static, destinationType, [sourceType]);

        var generator = mapMethod.GetILGenerator();
        generator.Emit(OpCodes.Newobj, destinationType.GetConstructor(Type.EmptyTypes));

        foreach (var prop in sourceType.GetProperties()) {
            var destinationProperty = destinationType.GetProperty(prop.Name);

            if (destinationProperty is null) continue;

            generator.Emit(OpCodes.Dup);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, prop.GetMethod);
            generator.Emit(OpCodes.Callvirt, destinationProperty.SetMethod);
        }

        generator.Emit(OpCodes.Ret);

        var type = typeBuilder.CreateType();

        var m = type.GetMethod("Map", BindingFlags.Static | BindingFlags.Public, [sourceType]);

        return m;
    }
}

public record A {
    public int Id { get; set; }
    public string Name { get; set; }
}

public record B {
    public int Id { get; set; }
    public string Name { get; set; }
}
//IL_0000 nop	
//IL_0001	newobj	B..ctor
//IL_0006	dup	
//IL_0007	ldarg.0	
//IL_0008	callvirt	A.get_Id ()
//IL_000D callvirt	B.set_Id (Int32)
//IL_0012 nop	
//IL_0013	dup	
//IL_0014	ldarg.0	
//IL_0015	callvirt	A.get_Prop2 ()
//IL_001A callvirt	B.set_Prop2 (Int32)
//IL_001F nop	
//IL_0020	stloc.0	
//IL_0021	br.s	IL_0023
//IL_0023	ldloc.0	
//IL_0024	ret	