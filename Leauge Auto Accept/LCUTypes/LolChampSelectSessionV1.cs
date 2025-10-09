using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes
{
	// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
	public record Bans(
		[property: JsonPropertyName("myTeamBans")] IReadOnlyList<object> MyTeamBans,
		[property: JsonPropertyName("numBans")] int NumBans,
		[property: JsonPropertyName("theirTeamBans")] IReadOnlyList<object> TheirTeamBans
	);

	public record ChatDetails(
		[property: JsonPropertyName("mucJwtDto")] MucJwtDto MucJwtDto,
		[property: JsonPropertyName("multiUserChatId")] string MultiUserChatId,
		[property: JsonPropertyName("multiUserChatPassword")] string MultiUserChatPassword
	);

	public record MucJwtDto(
		[property: JsonPropertyName("channelClaim")] string ChannelClaim,
		[property: JsonPropertyName("domain")] string Domain,
		[property: JsonPropertyName("jwt")] string Jwt,
		[property: JsonPropertyName("targetRegion")] string TargetRegion
	);

	public record MyTeam(
		[property: JsonPropertyName("assignedPosition")] string AssignedPosition,
		[property: JsonPropertyName("cellId")] int CellId,
		[property: JsonPropertyName("championId")] int ChampionId,
		[property: JsonPropertyName("championPickIntent")] int ChampionPickIntent,
		[property: JsonPropertyName("gameName")] string GameName,
		[property: JsonPropertyName("internalName")] string InternalName,
		[property: JsonPropertyName("isHumanoid")] bool IsHumanoid,
		[property: JsonPropertyName("nameVisibilityType")] string NameVisibilityType,
		[property: JsonPropertyName("obfuscatedPuuid")] string ObfuscatedPuuid,
		[property: JsonPropertyName("obfuscatedSummonerId")] object ObfuscatedSummonerId,
		[property: JsonPropertyName("pickMode")] int PickMode,
		[property: JsonPropertyName("pickTurn")] int PickTurn,
		[property: JsonPropertyName("playerAlias")] string PlayerAlias,
		[property: JsonPropertyName("playerType")] string PlayerType,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("selectedSkinId")] int SelectedSkinId,
		[property: JsonPropertyName("spell1Id")] int Spell1Id,
		[property: JsonPropertyName("spell2Id")] int Spell2Id,
		[property: JsonPropertyName("summonerId")] long SummonerId,
		[property: JsonPropertyName("tagLine")] string TagLine,
		[property: JsonPropertyName("team")] int Team,
		[property: JsonPropertyName("wardSkinId")] int WardSkinId
	);

	public record PickOrderSwap(
		[property: JsonPropertyName("cellId")] int CellId,
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("state")] string State
	);

	public record PositionSwap(
		[property: JsonPropertyName("cellId")] int CellId,
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("state")] string State
	);

	/// <summary>
	/// lol-champ-select/v1/session
	/// </summary>
	public record LolChampSelectSessionV1(
		[property: JsonPropertyName("actions")] JsonArray Actions,
		[property: JsonPropertyName("allowBattleBoost")] bool AllowBattleBoost,
		[property: JsonPropertyName("allowDuplicatePicks")] bool AllowDuplicatePicks,
		[property: JsonPropertyName("allowLockedEvents")] bool AllowLockedEvents,
		[property: JsonPropertyName("allowPlayerPickSameChampion")] bool AllowPlayerPickSameChampion,
		[property: JsonPropertyName("allowRerolling")] bool AllowRerolling,
		[property: JsonPropertyName("allowSkinSelection")] bool AllowSkinSelection,
		[property: JsonPropertyName("allowSubsetChampionPicks")] bool AllowSubsetChampionPicks,
		[property: JsonPropertyName("bans")] Bans Bans,
		[property: JsonPropertyName("benchChampions")] IReadOnlyList<object> BenchChampions,
		[property: JsonPropertyName("benchEnabled")] bool BenchEnabled,
		[property: JsonPropertyName("boostableSkinCount")] int BoostableSkinCount,
		[property: JsonPropertyName("chatDetails")] ChatDetails ChatDetails,
		[property: JsonPropertyName("counter")] int Counter,
		[property: JsonPropertyName("disallowBanningTeammateHoveredChampions")] bool DisallowBanningTeammateHoveredChampions,
		[property: JsonPropertyName("gameId")] long GameId,
		[property: JsonPropertyName("hasSimultaneousBans")] bool HasSimultaneousBans,
		[property: JsonPropertyName("hasSimultaneousPicks")] bool HasSimultaneousPicks,
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("isCustomGame")] bool IsCustomGame,
		[property: JsonPropertyName("isLegacyChampSelect")] bool IsLegacyChampSelect,
		[property: JsonPropertyName("isSpectating")] bool IsSpectating,
		[property: JsonPropertyName("localPlayerCellId")] int LocalPlayerCellId,
		[property: JsonPropertyName("lockedEventIndex")] int LockedEventIndex,
		[property: JsonPropertyName("myTeam")] IReadOnlyList<MyTeam> MyTeam,
		[property: JsonPropertyName("pickOrderSwaps")] IReadOnlyList<PickOrderSwap> PickOrderSwaps,
		[property: JsonPropertyName("positionSwaps")] IReadOnlyList<PositionSwap> PositionSwaps,
		[property: JsonPropertyName("queueId")] int QueueId,
		[property: JsonPropertyName("rerollsRemaining")] int RerollsRemaining,
		[property: JsonPropertyName("showQuitButton")] bool ShowQuitButton,
		[property: JsonPropertyName("skipChampionSelect")] bool SkipChampionSelect,
		[property: JsonPropertyName("theirTeam")] IReadOnlyList<TheirTeam> TheirTeam,
		[property: JsonPropertyName("timer")] Timer Timer,
		[property: JsonPropertyName("trades")] IReadOnlyList<object> Trades
	);

	public record TheirTeam(
		[property: JsonPropertyName("assignedPosition")] string AssignedPosition,
		[property: JsonPropertyName("cellId")] int CellId,
		[property: JsonPropertyName("championId")] int ChampionId,
		[property: JsonPropertyName("championPickIntent")] int ChampionPickIntent,
		[property: JsonPropertyName("gameName")] string GameName,
		[property: JsonPropertyName("internalName")] string InternalName,
		[property: JsonPropertyName("isHumanoid")] bool IsHumanoid,
		[property: JsonPropertyName("nameVisibilityType")] string NameVisibilityType,
		[property: JsonPropertyName("obfuscatedPuuid")] string ObfuscatedPuuid,
		[property: JsonPropertyName("obfuscatedSummonerId")] int ObfuscatedSummonerId,
		[property: JsonPropertyName("pickMode")] int PickMode,
		[property: JsonPropertyName("pickTurn")] int PickTurn,
		[property: JsonPropertyName("playerAlias")] string PlayerAlias,
		[property: JsonPropertyName("playerType")] string PlayerType,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("selectedSkinId")] int SelectedSkinId,
		[property: JsonPropertyName("spell1Id")] int Spell1Id,
		[property: JsonPropertyName("spell2Id")] int Spell2Id,
		[property: JsonPropertyName("summonerId")] int SummonerId,
		[property: JsonPropertyName("tagLine")] string TagLine,
		[property: JsonPropertyName("team")] int Team,
		[property: JsonPropertyName("wardSkinId")] int WardSkinId
	);

	public record Timer(
		[property: JsonPropertyName("adjustedTimeLeftInPhase")] int AdjustedTimeLeftInPhase,
		[property: JsonPropertyName("internalNowInEpochMs")] long InternalNowInEpochMs,
		[property: JsonPropertyName("isInfinite")] bool IsInfinite,
		[property: JsonPropertyName("phase")] string Phase,
		[property: JsonPropertyName("totalTimeInPhase")] int TotalTimeInPhase
	);


}
