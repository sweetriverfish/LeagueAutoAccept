using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Leauge_Auto_Accept
{
    public class itemList
    {
        public string name { get; set; }
        public string id { get; set; }
        public bool free { get; set; }
    }

    internal class Data
    {
        public static List<itemList> champsSorted = new List<itemList>();
        public static List<itemList> runesList = new List<itemList>();
        public static List<itemList> spellsSorted = new List<itemList>();

        public static long currentSummonerId = 0;
        public static string currentChatId = "";

        public static void loadSummonerId()
        {
            if (currentSummonerId == 0)
            {
                Print.printCentered("Getting summoner ID...", 15);
                var currentSummoner = LCU.clientRequestUntilSuccess<LCUTypes.LolSummonerV1CurrentSummoner>("GET", "lol-summoner/v1/current-summoner");
                Console.Clear();
                currentSummonerId = currentSummoner.Data.SummonerId;
            }
        }

        public static void loadPlayerChatId()
        {
            var myChatProfile = LCU.clientRequest("GET", "lol-chat/v1/me");
            currentChatId = myChatProfile.Content.Split("\"id\":\"")[1].Split("\",")[0];
            currentSummonerId = long.Parse(myChatProfile.Content.Split("\"summonerId\":")[1].Split(",\"")[0]);
        }

        public static void loadChampionsList()
        {
            Console.Clear();

            if (!champsSorted.Any())
            {
                loadSummonerId();

                List<itemList> champs = new List<itemList>();

                Print.printCentered("Getting champions and ownership list...", 15);
                var ownedChamps = LCU.clientRequestUntilSuccess("GET", "lol-champions/v1/inventories/" + currentSummonerId + "/champions-minimal");
                Console.Clear();
                string[] champsSplit = ownedChamps.Content.Split("},{");
                Debug.WriteLine(ownedChamps.Content);

                foreach (var champ in champsSplit)
                {
                    string champName = champ.Split("name\":\"")[1].Split('"')[0];
                    string champId = champ.Split("id\":")[1].Split(',')[0];
                    string champOwned = champ.Split("owned\":")[1].Split(',')[0];
                    string champFreeXboxPass = champ.Split("xboxGPReward\":")[1].Split('}')[0];
                    string champFree = champ.Split("freeToPlay\":")[1].Split(',')[0];

                    // For some reason Riot provides a "None" champion
                    if (champName == "None")
                    {
                        continue;
                    }

                    // Fuck the yeti
                    if (champName == "Nunu & Willump")
                    {
                        champName = "Nunu";
                    }

                    // Check if the champ can be picked
                    bool isAvailable = champOwned == "true" || champFree == "true" || champFreeXboxPass == "true";
                    champs.Add(new itemList() { name = champName, id = champId, free = isAvailable });
                }

                // Sort alphabetically
                champsSorted = champs.OrderBy(o => o.name).ToList();
            }

            SizeHandler.resizeBasedOnChampsCount();
            Console.Clear();
        }

        public static void loadRunesList()
        {
            Console.Clear();

            if (!runesList.Any())
            {
                loadSummonerId();

                List<itemList> list = new List<itemList>();

                Print.printCentered("Getting runes list...", 15);
                var runesResp = LCU.clientRequestUntilSuccess<LCUTypes.LolPerksPagesV1[]>("GET", "lol-perks/v1/pages");
                Console.Clear();

                runesList = runesResp.Data
                    .Where(x => x.IsValid == true)
                    .Select(x => new itemList() { name = (string)x.Name, id = x.Id.ToStringInvariant(), free = true })
                    .ToList();
            }

            Console.Clear();
        }

        public static void loadSpellsList()
        {
            Console.Clear();
            if (!spellsSorted.Any())
            {
                loadSummonerId();

                Print.printCentered("Getting a list of available summoner spells...", 15);
                var availableSpellsResp = LCU.clientRequestUntilSuccess<LCUTypes.LolCollectionsInventoriesSpellsV1>("GET", $"lol-collections/v1/inventories/{currentSummonerId}/spells");
                Console.Clear();
                var availableSpells = new List<ulong>(availableSpellsResp.Data.Spells);

                Print.printCentered("Getting a list of available gamemodes...", 15);
                var platformConfigResp = LCU.clientRequestUntilSuccess("GET", "lol-platform-config/v1/namespaces");

                //couldnt use generic because the (de)serializer in restsharp was failing on key ESports and Esports being a duplicate
                //this uses a case-sensitive deserializer
                var platformConfig = JsonNode.Parse(platformConfigResp.Content); 

                Console.Clear();
                var enabledGameModes = platformConfig["Mutators"]["EnabledModes"].AsArray().GetValues<string>();
                //string[] inactiveSpellsPerGameMode = platformConfigResp.Content.Split("gameModeToInactiveSpellIds\":{")[1].Split('}')[0].Split("],");

                Console.Clear();
                foreach (var gameMode in enabledGameModes)
                {
                    foreach (var spellInactive in platformConfig["ClientSystemStates"]["gameModeToInactiveSpellIds"][gameMode].AsArray())
                    {
                        availableSpells.Remove((ulong)spellInactive.GetValue<float>());
                    }
                }

                // Remove dupes
                availableSpells = availableSpells.Distinct().ToList();

                // Get sepll names
                Print.printCentered("Getting summoner spell names...", 15);
                var spellsResp = LCU.clientRequest<JsonArray>("GET", "lol-game-data/assets/v1/summoner-spells.json");
                Console.Clear();

                var spellsSorted2 = new List<itemList>(20);
                // Add to list with names
                foreach (var availableSpellId in availableSpells)
                {
                    var spellInfo = spellsResp.Data.FirstOrDefault(x => (ulong)x["id"] == availableSpellId);

                    if (spellInfo != null)
                        spellsSorted2.Add(new itemList() { name = (string)spellInfo["name"], id = spellInfo["id"].ToString() });
                }

                // Sort alphabetically
                spellsSorted = spellsSorted2.OrderBy(o => o.name).ToList();

                //Debug.WriteLine(spellsSorted2.Count);
            }
        }
    }
}
