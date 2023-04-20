using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Leauge_Auto_Accept
{
    internal class Settings
    {
        public static string[] currentChamp = { "None", "0" };
        public static string[] currentBan = { "None", "0" };
        public static string[] currentSpell1 = { "None", "0" };
        public static string[] currentSpell2 = { "None", "0" };
        public static bool chatMessagesEnabled = false;
        public static List<string> chatMessages = new List<string>();

        public static bool saveSettings = false;
        public static bool preloadData = false;
        public static bool instaLock = false;
        public static int lockDelay = 1500;
        public static string lockDelayString = "1500";
        public static bool disableUpdateCheck = false;
        public static bool autoPickOrderTrade = false;
        public static bool shouldAutoAcceptbeOn = false;

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
                    if (lockDelayString.Length == 0)
                    {
                        lockDelayString = "0";
                    }
                    else
                    {
                        lockDelayString = lockDelayString.TrimStart('0');
                    }
                    lockDelay = Int32.Parse(lockDelayString);
                    if (lockDelay < 500)
                    {
                        lockDelay = 500;
                    }
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

        public static void saveSelectedChamp()
        {
            List<itemList> champsFiltered = new List<itemList>();
            if ("none".Contains(Navigation.currentInput.ToLower()))
            {
                champsFiltered.Add(new itemList() { name = "None", id = "0" });
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
                    name = "None";
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
            if ("none".Contains(Navigation.currentInput.ToLower()))
            {
                spellsFiltered.Add(new itemList() { name = "None", id = "0" });
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
                    name = "None";
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
                ",lockDelay:" + lockDelay +
                ",autoPickOrderTrade:" + autoPickOrderTrade +
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
                        case "lockDelay":
                            lockDelay = Int32.Parse(columns[1]);
                            lockDelayString = columns[1];
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
                        case "disableUpdateCheck":
                            disableUpdateCheck = Boolean.Parse(columns[1]);
                            break;
                        case "autoPickOrderTrade":
                            autoPickOrderTrade = Boolean.Parse(columns[1]);
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
