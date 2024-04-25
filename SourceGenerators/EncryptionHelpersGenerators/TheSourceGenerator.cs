//using System.Collections.Immutable;

//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace EncryptionHelpersGenerators;

//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
//public sealed class EncryptAttribute : Attribute { }

//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
//public sealed class EncryptLvl2Attribute : Attribute { }

//[Generator]
//public class TheSourceGenerator : IIncrementalGenerator {

//    public void Initialize(IncrementalGeneratorInitializationContext context) {
//        var provider = context.SyntaxProvider.CreateSyntaxProvider(
//            predicate: static (node, _) => node is ClassDeclarationSyntax,
//            transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
//        ).Where(node => node is not null)
//        .Where(HasEncryptionAttribute);

//        var compilation = context.CompilationProvider.Combine(provider.Collect());

//        context.RegisterSourceOutput(compilation, GenerateSource);


//        var lvl2EncryptionProvider = context.SyntaxProvider.CreateSyntaxProvider(
//            predicate: static (node, _) => node is ClassDeclarationSyntax,
//            transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
//        ).Where(node => node is not null)
//        .Where(HasEncryptionLvl2Attribute);

//        var compilationlvl2 = context.CompilationProvider.Combine(provider.Collect());

//        context.RegisterSourceOutput(compilation, GenerateSource);
//    }

//    private bool HasEncryptionAttribute(ClassDeclarationSyntax syntax) {
//        if (syntax.AttributeLists.Count == 0) return false;

//        var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes);

//        var encryptionAttribute = attributes.FirstOrDefault(x => x.Name.ToString() == "Encrypt");

//        if (encryptionAttribute is null) return false;

//        return true;
//    }

//    private bool HasEncryptionLvl2Attribute(ClassDeclarationSyntax syntax) {
//        if (syntax.AttributeLists.Count == 0) return false;

//        var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes);

//        var encryptionAttribute = attributes.FirstOrDefault(x => x.Name.ToString() == "EncryptLvl2");

//        if (encryptionAttribute is null) return false;

//        return true;
//    }

//    private void GenerateSource(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) tuple) {
//        var (compilation, list) = tuple;

//        var nameList = new List<(string name, string ns)>();

//        foreach (var syntax in list) {
//            var model = compilation
//                .GetSemanticModel(syntax.SyntaxTree);

//            var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;

//            if (symbol is null) continue;

//            // log if class is not a partial class
//            nameList.Add((symbol.Name, symbol.ContainingNamespace.Name));
//        }

//        foreach (var names in nameList) {
//            var (name, ns) = names;
//            Console.WriteLine($"Class {name} in namespace {ns} has the Encrypt attribute");
//            var properties = list.First(x => x.Identifier.Text == name).Members.OfType<PropertyDeclarationSyntax>();

//            foreach (var property in properties) {
//                Console.WriteLine($"Property {property.Identifier.Text} is encrypted");
//            }

//            var classScript = $$"""
//            namespace {{ns}};
//            public partial class {{name}} {
//                public {{name}} Encrypt() {
//                    // encrypting the values
//                    Console.WriteLine("Encrypting the values");
//                    return this;
//                }
//            }
//            """;

//            context.AddSource($"{name}.encryption.g.cs", classScript);
//        }

//        var thecode = $$$""""
//            namespace TheGenerator;
//            public class TheGeneratedClass {
//                public static string TheGeneratedMethod() {
//                    return """
//                    {{{string.Join(",", nameList)}}}
//                    """;
//                }
//            }
//            """";

//        context.AddSource("TheGeneratedClass.g.cs", thecode);
//    }
//}


//public partial class ObjectToEncrypt {
//    public string Name { get; set; }
//    public string Value { get; set; }
//}

using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sybrin.Core.SourceGeneration;

