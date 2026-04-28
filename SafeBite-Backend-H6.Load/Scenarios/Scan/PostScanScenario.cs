using NATS.Client.Internals;
using NBomber.Contracts;
using NBomber.CSharp;
using SafeBite_Backend_H6.API.Contracts.Requests.Scans;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SafeBite_Backend_H6.Load.Scenarios.Scan;

public class PostScanScenario
{

    public static ScenarioProps Create(HttpClient httpClient, string baseUrl, string token, byte[] imageBytes, int rate)
    {
        return Scenario.Create("post_scan_scenario", async context =>
        {


            using var multipart = new MultipartFormDataContent();

            multipart.Add(new StringContent("Load Test Scan"), nameof(CreateScanRequest.Name)); // instead of just using "Name" we get the actual property name from the class.
            multipart.Add(new StringContent("eng"), nameof(CreateScanRequest.Lang));
            multipart.Add(new ByteArrayContent(imageBytes), nameof(CreateScanRequest.Image), "noodlesdingredients.png");

            // create request with bearer token and body. the body is the multipart form data created to match CreateScanRequest.
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/Scan");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = multipart;

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
