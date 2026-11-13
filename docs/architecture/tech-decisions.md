# Tech Decisions — Whiteout (가제)

`minimal` 단계: ADR 대신 이 한 장에 기록한다. 게임 규칙은 `design/quick-specs/core-rules.md`.

1. **데이터:** 게임 수치는 모두 ScriptableObject. 코드에 직접 쓰지 않는다.
   나중에 스프레드시트(CSV) 테이블 → ScriptableObject 가져오기 도구 (MVP 밖).
   이유: 에디터에서 바로 수정하고 플레이 중에도 조율할 수 있다.
2. **스탯과 증강:** 스탯 = 기본값 + 수정자 목록(더하기 / 곱하기).
   수치형 증강 = 수정자 묶음(데이터만으로 만든다). ★ 규칙형 증강 = 증강별 코드.
   이유: 증강이 쌓여도 값이 왜 바뀌었는지 추적할 수 있고, 새 분야는 데이터만 추가하면 된다.
3. **로직과 테스트:** 규칙(허들, 환경 온도, 증강 적용)은 MonoBehaviour가 아닌 순수 C# 클래스로 둔다.
   싱글톤 대신 의존성 주입.
   테스트는 지금은 선택사항(`qa.level: minimal`)이다. 핵심 기능을 빨리 구현하고 눈으로 확인하는 것을 우선한다.
   테스트할 수 있는 구조는 유지해서 나중에 `Assets/Tests/EditMode/`에 추가한다.
4. **외형 분리:** 프리팹 루트 = 로직, 자식 = 모델. 리소스는 미정이며 처음에는 기본 도형으로 만든다.
5. **플랫폼:** Steam PC, 키보드+마우스, Input System. 게임패드는 나중에 입력 매핑만 추가.
6. **렌더링:** URP, 3D 로우폴리, 고정 60도 탑다운 카메라.
7. **코드 위치:** `Assets/Scripts/Systems/<시스템>/` (Survival, Environment, Camp, Resources, Augments, Threats, RunFlow, Player).
8. **코드 구조:** 시스템당 어셈블리 2개(Logic / Unity), Core는 무참조, 조합 루트는 VContainer.
   상세: `docs/architecture/code-structure.md`
9. **VContainer 설치:** `Packages/manifest.json`에 git URL + 태그 고정
   `https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.19.0`
   설치 직후 컴파일과 간단한 주입이 되는지로 Unity 6.3 호환을 확인한다.
   **주의:** VContainer는 이름이 `LifetimeScope.cs`로 끝나는 새 파일의 `.meta`가 생길 때 그 파일을 빈 템플릿으로 덮어쓴다
   (패키지의 ScriptTemplateModifier). `Assets/Settings/VContainerSettings.asset`(Preloaded Assets에 등록)에서
   `DisableScriptModifier`를 켜 두었지만, **편집 모드에서는 막지 못한다** (2026-11-12 시험으로 확인).
   VContainer가 이 설정(`VContainerSettings.Instance`)을 플레이 모드에서만 읽기 때문이다.
   그래서 새 스코프 파일을 만든 뒤에는 에디터 임포트가 끝난 다음 반드시 내용을 다시 확인하고, 덮어써졌으면 다시 쓴다.
   (이미 `.meta`가 있는 파일은 다시 덮어써지지 않는다.)
10. **플레이어 이동:** CharacterController. 물리에 밀리지 않고 입력대로 정확히 움직인다.
11. **카메라:** Cinemachine(Unity 공식 패키지) `com.unity.cinemachine` 3.1.4 고정(Packages/manifest.json)으로 60도 탑다운 고정, 플레이어 추적.
    Unity 6.3은 기본으로 옛 2.10.4를 설치하는 알려진 문제가 있어 버전을 고정한다. 설치 직후 컴파일로 호환을 확인한다.
12. **씬:** Bootstrap 씬(공용 서비스·설정, 한 번만 생성) + Game 씬(추가로 불러옴). 재시작은 Game 씬만 다시 불러온다.
13. **온도 판정:** 장소 구역은 없다. View가 플레이어와 맵 중심 사이 거리(링), 근처 켜진 모닥불을 계산해
    Logic에 넘긴다. 트리거 볼륨은 쓰지 않는다. 은신처 휴식 범위 판정도 View가 한다.
