using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;
using IdentityModel.Client;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace MawMvcApp.Controllers
{
    [Route("")]
    public class HomeController 
        : MawBaseController<HomeController>
    {
		public HomeController(ILogger<HomeController> log)
			: base(log)
		{

		}


        [HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Home;

            return View();
        }


        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5010");
            if (disco.IsError)
            {
                _log.LogError(disco.Error);
                return BadRequest();
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "www.mikeandwan.us", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("admin");

            if (tokenResponse.IsError)
            {
                _log.LogError(tokenResponse.Error);
                return BadRequest();
            }

            _log.LogInformation(tokenResponse.Json.ToString());

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5020/identity");
            if (!response.IsSuccessStatusCode)
            {
                _log.LogError(response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                _log.LogInformation(JArray.Parse(content).ToString());
            }

            return Ok();
        }
    }
}
