using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes
{
	// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
	public record Assets(
		[property: JsonPropertyName("champ-select-background-sound")] string ChampSelectBackgroundSound,
		[property: JsonPropertyName("champ-select-flyout-background")] string ChampSelectFlyoutBackground,
		[property: JsonPropertyName("champ-select-planning-intro")] string ChampSelectPlanningIntro,
		[property: JsonPropertyName("game-select-icon-active")] string GameSelectIconActive,
		[property: JsonPropertyName("game-select-icon-active-video")] string GameSelectIconActiveVideo,
		[property: JsonPropertyName("game-select-icon-default")] string GameSelectIconDefault,
		[property: JsonPropertyName("game-select-icon-disabled")] string GameSelectIconDisabled,
		[property: JsonPropertyName("game-select-icon-hover")] string GameSelectIconHover,
		[property: JsonPropertyName("game-select-icon-intro-video")] string GameSelectIconIntroVideo,
		[property: JsonPropertyName("gameflow-background")] string GameflowBackground,
		[property: JsonPropertyName("gameflow-background-dark")] string GameflowBackgroundDark,
		[property: JsonPropertyName("gameselect-button-hover-sound")] string GameselectButtonHoverSound,
		[property: JsonPropertyName("icon-defeat")] string IconDefeat,
		[property: JsonPropertyName("icon-defeat-v2")] string IconDefeatV2,
		[property: JsonPropertyName("icon-defeat-video")] string IconDefeatVideo,
		[property: JsonPropertyName("icon-empty")] string IconEmpty,
		[property: JsonPropertyName("icon-hover")] string IconHover,
		[property: JsonPropertyName("icon-leaver")] string IconLeaver,
		[property: JsonPropertyName("icon-leaver-v2")] string IconLeaverV2,
		[property: JsonPropertyName("icon-loss-forgiven-v2")] string IconLossForgivenV2,
		[property: JsonPropertyName("icon-v2")] string IconV2,
		[property: JsonPropertyName("icon-victory")] string IconVictory,
		[property: JsonPropertyName("icon-victory-video")] string IconVictoryVideo,
		[property: JsonPropertyName("map-north")] string MapNorth,
		[property: JsonPropertyName("map-south")] string MapSouth,
		[property: JsonPropertyName("music-inqueue-loop-sound")] string MusicInqueueLoopSound,
		[property: JsonPropertyName("parties-background")] string PartiesBackground,
		[property: JsonPropertyName("postgame-ambience-loop-sound")] string PostgameAmbienceLoopSound,
		[property: JsonPropertyName("ready-check-background")] string ReadyCheckBackground,
		[property: JsonPropertyName("ready-check-background-sound")] string ReadyCheckBackgroundSound,
		[property: JsonPropertyName("sfx-ambience-pregame-loop-sound")] string SfxAmbiencePregameLoopSound,
		[property: JsonPropertyName("social-icon-leaver")] string SocialIconLeaver,
		[property: JsonPropertyName("social-icon-victory")] string SocialIconVictory
	);

	public record CategorizedContentBundles(

	);

	public record GameClient(
		[property: JsonPropertyName("observerServerIp")] string ObserverServerIp,
		[property: JsonPropertyName("observerServerPort")] int ObserverServerPort,
		[property: JsonPropertyName("running")] bool Running,
		[property: JsonPropertyName("serverIp")] string ServerIp,
		[property: JsonPropertyName("serverPort")] int ServerPort,
		[property: JsonPropertyName("visible")] bool Visible
	);

	public record GameData(
		[property: JsonPropertyName("gameId")] ulong GameId,
		[property: JsonPropertyName("gameName")] string GameName,
		[property: JsonPropertyName("isCustomGame")] bool IsCustomGame,
		[property: JsonPropertyName("password")] string Password,
		[property: JsonPropertyName("playerChampionSelections")] IReadOnlyList<object> PlayerChampionSelections,
		[property: JsonPropertyName("queue")] Queue Queue,
		[property: JsonPropertyName("spectatorsAllowed")] bool SpectatorsAllowed,
		[property: JsonPropertyName("teamOne")] IReadOnlyList<object> TeamOne,
		[property: JsonPropertyName("teamTwo")] IReadOnlyList<object> TeamTwo
	);

	public record GameDodge(
		[property: JsonPropertyName("dodgeIds")] IReadOnlyList<object> DodgeIds,
		[property: JsonPropertyName("phase")] string Phase,
		[property: JsonPropertyName("state")] string State
	);

	public record GameTypeConfig(
		[property: JsonPropertyName("advancedLearningQuests")] bool AdvancedLearningQuests,
		[property: JsonPropertyName("allowTrades")] bool AllowTrades,
		[property: JsonPropertyName("banMode")] string BanMode,
		[property: JsonPropertyName("banTimerDuration")] int BanTimerDuration,
		[property: JsonPropertyName("battleBoost")] bool BattleBoost,
		[property: JsonPropertyName("crossTeamChampionPool")] bool CrossTeamChampionPool,
		[property: JsonPropertyName("deathMatch")] bool DeathMatch,
		[property: JsonPropertyName("doNotRemove")] bool DoNotRemove,
		[property: JsonPropertyName("duplicatePick")] bool DuplicatePick,
		[property: JsonPropertyName("exclusivePick")] bool ExclusivePick,
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("learningQuests")] bool LearningQuests,
		[property: JsonPropertyName("mainPickTimerDuration")] int MainPickTimerDuration,
		[property: JsonPropertyName("maxAllowableBans")] int MaxAllowableBans,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("onboardCoopBeginner")] bool OnboardCoopBeginner,
		[property: JsonPropertyName("pickMode")] string PickMode,
		[property: JsonPropertyName("postPickTimerDuration")] int PostPickTimerDuration,
		[property: JsonPropertyName("reroll")] bool Reroll,
		[property: JsonPropertyName("teamChampionPool")] bool TeamChampionPool
	);

	public record Map(
		[property: JsonPropertyName("assets")] Assets Assets,
		[property: JsonPropertyName("categorizedContentBundles")] CategorizedContentBundles CategorizedContentBundles,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("gameMode")] string GameMode,
		[property: JsonPropertyName("gameModeName")] string GameModeName,
		[property: JsonPropertyName("gameModeShortName")] string GameModeShortName,
		[property: JsonPropertyName("gameMutator")] string GameMutator,
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("isRGM")] bool IsRGM,
		[property: JsonPropertyName("mapStringId")] string MapStringId,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("perPositionDisallowedSummonerSpells")] PerPositionDisallowedSummonerSpells PerPositionDisallowedSummonerSpells,
		[property: JsonPropertyName("perPositionRequiredSummonerSpells")] PerPositionRequiredSummonerSpells PerPositionRequiredSummonerSpells,
		[property: JsonPropertyName("platformId")] string PlatformId,
		[property: JsonPropertyName("platformName")] string PlatformName,
		[property: JsonPropertyName("properties")] Properties Properties
	);

	public record PerPositionDisallowedSummonerSpells(

	);

	public record PerPositionRequiredSummonerSpells(

	);

	public record Properties(
		[property: JsonPropertyName("suppressRunesMasteriesPerks")] bool SuppressRunesMasteriesPerks
	);

	public record Queue(
		[property: JsonPropertyName("allowablePremadeSizes")] IReadOnlyList<int> AllowablePremadeSizes,
		[property: JsonPropertyName("areFreeChampionsAllowed")] bool AreFreeChampionsAllowed,
		[property: JsonPropertyName("assetMutator")] string AssetMutator,
		[property: JsonPropertyName("category")] string Category,
		[property: JsonPropertyName("championsRequiredToPlay")] int ChampionsRequiredToPlay,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("detailedDescription")] string DetailedDescription,
		[property: JsonPropertyName("gameMode")] string GameMode,
		[property: JsonPropertyName("gameTypeConfig")] GameTypeConfig GameTypeConfig,
		[property: JsonPropertyName("id")] int Id,
		[property: JsonPropertyName("isBotHonoringAllowed")] bool IsBotHonoringAllowed,
		[property: JsonPropertyName("isCustom")] bool IsCustom,
		[property: JsonPropertyName("isRanked")] bool IsRanked,
		[property: JsonPropertyName("isTeamBuilderManaged")] bool IsTeamBuilderManaged,
		[property: JsonPropertyName("lastToggledOffTime")] int LastToggledOffTime,
		[property: JsonPropertyName("lastToggledOnTime")] int LastToggledOnTime,
		[property: JsonPropertyName("mapId")] int MapId,
		[property: JsonPropertyName("maximumParticipantListSize")] int MaximumParticipantListSize,
		[property: JsonPropertyName("minLevel")] int MinLevel,
		[property: JsonPropertyName("minimumParticipantListSize")] int MinimumParticipantListSize,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("numPlayersPerTeam")] int NumPlayersPerTeam,
		[property: JsonPropertyName("queueAvailability")] string QueueAvailability,
		[property: JsonPropertyName("queueRewards")] QueueRewards QueueRewards,
		[property: JsonPropertyName("removalFromGameAllowed")] bool RemovalFromGameAllowed,
		[property: JsonPropertyName("removalFromGameDelayMinutes")] int RemovalFromGameDelayMinutes,
		[property: JsonPropertyName("shortName")] string ShortName,
		[property: JsonPropertyName("showPositionSelector")] bool ShowPositionSelector,
		[property: JsonPropertyName("spectatorEnabled")] bool SpectatorEnabled,
		[property: JsonPropertyName("type")] string Type
	);

	public record QueueRewards(
		[property: JsonPropertyName("isChampionPointsEnabled")] bool IsChampionPointsEnabled,
		[property: JsonPropertyName("isIpEnabled")] bool IsIpEnabled,
		[property: JsonPropertyName("isXpEnabled")] bool IsXpEnabled,
		[property: JsonPropertyName("partySizeIpRewards")] IReadOnlyList<object> PartySizeIpRewards
	);

	public record LolGameflowSessionV1(
		[property: JsonPropertyName("gameClient")] GameClient GameClient,
		[property: JsonPropertyName("gameData")] GameData GameData,
		[property: JsonPropertyName("gameDodge")] GameDodge GameDodge,
		[property: JsonPropertyName("map")] Map Map,
		[property: JsonPropertyName("phase")] string Phase
	);


}
