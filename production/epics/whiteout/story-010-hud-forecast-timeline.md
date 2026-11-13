# Story 010: HUD 상단 배치와 예보 타임라인 막대

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief MVP feature 4` (낮/밤 주기와 날씨(3일 예보)) 의 HUD 표현 개선. UI 결정 D2 → T1~T7 (지영 확정 2026-11-13)
**근거 문서**: `design/ux/ui-plan.md` §1 원칙 7·8 · §7.1 레이아웃 · §7.2 정보 우선순위 · §7.3 [A]·[B]·[D] · §7.5 T1~T7 ·
`design/quick-specs/core-rules.md` §1(하루 180초 = 낮 120 + 밤 60, 정지 규칙, 구조 2일) · §3(날씨 4종, 3일 예보) ·
`docs/architecture/tech-decisions.md` 22·23(현재 HUD 임시값)·24(uGUI + TMP, 문구 데이터, Noto Sans KR)·25(검증 경로)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: LOW
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/ux/ui-plan.md` §7 and T1~T7, with the values from the 근거 문서:*

- [ ] **배치:** 위쪽 가운데에 현재 기온, 그 오른쪽에 예보 타임라인 막대, 왼쪽 위에는 허들 게이지만 있다. 이전의 `오늘/내일/모레/기온` 글자 HUD(story-002)는 이 배치로 대체된다 (ui-plan §7.1, T1)
- [ ] **기온:** 플레이어가 지금 받는 환경 온도(core-rules §3 공식 전체)를 HUD에서 가장 큰 숫자로 표시한다(예: `-15°C`). 모닥불·링이 바뀌면 숫자가 바로 바뀐다 (ui-plan §7.3 [A])
- [ ] **범위:** 막대는 지금부터 3일(게임 시간 540초) 너비를 보여 준다. 오늘·내일·모레는 항상 보이고, 예보가 없는 그 뒤 구간은 날씨 없이 흐린 `?` 구간으로 그린다. 아침에 예보가 밀리면(`WeatherForecastUpdated`) `?` 구간이 새 모레의 날씨로 바뀐다 (T2, core-rules §3)
- [ ] **흐름:** 왼쪽 끝이 '지금'(고정 표시)이고, 막대 내용이 게임 시간에 맞춰 오른쪽에서 왼쪽으로 계속 흐른다. 게임이 정지하면(증강 선택·일시정지, core-rules §1) 막대도 멈춘다 (ui-plan §7.3 [B])
- [ ] **눈금:** 큰 눈금 = 하루 시작에 날짜 숫자(현재 날은 `3일차`처럼 글자, 다음 날부터 `4`, `5`), 작은 눈금 = 밤 시작 (T3)
- [ ] **낮/밤:** 하루 구간을 실제 길이 비율(낮 120초 : 밤 60초 = 2 : 1)로 나눠 낮은 밝은 바탕, 밤은 어두운 바탕(남색)으로 칠한다 (T4, core-rules §1)
- [ ] **날씨 아이콘:** 하루 구간마다 그 날 날씨 아이콘 1개(맑음·눈·눈보라·한파 4종, 맑음 포함). 눈보라·한파는 위험 색 테두리로 강조한다. 아이콘에 마우스를 올리면 날씨 이름과 보정값 툴팁 (T5, ui-plan §7.3 [B])
- [ ] **구조 도착 깃발:** 구조를 요청하면(`RescueRequested`) 도착 시점(요청 + 360초)에 깃발이 막대 위에 표시되고 막대와 함께 흐른다 (T7, core-rules §1)
- [ ] **표시하지 않는 것:** 신호기 부품 등장일(2·4·6일차)은 막대에 표시하지 않는다 (ui-plan §7.3 [B], D8)
- [ ] **글자:** 모든 HUD 글자는 TextMeshPro + Noto Sans KR 폰트 에셋. 레거시 `Text`를 쓰는 HUD(날씨 글자·허들 게이지)도 TMP로 바꾼다 (tech-decisions 24)
- [ ] **문구 데이터:** 화면 문구(`일차`, 날씨 이름, 툴팁 등)는 코드에 쓰지 않고 데이터에서 읽는다. 기존 `WeatherHud.cs` 기본 문구도 옮긴다 (tech-decisions 24, story-002 Deviations)
- [ ] **값 데이터:** 막대 범위(540초)·크기·색은 설정 에셋 값이고 코드에 없다 (tech-decisions 1)
- [ ] **증거:** 아래 장면의 Game View 스크린샷을 `production/qa/evidence/story-010-hud-forecast-timeline/`에 남긴다: 낮(밤 구간이 다가오는 모습), 밤, 예보에 눈보라 또는 한파가 있는 날, `?` 구간이 보이는 모습, 구조 깃발 (tech-decisions 25)

