using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

// LolChampionsInventoriesChampionsMinimalV1 myDeserializedClass = JsonSerializer.Deserialize<List<LolChampionsInventoriesChampionsMinimalV1>>(myJsonResponse);
public record Ownership(
	[property: JsonPropertyName("loyaltyReward")] bool LoyaltyReward,
	[property: JsonPropertyName("owned")] bool Owned,
	[property: JsonPropertyName("rental")] Rental Rental,
	[property: JsonPropertyName("xboxGPReward")] bool XboxGPReward
);

public record Rental(
	[property: JsonPropertyName("endDate")] int EndDate,
	[property: JsonPropertyName("purchaseDate")] object PurchaseDate,
	[property: JsonPropertyName("rented")] bool Rented,
	[property: JsonPropertyName("winCountRemaining")] int WinCountRemaining
);

public record LolChampionsInventoriesChampionsMinimalV1(
	[property: JsonPropertyName("active")] bool Active,
	[property: JsonPropertyName("alias")] string Alias,
	[property: JsonPropertyName("banVoPath")] string BanVoPath,
	[property: JsonPropertyName("baseLoadScreenPath")] string BaseLoadScreenPath,
	[property: JsonPropertyName("baseSplashPath")] string BaseSplashPath,
	[property: JsonPropertyName("botEnabled")] bool BotEnabled,
	[property: JsonPropertyName("chooseVoPath")] string ChooseVoPath,
	[property: JsonPropertyName("disabledQueues")] IReadOnlyList<object> DisabledQueues,
	[property: JsonPropertyName("freeToPlay")] bool FreeToPlay,
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("isVisibleInClient")] bool IsVisibleInClient,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("ownership")] Ownership Ownership,
	[property: JsonPropertyName("purchased")] object Purchased,
	[property: JsonPropertyName("rankedPlayEnabled")] bool RankedPlayEnabled,
	[property: JsonPropertyName("roles")] IReadOnlyList<string> Roles,
	[property: JsonPropertyName("squarePortraitPath")] string SquarePortraitPath,
	[property: JsonPropertyName("stingerSfxPath")] string StingerSfxPath,
	[property: JsonPropertyName("title")] string Title
);


