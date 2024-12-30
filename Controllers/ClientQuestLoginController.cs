using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace XenaSharp.Controllers
{
    [ApiController]
    public class ClientQuestLoginController : ControllerBase
    {
        private readonly string profilesDir = Path.Combine(Directory.GetCurrentDirectory(), "static", "profiles");
        private static readonly HashSet<string> userPaths = new HashSet<string>();

        [HttpPost]
        [Route("/fortnite/api/game/v2/profile/{accountId}/client/ClientQuestLogin")]
        public IActionResult SetMtxPlatform(string accountId)
        {
            List<object> MultiUpdate = new List<object>();
            List<object> ApplyProfileChanges = new List<object>();
            int BaseRevision = 0;
            Dictionary<string, object> profile = null;

            var query = HttpContext.Request.Query;
            string profileId = query["profileId"];

            if (string.IsNullOrEmpty(profileId))
            {
                return NotFound("Profile ID not found");
            }

           
            var files = Directory.GetFiles(profilesDir);
            foreach (var file in files)
            {
                if (file.EndsWith(".json"))
                {
                    var profiles = JsonSerializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(file));

                    if (profiles == null) continue;

                 
                    if (!profiles.ContainsKey("rvn")) profiles["rvn"] = 0;
                    if (!profiles.ContainsKey("items")) profiles["items"] = new object();
                    if (!profiles.ContainsKey("stats")) profiles["stats"] = new object();
                    if (!profiles.ContainsKey("stats.attributes")) profiles["stats.attributes"] = new object();
                    if (!profiles.ContainsKey("commandRevision")) profiles["commandRevision"] = 0;

                    var profilePath = Path.Combine(profilesDir, $"profile_{profileId}.json");

                    if (!System.IO.File.Exists(profilePath))
                    {
                        System.IO.File.WriteAllText(profilePath, JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true }));
                    }

                    if (System.IO.File.Exists(profilePath))
                    {
                        profile = JsonSerializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(profilePath));
                    }
                    else
                    {
                        profile = profiles;
                    }
                    if (file.EndsWith($"profile_{profileId}.json"))
                    {
                        profile = System.IO.File.Exists(profilePath)
                            ? JsonSerializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(profilePath))
                            : profiles;
                    }
                }
            }

            
            BaseRevision = GetIntFromProfile(profile, "rvn");
            var profileRevision = GetIntFromProfile(profile, "rvn");
            var commandRevision = GetIntFromProfile(profile, "commandRevision");

            ApplyProfileChanges.Add(new
            {
                changeType = "fullProfileUpdate",
                profile = profile
            });

            var response = new
            {
                profileRevision = profileRevision,
                profileId = profileId,
                profileChangesBaseRevision = BaseRevision,
                profileChanges = ApplyProfileChanges,
                profileCommandRevision = commandRevision,
                serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                multiUpdate = MultiUpdate,
                responseVersion = 1
            };

            userPaths.Add(profileId);

            return Ok(response);
        }

        private int GetIntFromProfile(Dictionary<string, object> profile, string key)
        {
            if (profile != null && profile.ContainsKey(key))
            {
                var value = profile[key];
                if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number)
                {
                    return jsonElement.GetInt32();
                }
                if (value is int intValue)
                {
                    return intValue;
                }
            }
            return 0;
        }
    }
}
