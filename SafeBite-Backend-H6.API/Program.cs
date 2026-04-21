
using SafeBite_Backend_H6.API.Interfaces.Services.OCR;
using SafeBiteApi.Services.OCR;
using SafeBiteApi.Services.OCR.Engines;
using SafeBiteApi.Services.OCR.Helpers;
using SafeBite_Backend_H6.API.Auth;
using Scalar.AspNetCore;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppConnection")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthConnection")));

// Add services to the container.
builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<IAiExtractor, AiExtractor>();
builder.Services.AddScoped<IOcrService, OcrService>();

// identity
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();




// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});





// Service Registrations
# region Service Registrations

builder.Services.AddTransient<IEmailSender, EmailSender>();



# endregion



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi(options =>
{
    // scalar bearer token support - lets us set token once and have it apply to all endpoints in the docs
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// seeding setup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var appDbContext = services.GetRequiredService<AppDbContext>();

    await appDbContext.Database.MigrateAsync();

    await RoleSeeding.SeedRolesAsync(roleManager);
    await AllergySeeding.SeedAllergiesAsync(appDbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // scalar api docs
    app.MapScalarApiReference(options =>
    {
        options.Title = "SafeBite API";
        options.PersistentAuthentication = true;
        options.Theme = ScalarTheme.DeepSpace;
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["Bearer"]
        };
    });
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAllOrigins");


app.MapControllers();
app.MapGroup("/auth").MapCustomIdentityApi<ApplicationUser>();

app.MapPost("/logout", async ([FromServices] SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

app.Run();
