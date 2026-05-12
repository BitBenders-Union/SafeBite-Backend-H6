// https://nbomber.com/docs/getting-started/hello_world_tutorial
// follow the guide :)

// nbomber requires license for commercial use, but for this school project it should be fine :(

using Microsoft.Extensions.Configuration;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using SafeBite_Backend_H6.Load.Scenarios.Allergy;
using SafeBite_Backend_H6.Load.Scenarios.Scan;
using SafeBite_Backend_H6.Load.Utility;


public partial class LoadTestProgram
{
    private static async Task Main(string[] args)
    {


        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine("""
        WARNING:
        Make sure the Development API is deployed with fake OCR and fake scan analysis enabled.

        Run the API with:
        dotnet run --launch-profile loadtest

        The LoadTest environment uses fake OCR and AI services
        to avoid OpenAI token usage and external API calls.
        """);

        Console.ResetColor();

        Console.Write("Is the API running in LoadTest environment? (y/n): ");

        var input = Console.ReadLine();

        if (!string.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Load test cancelled.");
            return;
        }

        var config = new ConfigurationBuilder()
            .AddUserSecrets<LoadTestProgram>()
            .Build();

        var baseUrl = config["Api:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl missing.");

        var email = config["User:AdminEmail"] ?? throw new InvalidOperationException("Admin email is not configured.");
        var password = config["User:AdminPassword"] ?? throw new InvalidOperationException("Admin password is not configured.");

        var httpClient = Http.CreateDefaultClient();

        var token = await AuthHelper.GetBearerToken(baseUrl, httpClient, email, password);

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "noodlesdingredients.png");
        var imageBytes = await File.ReadAllBytesAsync(imagePath);

        var allergyGetScenarioLow = GetAllergyScenario.Create(httpClient, baseUrl, token, 1);
        var allergyGetScenarioMedium= GetAllergyScenario.Create(httpClient, baseUrl, token, 30);
        var allergyGetScenarioHigh = GetAllergyScenario.Create(httpClient, baseUrl, token, 100);

        var scanPostScenarioLow = PostScanScenario.Create(httpClient, baseUrl, token, imageBytes, 1);
        var scanPostScenarioMedium = PostScanScenario.Create(httpClient, baseUrl, token, imageBytes, 5);
        var scanPostScenarioHigh = PostScanScenario.Create(httpClient, baseUrl, token, imageBytes, 10);

        var scanGetScenarioLow = GetScanScenario.Create(httpClient, baseUrl, token, 1);
        var scanGetScenarioMedium = GetScanScenario.Create(httpClient, baseUrl, token, 30);
        var scanGetScenarioHigh = GetScanScenario.Create(httpClient, baseUrl, token, 100);

        Console.WriteLine("""
            Select scenario:

            1 - GET Allergy LOW
            2 - GET Allergy MEDIUM
            3 - GET Allergy HIGH

            4 - GET Scan LOW
            5 - GET Scan MEDIUM
            6 - GET Scan HIGH

            7 - POST Scan LOW
            8 - POST Scan MEDIUM
            9 - POST Scan HIGH
            """);

        Console.Write("Choice: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                NBomberRunner
                    .RegisterScenarios(allergyGetScenarioLow)
                    .Run();
                break;

            case "2":
                NBomberRunner
                    .RegisterScenarios(allergyGetScenarioMedium)
                    .Run();
                break;

            case "3":
                NBomberRunner
                    .RegisterScenarios(allergyGetScenarioHigh)
                    .Run();
                break;

            case "4":
                NBomberRunner
                    .RegisterScenarios(scanGetScenarioLow)
                    .Run();
                break;

            case "5":
                NBomberRunner
                    .RegisterScenarios(scanGetScenarioMedium)
                    .Run();
                break;

            case "6":
                NBomberRunner
                    .RegisterScenarios(scanGetScenarioHigh)
                    .Run();
                break;

            case "7":
                NBomberRunner
                    .RegisterScenarios(scanPostScenarioLow)
                    .Run();
                break;

            case "8":
                NBomberRunner
                    .RegisterScenarios(scanPostScenarioMedium)
                    .Run();
                break;

            case "9":
                NBomberRunner
                    .RegisterScenarios(scanPostScenarioHigh)
                    .Run();
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }


    }


}