---

## Implementation Notes

- 막대는 RunFlow의 현재 시간·날짜와 Environment의 예보를 읽기만 한다. UI가 두 시스템을 직접 참조하지 않고 이벤트(`DayStarted`·`NightStarted`·`WeatherForecastUpdated`·`RescueRequested`)와 Core 조회 인터페이스로 받는다 (code-structure §3·4).
- 예보 데이터는 3일치뿐이다. 그보다 먼 날의 날씨를 미리 뽑거나 추정해서 보여 주지 않는다 (core-rules §3).
- 문구 데이터 형식은 이 스토리가 처음 적용하는 경우 여기서 정하고 tech-decisions 24번에 기록한다 (선택지를 보고하고 지영 확인).
- Noto Sans KR은 SIL Open Font License이다. 폰트 파일과 라이선스 파일을 함께 넣는다.
- 정지 기능(005 증강 선택, 009 일시정지)이 아직 없으면, 막대가 게임 시간을 따르는지를 에디터에서 시간을 멈춰 확인한다.
- 구조 요청(006)이 아직 없으면, 깃발은 MCP로 `RescueRequested`를 발행해 확인한다.
- 문서에 없는 결정이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 가방 칸·모닥불 연료 HUD (story-004)
- 증강 아이콘 줄·장비 칸 (005·008)
- 기온 원인 분해 표시 (D12, MVP 이후)
- 알림 줄([E])과 위험 날씨 알림 문구 `모레: 한파` — 알림 시스템이 생기는 스토리에서 연결
- 화면 전체 피드백(서리 비네트 등, ui-plan §7.4)

---

## Files

- `Assets/Scripts/UI/**` (기온 표시, 타임라인 막대, 기존 `WeatherHud.cs` 대체)
- `Assets/Scripts/Systems/Survival/Unity/View/SurvivalHud.cs` (TMP 전환, 배치) — story-004 진행 중 변경과 겹치지 않게 시작 전에 확인
- `Assets/Scenes/Game.unity` (HUD 배치)
- `Assets/Data/UI/**` (막대 설정, 문구 데이터)
- Noto Sans KR TMP 폰트 에셋 + 라이선스

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: UI
**Test**: waived at qa.level minimal
**Required evidence**: AC '증거'의 스크린샷 5장. MCP로 에디터 Play Mode를 돌리고 Game View를 찍는다 (tech-decisions 18·21·25)
**지영 플레이 확인 필요** (tech-decisions 25): 막대 흐름 속도와 읽기 쉬움, 기온 숫자 크기, 밤 구간이 다가오는 느낌, 위험 날씨 강조가 눈에 띄는지. 확인 전까지 크기·색·강조는 임시값.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-002-time-weather-temperature.md` (시간·예보 이벤트), `production/epics/whiteout/story-003-hurdles-death.md` (허들 게이지)
- 작업 순서: story-004(진행 중)와 같은 HUD·씬 파일을 고치므로 004가 끝난 뒤 시작한다
- Unlocks: None
