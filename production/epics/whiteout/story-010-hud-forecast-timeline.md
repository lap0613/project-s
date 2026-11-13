# Story 010: HUD 상단 배치와 예보 타임라인 막대

> **Epic**: whiteout
> **Status**: Complete
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: M
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-13

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

- [x] **배치:** 위쪽 가운데에 현재 기온, 그 오른쪽에 예보 타임라인 막대, 왼쪽 위에는 허들 게이지만 있다. 이전의 `오늘/내일/모레/기온` 글자 HUD(story-002)는 이 배치로 대체된다 (ui-plan §7.1, T1)
- [x] **기온:** 플레이어가 지금 받는 환경 온도(core-rules §3 공식 전체)를 HUD에서 가장 큰 숫자로 표시한다(예: `-15°C`). 모닥불·링이 바뀌면 숫자가 바로 바뀐다 (ui-plan §7.3 [A])
- [x] **범위:** 막대는 지금부터 3일(게임 시간 540초) 너비를 보여 준다. 오늘·내일·모레는 항상 보이고, 예보가 없는 그 뒤 구간은 날씨 없이 흐린 `?` 구간으로 그린다. 아침에 예보가 밀리면(`WeatherForecastUpdated`) `?` 구간이 새 모레의 날씨로 바뀐다 (T2, core-rules §3)
- [x] **흐름:** 왼쪽 끝이 '지금'(고정 표시)이고, 막대 내용이 게임 시간에 맞춰 오른쪽에서 왼쪽으로 계속 흐른다. 게임이 정지하면(증강 선택·일시정지, core-rules §1) 막대도 멈춘다 (ui-plan §7.3 [B])
- [x] **눈금:** 큰 눈금 = 하루 시작에 날짜 숫자(현재 날은 `3일차`처럼 글자, 다음 날부터 `4`, `5`), 작은 눈금 = 밤 시작 (T3)
- [x] **낮/밤:** 하루 구간을 실제 길이 비율(낮 120초 : 밤 60초 = 2 : 1)로 나눠 낮은 밝은 바탕, 밤은 어두운 바탕(남색)으로 칠한다 (T4, core-rules §1)
- [x] **날씨 아이콘:** 하루 구간마다 그 날 날씨 아이콘 1개(맑음·눈·눈보라·한파 4종, 맑음 포함). 눈보라·한파는 위험 색 테두리로 강조한다. 아이콘에 마우스를 올리면 날씨 이름과 보정값 툴팁 (T5, ui-plan §7.3 [B])
- [x] **구조 도착 깃발:** 구조를 요청하면(`RescueRequested`) 도착 시점(요청 + 360초)에 깃발이 막대 위에 표시되고 막대와 함께 흐른다 (T7, core-rules §1)
- [x] **표시하지 않는 것:** 신호기 부품 등장일(2·4·6일차)은 막대에 표시하지 않는다 (ui-plan §7.3 [B], D8)
- [x] **글자:** 모든 HUD 글자는 TextMeshPro + Noto Sans KR 폰트 에셋. 레거시 `Text`를 쓰는 HUD(날씨 글자·허들 게이지)도 TMP로 바꾼다 (tech-decisions 24)
- [x] **문구 데이터:** 화면 문구(`일차`, 날씨 이름, 툴팁 등)는 코드에 쓰지 않고 데이터에서 읽는다. 기존 `WeatherHud.cs` 기본 문구도 옮긴다 (tech-decisions 24, story-002 Deviations)
- [x] **값 데이터:** 막대 범위(540초)·크기·색은 설정 에셋 값이고 코드에 없다 (tech-decisions 1)
- [x] **증거:** 아래 장면의 Game View 스크린샷을 `production/qa/evidence/story-010-hud-forecast-timeline/`에 남긴다: 낮(밤 구간이 다가오는 모습), 밤, 예보에 눈보라 또는 한파가 있는 날, `?` 구간이 보이는 모습, 구조 깃발 (tech-decisions 25)

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

