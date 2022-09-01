using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Demo.Bot.v4.Services
{
    public class SpaceXApi
    {
        private readonly HttpClient _httpClient;

        public SpaceXApi(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<CompanyInfo> GetCompanyInfo()
        {
            var response = await _httpClient.GetAsync("company");
            var content  = await response.Content.ReadAsStringAsync();
            var company  = JsonConvert.DeserializeObject<CompanyInfo>(content);

            return company;
        }

        public async Task<IEnumerable<Launch>> GetLaunches()
        {
            var response = await _httpClient.GetAsync("launches");
            var content  = await response.Content.ReadAsStringAsync();
            var launches = JsonConvert.DeserializeObject<IEnumerable<Launch>>(content);

            return launches;
        }

        public async Task<Launch> GetNextLaunch()
        {
            var response = await _httpClient.GetAsync("launches/next");
            var content  = await response.Content.ReadAsStringAsync();
            var launch   = JsonConvert.DeserializeObject<Launch>(content);
            return launch;
        }

        public async Task<Launch> GetPastLaunch()
        {
            var query = new ApiQuery
            {
                Query = new { upcoming = false },
                Options = new Options
                {
                    Sort       = new { date_unix = -1 },
                    Pagination = false
                }
            };
            var queryAsString = JsonConvert.SerializeObject(query);
            var response      = await _httpClient.PostAsync("launches/query", new StringContent(queryAsString, Encoding.UTF8, MediaTypeNames.Application.Json));
            var content       = await response.Content.ReadAsStringAsync();
            var launch        = JsonConvert.DeserializeObject<QueryResult<Launch>>(content)!;
            return launch.Docs.FirstOrDefault(l => l.Details is not null or "");
        }
    }

    #region SaceXApi DTOs

    public class QueryResult<T>
    {
        [JsonProperty("docs")]
        public IEnumerable<T> Docs { get; set; }

        [JsonProperty("totalDocs")]
        public string TotalDocs { get; set; }

        [JsonProperty("offset")]
        public string Offset { get; set; }

        [JsonProperty("limit")]
        public string Limit { get; set; }

        [JsonProperty("totalPages")]
        public string TotalPages { get; set; }

        [JsonProperty("page")]
        public string Page { get; set; }

        [JsonProperty("pagingCounter")]
        public string PagingCounter { get; set; }

        [JsonProperty("hasPrevPage")]
        public bool HasPrevPage { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonProperty("prevPage")]
        public string PrevPage { get; set; }

        [JsonProperty("nextPage")]
        public string NextPage { get; set; }
    }

    public class ApiQuery
    {
        [JsonProperty("query")]
        public object Query { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }
    }

    public class Options
    {
        [JsonProperty("sort")]
        public object Sort { get; set; }
        [JsonProperty("pagination")]
        public bool Pagination { get; set; }
    }

    public class CompanyInfo
    {
        [JsonProperty("headquarters")]
        public Headquarters Headquarters { get; set; }

        [JsonProperty("links")]
        public CompanyInfoLinks Links { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("founder")]
        public string Founder { get; set; }

        [JsonProperty("founded")]
        public long Founded { get; set; }

        [JsonProperty("employees")]
        public long Employees { get; set; }

        [JsonProperty("vehicles")]
        public long Vehicles { get; set; }

        [JsonProperty("launch_sites")]
        public long LaunchSites { get; set; }

        [JsonProperty("test_sites")]
        public long TestSites { get; set; }

        [JsonProperty("ceo")]
        public string Ceo { get; set; }

        [JsonProperty("cto")]
        public string Cto { get; set; }

        [JsonProperty("coo")]
        public string Coo { get; set; }

        [JsonProperty("cto_propulsion")]
        public string CtoPropulsion { get; set; }

        [JsonProperty("valuation")]
        public long Valuation { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Headquarters
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class CompanyInfoLinks
    {
        [JsonProperty("website")]
        public Uri Website { get; set; }

        [JsonProperty("flickr")]
        public Uri Flickr { get; set; }

        [JsonProperty("twitter")]
        public Uri Twitter { get; set; }

        [JsonProperty("elon_twitter")]
        public Uri ElonTwitter { get; set; }
    }

    public class Launch
    {
        [JsonProperty("fairings")]
        public Fairings Fairings { get; set; }

        [JsonProperty("links")]
        public LaunchLinks LaunchLinks { get; set; }

        [JsonProperty("static_fire_date_utc")]
        public DateTimeOffset? StaticFireDateUtc { get; set; }

        [JsonProperty("static_fire_date_unix")]
        public long? StaticFireDateUnix { get; set; }

        [JsonProperty("net")]
        public bool Net { get; set; }

        [JsonProperty("window")]
        public long? Window { get; set; }

        [JsonProperty("rocket")]
        public string Rocket { get; set; }

        [JsonProperty("success")]
        public bool? Success { get; set; }

        [JsonProperty("failures")]
        public Failure[] Failures { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("crew")]
        public string[] Crew { get; set; }

        [JsonProperty("ships")]
        public string[] Ships { get; set; }

        [JsonProperty("capsules")]
        public string[] Capsules { get; set; }

        [JsonProperty("payloads")]
        public string[] Payloads { get; set; }

        [JsonProperty("launchpad")]
        public string Launchpad { get; set; }

        [JsonProperty("flight_number")]
        public long FlightNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date_utc")]
        public DateTimeOffset DateUtc { get; set; }

        [JsonProperty("date_unix")]
        public long DateUnix { get; set; }

        [JsonProperty("date_local")]
        public DateTimeOffset DateLocal { get; set; }

        [JsonProperty("date_precision")]
        public string DatePrecision { get; set; }

        [JsonProperty("upcoming")]
        public bool Upcoming { get; set; }

        [JsonProperty("cores")]
        public Core[] Cores { get; set; }

        [JsonProperty("auto_update")]
        public bool AutoUpdate { get; set; }

        [JsonProperty("tbd")]
        public bool Tbd { get; set; }

        [JsonProperty("launch_library_id")]
        public Guid? LaunchLibraryId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Core
    {
        [JsonProperty("core")]
        public string CoreCore { get; set; }

        [JsonProperty("flight")]
        public long? Flight { get; set; }

        [JsonProperty("gridfins")]
        public bool? Gridfins { get; set; }

        [JsonProperty("legs")]
        public bool? Legs { get; set; }

        [JsonProperty("reused")]
        public bool? Reused { get; set; }

        [JsonProperty("landing_attempt")]
        public bool? LandingAttempt { get; set; }

        [JsonProperty("landing_success")]
        public bool? LandingSuccess { get; set; }

        [JsonProperty("landing_type")]
        public string LandingType { get; set; }

        [JsonProperty("landpad")]
        public string Landpad { get; set; }
    }

    public class Failure
    {
        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("altitude")]
        public long? Altitude { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class Fairings
    {
        [JsonProperty("reused")]
        public bool? Reused { get; set; }

        [JsonProperty("recovery_attempt")]
        public bool? RecoveryAttempt { get; set; }

        [JsonProperty("recovered")]
        public bool? Recovered { get; set; }

        [JsonProperty("ships")]
        public string[] Ships { get; set; }
    }

    public class LaunchLinks
    {
        [JsonProperty("patch")]
        public Patch Patch { get; set; }

        [JsonProperty("reddit")]
        public Reddit Reddit { get; set; }

        [JsonProperty("flickr")]
        public Flickr Flickr { get; set; }

        [JsonProperty("presskit")]
        public Uri Presskit { get; set; }

        [JsonProperty("webcast")]
        public Uri Webcast { get; set; }

        [JsonProperty("youtube_id")]
        public string YoutubeId { get; set; }

        [JsonProperty("article")]
        public Uri Article { get; set; }

        [JsonProperty("wikipedia")]
        public Uri Wikipedia { get; set; }
    }

    public class Flickr
    {
        [JsonProperty("small")]
        public object[] Small { get; set; }

        [JsonProperty("original")]
        public Uri[] Original { get; set; }
    }

    public class Patch
    {
        [JsonProperty("small")]
        public Uri Small { get; set; }

        [JsonProperty("large")]
        public Uri Large { get; set; }
    }

    public class Reddit
    {
        [JsonProperty("campaign")]
        public Uri Campaign { get; set; }

        [JsonProperty("launch")]
        public Uri Launch { get; set; }

        [JsonProperty("media")]
        public Uri Media { get; set; }

        [JsonProperty("recovery")]
        public Uri Recovery { get; set; }
    }

    #endregion
}
