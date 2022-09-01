using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Demo.Bot.v4.Services;

public class GithubApi
{
    private readonly HttpClient _httpClient;

    public GithubApi(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string> GetUserName(string code)
    {
        // Get access-token 
        var requestUri = new Uri("https://github.com/login/oauth/access_token");
        var authenticationParameters = new Dictionary<string, string>
        {
            ["code"]          = code,
            ["redirect_uri"]  = "http://localhost:3979/api/oauth/callback",
            ["client_id"]     = "7154fa8c5e000e28fe87",
            ["client_secret"] = "c801865e6159ef9567372272953e0df27ae727a7"
        };
        var encodedContent = new FormUrlEncodedContent(authenticationParameters);

        var response    = await _httpClient.PostAsync(requestUri, encodedContent);
        var content     = await response.Content.ReadAsStringAsync();
        var accessToken = JsonConvert.DeserializeObject<GithubToken>(content);

        // Use access token to get user
        // 
        // var authenticationHeader = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        // return username 
        return await Task.FromResult(accessToken?.AccessToken);
    }
}

public class GithubToken
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }
}