- `Assets/Scripts/UI/WeatherHud.cs` — 다시 씀 (기온 판 + 타임라인 조립, 이전 예보 글자 제거)
- `Assets/Scripts/UI/ForecastTimelineView.cs` — 새 파일 (막대, 눈금, 낮/밤, 아이콘, `?` 구간, 깃발, 툴팁)
- `Assets/Scripts/UI/WeatherIconSprites.cs` — 새 파일 (코드로 그린 날씨 아이콘 4종)
- `Assets/Scripts/UI/WeatherIconHover.cs` — 새 파일 (아이콘 마우스 올림)
- `Assets/Scripts/UI/WeatherStyle.cs` — 새 파일 (날씨 id별 모양·색·위험 여부)
- `Assets/Scripts/UI/HudConfig.cs`, `Assets/Data/UI/HudConfig.asset` — 기온 판·타임라인 값과 색
- `Assets/Data/UI/UiTextTable.asset` — 타임라인·날씨 문구 추가, `hud.forecast` 삭제
- `Assets/Scripts/Core/Contracts/IDayTimeSource.cs` — 새 파일 (게임 시간 조회)
- `Assets/Scripts/Core/Contracts/Events/RescueRequested.cs` — 새 파일 (구조 요청 이벤트)
- `Assets/Scripts/Systems/RunFlow/Logic/DayClock.cs` — `IDayTimeSource` 구현
- `Assets/Scripts/Game/GameLifetimeScope.cs` — `DayClock`을 `IDayTimeSource`로도 등록
- `Assets/Scenes/Game.unity` — 이전 예보 글자 오브젝트 삭제
- `docs/architecture/tech-decisions.md` 29번, `docs/architecture/code-structure.md` §4

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: UI
**Test**: waived at qa.level minimal
**Required evidence**: AC '증거'의 스크린샷 5장. MCP로 에디터 Play Mode를 돌리고 Game View를 찍는다 (tech-decisions 18·21·25)
**지영 플레이 확인 필요** (tech-decisions 25): 막대 흐름 속도와 읽기 쉬움, 기온 숫자 크기, 밤 구간이 다가오는 느낌, 위험 날씨 강조가 눈에 띄는지. 확인 전까지 크기·색·강조는 임시값.

**Status**: [x] Retained — `production/qa/evidence/story-010-hud-forecast-timeline/` (screenshots 5, stamp.txt)

---

## Dependencies

- Depends on: `production/epics/whiteout/story-002-time-weather-temperature.md` (시간·예보 이벤트), `production/epics/whiteout/story-003-hurdles-death.md` (허들 게이지)
- 작업 순서: story-004(진행 중)와 같은 HUD·씬 파일을 고치므로 004가 끝난 뒤 시작한다
- Unlocks: None

