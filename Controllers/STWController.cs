using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace XenaSharp.Controllers
{
    [ApiController]
    [Route("fortnite/api/game/v2")]
    public class StwController : ControllerBase
    {
        [HttpGet]
        [Route("world/info")]
        public async Task<IActionResult> GetWorldInfo()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "static", "stw", "worldstw.json");

            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    var content = await System.IO.File.ReadAllTextAsync(filePath);
                    var contentPages = JsonSerializer.Deserialize<object>(content);
                    return Ok(contentPages);
                }
                catch
                {
                    return BadRequest(new { error = "Error reading worldstw.json" });
                }
            }
            else
            {
                return NotFound(new { error = "worldstw.json not found" });
            }
        }

        [HttpGet]
        [Route("homebase/allowed-name-chars")]
        public IActionResult GetAllowedNameChars()
        {
            return Ok(new
            {
                ranges = new[]
                {
                    48, 57, 65, 90, 97, 122, 192, 255, 260, 265, 280, 281, 286, 287, 304,
                    305, 321, 324, 346, 347, 350, 351, 377, 380, 1024, 1279, 1536, 1791,
                    4352, 4607, 11904, 12031, 12288, 12351, 12352, 12543, 12592, 12687,
                    12800, 13055, 13056, 13311, 13312, 19903, 19968, 40959, 43360, 43391,
                    44032, 55215, 55216, 55295, 63744, 64255, 65072, 65103, 65281, 65470,
                    131072, 173791, 194560, 195103,
                },
                singlePoints = new[] { 32, 39, 45, 46, 95, 126 },
                excludedPoints = new[] { 208, 215, 222, 247 }
            });
        }
    }
}
