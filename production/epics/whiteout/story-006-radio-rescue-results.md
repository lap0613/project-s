# Story 006: 무전 부품·수리·구조·결과 화면

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
**Requirement**: `Brief MVP feature 7` (부품 3개 수집, 무전기 수리, 2일 후 구조)
**근거 문서**: `design/quick-specs/core-rules.md` §1·5 · `design/quick-specs/items.md` §6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 7 and Goal & fail state, scoped to this story, with the values from the 근거 문서:*

- [ ] 무전기는 시작 지점에 고정으로 있다 (core-rules §1)
- [ ] 무전 부품 3개(안테나, 배터리, 송신기)가 2·4·6일차 이후 하나씩 맵에 나타난다. 안테나는 가운데 링(60~130m), 배터리·송신기는 바깥 링(130~200m)의 후보 지점 중 무작위다 (core-rules §1·5, items §6)
- [ ] 부품은 E 1초로 채집하고, 가방 1칸을 차지하며 쌓이지 않는다 (items §6)
- [ ] 무전기에 부품 3개를 모두 설치하면 `RadioRepaired`가 발행된다. 수리 마감은 없다 (core-rules §1)
- [ ] 수리한 날부터 2일을 버티면 `RescueArrived`가 발행되고 승리한다
- [ ] 사망(`PlayerDied`)이나 구조 시 결과 화면이 뜬다: 생존 일수, 고른 증강, 사망 원인(동사·아사·늑대). "다시 하기"를 누르면 Game 씬이 다시 로드되고 증강은 사라진다 (core-rules §1)
- [ ] 결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 낮/밤·날짜는 RunFlow가 이미 가진 이벤트(002)를 구독해서 쓴다.
- 승패는 RunFlow가 `PlayerDied`·`RescueArrived`로 결정한다. 다른 시스템을 직접 참조하지 않는다 (code-structure §3·4).
- 재시작은 Game 씬 재로드만으로 한다 (tech-decisions 12).
- 사망 원인 중 "늑대"는 007이 `PlayerDamaged`에 원인을 실어 보낸 뒤 채워진다. 이 스토리는 원인 타입만 받으면 된다.
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 005: 증강 선택 자체 (결과 화면은 고른 증강을 보여주기만 한다)
- 007: 늑대 공격과 늑대 사망 원인 발생

---

## Files

- `Assets/Scripts/Systems/RunFlow/**`
- `Assets/Scripts/Systems/Resources/**` (무전 부품 정의·배치)
- `Assets/Scripts/UI/**` (결과 화면)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: None
