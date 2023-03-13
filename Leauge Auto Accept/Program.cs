using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept
{

    public class itemList
    {
        public string name { get; set; }
        public string id { get; set; }
        public bool free { get; set; }
    }

    class Program
    {
        public static string[] leagueAuth;
        public static int lcuPid = 0;
        public static bool isLeagueOpen = false;

        public static int totalRows = 28;
        public static int currentPos = 0;
        public static int consolePosLast = 0;
        public static int totalChamps = 0;
        public static int totalSpells = 0;
        public static int searchPos = 0;
        public static int lastPosMainNav = 0;
        public static bool canMovePos = false;
        public static bool isMovingPos = false;

        public static int currentChampPicker = 0;
        public static int currentSpellSlot = 0;
        public static string filterKeyword = "";

        public static List<itemList> champsSorterd = new List<itemList>();
        public static List<itemList> spellsSorted = new List<itemList>();

        public static string currentWindow = "";

        public static string[] currentChamp = { "None", "0" };
        public static string[] currentBan = { "None", "0" };
        public static string[] currentSpell0 = { "None", "0" };
        public static string[] currentSpell1 = { "None", "0" };

        public static bool isAutoAcceptOn = false;
        public static bool shouldAutoAcceptbeOn = false;

        public static bool pickedChamp = false;
        public static bool lockedChamp = false;
        public static bool pickedBan = false;
        public static bool LockedBan = false;
        public static bool pickedSpell0 = false;
        public static bool pickedSpell1 = false;
        public static long lastActStartTime;
        public static string lastActId = "";
        public static string lastChatRoom = "";

        public static string currentSummonerId = "";

        //                                 saveData preloadData instaLock lockDelay
        public static string[] settings = { "false", "false", "false", "1500" };
        public static int lockDelay = 1500;

        private static void Main()
        {
            canMovePos = false;
            currentWindow = "initializing";
            Console.SetCursorPosition(1, 15);
            Console.WriteLine(padSides("Initializing...", 118)[0]);
            // Set the console size
            Console.SetWindowSize(120, 30);
            // Hide cursor/caret
            Console.CursorVisible = false;
            Console.Title = "League Auto Accept";

            // Attempt to load existing settings
            loadSettings();

            // Start a bunch of task
            var taskKeys = new Task(ReadKeys);
            taskKeys.Start();
            var taskQueue = new Task(acceptQueue);
            taskQueue.Start();
            var taskLeagueAlive = new Task(CheckIfLeagueClientIsOpenTask);
            taskLeagueAlive.Start();

            // Indefinitely await them
            var tasks = new[] { taskKeys, taskQueue, taskLeagueAlive };
            Task.WaitAll(tasks);

            Console.ReadKey();
        }

        private static void CheckIfLeagueClientIsOpenTask()
        {
            while (true)
            {
                Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
                if (client != null)
                {
                    leagueAuth = getLeagueAuth(client);
                    isLeagueOpen = true;
                    if (lcuPid != client.Id)
                    {
                        lcuPid = client.Id;
                        // Check if preload data was enabled last time
                        if (settings[1] == "true")
                        {
                            loadChampionsList();
                            loadSpellsList();
                        }
                        if (shouldAutoAcceptbeOn)
                        {
                            isAutoAcceptOn = true;
                        }
                        if (currentWindow != "exitMenu")
                        {
                            mainScreen();
                        }
                    }
                }
                else
                {
                    isLeagueOpen = false;
                    isAutoAcceptOn = false;
                    champsSorterd.Clear();
                    spellsSorted.Clear();
                    currentSummonerId = "";
                    if (currentWindow != "lcuClosed" && currentWindow != "exitMenu")
                    {
                        LeagueClientIsClosedMsg();
                    }
                }
                Thread.Sleep(2000);
            }
        }

        private static bool CheckIfLeagueClientIsOpen()
        {
            Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
            if (client != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void LeagueClientIsClosedMsg()
        {
            canMovePos = false;
            currentWindow = "lcuClosed";

            Console.Clear();

            Console.SetCursorPosition(1, 15);
            Console.WriteLine(padSides("League client cannot be found.", 118)[0]);
        }

        private static void mainScreen()
        {
            canMovePos = false;
            currentWindow = "mainScreen";
            currentPos = lastPosMainNav;
            consolePosLast = lastPosMainNav;

            Console.Clear();

            writeLineWhenPossible(13, 3, "  _                                                 _                                     _   ", true);
            writeLineWhenPossible(13, 4, " | |                                     /\\        | |            /\\                     | |  ", true);
            writeLineWhenPossible(13, 5, " | |     ___  __ _  __ _ _   _  ___     /  \\  _   _| |_ ___      /  \\   ___ ___ ___ _ __ | |_ ", true);
            writeLineWhenPossible(13, 6, " | |    / _ \\/ _` |/ _` | | | |/ _ \\   / /\\ \\| | | | __/ _ \\    / /\\ \\ / __/ __/ _ \\ '_ \\| __|", true);
            writeLineWhenPossible(13, 7, " | |___|  __/ (_| | (_| | |_| |  __/  / ____ \\ |_| | || (_) |  / ____ \\ (_| (_|  __/ |_) | |_ ", true);
            writeLineWhenPossible(13, 8, " |______\\___|\\__,_|\\__, |\\__,_|\\___| /_/    \\_\\__,_|\\__\\___/  /_/    \\_\\___\\___\\___| .__/ \\__|", true);
            writeLineWhenPossible(13, 9, "                    __/ |                                                          | |        ", true);
            writeLineWhenPossible(13, 10, "                   |___/                                                           |_|        ", true);

            writeLineWhenPossible(35, 14, (" " + currentChamp[0]).PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 14, "Select a champion ", true);
            writeLineWhenPossible(35, 15, (" " + currentBan[0]).PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 15, "Select a ban ", true);
            writeLineWhenPossible(35, 16, (" " + currentSpell0[0]).PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 16, "Select summoner spell 1 ", true);
            writeLineWhenPossible(35, 17, (" " + currentSpell1[0]).PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 17, "Select summoner spell 2 ", true);
            writeLineWhenPossible(35, 18, (" " + "Disabled").PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 18, "Enable auto accept ", true);
            writeLineWhenPossible(35, 20, (" Info").PadLeft(44, ' '), true);
            writeLineWhenPossible(35, 20, "Settings ", true);

            if (isAutoAcceptOn)
            {
                writeLineWhenPossible(70, 18, ". Enabled", false);
            }

            if (consolePosLast == 5)
            {
                writeLineWhenPossible(32, 20, "->", false);
            }
            else if (consolePosLast == 6)
            {
                writeLineWhenPossible(72, 20, "->", false);
            }
            else
            {
                writeLineWhenPossible(32, 14 + lastPosMainNav, "->", false);
            }

            canMovePos = true;
        }

        private static void mainMenuNav()
        {
            // Swap sub menu
            if (currentPos == 0)
            {
                currentChampPicker = 0;
                champSelector();
            }
            else if (currentPos == 1)
            {
                currentChampPicker = 1;
                champSelector();
            }
            else if (currentPos == 2)
            {
                currentSpellSlot = 0;
                spellSelector();
            }
            else if (currentPos == 3)
            {
                currentSpellSlot = 1;
                spellSelector();
            }
            else if (currentPos == 4)
            {
                toggleAutoAccept();
            }
            else if (currentPos == 5)
            {
                settingsMenu();
            }
            else if (currentPos == 6)
            {
                infoMenu();
            }
        }

        private static void settingsMenu()
        {
            canMovePos = false;
            currentWindow = "settingsMenu";
            currentPos = 0;
            consolePosLast = 0;

            Console.Clear();

            writeLineWhenPossible(35, 13, "Save settings/config ......................", true);
            writeLineWhenPossible(35, 14, "Preload data ..............................", true);
            writeLineWhenPossible(35, 15, "Instalock bans/picks ......................", true);
            writeLineWhenPossible(35, 16, "Lock/ban delay ............................", true);

            if (settings[0] == "true")
            {
                writeLineWhenPossible(75, 13, " Yes", true);
            }
            else
            {
                writeLineWhenPossible(75, 13, ". No", true);
            }
            if (settings[1] == "true")
            {
                writeLineWhenPossible(75, 14, " Yes", true);
            }
            else
            {
                writeLineWhenPossible(75, 14, ". No", true);
            }
            if (settings[2] == "true")
            {
                writeLineWhenPossible(75, 15, " Yes", true);
            }
            else
            {
                writeLineWhenPossible(75, 15, ". No", true);
            }

            writeLineWhenPossible(70, 16, (" " + settings[3]).PadLeft(9, '.'), true);

            writeLineWhenPossible(32, 13, "->", false);

            canMovePos = true;
        }

        private static void settingsMenuNav(int item)
        {
            // Select item to toggle from settings
            // TODO: maybe delete switches and put a simple IF
            if (item == 3)
            {
                if (settings[3].Length == 0)
                {
                    settings[3] = "0";
                }
                else
                {
                    settings[3] = settings[3].TrimStart('0');
                }
                lockDelay = Int32.Parse(settings[3]);
                writeLineWhenPossible(70, 16, (" " + settings[3]).PadLeft(9, '.'), true);
            }
            else
            {
                if (settings[item] == "true")
                {
                    settings[item] = "false";
                    writeLineWhenPossible(75, item + 13, ". No", false);
                    switch (item)
                    {
                        case 0:
                            if (settings[1] == "true")
                            {
                                // if autosave is off then turn data preload off becasue otherwise it doesn't make sense
                                settingsMenuNav(1);
                            }
                            deleteSettings();
                            break;
                        case 1:
                            // nodataPreload
                            break;
                        case 2:
                            // instalock
                            break;
                    }
                }
                else
                {
                    settings[item] = "true";
                    writeLineWhenPossible(75, item + 13, " Yes", false);
                    switch (item)
                    {
                        case 0:
                            break;
                        case 1:
                            if (settings[0] == "false")
                            {
                                // if autosave is off then turn it on becasue otherwise data preload being enabled doesn't make sense
                                settingsMenuNav(0);
                            }
                            // dataPreload
                            break;
                        case 2:
                            // instalock
                            break;
                    }
                }
            }
            if (settings[0] == "true")
            {
                settingsSave();
            }
        }

        private static void infoMenu()
        {
            canMovePos = false;
            currentWindow = "infoMenu";
            currentPos = 0;
            consolePosLast = 0;

            Console.Clear();

            writeLineWhenPossible(35, 12, (" Artem").PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 12, "Made by ", true);
            writeLineWhenPossible(35, 13, (" 2.7").PadLeft(44, '.'), true);
            writeLineWhenPossible(35, 13, "Version ", true);
            writeLineWhenPossible(35, 15, padSides("Source code:", 46)[0], true);
            writeLineWhenPossible(35, 16, " github.com/sweetriverfish/LeagueAutoAccept", true);
        }

        private static void exitMenu()
        {
            canMovePos = false;
            currentWindow = "exitMenu";
            currentPos = 0;
            consolePosLast = 0;

            Console.Clear();

            writeLineWhenPossible(35, 13, padSides("Are you sure you want to close this app?", 46)[0], true);
            writeLineWhenPossible(42, 15, (" No").PadLeft(32, ' '), true);
            writeLineWhenPossible(42, 15, "Yes ", true);


            writeLineWhenPossible(39, 15, "->", false);

            canMovePos = true;
        }

        private static void exitMenuNav()
        {
            // Swap sub menu
            if (currentPos == 0)
            {
                Environment.Exit(0);
            }
            else if (currentPos == 1)
            {
                if (isLeagueOpen)
                {
                    mainScreen();
                }
                else
                {
                    LeagueClientIsClosedMsg();
                }
            }
        }

        private static void champSelector()
        {
            // Allows you to select which champion you wanna instalock
            currentWindow = "champSelector";
            filterKeyword = "";
            Console.Clear();

            loadChampionsList();

            displayChamps();
            updateCurrentFilter();
        }

        private static void loadChampionsList()
        {
            Console.Clear();

            if (!champsSorterd.Any())
            {
                List<itemList> champs = new List<itemList>();

                if (currentSummonerId == "")
                {
                    writeLineWhenPossible(1, 15, padSides("Getting summoner ID...", 118)[0], true);
                    string[] currentSummoner = clientRequestUntilSuccess(leagueAuth, "GET", "lol-summoner/v1/current-summoner", "");
                    Console.Clear();
                    currentSummonerId = currentSummoner[1].Split("summonerId\":")[1].Split(',')[0];
                }

                writeLineWhenPossible(1, 15, padSides("Getting champions and ownership list...", 118)[0], true);
                string[] ownedChamps = clientRequestUntilSuccess(leagueAuth, "GET", "lol-champions/v1/inventories/" + currentSummonerId + "/champions-minimal", "");
                Console.Clear();
                string[] champsSplit = ownedChamps[1].Split("},{");

                foreach (var champ in champsSplit)
                {
                    string champName = champ.Split("name\":\"")[1].Split('"')[0];
                    string champId = champ.Split("id\":")[1].Split(',')[0];
                    string champOwned = champ.Split("owned\":")[1].Split(',')[0];
                    string champFree = champ.Split("freeToPlay\":")[1].Split(',')[0];

                    // Fuck the yeti
                    if (champName == "Nunu & Willump")
                    {
                        champName = "Nunu";
                    }

                    // Check if the champ can be picked
                    bool isFree;
                    if (champOwned == "true" || champFree == "true")
                    {
                        isFree = true;
                    }
                    else
                    {
                        isFree = false;
                    }
                    champs.Add(new itemList() { name = champName, id = champId, free = isFree });
                }

                // Count total champs
                totalChamps = champs.Count;
                // Sort alphabetically
                champsSorterd = champs.OrderBy(o => o.name).ToList();
            }

            Console.Clear();
        }

        private static void displayChamps()
        {
            currentPos = 0;
            consolePosLast = 0;
            Console.SetCursorPosition(0, 0);

            List<itemList> champsFiltered = new List<itemList>();
            if ("none".Contains(filterKeyword.ToLower()))
            {
                champsFiltered.Add(new itemList() { name = "None", id = "0" });
            }
            foreach (var champ in champsSorterd)
            {
                if (champ.name.ToLower().Contains(filterKeyword.ToLower()))
                {
                    // Make sure the champ is free or if it's for a ban before adding it to the list
                    if (champ.free || currentChampPicker == 1)
                    {
                        champsFiltered.Add(new itemList() { name = champ.name, id = champ.id });
                    }
                }
            }

            totalChamps = champsFiltered.Count;

            int currentRow = 0;
            string[] champsOutput = new string[totalRows];

            foreach (var champ in champsFiltered)
            {
                string line = "   " + champ.name;
                line = line.PadRight(20, ' ');

                champsOutput[currentRow] += line;

                currentRow++;
                if (currentRow >= totalRows)
                {
                    currentRow = 0;
                }
            }

            foreach (var line in champsOutput)
            {
                if (line != null)
                {
                    string lineNew = line.Remove(line.Length - 1);
                    lineNew = lineNew.PadRight(119, ' ');
                    writeLineWhenPossible(-1, -1, lineNew, true);
                }
                else
                {
                    string lineNew = "".PadRight(119, ' ');
                    writeLineWhenPossible(-1, -1, lineNew, true);
                }
            }
            writeLineWhenPossible(0, 0, "->", false);
        }

        private static void saveSelectedChamp()
        {
            List<itemList> champsFiltered = new List<itemList>();
            if ("none".Contains(filterKeyword.ToLower()))
            {
                champsFiltered.Add(new itemList() { name = "None", id = "0" });
            }
            foreach (var champ in champsSorterd)
            {
                if (champ.name.ToLower().Contains(filterKeyword.ToLower()))
                {
                    if (currentChampPicker == 0)
                    {
                        if (!champ.free)
                        {
                            continue;
                        }
                    }
                    champsFiltered.Add(new itemList() { name = champ.name, id = champ.id });
                }
            }

            if (champsFiltered.Count > 0)
            {
                string name;
                string id;
                if (currentPos < 0)
                {
                    name = "None";
                    id = "0";
                }
                else
                {
                    name = champsFiltered[currentPos].name;
                    id = champsFiltered[currentPos].id;
                }
                if (currentChampPicker == 0)
                {
                    currentChamp[0] = name;
                    currentChamp[1] = id;
                }
                else
                {
                    currentBan[0] = name;
                    currentBan[1] = id;
                }
                if (settings[0] == "true")
                {
                    settingsSave();
                }
            }
        }

        private static void spellSelector()
        {
            currentWindow = "spellSelector";
            filterKeyword = "";
            Console.Clear();

            loadSpellsList();

            displaySpells();
            updateCurrentFilter();
        }

        private static void loadSpellsList()
        {
            Console.Clear();
            if (!spellsSorted.Any())
            {
                List<string> enabledSpells = new List<string>();

                if (currentSummonerId == "")
                {
                    writeLineWhenPossible(1, 15, padSides("Getting summoner ID...", 118)[0], true);
                    string[] currentSummoner = clientRequestUntilSuccess(leagueAuth, "GET", "lol-summoner/v1/current-summoner", "");
                    Console.Clear();
                    currentSummonerId = currentSummoner[1].Split("summonerId\":")[1].Split(',')[0];
                }

                writeLineWhenPossible(1, 15, padSides("Getting a list of available summoner spells...", 118)[0], true);
                string[] availableSpells = clientRequestUntilSuccess(leagueAuth, "GET", "lol-collections/v1/inventories/" + currentSummonerId + "/spells", "");
                Console.Clear();
                string[] spellsSplit = availableSpells[1].Split('[')[1].Split(']')[0].Split(',');

                writeLineWhenPossible(1, 15, padSides("Getting a list of available gamemodes...", 118)[0], true);
                string[] platformConfig = clientRequestUntilSuccess(leagueAuth, "GET", "lol-platform-config/v1/namespaces", "");
                Console.Clear();
                string[] enabledGameModes = platformConfig[1].Split("EnabledModes\":[")[1].Split(']')[0].Split(',');
                string[] inactiveSpellsPerGameMode = platformConfig[1].Split("gameModeToInactiveSpellIds\":{")[1].Split('}')[0].Split("],");

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
                writeLineWhenPossible(1, 15, padSides("Getting summoner spell names...", 118)[0], true);
                string[] spellsJson = clientRequest(leagueAuth, "GET", "lol-game-data/assets/v1/summoner-spells.json", "");
                Console.Clear();
                string[] spellsJsonSplit = spellsJson[1].Split('{');

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

        private static void displaySpells()
        {
            currentPos = 0;
            consolePosLast = 0;
            Console.SetCursorPosition(0, 0);

            List<itemList> spellsFiltered = new List<itemList>();
            if ("none".Contains(filterKeyword.ToLower()))
            {
                spellsFiltered.Add(new itemList() { name = "None", id = "0" });
            }
            foreach (var spell in spellsSorted)
            {
                if (spell.name.ToLower().Contains(filterKeyword.ToLower()))
                {
                    spellsFiltered.Add(new itemList() { name = spell.name, id = spell.id });
                }
            }

            totalSpells = spellsFiltered.Count;

            int currentRow = 0;
            string[] spelloutput = new string[totalRows];

            foreach (var spell in spellsFiltered)
            {
                string line = "   " + spell.name;
                line = line.PadRight(20, ' ');

                spelloutput[currentRow] += line;

                currentRow++;
                if (currentRow >= totalRows)
                {
                    currentRow = 0;
                }
            }

            foreach (var line in spelloutput)
            {
                if (line != null)
                {
                    string lineNew = line.Remove(line.Length - 1);
                    lineNew = lineNew.PadRight(119, ' ');
                    writeLineWhenPossible(-1, -1, lineNew, true);
                }
                else
                {
                    string lineNew = "".PadRight(119, ' ');
                    Console.WriteLine(lineNew);
                    writeLineWhenPossible(-1, -1, lineNew, true);
                }
            }
            writeLineWhenPossible(0, 0, "->", false);
        }

        private static void saveSelectedSpell()
        {
            List<itemList> spellsFiltered = new List<itemList>();
            if ("none".Contains(filterKeyword.ToLower()))
            {
                spellsFiltered.Add(new itemList() { name = "None", id = "0" });
            }
            foreach (var spell in spellsSorted)
            {
                if (spell.name.ToLower().Contains(filterKeyword.ToLower()))
                {
                    spellsFiltered.Add(new itemList() { name = spell.name, id = spell.id });
                }
            }

            if (spellsFiltered.Count > 0)
            {
                string name;
                string id;
                if (currentPos < 0)
                {
                    name = "None";
                    id = "0";
                }
                else
                {
                    name = spellsFiltered[currentPos].name;
                    id = spellsFiltered[currentPos].id;
                }
                if (currentSpellSlot == 0)
                {
                    currentSpell0[0] = name;
                    currentSpell0[1] = id;
                }
                else
                {
                    currentSpell1[0] = name;
                    currentSpell1[1] = id;
                }
                if (settings[0] == "true")
                {
                    settingsSave();
                }
            }

        }

        private static void updateCurrentFilter()
        {
            if (currentWindow == "champSelector")
            {
                displayChamps();
            }
            else if (currentWindow == "spellSelector")
            {
                displaySpells();
            }
            currentPos = 0;
            string consoleLine = "Search: " + filterKeyword;
            string[] consoleLine2 = padSides(consoleLine, 118);
            writeLineWhenPossible(1, 29, consoleLine2[0], false);
        }

        private static void settingsSave()
        {
            string config = "v2.7:" + currentChamp[0] + ":" + currentChamp[1] + ":" + currentBan[0] + ":" + currentBan[1] + ":" + currentSpell0[0] + ":" + currentSpell0[1] + ":" + currentSpell1[0] + ":" + currentSpell1[1] + ":" + shouldAutoAcceptbeOn + ":" + settings[1] + ":" + settings[2] + ":" + settings[3];

            string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\Leauge Auto Accept Config.txt";
            FileStream fParameter = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter);
            m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
            m_WriterParameter.Write(config);
            m_WriterParameter.Flush();
            m_WriterParameter.Close();
        }

        private static void deleteSettings()
        {
            string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\Leauge Auto Accept Config.txt";
            File.Delete(dirParameter);
        }

        private static void loadSettings()
        {
            string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\Leauge Auto Accept Config.txt";
            if (File.Exists(dirParameter))
            {
                string text = File.ReadAllText(dirParameter);
                string[] text2 = text.Split(':');
                if (text2[0] == "v2.7")
                {
                    currentChamp[0] = text2[1];
                    currentChamp[1] = text2[2];
                    currentBan[0] = text2[3];
                    currentBan[1] = text2[4];
                    currentSpell0[0] = text2[5];
                    currentSpell0[1] = text2[6];
                    currentSpell1[0] = text2[7];
                    currentSpell1[1] = text2[8];

                    if (text2[9] == "True")
                    {
                        shouldAutoAcceptbeOn = true;
                    }
                    if (text2[10] == "true")
                    {
                        settings[1] = "true";
                    }
                    if (text2[11] == "true")
                    {
                        settings[2] = "true";
                    }

                    settings[3] = text2[12];
                    lockDelay = Int32.Parse(settings[3]);

                    settings[0] = "true";
                }
            }
        }

        private static void toggleAutoAccept()
        {
            if (isAutoAcceptOn)
            {
                isAutoAcceptOn = false;
                shouldAutoAcceptbeOn = false;
                writeLineWhenPossible(70, 18, " Disabled", false);
            }
            else
            {
                isAutoAcceptOn = true;
                shouldAutoAcceptbeOn = true;
                writeLineWhenPossible(70, 18, ". Enabled", false);
            }
            if (settings[0] == "true")
            {
                settingsSave();
            }
        }

        private static void acceptQueue()
        {
            while (true)
            {

                if (isAutoAcceptOn)
                {
                    // Get the current game session
                    string[] gameSession = clientRequest(leagueAuth, "GET", "lol-gameflow/v1/session", "");
                    if (gameSession[0] == "200")
                    {
                        // Get the current game state
                        string phase = gameSession[1].Split("phase").Last().Split('"')[2];

                        // Get the current state, accept the queue if need to
                        switch (phase)
                        {
                            case "Lobby":
                                Thread.Sleep(5000);
                                break;
                            case "Matchmaking":
                                Thread.Sleep(2000);
                                break;
                            case "ReadyCheck":
                                // Accept queue if found
                                string[] matchAccept = clientRequest(leagueAuth, "POST", "lol-matchmaking/v1/ready-check/accept", "");
                                if (matchAccept[0] == "204")
                                {
                                    // Maybe I should add some sleep?
                                }
                                break;
                            case "ChampSelect":
                                // Get data for the current ongoing champ select
                                string[] currentChampSelect = clientRequest(leagueAuth, "GET", "lol-champ-select/v1/session", "");
                                if (currentChampSelect[0] == "200")
                                {
                                    // Get needed data from the current champ select
                                    string currentChatRoom = currentChampSelect[1].Split("multiUserChatId\":\"")[1].Split('"')[0];
                                    if (lastChatRoom != currentChatRoom || lastChatRoom == "")
                                    {
                                        // Reset stuff in case someone dodged the champ select
                                        pickedChamp = false;
                                        lockedChamp = false;
                                        pickedBan = false;
                                        LockedBan = false;
                                        pickedSpell0 = false;
                                        pickedSpell1 = false;
                                    }
                                    lastChatRoom = currentChatRoom;
                                    if (pickedChamp && lockedChamp && pickedBan && LockedBan && pickedSpell0 && pickedSpell1)
                                    {
                                        // Sleep a little if we already did everything we needed to do
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        // Get more needed data from the current champ select
                                        string localPlayerCellId = currentChampSelect[1].Split("localPlayerCellId\":")[1].Split(',')[0];

                                        if (currentChamp[1] == "0")
                                        {
                                            pickedChamp = true;
                                            lockedChamp = true;
                                        }
                                        if (currentBan[1] == "0")
                                        {
                                            pickedBan = true;
                                            LockedBan = true;
                                        }
                                        if (currentSpell0[1] == "0")
                                        {
                                            pickedSpell0 = true;
                                        }
                                        if (currentSpell1[1] == "0")
                                        {
                                            pickedSpell1 = true;
                                        }
                                        if (!pickedChamp || !lockedChamp || !pickedBan || !LockedBan)
                                        {
                                            // First make sure we are actually able to pick a champion
                                            string csActs = currentChampSelect[1].Split("actions\":[[{")[1].Split("}]],")[0];
                                            csActs = csActs.Replace("}],[{", "},{");
                                            string[] csActsArr = csActs.Split("},{");
                                            foreach (var act in csActsArr)
                                            {
                                                string ActCctorCellId = act.Split("actorCellId\":")[1].Split(',')[0];
                                                string championId = act.Split("championId\":")[1].Split(',')[0];
                                                string actId = act.Split(",\"id\":")[1].Split(',')[0];
                                                string ActCompleted = act.Split("completed\":")[1].Split(',')[0];
                                                string ActIsInProgress = act.Split("isInProgress\":")[1].Split(',')[0];
                                                string ActType = act.Split("type\":\"")[1].Split('"')[0];
                                                if (ActCctorCellId == localPlayerCellId && ActCompleted == "false" && ActType == "pick")
                                                {
                                                    if (!pickedChamp)
                                                    {
                                                        // hover champion when champ select starts, no need to check for whenever it's my turn or not to pick it
                                                        string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"championId\":" + currentChamp[1] + "}");
                                                        if (champSelectAction[0] == "204")
                                                        {
                                                            pickedChamp = true;
                                                        }
                                                    }
                                                    // ActIsInProgress makes sure it's my turn to pick the champion
                                                    if (ActIsInProgress == "true")
                                                    {
                                                        // Mark the start of the phase
                                                        if (actId != lastActId)
                                                        {
                                                            lastActId = actId;
                                                            lastActStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                        }
                                                        if (!lockedChamp)
                                                        {
                                                            // check the instalock setting
                                                            if (settings[2] == "false")
                                                            {
                                                                string timer = currentChampSelect[1].Split("totalTimeInPhase\":")[1].Split("}")[0];
                                                                long timerInt = Convert.ToInt64(timer);
                                                                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                                if (currentTime >= lastActStartTime + timerInt - lockDelay)
                                                                {
                                                                    string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");
                                                                    if (champSelectAction[0] == "204")
                                                                    {
                                                                        lockedChamp = true;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");
                                                                if (champSelectAction[0] == "204")
                                                                {
                                                                    lockedChamp = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (ActCctorCellId == localPlayerCellId && ActCompleted == "false" && ActType == "ban")
                                                {
                                                    string champSelectPhase = currentChampSelect[1].Split("\"phase\":\"")[1].Split('"')[0];

                                                    // ActIsInProgress makes sure it's my turn to pick the champion
                                                    if (ActIsInProgress == "true" && champSelectPhase != "PLANNING")
                                                    {
                                                        // Mark the start of the phase
                                                        if (actId != lastActId)
                                                        {
                                                            lastActId = actId;
                                                            lastActStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                        }

                                                        if (!pickedBan)
                                                        {
                                                            string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"championId\":" + currentBan[1] + "}");
                                                            if (champSelectAction[0] == "204")
                                                            {
                                                                pickedBan = true;
                                                                Thread.Sleep(1000);
                                                            }
                                                        } else if (!LockedBan)
                                                        {
                                                            // check the instalock setting
                                                            if (settings[2] == "false")
                                                            {
                                                                string timer = currentChampSelect[1].Split("totalTimeInPhase\":")[1].Split("}")[0];
                                                                long timerInt = Convert.ToInt64(timer);
                                                                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                                if (currentTime >= (lastActStartTime + timerInt - lockDelay))
                                                                {
                                                                    string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");
                                                                    if (champSelectAction[0] == "204")
                                                                    {
                                                                        LockedBan = true;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");
                                                                if (champSelectAction[0] == "204")
                                                                {
                                                                    LockedBan = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!pickedSpell0)
                                        {
                                            string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/my-selection", "{\"spell1Id\":" + currentSpell0[1] + "}");
                                            if (champSelectAction[0] == "204")
                                            {
                                                pickedSpell0 = true;
                                            }
                                        }
                                        if (!pickedSpell1)
                                        {
                                            string[] champSelectAction = clientRequest(leagueAuth, "PATCH", "lol-champ-select/v1/session/my-selection", "{\"spell2Id\":" + currentSpell1[1] + "}");
                                            if (champSelectAction[0] == "204")
                                            {
                                                pickedSpell1 = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Failed to get champ select data, should probably retry. maybe we did it too early? maybe the game was dodged?
                                    // state = "Failed to get champ select data";
                                }
                                break;
                            case "InProgress":
                                // No need to spam requests
                                // In game
                                Thread.Sleep(9000);
                                break;
                            case "WaitingForStats":
                                // No need to spam requests
                                // Waiting for stats screen (nice game riot)
                                Thread.Sleep(9000);
                                break;
                            case "PreEndOfGame":
                                // No need to spam requests
                                // Honor screen
                                Thread.Sleep(9000);
                                break;
                            case "EndOfGame":
                                // No need to spam requests
                                // End of game stats screen
                                Thread.Sleep(5000);
                                break;
                            default:
                                //Debug.WriteLine(phase);
                                // TODO: add more special cases?
                                Thread.Sleep(1000);
                                break;
                        }

                        if (phase != "ChampSelect")
                        {
                            lastChatRoom = "";
                        }
                    }
                }
                if (!isAutoAcceptOn)
                {
                    Thread.Sleep(1000);
                }
                Thread.Sleep(50);
            }
        }

        private static void ReadKeys()
        {
            while (true)
            {
                bool movePointer = false;

                ConsoleKeyInfo key = new ConsoleKeyInfo();
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentPos--;
                        movePointer = true;
                        break;

                    case ConsoleKey.DownArrow:
                        currentPos++;
                        movePointer = true;
                        break;

                    case ConsoleKey.RightArrow:
                        currentPos += totalRows;
                        movePointer = true;
                        break;

                    case ConsoleKey.LeftArrow:
                        currentPos -= totalRows;
                        movePointer = true;
                        break;

                    case ConsoleKey.D1:
                        mainScreen();
                        break;

                    case ConsoleKey.D2:
                        currentChampPicker = 0;
                        champSelector();
                        break;

                    case ConsoleKey.D3:
                        currentChampPicker = 1;
                        champSelector();
                        break;

                    case ConsoleKey.D4:
                        currentSpellSlot = 0;
                        spellSelector();
                        break;

                    case ConsoleKey.D5:
                        currentSpellSlot = 1;
                        spellSelector();
                        break;

                    case ConsoleKey.D6:
                        settingsMenu();
                        break;

                    case ConsoleKey.D7:
                        infoMenu();
                        break;

                    case ConsoleKey.Escape:
                        if (currentWindow == "mainScreen")
                        {
                            exitMenu();
                        }
                        else if (currentWindow == "exitMenu")
                        {
                            if (isLeagueOpen)
                            {
                                mainScreen();
                            }
                            else
                            {
                                LeagueClientIsClosedMsg();
                            }
                        }
                        else if (currentWindow == "lcuClosed")
                        {
                            exitMenu();
                        }
                        else if (currentWindow == "initializing")
                        {
                            // do nothing I guess sounds pointless
                        }
                        else if (currentWindow != "mainScreen")
                        {
                            if (filterKeyword == "Lochel")
                            {
                                filterKeyword = "";
                            }
                            mainScreen();
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (currentWindow == "mainScreen")
                        {
                            mainMenuNav();
                        }
                        else if (currentWindow == "settingsMenu")
                        {
                            settingsMenuNav(currentPos);
                        }
                        else if (currentWindow == "exitMenu")
                        {
                            exitMenuNav();
                        }
                        else if (currentWindow == "champSelector" || currentWindow == "spellSelector")
                        {
                            if (filterKeyword != "Lochel")
                            {
                                if (currentWindow == "champSelector")
                                {
                                    saveSelectedChamp();
                                }
                                else if (currentWindow == "spellSelector")
                                {
                                    saveSelectedSpell();
                                }
                                mainScreen();
                            }
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (currentWindow == "champSelector" || currentWindow == "spellSelector")
                        {
                            if (filterKeyword.Length > 0)
                            {
                                filterKeyword = filterKeyword.Remove(filterKeyword.Length - 1);
                                updateCurrentFilter();
                            }
                        }
                        else if (currentWindow == "settingsMenu")
                        {
                            if (currentPos == 3)
                            {
                                if (settings[3].Length > 0)
                                {
                                    settings[3] = settings[3].Remove(settings[3].Length - 1);
                                    settingsMenuNav(currentPos);
                                }
                            }
                        }
                        break;

                    default:
                        if (currentWindow == "champSelector" || currentWindow == "spellSelector")
                        {
                            if (IsEnglishLetter(key.KeyChar))
                            {
                                if (filterKeyword.Length < 100)
                                {
                                    filterKeyword += key.KeyChar;
                                    updateCurrentFilter();
                                }
                            }
                        }
                        else if (currentWindow == "settingsMenu")
                        {
                            if (currentPos == 3)
                            {
                                if (IsNumeric(key.KeyChar))
                                {
                                    if (settings[3].Length < 5)
                                    {
                                        settings[3] += key.KeyChar;
                                        settingsMenuNav(currentPos);
                                    }
                                }
                            }
                        }
                        break;
                }

                if (filterKeyword == "Lochel")
                {
                    printHeart();
                }
                else if (movePointer && currentWindow != "infoMenu" && currentWindow != "lcuClosed" && currentWindow != "initializing")
                {
                    while (!canMovePos)
                    {
                        Thread.Sleep(2);
                    }
                    isMovingPos = true;
                    int topPad = 0;
                    int leftPad = 0;
                    int maxPos = 0;
                    if (currentWindow == "mainScreen")
                    {
                        topPad = 14;
                        leftPad = 32;
                        maxPos = 7;
                    }
                    else if (currentWindow == "exitMenu")
                    {
                        topPad = 15;
                        leftPad = 39;
                        maxPos = 2;
                    }
                    else if (currentWindow == "champSelector")
                    {
                        maxPos = totalChamps;
                    }
                    else if (currentWindow == "spellSelector")
                    {
                        maxPos = totalSpells;
                    }
                    else if (currentWindow == "settingsMenu")
                    {
                        topPad = 13;
                        leftPad = 32;
                        maxPos = 4;
                    }

                    if (currentWindow == "mainScreen" && consolePosLast > 4)
                    {
                        // Handles the weird main menu navigation
                        if (consolePosLast == 5)
                        {
                            Console.SetCursorPosition(32, 20);
                        }
                        else if (consolePosLast == 6)
                        {
                            Console.SetCursorPosition(72, 20);
                        }
                    }
                    else if (currentWindow == "exitMenu" && consolePosLast == 1)
                    {
                        // Handles the weird exit menu navigation
                        Console.SetCursorPosition(69, 15);
                    }
                    else
                    {
                        int[] consolePosOld = getPositionOnConsole(consolePosLast, leftPad, topPad);
                        Console.SetCursorPosition(consolePosOld[1], consolePosOld[0]);
                    }
                    writeLineWhenPossible(-1, -1, "  ", false);

                    if (currentPos < 0)
                    {
                        currentPos = 0;
                    }
                    else if (currentPos >= maxPos)
                    {
                        currentPos = maxPos - 1;
                    }
                    consolePosLast = currentPos;
                    if (currentWindow == "mainScreen")
                    {
                        lastPosMainNav = currentPos;
                    }

                    if (currentWindow == "mainScreen" && currentPos > 4)
                    {
                        // Handles the weird main menu navigation
                        if (currentPos == 5)
                        {
                            Console.SetCursorPosition(32, 20);
                        }
                        else if (currentPos == 6)
                        {
                            Console.SetCursorPosition(72, 20);
                        }
                    }
                    else if (currentWindow == "exitMenu" && currentPos == 1)
                    {
                        // Handles the weird exit menu navigation
                        Console.SetCursorPosition(69, 15);
                    }
                    else
                    {
                        int[] consolePos = getPositionOnConsole(currentPos, leftPad, topPad);
                        Console.SetCursorPosition(consolePos[1], consolePos[0]);
                    }
                    writeLineWhenPossible(-1, -1, "->", false);
                    if (currentWindow == "champSelector" || currentWindow == "spellSelector")
                    {
                        Console.SetCursorPosition(searchPos, 29);
                    }
                    isMovingPos = false;
                }
            }
        }

        private static void writeLineWhenPossible(int left, int top, string text, bool line)
        {
            if (isMovingPos)
            {
                Thread.Sleep(2);
            }
            canMovePos = false;
            if (left >= 0 && top >= 0)
            {
                Console.SetCursorPosition(left, top);
            }
            if (line)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            canMovePos = true;
        }

        private static int[] getPositionOnConsole(int pos, int leftPad, int topPad)
        {
            double num1 = pos / totalRows;  //  1.111111111111111
            double num2 = Math.Floor(num1); //  1
            double num3 = num2 * totalRows; //  27
            double num4 = pos - num3;       //  3

            int output1 = Convert.ToInt32(num4);
            int output2 = Convert.ToInt32(num2) * 20;

            if (output1 < 0)
                output1 = 0;
            if (output2 < 0)
                output2 = 0;

            int[] output = { output1 + topPad, output2 + leftPad };
            return output;
        }

        private static void printHeart()
        {
            // Print a heart
            Console.SetCursorPosition(54, 10);
            Console.WriteLine("oooo   oooo");
            Console.SetCursorPosition(52, 11);
            Console.WriteLine(" o    o o    o ");
            Console.SetCursorPosition(52, 12);
            Console.WriteLine("o      o      o");
            Console.SetCursorPosition(53, 13);
            Console.WriteLine("o           o");
            Console.SetCursorPosition(54, 14);
            Console.WriteLine("o         o");
            Console.SetCursorPosition(55, 15);
            Console.WriteLine("o       o");
            Console.SetCursorPosition(56, 16);
            Console.WriteLine("o     o");
            Console.SetCursorPosition(57, 17);
            Console.WriteLine("o   o");
            Console.SetCursorPosition(58, 18);
            Console.WriteLine("o o");
            Console.SetCursorPosition(59, 19);
            Console.WriteLine("o");
        }

        private static string[] getLeagueAuth(Process client)
        {
            // Open a cmd window
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            // Get the command line for our process
            cmd.StandardInput.WriteLine("wmic process where 'Processid=" + client.Id + "' get Commandline");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();

            // Parse the port and key into variables
            string Commandline = cmd.StandardOutput.ReadToEnd();
            string port = Commandline.Split("--app-port=")[1].Split('"')[0];
            string key = Commandline.Split("--remoting-auth-token=")[1].Split('"')[0];

            // Exit cmd
            cmd.WaitForExit();
            
            // Get the encoded auth key
            string auth = "riot:" + key;
            var authPlainText = System.Text.Encoding.UTF8.GetBytes(auth);
            string authBase64 = System.Convert.ToBase64String(authPlainText);

            // Return content
            string[] output = { authBase64, port };
            return output;
        }

        private static string[] clientRequest(string[] leagueAuth, string method, string url, string body)
        {
            // Ignore invalid https
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                // Set URL
                WebRequest request = WebRequest.Create("https://127.0.0.1:" + leagueAuth[1] + "/" + url);

                // Set headers
                request.Method = method;
                request.Headers.Add("Authorization", "Basic " + leagueAuth[0]);
                request.ContentType = "application/json";

                // Send POST data when doing a post request
                Stream dataStream;
                if (method == "POST" || method == "PUT" || method == "PATCH")
                {
                    string postData = body;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = byteArray.Length;
                    dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                // Get the response
                WebResponse response;
                try
                {
                    response = request.GetResponse();
                }
                catch (WebException e)
                {
                    response = e.Response;
                }

                // If the response is null (League client closed?)
                if (((HttpWebResponse)response) == null)
                {
                    string[] outputDef = { "999", "" };
                    return outputDef;
                }

                // Get the HTTP status code
                int statusCode = (int)((HttpWebResponse)response).StatusCode;
                string statusString = statusCode.ToString();

                // Get the body
                string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Clean up the stream
                response.Close();

                // Return content
                string[] output = { statusString, responseFromServer };
                return output;
            }
            catch
            {
                // If the URL is invalid (League client closed?)
                string[] output = { "999", "" };
                return output;
            }
        }

        private static string[] clientRequestUntilSuccess(string[] leagueAuth, string method, string url, string body)
        {
            string[] request = { "000", "" };
            while (request[0].Substring(0, 1) != "2")
            {
                request = clientRequest(leagueAuth, method, url, body);
                if (request[0].Substring(0, 1) == "2")
                {
                    return request;
                }
                else
                {
                    if (CheckIfLeagueClientIsOpen())
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return request;
                    }
                }
            }
            return request;
        }

        private static bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z' || c == ' ');
        }

        private static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private static string[] padSides(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            string output = source.PadLeft(padLeft);
            int padRight = length - output.Length;
            output = output.PadRight(length);
            string[] final = { output, padLeft.ToString(), padRight.ToString() };
            return final;
        }
    }
}
