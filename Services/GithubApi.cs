using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Demo.Bot.v4.Services;

public class GithubApi
{
    private readonly HttpClient _httpClient;

    public GithubApi(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<GithubUser> GetGithubUser(string code)
    {
        // Get access-token 
        var githubTokenRequestUri = new Uri("https://github.com/login/oauth/access_token");
        var authenticationParameters = new Dictionary<string, string>
        {
            ["code"]          = code,
            ["redirect_uri"]  = "http://localhost:3979/api/oauth/callback",
            ["client_id"]     = "7154fa8c5e000e28fe87",
            ["client_secret"] = "c801865e6159ef9567372272953e0df27ae727a7"
        };
        var accessTokeContent = new FormUrlEncodedContent(authenticationParameters);

        var githubTokenResponse = await _httpClient.PostAsync(githubTokenRequestUri, accessTokeContent);
        var githubTokenContent  = await githubTokenResponse.Content.ReadAsStringAsync();
        var githubToken         = JsonConvert.DeserializeObject<GithubToken>(githubTokenContent)!;

        // Use access token to get user
        var userRequestUri       = new Uri("https://api.github.com/user");
        var authenticationHeader = new AuthenticationHeaderValue(githubToken.TokenType, githubToken.AccessToken);
        _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

        var userResponse = await _httpClient.GetAsync(userRequestUri);
        var userContent  = await userResponse.Content.ReadAsStringAsync();
        var githubUser   = JsonConvert.DeserializeObject<GithubUser>(userContent)!;

        return githubUser;
    }
}

#region Github DTOs

public class GithubToken
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }
}

public class GithubUser
{
    [JsonProperty("login")]
    public string Login { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("node_id")]
    public string NodeId { get; set; }

    [JsonProperty("avatar_url")]
    public Uri AvatarUrl { get; set; }

    [JsonProperty("gravatar_id")]
    public string GravatarId { get; set; }

    [JsonProperty("url")]
    public Uri Url { get; set; }

    [JsonProperty("html_url")]
    public Uri HtmlUrl { get; set; }

    [JsonProperty("followers_url")]
    public Uri FollowersUrl { get; set; }

    [JsonProperty("following_url")]
    public string FollowingUrl { get; set; }

    [JsonProperty("gists_url")]
    public string GistsUrl { get; set; }

    [JsonProperty("starred_url")]
    public string StarredUrl { get; set; }

    [JsonProperty("subscriptions_url")]
    public Uri SubscriptionsUrl { get; set; }

    [JsonProperty("organizations_url")]
    public Uri OrganizationsUrl { get; set; }

    [JsonProperty("repos_url")]
    public Uri ReposUrl { get; set; }

    [JsonProperty("events_url")]
    public string EventsUrl { get; set; }

    [JsonProperty("received_events_url")]
    public Uri ReceivedEventsUrl { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("site_admin")]
    public bool SiteAdmin { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("company")]
    public object Company { get; set; }

    [JsonProperty("blog")]
    public string Blog { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("hireable")]
    public object Hireable { get; set; }

    [JsonProperty("bio")]
    public object Bio { get; set; }

    [JsonProperty("twitter_username")]
    public object TwitterUsername { get; set; }

    [JsonProperty("public_repos")]
    public long PublicRepos { get; set; }

    [JsonProperty("public_gists")]
    public long PublicGists { get; set; }

    [JsonProperty("followers")]
    public long Followers { get; set; }

    [JsonProperty("following")]
    public long Following { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonProperty("private_gists")]
    public long PrivateGists { get; set; }

    [JsonProperty("total_private_repos")]
    public long TotalPrivateRepos { get; set; }

    [JsonProperty("owned_private_repos")]
    public long OwnedPrivateRepos { get; set; }

    [JsonProperty("disk_usage")]
    public long DiskUsage { get; set; }

    [JsonProperty("collaborators")]
    public long Collaborators { get; set; }

    [JsonProperty("two_factor_authentication")]
    public bool TwoFactorAuthentication { get; set; }
}

#endregion
