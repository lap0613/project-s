# Story 001: 기반 + 이동 + 설원 맵

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Foundation
> **Type**: Integration
> **Estimate**: L
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 1` (탑다운 이동과 설원 맵 1개)
**근거 문서**: `docs/architecture/tech-decisions.md` 4·5·8~12 · `docs/architecture/code-structure.md` §2·3·4·5 · `design/quick-specs/core-rules.md` §3·5

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 1, scoped to this story, with the values from the 근거 문서:*

- [ ] VContainer 1.19.0이 `Packages/manifest.json`에 git URL + 태그로 설치되고, 컴파일 오류가 없고, 간단한 주입이 동작한다 (tech-decisions 9)
- [ ] 이 스토리에서 쓰는 어셈블리가 code-structure §2·3대로 만들어진다: Whiteout.Core(무참조), Core.Unity, Input, Player(Logic/Unity), Game. 시스템 Logic은 `noEngineReferences: true`. 나머지 시스템 어셈블리는 해당 스토리에서 같은 규칙으로 추가
- [ ] Core에 `IEventBus`(구독 해제는 `IDisposable`), `IRandom`(시드 지정 가능), 스탯 + 수정자(최종값 = (기본값 + Σ더하기) × (1 + Σ곱하기), 수정자마다 출처 기록)가 최소 구현으로 있다 (code-structure §4·5)
- [ ] Bootstrap 씬이 Game 씬을 추가로 불러오고, Game 씬 재로드로 재시작된다 (tech-decisions 12)
- [ ] WASD로 이동한다: Input System + CharacterController, 이동속도 초당 4m, 물리에 밀리지 않는다 (core-rules §5, tech-decisions 5·10)
- [ ] 카메라는 Cinemachine, 60도 고정 탑다운, 플레이어 추적 (tech-decisions 11)
- [ ] 설원 맵 1개: 맵 중심에서 반경 약 200m, 거리 링 경계(60m, 130m)를 알 수 있다(디버그 표시 가능), 장애물에 막힌다. 플레이어는 맵 중심 0~60m 링 안의 무작위 지점에서 시작한다 (core-rules §3·5)
- [ ] 플레이어 프리팹은 루트 = 로직, 자식 = 기본 도형 모델이다 (tech-decisions 4)
- [ ] 플레이 모드로 실행해 화면 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- Logic은 `UnityEngine`을 쓰지 않는다. `Mathf`·`Vector3`·`Debug` 대신 `System.Math`와 자체 타입 (code-structure §7).
- 시간은 `Tick(dt)`로만 진행한다.
- 조합 루트는 VContainer `LifetimeScope`. Core와 Logic은 VContainer를 참조하지 않고 생성자 주입만 쓴다 (code-structure §3).
- 모든 수치는 ScriptableObject로 둔다 (tech-decisions 1). 이동속도도 데이터.
- 문서에 없는 결정이 필요하면 구현하지 말고 보고한다.

---

## Out of Scope

- 002: 낮/밤·날씨·환경 온도
- 003: 허들과 사망
- 004: 아이템·가방·설치
- 이 스토리의 맵은 빈 지형과 장애물이다. 자원 배치는 004 이후.

---

## Files

- `Packages/manifest.json`
- `Assets/Scripts/Core/**`, `Assets/Scripts/Core.Unity/**`
- `Assets/Scripts/Systems/Player/**`, `Assets/Scripts/Input/**`, `Assets/Scripts/Game/**`
- `Assets/Scenes/Bootstrap.unity`, `Assets/Scenes/Game.unity`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 플레이 스크린샷을 `production/qa/evidence/`에 남긴다 (코딩 표준: 파싱 확인은 실행이 아니다)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/whiteout/story-002-time-weather-temperature.md`
