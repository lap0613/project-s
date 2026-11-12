# Story 006: 구조 신호기·부품·구조·결과 화면

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

- [ ] 구조 신호기 부품 3개(안테나, 배터리, 송신기)가 2·4·6일차 이후 하나씩 맵에 나타난다. 안테나는 가운데 링(60~130m), 배터리·송신기는 바깥 링(130~200m)의 후보 지점 중 무작위. E 1초로 줍고, 가방 1칸, 쌓이지 않는다 (core-rules §1·5, items §6)
- [ ] 금속 조각 6개가 매 판 후보 지점 중 무작위로 맵에 배치된다(쌓임 5) (items §2)
- [ ] B로 구조 신호기 틀을 원하는 위치에 설치한다: 나무 4 + 금속 조각 2(임시값). 한 판에 1개만, 설치 후 옮길 수 없고, 늑대가 부술 수 없다 (core-rules §1, items §7)
- [ ] 틀에 E로 부품을 하나씩 장착한다. 3개를 모두 장착하면 완성되고 `SignalBeaconCompleted`가 발행된다 (core-rules §1)
- [ ] 완성된 신호기에 E로 구조를 요청하면 `RescueRequested`가 발행되고, 그 시점부터 2일(게임 시간 360초) 타이머가 시작된다. HUD에 구조까지 남은 시간이 보인다. 증강 선택 중에는 타이머도 멈춘다. 요청 마감은 없다 (core-rules §1)
- [ ] 타이머가 끝나면 `RescueArrived`가 발행되고 승리한다. 위치 제약은 없다
- [ ] 사망(`PlayerDied`)이나 구조 시 결과 화면이 뜬다: 생존 일수, 고른 증강, 사망 원인(동사·아사·늑대). "다시 하기"를 누르면 Game 씬이 다시 로드되고 증강은 사라진다 (core-rules §1)
- [ ] 신호기, 남은 시간 HUD, 결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 신호기 틀 설치와 부품 장착은 Camp 시스템, 구조 타이머와 승패는 RunFlow 시스템이다. 둘은 이벤트로만 통신한다 (code-structure §2·3·4).
- 신호기는 004의 구조물 정의(데이터)로 추가한다. 설치 시스템 코드를 고치지 않는다 (code-structure §6).
- 재시작은 Game 씬 재로드만으로 한다 (tech-decisions 12).
- 사망 원인 "늑대"는 007이 `PlayerDamaged`에 원인을 실어 보낸 뒤 채워진다. 이 스토리는 원인 타입만 받는다.
- 문서에 없는 규칙이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 005: 증강 선택 자체 (결과 화면은 고른 증강을 보여주기만 한다)
- 007: 늑대 공격과 늑대 사망 원인 발생
- 008: 금속 조각을 쓰는 도구 제작

---

## Files

- `Assets/Scripts/Systems/Camp/**` (신호기 틀, 부품 장착)
- `Assets/Scripts/Systems/RunFlow/**` (구조 타이머, 승패)
- `Assets/Scripts/Systems/Resources/**` (부품·금속 조각 정의와 배치)
- `Assets/Scripts/UI/**` (남은 시간 HUD, 결과 화면)
- `Assets/Data/**` (신호기 구조물 정의, 부품 정의)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 신호기·타이머 HUD·결과 화면 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: None
