using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Leauge_Auto_Accept
{
    internal class MainLogic
    {
        public static bool isAutoAcceptOn = false;

        private static bool pickedChamp = false;
        private static bool lockedChamp = false;
        private static bool pickedBan = false;
        private static bool lockedBan = false;
        private static bool pickedRunes = false;
        private static bool pickedSpell1 = false;
        private static bool pickedSpell2 = false;
        private static bool sentChatMessages = false;

        // Arena
        private static bool isArena = false;
        private static string crowdFavorite1ChampId = "";
        private static string crowdFavorite2ChampId = "";
        private static string crowdFavorite3ChampId = "";
        private static string crowdFavorite4ChampId = "";
        private static string crowdFavorite5ChampId = "";

        private static long lastActStartTime;
        private static long champSelectStart;
        private static string lastActId = "";
        private static string lastChatRoom = "";

        private static long queueStartTime;
        private static string lastPhase = "";

        public static void acceptQueue()
        {
            while (true)
            {
                if (isAutoAcceptOn)
                {
                    string[] gameSession = LCU.clientRequest("GET", "lol-gameflow/v1/session");

                    if (gameSession[0] == "200")
                    {
                        string phase = gameSession[1].Split("phase").Last().Split('"')[2];

                        if (Settings.autoRestartQueue)
                        {
                            handleQueueRestart(phase);
                        }

                        switch (phase)
                        {
                            case "Lobby":
                                Thread.Sleep(5000);
                                break;
                            case "Matchmaking":
                                handleMatchmakingCancel();
                                Thread.Sleep(2000);
                                break;
                            case "ReadyCheck":
                                handleMatchmakingAccept();
                                break;
                            case "ChampSelect":
                                handleChampSelect();
                                handlePickOrderSwap();
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
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static void handleMatchmakingCancel()
        {
            if (!Settings.cancelQueueAfterDodge)
            {
                return;
            }
            if (lastChatRoom != "")
            {
                LCU.clientRequest("DELETE", "lol-lobby/v2/lobby/matchmaking/search");
                lastChatRoom = "";
            }
        }

        private static void handleMatchmakingAccept()
        {
            if (!Settings.cancelQueueAfterDodge)
            {
                LCU.clientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
                return;
            }
            else
            {
                if (lastChatRoom != "")
                {
                    LCU.clientRequest("POST", "lol-matchmaking/v1/ready-check/decline");
                    lastChatRoom = "";
                }
                else
                {
                    LCU.clientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
                }
            }
        }

        private static void handleQueueRestart(string phase)
        {
            if (phase == "Matchmaking" && phase != lastPhase)
            {
                queueStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
            else if (phase == "Matchmaking")
            {
                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if ((currentTime - Settings.queueMaxTime) > queueStartTime)
                {
                    LCU.clientRequest("DELETE", "lol-lobby/v2/lobby/matchmaking/search");
                    LCU.clientRequest("POST", "lol-lobby/v2/lobby/matchmaking/search");
                    queueStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            }
            lastPhase = phase;
        }


        private static void handleChampSelect()
        {
            // Get data for the current ongoing champ select
            string[] currentChampSelect = LCU.clientRequest("GET", "lol-champ-select/v1/session");

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
                    lockedBan = false;
                    pickedRunes = false;
                    pickedSpell1 = false;
                    pickedSpell2 = false;
                    sentChatMessages = false;
                    champSelectStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    
                    isArena = currentChampSelect[1].Contains("\"gameMode\":\"CHERRY\"");
                    crowdFavorite1ChampId = "";
                    crowdFavorite2ChampId = "";
                    crowdFavorite3ChampId = "";
                    crowdFavorite4ChampId = "";
                    crowdFavorite5ChampId = "";
                }
                lastChatRoom = currentChatRoom;

                if (pickedChamp && lockedChamp && pickedBan && lockedBan && pickedSpell1 && pickedSpell2 && sentChatMessages)
                {
                    // Sleep a little if we already did everything we needed to do
                    Thread.Sleep(1000);
                }
                else
                {
                    // Get more needed data from the current champ select
                    string localPlayerCellId = currentChampSelect[1].Split("localPlayerCellId\":")[1].Split(',')[0];

                    if (Settings.currentChamp[1] == "0")
                    {
                        pickedChamp = true;
                        lockedChamp = true;
                    }
                    if (Settings.currentBan[1] == "0")
                    {
                        pickedBan = true;
                        lockedBan = true;
                    }
                    if (Settings.currentChampRunes[1] == "0")
                    {
                        pickedRunes = true;
                    }
                    if (Settings.currentSpell1[1] == "0")
                    {
                        pickedSpell1 = true;
                    }
                    if (Settings.currentSpell2[1] == "0")
                    {
                        pickedSpell2 = true;
                    }
                    if (!Settings.chatMessagesEnabled)
                    {
                        sentChatMessages = true;
                    }
                    else
                    {
                        if (Settings.chatMessages.Count == 0)
                        {
                            sentChatMessages = true;
                        }
                    }

                    if (isArena)
                    {
                        // Output example:
                        //[
                        // 51,
                        // 23,
                        // 120,
                        // 555,
                        // 111
                        //]
                        string[] arenaCrowdFavorites = LCU.clientRequest("GET", "lol-lobby-team-builder/champ-select/v1/crowd-favorite-champion-list");
                        if (arenaCrowdFavorites[0] == "200")
                        {
                            string arenaCrowdFavoritesData = arenaCrowdFavorites[1].Replace("[", "").Replace("]", "").Replace("\n", "").Replace(" ", "");
                            string[] arenaCrowdFavoritesSplit = arenaCrowdFavoritesData.Split(',');

                            // Might be less than 5 champs if the player doesn't own that many champs
                            if (arenaCrowdFavoritesSplit.Length > 0) crowdFavorite1ChampId = arenaCrowdFavoritesSplit[0];
                            if (arenaCrowdFavoritesSplit.Length > 1) crowdFavorite2ChampId = arenaCrowdFavoritesSplit[1];
                            if (arenaCrowdFavoritesSplit.Length > 2) crowdFavorite3ChampId = arenaCrowdFavoritesSplit[2];
                            if (arenaCrowdFavoritesSplit.Length > 3) crowdFavorite4ChampId = arenaCrowdFavoritesSplit[3];
                            if (arenaCrowdFavoritesSplit.Length > 4) crowdFavorite5ChampId = arenaCrowdFavoritesSplit[4];
                        }
                    }
                    
                    if (!pickedChamp || !lockedChamp || !pickedBan || !lockedBan)
                    {
                        handleChampSelectActions(currentChampSelect, localPlayerCellId);
                    }
                    if (!sentChatMessages)
                    {
                        handleChampSelectChat(currentChatRoom);
                    }
                    if (!pickedSpell1)
                    {
                        string[] champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/my-selection", "{\"spell1Id\":" + Settings.currentSpell1[1] + "}");
                        if (champSelectAction[0] == "204")
                        {
                            pickedSpell1 = true;
                        }
                    }
                    if (!pickedSpell2)
                    {
                        string[] champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/my-selection", "{\"spell2Id\":" + Settings.currentSpell2[1] + "}");
                        if (champSelectAction[0] == "204")
                        {
                            pickedSpell2 = true;
                        }
                    }
                }
            }
        }


        // Check player's assigned position and adjust champion pick to primary (true) or secondary (false)
        private static bool handleChampPositionPreferences(string[] currentChampSelect, string localPlayerCellId)
        {
            // Check lobby endpoint for position preferences
            string[] lobbySession = LCU.clientRequest("GET", "lol-lobby/v2/lobby");
            if (!lobbySession[1].Contains("firstPositionPreference\":") || !lobbySession[1].Contains("secondPositionPreference\":")) return true;

            string firstPositionPreference = lobbySession[1].Split("firstPositionPreference\":")[1].Split(',')[0].Trim('"').ToLower();
            string secondPositionPreference = lobbySession[1].Split("secondPositionPreference\":")[1].Split(',')[0].Trim('"').ToLower();

            // Locate the "myTeam":[{ ... }] block
            int startIndex = currentChampSelect[1].IndexOf("\"myTeam\":[");
            if (startIndex == -1) return true;

            int teamDataStart = currentChampSelect[1].IndexOf("[{", startIndex);
            int teamDataEnd = currentChampSelect[1].IndexOf("}]", teamDataStart);
            if (teamDataStart == -1 || teamDataEnd == -1) return true;
            
            string myTeam = currentChampSelect[1].Substring(teamDataStart + 1, teamDataEnd - teamDataStart);

            // Split objects (players) using '{' as a separator
            string[] players = myTeam.Split(new string[] { "{" }, StringSplitOptions.RemoveEmptyEntries);

            string assignedPosition = "";
            foreach (var player in players)
            {
                // Match the cell IDs
                if (player.Contains("\"cellId\":" + localPlayerCellId + ","))
                {
                    string[] lines = player.Split(',');
                    foreach (var line in lines)
                    {
                        // Extract the assigned position
                        if (line.Contains("\"assignedPosition\""))
                        {
                            string[] parts = line.Split(':');
                            assignedPosition = parts[1].Trim('"');
                        }
                    }
                }
            }
            
            if (firstPositionPreference == assignedPosition) return true;
            if (secondPositionPreference == assignedPosition) return false;
            
            return true;
        }


        private static void handleChampSelectChat(string chatId)
        {
            string[] chats = LCU.clientRequest("GET", "lol-chat/v1/conversations", "");
            if (chats[1].Contains(chatId))
            {
                Data.loadPlayerChatId();

                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if ((currentTime - Settings.chatMessagesDelay) > champSelectStart)
                {
                    handleChampSelectChatSendMsg(chatId);
                }

            }
        }

        private static void handleChampSelectChatSendMsg(string chatId)
        {
            foreach (var message in Settings.chatMessages)
            {
                int attempts = 0;
                string httpRes = "";
                while (httpRes != "200" && attempts < 3)
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string body = "{\"type\":\"chat\",\"fromId\":\"" + Data.currentChatId + "\",\"fromSummonerId\":" + Data.currentSummonerId + ",\"isHistorical\":false,\"timestamp\":\"" + timestamp + "\",\"body\":\"" + message + "\"}";
                    string[] response = LCU.clientRequest("POST", "lol-chat/v1/conversations/" + chatId + "/messages", body);
                    attempts++;
                    httpRes = response[0];
                    Thread.Sleep(attempts * 20);
                }
            }
            sentChatMessages = true;
        }

        private static void handleChampSelectActions(string[] currentChampSelect, string localPlayerCellId)
        {
            // This logic skips modes that aren't draft
            if (!currentChampSelect[1].Contains("actions\":[[{")) return;

            string csActs = currentChampSelect[1].Split("actions\":[[{")[1].Split("}]],")[0];
            csActs = csActs.Replace("}],[{", "},{");
            string[] csActsArr = csActs.Split("},{");

            foreach (var act in csActsArr)
            {
                string ActCctorCellId = act.Split("actorCellId\":")[1].Split(',')[0];
                string ActCompleted = act.Split("completed\":")[1].Split(',')[0];
                string ActType = act.Split("type\":\"")[1].Split('"')[0];
                string championId = act.Split("championId\":")[1].Split(',')[0];
                string actId = act.Split(",\"id\":")[1].Split(',')[0];
                string ActIsInProgress = act.Split("isInProgress\":")[1].Split(',')[0];

                if (ActCctorCellId == localPlayerCellId && ActCompleted == "false" && ActType == "pick")
                {
                    bool usePrimaryChamp = handleChampPositionPreferences(currentChampSelect, localPlayerCellId);
                    handlePickAction(actId, championId, ActIsInProgress, currentChampSelect, usePrimaryChamp);
                }
                else if (ActCctorCellId == localPlayerCellId && ActCompleted == "false" && ActType == "ban")
                {
                    handleBanAction(actId, championId, ActIsInProgress, currentChampSelect);
                }
            }
        }

        private static bool ShouldHoverChampion(string[] currentChampSelect)
        {
            // Hover champion when champ select starts
            string champSelectPhase = currentChampSelect[1].Split("\"phase\":\"")[1].Split('"')[0];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return (currentTime - Settings.pickStartHoverDelay) > champSelectStart
                   || champSelectPhase != "PLANNING"
                   || Settings.instantHover;
        }

        private static void handlePickAction(string actId, string championId, string ActIsInProgress, string[] currentChampSelect, bool usePrimaryChamp)
        {
            // Check if the hover gets cleared (by either a ban or teammate taking it)
            if (championId == "0") pickedChamp = false;

            // If the mode is Arena
            if (isArena && !pickedChamp && !lockedChamp)
            {
                // If the player picked one of the crowd favorite champs, use that champ. Otherwise, if bravery is enabled, use that.
                if (Settings.crowdFavouraiteChamp1[1] == crowdFavorite1ChampId)
                    championId = crowdFavorite1ChampId;
                else if (Settings.crowdFavouraiteChamp2[1] == crowdFavorite2ChampId)
                    championId = crowdFavorite2ChampId;
                else if (Settings.crowdFavouraiteChamp3[1] == crowdFavorite3ChampId)
                    championId = crowdFavorite3ChampId;
                else if (Settings.crowdFavouraiteChamp4[1] == crowdFavorite4ChampId)
                    championId = crowdFavorite4ChampId;
                else if (Settings.crowdFavouraiteChamp5[1] == crowdFavorite5ChampId)
                    championId = crowdFavorite5ChampId;
                else if (Settings.bravery) championId = "-3";

                if (ShouldHoverChampion(currentChampSelect)) {
                    hoverChampion(actId, championId, "pick");
                    
                    // In arena mode runes and spells are disabled, so mark them as picked
                    pickedRunes = true;
                    pickedSpell1 = true;
                    pickedSpell2 = true;
                }
            }

            if (!pickedChamp)
            {
                if (ShouldHoverChampion(currentChampSelect)) {
                    // Try first choice based on player is assigned primary or secondary role
                    hoverChampion(actId, usePrimaryChamp ? Settings.currentChamp[1] : Settings.secondaryChamp[1], "pick");
                    handleRunes(usePrimaryChamp ? Settings.currentChampRunes[1] : Settings.secondaryChampRunes[1]);

                    // If first choice didn't work (pickedChamp is still false), try second choice
                    if (!pickedChamp)
                    {
                        hoverChampion(actId, usePrimaryChamp ? Settings.currentBackupChamp[1] : Settings.secondaryBackupChamp[1], "pick");
                        handleRunes(usePrimaryChamp ? Settings.currentBackupChampRunes[1] : Settings.secondaryBackupChampRunes[1]);
                    }
                }

            }

            if (ActIsInProgress == "true")
            {
                markPhaseStart(actId);

                if (!lockedChamp)
                {
                    // Check the instalock setting
                    if (!Settings.instaLock)
                    {
                        checkLockDelay(actId, championId, currentChampSelect, "pick");
                    }
                    else
                    {
                        lockChampion(actId, championId, "pick");
                    }
                }
            }
        }

        private static void handleBanAction(string actId, string championId, string ActIsInProgress, string[] currentChampSelect)
        {
            string champSelectPhase = currentChampSelect[1].Split("\"phase\":\"")[1].Split('"')[0];

            // make sure it's my turn to pick and that it is not the planning phase anymore
            if (ActIsInProgress == "true" && champSelectPhase != "PLANNING")
            {
                markPhaseStart(actId);

                if (!pickedBan)
                {
                    // Hover champion when champ select starts
                    long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    if ((currentTime - Settings.banStartHoverDelay) > champSelectStart) // Check if enough time has passed since planning phase has started
                    {
                        hoverChampion(actId, Settings.currentBan[1], "ban");
                    }
                }

                if (!lockedBan)
                {
                    // Check the instaBan setting
                    if (!Settings.instaBan)
                    {
                        checkLockDelay(actId, championId, currentChampSelect, "ban");
                    }
                    else
                    {
                        lockChampion(actId, championId, "ban");
                    }
                }
            }
        }

        private static void markPhaseStart(string actId)
        {
            if (actId != lastActId)
            {
                lastActId = actId;
                lastActStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

        private static void hoverChampion(string actId, string currentChamp, string actType)
        {
            string[] champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"championId\":" + currentChamp + "}");
            if (champSelectAction[0] == "204")
            {
                if (actType == "pick")
                {
                    pickedChamp = true;
                }
                else if (actType == "ban")
                {
                    pickedBan = true;
                }
            }
        }

        private static void handleRunes(string currentRunes)
        {
            string[] runeSelectAction = LCU.clientRequest("PUT", "lol-perks/v1/currentpage", currentRunes);
            if (runeSelectAction[0] == "201")
            {
                pickedRunes = true;
            }
        }

        private static void lockChampion(string actId, string championId, string actType)
        {
            string[] champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");
            if (champSelectAction[0] == "204")
            {
                if (actType == "pick")
                {
                    lockedChamp = true;
                }
                else if (actType == "ban")
                {
                    lockedBan = true;
                }
            }
        }

        private static void checkLockDelay(string actId, string championId, string[] currentChampSelect, string actType)
        {
            string timer = currentChampSelect[1].Split("totalTimeInPhase\":")[1].Split("}")[0];
            long timerInt = Convert.ToInt64(timer);
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            int delayPreEnd = 0;
            if (actType == "pick")
            {
                delayPreEnd = Settings.pickEndlockDelay;
            }
            else if (actType == "ban")
            {
                delayPreEnd = Settings.banEndlockDelay;
            }
            if (currentTime >= lastActStartTime + timerInt - delayPreEnd)
            {
                lockChampion(actId, championId, actType);
                return;
            }

            int delayAfterStart = 0;
            if (actType == "pick")
            {
                delayAfterStart = Settings.pickStartlockDelay;
            }
            else if (actType == "ban")
            {
                delayAfterStart = Settings.banStartlockDelay;
            }

            if (currentTime >= lastActStartTime + delayAfterStart)
            {
                lockChampion(actId, championId, actType);
            }
        }

        private static void handlePickOrderSwap()
        {
            // Return if we already locked in or if the settings is off
            if (!Settings.autoPickOrderTrade || lockedChamp)
            {
                return;
            }

            // Get ongoing swap data
            string[] swap = LCU.clientRequest("GET", "lol-champ-select/v1/ongoing-swap");
            if (swap[0] == "200")
            {
                // If the swap was called by local player, return
                if (swap.Contains("initiatedByLocalPlayer\":true"))
                {
                    return;
                }
                // Get action ID
                string swapId = swap[1].Split("\"id\":")[1].Split(',')[0];

                // Swap pick order
                LCU.clientRequest("POST", "lol-champ-select/v1/session/swaps/" + swapId + "/accept");
                LCU.clientRequest("POST", "lol-champ-select/v1/ongoing-swap/" + swapId + "/clear");
            }
        }
    }
}
