using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyFinance.CodeAnalysis.SourceGenerators;

[Generator]
public class ApiIdSourceGenerator : IIncrementalGenerator
{
    private const string Attribute =
        """
        namespace MyFinance.CodeAnalysis;

        [AttributeUsage(AttributeTargets.Struct)]
        public sealed class ApiIdAttribute : Attribute;
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("ApiIdAttribute.g.cs", SourceText.From(Attribute, Encoding.UTF8));
        });

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is RecordDeclarationSyntax rds &&
                    rds.AttributeLists
                        .Any(al => al.Attributes
                            .Any(a => a.Name.ToString().EndsWith("ApiId"))),
                transform: static (ctx, _) => ctx)
            .Collect();

        var compilationAndCandidates = context.CompilationProvider.Combine(candidates);

        context.RegisterSourceOutput(compilationAndCandidates, (spc, source) =>
        {
            var (compilation, list) = source;
            foreach (var syntaxContext in list)
            {
                var rds = (RecordDeclarationSyntax)syntaxContext.Node;
                var generatedSource = GenerateSourceCode(compilation, rds);
                spc.AddSource($"{rds.Identifier.Text}.g.cs", SourceText.From(generatedSource, Encoding.UTF8));
            }
        });
    }

    private static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        var nameSpace = string.Empty;
        var potentialNamespaceParent = syntax.Parent;
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            nameSpace = namespaceParent.Name.ToString();
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }
        return nameSpace;
    }

    private static string GenerateSourceCode(Compilation compilation, RecordDeclarationSyntax rds)
    {
        var className = rds.Identifier.Text;
        var namespaceName = GetNamespace(rds);
        var model = compilation.GetSemanticModel(rds.SyntaxTree);
        var rawIdTypeSyntax = rds.ParameterList!.Parameters[0].Type!;
        var rawIdTypeSymbol = model.GetSymbolInfo(rawIdTypeSyntax).Symbol as INamedTypeSymbol;
        var isEnum = rawIdTypeSymbol?.TypeKind == TypeKind.Enum;
        var rawIdType = rawIdTypeSymbol?.ToDisplayString();

        var newFactoryMethod = isEnum
            ? ""
            : $"public static {className} New() => new(Guid.NewGuid());";

        var emptyFactoryMethod = isEnum
            ? ""
            : $"public static {className} Empty {{ get; }} = new(Guid.Empty);";

        var defaultConstructor = isEnum
            ? ""
            : $"public {className}() : this(Guid.NewGuid()) {{ }}";

        var parseMethod = isEnum
            ? $"var enumValue = Enum.Parse<{rawIdType}>(id[Prefix.Length..], true);"
            : $"var guid = Guid.Parse(id[Prefix.Length..]);";

        var tryParseMethod = isEnum
            ? $"if (Enum.TryParse<{rawIdType}>(id[Prefix.Length..], true, out var enumValue))"
            : $"if (Guid.TryParse(id[Prefix.Length..], out var guid))";

        var newIdInstance = isEnum
            ? $"new {className}(enumValue)"
            : $"new {className}(guid)";

        var toStringMethod = isEnum
            ? "return Prefix + RawId.ToString().ToLowerInvariant();"
            : "return Prefix + RawId.ToString(\"N\");";

        var alternateToStringMethod = isEnum
            ? ""
            : """public string ToCompactGuidString() => RawId.ToString("N");""";

        var source =
            $$"""
              using System;
              using System.Text.Json;
              using System.Text.Json.Serialization;

              namespace {{namespaceName}};

              [JsonConverter(typeof({{className}}Converter))]
              public readonly partial record struct {{className}}
              {
                  {{newFactoryMethod}}
                  {{emptyFactoryMethod}}

                  {{defaultConstructor}}

                  public static {{className}} Parse(string id)
                  {
                      if (!id.StartsWith(Prefix))
                      {
                          throw new ArgumentException($"Id must start with '{Prefix}'", nameof(id));
                      }

                      {{parseMethod}}
                      return {{newIdInstance}};
                  }

                  public static bool TryParse(string id, out {{className}} idInstance)
                  {
                      if (!id.StartsWith(Prefix))
                      {
                          idInstance = new {{className}}(default);
                          return false;
                      }

                      {{tryParseMethod}}
                      {
                          idInstance = {{newIdInstance}};
                          return true;
                      }

                      idInstance = new {{className}}(default);
                      return false;
                  }

                  public override string ToString()
                  {
                      {{toStringMethod}}
                  }

                  public static implicit operator {{rawIdType}}({{className}} d)
                  {
                      return d.RawId;
                  }

                  public static implicit operator {{className}}({{rawIdType}} d)
                  {
                      return new {{className}}(d);
                  }

                  {{alternateToStringMethod}}

                  public class {{className}}Converter : JsonConverter<{{className}}>
                  {
                      public override {{className}} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                      {
                          var idString = reader.GetString();

                          if (idString is null)
                          {
                              throw new JsonException("Expected string missing");
                          }

                          if (!{{className}}.TryParse(idString, out var id))
                          {
                              throw new JsonException("Invalid id format");
                          }

                          return id;
                      }

                      public override void Write(Utf8JsonWriter writer, {{className}} value, JsonSerializerOptions options)
                      {
                          writer.WriteStringValue(value.ToString());
                      }
                  }
              }
              """;

        return source;
    }
}
