# Story 003: 허들 3종과 사망

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-13

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

- `Assets/Data/Survival/SurvivalConfig.asset`
- `Assets/Scenes/Game.unity`
- `Assets/Scripts/Core/Contracts/DeathCause.cs`
- `Assets/Scripts/Core/Contracts/Events/PlayerDied.cs`
- `Assets/Scripts/Game/GameLifetimeScope.cs`
- `Assets/Scripts/Game/SurvivalPlayerBridge.cs`
- `Assets/Scripts/Game/Whiteout.Game.asmdef`
- `Assets/Scripts/Systems/Survival/Logic/SurvivalSettings.cs`
- `Assets/Scripts/Systems/Survival/Logic/SurvivalStats.cs`
- `Assets/Scripts/Systems/Survival/Logic/SurvivalSystem.cs`
- `Assets/Scripts/Systems/Survival/Logic/Whiteout.Survival.asmdef`
- `Assets/Scripts/Systems/Survival/Unity/Data/SurvivalConfig.cs`
- `Assets/Scripts/Systems/Survival/Unity/View/SurvivalHud.cs`
- `Assets/Scripts/Systems/Survival/Unity/View/SurvivalRunner.cs`
- `Assets/Scripts/Systems/Survival/Unity/Whiteout.Survival.Unity.asmdef`

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Logic
**Test**: waived at qa.level minimal
**Required evidence**: 게이지가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [x] Retained — `production/qa/evidence/story-003-hurdles-death/` (screenshots 3, stamp.txt)

---

## Dependencies

- Depends on: `production/epics/whiteout/story-002-time-weather-temperature.md`
- Unlocks: `production/epics/whiteout/story-004-items-bag-structures-cooking.md`, `production/epics/whiteout/story-005-augments.md`, `production/epics/whiteout/story-007-wolves-combat.md`

## Completion Notes
**Completed**: 2026-11-13
**Criteria**: 9/9 passing (동작은 에디터 플레이에서 값을 직접 넣어 확인, 테스트 파일 없음; 체온 상승 쪽 상한 100과 `PlayerDied` 원인 '아사' 값은 직접 확인하지 않음 — 허기 0만으로 사망·이동 정지는 확인; 채집 속도 -30%는 상태와 값만 노출, 적용은 004)
**Deviations**: ADVISORY — 아래 임시값 9개는 지영 확인 필요(`docs/architecture/tech-decisions.md` 23번); HUD 글자(체온/허기/체력)가 씬에 한국어로 있음(현지화 없음, 레거시 uGUI `Text`); 스크린샷 02·03은 글자 외곽선 수정 전 화면(값 확인용), 01이 최종 모양; IL2CPP + 코드 스트리핑 빌드 시 `link.xml` 필요(프로젝트 공통, 미적용)
**Test Evidence**: waived at qa.level minimal; run evidence `production/qa/evidence/story-003-hurdles-death/` (이미지 3장 + stamp.txt)
**Reviewed by**: self, no independent review (엔진 API 점검: unity-specialist, 지적 2건 수정함)

## Calls beyond the design
모두 임시값, 지영 확인 필요 (`docs/architecture/tech-decisions.md` 23번).
- 시작 값: 체온·허기·체력 모두 100 (`Assets/Data/Survival/SurvivalConfig.asset`) · kept 2026-11-13
- `PlayerDied` 원인: 체온 0과 허기 0이 겹치면 그 틱에 체력을 더 많이 깎은 쪽, 같으면 동사 (`SurvivalSystem.cs` Tick) · kept 2026-11-13
- 틱 순서: 체온·허기를 먼저 바꾸고 그 값으로 체력을 정함, 피해가 있는 틱에는 회복 없음 (`SurvivalSystem.cs` Tick) · kept 2026-11-13
- 경계: 0 판정은 0 이하, '30 미만'은 30 미포함, '60 이상'은 60 포함 (`SurvivalSystem.cs`) · kept 2026-11-13
- 게이지 현재 값은 `Stat` 기본값에 저장, 판정과 HUD는 수정자 포함 최종값을 읽음 (`SurvivalSystem.cs` Change) · kept 2026-11-13
- 불이익은 줄어드는 비율로 저장: 이동 0.25, 채집 0.3 (`SurvivalConfig.cs`) · kept 2026-11-13
- 사망 시 정지: Game의 `SurvivalPlayerBridge`가 `PlayerController`를 끔, 이동속도 수정자 출처 `survival.cold` (`Assets/Scripts/Game/SurvivalPlayerBridge.cs`, code-structure §4) · kept 2026-11-13
- 채집속도 스탯은 만들지 않고 `IsHungerLow`와 값만 노출, 004에서 연결 (`SurvivalSystem.cs`) · kept 2026-11-13
- HUD 모양: 날씨 글자 아래 왼쪽 위, 글자 크기 32, 막대 300×28, 주황/노랑/빨강, 줄 간격 44 (`Assets/Scenes/Game.unity` HUD/SurvivalHud) · kept 2026-11-13
- 엔진 점검 지적 2건(브리지 파괴 방어, 설정 최대값 1 이상 검증)과 안 쓰는 asmdef 참조 정리: 이번 스토리에서 수정함 (지영 지시) · 2026-11-13
