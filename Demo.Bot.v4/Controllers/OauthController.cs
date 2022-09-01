using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Bot.v4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        [HttpGet("callback")]
        public IActionResult Callback(string code) => new ContentResult
        {
            Content = "<html>" +
                      "<body>" +
                      "<h1>Please send this code yo your bot</h1>" +
                      $"<div>{code}</div>" +
                      "</body>" +
                      "</html>",
            ContentType = "text/html",
            StatusCode  = (int)HttpStatusCode.OK
        };
    }
}
