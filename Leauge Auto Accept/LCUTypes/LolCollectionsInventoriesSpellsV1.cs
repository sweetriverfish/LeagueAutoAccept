using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept.LCUTypes;

public record LolCollectionsInventoriesSpellsV1(
	[property: JsonPropertyName("spells")] IReadOnlyList<ulong> Spells,
	[property: JsonPropertyName("summonerId")] long SummonerId
);