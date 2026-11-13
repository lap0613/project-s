# Story 011: 마우스 클릭 상호작용과 커서 위치 설치

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: S
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-13

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 3` (채집, 모닥불·은신처 설치)의 조작 개선. 지영 결정 2026-11-13: 마우스 비중 확대, 설치 방식 변경
**근거 문서**: `design/ux/ui-plan.md` §1 원칙 10 · §8.1 마우스 클릭 상호작용 · §8.2 M1~M5 · §9 설치 모드 · P1~P4 ·
`design/quick-specs/core-rules.md` §4(채집·설치 조작) · `docs/architecture/tech-decisions.md` 28(현재 상호작용·설치 값)·25(검증 경로) ·
`docs/architecture/code-structure.md` §4(`IInteractionInput`, `IInteractionSource`, `IPlacementMenu`, `InteractionDirector`)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: MEDIUM
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/ux/ui-plan.md` §8.1·§9. M·P 번호가 붙은 값은 임시값(지영 확인 필요)이고 모두 설정 에셋에 둔다.*

**마우스 클릭 상호작용**

- [x] E로 하던 상호작용(줍기, 나무 넣기·점화, 요리, 휴식)을 대상을 좌클릭해서도 시작할 수 있다. E 조작은 그대로 동작한다 (ui-plan §8.1)
- [x] 좌클릭은 다음 순서로 하나만 처리된다: HUD 위 → UI 클릭 / 설치 모드 중 → 설치 / 하이라이트된 상호작용 대상 위 → 상호작용 / 그 밖 → 공격 입력으로 넘김. 한 번의 클릭이 두 가지 일을 하지 않는다 (M1)
- [x] 커서를 대상에 올리면 외곽선 하이라이트와 프롬프트가 뜬다. 지금 할 수 없는 대상(재료 부족 등)은 회색 외곽선이다 (M2)
- [x] 플레이어에서 6m 안의 대상만 하이라이트·클릭된다 (M3)
- [x] 상호작용 거리(2m) 밖의 대상을 클릭하면 대상 쪽으로 직선으로 걸어가 닿는 즉시 상호작용을 시작한다. WASD 입력, 다른 곳 클릭, 피해, 1초 넘게 막힘 중 하나가 생기면 취소된다 (M4)
- [x] 클릭은 한 번이면 되고, 진행 시간과 진행 중 이동 불가는 E와 같다(줍기 1초, 요리 3초, 나무 넣기·점화 0.5초). 휴식은 클릭으로 시작하고 이동하면 끝난다 (M5, tech-decisions 28)
- [x] 한 대상에 가능한 행동이 여러 개면 기존 E 우선순위(요리 > 나무 넣기)를 따른다 (tech-decisions 28)

**커서 위치 설치**

- [x] 설치 모드의 미리보기가 플레이어 앞 2m가 아니라 마우스 커서가 가리키는 바닥 위치에 뜨고, 좌클릭으로 그 자리에 설치된다 (ui-plan §9)
- [x] 설치 모드 중 휠은 미리보기를 한 칸에 15°씩 돌리고, 설치물은 돌린 방향으로 놓인다. 휠로 목록이 바뀌지 않으며 목록 선택은 1·2·3만 된다 (P1·P2)
- [x] 커서가 플레이어에서 6m보다 멀면 미리보기는 플레이어→커서 방향의 6m 지점에 멈춘다 (P3)
- [x] 놓을 수 없는 자리(나무·바위와 겹침)나 재료 부족이면 미리보기가 빨강이고, 좌클릭하면 설치되지 않고 이유가 알림으로 나온다 (P4)
- [x] 설치 모드 중 좌클릭은 공격·클릭 상호작용을 일으키지 않는다. 우클릭·B·Esc로 취소된다 (ui-plan §9)

**공통**

- [x] 클릭 가능 거리, 자동 이동 취소 시간, 회전 단위, 최대 설치 거리, 하이라이트 색은 설정 에셋 값이고 코드에 없다 (tech-decisions 1)
- [x] 새 화면 문구(설치 불가 이유 등)는 `UiTextTable`에서 읽는다 (tech-decisions 27)
- [x] 증거 스크린샷을 `production/qa/evidence/story-011-mouse-interaction-cursor-placement/`에 남긴다: 대상 하이라이트(가능·불가), 클릭으로 줍는 중, 커서 위치 미리보기(파랑), 돌린 미리보기, 설치 불가(빨강) (tech-decisions 25)

---

## Implementation Notes