14. **단위:** 거리 m, 속도 m/s, Unity 1유닛 = 1m. 그리드 판정은 쓰지 않는다.
15. **길찾기:** NavMesh(`com.unity.ai.navigation`). 적의 상태 전환은 Logic, 길찾기·이동은 View의 NavMeshAgent가 맡는다.
16. **시야 가림 판정:** View가 적의 눈 높이에서 플레이어까지 Physics.Linecast(레이캐스트)로 확인한다. 시야를 가리는 오브젝트는
    전용 레이어('시야 차단')로 구분해 데이터로 바꿀 수 있게 한다. 이 레이어에는 나무·바위·지형만 넣는다(설치물·아이템 제외, core-rules §6).
    감지와 놓침 모두에 쓴다. 결과(보임/가림)만 Logic에 넘긴다. 판정 주기 0.2초 (임시값).
17. **카메라 가림 처리:** 카메라와 플레이어 사이에서 시야를 가리는 나무·바위만 반투명으로 바꾸고, 벗어나면 되돌린다.
    대상은 가림 표시 컴포넌트(`CameraOccluder`)를 붙인 오브젝트뿐이다(바닥·가장자리 벽·플레이어 설치물 제외).
    View(Whiteout.Game 어셈블리, 카메라 연출)가 매 프레임 카메라→플레이어로 SphereCast(반경 0.5m, 임시값)해 판정하고,
    반투명 머티리얼(알파 0.3, 임시값)로 바꿔 끼운다. 나무 높이 8m는 그대로 둔다(16번 늑대 시야 가림에 쓰임).

18. **포커스 없는 플레이 확인 (에디터 전용):** Unity 창에 포커스가 없으면 에디터 플레이가 첫 프레임에서 멈춘다.
    Player Settings의 Run In Background는 이미 켜져 있지만 에디터 플레이 중 `Application.runInBackground`는 꺼져 있었다.
    그래서 에디터 전용 스크립트(`Whiteout.Editor` 어셈블리, 빌드 제외)가 플레이 시작 때 `Application.runInBackground = true`로 켠다.
    빌드된 게임의 동작(포커스를 잃었을 때)은 바꾸지 않는다. Interaction Mode 변경은 필요 없었다.
    **입력:** Input System은 포커스가 없을 때 키보드를 끄고(`ResetAndDisableNonBackgroundDevices`), 에디터에서는 Game 뷰 포커스가
    있을 때만 키보드를 받는다(`PointersAndKeyboardsRespectGameViewFocus`). 이 값은 프로젝트에 영구로 바꾸지 않는다(개발 중 다른 창 입력이
    게임으로 들어가기 때문). 자동 확인(MCP) 때만 실행 중에 `IgnoreFocus` / `AllDeviceInputAlwaysGoesToGameView`로 바꾸고, 꺼진 장치는
    `InputSystem.EnableDevice`로 다시 켜고, 끝나면 원래 값으로 되돌린다. (이 프로젝트에는 Input System 설정 에셋이 없어 이 변경은 파일에 남지 않는다.)

19. **시작 위치 장애물 회피:** Logic(`PlayerStartPicker`)이 안쪽 링 안에서 후보를 뽑고, View(`PlayerController`)가 그 자리에
    플레이어 크기 + 여유 0.5m(임시값)만큼 충돌체가 있는지 `Physics.CheckCapsule`로 확인한다(바닥·트리거 제외).
    겹치면 다시 뽑고, 최대 30번(임시값) 실패하면 맵 중심(장애물을 두지 않는 자리, core-rules §5)에서 시작한다.
    여유와 시도 횟수는 `PlayerConfig` 데이터.
20. **개발용 디버그 표시:** 링 선 같은 디버그 표시는 `Debug.isDebugBuild`가 참일 때만 켠다(에디터, Development Build).
    실제(릴리스) 빌드에서는 그리지 않는다. Scene 뷰 Gizmos는 원래 에디터 전용이다.

