using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public record GameConfig(
	[property: JsonPropertyName("allowablePremadeSizes")] IReadOnlyList<int> AllowablePremadeSizes,
	[property: JsonPropertyName("customLobbyName")] string CustomLobbyName,
	[property: JsonPropertyName("customMutatorName")] string CustomMutatorName,
	[property: JsonPropertyName("customRewardsDisabledReasons")] IReadOnlyList<object> CustomRewardsDisabledReasons,
	[property: JsonPropertyName("customSpectatorPolicy")] string CustomSpectatorPolicy,
	[property: JsonPropertyName("customSpectators")] IReadOnlyList<object> CustomSpectators,
	[property: JsonPropertyName("customTeam100")] IReadOnlyList<object> CustomTeam100,
	[property: JsonPropertyName("customTeam200")] IReadOnlyList<object> CustomTeam200,
	[property: JsonPropertyName("gameMode")] string GameMode,
	[property: JsonPropertyName("isCustom")] bool IsCustom,
	[property: JsonPropertyName("isLobbyFull")] bool IsLobbyFull,
	[property: JsonPropertyName("isTeamBuilderManaged")] bool IsTeamBuilderManaged,
	[property: JsonPropertyName("mapId")] int MapId,
	[property: JsonPropertyName("maxHumanPlayers")] int MaxHumanPlayers,
	[property: JsonPropertyName("maxLobbySize")] int MaxLobbySize,
	[property: JsonPropertyName("maxLobbySpectatorCount")] int MaxLobbySpectatorCount,
	[property: JsonPropertyName("maxTeamSize")] int MaxTeamSize,
	[property: JsonPropertyName("numPlayersPerTeam")] int NumPlayersPerTeam,
	[property: JsonPropertyName("numberOfTeamsInLobby")] int NumberOfTeamsInLobby,
	[property: JsonPropertyName("pickType")] string PickType,
	[property: JsonPropertyName("premadeSizeAllowed")] bool PremadeSizeAllowed,
	[property: JsonPropertyName("queueId")] int QueueId,
	[property: JsonPropertyName("shouldForceScarcePositionSelection")] bool ShouldForceScarcePositionSelection,
	[property: JsonPropertyName("showPositionSelector")] bool ShowPositionSelector,
	[property: JsonPropertyName("showQuickPlaySlotSelection")] bool ShowQuickPlaySlotSelection
);

public record Invitation(
	[property: JsonPropertyName("invitationId")] string InvitationId,
	[property: JsonPropertyName("invitationType")] string InvitationType,
	[property: JsonPropertyName("state")] string State,
	[property: JsonPropertyName("timestamp")] string Timestamp,
	[property: JsonPropertyName("toSummonerId")] long ToSummonerId,
	[property: JsonPropertyName("toSummonerName")] string ToSummonerName
);

public record LocalMember(
	[property: JsonPropertyName("allowedChangeActivity")] bool AllowedChangeActivity,
	[property: JsonPropertyName("allowedInviteOthers")] bool AllowedInviteOthers,
	[property: JsonPropertyName("allowedKickOthers")] bool AllowedKickOthers,
	[property: JsonPropertyName("allowedStartActivity")] bool AllowedStartActivity,
	[property: JsonPropertyName("allowedToggleInvite")] bool AllowedToggleInvite,
	[property: JsonPropertyName("autoFillEligible")] bool AutoFillEligible,
	[property: JsonPropertyName("autoFillProtectedForPromos")] bool AutoFillProtectedForPromos,
	[property: JsonPropertyName("autoFillProtectedForRemedy")] bool AutoFillProtectedForRemedy,
	[property: JsonPropertyName("autoFillProtectedForSoloing")] bool AutoFillProtectedForSoloing,
	[property: JsonPropertyName("autoFillProtectedForStreaking")] bool AutoFillProtectedForStreaking,
	[property: JsonPropertyName("botChampionId")] int BotChampionId,
	[property: JsonPropertyName("botDifficulty")] string BotDifficulty,
	[property: JsonPropertyName("botId")] string BotId,
	[property: JsonPropertyName("botPosition")] string BotPosition,
	[property: JsonPropertyName("botUuid")] string BotUuid,
	[property: JsonPropertyName("firstPositionPreference")] string FirstPositionPreference,
	[property: JsonPropertyName("intraSubteamPosition")] object IntraSubteamPosition,
	[property: JsonPropertyName("isBot")] bool IsBot,
	[property: JsonPropertyName("isLeader")] bool IsLeader,
	[property: JsonPropertyName("isSpectator")] bool IsSpectator,
	[property: JsonPropertyName("memberData")] object MemberData,
	[property: JsonPropertyName("playerSlots")] IReadOnlyList<object> PlayerSlots,
	[property: JsonPropertyName("puuid")] string Puuid,
	[property: JsonPropertyName("ready")] bool Ready,
	[property: JsonPropertyName("secondPositionPreference")] string SecondPositionPreference,
	[property: JsonPropertyName("showGhostedBanner")] bool ShowGhostedBanner,
	[property: JsonPropertyName("strawberryMapId")] object StrawberryMapId,
	[property: JsonPropertyName("subteamIndex")] object SubteamIndex,
	[property: JsonPropertyName("summonerIconId")] int SummonerIconId,
	[property: JsonPropertyName("summonerId")] long SummonerId,
	[property: JsonPropertyName("summonerInternalName")] string SummonerInternalName,
	[property: JsonPropertyName("summonerLevel")] int SummonerLevel,
	[property: JsonPropertyName("summonerName")] string SummonerName,
	[property: JsonPropertyName("teamId")] int TeamId
);

