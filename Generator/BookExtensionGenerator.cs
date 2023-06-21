using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generator;

[Generator]
public sealed class BookExtensionGenerator : IIncrementalGenerator
{
	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
			predicate: (s, _) => s is ClassDeclarationSyntax,
			transform: GetTypeSymbols).Collect();

		context.RegisterSourceOutput(classDeclarations, GenerateSource);
	}

	private static void GenerateSource(SourceProductionContext context, ImmutableArray<INamedTypeSymbol?> items)
	{
		// if (items.All(item => item is null))
		// {
		// 	return;
		// }

		StringBuilder sb = new();

		sb.AppendLine("using HotChocolate;");
		sb.AppendLine("using HotChocolate.Types;");
		sb.AppendLine();
		sb.AppendLine("namespace Server.Types;");
		sb.AppendLine();
		sb.AppendLine("[ExtendObjectType<Book>]");
		sb.AppendLine("public static class BookExtensions");
		sb.AppendLine("{");
		//sb.AppendLine("\t[GraphQLNonNullType]");
		sb.AppendLine("\tpublic static Author Mastermind([Parent] Book book) => book.Author;");
		sb.AppendLine("}");
		
		context.AddSource("BookExtensions.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
	}

	private static INamedTypeSymbol? GetTypeSymbols(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var decl = (ClassDeclarationSyntax) context.Node;

		if (context.SemanticModel.GetDeclaredSymbol(decl, cancellationToken) is INamedTypeSymbol { Name: "Book" } classSymbol)
		{
			return classSymbol;
		}

		return null;
	}
}