21. **Play는 항상 Bootstrap에서 시작 (에디터 전용):** 에디터에서 어떤 씬을 연 채 Play해도 Bootstrap 씬부터 시작한다
    (`EditorSceneManager.playModeStartScene`). Game 씬만 열고 Play하면 Bootstrap 스코프가 없어 주입이 실패하기 때문이다.
    시작 씬은 Build Settings의 0번 씬(Bootstrap, 12번)을 쓴다. Unity 메뉴 `Whiteout > Play From Bootstrap`으로 끄고 켤 수 있고
    (기본 켜짐), 이 값은 이 PC의 에디터 설정(EditorPrefs)에만 저장된다. 샌드박스 씬을 단독으로 시험할 때만 잠깐 끈다.
    `Whiteout.Editor` 어셈블리에 있어 빌드에는 영향이 없다. Play를 멈추면 에디터는 원래 열려 있던 씬으로 돌아간다.

## 변경 이력
- 2026-11-12: 최초 작성 (1~7번).
- 2026-11-12: 8번 코드 구조 추가, 7번 경로를 시스템 폴더 구조에 맞춤.
- 2026-11-12: 3번 테스트를 선택사항으로 변경 (핵심 기능 구현과 눈으로 확인을 우선). 9번 VContainer 설치, 10번 이동, 11번 카메라 추가.
- 2026-11-12: 12번 씬(Bootstrap + Game), 13번 온도 판정(거리 링 + 모닥불, 트리거 볼륨 없음) 추가.
- 2026-11-12: 14번 단위 규칙(m, m/s, 1유닛 = 1m), 15번 NavMesh 길찾기 추가 (늑대 추적).
- 2026-11-12: 13번 거리 기준을 시작 지점에서 맵 중심으로 변경.
- 2026-11-12: 16번 시야 가림 판정(Linecast + '시야 차단' 레이어, 결과만 Logic에, 0.2초 주기 임시값) 추가.
- 2026-11-12: 16번에 '시야 차단' 레이어 대상(나무·바위·지형만)과 감지·놓침 공통 적용 명시.
- 2026-11-12: 11번 Cinemachine 3.1.4 고정 (6.3 기본 설치가 옛 2.10.4인 문제 회피). story-001 구현 전 확정.
- 2026-11-12: 9번에 VContainer 스코프 파일 덮어쓰기 주의사항과 DisableScriptModifier 설정 추가, 17번 카메라 가림 처리(가리는 나무·바위만 반투명, 나무 8m 유지) 추가.
- 2026-11-12: 9번 정정: DisableScriptModifier는 편집 모드에서 덮어쓰기를 막지 못함(시험으로 확인). 새 스코프 파일은 임포트 후 내용 재확인으로 대응.
- 2026-11-12: 9번 VContainer 덮어쓰기는 '새 스코프 파일은 만든 뒤 내용 재확인' 규칙만 유지하고 우회 스크립트는 두지 않기로 확정 (지영 결정).
- 2026-11-12: 18번 포커스 없는 플레이 확인 추가. 검증: Unity 창 포커스 없음(focused=False) 상태에서 Bootstrap 플레이가 수동 조작 없이 진행(시작 후 4초에 739프레임, runInBackground=True), 기록한 입력 절차로 A 입력 5.55초에 22.21m 이동(4.0 m/s), 콘솔 오류 0.
- 2026-11-12: 19번 시작 위치 장애물 회피, 20번 개발용 디버그 표시(에디터·개발 빌드만) 추가 (지영 결정, story-001 Own calls 2·12번 변경).
- 2026-11-13: 21번 Play 시 항상 Bootstrap에서 시작(에디터 전용, 메뉴로 끄기, EditorPrefs) 추가 (지영 결정).
- 2026-11-13: 21번 검증: Game 씬만 연 채 Play하면 Bootstrap+Game 2개가 로드되고 Game 스코프의 부모가 Bootstrap, 주입 오류 0, 플레이어는 안쪽 링(33.3m)에서 시작. Play를 멈추면 Game 씬으로 돌아옴. 메뉴로 끄면 Game 씬만 로드됨(부모 없음, 예상대로 주입 오류). 검증 후 다시 켬.
