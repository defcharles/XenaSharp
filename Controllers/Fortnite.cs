using Microsoft.AspNetCore.Mvc;

namespace XenaSharp.Controllers
{
    [ApiController]
    public class FortniteGameController : ControllerBase
    {
        [HttpPost]
        [Route("/datarouter/api/v1/public/data")]
        public IActionResult DataRouter()
        {
            return Ok(new { status = "OK", code = 200 });
        }

        [HttpPost]
        [Route("/fortnite/api/game/v2/grant_access/*")]
        public IActionResult GrantAccess()
        {
            return Ok(new { status = "OK", code = 200 });
        }

        [HttpGet]
        [Route("/lightswitch/api/service/Fortnite/status")]
        public IActionResult FortniteStatus()
        {
            return Ok(new
            {
                serviceInstanceId = "fortnite",
                status = "UP",
                message = "Fortnite is online",
                maintenanceUri = (string)null,
                overrideCatalogIds = new[] { "a7f138b2e51945ffbfdacc1af0541053" },
                allowedActions = Array.Empty<string>(),
                banned = false,
                launcherInfoDTO = new
                {
                    appName = "Fortnite",
                    catalogItemId = "4fe75bbc5a674f4f9b356b5c90567da5",
                    @namespace = "fn"
                }
            });
        }

        [HttpGet]
        [Route("/lightswitch/api/service/bulk/status")]
        public IActionResult BulkStatus()
        {
            return Ok(new[]
            {
                new
                {
                    serviceInstanceId = "fortnite",
                    status = "UP",
                    message = "fortnite is up.",
                    maintenanceUri = (string)null,
                    overrideCatalogIds = new[] { "a7f138b2e51945ffbfdacc1af0541053" },
                    allowedActions = new[] { "PLAY", "DOWNLOAD" },
                    banned = false,
                    launcherInfoDTO = new
                    {
                        appName = "Fortnite",
                        catalogItemId = "4fe75bbc5a674f4f9b356b5c90567da5",
                        @namespace = "fn"
                    }
                }
            });
        }

        [HttpPost]
        [Route("/fortnite/api/game/v2/tryPlayOnPlatform/account/{accountId}")]
        public IActionResult TryPlayOnPlatform(string accountId)
        {
            Response.ContentType = "text/plain";
            return Ok("true");
        }

        [HttpGet]
        [Route("/fortnite/api/game/v2/enabled_features")]
        public IActionResult EnabledFeatures()
        {
            return Ok(new string[] { });
        }

        [HttpGet]
        [Route("/fortnite/api/game/v2/privacy/account/{accountId}")]
        public IActionResult GetPrivacy(string accountId)
        {
            return Ok(new
            {
                accountId = accountId,
                optOutOfPublicLeaderboards = false
            });
        }

        [HttpGet]
        [Route("/region")]
        public IActionResult GetRegion()
        {
            return Ok(new object[] { });
        }

        [HttpGet]
        [Route("/content/api/pages/fortnite-game/media-events")]
        public IActionResult GetMediaEvents()
        {
            return Ok(new object[] { });
        }

        [HttpPost]
        [Route("/api/v1/assets/Fortnite/{param1}/{param2}")]
        public IActionResult PostAssets(string param1, string param2)
        {
            return Ok(new object[] { });
        }

        [HttpPut]
        [Route("/profile/languages")]
        public IActionResult UpdateLanguages()
        {
            return Ok(new
            {
                @namespace = "Fortnite",
                languages = new[] { "en" }
            });
        }

        [HttpPut]
        [Route("/profile/privacy_settings")]
        public IActionResult UpdatePrivacySettings()
        {
            return Ok(new
            {
                privacySettings = new
                {
                    playRegion = "UNDEFINED_LEVEL",
                    badges = "UNDEFINED_LEVEL",
                    languages = "UNDEFINED_LEVEL"
                }
            });
        }

        [HttpGet]
        [Route("/fortnite/api/game/v2/br-inventory/account")]
        public IActionResult GetBrInventory()
        {
            return Ok(new
            {
                stash = new
                {
                    globalcash = 600
                }
            });
        }

        [HttpPut]
        [Route("/profile/play_region")]
        public IActionResult UpdatePlayRegion()
        {
            return Ok(new
            {
                @namespace = "Fortnite",
                play_region = new[] { "en" }
            });
        }

        [HttpGet]
        [Route("/account/api/epicdomains/ssodomains")]
        public IActionResult GetSsoDomains()
        {
            return Ok(new
            {
                distributions = new[]
                {
                    "http://localhost:3551/",
                    "https://download.epicgames.com/",
                    "https://epicgames-download1.akamaized.net/",
                    "https://fastly-download.epicgames.com/"
                }
            });
        }

        [HttpGet]
        [Route("/fortnite/api/game/v2/twitch/{accountId}")]
        public IActionResult GetTwitchIntegration(string accountId)
        {
            return Ok(new object { });
        }
    }
}
