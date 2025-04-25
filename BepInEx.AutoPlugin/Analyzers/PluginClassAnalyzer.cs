using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BepInEx.AutoPlugin.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PluginClassAnalyzer : DiagnosticAnalyzer
{
    const string Category = "BepInEx.AutoPlugin";

    public static readonly DiagnosticDescriptor PluginClassMustBeMarkedPartial = new(
        "AutoPlugin0001",
        "Plugin class must be marked partial",
        "Plugin class '{0}' must be marked partial for use with AutoPlugin",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(PluginClassMustBeMarkedPartial);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;

        var semanticModel = context.SemanticModel;
        var compilation = context.Compilation;

        INamedTypeSymbol?[] autoAttributes =
        [
            compilation.GetTypeByMetadataName(AutoPluginGenerator.BepInAutoPluginAttribute),
            compilation.GetTypeByMetadataName(AutoPluginGenerator.PatcherAutoPluginAttribute),
        ];

        var attributes = classSyntax.AttributeLists.SelectMany(attrList => attrList.Attributes);
        var hasAutoPluginAttribute = attributes.Any(attribute =>
        {
            if (semanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                return false;

            foreach (var autoAttribute in autoAttributes)
            {
                if (
                    SymbolEqualityComparer.Default.Equals(
                        attributeSymbol.ContainingSymbol,
                        autoAttribute
                    )
                )
                {
                    return true;
                }
            }

            return false;
        });

        if (!hasAutoPluginAttribute)
            return;

        if (classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            return;

        var diagnostic = Diagnostic.Create(
            PluginClassMustBeMarkedPartial,
            classSyntax.Identifier.GetLocation(),
            classSyntax.Identifier.ToString()
        );

        context.ReportDiagnostic(diagnostic);
    }
}
