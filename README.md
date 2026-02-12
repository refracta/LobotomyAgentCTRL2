# LobotomyAgentCTRL2

This repository contains the source code for `refracta_AgentCTRL2_MOD`, a hotkey mod for managing agents in Lobotomy Corporation.
It is a modified version of decompiled code from abcdcode's original AgentCTRL mod.

## Demo

![Demo](assets/demo.gif)

## Quick Usage

- Select agents, then press `Ctrl + F2` to `Ctrl + F12` to bind the selected agents to a slot.
- Press `F2` to `F12` to select the agents bound to that slot.
- Press `Shift + F2` to `Shift + F12` to print the saved agent-name list in the in-game system log.
- Bound slots are persisted between game sessions.

## 1. Source

- Core patch logic: `src/refracta_AgentCTRL2_MOD/Harmony_Patch.cs`
- Assembly metadata: `src/refracta_AgentCTRL2_MOD/AssemblyInfo.cs`
- Mod info XML: `mod/refracta_AgentCTRL2_MOD/Info/kr/Info.xml`, `mod/refracta_AgentCTRL2_MOD/Info/en/Info.xml`
- Project file: `src/refracta_AgentCTRL2_MOD/refracta_AgentCTRL2_MOD.csproj`

## 2. Build

### Requirements

- .NET SDK 8+
- PowerShell

### Local build

```powershell
./scripts/build.ps1 -Configuration Release
```

### Output

- DLL: `out/Release/refracta_AgentCTRL2_MOD.dll`
- ZIP package: `dist/refracta_AgentCTRL2_MOD.zip`

ZIP layout:

- `refracta_AgentCTRL2_MOD/refracta_AgentCTRL2_MOD.dll`
- `refracta_AgentCTRL2_MOD/Info/kr/Info.xml`
- `refracta_AgentCTRL2_MOD/Info/en/Info.xml`

## Credits

- NexusMods: https://www.nexusmods.com/lobotomycorporation/mods/109
- DCInside post: https://gall.dcinside.com/mgallery/board/view/?id=lobotomycorporation&no=99609
