# Story 012: 목표 안내와 안도 연출

> **Epic**: whiteout
> **Status**: Ready
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: S
> **Build step**: core
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: `Brief Goal & fail state`(구조 목표)를 첫날부터 보이게 하고, 타겟 감정 '긴장과 안도'의 안도 쪽 표현을 넣는다. 지영 결정 2026-11-13 (영상 분석 제안 4·6번)
**근거 문서**: `design/ux/ui-plan.md` §1 원칙 2·9 · §7.3 [C] · §7.4 · §7.6 (N1~N5) · §12 D8 ·
`design/quick-specs/core-rules.md` §1(구조 신호기 부품 3개) · §3(모닥불 온기 반경) ·
`docs/architecture/tech-decisions.md` 1(값은 데이터)·25(검증 경로)·27(문구 데이터)·29(HUD 상단 배치 값)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.28f1 | **Risk**: LOW
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/ux/ui-plan.md` §7.6. N 번호가 붙은 값은 임시값(지영 확인 필요)이고 모두 설정 에셋에 둔다.*

**목표 안내**

- [ ] 런 시작부터 HUD [C] 자리에 부품 진행이 보인다: 신호기 아이콘 + 부품 칸 3개(없는 부품은 흐린 실루엣) + `0/3`. '신호기 부품' 같은 글자 줄은 쓰지 않는다 (N1, ui-plan §7.3 [C], 원칙 9)
- [ ] 부품 수는 Core 조회 인터페이스 또는 이벤트로 받는다. 부품 기능(story-006)이 생기기 전에는 `0/3`으로 고정 표시되고, 006이 값을 연결하면 코드 수정 없이 바뀐다
- [ ] 런이 시작되면 화면 위쪽 가운데에 시작 안내(그림 줄 + 12자 이내 문구)가 6초 보였다가 [C] 표시 쪽으로 줄어들며 사라진다. 그동안 게임은 멈추지 않고 조작할 수 있다 (N2)
- [ ] 시작 안내와 부품 진행은 부품이 언제·어디에 나오는지 알리지 않는다. 위치 안내는 등장 알림 + 링 이름뿐이다 (D8, core-rules §1)
- [ ] 다시 하기로 새 런을 시작해도 시작 안내가 다시 나온다

**안도 연출**

- [ ] 켜진 모닥불의 온기 반경 안으로 들어오면 화면 가장자리에 주황빛 비네트가 0.5초에 걸쳐 켜지고(불투명도 최대 25%), 나가면 1초에 걸쳐 꺼진다. 꺼진 모닥불에서는 켜지지 않는다 (N3, core-rules §3)
- [ ] 온기 범위에 들어오는 순간 효과음이 1회 나고(같은 불은 10초 안에 다시 나지 않음), 켜진 모닥불 근처에서는 불 타는 소리가 거리에 따라 커진다 (N3·N5)
- [ ] 밤을 넘기고 아침이 되면(2일차부터) 해 아이콘 + `N일차`가 1.5초 떴다 사라지고, 화면 가장자리가 0.8초 금빛으로 빛나며 효과음이 1회 난다. 사망과 같은 순간이면 나오지 않는다 (N4, ui-plan §3)
- [ ] 화면 효과는 가장자리에만 있고 화면 가운데(플레이 영역)와 HUD 숫자를 가리지 않는다 (원칙 2)

**공통**

- [ ] 표시 시간·불투명도·색·소리 크기는 설정 에셋 값이고 코드에 없다 (tech-decisions 1)
- [ ] 화면 문구(시작 안내, `일차`)는 `UiTextTable`에서 읽는다 (tech-decisions 27)
- [ ] 소리 재생 방식과 임시 음원의 출처·라이선스를 tech-decisions에 기록한다 (N5)
- [ ] 증거 스크린샷을 `production/qa/evidence/story-012-goal-guidance-relief-feedback/`에 남긴다: 시작 안내, `0/3` 표시, 온기 비네트(불 안/밖 비교), 아침 연출 (tech-decisions 25)

---

## Implementation Notes

- 온기 범위 안인지는 이미 있는 값(`IFireStatus` 또는 환경 온도의 모닥불 온기)을 읽는다. UI가 Camp를 직접 참조하지 않는다 (code-structure §3·4).
- 아침은 `DayStarted` 이벤트로 받는다. 날짜 표시는 story-010의 타임라인과 같은 날짜 값을 쓴다.
- 서리 비네트(체온 30 미만, ui-plan §7.4)는 아직 없다. 이 스토리는 온기 비네트만 만들고, 서리 비네트가 생길 때 둘의 겹침 규칙(§7.6)을 적용한다.
- 소리는 프로젝트의 첫 오디오다. 방식이 문서에 없으므로 구현 전에 선택지(단순 AudioSource / 오디오 믹서 구성)를 보고하고 지영 확인 후 tech-decisions에 기록한다.
- 문서에 없는 결정이 필요하면 만들지 말고 보고한다.

---

## Out of Scope

- 부품 등장·줍기·장착과 구조 타이머 (story-006)
- 증강 선택 화면 (story-005, 증강 방향 안 확정 뒤)
- 서리 비네트·피해 플래시 등 다른 화면 효과 (ui-plan §7.4)
- 배경 음악과 전체 오디오 방향
- 타이틀·튜토리얼 (story-009)

---

## Files

- `Assets/Scripts/UI/**` (부품 진행 표시, 시작 안내, 온기 비네트, 아침 연출)
- `Assets/Scripts/Core/Contracts/**` (부품 수 조회 인터페이스 또는 이벤트가 없으면 추가)
- `Assets/Data/UI/**` (설정 값, 문구)
- 임시 효과음 파일 (`docs/architecture/asset-rules.md`에 오디오 폴더·이름 규칙이 아직 없다. 구현 전에 위치를 정해 그 문서에 기록한다)
- `Assets/Scenes/Game.unity` (HUD)
- `docs/architecture/tech-decisions.md` (소리 방식, 임시값 기록)

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

**Story Type**: UI
**Test**: waived at qa.level minimal
**Required evidence**: AC '증거'의 스크린샷. MCP로 에디터 Play Mode를 돌리고 Game View를 찍는다 (tech-decisions 18·21·25). 소리는 스크린샷으로 확인할 수 없다
**지영 플레이 확인 필요** (tech-decisions 25): 시작 안내가 목표를 알려 주는지와 길이(6초), 온기 비네트가 '돌아왔다'는 안도로 느껴지는지와 세기(25%),
효과음과 불 타는 소리의 느낌·크기, 아침 연출이 '넘겼다'는 보상으로 느껴지는지. 확인 전까지 N1~N5는 임시값.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/whiteout/story-010-hud-forecast-timeline.md` (HUD 상단 배치·[C] 자리·날짜), `production/epics/whiteout/story-004-items-bag-structures-cooking.md` (모닥불 온기)
- 작업 순서 (지영 결정 2026-11-13): `production/epics/whiteout/story-011-mouse-interaction-cursor-placement.md` → 모닥불 1.5배 → 이 스토리
  - '모닥불 1.5배' 작업: **미확인.** 저장소의 스토리·tech-decisions·세션 기록에서 이 이름의 작업이나 문서를 찾지 못했다.
    가장 가까운 것은 모닥불 에셋 통합(`docs/architecture/tech-decisions.md` 30번, 커밋 d821129)이지만 1.5배에 대한 내용은 없다.
    그 작업이 온기 반경을 바꾼다면 이 스토리의 온기 비네트 범위(N3)가 그 값을 따라간다.
- Unlocks: None
