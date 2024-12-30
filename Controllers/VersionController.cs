using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace XenaSharp.Controllers
{
    [ApiController]
    [Route("fortnite/api")]
    public class VersionController : ControllerBase
    {
        [HttpGet("v2/versioncheck")]
        public IActionResult CheckVersion()
        {
            return Ok(new { type = "NO_UPDATE" });
        }


        [HttpGet("/versioncheck/*")]
        public IActionResult CheckVersion2()
        {
            return Ok(new { type = "NO_UPDATE" });
        }

        [HttpGet("calendar/v1/timeline")]
        public IActionResult GetTimeline()
        {
            var activeEvents = new[]
            {
                new
                {
                    eventType = "EventFlag.Season12",
                    activeUntil = "9999-12-31T23:59:59.999Z",
                    activeSince = "2019-12-31T23:59:59.999Z"
                },
                new
                {
                    eventType = "EventFlag.LobbySeason12",
                    activeUntil = "9999-12-31T23:59:59.999Z",
                    activeSince = "2019-12-31T23:59:59.999Z"
                }
            };

            var response = new
            {
                channels = new Channels
                {
                    ClientMatchmaking = new Channel
                    {
                        States = Array.Empty<object>(),
                        CacheExpire = "9999-01-01T00:00:00.000Z"
                    },
                    ClientEvents = new Channel
                    {
                        States = new[]
                        {
                            new
                            {
                                validFrom = "0001-01-01T00:00:00.000Z",
                                activeEvents = activeEvents,
                                state = new
                                {
                                    activeStorefronts = Array.Empty<object>(),
                                    eventNamedWeights = new { },
                                    seasonNumber = 12.41,
                                    seasonTemplateId = "AthenaSeason:athenaseason12",
                                    matchXpBonusPoints = 0,
                                    seasonBegin = "2020-01-01T00:00:00Z",
                                    seasonEnd = "9999-01-01T00:00:00Z",
                                    seasonDisplayedEnd = "9999-01-01T00:00:00Z",
                                    weeklyStoreEnd = "9999-01-01T00:00:00Z",
                                    stwEventStoreEnd = "9999-01-01T00:00:00.000Z",
                                    stwWeeklyStoreEnd = "9999-01-01T00:00:00.000Z",
                                    sectionStoreEnds = new
                                    {
                                        Featured = "9999-01-01T00:00:00.000Z"
                                    },
                                    dailyStoreEnd = "9999-01-01T00:00:00Z"
                                }
                            }
                        },
                        CacheExpire = "9999-01-01T00:00:00.000Z"
                    }
                },
                eventsTimeOffsetHrs = 0,
                cacheIntervalMins = 10,
                currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return Ok(response);
        }

       
        public class Channels
        {
            [JsonPropertyName("client-matchmaking")]
            public Channel ClientMatchmaking { get; set; }

            [JsonPropertyName("client-events")]
            public Channel ClientEvents { get; set; }
        }

        public class Channel
        {
            public object[] States { get; set; }
            public string CacheExpire { get; set; }
        }
    }
}
