
using AniVault.Api.Extensions;
using AniVault.Database.Context;
using AniVault.Services;
using AniVault.Services.Extensions;
using AniVault.Services.ScheduledTasks;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .SetupServices(builder.Configuration, builder.Environment)
    .SetupApiServices();

var app = builder.Build();
app.SetupApiConfiguration();

using (var scope = app.Services.CreateScope())
{
    using (var dbContext = scope.ServiceProvider.GetRequiredService<AniVaultDbContext>())
    {
        dbContext.Migrate();
    }

    scope.ServiceProvider.UseScheduler(scheduler =>
    {
        scheduler.Schedule<StartupTask>()
            .EverySecond()
            .Once()
            .PreventOverlapping(nameof(StartupTask));
    });
}

// var c = app.Services.GetService<TelegramClientService>();
//
// await c.Connect();

app.Run();



internal static class ApiExtensions
{
    private const string SecureTag = "secure";

    public static void UseSecurity(this RouteHandlerBuilder builder)
    {
        builder.WithMetadata(new RequireApiKeyAttribute())
            .WithTags(SecureTag);
    }
}
internal class RequireApiKeyAttribute : Attribute { }
internal sealed class ApiKeySecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "x-api-key",
            Description = "Chiave di accesso necessaria per chiamare endpoint protetti",
        };
        
        // Aggiungi la security requirement a ogni operazione che vuoi proteggere
        foreach (var pathItem in document.Paths.Values)
        {
            // Per ogni operazione HTTP
            foreach (var operation in pathItem.Operations.Values)
            {
                // Qui puoi mettere una condizione per scegliere quali operazioni proteggere, ad esempio path o tag
                if (operation.Tags != null && operation.Tags.Any(t=>t.Name == "secure"))
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey"
                                }
                            }
                            ,Array.Empty<string>()
                        }
                    });
                }
            }
        }
        return Task.CompletedTask;
    }

}