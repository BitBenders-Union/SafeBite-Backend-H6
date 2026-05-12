using NBomber.Contracts;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafeBite_Backend_H6.Load.Scenarios.Scan;

public class GetScanScenario
{

    public static ScenarioProps Create(HttpClient httpClient, string baseUrl, string token, int rate)
    {
        return Scenario.Create("get_scan_scenario", async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/Scan?Page=1&PageSize=10");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.Inject(
                    rate: rate,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(30)
                )
        );
    }

}
