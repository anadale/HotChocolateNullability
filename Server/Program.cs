using Server.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddGraphQLServer()
	.AddTypes()
	.AddTypeExtension(typeof(Server.Types.BookExtensions));

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);