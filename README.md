# LobotomyAgentCTRL2

Source repository for the Lobotomy Corporation squad-hotkey mod: `refracta_AgentCTRL2_MOD`.

## Demo

![Demo](assets/demo.gif)

## Quick Usage

- Select agents, then press `Ctrl + F2` to `Ctrl + F12` to save a squad to a slot.
- Press `F2` to `F12` to re-select the saved squad in that slot.
- Press `Shift + F2` to `Shift + F12` to print the saved agent-name list for that slot.
- Saved slots persist between game sessions.

## 1. Source

- Core patch logic: `src/refracta_AgentCTRL2_MOD/Harmony_Patch.cs`
- Assembly metadata: `src/refracta_AgentCTRL2_MOD/AssemblyInfo.cs`
- Mod info XML: `mod/refracta_AgentCTRL2_MOD/Info/kr/Info.xml`
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

## Credits

- NexusMods: https://www.nexusmods.com/lobotomycorporation/mods/109
- DCInside post: https://gall.dcinside.com/mgallery/board/view/?id=lobotomycorporation&no=99609
