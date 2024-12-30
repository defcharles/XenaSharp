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
    [Route("/fortnite/api/game/v2/profile/{accountId}/client/SetCosmeticLockerSlot")]
    public class SetCosmeticLockerSlotController : ControllerBase
    {
        private readonly ILogger<SetCosmeticLockerSlotController> _logger;
        private readonly string profilesDir = Path.Combine(Directory.GetCurrentDirectory(), "static", "profiles");

        private static readonly string[] SpecialCosmetics = new[]
        {
            "AthenaCharacter:cid_random",
            "AthenaBackpack:bid_random",
            "AthenaPickaxe:pickaxe_random",
            "AthenaGlider:glider_random",
            "AthenaSkyDiveContrail:trails_random",
            "AthenaItemWrap:wrap_random",
            "AthenaMusicPack:musicpack_random",
            "AthenaLoadingScreen:lsid_random"
        };

        public SetCosmeticLockerSlotController(ILogger<SetCosmeticLockerSlotController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SetCosmeticLockerSlot(string accountId)
        {
            try
            {
                var query = HttpContext.Request.Query;
                string profileId = query["profileId"];

                if (string.IsNullOrEmpty(profileId))
                {
                    _logger.LogError("Profile ID is missing in the request.");
                    return BadRequest(new { error = "Invalid Payload" });
                }

                string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogError($"Empty request body for accountId: {accountId}, profileId: {profileId}");
                    return BadRequest(new { error = "Invalid Payload" });
                }

                _logger.LogDebug($"Request Body: {requestBody}");

                var requestJson = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
                if (requestJson == null)
                {
                    _logger.LogError($"Failed to deserialize request body for accountId: {accountId}, profileId: {profileId}");
                    return BadRequest(new { error = "Invalid Payload" });
                }

                string lockerItem = requestJson.ContainsKey("lockerItem") ? requestJson["lockerItem"].ToString() : string.Empty;
                string itemToSlot = requestJson.ContainsKey("itemToSlot") ? requestJson["itemToSlot"].ToString() : string.Empty;
                string category = requestJson.ContainsKey("category") ? requestJson["category"].ToString() : string.Empty;
                int slotIndex = requestJson.ContainsKey("slotIndex") && requestJson["slotIndex"] is JsonElement indexElement && indexElement.TryGetInt32(out int idx) ? idx : -1;

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

                
                if (!profile.ContainsKey("items") || profile["items"] is not JsonElement itemsElement)
                {
                    return BadRequest(new { error = "InvalidLockerSlotIndex" });
                }

                var items = JsonSerializer.Deserialize<Dictionary<string, object>>(itemsElement.GetRawText());
                if (!items.ContainsKey(lockerItem))
                {
                    return BadRequest(new { error = "InvalidLockerSlotIndex" });
                }

                var lockerItemData = JsonSerializer.Deserialize<Dictionary<string, object>>(items[lockerItem].ToString());
                if (lockerItemData["templateId"].ToString().ToLower() != "cosmeticlocker:cosmeticlocker_athena")
                {
                    return BadRequest(new { error = "InvalidLockerSlotIndex" });
                }

              
                string itemToSlotId = "";
                if (!string.IsNullOrEmpty(itemToSlot) && !SpecialCosmetics.Contains(itemToSlot))
                {
                    bool found = false;
                    foreach (var item in items)
                    {
                        var itemData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Value.ToString());
                        if (itemData["templateId"].ToString().ToLower() == itemToSlot.ToLower())
                        {
                            itemToSlotId = item.Key;
                            found = true;
                            break;
                        }
                    }

                    if (!found && !SpecialCosmetics.Contains(itemToSlot))
                    {
                        return BadRequest(new { error = "ItemNotFound" });
                    }
                }

               
                var profileChanges = new List<object>();
                if (requestJson.ContainsKey("variantUpdates") && !string.IsNullOrEmpty(itemToSlotId))
                {
                    var variantUpdates = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(requestJson["variantUpdates"].ToString());
                    var targetItemData = JsonSerializer.Deserialize<Dictionary<string, object>>(items[itemToSlotId].ToString());
                    var targetItemAttributes = JsonSerializer.Deserialize<Dictionary<string, object>>(targetItemData["attributes"].ToString());

                    if (targetItemAttributes.ContainsKey("variants"))
                    {
                        var variants = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(targetItemAttributes["variants"].ToString());
                        foreach (var update in variantUpdates)
                        {
                            var variant = variants.Find(v => v["channel"] == update["channel"]);
                            if (variant != null)
                            {
                                variant["active"] = update["active"];
                            }
                            else
                            {
                                variants.Add(update);
                            }
                        }
                        targetItemAttributes["variants"] = variants;
                        targetItemData["attributes"] = targetItemAttributes;
                        items[itemToSlotId] = targetItemData;

                        profileChanges.Add(new
                        {
                            changeType = "itemAttrChanged",
                            itemId = itemToSlotId,
                            attributeName = "variants",
                            attributeValue = variants
                        });
                    }
                }

             
                var lockerItemAttributes = JsonSerializer.Deserialize<Dictionary<string, object>>(lockerItemData["attributes"].ToString());
                var lockerData = JsonSerializer.Deserialize<Dictionary<string, object>>(lockerItemAttributes["locker_slots_data"].ToString());
                var slots = JsonSerializer.Deserialize<Dictionary<string, object>>(lockerData["slots"].ToString());

                if (!slots.ContainsKey(category))
                {
                    return BadRequest(new { error = "InvalidLockerSlotIndex" });
                }

                var categorySlot = JsonSerializer.Deserialize<Dictionary<string, object>>(slots[category].ToString());
                var slotItems = JsonSerializer.Deserialize<List<string>>(categorySlot["items"].ToString());

              
                var stats = JsonSerializer.Deserialize<Dictionary<string, object>>(profile["stats"].ToString());
                var statsAttributes = JsonSerializer.Deserialize<Dictionary<string, object>>(stats["attributes"].ToString());

               
                if (category == "Dance")
                {
                    if (slotIndex >= 0 && slotIndex <= 5)
                    {
                        slotItems[slotIndex] = itemToSlot;
                        var favoriteArray = JsonSerializer.Deserialize<List<string>>(statsAttributes["favorite_dance"].ToString());
                        favoriteArray[slotIndex] = string.IsNullOrEmpty(itemToSlotId) ? itemToSlot : itemToSlotId;
                        statsAttributes["favorite_dance"] = favoriteArray;
                    }
                    else if (slotIndex == -1)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            slotItems[i] = itemToSlot;
                            var favoriteArray = JsonSerializer.Deserialize<List<string>>(statsAttributes["favorite_dance"].ToString());
                            favoriteArray[i] = string.IsNullOrEmpty(itemToSlotId) ? itemToSlot : itemToSlotId;
                            statsAttributes["favorite_dance"] = favoriteArray;
                        }
                    }
                }
                else if (category == "ItemWrap")
                {
                    if (slotIndex >= 0 && slotIndex <= 7)
                    {
                        slotItems[slotIndex] = itemToSlot;
                        var favoriteArray = JsonSerializer.Deserialize<List<string>>(statsAttributes["favorite_itemwraps"].ToString());
                        favoriteArray[slotIndex] = string.IsNullOrEmpty(itemToSlotId) ? itemToSlot : itemToSlotId;
                        statsAttributes["favorite_itemwraps"] = favoriteArray;
                    }
                    else if (slotIndex == -1)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            slotItems[i] = itemToSlot;
                            var favoriteArray = JsonSerializer.Deserialize<List<string>>(statsAttributes["favorite_itemwraps"].ToString());
                            favoriteArray[i] = string.IsNullOrEmpty(itemToSlotId) ? itemToSlot : itemToSlotId;
                            statsAttributes["favorite_itemwraps"] = favoriteArray;
                        }
                    }
                }
                else
                {
                    slotItems = new List<string> { itemToSlot };
                    statsAttributes[$"favorite_{category.ToLower()}"] = string.IsNullOrEmpty(itemToSlotId) ? itemToSlot : itemToSlotId;
                }

                
                categorySlot["items"] = slotItems;
                slots[category] = categorySlot;
                lockerData["slots"] = slots;
                lockerItemAttributes["locker_slots_data"] = lockerData;
                lockerItemData["attributes"] = lockerItemAttributes;
                items[lockerItem] = lockerItemData;
                profile["items"] = items;

                stats["attributes"] = statsAttributes;
                profile["stats"] = stats;

               
                profile["rvn"] = (profile.ContainsKey("rvn") && profile["rvn"] is JsonElement rvnElement && rvnElement.TryGetInt32(out int rvn)) ? rvn + 1 : 0;
                profile["commandRevision"] = (profile.ContainsKey("commandRevision") && profile["commandRevision"] is JsonElement cmdElement && cmdElement.TryGetInt32(out int cmd)) ? cmd + 1 : 0;

                profileChanges.Add(new
                {
                    changeType = "itemAttrChanged",
                    itemId = lockerItem,
                    attributeName = "locker_slots_data",
                    attributeValue = lockerData
                });

               
                System.IO.File.WriteAllText(profilePath, JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true }));

                return Ok(new
                {
                    profileRevision = profile["rvn"],
                    profileId = profileId,
                    profileChangesBaseRevision = Convert.ToInt32(profile["rvn"]) - 1,
                    profileChanges = profileChanges,
                    profileCommandRevision = profile["commandRevision"],
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SetCosmeticLockerSlot: {ex}");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }
    }
}