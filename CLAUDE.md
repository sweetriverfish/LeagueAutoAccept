# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LeagueAutoAccept is a C# .NET 9 console application that automates League of Legends queue acceptance and champion selection using the League Client Update (LCU) API. It runs alongside the League client and communicates with it via its local HTTP API.

**Note:** The project directory has a typo — it is named `Leauge Auto Accept` (not "League").

## Build & Run

```bash
# Build
dotnet build "Leauge Auto Accept.sln"

# Build release
dotnet build -c Release "Leauge Auto Accept.sln"

# Publish (produces single executable)
dotnet publish -c Release

# Run (requires League of Legends client to be open)
dotnet run --project "Leauge Auto Accept/Leauge Auto Accept.csproj"
```

There are no automated tests in this project — testing is manual with a running League client.

## Architecture

The app is structured as cooperating static classes running concurrent long-lived tasks:

**Program.cs** — Entry point. Spawns 4 concurrent `Task`s and blocks with `Task.WaitAll()`:
1. `Navigation.ReadKeys()` — keyboard input loop
2. `MainLogic.acceptQueue()` — core game state polling loop
3. `LCU.CheckIfLeagueClientIsOpenTask()` — monitors if League client is running
4. `SizeHandler.SizeReader()` — enforces minimum console window size (120×30)

**LCU.cs** — All communication with the League Client Update API:
- Discovers the client's port and auth token by reading the `LeagueClientUx` process command-line arguments via `System.Management`
- Uses RestSharp with Basic Auth and disabled SSL validation (localhost only)
- `clientRequestUntilSuccess()` retries requests until the client responds
- Static `RestClient` is reused across calls

**MainLogic.cs** — Core automation logic:
- Polls the LCU API in a loop, checking `LolGameflowSessionV1` phase
- Phases handled: `Lobby`, `Matchmaking`, `ReadyCheck`, `ChampSelect`, `InProgress`, `EndOfGame`, etc.
- Tracks boolean state flags to avoid duplicate actions (e.g., `championPicked`, `championBanned`)
- Arena game mode has separate handling with crowd-favorite champion tracking
- Supports pick order detection and automatic position-based champion swapping

**Settings.cs** — All user configuration as static properties. Persisted across sessions.

**Data.cs** — Loads and caches champion inventory, summoner spells, and rune pages from the LCU API.

**UI.cs** — Console menu system with grid layouts. Menus for champion picker, spell picker, rune selector, arena mode, delays, and settings.

**Navigation.cs** — Reads `ConsoleKey` input in a dedicated loop; drives UI menu navigation and text entry.

**LCUTypes/** — C# records that map to LCU API JSON responses (used for deserialization with `System.Text.Json` or similar).

**Updater.cs** — Checks GitHub Releases API for new versions and shows update notifications.

## Key Conventions

- All major classes are `static` with public static methods and properties
- Game state is polled with `Thread.Sleep()` delays — no event-driven model
- `ParentProcessUtilities.cs` uses P/Invoke into `ntdll.dll` for Windows-specific parent process detection
- Logging is via NLog; `NLog.config` writes to `logs/` directory with daily rotation. A disabled JSON log target (`logs/json_out.log`) can be enabled to capture raw LCU API responses for debugging
- The console is launched under `conhost.exe` (set in Program.cs) to support window resizing on Windows
