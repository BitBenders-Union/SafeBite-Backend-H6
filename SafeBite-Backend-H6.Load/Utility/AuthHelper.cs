using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SafeBite_Backend_H6.Load.Utility;

public class AuthHelper
{
    public static async Task<string> GetBearerToken(string baseUrl, HttpClient httpClient, string email, string password)
    {
        var loginRequest = new
        {
            email,
            password,
            twoFactorCode = (string?)null, // need to provide type or we cannot create the anonymous object
            twoFactorRecoveryCode = (string?)null
        };

        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/auth/login", loginRequest);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json
            .GetProperty("accessToken")
            .GetString()!;
    }
}
