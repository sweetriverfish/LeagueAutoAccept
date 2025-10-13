using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

// Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);
public record PageKeystone(
	[property: JsonPropertyName("iconPath")] string IconPath,
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("slotType")] string SlotType,
	[property: JsonPropertyName("styleId")] int StyleId
);

public record LolPerksPagesV1(
	[property: JsonPropertyName("autoModifiedSelections")] IReadOnlyList<object> AutoModifiedSelections,
	[property: JsonPropertyName("current")] bool Current,
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("isActive")] bool IsActive,
	[property: JsonPropertyName("isDeletable")] bool IsDeletable,
	[property: JsonPropertyName("isEditable")] bool IsEditable,
	[property: JsonPropertyName("isRecommendationOverride")] bool IsRecommendationOverride,
	[property: JsonPropertyName("isTemporary")] bool IsTemporary,
	[property: JsonPropertyName("isValid")] bool IsValid,
	[property: JsonPropertyName("lastModified")] object LastModified,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("order")] int Order,
	[property: JsonPropertyName("pageKeystone")] PageKeystone PageKeystone,
	[property: JsonPropertyName("primaryStyleIconPath")] string PrimaryStyleIconPath,
	[property: JsonPropertyName("primaryStyleId")] int PrimaryStyleId,
	[property: JsonPropertyName("primaryStyleName")] string PrimaryStyleName,
	[property: JsonPropertyName("quickPlayChampionIds")] IReadOnlyList<object> QuickPlayChampionIds,
	[property: JsonPropertyName("recommendationChampionId")] int RecommendationChampionId,
	[property: JsonPropertyName("recommendationIndex")] int RecommendationIndex,
	[property: JsonPropertyName("runeRecommendationId")] string RuneRecommendationId,
	[property: JsonPropertyName("secondaryStyleIconPath")] string SecondaryStyleIconPath,
	[property: JsonPropertyName("secondaryStyleName")] string SecondaryStyleName,
	[property: JsonPropertyName("selectedPerkIds")] IReadOnlyList<int> SelectedPerkIds,
	[property: JsonPropertyName("subStyleId")] int SubStyleId,
	[property: JsonPropertyName("tooltipBgPath")] string TooltipBgPath,
	[property: JsonPropertyName("uiPerks")] IReadOnlyList<UiPerk> UiPerks
);

public record UiPerk(
	[property: JsonPropertyName("iconPath")] string IconPath,
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("slotType")] string SlotType,
	[property: JsonPropertyName("styleId")] int StyleId
);


