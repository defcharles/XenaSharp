using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace XenaSharp.Controllers
{
    [ApiController]
    [Route("/fortnite/api/game/v2/profile/{accountId}/client/EquipBattleRoyaleCustomization")]
    public class EquipBattleRoyaleCustomizationController : ControllerBase
    {
        private readonly ILogger<EquipBattleRoyaleCustomizationController> _logger;
        private readonly string profilesDir = Path.Combine(Directory.GetCurrentDirectory(), "static", "profiles");

        private static readonly Dictionary<string, string> SLOTS = new()
        {
            { "Character", "favorite_character" },
            { "Backpack", "favorite_backpack" },
            { "Pickaxe", "favorite_pickaxe" },
            { "Glider", "favorite_glider" },
            { "SkyDiveContrail", "favorite_skydivecontrail" },
            { "MusicPack", "favorite_musicpack" },
            { "LoadingScreen", "favorite_loadingscreen" },
            { "Dance", "favorite_dance" },
            { "ItemWrap", "favorite_itemwraps" }
        };

        public EquipBattleRoyaleCustomizationController(ILogger<EquipBattleRoyaleCustomizationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> EquipCustomization(string accountId)
        {
            try
            {
                var query = HttpContext.Request.Query;
                string profileId = query["profileId"];

                if (string.IsNullOrEmpty(profileId))
                {
                    _logger.LogError("Profile ID is missing in the request.");
                    return BadRequest(new { error = "Invalid Payload: Missing profileId" });
                }

                string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogError($"Empty request body for accountId: {accountId}, profileId: {profileId}");
                    return BadRequest(new { error = "Invalid Payload: Empty request body" });
                }

                _logger.LogDebug($"Request Body: {requestBody}");

                var requestJson = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
                if (requestJson == null)
                {
                    _logger.LogError($"Failed to deserialize request body for accountId: {accountId}, profileId: {profileId}. RequestBody: {requestBody}");
                    return BadRequest(new { error = "Invalid Payload: Failed to deserialize" });
                }

           
                string slotName = requestJson.ContainsKey("slotName") ? requestJson["slotName"].ToString() : string.Empty;
                string itemToSlot = requestJson.ContainsKey("itemToSlot") ? requestJson["itemToSlot"].ToString() : string.Empty;
                int indexWithinSlot = requestJson.ContainsKey("indexWithinSlot") && requestJson["indexWithinSlot"] is JsonElement indexElement && indexElement.TryGetInt32(out int idx) ? idx : -1;

               
                _logger.LogDebug($"Extracted slotName: '{slotName}', itemToSlot: '{itemToSlot}', indexWithinSlot: {indexWithinSlot}");

                if (string.IsNullOrEmpty(slotName))
                {
                    _logger.LogError($"Empty or missing slotName for accountId: {accountId}, profileId: {profileId}. RequestBody: {requestBody}");
                    return BadRequest(new { error = "Invalid slot name: empty or null" });
                }

                if (!SLOTS.TryGetValue(slotName, out string statName))
                {
                    _logger.LogError($"Invalid slot name '{slotName}' for accountId: {accountId}, profileId: {profileId}. RequestBody: {requestBody}");
                    return BadRequest(new { error = $"Invalid slot name: '{slotName}'" });
                }

                string profilePath = Path.Combine(profilesDir, $"profile_{profileId}.json");
                if (!System.IO.File.Exists(profilePath))
                {
                    _logger.LogError($"Profile file not found for accountId: {accountId}, profileId: {profileId}");
                    return NotFound(new { error = "Profile not found" });
                }

                var profile = JsonSerializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(profilePath));
                if (profile == null)
                {
                    _logger.LogError($"Failed to deserialize profile for accountId: {accountId}, profileId: {profileId}");
                    return StatusCode(500, new { error = "Failed to load profile" });
                }

              
                if (!profile.ContainsKey("stats") || profile["stats"] is not JsonElement statsElement)
                {
                    _logger.LogError($"Missing or invalid 'stats' in profile for accountId: {accountId}, profileId: {profileId}");
                    return StatusCode(500, new { error = "Invalid profile structure: Missing stats" });
                }

                var profileStats = JsonSerializer.Deserialize<Dictionary<string, object>>(statsElement.GetRawText());
                if (profileStats == null || !profileStats.ContainsKey("attributes"))
                {
                    _logger.LogError($"Missing attributes in stats for accountId: {accountId}, profileId: {profileId}");
                    return StatusCode(500, new { error = "Invalid profile structure: Missing stats.attributes" });
                }

                var profileAttributes = JsonSerializer.Deserialize<Dictionary<string, object>>(profileStats["attributes"].ToString());

                if (profileAttributes == null)
                {
                    _logger.LogError($"Failed to deserialize 'attributes' for accountId: {accountId}, profileId: {profileId}");
                    return StatusCode(500, new { error = "Failed to deserialize attributes" });
                }

             
                if (slotName == "Dance")
                {
                    var danceSlots = profileAttributes.ContainsKey(statName) && profileAttributes[statName] is JsonElement danceElement && danceElement.ValueKind == JsonValueKind.Array
                        ? JsonSerializer.Deserialize<List<string>>(danceElement.GetRawText())
                        : new List<string>(new string[6]);

                    if (indexWithinSlot == -1)
                    {
                        for (int i = 0; i < danceSlots.Count; i++) danceSlots[i] = itemToSlot;
                    }
                    else if (indexWithinSlot >= 0 && indexWithinSlot < danceSlots.Count)
                    {
                        danceSlots[indexWithinSlot] = itemToSlot;
                    }

                    profileAttributes[statName] = danceSlots;
                }
                else
                {
                    profileAttributes[statName] = itemToSlot;
                }

                
                profileStats["attributes"] = profileAttributes;
                profile["stats"] = profileStats;
                profile["rvn"] = (profile.ContainsKey("rvn") && profile["rvn"] is JsonElement rvnElement && rvnElement.TryGetInt32(out int rvn)) ? rvn + 1 : 0;

               
                System.IO.File.WriteAllText(profilePath, JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true }));

                _logger.LogInformation($"Customization applied successfully for accountId: {accountId}, profileId: {profileId}");

                var response = new
                {
                    profileRevision = profile["rvn"],
                    profileId = profileId,
                    profileChangesBaseRevision = profile["rvn"],
                    profileChanges = new[] { new { changeType = "fullProfileUpdate", profile } },
                    profileCommandRevision = profile["commandRevision"],
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in EquipCustomization for accountId: {accountId}: {ex}");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }
    }
}
