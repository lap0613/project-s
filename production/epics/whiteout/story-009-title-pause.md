# Story 009: 최소 타이틀·로딩 페이드·일시정지

> **Epic**: whiteout
> **Status**: Draft
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: S
> **Build step**: polish
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: UI 결정 D1·D7 (최소 타이틀, Esc 일시정지). 브리프 Features에는 없는 화면 흐름 보강.
**근거 문서**: `design/ux/ui-plan.md` §3 화면 흐름·§4 S0·§5 S1·§11 S9·§12 D1·D7 · `design/quick-specs/core-rules.md` §1(정지 규칙) · `docs/architecture/tech-decisions.md` 12(씬)·24(UI 기술·문구)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: LOW
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/ux/ui-plan.md` D1·D7, with the values from the 근거 문서:*

- [ ] 게임을 실행하면 타이틀 화면이 뜬다. 버튼은 '새 게임'·'종료' 두 개뿐이다(이어하기·설정 없음). 마우스 클릭과 ↑↓(W·S) + Enter(Space) 둘 다로 고를 수 있다 (ui-plan §5)
- [ ] 타이틀에서 Esc를 누르면 종료 확인이 뜬다 (ui-plan §5)
- [ ] '새 게임'을 누르면 검은 화면 페이드(0.3초, 임시값) 뒤 Game 씬이 시작된다 (ui-plan §4)
- [ ] 플레이 중 Esc를 누르면 일시정지 메뉴(계속 / 다시 하기 / 타이틀로)가 뜨고, 증강 선택과 같은 범위로 게임이 정지한다(체온·허기·늑대·시간·구조 타이머) (core-rules §1, ui-plan §11 S9)
- [ ] '다시 하기'·'타이틀로'는 확인 창을 한 번 거친다. '다시 하기'는 Game 씬을 다시 로드하고 증강이 사라진다. '타이틀로'는 타이틀 화면으로 간다 (ui-plan §11)
- [ ] 증강 선택 화면(S8)·결과 화면(S10)·설치 모드·제작 화면이 떠 있을 때 Esc는 일시정지를 열지 않는다 (ui-plan §3·§9·§10·§11)
- [ ] 결과 화면(S10)의 '타이틀로'가 타이틀 화면으로 간다 (ui-plan §11 S10)
- [ ] 화면 문구는 코드에 직접 쓰지 않고 데이터에서 읽는다. 글자는 TextMeshPro (tech-decisions 24)
- [ ] 타이틀, 일시정지 메뉴, 확인 창 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 정지는 증강 선택(005)과 같은 방식으로 건다. 두 화면이 각자 다른 정지 방법을 쓰지 않는다.
- 타이틀을 어느 씬에 둘지(별도 Title 씬 / Bootstrap 씬 안의 UI)는 문서에 없다. tech-decisions 12번(Bootstrap + Game)과
  21번(Play는 Bootstrap에서 시작)에 영향이 있으므로 구현 전에 선택지를 보고하고 지영 결정 후 tech-decisions에 기록한다.
- '다시 하기'의 Game 씬 재로드는 006의 결과 화면과 같은 경로(`GameRestarter`)를 쓴다. code-structure §7의 미정 메모(Game 씬 → 재시작 호출 경로)가 먼저 정해져야 한다.
- 문서에 없는 결정이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 설정 화면(S2): 볼륨·해상도 등 (ui-plan D1, MVP 이후)
- 로고·진행 표시·팁이 있는 로딩 화면, 타이틀 배경 연출 (ui-plan §4·§5, MVP 이후)
- 이어하기·저장 (game-brief Out of scope)
- 게임패드 조작 (tech-decisions 5·24, MVP 이후)

---

## Files

- `Assets/Scripts/UI/**` (타이틀·일시정지·확인 창)
- 타이틀 씬 또는 Bootstrap 씬 UI (위 결정 후 확정)
- UI 문구 데이터 (tech-decisions 24, 형식은 처음 적용하는 스토리에서 정함)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: UI
**Test**: waived at qa.level minimal
**Required evidence**: 타이틀, 일시정지 메뉴, 확인 창 스크린샷을 `production/qa/evidence/`에 남긴다 (MCP 에디터 Play Mode + Game View, tech-decisions 25)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-005-augments.md` (정지 방식 공유), `production/epics/whiteout/story-006-radio-rescue-results.md` (결과 화면·재시작 경로)
- Unlocks: None
