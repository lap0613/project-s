# Story 002: 낮/밤·날씨(3일 예보)·환경 온도

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Core
> **Type**: Integration
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 4` (낮/밤 주기(시야)와 날씨(3일 예보)·시작 지점 거리 기반 환경 온도)
**근거 문서**: `design/quick-specs/core-rules.md` §1·3 · `docs/architecture/code-structure.md` §4(이벤트, `IAmbientTemperatureSource`, `IRandom`)·§7

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md` Features 4, scoped to this story, with the values from the 근거 문서:*

- [ ] 하루 180초 = 낮 120초 + 밤 60초. `DayStarted`·`NightStarted`·`DayEnded` 이벤트를 발행한다 (core-rules §1)
- [ ] 날씨는 아침마다 1회 결정되고 하루 동안 유지된다. 확률은 맑음 40 / 눈 30 / 눈보라 20 / 한파 10(%), 날짜와 무관하다. `IRandom`을 쓰며 같은 시드면 같은 결과다 (core-rules §3)
- [ ] 3일치(오늘·내일·모레)를 미리 뽑아 두고, 예보가 실제 날씨와 항상 일치한다. 아침마다 예보를 하루 밀고 `WeatherForecastUpdated`를 발행한다
- [ ] 환경 온도 = -10 + 날씨(0 / -5 / -15 / -25) + 거리 링 + 모닥불 온기 + 증강 보정 (core-rules §3)
- [ ] 거리 링(시작 지점 기준): 0~60m +5 / 60~130m -5 / 130~200m -15. 경계값(60m, 130m)이 한쪽으로 일관되게 처리된다
- [ ] 모닥불 온기는 `IAmbientTemperatureSource` 같은 Core 인터페이스로 받는다(켜진 불 중 가장 따뜻한 1개). 불을 만드는 쪽은 004가 연결한다. 증강 보정은 스탯 수정자 슬롯으로 둔다
- [ ] 시야: 밤에는 플레이어 주변 12m와 모닥불 빛만 보이고, 눈보라는 안개로 약 15m 너머가 흐려진다. 둘이 겹치면 더 좁은 쪽 (core-rules §3)
- [ ] 장소 구역(숲·설원)은 만들지 않는다. 장소에 따른 온도는 거리 링뿐이다 (core-rules §3)
- [ ] HUD에 오늘·내일·모레 날씨와 현재 기온이 보이고, 스크린샷을 `production/qa/evidence/`에 남긴다

---

## Implementation Notes

- 시간·날씨·온도 규칙은 순수 C#(Whiteout.Environment, Whiteout.RunFlow Logic)로 두고, 거리·시야·조명은 View가 계산해 넘긴다 (tech-decisions 13).
- 낮/밤 주기는 RunFlow, 날씨와 온도는 Environment 시스템. 둘은 서로 참조하지 않고 이벤트로만 통신한다 (code-structure §3·4).
- 날씨 확률·온도 값은 모두 ScriptableObject (`WeatherDefinition` 에셋, 새 날씨는 에셋 추가만으로 가능).
- 문서에 없는 결정(예: 연속 날씨 제한)은 만들지 말고 보고한다. core-rules §3은 "플레이 후 판단"이라고 되어 있다.

---

## Out of Scope

- 001: 이동·맵·기반
- 003: 체온 등 허들이 이 온도를 어떻게 쓰는지
- 004: 모닥불 자체와 온기 값 공급
- 005: 증강의 실제 효과

---

## Files

- `Assets/Scripts/Systems/Environment/**`
- `Assets/Scripts/Systems/RunFlow/**`
- `Assets/Scripts/Core/Contracts/**` (`IAmbientTemperatureSource`, 이벤트)
- `Assets/Scripts/UI/**` (날씨·기온 HUD)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: Integration
**Test**: waived at qa.level minimal
**Required evidence**: HUD가 보이는 플레이 스크린샷을 `production/qa/evidence/`에 남긴다

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-001-foundation-movement-map.md`
- Unlocks: `production/epics/whiteout/story-003-hurdles-death.md`, `production/epics/whiteout/story-004-items-bag-structures-cooking.md`
