using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public record Lol(
	[property: JsonPropertyName("championId")] string ChampionId,
	[property: JsonPropertyName("companionId")] string CompanionId,
	[property: JsonPropertyName("damageSkinId")] string DamageSkinId,
	[property: JsonPropertyName("gameMode")] string GameMode,
	[property: JsonPropertyName("gameQueueType")] string GameQueueType,
	[property: JsonPropertyName("gameStatus")] string GameStatus,
	[property: JsonPropertyName("iconOverride")] string IconOverride,
	[property: JsonPropertyName("isObservable")] string IsObservable,
	[property: JsonPropertyName("legendaryMasteryScore")] string LegendaryMasteryScore,
	[property: JsonPropertyName("level")] string Level,
	[property: JsonPropertyName("mapId")] string MapId,
	[property: JsonPropertyName("mapSkinId")] string MapSkinId,
	[property: JsonPropertyName("puuid")] string Puuid,
	[property: JsonPropertyName("queueId")] string QueueId,
	[property: JsonPropertyName("rankedLeagueDivision")] string RankedLeagueDivision,
	[property: JsonPropertyName("rankedLeagueQueue")] string RankedLeagueQueue,
	[property: JsonPropertyName("rankedLeagueTier")] string RankedLeagueTier,
	[property: JsonPropertyName("rankedLosses")] string RankedLosses,
	[property: JsonPropertyName("rankedPrevSeasonDivision")] string RankedPrevSeasonDivision,
	[property: JsonPropertyName("rankedPrevSeasonTier")] string RankedPrevSeasonTier,
	[property: JsonPropertyName("rankedSplitRewardLevel")] string RankedSplitRewardLevel,
	[property: JsonPropertyName("rankedWins")] string RankedWins,
	[property: JsonPropertyName("regalia")] string Regalia,
	[property: JsonPropertyName("skinVariant")] string SkinVariant,
	[property: JsonPropertyName("skinname")] string Skinname,
	[property: JsonPropertyName("timeStamp")] string TimeStamp
);

public record LolChatMeV1(
	[property: JsonPropertyName("availability")] string Availability,
	[property: JsonPropertyName("gameName")] string GameName,
	[property: JsonPropertyName("gameTag")] string GameTag,
	[property: JsonPropertyName("icon")] int Icon,
	[property: JsonPropertyName("id")] string Id,
	[property: JsonPropertyName("lastSeenOnlineTimestamp")] object LastSeenOnlineTimestamp,
	[property: JsonPropertyName("lol")] Lol Lol,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("obfuscatedSummonerId")] int ObfuscatedSummonerId,
	[property: JsonPropertyName("patchline")] string Patchline,
	[property: JsonPropertyName("pid")] string Pid,
	[property: JsonPropertyName("platformId")] string PlatformId,
	[property: JsonPropertyName("product")] string Product,
	[property: JsonPropertyName("productName")] string ProductName,
	[property: JsonPropertyName("puuid")] string Puuid,
	[property: JsonPropertyName("statusMessage")] string StatusMessage,
	[property: JsonPropertyName("summary")] string Summary,
	[property: JsonPropertyName("summonerId")] long SummonerId,
	[property: JsonPropertyName("time")] int Time
);

