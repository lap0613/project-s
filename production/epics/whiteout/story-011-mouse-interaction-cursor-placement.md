# Story 011: 마우스 클릭 상호작용과 커서 위치 설치

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: S
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

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

- [ ] E로 하던 상호작용(줍기, 나무 넣기·점화, 요리, 휴식)을 대상을 좌클릭해서도 시작할 수 있다. E 조작은 그대로 동작한다 (ui-plan §8.1)
- [ ] 좌클릭은 다음 순서로 하나만 처리된다: HUD 위 → UI 클릭 / 설치 모드 중 → 설치 / 하이라이트된 상호작용 대상 위 → 상호작용 / 그 밖 → 공격 입력으로 넘김. 한 번의 클릭이 두 가지 일을 하지 않는다 (M1)
- [ ] 커서를 대상에 올리면 외곽선 하이라이트와 프롬프트가 뜬다. 지금 할 수 없는 대상(재료 부족 등)은 회색 외곽선이다 (M2)
- [ ] 플레이어에서 6m 안의 대상만 하이라이트·클릭된다 (M3)
- [ ] 상호작용 거리(2m) 밖의 대상을 클릭하면 대상 쪽으로 직선으로 걸어가 닿는 즉시 상호작용을 시작한다. WASD 입력, 다른 곳 클릭, 피해, 1초 넘게 막힘 중 하나가 생기면 취소된다 (M4)
- [ ] 클릭은 한 번이면 되고, 진행 시간과 진행 중 이동 불가는 E와 같다(줍기 1초, 요리 3초, 나무 넣기·점화 0.5초). 휴식은 클릭으로 시작하고 이동하면 끝난다 (M5, tech-decisions 28)
- [ ] 한 대상에 가능한 행동이 여러 개면 기존 E 우선순위(요리 > 나무 넣기)를 따른다 (tech-decisions 28)

**커서 위치 설치**

- [ ] 설치 모드의 미리보기가 플레이어 앞 2m가 아니라 마우스 커서가 가리키는 바닥 위치에 뜨고, 좌클릭으로 그 자리에 설치된다 (ui-plan §9)
- [ ] 설치 모드 중 휠은 미리보기를 한 칸에 15°씩 돌리고, 설치물은 돌린 방향으로 놓인다. 휠로 목록이 바뀌지 않으며 목록 선택은 1·2·3만 된다 (P1·P2)
- [ ] 커서가 플레이어에서 6m보다 멀면 미리보기는 플레이어→커서 방향의 6m 지점에 멈춘다 (P3)
- [ ] 놓을 수 없는 자리(나무·바위와 겹침)나 재료 부족이면 미리보기가 빨강이고, 좌클릭하면 설치되지 않고 이유가 알림으로 나온다 (P4)
- [ ] 설치 모드 중 좌클릭은 공격·클릭 상호작용을 일으키지 않는다. 우클릭·B·Esc로 취소된다 (ui-plan §9)

**공통**

- [ ] 클릭 가능 거리, 자동 이동 취소 시간, 회전 단위, 최대 설치 거리, 하이라이트 색은 설정 에셋 값이고 코드에 없다 (tech-decisions 1)
- [ ] 새 화면 문구(설치 불가 이유 등)는 `UiTextTable`에서 읽는다 (tech-decisions 27)
- [ ] 증거 스크린샷을 `production/qa/evidence/story-011-mouse-interaction-cursor-placement/`에 남긴다: 대상 하이라이트(가능·불가), 클릭으로 줍는 중, 커서 위치 미리보기(파랑), 돌린 미리보기, 설치 불가(빨강) (tech-decisions 25)

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

- `Assets/Scripts/Input/**` (커서 위치, 클릭·휠 입력 정리)
- `Assets/Scripts/Core/Contracts/IInteractionInput.cs` (커서·회전 입력 추가)
- `Assets/Scripts/Game/**` (`InteractionDirector`: 클릭 시작, 자동 이동)
- `Assets/Scripts/Systems/Camp/Unity/View/PlacementController.cs` (커서 위치, 회전, 최대 거리)
- `Assets/Scripts/Systems/Gathering/**` (실제 폴더는 `Resources`일 수 있음: 줍기 대상 하이라이트)
- `Assets/Scripts/UI/**` (프롬프트, 설치 안내)
- `Assets/Data/**` (설정 값, 문구)
- `docs/architecture/tech-decisions.md` (28번 설치·입력 값 갱신), `docs/architecture/code-structure.md` (§4 입력 인터페이스)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: AC '증거'의 스크린샷. MCP로 에디터 Play Mode를 돌리고 입력 이벤트(마우스 위치·클릭·휠)를 넣어 Game View를 찍는다 (tech-decisions 18·21·25)
**지영 플레이 확인 필요** (tech-decisions 25): 클릭으로 줍는 손맛과 E 대비 편한지, 자동 이동이 답답하거나 위험하지 않은지, 클릭 상호작용이 의도치 않게 나가지 않는지(M1),
커서 설치가 원하는 자리에 놓이는지, 회전 15°와 최대 거리 6m가 맞는지. 확인 전까지 M1~M5·P1~P4는 임시값.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md` (상호작용·설치 기반)
- 작업 순서: `production/epics/whiteout/story-010-hud-forecast-timeline.md` 다음에 한다 (같은 씬·UI 파일을 고친다)
- Unlocks: None (story-006·007·008이 이 클릭·설치 방식을 따른다)