[Generator]
public class HelloSourceGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => node is InterfaceDeclarationSyntax,
            transform: (ctx, _) => (InterfaceDeclarationSyntax)ctx.Node
        ).Where(node => node is not null).Where(HasPIIAwareAttribute);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, GeneratePIIAwareWrappers);
    }

    private void GeneratePIIAwareWrappers(SourceProductionContext context, (Compilation Left, ImmutableArray<InterfaceDeclarationSyntax> Right) tuple) {
        var (compilation, list) = tuple;

        foreach (var syntaxNode in list) {
            var model = compilation.GetSemanticModel(syntaxNode.SyntaxTree);

            var getUsingsForClass = syntaxNode.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();

            var symbol = model.GetDeclaredSymbol(syntaxNode) as INamedTypeSymbol;

            if (symbol is null) continue;

            var name = symbol.Name.AsSpan();
            var startIndex = name.IndexOf("I".AsSpan());

            if (startIndex > -1) {
                name = name.Slice(startIndex + 1);
            }

            var className = $"PIIAware{name.ToString()}";

            var text = $$"""
            namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                public partial class {{className}} : {{symbol.Name}} {
                    public {{className}}({{symbol.Name}} @internal) {
                        Internal = @internal;
                    }

                    public {{symbol.Name}} Internal { get; }
                }
            }
            """ + "\n";

            var doesImplementIDisposable = symbol.AllInterfaces.Any(x => x.Name == "IDisposable");

            if (doesImplementIDisposable) {
                text += $$"""
                namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                    public partial class {{className}} : {{symbol.Name}} {
                        public void Dispose() {
                            Internal.Dispose();
                        }
                    }
                }
                """ + "\n";
            }

            var doesImplementIUnitOfWork = symbol.AllInterfaces.Any(x => x.Name == "IUnitOfWork");

            if (doesImplementIUnitOfWork) {
                text += $$"""
                namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                    public partial class {{className}} : {{symbol.Name}} {
                        public void Commit() {
                            Internal.Commit();
                        }

                        public void Rollback() {
                            Internal.Rollback();
                        }
                    }
                }
                """ + "\n";
            }

            var properties = symbol.GetMembers().OfType<IPropertySymbol>();
            var propertiesOnInterfaces = symbol.AllInterfaces.SelectMany(x => x.GetMembers().OfType<IPropertySymbol>());
            var combinedProperties = properties.Concat(propertiesOnInterfaces);
            //var propertiesOnInterfaces = symbol.GetMembers().OfType<IPropertySymbol>();

            foreach (var property in properties) {
                var doesPropertyHavePIIAwareAttribute = property.GetAttributes().Any(x => x.AttributeClass.Name == "PIIAwareAttribute");
                var propertyIdentifier = property.Name.AsSpan();
                    
                    //property.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == "PIIAware");

                if (!doesPropertyHavePIIAwareAttribute) {
                    text += $$"""
                
                            namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                                public partial class {{className}} : {{symbol.Name}} {
                                    public {{property.Type}} {{property.Name}} {
                                        get => Internal.{{property.Name}};
                                    }
                                }
                            }
                            """ + "\n";

                    continue;
                }

                var isString = property.Type.ToString() == "string";

                var implementationName = property.Type.ToString().AsSpan();
                startIndex = implementationName.IndexOf("I".AsSpan());

                if (startIndex > -1) {
                    implementationName = implementationName.Slice(startIndex + 1);
                }

                var namespaceForProperty = property.Type.ContainingNamespace.ToDisplayString();
                
                var piiAwareWrapper = $"PIIAware{implementationName.ToString()}";

                text += $$"""
                        namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                            public partial class {{className}} : {{symbol.Name}} {
                                public /*{{namespaceForProperty}}*/ {{property.Type}} {{property.Name}} {
                                    get => new {{piiAwareWrapper}}(Internal.{{property.Name}});
                                }
                            }
                        }
                        """ + "\n";
            }

            var methods = syntaxNode.Members.OfType<MethodDeclarationSyntax>();

            foreach (var method in methods) {
                var isReturnTypeVoid = method.ReturnType.ToString() == "void";

                var returnText= isReturnTypeVoid ? "" : "return ";

                var doesReturnTypeHavePIIAwareAttribute  = compilation.GetSemanticModel(method.SyntaxTree).GetDeclaredSymbol(method.ReturnType) is INamedTypeSymbol returnTypeSymbol && returnTypeSymbol.GetAttributes().Any(x => x.AttributeClass.Name == "PIIAwareAttribute");

                text += $$"""
                namespace {{symbol.ContainingNamespace.ToDisplayString()}} {
                    public partial class {{className}} : {{symbol.Name}} {
                        public {{method.ReturnType}} {{method.Identifier}}({{string.Join(", ", method.ParameterList.Parameters)}}) {
                            {{returnText}} Internal.{{method.Identifier}}({{string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier))}});
                        }
                    }
                }
                """;
            }

            context.AddSource($"{className}.g.cs", text);
        }
    }

    public bool HasPIIAwareAttribute(InterfaceDeclarationSyntax node) {
        if (node.AttributeLists.Count == 0) return false;

        var attributes = node.AttributeLists.SelectMany(x => x.Attributes);

        var encryptionAttribute = attributes.FirstOrDefault(x => x.Name.ToString() == "PIIAware");

        if (encryptionAttribute is null) return false;

        return true;
    }
}

[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class PIIDecryptAttribute : Attribute {
    public PIIDecryptAttribute() { }
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class PIIAwareAttribute : Attribute {
    public PIIAwareAttribute() { }
}

[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class PIIEncryptAttribute : Attribute {
    public PIIEncryptAttribute() { }
}
