using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

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
                var runesResponse = LCU.clientRequestUntilSuccess("GET", "lol-perks/v1/pages");
                Console.Clear();
                using JsonDocument runesJSON = JsonDocument.Parse(runesResponse.Content);
                Debug.WriteLine(runesJSON);

                foreach (JsonElement rune in runesJSON.RootElement.EnumerateArray())
                {
                    string runeName = rune.GetProperty("name").GetString();
                    string runeId = rune.GetProperty("id").GetInt32().ToString();

                    
                    list.Add(new itemList() { name = runeName, id = runeId, free = true });
                }

                // Sort alphabetically
                runesList = list;
            }

            Console.Clear();
        }

        public static void loadSpellsList()
        {
            Console.Clear();
            if (!spellsSorted.Any())
            {
                loadSummonerId();

                List<string> enabledSpells = new List<string>();

                Print.printCentered("Getting a list of available summoner spells...", 15);
                var availableSpells = LCU.clientRequestUntilSuccess("GET", "lol-collections/v1/inventories/" + currentSummonerId + "/spells");
                Console.Clear();
                string[] spellsSplit = availableSpells.Content.Split('[')[1].Split(']')[0].Split(',');

                Print.printCentered("Getting a list of available gamemodes...", 15);
                var platformConfig = LCU.clientRequestUntilSuccess("GET", "lol-platform-config/v1/namespaces");
                Console.Clear();
                string[] enabledGameModes = platformConfig.Content.Split("EnabledModes\":[")[1].Split(']')[0].Split(',');
                string[] inactiveSpellsPerGameMode = platformConfig.Content.Split("gameModeToInactiveSpellIds\":{")[1].Split('}')[0].Split("],");

                Console.Clear();
                foreach (var gameMode in enabledGameModes)
                {
                    foreach (var gameMode2 in inactiveSpellsPerGameMode)
                    {
                        string gameMode2tmp = gameMode2 + "]".Replace("]]", "]");
                        string gameMode2Name = gameMode2tmp.Split(':')[0];
                        if (gameMode == gameMode2Name)
                        {
                            string[] inactiveSpells = gameMode2tmp.Split('[')[1].Split(']')[0].Split(',');
                            foreach (var spell in spellsSplit)
                            {
                                bool isActive = true;
                                foreach (var spellInactive in inactiveSpells)
                                {
                                    if (spell + ".0" == spellInactive)
                                    {
                                        isActive = false;
                                        break;
                                    }
                                }
                                if (isActive)
                                {
                                    enabledSpells.Add(spell);
                                }
                            }
                        }
                    }
                }

                // Remove dupes
                enabledSpells = enabledSpells.Distinct().ToList();

                // Get sepll names
                Print.printCentered("Getting summoner spell names...", 15);
                var spellsResp = LCU.clientRequest("GET", "lol-game-data/assets/v1/summoner-spells.json");
                Console.Clear();
                string[] spellsJsonSplit = spellsResp.Content.Split('{');

                // Add to list with names
                foreach (var spell in enabledSpells)
                {
                    string spellName = "";
                    foreach (var spellSingle in spellsJsonSplit)
                    {
                        if (spellSingle == "[" || spellSingle == "]")
                        {
                            continue;
                        }
                        string spellId = spellSingle.Split("id\":")[1].Split(',')[0];
                        if (spell == spellId)
                        {
                            spellName = spellSingle.Split("name\":\"")[1].Split('"')[0];
                        }
                    }
                    spellsSorted.Add(new itemList() { name = spellName, id = spell });
                }

                // Sort alphabetically
                spellsSorted = spellsSorted.OrderBy(o => o.name).ToList();
            }
        }
    }
}
