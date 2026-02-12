# LobotomyAgentCTRL2

로보토미 코퍼레이션 부대 지정 모드(`refracta_AgentCTRL2_MOD`) 소스 및 자동 빌드/릴리즈 레포입니다.

## 1. 소스

- 핵심 패치 코드: `src/refracta_AgentCTRL2_MOD/Harmony_Patch.cs`
- 어셈블리 정보: `src/refracta_AgentCTRL2_MOD/AssemblyInfo.cs`
- 모드 정보 XML: `mod/refracta_AgentCTRL2_MOD/Info/kr/Info.xml`
- 프로젝트 파일: `src/refracta_AgentCTRL2_MOD/refracta_AgentCTRL2_MOD.csproj`

## 2. 빌드 방법

### 요구 사항

- .NET SDK 8 이상
- PowerShell

### 로컬 빌드

```powershell
./scripts/build.ps1 -Configuration Release
```

### 결과물

- DLL: `out/Release/refracta_AgentCTRL2_MOD.dll`
- 배포 ZIP: `dist/refracta_AgentCTRL2_MOD.zip`

ZIP 내부 구조:

- `refracta_AgentCTRL2_MOD/refracta_AgentCTRL2_MOD.dll`
- `refracta_AgentCTRL2_MOD/Info/kr/Info.xml`

## 3. GitHub Actions 자동 릴리즈

- 워크플로 파일: `.github/workflows/upload-build.yml`
- 트리거:
  - 모든 브랜치 push 시 빌드 + artifact 업로드
  - `main` 브랜치 push 시 GitHub Release 자동 생성
- 릴리즈 에셋 파일명:
  - `refracta_AgentCTRL2_MOD.zip`
- 릴리즈 유지 정책:
  - 최신 3개만 유지 (오래된 릴리즈 자동 정리)

## Credits

- NexusMods: https://www.nexusmods.com/lobotomycorporation/mods/109
- DCInside 글: https://gall.dcinside.com/mgallery/board/view/?id=lobotomycorporation&no=99609
