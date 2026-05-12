using NBomber.Contracts;
using NBomber.CSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SafeBite_Backend_H6.Load.Scenarios.Allergy;

public class GetAllergyScenario
{
    public static ScenarioProps Create(HttpClient httpClient, string baseUrl, string token, int rate)
    {

        return Scenario.Create("get_allergy_paged_scenario", async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/Allergy?Page=1&PageSize=10");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
                // 10 requests per second for 30 seconds. 300 total requests over 30 seconds.
                // rate limiting is set to 50 / min so the expected result is 50 successful requests and 250 failed requests due to rate limiting.
                // we fixed this using a new environemt
                );
    }
}
