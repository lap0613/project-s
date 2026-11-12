# Story 003: 허들 3종과 사망

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 2` (허들 3종(체온·허기·체력)과 상호 영향)
**근거 문서**: `design/quick-specs/core-rules.md` §2·3 · `docs/architecture/tech-decisions.md` 2·3 · `docs/architecture/code-structure.md` §4·5

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 2, scoped to this story, with the values from the 근거 문서:*

- [ ] 체온·허기·체력은 0~100 스탯이고, 기본값 + 수정자로 계산된다 (code-structure §5)
- [ ] 체온: 초당 변화 = 환경 온도 × 0.1. 허기가 30 미만이면 감소량만 ×1.5. 100을 넘지 않는다 (core-rules §2·3)
- [ ] 허기: 매초 -0.4. 식량으로 오르는 것은 004가 연결한다
- [ ] 체력: 체온·허기가 모두 60 이상이면 초당 +0.5 회복한다. 은신처 휴식으로 조건이 바뀌는 것은 004가 연결한다
- [ ] 30 미만 불이익: 체온이 30 미만이면 이동속도 -25%, 허기가 30 미만이면 채집 속도 -30%(적용 연결은 004)
- [ ] 0일 때: 체온 0이면 체력 -2/초, 허기 0이면 체력 -1/초, 둘 다 0이면 합산된다
- [ ] 체력이 0이 되면 `PlayerDied`(원인: 동사 / 아사)를 발행하고 입력이 멈춘다
- [ ] 허들 값은 환경 온도를 `IAmbientTemperatureSource`로만 읽는다. Survival은 Environment를 직접 참조하지 않는다 (code-structure §3)
- [ ] 체온·허기·체력 게이지 HUD를 스크린샷으로 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 규칙은 순수 C# 클래스(Whiteout.Survival)로 두고 `Tick(dt)`로 진행한다. MonoBehaviour는 View에서 호출만 한다 (tech-decisions 3).
- 모든 속도·기준값(0.1, 0.4, 0.5, 30, 60, 1.5)은 ScriptableObject 데이터 (tech-decisions 1).
- 테스트는 지금 선택사항이지만 테스트할 수 있는 구조(시간 주입, 인터페이스 주입)는 유지한다 (tech-decisions 3).
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 002: 환경 온도 계산
- 004: 식량 섭취, 은신처 휴식 회복, 채집 속도 적용
- 006: 결과 화면 (`PlayerDied` 구독)

---

## Files

- `Assets/Scripts/Systems/Survival/Logic/**`
- `Assets/Scripts/Systems/Survival/Unity/**` (Data, HUD View)
- `Assets/Scripts/Core/Contracts/**` (`PlayerDied`)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Test**: waived at qa.level minimal
**Required evidence**: 게이지가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-002-time-weather-temperature.md`
- Unlocks: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`, `production/epics/whiteout/story-005-augments.md`, `production/epics/whiteout/story-007-wolves-combat.md`
