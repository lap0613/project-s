# Story 005: 증강 3택 1 (16개)

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
**Requirement**: `Brief MVP feature 5` (하루 생존 시 증강 3택 1 (16개, 중복 없음))
**근거 문서**: `design/quick-specs/core-rules.md` §7·§1 · `docs/architecture/tech-decisions.md` 2 · `docs/architecture/code-structure.md` §5·6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 5, scoped to this story, with the values from the 근거 문서:*

- [ ] 증강은 `AugmentDefinition` ScriptableObject이고 카탈로그에 등록된 16개가 core-rules §7 표와 같다
- [ ] 하루가 끝난 아침(`DayEnded`)에 3개를 무작위로 보여주고 1개를 고른다. 이미 고른 증강은 다시 나오지 않고, 리롤은 없다
- [ ] 선택 화면이 떠 있는 동안 게임 시간이 정지한다(체온·허기·늑대·시간 모두)
- [ ] 수치형 11개는 스탯 수정자 묶음만으로 동작하고 코드가 따로 없다 (두꺼운 외투 +4°C, 방풍 망토 날씨 보정 절반, 털모자 거리 링 추위 절반, 마른 장작 요령 나무 연소 +40%, 큰 불 반경 4→6m, 빠른 손 채집 +30%, 큰 배낭 +3칸, 알뜰한 채집 20%로 1개 추가, 설피 이동속도 +15%, 밝은 눈 시야 +30%, 지도 감각 15m 자원 표시)
- [ ] 규칙형 ★ 5개가 각자의 코드로 동작한다: 마지막 온기(체온 0 이후 15초간 체력 감소 없음), 휴대용 화로(하루 1회 30초 동안 주변 1.5m +10°C), 숯불(꺼져도 30초간 반경 2m 온기), 썰매(운반량 ×2, 이동속도 -10%), 귀환 본능(하루 1회 가장 가까운 켜진 모닥불로 이동, 켜진 불이 없으면 사용 불가)
- [ ] 효과는 수정자 출처(어느 증강)를 기록해 합산되고, 장비 효과와도 합산된다 (code-structure §5)
- [ ] 새 증강을 추가할 때 기존 코드를 고치지 않는다: 수치형은 에셋만, ★은 효과 클래스 1개 + 에셋 (code-structure §6)
- [ ] `AugmentChosen`이 발행되고, 선택 화면 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 스탯 = 기본값 + 수정자 목록. 증강은 수정자를 추가하기만 한다 (tech-decisions 2).
- ★ 증강은 Whiteout.Augments.Effects 어셈블리에 모은다. 효과에 필요한 시스템 Logic만 참조한다 (code-structure §3).
- 다른 시스템 값(가방 칸, 시야, 온기 반경 등)은 해당 시스템이 노출한 스탯 ID로만 바꾼다. 그 시스템 코드를 고치지 않는다.
- 증강 뽑기는 `IRandom`을 쓴다.
- 문서에 없는 규칙(예: 후반에 3개가 안 남을 때)이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 사냥·제작 품질 증강 (브리프 Your systems의 later)
- 증강 리롤 (Out of scope)
- 007·008: 늑대·장비가 증강과 합산되는 값 자체

---

## Files

- `Assets/Scripts/Systems/Augments/**`
- `Assets/Scripts/Systems/Augments.Effects/**`
- `Assets/Scripts/UI/**` (선택 화면)
- `Assets/Data/Augments/**` (16개 에셋과 카탈로그)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 증강 선택 화면 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-003-hurdles-death.md`, `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: None

## 순서 메모 (지영 결정 2026-11-13)
- 이 스토리는 증강 방향 안(`design/proposals/augment-direction.md`, 제안)이 확정된 뒤에 시작한다.
  확정되면 `design/quick-specs/core-rules.md` §7과 이 스토리의 Acceptance Criteria(증강 개수·목록)를 먼저 고친다.
