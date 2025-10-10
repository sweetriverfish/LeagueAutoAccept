using RestSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Leauge_Auto_Accept
{
    internal class MainLogic
    {
        private static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();

        public static bool isAutoAcceptOn = false;

        private static bool pickedChamp = false;
        private static bool lockedChamp = false;
        private static bool pickedBan = false;
        private static bool lockedBan = false;
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

        private static long champSelectStart;
        private static string lastChatRoom = "";

        private static long queueStartTime;
        private static string lastPhase = "";

        public static void acceptQueue()
        {
            while (true)
            {
                if (isAutoAcceptOn)
                {
                    var gameSessionResp = LCU.clientRequest<LCUTypes.LolGameflowSessionV1>("GET", "lol-gameflow/v1/session");

                    if (gameSessionResp.IsSuccessful)
                    {
                        var gameSession = gameSessionResp.Data;

                        Log.Info("Phase={0}", gameSession.Phase);

                        if (Settings.autoRestartQueue)
                        {
                            handleQueueRestart(gameSession.Phase);
                        }

                        switch (gameSession.Phase)
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

                        if (gameSession.Phase != "ChampSelect")
                        {
                            lastChatRoom = "";
                        }
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(10000);
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
            var currentChampSelect = LCU.clientRequest<LCUTypes.LolChampSelectSessionV1>("GET", "lol-champ-select/v1/session");

            if (currentChampSelect.IsSuccessStatusCode)
            {
                // Get needed data from the current champ select 
                string currentChatRoom = currentChampSelect.Data.ChatDetails.MultiUserChatId;// Content.Split("multiUserChatId\":\"")[1].Split('"')[0];
                if (lastChatRoom != currentChatRoom || lastChatRoom == "")
                {
                    // Reset stuff in case someone dodged the champ select
                    Log.Debug("Resetting variables due to chat room change. currentChatRoom={0} lastChatRoom={1}", currentChatRoom, lastChatRoom);

                    pickedChamp = false;
                    lockedChamp = false;
                    pickedBan = false;
                    lockedBan = false;
                    pickedSpell1 = false;
                    pickedSpell2 = false;
                    sentChatMessages = false;
                    champSelectStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    isArena = currentChampSelect.Data.QueueId == 1700; //1700=arena
                    crowdFavorite1ChampId = "";
                    crowdFavorite2ChampId = "";
                    crowdFavorite3ChampId = "";
                    crowdFavorite4ChampId = "";
                    crowdFavorite5ChampId = "";
                }
                lastChatRoom = currentChatRoom;

                Log.Debug("pickedChamp={0} lockedChamp={1} pickedBan={2} lockedBan={3} pickedSpell1={4} pickedSpell2={5}", pickedChamp, lockedChamp, pickedBan, lockedBan, pickedSpell1, pickedSpell2, sentChatMessages);
                if (pickedChamp && lockedChamp && pickedBan && lockedBan && pickedSpell1 && pickedSpell2 && sentChatMessages)
                {
                    // Sleep a little if we already did everything we needed to do
                    Log.Debug("Sleep 1000");
                    Thread.Sleep(1000);
                }
                else
                {
                    // Get more needed data from the current champ select
                    if (Settings.currentChamp[1] == "0" && !isArena)
                    {
                        pickedChamp = true;
                        lockedChamp = true;
                    }
                    if (Settings.currentBan[1] == "0")
                    {
                        pickedBan = true;
                        lockedBan = true;
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
                        // It's misspelled in the endpoint as "favorte" instead of "favorite"
                        var arenaCrowdFavorites = LCU.clientRequest("GET", "lol-lobby-team-builder/champ-select/v1/crowd-favorte-champion-list");
                        if (arenaCrowdFavorites.IsSuccessStatusCode)
                        {
                            string arenaCrowdFavoritesData = arenaCrowdFavorites.Content.Replace("[", "").Replace("]", "").Replace("\n", "").Replace(" ", "");
                            string[] arenaCrowdFavoritesSplit = arenaCrowdFavoritesData.Split(',');

                            // Might be less than 5 champs if the player doesn't own that many champs
                            if (arenaCrowdFavoritesSplit.Length > 0) crowdFavorite1ChampId = arenaCrowdFavoritesSplit[0];
                            if (arenaCrowdFavoritesSplit.Length > 1) crowdFavorite2ChampId = arenaCrowdFavoritesSplit[1];
                            if (arenaCrowdFavoritesSplit.Length > 2) crowdFavorite3ChampId = arenaCrowdFavoritesSplit[2];
                            if (arenaCrowdFavoritesSplit.Length > 3) crowdFavorite4ChampId = arenaCrowdFavoritesSplit[3];
                            if (arenaCrowdFavoritesSplit.Length > 4) crowdFavorite5ChampId = arenaCrowdFavoritesSplit[4];
                        }
                    }
                    
                    if (isArena || !pickedChamp || !lockedChamp || !pickedBan || !lockedBan)
                    {
                        handleChampSelectActions(currentChampSelect, currentChampSelect.Data.LocalPlayerCellId);
                    }
                    if (!sentChatMessages)
                    {
                        handleChampSelectChat(currentChatRoom);
                    }
                    if (!pickedSpell1)
                    {
                        var champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/my-selection", new { spellId = int.Parse(Settings.currentSpell1[1]) });
                        if (champSelectAction.IsSuccessStatusCode)
                        {
                            pickedSpell1 = true;
                        }
                    }
                    if (!pickedSpell2)
                    {
                        var champSelectAction = LCU.clientRequest("PATCH", "lol-champ-select/v1/session/my-selection", new { spellId = int.Parse(Settings.currentSpell2[1]) });
                        if (champSelectAction.IsSuccessStatusCode)
                        {
                            pickedSpell2 = true;
                        }
                    }
                }
            }
        }


        // Check player's assigned position and adjust champion pick to primary (true) or secondary (false)
        private static bool handleChampPositionPreferences(RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect, int localPlayerCellId)
        {
            // Check lobby endpoint for position preferences
            var lobbySession = LCU.clientRequest("GET", "lol-lobby/v2/lobby");
            if (!lobbySession.Content.Contains("firstPositionPreference\":") || !lobbySession.Content.Contains("secondPositionPreference\":")) return true;

            string firstPositionPreference = lobbySession.Content.Split("firstPositionPreference\":")[1].Split(',')[0].Trim('"').ToLower();
            string secondPositionPreference = lobbySession.Content.Split("secondPositionPreference\":")[1].Split(',')[0].Trim('"').ToLower();

            // Locate the "myTeam":[{ ... }] block
            int startIndex = currentChampSelect.Content.IndexOf("\"myTeam\":[");
            if (startIndex == -1) return true;

            int teamDataStart = currentChampSelect.Content.IndexOf("[{", startIndex);
            int teamDataEnd = currentChampSelect.Content.IndexOf("}]", teamDataStart);
            if (teamDataStart == -1 || teamDataEnd == -1) return true;
            
            string myTeam = currentChampSelect.Content.Substring(teamDataStart + 1, teamDataEnd - teamDataStart);

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
            var chats = LCU.clientRequest("GET", "lol-chat/v1/conversations", "");
            if (chats.Content.Contains(chatId))
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
                RestResponse httpRes;
                do
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string body = "{\"type\":\"chat\",\"fromId\":\"" + Data.currentChatId + "\",\"fromSummonerId\":" + Data.currentSummonerId + ",\"isHistorical\":false,\"timestamp\":\"" + timestamp + "\",\"body\":\"" + message + "\"}";
                    httpRes = LCU.clientRequest("POST", "lol-chat/v1/conversations/" + chatId + "/messages", body);

                    if (httpRes.IsSuccessful == false)
                    {
                        attempts++;
                        Thread.Sleep(attempts * 20);
                    }

                } while (httpRes.IsSuccessful == false && attempts < 3);

			}
            sentChatMessages = true;
        }

        private static void handleChampSelectActions(RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect, int localPlayerCellId)
        {
            // This logic skips modes that aren't draft
            if (currentChampSelect.Data.Actions.Count == 0) return;

            foreach (var act in currentChampSelect.Data.Actions.SelectMany(list=>list.AsArray()))
            {
                int ActCctorCellId = act["actorCellId"].GetValue<int>();
                bool ActCompleted = act["completed"].GetValue<bool>();
                string ActType = act["type"].GetValue<string>();
                int championId = act["championId"].GetValue<int>();
                int actId = act["id"].GetValue<int>();
                bool ActIsInProgress = act["isInProgress"].GetValue<bool>();

                if (ActCctorCellId == localPlayerCellId && ActCompleted == false && ActType == "pick")
                {
                    bool usePrimaryChamp = handleChampPositionPreferences(currentChampSelect, currentChampSelect.Data.LocalPlayerCellId);
                    handlePickAction(actId, championId, ActIsInProgress, currentChampSelect, usePrimaryChamp);
                }
                else if (ActCctorCellId == localPlayerCellId && ActCompleted == false && ActType == "ban")
                {
                    handleBanAction(actId, championId, ActIsInProgress, currentChampSelect);
                }
            }
        }

        private static bool ShouldHoverChampion(RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect)
        {
            // Hover champion when champ select starts
            string champSelectPhase = currentChampSelect.Content.Split("\"phase\":\"")[1].Split('"')[0];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return (currentTime - Settings.pickStartHoverDelay) > champSelectStart
                   || champSelectPhase != "PLANNING"
                   || Settings.instantHover;
        }

        private static bool isInCrowdFavoriteChamps(string champId) =>
            champId == crowdFavorite1ChampId ||
            champId == crowdFavorite2ChampId ||
            champId == crowdFavorite3ChampId ||
            champId == crowdFavorite4ChampId ||
            champId == crowdFavorite5ChampId;

        private static void handlePickAction(int actId, int championId, bool ActIsInProgress, RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect, bool usePrimaryChamp)
        {
            // Check if the hover gets cleared (by either a ban or teammate taking it)
            if (championId == 0) pickedChamp = false;

            if (!pickedChamp && ShouldHoverChampion(currentChampSelect))
            {
                if (isArena)
                {
                    string[] favorites = {
                        Settings.crowdFavouraiteChamp1[1],
                        Settings.crowdFavouraiteChamp2[1],
                        Settings.crowdFavouraiteChamp3[1],
                        Settings.crowdFavouraiteChamp4[1],
                        Settings.crowdFavouraiteChamp5[1]
                    };

                    foreach (var favIdStr in favorites)
                    {
                        if (!pickedChamp && isInCrowdFavoriteChamps(favIdStr))
                        {
                            int favId = int.Parse(favIdStr);
                            hoverChampion(actId, favId, "pick");
                            if (pickedChamp) championId = favId; // assign only if hover succeeded
                        }
                    }

                    // In arena mode runes and spells are disabled, so mark them as picked
                    pickedSpell1 = pickedSpell2 = true;
                }

                if (!pickedChamp && championId != -3)  //TODO: -3 is what???
                {
                    int primaryChampId = int.Parse(usePrimaryChamp ? Settings.currentChamp[1] : Settings.secondaryChamp[1]);
                    int primaryRunesId = int.Parse(usePrimaryChamp ? Settings.currentChampRunes[1] : Settings.secondaryChampRunes[1]);
                    int backupChampId = int.Parse(usePrimaryChamp ? Settings.currentBackupChamp[1] : Settings.secondaryBackupChamp[1]);
                    int backupRunesId = int.Parse(usePrimaryChamp ? Settings.currentBackupChampRunes[1] : Settings.secondaryBackupChampRunes[1]);

                    // Try first choice based on player is assigned primary or secondary role
                    hoverChampion(actId, primaryChampId, "pick");
                    handleRunes(primaryRunesId);

                    // If first choice didn't work (pickedChamp is still false), try second choice
                    if (!pickedChamp)
                    {
                        hoverChampion(actId, backupChampId, "pick");
                        handleRunes(backupRunesId);
                        if (pickedChamp) championId = backupChampId;
                    } else championId = primaryChampId;
                }
            }

            if (ActIsInProgress == true)
            {
                if (Log.IsDebugEnabled)
                    Log.Debug($"ActIsInProgress: {ActIsInProgress} | pickedChamp: {pickedChamp}, lockedChamp: {lockedChamp}, championId: {championId}");

                if (isArena && Settings.bravery)
                {
                    if (!pickedChamp)
                    {
                        hoverChampion(actId, -3, "pick");
                        if (pickedChamp) championId = -3;
                    }

                    lockedChamp = false;
                }

                if (!lockedChamp)
                {
                    if (Settings.instaLock) 
                        lockChampion(actId, championId, "pick");
                    else 
                        checkLockDelay(actId, championId, currentChampSelect, "pick");
                }
            }
        }

        private static void handleBanAction(int actId, int championId, bool ActIsInProgress, RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect)
        {
            string champSelectPhase = currentChampSelect.Data.Timer.Phase;

            // make sure it's my turn to pick and that it is not the planning phase anymore
            if (ActIsInProgress == true && champSelectPhase != "PLANNING")
            {

                if (!pickedBan)
                {
                    // Hover champion when champ select starts
                    long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    if (currentTime - Settings.banStartHoverDelay > champSelectStart) // Check if enough time has passed since planning phase has started
                    {
                        // Ban none if the setting is disabled.
                        bool dontBanCrowd = isArena && Settings.banCrowdFavourite && isInCrowdFavoriteChamps(Settings.currentBan[1]);
                        hoverChampion(actId, int.Parse(dontBanCrowd ? "0" : Settings.currentBan[1]), "ban");
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

        private static void hoverChampion(int actId, int currentChampId, string actType)
        {
            var champSelectAction = LCU.clientRequest("PATCH", $"lol-champ-select/v1/session/actions/{actId}", new { championId = currentChampId });
            if (champSelectAction.IsSuccessStatusCode)
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

        private static void handleRunes(int currentRunesId)
        {
            LCU.clientRequest("PUT", "lol-perks/v1/currentpage", $"{currentRunesId}");
        }

        private static void lockChampion(int actId, int championId, string actType)
        {
            var champSelectAction = LCU.clientRequest("PATCH", $"lol-champ-select/v1/session/actions/{actId}", "{'completed':true,'championId':" + championId + "}");
            if (champSelectAction.IsSuccessStatusCode)
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

        private static void checkLockDelay(int actId, int championId, RestResponse<LCUTypes.LolChampSelectSessionV1> currentChampSelect, string actType)
        {
            long totalTime = currentChampSelect.Data.Timer.TotalTimeInPhase; // Convert.ToInt64(currentChampSelect.Content.Split("totalTimeInPhase\":")[1].Split(",")[0].Split("}")[0]);
            long remaining = currentChampSelect.Data.Timer.AdjustedTimeLeftInPhase; // Convert.ToInt64(currentChampSelect.Content.Split("adjustedTimeLeftInPhase\":")[1].Split(",")[0].Split("}")[0]);
            long elapsed = totalTime - remaining;

            int startDelay = actType == "pick" ? Settings.pickStartlockDelay : Settings.banStartlockDelay;
            int endDelay = actType == "pick" ? Settings.pickEndlockDelay : Settings.banEndlockDelay;

            if (remaining <= endDelay || elapsed >= startDelay)
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
            var swapResp = LCU.clientRequest("GET", "lol-champ-select/v1/ongoing-swap");
            if (swapResp.IsSuccessStatusCode)
            {
                // If the swap was called by local player, return
                if (swapResp.Content.Contains("initiatedByLocalPlayer\":true"))
                {
                    return;
                }
                // Get action ID
                string swapId = swapResp.Content.Split("\"id\":")[1].Split(',')[0];

                // Swap pick order
                LCU.clientRequest("POST", "lol-champ-select/v1/session/swaps/" + swapId + "/accept");
                LCU.clientRequest("POST", "lol-champ-select/v1/ongoing-swap/" + swapId + "/clear");
            }
        }
    }
}
