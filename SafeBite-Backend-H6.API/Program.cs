

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppConnection")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthConnection")));



// identity
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();


// https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0
// follow microsoft's reccomendation.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // proper status code instead of 503
    var adminLimit = builder.Configuration.GetValue<int>("RateLimiting:Global:AdminPermitLimit");
    var userLimit = builder.Configuration.GetValue<int>("RateLimiting:Global:UserPermitLimit");
    var scanLimit = builder.Configuration.GetValue<int>("RateLimiting:Scan:PermitLimit");

    // global limit applies different limits for admins and non-admins.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var isAdmin = httpContext.User.IsInRole("Admin");
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) // userId from token
        ?? httpContext.Connection.RemoteIpAddress?.ToString() // if user is not found we use ip address as the specific limiter key.
        ?? "anonymous"; // last fallback if the client for some reason do not have an ip. 
        var partitionKey = isAdmin ? $"Admin:{userId}" : $"Standard:{userId}";
        // need userId or everyone would share the same limit pool. (user1 and user2 would have 40 request total together instead of 40 each)

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = isAdmin ? adminLimit : userLimit,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });

    // specific limit for scan
    options.AddPolicy(RateLimitPolicyNames.Scan, httpContext =>
    {
        // we need to seperate the users again or everyone will share the same limit of 10.
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            $"Scan:{userId}",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = scanLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 4,
                AutoReplenishment = true
            });
    });
});


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

builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<IAiExtractor, AiExtractor>();

builder.Services.AddScoped<IAllergyService, AllergyService>();
builder.Services.AddScoped<IAllergyUserService, AllergyUserService>();
builder.Services.AddScoped<ICustomAllergyService, CustomAllergyService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAllergyRepository, AllergyRepository>();
builder.Services.AddScoped<IAllergyUserRepository, AllergyUserRepository>();
builder.Services.AddScoped<ICustomAllergyRepository, CustomAllergyRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();

builder.Services.AddScoped<IUserAllergyAnalysisService, UserAllergyAnalysisService>();

builder.Services.AddScoped<IScanRepository, ScanRepository>();
builder.Services.AddScoped<IScanService, ScanService>();
builder.Services.AddScoped<IAllergyMatcher, AllergyMatcher>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var useFakeOcr = builder.Configuration.GetValue<bool>("Testing:UseFakeOcr");
if (useFakeOcr)
{
    builder.Services.AddScoped<IOcrService, FakeOcrService>();
}
else
{
    builder.Services.AddScoped<IOcrService, OcrService>();
}

var useFakeScan = builder.Configuration.GetValue<bool>("Testing:UseFakeScan");
if (useFakeScan)
{
    builder.Services.AddScoped<IScanAnalysisService, FakeScanAnalysisService>();
}
else
{
    builder.Services.AddScoped<IScanAnalysisService, ScanAnalysisService>();
}

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
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var config = services.GetRequiredService<IConfiguration>();
    var authDbContext = services.GetRequiredService<AuthDbContext>();
    var appDbContext = services.GetRequiredService<AppDbContext>();

    await authDbContext.Database.MigrateAsync();
    await appDbContext.Database.MigrateAsync();

    await RoleSeeding.SeedRolesAsync(roleManager);
    await UserSeeding.SeedAdminAsync(userManager, config);
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

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();
app.MapGroup("/auth").MapCustomIdentityApi<ApplicationUser>();

app.MapPost("/logout", async ([FromServices] SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

app.Run();
