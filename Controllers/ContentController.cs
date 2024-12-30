using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;

namespace XenaSharp.Controllers
{
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ContentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("/content/api/pages/fortnite-game")]
        public IActionResult GetFortniteGameContent()
        {
            
            var response = new
            {
                _activeDate = "2000-01-01T00:00:00.0000000Z",
                lastModified = "2000-01-01T00:00:00.0000000Z",
                _locale = "en-us",
                _suggestedPrefetch = new string[] { },
                _title = "Fortnite Game",
                emergencynotice = new
                {
                    _activeDate = "2000-01-01T00:00:00.0000000Z",
                    _locale = "en-us",
                    _noIndex = false,
                    _title = "emergencynotice",
                    alwaysShow = true,
                    lastModified = DateTime.UtcNow.ToString("o"),
                    news = new
                    {
                        _type = "Battle Royale News",
                        messages = new[]
                        {
                            new
                            {
                                _type = "Battle Royale News",
                                title = "XenaSharp",
                                body = "Made By Defcharles",
                                hidden = false,
                                spotlight = true
                            }
                        }
                    }
                },
                loginmessage = new
                {
                    _title = "LoginMessage",
                    loginmessage = new
                    {
                        _type = "CommonUI Simple Message Base",
                        title = "XenaSharp",
                        body = "XenaSharp is a Fortnite backend written in C#"
                    },
                    _activeDate = "2000-01-01T00:00:00.0000000Z",
                    lastModified = "2000-01-01T00:00:00.0000000Z",
                    _locale = "en-us"
                },
                },
            };

            
            var jsonResponse = JsonSerializer.Serialize(response);

           
            return Ok(jsonResponse);
        }

        [HttpGet]
        [Route("/fortnite/api/storefront/v2/keychain")]
        public IActionResult GetKeychain()
        {
            try
            {
                string keychainPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "keychain.json");
                if (!System.IO.File.Exists(keychainPath))
                {
                    return NotFound(new { error = "Keychain file not found" });
                }

                string jsonContent = System.IO.File.ReadAllText(keychainPath);
                return Ok(System.Text.Json.JsonSerializer.Deserialize<object>(jsonContent));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error reading keychain file", message = ex.Message });
            }
        }
    }
}
