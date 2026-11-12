# Story 007: 늑대 무리와 단순 공격

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
**Requirement**: `Brief MVP feature 6` (밤에 움직이는 늑대 무리와 단순 공격)
**근거 문서**: `design/quick-specs/core-rules.md` §6 · `design/quick-specs/items.md` §2·3·4 · `docs/architecture/tech-decisions.md` 14·15·16 · `docs/architecture/code-structure.md` §4·6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 6, scoped to this story, with the values from the 근거 문서. 모든 수치는 임시값(플레이 후 조정). 단위: m, m/s.*

- [ ] 적은 `EnemyDefinition` ScriptableObject + `IEnemyBehaviour` + `IEnemySpawnRule`로 만들어, 새 적을 기존 코드 수정 없이 추가할 수 있다 (code-structure §6)
- [ ] 맵 생성 시 늑대 무리 중심지가 스폰된다: 무리 2~3개, 무리당 2마리(임시값). 중심지는 가운데 링(60~130m) 안 무작위 (core-rules §6)
- [ ] 낮에는 배회 상태의 '낮 모드'로 중심지에서 반경 3m 안에서 idle 또는 1 m/s로 걷는다. 플레이어가 4m 안에 오거나 늑대를 공격하면 추적·공격한다 (core-rules §6)
- [ ] 추적 중인 늑대는 아침(`DayStarted`)이 와도 추적·공격을 계속한다. 25m 이상 멀어지거나 8초 동안 놓치면(감지 거리 밖이거나 나무·바위·지형에 시야가 가려짐) 추적을 멈추고 무리 중심지로 돌아간다. 처치한 늑대는 다시 생기지 않는다 (core-rules §6)
- [ ] 밤(`NightStarted`)의 상태 5단계가 core-rules §6 표대로 전환된다: 배회 2 m/s(무리 중심지 주변) → 감지 12m(눈보라 8m) → 추적 4.5 m/s → 공격 1.5m(물기 체력 -15, 쿨다운 1.5초) → 포기·후퇴(25m 이상 멀어지거나 8초 놓치면 무리 중심지로 귀환)
- [ ] 불빛에 닿으면 즉시 후퇴하고, 불빛 범위 안으로 들어오지 않는다: 켜진 모닥불은 온기 반경(기본 4m, 큰 불 증강 6m), 횃불은 4m (core-rules §6, items §4)
- [ ] 길찾기는 NavMesh. 상태 전환 규칙은 Threats Logic(순수 C#), 이동은 View의 NavMeshAgent (tech-decisions 15)
- [ ] 시야 가림은 View가 늑대 눈 높이에서 플레이어까지 Linecast로 판정하고('시야 차단' 레이어 = 나무·바위·지형만, 0.2초 주기 임시값), 보임/가림 결과만 Logic에 넘긴다. 설치물·모닥불·바닥 아이템은 가리지 않는다. 시야가 가려진 플레이어는 처음부터 감지하지 못한다 (tech-decisions 16, core-rules §6)
- [ ] 플레이어 공격: 좌클릭, 근접 1.5m, 0.6초 간격, 마우스 방향. 늑대는 3번 맞으면 쓰러지고 날고기 1 + 가죽 1을 떨어뜨린다 (core-rules §6, items §2·3)
- [ ] 물기는 `PlayerDamaged`를 발행하고 사망 원인에 "늑대"가 실린다 (code-structure §4)
- [ ] 늑대가 움직이는 밤 장면과 모닥불 빛 경계 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 모든 수치(속도, 거리, 시간, 피해량)는 ScriptableObject 데이터다.
- 켜진 모닥불 목록과 반경은 Camp가 이벤트(`FireStateChanged`)로 알린 것을 Threats가 구독한다. 횃불 범위는 008이 Core 인터페이스(빛 영역)로 알린다. Threats는 Camp·Resources를 직접 참조하지 않는다 (code-structure §3·4).
- 큰 불 증강의 반경은 모닥불 반경 스탯을 그대로 읽으면 된다. 늑대 쪽에 따로 값을 두지 않는다.
- 낮/밤 차이는 별도 상태가 아니라 배회 상태의 모드다. 모드별 반경·속도·감지 거리는 `EnemyDefinition` 데이터로 둔다.
- 문서에 없는 동작은 만들지 않고 보고한다.

---

## Out of Scope

- 008: 창 공격력, 횃불 아이템 자체
- 사냥 시스템, 곰 등 다른 적 (브리프 later)
- 설피와 늑대 속도의 밸런스 (core-rules 밸런스 TODO)

---

## Files

- `Assets/Scripts/Systems/Threats/**`
- `Assets/Scripts/Systems/Player/**` (공격)
- `Assets/Scripts/Core/Contracts/**` (`PlayerDamaged`, 빛 영역 인터페이스)
- `Assets/Data/**` (`EnemyDefinition` 에셋, 가죽 정의)
- Game 씬 NavMesh 베이크

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 늑대가 보이는 밤 장면 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-003-hurdles-death.md`, `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
- Unlocks: `production/epics/whiteout/story-008-tools-equipment-crafting.md`
