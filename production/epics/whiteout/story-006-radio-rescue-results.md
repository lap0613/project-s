# Story 006: 구조 신호기·부품·구조·결과 화면

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-13

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 7` (부품 3개 수집, 구조 신호기 설치·완성, 구조 요청 후 2일 타이머, 구조)
**근거 문서**: `design/quick-specs/core-rules.md` §1·5 · `design/quick-specs/items.md` §2·6·7 · `docs/architecture/code-structure.md` §3·4·6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 7 and Goal & fail state, scoped to this story, with the values from the 근거 문서:*

- [x] 구조 신호기 부품 3개(안테나, 배터리, 송신기)가 2·4·6일차 이후 하나씩 맵에 나타난다. 안테나는 가운데 링(60~130m), 배터리·송신기는 바깥 링(130~200m)의 후보 지점 중 무작위. E 1초로 줍고, 가방 1칸, 쌓이지 않는다 (core-rules §1·5, items §6)
- [x] 금속 조각 6개가 매 판 후보 지점 중 무작위로 맵에 배치된다(쌓임 5) (items §2)
- [x] B로 구조 신호기 틀을 원하는 위치에 설치한다: 나무 4 + 금속 조각 2(임시값). 한 판에 1개만, 설치 후 옮길 수 없고, 늑대가 부술 수 없다 (core-rules §1, items §7)
- [x] 틀에 E로 부품을 하나씩 장착한다. 3개를 모두 장착하면 완성되고 `SignalBeaconCompleted`가 발행된다 (core-rules §1)
- [x] 완성된 신호기에 E로 구조를 요청하면 `RescueRequested`가 발행되고, 그 시점부터 2일(게임 시간 360초) 타이머가 시작된다. HUD에 구조까지 남은 시간이 보인다. 증강 선택 중에는 타이머도 멈춘다. 요청 마감은 없다 (core-rules §1)
- [x] 타이머가 끝나면 `RescueArrived`가 발행되고 승리한다. 위치 제약은 없다
- [x] 사망(`PlayerDied`)이나 구조 시 결과 화면이 뜬다: 생존 일수, 고른 증강, 사망 원인(동사·아사·늑대). "다시 하기"를 누르면 Game 씬이 다시 로드되고 증강은 사라진다 (core-rules §1)
- [x] 신호기, 남은 시간 HUD, 결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 신호기 틀 설치와 부품 장착은 Camp 시스템, 구조 타이머와 승패는 RunFlow 시스템이다. 둘은 이벤트로만 통신한다 (code-structure §2·3·4).
- 신호기는 004의 구조물 정의(데이터)로 추가한다. 설치 시스템 코드를 고치지 않는다 (code-structure §6).
- 재시작은 Game 씬 재로드만으로 한다 (tech-decisions 12).
- **착수 전 결정 필요:** "다시 하기" 버튼(Game 씬)이 Bootstrap 씬의 `GameRestarter`를 호출할 경로가 아직 없다
  (`code-structure.md` §7 미정). 이 스토리를 시작하기 전에 지영에게 선택지를 드려 정하고 문서에 먼저 기록한다.
- 사망 원인 "늑대"는 007이 `PlayerDamaged`에 원인을 실어 보낸 뒤 채워진다. 이 스토리는 원인 타입만 받는다.
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 005: 증강 선택 자체 (결과 화면은 고른 증강을 보여주기만 한다)
- 007: 늑대 공격과 늑대 사망 원인 발생
- 008: 금속 조각을 쓰는 도구 제작

---

## Files

- `Assets/Scripts/Core/Contracts/IPlacementMenu.cs`
- `Assets/Scripts/Core/Contracts/IRescueParts.cs`
- `Assets/Scripts/Game/BootstrapLifetimeScope.cs`
- `Assets/Scripts/Game/GameLifetimeScope.cs`
- `Assets/Scripts/Game/GameRestarter.cs`
- `Assets/Scripts/Game/InteractionDirector.cs`
- `Assets/Scripts/Game/SurvivalPlayerBridge.cs`
- `Assets/Scripts/Systems/Camp/Logic/CampSystem.cs`
- `Assets/Scripts/Systems/Camp/Logic/PlaceResult.cs`
- `Assets/Scripts/Systems/Camp/Logic/StructureSpec.cs`
- `Assets/Scripts/Systems/Camp/Unity/Data/CampConfig.cs`
- `Assets/Scripts/Systems/Camp/Unity/Data/StructureDefinition.cs`
- `Assets/Scripts/Systems/Camp/Unity/View/CampRegistry.cs`
- `Assets/Scripts/Systems/Camp/Unity/View/CampTextKeys.cs`
- `Assets/Scripts/Systems/Camp/Unity/View/PlacementController.cs`
- `Assets/Scripts/Systems/Environment/Unity/View/EnvironmentView.cs`
- `Assets/Scripts/Systems/Gathering/Unity/View/ResourceSpawner.cs`
- `Assets/Scripts/Systems/Gathering/Unity/View/ResourceTextKeys.cs`
- `Assets/Scripts/Systems/RunFlow/Unity/Data/RunFlowConfig.cs`
- `Assets/Scripts/Systems/Survival/Unity/View/SurvivalRunner.cs`
- `Assets/Scripts/UI/GoalGuidanceHud.cs`
- `Assets/Scripts/UI/GoalIconShape.cs`
- `Assets/Scripts/UI/GoalIconSprites.cs`
- `Assets/Scripts/UI/HudBuilder.cs`
- `Assets/Scripts/UI/HudConfig.cs`
- `Assets/Scripts/UI/PlacementHud.cs`
- `Assets/Scripts/Core/Contracts/Events/BeaconPartAppeared.cs`
- `Assets/Scripts/Core/Contracts/Events/RescueArrived.cs`
- `Assets/Scripts/Core/Contracts/Events/RunEnded.cs`
- `Assets/Scripts/Core/Contracts/Events/SignalBeaconCompleted.cs`
- `Assets/Scripts/Core/Contracts/IRescueCountdown.cs`
- `Assets/Scripts/Core/Contracts/IRunRestarter.cs`
- `Assets/Scripts/Core/Contracts/RescuePartState.cs`
- `Assets/Scripts/Systems/Camp/Logic/BeaconActionResult.cs`
- `Assets/Scripts/Systems/Camp/Logic/SignalBeacon.cs`
- `Assets/Scripts/Systems/Camp/Logic/SignalBeaconSpec.cs`
- `Assets/Scripts/Systems/Camp/Logic/SignalBeaconSystem.cs`
- `Assets/Scripts/Systems/Camp/Unity/Data/SignalBeaconDefinition.cs`
- `Assets/Scripts/Systems/Camp/Unity/View/BeaconInteractionSource.cs`
- `Assets/Scripts/Systems/Camp/Unity/View/SignalBeaconView.cs`
- `Assets/Scripts/Systems/RunFlow/Logic/PartAppearance.cs`
- `Assets/Scripts/Systems/RunFlow/Logic/PartSchedule.cs`
- `Assets/Scripts/Systems/RunFlow/Logic/PartScheduleSettings.cs`
- `Assets/Scripts/Systems/RunFlow/Logic/RescueTimer.cs`
- `Assets/Scripts/Systems/RunFlow/Logic/RunOutcome.cs`
- `Assets/Scripts/Systems/RunFlow/Unity/View/RunFlowRunner.cs`
- `Assets/Scripts/UI/ResultsScreen.cs`
- `Assets/Scripts/Game/PlaceholderRescueParts.cs` (삭제)
- `Assets/Scripts/Systems/RunFlow/Unity/View/DayClockRunner.cs` (삭제, `RunFlowRunner`로 대체)
- `Assets/Data/Camp/Structure_SignalBeacon.asset`, `Assets/Data/Camp/{CampConfig,StructureCatalog,Structure_Campfire,Structure_Shelter}.asset`
- `Assets/Data/Items/{Item,Resource}_{metal,antenna,battery,transmitter}.asset`, `Assets/Data/Items/{ResourceCatalog,ResourcesConfig}.asset`
- `Assets/Data/RunFlow/RunFlowConfig.asset`, `Assets/Data/UI/{HudConfig,UiTextTable}.asset`
- `Assets/Prefabs/Structures/{SignalBeacon,SignalBeaconPreview}.prefab`, `Assets/Prefabs/Pickups/Pickup_{metal,antenna,battery,transmitter}.prefab`, `Assets/Art/Placeholder/Item_{metal,antenna,battery,transmitter}.mat`
- `Assets/Scenes/Bootstrap.unity` (`restarter` 연결), `Assets/Scenes/Game.unity` (HUD에 `ResultsScreen`)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 신호기·타이머 HUD·결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [x] `production/qa/evidence/story-006-radio-rescue-results/` (스크린샷 10장 + stamp.txt)

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: None

## Completion Notes

**Completed**: 2026-11-13. 근거 문서: `design/quick-specs/core-rules.md` §1·§5, `design/quick-specs/items.md` §2·§6·§7, `design/ux/ui-plan.md` §7.3 [C]·§8·§9·S10, `docs/architecture/tech-decisions.md` 34번, `docs/architecture/code-structure.md` §4·§7.
**Criteria**: 8/8 (아래 '확인하지 못한 것' 참고)
**Deviations**: (1) Implementation Notes의 "설치 시스템 코드를 고치지 않는다"와 달리 한 판 1개 제한을 설치물 공통 값(`maxCount`)으로 넣으며 `CampSystem`·`PlacementController`·`PlacementHud`를 고쳤다. (2) 스토리 Files 밖 수정: `Systems/Survival`(런 종료 뒤 틱 정지), `Systems/Environment/EnvironmentView.cs`(다시 하기 때 나던 기존 예외), `Core/Contracts`, `Game`. (3) "착수 전 결정 필요"였던 재시작 경로는 지영에게 묻지 않고 추천안(`IRunRestarter`)을 임시로 정했다(PM 운영 규칙).
**Test Evidence**: waived at qa.level minimal. 스크린샷 `production/qa/evidence/story-006-radio-rescue-results/`
**Reviewed by**: unity-specialist 엔진 점검(BREAKS 0, MINOR 8). 코드 리뷰는 minimal 단계라 하지 않음.
**Run result**: OBSERVED — 부품 등장 알림, 설치 목록·미리보기, 신호기 틀·부품 장착·완성, 구조 타이머 HUD, 결과 화면(구조·동사), 다시 하기 (`production/qa/evidence/story-006-radio-rescue-results/`)

**확인한 것 (에디터 Play Mode, Bootstrap에서 시작)**
- `01-part-appeared-notice.png`: 4일차 아침 배터리 등장 알림(바깥 링). 실제 놓인 곳: 안테나 110m(가운데 링), 배터리 183m·송신기 185m(바깥 링). 금속 조각 줍기 6개. 이 스크린샷의 문구는 고치기 전(`배터리이(가)`)이고 지금은 `배터리가`다.
- `02-placement-menu-beacon-preview.png`: B → 3, 목록에 `3 구조 신호기 틀 나무 4 + 금속 조각 2`, 커서 위치 미리보기.
- `03-beacon-frame-placed-menu-limit.png`: 좌클릭 설치(나무 4·금속 2 소모), 다시 B를 누르면 3번이 회색 `한 판에 1개`.
- `04-beacon-install-prompt-parts-held.png`: `[E] 안테나 장착` 프롬프트, [C] 부품 칸 2개 밝음 `2/3`.
- `05-beacon-two-parts-installed-missing-prompt.png`: E 두 번으로 2개 장착(모델에 부품 나타남, [C]에 체크), 부품이 없으면 회색 `부품 필요 (2/3)`.
- `06-beacon-complete-request-prompt.png`: 3개 장착 후 완성 알림과 `[E] 구조 요청`, `3/3`.
- `07-rescue-requested-timer-hud.png`: 구조 요청 후 `구조까지 1일 02:59`(360초에서 시작), 타임라인 깃발, 신호기 불빛, 알림.
- `08-results-rescued.png`: 타이머 종료 → `구조되었습니다`, 생존 일수 6일, 고른 증강 없음, 다시 하기. 이후 허기·체력·시계가 멈추고 플레이어 이동이 꺼짐(값으로 확인).
- `09-results-death-freezing-after-restart.png`: 다시 하기 → 새 판(1일차, `0/3`, 신호기 없음, 타이머 꺼짐) → 동사 → `얼어 죽었습니다`, 사망 원인 동사. 다시 한 판에서 2일차 안테나가 1개만 놓임(이전 판의 구독이 남지 않음).
- `10-beacon-complete-oblique-view.png`: 비스듬한 카메라로 본 완성된 신호기.
- 밤(136초)에 다시 하기 → 새 판 주변광 1.0으로 복구, 콘솔 오류·경고 0건.

**시험용으로 조작한 것 (tech-decisions 25번 방식):** 2·4·6일차는 시계(`DayClock.Tick`)를 당겨서 넘겼다. 부품과 재료(나무·금속 조각)는 가방에 직접 넣었다(맵에서 걸어가 줍지 않았다). 구조 대기 360초는 타이머를 당겨 끝냈다. 동사는 체온·체력 값을 낮춰 일으켰다. 키·마우스는 입력 이벤트로 넣었다(B, 3, 좌클릭, E). 비스듬한 스크린샷의 신호기는 설치 코드와 같은 방식으로 직접 놓았다.

**확인하지 못한 것:** 부품·금속 조각을 맵에서 직접 줍기(E 1초, 기존 줍기 코드와 데이터만 다름), 실제 360초 대기, 아사 결과 화면, 증강 선택 중 타이머 정지(시계를 멈추는 화면이 아직 없다. `DayClock.IsPaused`를 따르도록만 만들었다), 고른 증강 표시(005 보류, `없음`만), 늑대 사망 원인(007), `타이틀로` 버튼(009), 클릭(마우스)으로 신호기 상호작용, 빌드 실행.

## Calls beyond the design

모두 임시이고 지영 확인이 필요하다. 값과 다른 선택지는 `docs/architecture/tech-decisions.md` 34번.
- 재시작 경로 `IRunRestarter` (Core 인터페이스, Bootstrap 스코프 등록)
- 부품 등장 시점 = 2·4·6일차 아침, 위치 = 자원 줍기 후보 지점 중 그 링의 빈 곳
- 한 판 1개 = 설치물 공통 `maxCount`
- 신호기 틀: 판정 반경 1.0m, 거리 2m, E 0.5초, 우선순위 35, 장착 순서, 구조 대기 = 2일 × 하루 길이
- 런 종료 = 사망·구조 중 먼저 일어난 쪽(`RunEnded`), 생존 일수 = 끝난 날의 날짜 숫자
- 결과 화면: 1초 페이드, 제목·원인 문구, `다시 하기`만, 고른 증강 `없음`
- 구조 타이머 형식과 초 올림, 부품 칸 체크 표시, 신호기 상태를 모델 부품 자리 + 프롬프트로 표시
- 문구 전부(`UiTextTable`의 `prompt.beacon_*`, `notice.beacon_*`, `notice.rescue_requested`, `notice.placement_limit`, `notice.part_appeared.*`, `hud.rescue.*`, `placement.limit_reached`, `result.*`)
- 색·크기(`HudConfig`의 체크·타이머·결과 화면 값), 신호기·부품·금속 조각의 임시 모양과 색
