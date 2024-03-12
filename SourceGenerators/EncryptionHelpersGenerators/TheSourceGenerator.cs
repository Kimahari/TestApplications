using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EncryptionHelpersGenerators;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class EncryptAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class EncryptLvl2Attribute : Attribute { }

[Generator]
public class TheSourceGenerator : IIncrementalGenerator {


    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
        ).Where(node => node is not null)
        .Where(HasEncryptionAttribute);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, GenerateSource);
    }

    private bool HasEncryptionAttribute(ClassDeclarationSyntax syntax) {
        if (syntax.AttributeLists.Count == 0) return false;

        var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes);

        var encryptionAttribute = attributes.FirstOrDefault(x => x.Name.ToString() == "Encrypt");

        if (encryptionAttribute is null) return false;

        return true;
    }

    private void GenerateSource(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) tuple) {
        var (compilation, list) = tuple;

        var nameList = new List<(string name, string ns)>();

        foreach (var syntax in list) {
            var model = compilation
                .GetSemanticModel(syntax.SyntaxTree);

            var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;

            if (symbol is null) continue;

            // log if class is not a partial class
            nameList.Add((symbol.Name, symbol.ContainingNamespace.Name));
        }

        foreach (var names in nameList) {
            var (name, ns) = names;
            Console.WriteLine($"Class {name} in namespace {ns} has the Encrypt attribute");
            var properties = list.First(x => x.Identifier.Text == name).Members.OfType<PropertyDeclarationSyntax>();

            foreach (var property in properties) {
                Console.WriteLine($"Property {property.Identifier.Text} is encrypted");
            }

            var classScript = $$"""
            namespace {{ns}};
            public partial class {{name}} {
                public {{name}} Encrypt() {
                    // encrypting the values
                    Console.WriteLine("Encrypting the values");
                    return this;
                }
            }
            """;

            context.AddSource($"{name}.encryption.g.cs", classScript);
        }

        var thecode = $$$""""
            namespace TheGenerator;
            public class TheGeneratedClass {
                public static string TheGeneratedMethod() {
                    return """
                    {{{string.Join(",", nameList)}}}
                    """;
                }
            }
            """";

        context.AddSource("TheGeneratedClass.g.cs", thecode);
    }
}


public partial class ObjectToEncrypt {
    public string Name { get; set; }
    public string Value { get; set; }
}