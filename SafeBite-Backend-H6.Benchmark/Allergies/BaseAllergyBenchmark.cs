using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SafeBite_Backend_H6.API.Data.AppDb;
using SafeBite_Backend_H6.API.Interfaces.Services;
using SafeBite_Backend_H6.API.Repositories;
using SafeBite_Backend_H6.API.Services.Allergies;

namespace SafeBite_Backend_H6.Benchmark.Allergies;

public abstract class BaseAllergyBenchmark
{
    protected AppDbContext DbContext = null!;
    protected IAllergyService AllergyService = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config["ConnectionStrings:AppConnection"] ?? throw new InvalidOperationException("Connection string 'AppConnection' was not found.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        DbContext = new AppDbContext(options);

        var repository = new AllergyRepository(DbContext);
        AllergyService = new AllergyService(repository);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        DbContext.Dispose();
    }
}
