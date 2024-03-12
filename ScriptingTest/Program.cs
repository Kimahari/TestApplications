using System.Xml;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

var script = @"
    public class Mapper : IMapper {
        public string Print(object obj){
            Console.WriteLine(""Hello World!!!"");
            return $""Test {obj}"";
        }
    }

    return typeof(Mapper);
";

Console.WriteLine("Hello, World!");

var options = ScriptOptions.Default
             .AddImports("System")
             .AddReferences(typeof(IMapper).Assembly);
 
var scriptExec = CSharpScript.Create(script, options);

var result = await scriptExec.RunAsync();

var type = result.ReturnValue as Type;

var ssss = Activator.CreateInstance(type) as IMapper;

Console.WriteLine($"Result was {ssss.Print("TestTest")}");

Console.ReadKey();

public interface IMapper {
    string Print(object obj);
}

class Globals {
    public int TestTes { get; set; }
}