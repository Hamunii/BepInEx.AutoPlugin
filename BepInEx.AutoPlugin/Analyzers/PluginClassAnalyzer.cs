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

        context.RegisterSymbolAction(AnalyzeClassSymbol, SymbolKind.NamedType);
    }

    static void AnalyzeClassSymbol(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        bool isNotClassOrIsPartialClass = typeSymbol.DeclaringSyntaxReferences.All(
            static syntaxReference =>
                syntaxReference.GetSyntax() is not ClassDeclarationSyntax classSyntax
                || classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
        );

        if (isNotClassOrIsPartialClass)
        {
            return;
        }

        foreach (AttributeData attribute in typeSymbol.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
            {
                continue;
            }

            switch (attributeClass.Name)
            {
                case AutoPluginGenerator.BepInAutoPluginAttributeName:
                    string attributeFullName = attributeClass.ToDisplayString();
                    if (attributeFullName != AutoPluginGenerator.BepInAutoPluginAttributeFullName)
                    {
                        continue;
                    }
                    break;

                case AutoPluginGenerator.PatcherAutoPluginAttributeName:
                    attributeFullName = attributeClass.ToDisplayString();
                    if (attributeFullName != AutoPluginGenerator.PatcherAutoPluginAttributeFullName)
                    {
                        continue;
                    }
                    break;

                default:
                    continue;
            }

            if (
                attribute.ApplicationSyntaxReference?.GetSyntax()
                is not AttributeSyntax attributeSyntax
            )
            {
                continue;
            }

            if (
                attributeSyntax.Parent is not AttributeListSyntax attributeList
                || attributeList.Parent is not ClassDeclarationSyntax classDeclaration
            )
            {
                continue;
            }

            var diagnostic = Diagnostic.Create(
                PluginClassMustBeMarkedPartial,
                classDeclaration.Identifier.GetLocation(),
                classDeclaration.Identifier.ToString()
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}
