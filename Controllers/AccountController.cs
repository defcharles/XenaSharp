using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;


namespace XenaSharp.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [Route("/account/api/public/account/{accountId}/externalAuths")]
        public IActionResult GetExternalAuths(string accountId)
        {
            return Ok(new object[] { });
        }

        [HttpGet]
        [Route("/content-controls/{accountId}")]
        public IActionResult GetContentControls(string accountId)
        {
            return Ok(new object[] { });
        }

        [HttpGet]
        [Route("/account/api/public/account")]
        public IActionResult GetAccount([FromQuery] string[] accountId)
        {
            var response = new List<object>();

            foreach (var id in accountId)
            {
                var accountIdFN = id.Contains("@") ? id.Split('@')[0] : id;
                response.Add(new
                {
                    id = accountIdFN,
                    displayName = accountIdFN,
                    externalAuths = new { }
                });
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("/account/api/public/account/{accountId}")]
        public IActionResult GetAccountDetails(string accountId)
        {
            return Ok(new
            {
                id = "XenaSharp",
                displayName = "XenaSharp",
                name = accountId,
                email = "XenaSharp@XenaSharp.dev",
                failedLoginAttempts = 0,
                lastLogin = "Timestamp",
                numberOfDisplayNameChanges = 3,
                ageGroup = "UNKNOWN",
                headless = false,
                country = "US",
                lastName = "Server",
                preferredLanguage = "en",
                canUpdateDisplayName = false,
                tfaEnabled = true,
                emailVerified = true,
                minorVerified = true,
                minorExpected = true,
                minorStatus = "UNKNOWN",
                cabinedMode = false,
                hasHashedEmail = false,
            });
        }

        [HttpPost]
        [Route("/account/api/oauth/token")]
        public IActionResult GetTokenResponse()
        {
            return Ok(new
            {

                access_token = "eg1~XenaSharp",
                session_id = "9a1f5e80b47d2c3e6f8a0dc592b4fe7d",
                token_type = "bearer",
                client_id = "XenaSharp",
                internal_client = true,
                client_service = "XenaSharp",
                account_id = "XenaSharp",
                expires_in = 28800,
                expires_at = "9999-12-02T01:12:01.100Z",
                auth_method = "exchange_code",
                displayName = "XenaSharp",
                app = "XenaSharp",
                in_app_id = "XenaSharp",
                device_id = "XenaSharp"


            });
        }

        [HttpPost]
        [Route("/account/api/oauth/verify")]
        public IActionResult GetTokenVerifyResponse()
        {
            return Ok(new
            {
                access_token = "eg1~XenaSharp",
                session_id = "9a1f5e80b47d2c3e6f8a0dc592b4fe7d",
                token_type = "bearer",
                client_id = "XenaSharp",
                internal_client = true,
                client_service = "XenaSharp",
                account_id = "XenaSharp",
                expires_in = 28800,
                expires_at = "9999-12-02T01:12:01.100Z",
                auth_method = "exchange_code",
                displayName = "XenaSharp",
                app = "XenaSharp",
                in_app_id = "XenaSharp",
                device_id = "XenaSharp"
            });
        }

        [HttpDelete]
        [Route("/account/api/oauth/sessions/kill")]
        public IActionResult KillSessions()
        {
            return Ok(new { status = "OK", code = 200 });
        }

        [HttpDelete]
        [Route("/account/api/oauth/sessions/kill/{token}")]
        public IActionResult KillSession(string token)
        {
            return Ok(new { status = "OK", code = 200 });
        }
    }
}