public record Member(
	[property: JsonPropertyName("allowedChangeActivity")] bool AllowedChangeActivity,
	[property: JsonPropertyName("allowedInviteOthers")] bool AllowedInviteOthers,
	[property: JsonPropertyName("allowedKickOthers")] bool AllowedKickOthers,
	[property: JsonPropertyName("allowedStartActivity")] bool AllowedStartActivity,
	[property: JsonPropertyName("allowedToggleInvite")] bool AllowedToggleInvite,
	[property: JsonPropertyName("autoFillEligible")] bool AutoFillEligible,
	[property: JsonPropertyName("autoFillProtectedForPromos")] bool AutoFillProtectedForPromos,
	[property: JsonPropertyName("autoFillProtectedForRemedy")] bool AutoFillProtectedForRemedy,
	[property: JsonPropertyName("autoFillProtectedForSoloing")] bool AutoFillProtectedForSoloing,
	[property: JsonPropertyName("autoFillProtectedForStreaking")] bool AutoFillProtectedForStreaking,
	[property: JsonPropertyName("botChampionId")] int BotChampionId,
	[property: JsonPropertyName("botDifficulty")] string BotDifficulty,
	[property: JsonPropertyName("botId")] string BotId,
	[property: JsonPropertyName("botPosition")] string BotPosition,
	[property: JsonPropertyName("botUuid")] string BotUuid,
	[property: JsonPropertyName("firstPositionPreference")] string FirstPositionPreference,
	[property: JsonPropertyName("intraSubteamPosition")] object IntraSubteamPosition,
	[property: JsonPropertyName("isBot")] bool IsBot,
	[property: JsonPropertyName("isLeader")] bool IsLeader,
	[property: JsonPropertyName("isSpectator")] bool IsSpectator,
	[property: JsonPropertyName("memberData")] object MemberData,
	[property: JsonPropertyName("playerSlots")] IReadOnlyList<object> PlayerSlots,
	[property: JsonPropertyName("puuid")] string Puuid,
	[property: JsonPropertyName("ready")] bool Ready,
	[property: JsonPropertyName("secondPositionPreference")] string SecondPositionPreference,
	[property: JsonPropertyName("showGhostedBanner")] bool ShowGhostedBanner,
	[property: JsonPropertyName("strawberryMapId")] object StrawberryMapId,
	[property: JsonPropertyName("subteamIndex")] object SubteamIndex,
	[property: JsonPropertyName("summonerIconId")] int SummonerIconId,
	[property: JsonPropertyName("summonerId")] long SummonerId,
	[property: JsonPropertyName("summonerInternalName")] string SummonerInternalName,
	[property: JsonPropertyName("summonerLevel")] int SummonerLevel,
	[property: JsonPropertyName("summonerName")] string SummonerName,
	[property: JsonPropertyName("teamId")] int TeamId
);

public record MucJwtDto__(
	[property: JsonPropertyName("channelClaim")] string ChannelClaim,
	[property: JsonPropertyName("domain")] string Domain,
	[property: JsonPropertyName("jwt")] string Jwt,
	[property: JsonPropertyName("targetRegion")] string TargetRegion
);

/// <summary>
/// lol-lobby/v2/lobby
/// </summary>
public record LolLobbyV2Lobby(
	[property: JsonPropertyName("canStartActivity")] bool CanStartActivity,
	[property: JsonPropertyName("gameConfig")] GameConfig GameConfig,
	[property: JsonPropertyName("invitations")] IReadOnlyList<Invitation> Invitations,
	[property: JsonPropertyName("localMember")] LocalMember LocalMember,
	[property: JsonPropertyName("members")] IReadOnlyList<Member> Members,
	[property: JsonPropertyName("mucJwtDto")] MucJwtDto MucJwtDto,
	[property: JsonPropertyName("multiUserChatId")] string MultiUserChatId,
	[property: JsonPropertyName("multiUserChatPassword")] string MultiUserChatPassword,
	[property: JsonPropertyName("partyId")] string PartyId,
	[property: JsonPropertyName("partyType")] string PartyType,
	[property: JsonPropertyName("popularChampions")] IReadOnlyList<object> PopularChampions,
	[property: JsonPropertyName("restrictions")] IReadOnlyList<object> Restrictions,
	[property: JsonPropertyName("scarcePositions")] IReadOnlyList<string> ScarcePositions,
	[property: JsonPropertyName("warnings")] IReadOnlyList<object> Warnings
);

