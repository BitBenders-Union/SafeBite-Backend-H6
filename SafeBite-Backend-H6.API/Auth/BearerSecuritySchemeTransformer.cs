

namespace SafeBite_Backend_H6.API.Auth;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/customize-openapi?view=aspnetcore-10.0
// https://yogeshhadiya33.medium.com/implement-scalar-in-net-api-91d284479d1d
// taken from these 2 sites
// tells scalar to use a bearer token for authentication
// allows us to insert token in scalar and have it automatically added to the header of each request,

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var schemeName = IdentityConstants.BearerScheme;
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

        if (!authenticationSchemes.Any(authScheme => authScheme.Name == schemeName))
            return;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [schemeName] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header
            }
        };

        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
        {
            operation.Value.Security ??= [];
            operation.Value.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(schemeName, document)] = []
            });
        }
    }
}
