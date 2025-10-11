using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

public record RerollPoints(
	[property: JsonPropertyName("currentPoints")] int CurrentPoints,
	[property: JsonPropertyName("maxRolls")] int MaxRolls,
	[property: JsonPropertyName("numberOfRolls")] int NumberOfRolls,
	[property: JsonPropertyName("pointsCostToRoll")] int PointsCostToRoll,
	[property: JsonPropertyName("pointsToReroll")] int PointsToReroll
);

public record LolSummonerV1CurrentSummoner(
	[property: JsonPropertyName("accountId")] long AccountId,
	[property: JsonPropertyName("displayName")] string DisplayName,
	[property: JsonPropertyName("gameName")] string GameName,
	[property: JsonPropertyName("internalName")] string InternalName,
	[property: JsonPropertyName("nameChangeFlag")] bool NameChangeFlag,
	[property: JsonPropertyName("percentCompleteForNextLevel")] int PercentCompleteForNextLevel,
	[property: JsonPropertyName("privacy")] string Privacy,
	[property: JsonPropertyName("profileIconId")] int ProfileIconId,
	[property: JsonPropertyName("puuid")] string Puuid,
	[property: JsonPropertyName("rerollPoints")] RerollPoints RerollPoints,
	[property: JsonPropertyName("summonerId")] long SummonerId,
	[property: JsonPropertyName("summonerLevel")] int SummonerLevel,
	[property: JsonPropertyName("tagLine")] string TagLine,
	[property: JsonPropertyName("unnamed")] bool Unnamed,
	[property: JsonPropertyName("xpSinceLastLevel")] int XpSinceLastLevel,
	[property: JsonPropertyName("xpUntilNextLevel")] int XpUntilNextLevel
);
