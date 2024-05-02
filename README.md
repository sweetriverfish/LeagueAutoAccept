# Automatic queue accepter for League of Legends
Ever wanted to go pee or grab something but you're already 10 minutes into the queue? LeagueAutoAccept is a C# console app made using the LCU API to automatically accept queue and more!

## Screenshot
![LeagueAutoAccept Screenshot](screenshot.png?raw=true)

## Warnings/Disclaimer
- Be aware that the use of the LCU API is not allowed on the Korean server (which this application does)
- This application is not endorsed nor approved by Riot
- It's basically a gray area but should be fine to use on servers other than Korea

## Features
- Automatically accept queue
- Pick a champion
- Ban a champion
- Can instalock
- Pick summoner spells
- Send a chat message when entering lobby

## Planned
- Backup champion to pick/ban
- Automatic runes picker (from the premade sets)
- Manual runes creator, maybe

## Notes and stuff
- You can build your rune page via collection>runes. The last rune page you clicked is the currently selected one.
- Mac os is (currently) not supported
- Feel free to suggest stuff
- If it looks like I don't know what I'm doing that's probably because I don't know what I'm doing
- If the application fails to launch, chances are you don't have .NET Runtime installed. You can find the latest version at https://dotnet.microsoft.com/en-us/download/dotnet/7.0

## License
Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
