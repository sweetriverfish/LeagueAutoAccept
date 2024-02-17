using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Leauge_Auto_Accept
{
    internal class Settings
    {
        public static string[] currentChamp = { "Unselected", "0" };
        public static string[] currentBan = { "Unselected", "0" };
        public static string[] currentSpell1 = { "Unselected", "0" };
        public static string[] currentSpell2 = { "Unselected", "0" };
        public static bool chatMessagesEnabled = false;
        public static List<string> chatMessages = new List<string>();

        public static bool saveSettings = false;
        public static bool preloadData = false;
        public static bool instaLock = false;
        public static bool instaBan = false;
        public static bool disableUpdateCheck = false;
        public static bool autoPickOrderTrade = false;
        public static bool instantHover = false;
        public static bool shouldAutoAcceptbeOn = false;
        public static bool autoRestartQueue = false;

        public static int pickStartHoverDelay = 10000;
        public static int pickStartlockDelay = 999999999;
        public static int pickEndlockDelay = 1000;
        public static int banStartHoverDelay = 1500;
        public static int banStartlockDelay = 999999999;
        public static int banEndlockDelay = 1000;
        public static int queueMaxTime = 300000;
        public static int chatMessagesDelay = 100;

        public static void settingsModify(int item)
        {
            switch (item)
            {
                case 0:
                    if (saveSettings && preloadData)
                    {
                        preloadData = !preloadData;
                        UI.settingsMenuUpdateUI(1);
                    }
                    if (saveSettings && disableUpdateCheck)
                    {
                        disableUpdateCheck = !disableUpdateCheck;
                        UI.settingsMenuUpdateUI(4);
                    }
                    saveSettings = !saveSettings;
                    break;
                case 1:
                    if (!preloadData && !saveSettings)
                    {
                        saveSettings = !saveSettings;
                        UI.settingsMenuUpdateUI(0);
                    }
                    preloadData = !preloadData;
                    break;
                case 2:
                    instaLock = !instaLock;
                    break;
                case 3:
                    instaBan = !instaBan;
                    break;
                case 4:
                    if (!disableUpdateCheck && !saveSettings)
                    {
                        saveSettings = !saveSettings;
                        UI.settingsMenuUpdateUI(0);
                    }
                    disableUpdateCheck = !disableUpdateCheck;
                    break;
                case 5:
                    autoPickOrderTrade = !autoPickOrderTrade;
                    break;
                case 6:
                    instantHover = !instantHover;
                    break;
                case 7:
                    autoRestartQueue = !autoRestartQueue;
                    break;
                case 8:
                    UI.delayMenu();
                    break;
            }

            if (saveSettings)
            {
                settingsSave();
            }
            else if (item == 0)
            {
                deleteSettings();
            }
        }

        public static void delayModify(int item, int number)
        {
            switch (item)
            {
                case 0:
                    {
                        int newNum = delayCalculateNewValue(pickStartHoverDelay, number);
                        pickStartHoverDelay = newNum;
                    }
                    break;
                case 1:
                    {
                        int newNum = delayCalculateNewValue(pickStartlockDelay, number);
                        pickStartlockDelay = newNum;
                    }
                    break;
                case 2:
                    {
                        int newNum = delayCalculateNewValue(pickEndlockDelay, number);
                        pickEndlockDelay = newNum;
                    }
                    break;
                case 3:
                    {
                        int newNum = delayCalculateNewValue(banStartHoverDelay, number);
                        banStartHoverDelay = newNum;
                    }
                    break;
                case 4:
                    {
                        int newNum = delayCalculateNewValue(banStartlockDelay, number);
                        banStartlockDelay = newNum;
                    }
                    break;
                case 5:
                    {
                        int newNum = delayCalculateNewValue(banEndlockDelay, number);
                        banEndlockDelay = newNum;
                    }
                    break;
                case 6:
                    {
                        int newNum = delayCalculateNewValue(queueMaxTime, number);
                        queueMaxTime = newNum;
                    }
                    break;
                case 7:
                    {
                        int newNum = delayCalculateNewValue(chatMessagesDelay, number);
                        chatMessagesDelay = newNum;
                    }
                    break;
            }

            if (saveSettings)
            {
                settingsSave();
            }
        }

        public static int delayCalculateNewValue(int oldValue, int modifier)
        {
            string newNumString = oldValue.ToString();

            if (modifier >= 0)
            {
                newNumString = newNumString + modifier.ToString();
                if (newNumString.Length > 9)
                {
                    newNumString = "999999999";
                }
            }
            else
            {
                newNumString = newNumString.Substring(0, newNumString.Length - 1);
                if (newNumString.Length == 0)
                {
                    newNumString = "0";
                }
            }

            int newNum = Int32.Parse(newNumString);

            return newNum;
        }

        public static void saveSelectedChamp()
        {
            List<itemList> champsFiltered = new List<itemList>();
            if ("unselected".Contains(Navigation.currentInput.ToLower()))
            {
                champsFiltered.Add(new itemList() { name = "Unselected", id = "0" });
            }
            if (UI.currentChampPicker == 1)
            {
                if ("none".Contains(Navigation.currentInput.ToLower()))
                {
                    champsFiltered.Add(new itemList() { name = "None", id = "-1" });
                }
            }
            foreach (var champ in Data.champsSorterd)
            {
                if (champ.name.ToLower().Contains(Navigation.currentInput.ToLower()))
                {
                    if (UI.currentChampPicker == 0)
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
                if (Navigation.currentPos < 0)
                {
                    name = "Unselected";
                    id = "0";
                }
                else
                {
                    name = champsFiltered[Navigation.currentPos].name;
                    id = champsFiltered[Navigation.currentPos].id;
                }
                if (UI.currentChampPicker == 0)
                {
                    currentChamp[0] = name;
                    currentChamp[1] = id;
                }
                else
                {
                    currentBan[0] = name;
                    currentBan[1] = id;
                }
                if (saveSettings)
                {
                    settingsSave();
                }
            }
        }

        public static void saveSelectedSpell()
        {
            List<itemList> spellsFiltered = new List<itemList>();
            if ("unselected".Contains(Navigation.currentInput.ToLower()))
            {
                spellsFiltered.Add(new itemList() { name = "Unselected", id = "0" });
            }
            foreach (var spell in Data.spellsSorted)
            {
                if (spell.name.ToLower().Contains(Navigation.currentInput.ToLower()))
                {
                    spellsFiltered.Add(new itemList() { name = spell.name, id = spell.id });
                }
            }

            if (spellsFiltered.Count > 0)
            {
                string name;
                string id;
                if (Navigation.currentPos < 0)
                {
                    name = "Unselected";
                    id = "0";
                }
                else
                {
                    name = spellsFiltered[Navigation.currentPos].name;
                    id = spellsFiltered[Navigation.currentPos].id;
                }
                if (UI.currentSpellSlot == 0)
                {
                    currentSpell1[0] = name;
                    currentSpell1[1] = id;
                }
                else
                {
                    currentSpell2[0] = name;
                    currentSpell2[1] = id;
                }
                if (saveSettings)
                {
                    settingsSave();
                }
            }
        }

        public static void updateChatMessage()
        {
            if (chatMessages.Count > UI.messageIndex)
            {
                chatMessages[UI.messageIndex] = Navigation.currentInput;
            }
            else
            {
                chatMessages.Add(Navigation.currentInput);
            }
            updateChatMessagesToggle();
            if (saveSettings)
            {
                settingsSave();
            }
        }

        public static void deleteChatMessage()
        {
            if (chatMessages.Count > UI.messageIndex)
            {
                chatMessages.RemoveAt(UI.messageIndex);
            }
            updateChatMessagesToggle();

            if (saveSettings)
            {
                settingsSave();
            }
        }

        private static void updateChatMessagesToggle()
        {
            if (chatMessages.Count > 0)
            {
                chatMessagesEnabled = true;
            }
            else
            {
                chatMessagesEnabled = false;
            }
        }

        private static string encodeMessagesIntoBase64()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(string.Join('|', chatMessages));
            string base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }

        private static void decodeMessagesFromBase64(string messages)
        {
            if (messages == "") { return; }
            byte[] byteArray = Convert.FromBase64String(messages);
            string joinedString = Encoding.UTF8.GetString(byteArray);
            chatMessages = new List<string>(joinedString.Split('|'));
        }

        public static void settingsSave()
        {
            string config =
                "champName:" + currentChamp[0] +
                ",champId:" + currentChamp[1] +
                ",banName:" + currentBan[0] +
                ",banId:" + currentBan[1] +
                ",spell1Name:" + currentSpell1[0] +
                ",spell1Id:" + currentSpell1[1] +
                ",spell2Name:" + currentSpell2[0] +
                ",spell2Id:" + currentSpell2[1] +
                ",autoAcceptOn:" + shouldAutoAcceptbeOn +
                ",preloadData:" + preloadData +
                ",instaLock:" + instaLock +
                ",instaBan:" + instaBan +
                ",pickStartHoverDelay:" + pickStartHoverDelay +
                ",pickStartlockDelay:" + pickStartlockDelay +
                ",pickEndlockDelay:" + pickEndlockDelay +
                ",banStartHoverDelay:" + banStartHoverDelay +
                ",banStartlockDelay:" + banStartlockDelay +
                ",banEndlockDelay:" + banEndlockDelay +
                ",queueMaxTime:" + queueMaxTime +
                ",chatMessagesDelay:" + chatMessagesDelay +
                ",autoPickOrderTrade:" + autoPickOrderTrade +
                ",instantHover:" + instantHover +
                ",autoRestartQueue:" + autoRestartQueue +
                ",disableUpdateCheck:" + disableUpdateCheck +
                ",chatMessages:" + encodeMessagesIntoBase64();

            string dirParameter = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Leauge Auto Accept Config.txt";
            using (StreamWriter m_WriterParameter = new StreamWriter(dirParameter, false))
            {
                m_WriterParameter.Write(config);
            }
        }

        public static void toggleAutoAcceptSetting()
        {
            if (MainLogic.isAutoAcceptOn)
            {
                MainLogic.isAutoAcceptOn = false;
                shouldAutoAcceptbeOn = false;
            }
            else
            {
                MainLogic.isAutoAcceptOn = true;
                shouldAutoAcceptbeOn = true;
            }
            if (saveSettings)
            {
                settingsSave();
            }
        }

        public static void deleteSettings()
        {
            string dirParameter = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Leauge Auto Accept Config.txt";
            File.Delete(dirParameter);
        }

        public static void loadSettings()
        {
            string dirParameter = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Leauge Auto Accept Config.txt";
            if (File.Exists(dirParameter))
            {
                string text = File.ReadAllText(dirParameter);
                string[] commas = text.Split(',');
                foreach (var comma in commas)
                {
                    string[] columns = comma.Split(':');
                    switch (columns[0])
                    {
                        case "champName":
                            currentChamp[0] = columns[1];
                            break;
                        case "champId":
                            currentChamp[1] = columns[1];
                            break;
                        case "banName":
                            currentBan[0] = columns[1];
                            break;
                        case "banId":
                            currentBan[1] = columns[1];
                            break;
                        case "spell1Name":
                            currentSpell1[0] = columns[1];
                            break;
                        case "spell1Id":
                            currentSpell1[1] = columns[1];
                            break;
                        case "spell2Name":
                            currentSpell2[0] = columns[1];
                            break;
                        case "spell2Id":
                            currentSpell2[1] = columns[1];
                            break;
                        case "pickStartHoverDelay":
                            pickStartHoverDelay = Int32.Parse(columns[1]);
                            break;
                        case "pickStartlockDelay":
                            pickStartlockDelay = Int32.Parse(columns[1]);
                            break;
                        case "pickEndlockDelay":
                            pickEndlockDelay = Int32.Parse(columns[1]);
                            break;
                        case "banStartHoverDelay":
                            banStartHoverDelay = Int32.Parse(columns[1]);
                            break;
                        case "banStartlockDelay":
                            banStartlockDelay = Int32.Parse(columns[1]);
                            break;
                        case "banEndlockDelay":
                            banEndlockDelay = Int32.Parse(columns[1]);
                            break;
                        case "queueMaxTime":
                            queueMaxTime = Int32.Parse(columns[1]);
                            break;
                        case "chatMessagesDelay":
                            chatMessagesDelay = Int32.Parse(columns[1]);
                            break;
                        case "autoAcceptOn":
                            shouldAutoAcceptbeOn = Boolean.Parse(columns[1]);
                            break;
                        case "preloadData":
                            preloadData = Boolean.Parse(columns[1]);
                            break;
                        case "instaLock":
                            instaLock = Boolean.Parse(columns[1]);
                            break;
                        case "instaBan":
                            instaBan = Boolean.Parse(columns[1]);
                            break;
                        case "disableUpdateCheck":
                            disableUpdateCheck = Boolean.Parse(columns[1]);
                            break;
                        case "autoPickOrderTrade":
                            autoPickOrderTrade = Boolean.Parse(columns[1]);
                            break;
                        case "instantHover":
                            instantHover = Boolean.Parse(columns[1]);
                            break;
                        case "autoRestartQueue":
                            autoRestartQueue = Boolean.Parse(columns[1]);
                            break;
                        case "chatMessages":
                            decodeMessagesFromBase64(columns[1]);
                            updateChatMessagesToggle();
                            break;
                    }
                    saveSettings = true;
                }
            }
        }
    }
}