## Completion Notes
**Completed**: 2026-11-13
**Criteria**: 13/13 passing. 에디터 Play Mode에서 실행해 확인 (tech-decisions 25번). 흐름 속도·읽기 쉬움·숫자 크기·강조는 지영 플레이 확인 필요.
**실행으로 확인한 것**: 기온 판 정가운데 위 + 오른쪽 타임라인 + 왼쪽 위 허들만, 낮 2 : 밤 1 바탕, 하루 시작 큰 눈금과 `1일차`·`2`·`3`, 밤 시작 작은 눈금,
날씨 아이콘(맑음·눈·눈보라)과 눈보라의 빨간 테두리, 툴팁 `눈보라 -15°C`, `?` 구간과 아침에 새 모레 날씨로 바뀌는 것, 구조 깃발(요청 + 360초 위치),
시계를 멈추면 게임 시간이 그대로인 것(80초 유지), 기온 숫자 색(-10°C 흰색, -20°C 하늘색).
**실행으로 확인하지 못한 것**: 한파 아이콘(이번 실행의 예보에 나오지 않음), 실제 마우스로 올린 툴팁(포인터 진입 이벤트를 보내 확인, 아이콘이 레이캐스트 첫 대상인 것은 확인),
모닥불 옆에서 기온 숫자가 바뀌는 것(이번 실행에서는 날씨로 바뀌는 것만 봄, 같은 조회 값은 story-004에서 30°C로 확인), 깃발이 막대와 함께 흐르는 모습(위치 계산만 확인),
1920×1080 외 해상도.
**시험 조건**: `DayClock`을 시험용으로 멈추고 앞으로 돌려 장면을 만들었다. 구조 요청(006)이 없어 `RescueRequested(360)`을 MCP로 발행했다.
**Deviations**:
- ADVISORY: Core에 `IDayTimeSource`와 이벤트 `RescueRequested`를 추가했다. UI가 게임 시간을 읽을 통로와 이 이벤트가 코드에 없었다 (code-structure §4에 기록).
- ADVISORY: `DayStarted`·`NightStarted`는 구독하지 않는다. 같은 경계가 `IDayTimeSource`의 시간에서 나온다.
- ADVISORY: ui-plan [B] 피드백의 [가정] 3개(밤 15초 전 깜빡임, 위험 날씨 맥박, 새 모레 1초 강조)와 알림 `모레: 한파`는 만들지 않았다 (AC에 없음, 알림은 Out of Scope).
- ADVISORY: `SurvivalHud.cs`는 story-004에서 이미 TextMeshPro였고 기온 글자도 없어 고치지 않았다.
- OUT OF SCOPE: `Assets/Scripts/Core/Contracts/**`, `Assets/Scripts/Systems/RunFlow/Logic/DayClock.cs`, `Assets/Scripts/Game/GameLifetimeScope.cs`. 스토리의 Files 목록에 없던 경로다.
**Engine check** (unity-ui-specialist, 엔진 참고 문서와는 대조하지 않음): MUST 0. 수정함: 바뀐 것이 없으면 막대를 다시 그리지 않음, 툴팁 문구를 매 프레임 만들지 않음,
아이콘이 사라진 칸의 올림 상태 지움, 기온 글자 레이캐스트 끔, 아이콘 스프라이트 FullRect. 남김: 타임라인을 별도 Canvas로 나누지 않음, 아이콘 텍스처의 HideAndDontSave.
**Test Evidence**: waived at qa.level minimal. 스크린샷 5장 + stamp.txt: `production/qa/evidence/story-010-hud-forecast-timeline/`
**Run result**: OBSERVED — 기온 판(-10°C) 오른쪽에 낮/밤 구간·날짜 숫자·날씨 아이콘·`?` 구간이 있는 타임라인 막대 (`production/qa/evidence/story-010-hud-forecast-timeline/01-day-night-approaching.png`)
**Reviewed by**: self, no independent review

## Calls beyond the design
모두 임시값, 지영 확인 필요 (`docs/architecture/tech-decisions.md` 29번). PM 운영 규칙에 따라 임시로 유지하고 기록했다.
- 크기: 기온 판 225×96, 글자 60, 막대 630×60, 아이콘 30, 여백 18 (1920×1080 기준, `Assets/Data/UI/HudConfig.asset`) · kept 2026-11-13
- 기온 판은 둥근 판이 아니라 반투명 검은 사각형, 기온은 정수 표시 (`WeatherHud.cs`, `UiTextTable.asset`) · kept 2026-11-13
- 색: 낮 회청색, 밤 남색, `?` 흐린 회색, 지금 표시 노랑, 위험 테두리 빨강, 깃발 초록 (`HudConfig.asset`) · kept 2026-11-13
- 날씨 아이콘은 코드로 그린 도형(해·눈송이·바람 줄·온도계) (`WeatherIconSprites.cs`) · kept 2026-11-13
- 현재 날 글자·아이콘은 밤까지 왼쪽 끝에 붙어 있다가 밤이 끝날 무렵 밀려 나감, 밤 위에서는 밝은 글자 (`ForecastTimelineView.cs`) · kept 2026-11-13
- `?` 구간은 낮/밤 구분 없는 한 덩어리 + 날짜 숫자 (`ForecastTimelineView.cs`) · kept 2026-11-13
- 툴팁은 아이콘 아래 검은 판, `눈보라 -15°C` 형식 (`UiTextTable.asset` `hud.timeline.tooltip`) · kept 2026-11-13
- 지금 표시는 세로선 + 마름모, 깃발은 깃대 + 사각 깃 (`ForecastTimelineView.cs`) · kept 2026-11-13
- `RescueRequested`는 도착까지 남은 초를 값으로 가짐, 360초는 발행 쪽이 넣음 (`RescueRequested.cs`) · kept 2026-11-13