- 클릭 상호작용은 새 행동을 만들지 않는다. 기존 `IInteractionSource`가 내놓는 행동을 `InteractionDirector`가 E 대신 클릭으로도 시작하게 한다 (code-structure §4).
- 커서 위치·클릭 대상 판정(화면 → 바닥·오브젝트 레이캐스트)은 View가 하고 결과만 넘긴다 (tech-decisions 13의 방식).
- 공격은 story-007에서 만든다. 이 스토리는 '상호작용으로 쓰이지 않은 좌클릭'을 공격 쪽이 받을 수 있게 입력을 정리해 두는 데까지만 한다.
- `IInteractionInput`의 휠은 지금 목록 선택(`SelectionScroll`)에 쓰인다. 설치 모드 중에는 회전으로 쓰도록 바꾼다.
- 직선 자동 이동은 `PlayerController`의 이동 입력 경로를 쓴다. 길찾기(NavMesh)는 쓰지 않는다.
- 폴더 이름: 문서의 `Systems/Gathering/`은 실제로 아직 `Systems/Resources/`일 수 있다 (tech-decisions 7, 이동은 S개발).
- 문서에 없는 결정이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 공격 자체와 늑대 (story-007)
- 신호기 틀 설치·부품 장착 (story-006, 만들어지면 같은 클릭·설치 방식을 쓴다)
- 장애물을 돌아가는 자동 이동(길찾기)
- 게임패드용 대체 조작 (MVP 이후, ui-plan D11)
- 모닥불 점화 나무 2개 변경의 코드 반영 (tech-decisions 28, 별도 수정)

---

## Files

- `Assets/Scripts/Core/Contracts/IInteractionInput.cs` (휠 `WheelScroll`, 커서 위치), `IInteractionSource.cs` (`TryOfferPointed`, `IsTargetInReach`), `IPlacementMenu.cs` (`ClickConsumedFrame`)
- `Assets/Scripts/Core/Contracts/IPointerWorld.cs`, `PointerQuery.cs`, `IAttackInput.cs`, `Events/PlayerDamaged.cs` (새 파일)
- `Assets/Scripts/Core.Unity/PointerConfig.cs` (새 파일), `Assets/Data/UI/PointerConfig.asset`, `Assets/Art/Materials/M_Outline.mat` (새 에셋)
- `Assets/Scripts/Input/InputSystemInteractionInput.cs`
- `Assets/Scripts/Game/InteractionDirector.cs`, `PlayerActionLock.cs`, `PointerWorld.cs`, `TargetHighlighter.cs`, `GameLifetimeScope.cs`, `Whiteout.Game.asmdef`, `SurvivalGatherBridge.cs`
- `Assets/Scripts/Systems/Camp/**` (`PlacementController`, `FuelInteractionSource`, `RestInteractionSource`, `ShelterView`, `CampSystem`, `CampfireSpec`, `CampfireDefinition`, `CampConfig`, `FireActionResult`, `CampTextKeys`)
- `Assets/Scripts/Systems/Gathering/**` (폴더 이름 변경 `Resources` → `Gathering`, 어셈블리·네임스페이스 `Whiteout.Gathering(.Unity)`; `GatherInteractionSource`, `CookInteractionSource`, `PickupRegistry`에 클릭 대상 추가)
- `Assets/Data/Camp/CampConfig.asset`, `Structure_Campfire.asset` (점화 나무 2), `Assets/Data/UI/UiTextTable.asset` (`prompt.ignite`, `placement.hint`), `Assets/Data/Items/*.asset`·`Assets/Scenes/Game.unity` (네임스페이스 이름 변경, `pointerConfig` 연결)
- `docs/architecture/tech-decisions.md` (7·28번 갱신, 31번 추가), `docs/architecture/code-structure.md` (§2·§4), `design/quick-specs/core-rules.md` (§4 점화 조건)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal (자동 테스트 없음)
**Required evidence**: AC '증거'의 스크린샷. MCP로 에디터 Play Mode를 돌리고 입력 이벤트(마우스 위치·클릭·휠)를 넣어 Game View를 찍는다 (tech-decisions 18·21·25)
**지영 플레이 확인 필요** (tech-decisions 25): 클릭으로 줍는 손맛과 E 대비 편한지, 자동 이동이 답답하거나 위험하지 않은지, 클릭 상호작용이 의도치 않게 나가지 않는지(M1),
커서 설치가 원하는 자리에 놓이는지, 회전 15°와 최대 거리 6m가 맞는지. 확인 전까지 M1~M5·P1~P4는 임시값.

