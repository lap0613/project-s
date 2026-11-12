# Story 007: 늑대와 단순 공격

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 6` (밤에 나타나는 늑대와 단순 공격)
**근거 문서**: `design/quick-specs/core-rules.md` §6 · `design/quick-specs/items.md` §2·3 · `docs/architecture/code-structure.md` §4·6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 6, scoped to this story, with the values from the 근거 문서:*

- [ ] 적은 `EnemyDefinition` ScriptableObject + `IEnemyBehaviour` + `IEnemySpawnRule`로 만들어, 새 적을 기존 코드 수정 없이 추가할 수 있다 (code-structure §6)
- [ ] 늑대는 밤에만 나타난다: 켜진 모닥불 빛 밖에 1~2마리, 아침(`DayStarted`)에 사라진다 (core-rules §6)
- [ ] 늑대는 켜진 모든 모닥불의 빛 안으로 들어오지 않는다 (core-rules §6)
- [ ] 플레이어 공격: 좌클릭, 근접 1.5m, 0.6초 간격, 마우스 방향. 늑대는 3번 맞으면 쓰러진다 (core-rules §6)
- [ ] 늑대의 물기: 체력 -15, 1.5초 간격. `PlayerDamaged`를 발행하고 사망 원인에 "늑대"가 실린다 (core-rules §6, code-structure §4)
- [ ] 늑대를 처치하면 날고기 1개와 가죽 1개를 떨어뜨린다 (items §2·3). 가죽 아이템 정의는 이 스토리에서 만든다
- [ ] 스크린샷(늑대와 모닥불 빛 경계)을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 늑대 행동·전투 규칙은 Threats 시스템의 순수 C#로 두고, 위치·충돌·빛 판정은 View가 한다.
- 켜진 모닥불 목록은 Camp 시스템이 이벤트(`FireStateChanged`)로 알린 것을 Threats가 구독해서 쓴다. Camp를 직접 참조하지 않는다 (code-structure §3·4).
- 공격 거리·간격·체력·피해량은 모두 ScriptableObject 데이터다.
- 문서에 없는 규칙(늑대 이동 속도, 추적 방식 등)이 필요하면 만들지 말고 보고한다. core-rules §6에는 이 값들이 없다.

---

## Out of Scope

- 008: 창 등 도구로 바뀌는 공격력, 횃불의 늑대 접근 금지
- 사냥 시스템 (브리프 Your systems의 later)
- 늑대 외의 다른 적

---

## Files

- `Assets/Scripts/Systems/Threats/**`
- `Assets/Scripts/Systems/Player/**` (공격)
- `Assets/Scripts/Core/Contracts/**` (`PlayerDamaged`)
- `Assets/Data/**` (`EnemyDefinition` 에셋, 가죽 정의)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 늑대가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-003-hurdles-death.md`, `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: `production/epics/whiteout/story-008-tools-equipment-crafting.md`
