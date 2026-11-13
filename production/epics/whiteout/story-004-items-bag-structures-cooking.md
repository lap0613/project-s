# Story 004: 아이템 채집·가방·모닥불/은신처 설치·요리

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Core
> **Type**: Integration
> **Estimate**: L
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-13

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 3` (아이템 채집, 모닥불·은신처 설치, 요리 부분. 제작은 008)
**근거 문서**: `design/quick-specs/items.md` §1~3·7·8 · `design/quick-specs/core-rules.md` §2·3·4 · `docs/architecture/code-structure.md` §6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 3, scoped to this story, with the values from the 근거 문서:*

- [x] 아이템 정의는 ScriptableObject(`ResourceDefinition`)이고 새 아이템은 에셋 추가만으로 만든다 (code-structure §6)
- [x] 가방은 기본 6칸. 같은 아이템은 한 칸에 쌓임 수만큼 쌓인다(나무 5, 부싯깃 10, 열매 3, 날고기 3, 구운 고기 3, 약초 5). 가득 차면 채집되지 않고 알려준다 (items §1~3)
- [x] E키를 1초 누르면 채집된다. 채집하는 동안 이동할 수 없다 (core-rules §4)
- [x] 나무·부싯깃·열매·날고기·약초가 매 판 후보 지점 중 무작위로 배치된다(items §2·3의 개수). 한 번 쓰면 다시 생기지 않는다
- [x] 불 없이 시작한다. 시작 소지품: 부싯돌 1, 부싯깃 2, 나무 3, 열매 1. 시작 위치 근처에 첫날 분량의 나무·식량을 보장한다 (items §8)
- [x] B로 모닥불을 설치한다: 나무 3 + 부싯깃 1, 부싯돌이 가방에 있어야 한다. 설치 후 나무를 넣어야 켜지고, 나무 1개 = 60초 연소, 최대 5개, 개수 제한 없음 (core-rules §4)
- [x] 꺼진 모닥불은 부싯깃 1 + 나무 2로 다시 피운다(부싯돌 필요). 켜진 모닥불은 반경 4m 안에 +35°C를 환경 온도(002)에 전달하고, 겹치면 가장 따뜻한 1개만 적용된다 (core-rules §3·4)
- [x] B로 은신처를 설치한다: 나무 6, 반경 3m, 온기 없음, 개수 제한 없음. 안에서 E로 휴식하는 동안 체력 회복 조건이 체온·허기 30 이상으로 낮아지고 회복량이 3배(초당 +1.5)가 된다 (core-rules §2·4)
- [x] 소모품: 열매 허기 +15, 날고기 +10, 구운 고기 +40, 약초 체력 +20. 켜진 모닥불에서 날고기를 E 3초로 구우면 구운 고기가 된다 (items §3, core-rules §4). 허기 30 미만의 채집 속도 -30%(003)가 실제로 적용된다
- [x] `FireStateChanged`·`StructurePlaced`·`ResourceGathered` 이벤트가 발행되고, 가방과 모닥불 연료가 HUD에 보인다. 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 아이템·가방·채집·제작은 Resources 시스템, 모닥불·은신처는 Camp 시스템. 서로 참조하지 않고 Core 인터페이스와 이벤트로만 통신한다 (code-structure §3·4).
- 위치 판정(모닥불 반경, 은신처 안, 채집 거리)은 View가 하고 결과만 Logic에 넘긴다 (tech-decisions 13).
- 가방 칸 수는 스탯 수정자로 만든다. 005의 큰 배낭·썰매가 같은 방식으로 붙는다.
- 설치는 구조물 정의(데이터) 기반으로 만든다. 006의 구조 신호기를 기존 코드 수정 없이 추가할 수 있어야 한다 (code-structure §6).
- 천 조각·가죽·금속 조각·신호기 부품은 이 스토리에서 배치하지 않는다 (008, 006).
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 002: 환경 온도 합산 (이 스토리는 온기 값을 공급한다)
- 003: 허들 계산 자체
- 005: 가방 칸 증강
- 006: 구조 신호기와 부품, 금속 조각 배치
- 007: 늑대 고기·가죽 드롭
- 008: 도구·장비·제작·천 조각·가죽·금속

---

## Files

- `Assets/Scripts/Systems/Resources/**` (새 어셈블리 `Whiteout.Resources`, `Whiteout.Resources.Unity`: 아이템 정의, 가방, 채집, 배치, 요리, 사용·버리기)
- `Assets/Scripts/Systems/Camp/**` (새 어셈블리 `Whiteout.Camp`, `Whiteout.Camp.Unity`: 모닥불·은신처 설치, 연료, 재점화, 온기, 휴식)
- `Assets/Scripts/Core.Unity/Whiteout.Core.Unity.asmdef` (수정)
- `Assets/Scripts/Game/GameLifetimeScope.cs` (수정)
- `Assets/Scripts/Game/NoWarmthSource.cs` (삭제)
- `Assets/Scripts/Game/Whiteout.Game.asmdef` (수정)
- `Assets/Scripts/Systems/Survival/Logic/SurvivalSettings.cs` (수정)
- `Assets/Scripts/Systems/Survival/Logic/SurvivalSystem.cs` (수정)
- `Assets/Scripts/Systems/Survival/Unity/Data/SurvivalConfig.cs` (수정)
- `Assets/Scripts/Systems/Survival/Unity/View/SurvivalHud.cs` (수정)
- `Assets/Scripts/Systems/Survival/Unity/Whiteout.Survival.Unity.asmdef` (수정)
- `Assets/Scripts/UI/WeatherHud.cs` (수정)
- `Assets/Scripts/UI/Whiteout.UI.asmdef` (수정)
- `Assets/Scripts/Core.Unity/ItemIdAsset.cs` (새 파일)
- `Assets/Scripts/Core.Unity/UiTextTable.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/BagSlotInfo.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/Events/FireStateChanged.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/Events/NoticePosted.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/Events/ResourceGathered.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/Events/StructurePlaced.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IBag.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IBagAction.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IFireStatus.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IInteractionInput.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IInteractionSource.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IInteractionStatus.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IItemNames.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IPlacementMenu.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IPlayerPosition.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IRestTarget.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/IVitalsReceiver.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/InteractionPrompt.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/ItemAmount.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/ItemId.cs` (새 파일)
- `Assets/Scripts/Core/Contracts/VitalKind.cs` (새 파일)
- `Assets/Scripts/Game/InteractionDirector.cs` (새 파일)
- `Assets/Scripts/Game/PlayerActionLock.cs` (새 파일)
- `Assets/Scripts/Game/PlayerPositionSource.cs` (새 파일)
- `Assets/Scripts/Game/SurvivalGatherBridge.cs` (새 파일)
- `Assets/Scripts/Input/InputSystemInteractionInput.cs` (새 파일)
- `Assets/Scripts/UI/BagHud.cs` (새 파일)
- `Assets/Scripts/UI/BagSlotView.cs` (새 파일)
- `Assets/Scripts/UI/HudConfig.cs` (새 파일)
- `Assets/Scripts/UI/NotificationHud.cs` (새 파일)
- `Assets/Scripts/UI/PlacementHud.cs` (새 파일)
- `Assets/Scripts/UI/WorldUiHud.cs` (새 파일)
- `Assets/Data/Items/**`, `Assets/Data/Camp/**`, `Assets/Data/UI/**` (아이템·효과·구조물 정의, 설정, 문구 테이블 에셋)
- `Assets/Prefabs/Pickups/**`, `Assets/Prefabs/Structures/**`, `Assets/Prefabs/UI/**`, `Assets/Art/Placeholder/*.mat` (임시 프리팹·머티리얼)
- `Assets/Fonts/NotoSansKR/**`, `Assets/TextMesh Pro/**` (Noto Sans KR + OFL, TMP 폰트 에셋, TMP 기본 리소스)
- `Assets/Scenes/Game.unity` (HUD 재배치·TMP, 가방·알림·설치·월드 UI, EventSystem, ResourceSpawner + 후보 지점 170개, 장애물 레이어), `ProjectSettings/TagManager.asset` (`Obstacle` 레이어)
- `docs/architecture/tech-decisions.md` (26~28번), `docs/architecture/code-structure.md` (§4)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 설치한 모닥불과 HUD가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [x] Retained — `production/qa/evidence/story-004-items-bag-structures-cooking/` (screenshots 9, stamp.txt)

---

## Dependencies

- Depends on: `production/epics/whiteout/story-001-foundation-movement-map.md`, `production/epics/whiteout/story-002-time-weather-temperature.md`, `production/epics/whiteout/story-003-hurdles-death.md`
- Unlocks: `production/epics/whiteout/story-005-augments.md`, `production/epics/whiteout/story-006-radio-rescue-results.md`, `production/epics/whiteout/story-007-wolves-combat.md`, `production/epics/whiteout/story-008-tools-equipment-crafting.md`

## Completion Notes
**Completed**: 2026-11-13
**Criteria**: 10/10 passing. 에디터 Play Mode에서 입력 이벤트로 실행해 확인 (tech-decisions 25번). 조작감·밸런스는 지영 플레이 확인 필요.
**실행으로 확인한 것**: 시작 소지품과 6칸, 배치 개수(나무 50·부싯깃 10·열매 30·날고기 10·약초 6), E 1초 채집과 채집 중 이동 불가,
B 목록 → 모닥불·은신처 설치와 비용 차감, 나무 1개 점화, 기온 +35°C, 연소 후 재점화(부싯깃 1 + 나무 2), E 3초 요리, 칸 우클릭 사용(허기 +40)·버리기,
숫자키 칸 선택, 은신처 휴식(체력 +1.5/초, 허기 31~35에서), 허기 30 미만 채집 속도 -30%, 가방 가득 참 알림.
**실행으로 확인하지 못한 것**: 연료 최대 5개 제한, 온기 반경 4m 경계, 모닥불 2개가 겹칠 때 가장 따뜻한 1개만 적용, 열매·날고기·약초 사용 수치(데이터 값으로만 확인),
이벤트 3종 발행(코드와 HUD 반응으로만 확인).
**시험 조건**: 요리·은신처·가방 가득 참 확인에 필요한 재료는 가방에 직접 넣었고, 휴식·채집 속도는 체력·허기를 시험용 값으로 맞춰 쟀다.
**Deviations**:
- ADVISORY: UI 결정 D3~D5·D9·D10 반영으로 범위가 늘었다(가방 칸 조작, 설치 목록, 알림, 월드 UI, TextMeshPro 전환, 문구 데이터화, Noto Sans KR).
- ADVISORY: 상단 HUD 배치(기온 왼쪽 위, 예보 위쪽 가운데)는 중간 상태다. 최종 배치는 story-010 (tech-decisions 24번).
- ADVISORY: `Assets/Scripts/Systems/Resources/` 폴더 이름은 Unity 특수 폴더(`Resources`)라 그 안의 에셋이 모든 빌드에 들어간다. 지금은 스크립트뿐이라 영향이 없다.
  code-structure §2에 정해진 이름이라 바꾸지 않았다. 지영 결정 필요. 데이터 에셋은 `Assets/Data/Items/`에 두었다.
- ADVISORY: 스크린샷 03~09는 엔진 점검 수정 4건 전에 찍었다. 수정은 화면 모양을 바꾸지 않는다. 01·02는 수정 후 다시 찍었다.
- OUT OF SCOPE: `Assets/Scripts/Systems/Survival/**`(휴식·회복 받기 연결, HUD TextMeshPro 전환), `Assets/Scripts/Input/**`(E·B·숫자키 입력), `Assets/Scripts/Game/**`(조합 루트·브리지),
  `Assets/Fonts/**`, `Assets/TextMesh Pro/**`, `Assets/Prefabs/**`, `Assets/Art/Placeholder/**`, `Assets/Scenes/Game.unity`, `ProjectSettings/TagManager.asset`. 스토리의 Files 목록에 없던 경로다.
**Engine check** (unity-specialist): MUST 0. 수정함: 기온 글자를 표시 단위로만 갱신, 게이지 값이 바뀔 때만 갱신, 스코프 null 검사에 Player Config·Input Actions 추가,
HUD 위 좌클릭은 설치로 넘기지 않음. 남김: 휠 입력이 고해상도 휠에서 여러 칸 넘어갈 수 있음, 미리보기 머티리얼은 0번 슬롯만 바꿈, 모닥불이 매 프레임 켜짐 상태를 읽음,
월드 UI가 보이는 동안 HUD 캔버스를 매 프레임 다시 그림, 슬롯 우클릭을 누르는 순간 설치 목록이 닫힘.
**Test Evidence**: waived at qa.level minimal. 스크린샷 9장 + stamp.txt: `production/qa/evidence/story-004-items-bag-structures-cooking/`
**Run result**: OBSERVED — 켜진 모닥불과 연료 칸·남은 초, 기온 30°C, 가방 칸 줄이 보이는 플레이 화면 (`production/qa/evidence/story-004-items-bag-structures-cooking/04-campfire-lit-fuel-warmth.png`)
**Reviewed by**: self, no independent review

## Calls beyond the design
모두 임시값, 지영 확인 필요 (`docs/architecture/tech-decisions.md` 26~28번). PM 운영 규칙에 따라 임시로 유지하고 기록했다.
- UI 문구 형식: `UiTextTable` 에셋 1개에 키 → 문구, 알림은 `NoticePosted` 이벤트 (`Assets/Data/UI/UiTextTable.asset`) · kept 2026-11-13
- 줍기 1개 = 아이템 1개, 채집 거리 2m, 후보 지점 170개 (`Assets/Data/Items/ResourcesConfig.asset`, `Assets/Scenes/Game.unity`) · kept 2026-11-13
- 첫날 보장: 시작 위치에서 가까운 후보 20곳 안에 나무 6, 열매 5 (`ResourcesConfig.asset`) · kept 2026-11-13
- 가방: 쓴 칸은 앞으로 당겨짐, 칸 수가 줄어도 가진 것은 잃지 않음 (`Assets/Scripts/Systems/Resources/Logic/Bag.cs`) · kept 2026-11-13
- 버리기는 칸 전체를 한 덩어리로 발밑에, 좌클릭도 칸 선택 (`DropItemAction.cs`, `BagSlotView.cs`) · kept 2026-11-13
- 사용은 즉시, 게이지가 가득 차도 사용 가능, 사망 후에는 효과 없음 (`ConsumableRules.cs`) · kept 2026-11-13
- 요리는 가방 앞쪽 아이템부터, 켜진 모닥불 4m 안 (`CookInteractionSource.cs`) · kept 2026-11-13
- E 우선순위: 요리 40 > 나무 넣기 30 > 휴식 20 > 줍기 10, E를 새로 눌러야 시작, E 행동 중 이동 불가(휴식 포함) (`ResourcesConfig.asset`, `CampConfig.asset`, `InteractionDirector.cs`) · kept 2026-11-13
- 나무 넣기·재점화 E 0.5초, 거리 2m (`Assets/Data/Camp/CampConfig.asset`, `Structure_Campfire.asset`) · kept 2026-11-13
- 첫 점화는 나무 1개만, 재점화의 나무 2개는 연료 120초, 꺼진 불은 재점화만 가능, 남은 시간 + 60초가 300초를 넘으면 넣지 못함 (`Assets/Scripts/Systems/Camp/Logic/CampSystem.cs`) · kept 2026-11-13
- 설치: 미리보기 앞 2m, 크기 모닥불 0.75m·은신처 1.5m가 나무·바위와 겹치면 불가, 성공하면 목록 닫힘, 휠은 끝에서 돌아감 (`PlacementController.cs`, `CampConfig.asset`) · kept 2026-11-13
- 나무·바위를 `Obstacle` 레이어(6번)로 옮김 (`ProjectSettings/TagManager.asset`, `Game.unity`) · kept 2026-11-13
- 키 입력은 코드에서 만든 InputAction (`Assets/Scripts/Input/InputSystemInteractionInput.cs`) · kept 2026-11-13
- 연료 표시는 6m 안 가장 가까운 모닥불 1개, 진행은 막대, 가방 칸은 이름 + 개수, 설치 목록은 글자 줄 (`WorldUiHud.cs`, `BagHud.cs`, `PlacementHud.cs`) · kept 2026-11-13
- HUD 값: 알림 3줄·3초, 칸 테두리 색, 흐린 글자 0.6, 프롬프트 높이 1.5m, 연료 표시 높이 3m (`Assets/Data/UI/HudConfig.asset`) · kept 2026-11-13
- 폰트 에셋: Regular 굵기 하나, Dynamic, 아틀라스 2048, 검은 외곽선 0.2 (`Assets/Fonts/NotoSansKR/`) · kept 2026-11-13
- 임시 모양: 줍기·모닥불·은신처는 기본 도형과 색, 은신처 범위는 바닥의 반투명 원 (`Assets/Prefabs/`, `Assets/Art/Placeholder/`) · kept 2026-11-13
- HUD 위 좌클릭은 설치로 넘기지 않음 (엔진 점검 지적, `PlacementController.cs`) · kept 2026-11-13
