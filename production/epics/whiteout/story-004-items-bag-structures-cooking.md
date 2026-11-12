# Story 004: 아이템 채집·가방·모닥불/은신처 설치·요리

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Core
> **Type**: Integration
> **Estimate**: L
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

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

- [ ] 아이템 정의는 ScriptableObject(`ResourceDefinition`)이고 새 아이템은 에셋 추가만으로 만든다 (code-structure §6)
- [ ] 가방은 기본 6칸. 같은 아이템은 한 칸에 쌓임 수만큼 쌓인다(나무 5, 부싯깃 10, 열매 3, 날고기 3, 구운 고기 3, 약초 5). 가득 차면 채집되지 않고 알려준다 (items §1~3)
- [ ] E키를 1초 누르면 채집된다. 채집하는 동안 이동할 수 없다 (core-rules §4)
- [ ] 나무·부싯깃·열매·날고기·약초가 매 판 후보 지점 중 무작위로 배치된다(items §2·3의 개수). 한 번 쓰면 다시 생기지 않는다
- [ ] 불 없이 시작한다. 시작 소지품: 부싯돌 1, 부싯깃 2, 나무 3, 열매 1. 시작 지점 근처에 첫날 분량의 나무·식량을 보장한다 (items §8)
- [ ] B로 모닥불을 설치한다: 나무 3 + 부싯깃 1, 부싯돌이 가방에 있어야 한다. 설치 후 나무를 넣어야 켜지고, 나무 1개 = 60초 연소, 최대 5개, 개수 제한 없음 (core-rules §4)
- [ ] 꺼진 모닥불은 부싯깃 1 + 나무 2로 다시 피운다(부싯돌 필요). 켜진 모닥불은 반경 4m 안에 +35°C를 환경 온도(002)에 전달하고, 겹치면 가장 따뜻한 1개만 적용된다 (core-rules §3·4)
- [ ] B로 은신처를 설치한다: 나무 6, 반경 3m, 온기 없음, 개수 제한 없음. 안에서 E로 휴식하는 동안 체력 회복 조건이 체온·허기 30 이상으로 낮아지고 회복량이 3배(초당 +1.5)가 된다 (core-rules §2·4)
- [ ] 소모품: 열매 허기 +15, 날고기 +10, 구운 고기 +40, 약초 체력 +20. 켜진 모닥불에서 날고기를 E 3초로 구우면 구운 고기가 된다 (items §3, core-rules §4). 허기 30 미만의 채집 속도 -30%(003)가 실제로 적용된다
- [ ] `FireStateChanged`·`StructurePlaced`·`ResourceGathered` 이벤트가 발행되고, 가방과 모닥불 연료가 HUD에 보인다. 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 아이템·가방·채집·제작은 Resources 시스템, 모닥불·은신처는 Camp 시스템. 서로 참조하지 않고 Core 인터페이스와 이벤트로만 통신한다 (code-structure §3·4).
- 위치 판정(모닥불 반경, 은신처 안, 채집 거리)은 View가 하고 결과만 Logic에 넘긴다 (tech-decisions 13).
- 가방 칸 수는 스탯 수정자로 만든다. 005의 큰 배낭·썰매가 같은 방식으로 붙는다.
- 천 조각·가죽·금속 조각·무전 부품은 이 스토리에서 배치하지 않는다 (008, 006).
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 002: 환경 온도 합산 (이 스토리는 온기 값을 공급한다)
- 003: 허들 계산 자체
- 005: 가방 칸 증강
- 006: 무전 부품
- 007: 늑대 고기·가죽 드롭
- 008: 도구·장비·제작·천 조각·가죽·금속

---

## Files

- `Assets/Scripts/Systems/Resources/**`
- `Assets/Scripts/Systems/Camp/**`
- `Assets/Scripts/Core/Contracts/**` (이벤트, 가방·온기 인터페이스)
- `Assets/Scripts/UI/**` (가방·연료 HUD)
- `Assets/Data/**` (아이템·구조물 정의 에셋)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 설치한 모닥불과 HUD가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-001-foundation-movement-map.md`, `production/epics/whiteout/story-002-time-weather-temperature.md`, `production/epics/whiteout/story-003-hurdles-death.md`
- Unlocks: `production/epics/whiteout/story-005-augments.md`, `production/epics/whiteout/story-006-radio-rescue-results.md`, `production/epics/whiteout/story-007-wolves-combat.md`, `production/epics/whiteout/story-008-tools-equipment-crafting.md`
