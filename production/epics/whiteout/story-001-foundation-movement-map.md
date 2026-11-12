# Story 001: 기반 + 이동 + 설원 맵

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Integration
> **Estimate**: L
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-12

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
- [ ] Core에 `IEventBus`(구독 해제는 `IDisposable`), `IRandom`(시드 지정 가능), 스탯 + 수정자(최종값 = (기본값 + Σ더하기) × max(1 + Σ곱하기, 0.1), 수정자마다 출처 기록)가 최소 구현으로 있다 (code-structure §4·5)
- [ ] Bootstrap 씬이 Game 씬을 추가로 불러오고, Game 씬 재로드로 재시작된다 (tech-decisions 12)
- [ ] WASD로 이동한다: Input System + CharacterController, 이동속도 초당 4m, 물리에 밀리지 않는다 (core-rules §5, tech-decisions 5·10)
- [ ] 카메라는 Cinemachine, 60도 고정 탑다운, 플레이어 추적 (tech-decisions 11)
- [ ] 설원 맵 1개: 맵 중심에서 반경 약 200m, 거리 링 경계(60m, 130m)를 알 수 있다(디버그 표시 가능), 장애물에 막힌다. 안쪽 링에도 장애물이 있고, 플레이어는 맵 중심 0~60m 링 안에서 장애물과 겹치지 않는 무작위 지점에서 시작한다. 링 디버그 표시는 에디터·개발 빌드에서만 보인다 (core-rules §3·5, tech-decisions 19·20)
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

- `Assets/Scripts/Core.Unity/MapLayoutDefinition.cs`
- `Assets/Scripts/Core.Unity/StatIdAsset.cs`
- `Assets/Scripts/Core.Unity/Whiteout.Core.Unity.asmdef`
- `Assets/Scripts/Core/Contracts/IMapLayout.cs`
- `Assets/Scripts/Core/Contracts/IPlayerInput.cs`
- `Assets/Scripts/Core/Contracts/MapRing.cs`
- `Assets/Scripts/Core/Contracts/MapRingClassifier.cs`
- `Assets/Scripts/Core/Events/EventBus.cs`
- `Assets/Scripts/Core/Events/IEventBus.cs`
- `Assets/Scripts/Core/Random/IRandom.cs`
- `Assets/Scripts/Core/Random/SeededRandom.cs`
- `Assets/Scripts/Core/Stats/Stat.cs`
- `Assets/Scripts/Core/Stats/StatId.cs`
- `Assets/Scripts/Core/Stats/StatModifier.cs`
- `Assets/Scripts/Core/Stats/StatModifierKind.cs`
- `Assets/Scripts/Core/Whiteout.Core.asmdef`
- `Assets/Scripts/Editor/PlayModeRunInBackground.cs`
- `Assets/Scripts/Editor/Whiteout.Editor.asmdef`
- `Assets/Scripts/Game/BootstrapEntry.cs`
- `Assets/Scripts/Game/BootstrapLifetimeScope.cs`
- `Assets/Scripts/Game/CameraOccluder.cs`
- `Assets/Scripts/Game/CameraOcclusionFader.cs`
- `Assets/Scripts/Game/GameLifetimeScope.cs`
- `Assets/Scripts/Game/GameRestarter.cs`
- `Assets/Scripts/Game/GameSceneLoader.cs`
- `Assets/Scripts/Game/MapRingDebugView.cs`
- `Assets/Scripts/Game/Whiteout.Game.asmdef`
- `Assets/Scripts/Input/InputSystemPlayerInput.cs`
- `Assets/Scripts/Input/Whiteout.Input.asmdef`
- `Assets/Scripts/Systems/Player/Logic/PlanarVector.cs`
- `Assets/Scripts/Systems/Player/Logic/PlayerMoveLogic.cs`
- `Assets/Scripts/Systems/Player/Logic/PlayerStartPicker.cs`
- `Assets/Scripts/Systems/Player/Logic/PlayerStats.cs`
- `Assets/Scripts/Systems/Player/Logic/Whiteout.Player.asmdef`
- `Assets/Scripts/Systems/Player/Unity/Data/PlayerConfig.cs`
- `Assets/Scripts/Systems/Player/Unity/View/PlayerController.cs`
- `Assets/Scripts/Systems/Player/Unity/Whiteout.Player.Unity.asmdef`
- `Assets/Scenes/Bootstrap.unity`
- `Assets/Scenes/Game.unity`
- `Assets/Prefabs/Player.prefab`
- `Assets/Data/Map/MapLayout.asset`
- `Assets/Data/Player/PlayerConfig.asset`
- `Assets/Art/Placeholder/Snow.mat`, `ObstacleTree.mat`, `ObstacleRock.mat`, `ObstacleTreeFaded.mat`, `ObstacleRockFaded.mat`, `DebugLine.mat`
- `Assets/Settings/VContainerSettings.asset`
- `Packages/manifest.json` (VContainer 1.19.0, Cinemachine 3.1.4)
- `ProjectSettings/EditorBuildSettings.asset` (Bootstrap 0, Game 1)
- `ProjectSettings/ProjectSettings.asset` (Preloaded Assets에 VContainerSettings; 나머지는 Unity 6.3 형식 자동 갱신)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: 플레이 스크린샷을 `production/qa/evidence/`에 남긴다 (코딩 표준: 파싱 확인은 실행이 아니다)

