using Microsoft.AspNetCore.Mvc;

namespace XenaSharp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("/api/v1/user/setting")]
        public IActionResult UpdateUserSetting()
        {
            return Ok(new { status = "OK", code = 200 });
        }

        [HttpGet]
        [Route("/socialban/api/public/v1/{accountId}")]
        public IActionResult GetSocialBan(string accountId)
        {
            return Ok(new object[] { });
        }

        [HttpGet]
        [Route("/presence/api/v1/_/{accountId}/settings/subscriptions")]
        public IActionResult GetPresenceSubscriptions(string accountId)
        {
            return Ok(new object[] { });
        }
    }
}