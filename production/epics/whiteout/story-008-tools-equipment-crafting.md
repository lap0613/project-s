# Story 008: 도구·장비·한 단계 제작

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: L
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 3` (한 단계 제작(은신처). 채집·설치·요리는 004)
**근거 문서**: `design/quick-specs/items.md` §1·2·4·5 · `design/quick-specs/core-rules.md` §4 · `docs/architecture/tech-decisions.md` 2

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 3, scoped to this story, with the values from the 근거 문서:*

- [ ] 천 조각 8개, 금속 조각 6개, 가죽 3개가 매 판 후보 지점 중 무작위로 맵에 배치된다(쌓임: 천 10, 금속 5, 가죽 5). 가죽은 늑대에게서도 나온다 (items §2)
- [ ] 맵에 "버려진 캠프" 상자 3~4개가 있고, 각각 무작위 도구 또는 장비 1개가 나온다 (items §1)
- [ ] 제작은 은신처 안에서만 한다. 한 단계(재료 → 완성품, 중간 재료 없음)이고 레시피는 데이터다 (items §1)
- [ ] 도구: 손도끼(나무 1 + 금속 1, 나무 채집 시간 절반, 내구도 30회), 창(나무 2 + 금속 1, 공격력 2배·사거리 2m, 내구도 20회), 횃불(나무 1 + 천 1, 켜진 모닥불에서 붙임, 주변 6m 밝힘, 늑대가 4m 안으로 오지 않음, 120초) (items §4)
- [ ] 도구는 가방에 넣고 숫자키로 1개를 손에 든다. 부싯돌은 가방에만 있으면 되고 내구도가 없다 (items §1·4)
- [ ] 장비 4부위에 1개씩 입는다: 털 두건(가죽 1 + 천 1, 체감 +2°C), 누빔 외투(천 3 + 가죽 1, +4°C), 가죽 장갑(가죽 1, 채집 +15%), 방한 장화(가죽 2, +2°C). 장비는 가방 칸을 차지하지 않는다 (items §5)
- [ ] 장비 효과는 스탯 수정자로 처리되어 증강(005)과 합산된다 (items §5, tech-decisions 2)
- [ ] 스크린샷(제작 화면과 장비·도구 HUD)을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 제작·도구·장비 정의는 Resources 시스템의 ScriptableObject 데이터다. 새 도구·장비·레시피는 에셋 추가만으로 가능해야 한다 (code-structure §6).
- 장비·도구의 효과는 다른 시스템 코드를 고치지 않고 스탯 ID로 수정자를 붙인다.
- 횃불의 "늑대가 4m 안으로 오지 않음"은 Threats가 읽는 Core 인터페이스(빛 영역)를 통해 알린다.
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 여러 단계 제작 트리 (Out of scope)
- 사냥·제작 품질 증강 (Your systems의 later)
- 요리 (004에서 구현됨)

---

## Files

- `Assets/Scripts/Systems/Resources/**` (제작, 도구, 장비)
- `Assets/Scripts/Systems/Camp/**` (은신처 제작 상호작용)
- `Assets/Scripts/UI/**` (제작·장비 UI)
- `Assets/Data/**` (재료·도구·장비·레시피 에셋)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 제작 화면과 HUD 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`, `production/epics/whiteout/story-007-wolves-combat.md`
- Unlocks: None