**Status**: [x] Retained — `production/qa/evidence/story-001-foundation-movement-map/` (screenshots 6, stamp.txt)

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/whiteout/story-002-time-weather-temperature.md`

## Completion Notes
**Completed**: 2026-11-12
**Criteria**: 9/9 passing (AC-7의 "릴리스 빌드에서 링 디버그 표시가 꺼짐" 부분은 릴리스 빌드로 확인하지 않음: untested, advisory)
**Deviations**: None (AC-3 문구를 code-structure §5의 하한 ×0.1 공식에 맞춤). Out of scope 참고: `.mcp.json`(이 작업 외 변경), `Assets/Settings/Mobile_RPAsset.asset`(Unity 자동 저장), `Packages/packages-lock.json`(패키지 설치로 생성)
**Test Evidence**: waived at qa.level minimal (Integration). Screenshots: `production/qa/evidence/story-001-foundation-movement-map/`
**Reviewed by**: self, no independent review (Solo mode; unity-specialist 엔진 API 점검은 수행, 지적 2건 수정)

## Calls beyond the design
- 카메라 오프셋 (0, 17.32, −10), 시야각 40 (Game 씬 `CM Follow Player`) · kept 2026-11-12
- 장애물 크기: 나무 지름 3m, 바위 4~5 × 3 × 4~5m (Game 씬 `Obstacles`) · kept 2026-11-12
- 맵 가장자리: 보이지 않는 박스 벽 36개, 반경 201m, 높이 6m (Game 씬 `MapBoundary`) · kept 2026-11-12
- 플레이어 모델: 캡슐 + 방향 표시 큐브 (`Assets/Prefabs/Player.prefab`) · kept 2026-11-12
- 하강 속도 2 m/s, 모델 회전 720도/초 (`Assets/Data/Player/PlayerConfig.asset`) · kept 2026-11-12
- 시작 위치는 안쪽 링 안에서 면적 기준 균등 (`PlayerStartPicker.cs`) · kept 2026-11-12
- 링 경계: <60 안쪽, 60~<130 가운데, 130~200 바깥, >200 맵 밖 (`MapRingClassifier.cs`) · kept 2026-11-12
- 곱하기 수정자는 분수 표기, 0.15 = +15% (`StatModifier.cs`) · kept 2026-11-12
- 이벤트 버스: 핸들러 예외는 기록 후 계속, 발행 중 해제는 즉시 반영, 발행 중 구독은 다음 발행부터 (`EventBus.cs`) · kept 2026-11-12
- 난수 시드 기본 0 = 실행마다 시간에서 만듦 (Bootstrap 씬 `BootstrapLifetimeScope`) · kept 2026-11-12
- 카메라 가림 반투명 알파 0.3, 판정 반경 0.5m (tech-decisions 17) · kept 2026-11-12
- 템플릿 `InputSystem_Actions`의 Player/Move 재사용 (WASD 외 방향키·게임패드 바인딩 포함) · kept 2026-11-12
- 빌드 씬 목록: SampleScene 제외, Bootstrap(0)·Game(1) · kept 2026-11-12
- 안쪽 링 장애물 6개: 나무 4, 바위 2, 맵 중심에서 32~57m (Game 씬 `Obstacle_Tree_20`~`25`) · kept 2026-11-12
- 시작 위치 여유 0.5m, 시도 30번 (`PlayerConfig.asset`, tech-decisions 19) · kept 2026-11-12
- 맵 중심 반경 3m는 장애물 없음 (core-rules §5) · kept 2026-11-12
- 시작 위치 30번 실패 시 맵 중심에서 시작 (`PlayerStartPicker.PickFree`) · kept 2026-11-12
- 지영 결정으로 변경 반영: 안쪽 링 장애물 + 장애물 회피 시작(tech-decisions 19), 링 디버그 표시는 에디터·개발 빌드만(tech-decisions 20)