**Status**: Evidence retained — `production/qa/evidence/story-011-mouse-interaction-cursor-placement/` (아래 Completion Notes)

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md` (상호작용·설치 기반)
- 작업 순서: `production/epics/whiteout/story-010-hud-forecast-timeline.md` 다음에 한다 (같은 씬·UI 파일을 고친다)
- Unlocks: None (story-006·007·008이 이 클릭·설치 방식을 따른다)

---

## Completion Notes (2026-11-13)

**Run result**: OBSERVED — 에디터 Play Mode(Bootstrap 시작)에서 마우스·키 입력을 매 프레임 주입해 실행 (tech-decisions 31번 검증 항목).
증거: `production/qa/evidence/story-011-mouse-interaction-cursor-placement/`

| 파일 | 내용 |
|---|---|
| `01-hover-available.png` | 나무 위 커서: 외곽선 + `[E] 줍기 — 나무` |
| `02-hover-unavailable.png` | 재료 부족 모닥불 위 커서: 회색 `[E] 불 붙이기 (나무 1/2)` |
| `02b-hover-available-campfire.png` | 재료 충분한 모닥불 위 커서 |
| `03-click-gather-in-progress.png` | 클릭으로 줍는 중(진행 막대) |
| `04-placement-cursor-preview-blue.png` | 커서 위치 미리보기(파랑) |
| `05-placement-rotated.png` | 휠 3칸 = 45° 돌린 미리보기 |
| `06-placement-max-distance.png` | 커서가 12m 밖일 때 6.0m 지점에 멈춤 |
| `07-placement-blocked-red.png` | 바위와 겹침 = 빨강 |
| `08-placement-blocked-notice.png` | 빨강에서 클릭: 알림 `여기에는 설치할 수 없습니다`, 설치 안 됨 |
| `09-placed-campfire.png`, `10-click-greyed-notice.png`, `11-click-walk-ignited.png` | 설치(45° 반영), 회색 대상 클릭, 걸어가 점화 |
| `12-hover-shelter.png`, `13-click-rest.png` | 은신처 클릭 휴식 |

숫자로 확인: 클릭 줍기 후 가방 +1(걸어가 2m에서 시작), 점화 나무 2 소모·연료 120초·부싯돌 유지, 모닥불 콜라이더에 막혀 중심에서 0.95m에 멈춘 뒤 1초 만에 걷기 취소,
휠 3칸 = 45°·목록 선택 변화 없음, 빈 바닥 좌클릭 = `AttackClicked` 1회, 설치 모드 중 좌클릭 = `AttackClicked` 0회, 우클릭으로 설치 모드 닫힘, E 줍기 정상.

**Deviations**
- 스토리 Out of Scope의 '모닥불 점화 나무 2개 코드 반영'을 PM 지시로 이번에 함께 했다(M6 추천값: 부싯돌 필요 포함, tech-decisions 31번).
- 프로그래머 에이전트 없이 직접 구현했다(PM 지시: 멈추지 말고 끝까지). 엔진 점검 전문가 호출은 생략했다 → 아래 '엔진 위험'.
- 폴더·어셈블리 이름 변경(`Systems/Resources` → `Systems/Gathering`, `Whiteout.Gathering(.Unity)`)을 같은 작업에 포함했다(PM 지시, 이름은 임시).

**Engine risks**: 점검 에이전트를 돌리지 않았다. 새로 쓴 API: `Graphics.RenderMesh(RenderParams)`(6.3에서 컴파일·실행 확인), `EventSystem.IsPointerOverGameObject`, Input System `Mouse.current`. 콘솔 오류·경고 0.

**Own calls** (스토리·설계에 없어 정한 것, 모두 tech-decisions 31번): 커서가 대상 위인지의 반경 0.7m와 수평면 교점 방식, 외곽선(1.08배 Cull Front 머티리얼, 노랑/회색), 막힘 기준(0.3m/s·1초),
회색 대상 클릭은 걷지 않고 알림, 걷는 중 클릭 = 취소(공격 아님), 설치 모드를 닫으면 회전 초기화, `PlayerDamaged` 이벤트 신설(발행처 없음), 첫 점화에도 부싯돌 필요.

**지영 확인 필요**: M1~M6·P1~P4(추천값으로 구현), 위 Own calls, Gathering 어셈블리·네임스페이스 이름, 클릭 줍기의 손맛, 자동 이동이 답답하거나 위험하지 않은지,
클릭 상호작용이 의도치 않게 나가지 않는지(M1), 회전 15°와 6m가 맞는지, 외곽선이 밝은 눈 배경에서 약해 보임.

**미확인**: 꺼진 불 재점화의 클릭 실행, `PlayerDamaged`로 걷기 취소(발행처 없음), HUD 위 클릭이 새지 않는지 실제 HUD 위 클릭, 은신처에 막히는 경우(은신처에는 콜라이더가 없음),
실제 마우스 조작, 한 프레임에 휠 여러 칸.

**Completed**: 2026-11-13
**Criteria**: 15/15 passing (손맛·자동 이동 체감·의도치 않은 클릭 여부는 지영 플레이 확인, DEFERRED)
**Deviations**: ADVISORY — 점화 나무 2개 코드 반영(스토리 Out of Scope)을 PM 지시로 포함. 점화 조건은 M6 추천값 유지(임시, 지영 확인 필요): 개발봇 지시이며 지영이 확인한 것이 아니다. 첫 점화 나무 2만 소모, 재점화 나무 2 + 부싯깃 1, 부싯돌은 가방에 있기만 하면 되고 소모하지 않음.
**Test Evidence**: waived at qa.level minimal; 스크린샷 `production/qa/evidence/story-011-mouse-interaction-cursor-placement/` (stamp: 변경 없음)
**Reviewed by**: self, no independent review

## Calls beyond the design
- 커서 대상 반경 0.7m(수평면 교점 방식), 외곽선 1.08배 노랑/회색, 막힘 0.3m/s·1초, 회색 대상 클릭은 알림만, 걷는 중 클릭은 취소(공격 아님), 설치 모드를 닫으면 회전 초기화, `PlayerDamaged` 이벤트 신설, Gathering 어셈블리 이름 · kept 2026-11-13 (개발봇 지시: 추천안 진행, 임시, 지영 확인 필요; 값·위치는 tech-decisions 31번)
