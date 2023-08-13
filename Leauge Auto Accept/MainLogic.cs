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
        private static bool pickedSpell1 = false;
        private static bool pickedSpell2 = false;
        private static bool sentChatMessages = false;

        private static long lastActStartTime;
        private static long champSelectStart;
        private static string lastActId = "";
        private static string lastChatRoom = "";

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

                        switch (phase)
                        {
                            case "Lobby":
                                Thread.Sleep(5000);
                                break;
                            case "Matchmaking":
                                Thread.Sleep(2000);
                                break;
                            case "ReadyCheck":
                                LCU.clientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
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
                    pickedSpell1 = false;
                    pickedSpell2 = false;
                    sentChatMessages = false;
                    champSelectStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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

        private static void handleChampSelectChat(string chatId)
        {
            Data.loadPlayerChatId();

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            foreach (var message in Settings.chatMessages)
            {
                int attempts = 0;
                string httpRes = "";
                while (httpRes != "200" && attempts < 3)
                {
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
                    handlePickAction(actId, championId, ActIsInProgress, currentChampSelect);
                }
                else if (ActCctorCellId == localPlayerCellId && ActCompleted == "false" && ActType == "ban")
                {
                    handleBanAction(actId, championId, ActIsInProgress, currentChampSelect);
                }
            }
        }

        private static void handlePickAction(string actId, string championId, string ActIsInProgress, string[] currentChampSelect)
        {
            if (!pickedChamp)
            {
                // Hover champion when champ select starts
                string champSelectPhase = currentChampSelect[1].Split("\"phase\":\"")[1].Split('"')[0];
                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if ((currentTime - 10000) > champSelectStart // Check if enough time has passed since planning phase has started
                    || champSelectPhase != "PLANNING" // Check if it's even planning phase at all
                    || Settings.instantHover) // Check if instahover setting is on
                {
                    hoverChampion(actId, Settings.currentChamp[1], "pick");
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
                    hoverChampion(actId, Settings.currentBan[1], "ban");
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
            if (currentTime >= lastActStartTime + timerInt - Settings.lockDelay)
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
