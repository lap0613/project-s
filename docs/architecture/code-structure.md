# Code Structure — Whiteout (가제)

상위 문서: `docs/architecture/tech-decisions.md` · 게임 규칙: `design/quick-specs/core-rules.md`

## 원칙
1. 시스템마다 어셈블리를 나누고, 참조 방향은 한쪽으로만 흐른다.
2. Core는 아무것도 참조하지 않는다. 시스템은 Core의 인터페이스에만 의존한다.
3. 시스템 간 결합은 이벤트(일어난 일)와 Core 인터페이스(조회)로만 한다.
4. Data(ScriptableObject) · Logic(순수 C#) · View(MonoBehaviour)를 분리한다.
5. 새 증강·적·자원은 기존 코드를 고치지 않고 추가한다.

## 1. 레이어

| 레이어 | 형태 | Unity 참조 | 맡는 일 |
|---|---|---|---|
| Data | ScriptableObject | O | 정의와 수치. 실행 시 Logic이 쓰는 설정 객체로 변환 |
| Logic | 순수 C# | **X** (asmdef `noEngineReferences: true`) | 규칙과 상태. `Tick(dt)`로만 진행 |
| View | MonoBehaviour | O | 입력, 물리, 표시, 위치 판정. Logic 호출, 이벤트 구독 |
| 조합 루트 | VContainer `LifetimeScope` | O | 모든 객체를 등록·연결. 유일하게 모든 어셈블리를 참조 |

## 2. 폴더와 어셈블리 (시스템당 2개)

```
Assets/Scripts/
  Core/                  Whiteout.Core            (엔진 참조 없음, 아무것도 참조 안 함)
    Stats/  Events/  Contracts/  Random/
  Core.Unity/            Whiteout.Core.Unity      (StatIdAsset 등 Core의 Unity 쪽 도구)
  Systems/
    Survival/            허들: 체온·허기·체력
      Logic/             Whiteout.Survival        (엔진 참조 없음)
      Unity/             Whiteout.Survival.Unity  (Data/ 와 View/ 폴더로 분리)
    Environment/         날씨(3일 예보), 거리 링, 환경 온도
    Camp/                모닥불·은신처·구조 신호기 설치, 연료, 점화(부싯돌·부싯깃), 재점화, 온기·휴식 범위, 신호기 부품 장착
    Gathering/           아이템 정의(재료·소모품·도구·장비·부품), 가방, 채집, 배치, 제작, 요리
    Augments/            증강 정의, 풀, 선택, 수정자 적용
    Augments.Effects/    Whiteout.Augments.Effects (규칙형 증강 모음, Unity 어셈블리 1개)
    Threats/             적 정의, 등장 규칙, 행동, 전투
    RunFlow/             낮/밤, 날짜, 부품 등장, 구조 요청 타이머, 승패
    Player/              이동, 공격
  Input/                 Whiteout.Input           (Input System → Core 인터페이스)
  UI/                    Whiteout.UI              (HUD, 증강 선택 화면)
  Game/                  Whiteout.Game            (조합 루트)
  Editor/                Whiteout.Editor          (에디터 전용 개발 도구, 빌드에 포함되지 않음, 게임 어셈블리를 참조하지 않음:
                                                   포커스 없는 플레이 확인(tech 18), Play 시 Bootstrap에서 시작(tech 21))
Assets/Tests/EditMode/   Whiteout.Tests.EditMode  (Core + 모든 Logic 테스트, 지금은 선택사항, 나중에 추가)
```

**코드 밖 에셋 폴더:** 모델·텍스처·머티리얼은 `Assets/Art/Models/<분류>/`·`Assets/Art/Textures/`·`Assets/Art/Materials/`, 프리팹은 `Assets/Prefabs/<분류>/`에 둔다.
규칙은 `docs/architecture/asset-rules.md` §7. 프리팹 루트에 View 컴포넌트(로직)와 콜라이더를 두고 모델 FBX는 자식으로 붙인다(tech-decisions 4·30번).
모델을 바꿔도 View 코드는 바뀌지 않는다: `CampfireView`는 `litVisual` 오브젝트 참조만 알고 모델 구조는 모른다.

각 시스템 폴더는 `Logic/`(Whiteout.<System>)과 `Unity/`(Whiteout.<System>.Unity, 하위에 `Data/`·`View/`)로 같은 모양이다.
`Unity/Data/`는 `Unity/View/`를 참조하지 않는다 (폴더 규칙, 리뷰로 지킨다).

**폴더 이름 변경 (지영 결정 2026-11-13):** `Systems/Resources/` → `Systems/Gathering/`. `Resources`는 Unity 특수 폴더 이름이라
그 안의 에셋이 모두 빌드에 들어간다. 이 문서의 시스템 이름도 Gathering으로 바꿨다. **코드 반영함 (story-011):** 폴더 `Systems/Gathering/`,
어셈블리·네임스페이스 `Whiteout.Gathering`, `Whiteout.Gathering.Unity` (이름은 임시, 지영 확인 필요. tech-decisions 31번). `ResourceDefinition`·`ResourcesConfig`·`ResourceGathered` 같은 타입·에셋 이름은 이번 결정 대상이 아니다.

## 3. 의존 그래프 (화살표 = 참조 방향)

```
                       Whiteout.Core   ← 아무것도 참조하지 않음
                             ▲
   Survival  Environment  Camp  Gathering  Augments  Threats  RunFlow  Player   (Logic)
       ▲          ▲         ▲       ▲          ▲         ▲        ▲       ▲
   각 시스템의 .Unity  = Core + Core.Unity + 자기 Logic (+ VContainer: [Inject]용)
   Augments.Effects    = Core + Core.Unity + Augments + 효과에 필요한 시스템 Logic
   Input, UI           = Core + Core.Unity (+ VContainer)
   Game                = 전부 + VContainer  ← 아무도 Game을 참조하지 않음
   Tests.EditMode      = Core + 모든 Logic  (지금은 선택사항, 나중에 추가)
```

**금지 규칙** (asmdef 참조로 강제):
- 시스템 Logic끼리 참조 금지 (예: Survival → Environment).
- 시스템 `.Unity`끼리 참조 금지.
- Logic → 자기 `.Unity` 참조 금지 (한 방향만).
- Core와 모든 Logic은 VContainer를 참조하지 않는다. 생성자 주입만 쓰고(공개 생성자 1개), 등록은 Game이 한다.

## 4. 시스템 간 통신

- **이벤트 (일어난 일 알림):** Core의 `IEventBus`로 발행·구독, 구독 해제는 `IDisposable`.
  여러 시스템이 쓰는 이벤트 타입은 `Core/Contracts/Events/`, 시스템 내부용은 그 시스템 안에 둔다.
- **조회 인터페이스 (매 프레임 읽는 값):** `Core/Contracts/`에 정의, 구현은 해당 시스템, 연결은 Game.
- 이벤트는 알림에만 쓰고, 명령은 인터페이스 호출로 한다 (흐름 추적을 위해).
- **시스템 사이 연결(브리지):** 두 시스템을 잇는 코드는 Game에 둔다. 예: `SurvivalPlayerBridge`는 Survival의 체온 30 미만 상태를
  Player의 이동속도 스탯 수정자로 붙이고, `PlayerDied`에 `PlayerController`를 끈다. Survival과 Player는 서로를 모른다.
  story-004의 브리지: `InteractionDirector`(E 행동을 우선순위대로 하나만 시작하고, 누르는 시간·취소·이동 잠금을 맡는다. 줍기·요리는 Gathering,
  나무 넣기·휴식은 Camp가 `IInteractionSource`로 내놓는다), `PlayerActionLock`(E 행동 중 이동 입력을 막는다),
  `SurvivalGatherBridge`(허기 30 미만이면 채집 속도 스탯에 -30% 수정자를 붙인다), `PlayerPositionSource`.
  story-011: `InteractionDirector`가 좌클릭도 처리한다. 순서는 HUD > 설치 모드(`PlacementController`가 그 프레임의 클릭을 썼다고 `IPlacementMenu.ClickConsumedFrame`으로 알림) >
  커서 밑 대상(각 `IInteractionSource.TryOfferPointed`) > 공격(`IAttackInput.AttackClicked`, story-007이 읽음). 멀면 `PlayerActionLock`의 자동 이동으로 직선으로 걸어간다.
  커서가 가리키는 바닥·대상 판정은 Game의 `PointerWorld`(`IPointerWorld`: 카메라 광선과 수평면의 교점, 대상에 콜라이더가 없어도 된다)가 하고,
  외곽선은 Game의 `TargetHighlighter`가 그린다. 값은 `PointerConfig`(Core.Unity) 에셋.

| 이벤트 | 발행 | 구독 |
|---|---|---|
| `DayStarted` / `NightStarted` | RunFlow | Threats, View(조명·시야) |
| `DayEnded` | RunFlow | Augments(선택 열기), UI |
| `PlayerDamaged` (공격에 맞음, 클릭 자동 이동 취소) | 아직 없음(story-007) | Game(`InteractionDirector`) |
| `WeatherChanged` | Environment | UI, View |
| `FireStateChanged` | Camp | UI, View |
| `ResourceGathered` | Gathering | UI |
| `PlayerDamaged` | Threats | Survival |
| `PlayerDied` | Survival | RunFlow(`RunOutcome`) |
| `AugmentChosen` | Augments | UI |
| `SignalBeaconCompleted` | Camp | UI (지금은 구독자 없음. 완성 알림은 Camp가 `NoticePosted`로 보낸다) |
| `RescueRequested` (도착까지 남은 게임 초) | Camp | RunFlow(타이머 시작), UI(타임라인 깃발) |
| `RescueArrived` | RunFlow (구조 타이머 끝) | RunFlow(`RunOutcome`) |
| `BeaconPartAppeared` (부품, 링) | RunFlow (`PartSchedule`: 2·4·6일차 아침) | Gathering(그 링의 후보 지점에 줍기를 놓고 알림) |
| `RunEnded` (구조 여부, 사망 원인, 날짜. 사망·구조 중 먼저 일어난 쪽 한 번만) | RunFlow (`RunOutcome`) | UI(결과 화면), Game(`InteractionDirector`, `SurvivalPlayerBridge`: 플레이어 이동 정지), Camp(설치 모드 닫기), Survival·RunFlow 러너(틱 정지) |
| `StructurePlaced` | Camp | UI, View |
| `WeatherForecastUpdated` | Environment | UI |
| `NoticePosted` (화면 알림: 문구 키 + 값) | Gathering, Camp | UI |

| 조회 인터페이스 | 구현 | 사용 |
|---|---|---|
| `IAmbientTemperatureSource` | Environment | Survival, UI(기온 표시) |
| `IDayTimeSource` (날짜, 그날 지난 초, 낮·밤 길이) | RunFlow (`DayClock`) | UI(예보 타임라인) |
| `IWarmthSource` (플레이어 위치의 켜진 모닥불 중 가장 따뜻한 1개의 온기, 없으면 0) | Camp (`CampSystem`) | Environment |
| `IStatRegistry` | Core | 스탯을 쓰는 모든 시스템, Augments |
| `IRandom` | Core (시드 지정 가능) | Gathering(배치), Environment(날씨), Augments(뽑기) |
| `IBag` (가방: 칸, 개수, 넣기·빼기, 고른 칸) | Gathering (`Bag`) | Camp(설치·연료 비용), UI(가방 칸 줄) |
| `IItemNames` (아이템 표시 이름) | Gathering | Camp, UI |
| `IBagAction` (칸 우클릭 메뉴 항목: 사용·버리기, 008에서 입기 추가) | Gathering | UI |
| `IVitalsReceiver` (허기·체력 회복 받기) / `IRestTarget` (휴식 상태 켜고 끄기) | Survival | Gathering(소모품 효과) / Camp(은신처 휴식) |
| `IFireStatus` (근처 모닥불의 켜짐·연료) | Camp | Gathering(요리), UI(연료 표시) |
| `IRescueParts` (신호기 부품: 필요한 수, 모은 수, 부품별 상태 없음·가짐·장착) | Camp (`SignalBeaconSystem`: 가방과 장착 상태에서 계산) | UI(목표 안내 [C]) |
| `IRescueCountdown` (구조 타이머: 켜졌는지, 남은 게임 초) | RunFlow (`RescueTimer`) | UI(구조 타이머 [C]) |
| `IRunRestarter` (`Restart()`: Game 씬 다시 불러오기) | Game (Bootstrap 씬의 `GameRestarter`, Bootstrap 스코프에 등록) | UI(결과 화면 `다시 하기`) |
| `IPlacementMenu` (설치 목록: 열림, 항목, 고른 항목, 항목이 개수 한도에 닿았는지) | Camp | UI, Game, Gathering(목록이 열린 동안 숫자키는 가방 칸을 고르지 않음) |
| `IPlayerPosition` (플레이어 위치(발 높이)와 바라보는 방향) | Game (`PlayerPositionSource`) | Gathering, Camp |
| `IInteractionInput` (E, B, 확인(좌클릭)·취소, 숫자키, 휠(`WheelScroll`: 설치 모드에서 회전), 커서 화면 위치) | Input | Game, Camp, UI |
| `IInteractionSource` (행동 1개: 줍기·요리·나무 넣기·휴식. E로 가까운 대상, `TryOfferPointed`로 커서 밑 대상, `IsTargetInReach`로 닿았는지) | Gathering, Camp | Game(`InteractionDirector`) |
| `IPointerWorld` (커서가 HUD 위인지, 커서 광선이 수평면과 만나는 점) | Game (`PointerWorld`) | Gathering, Camp, Game |
| `IAttackInput` (다른 곳에 쓰이지 않은 좌클릭) | Game (`InteractionDirector`) | story-007 전투 |
| `IInteractionStatus` (지금 보여 줄 프롬프트와 진행률) | Game (`InteractionDirector`) | UI |
| `IMapLayout` (맵 중심, 링 반지름 60/130/200m, 거리가 어느 링인지) | Core (값은 Core.Unity의 ScriptableObject) | Player(시작 위치), Environment(온도), Gathering(배치), Threats(늑대 중심지) |

## 5. 스탯과 수정자

- `StatId`: Core의 문자열 기반 값 타입. enum이 아니므로 새 스탯 추가 시 기존 코드 수정 없음.
  에디터에서는 `StatIdAsset`(Core.Unity)으로 고른다.
- 최종값 = (기본값 + Σ더하기) × max(1 + Σ곱하기, 0.1)
  - **% 효과는 합연산:** 같은 스탯에 붙은 %는 모두 더한 뒤 한 번만 곱한다. 예: +15%와 -10% → ×1.05.
  - **하한:** 곱하는 값은 최소 ×0.1 (임시값). 음수 %가 쌓여 0 이하가 되는 것을 막는다.
  - **확률 효과는 예외:** '알뜰한 채집 20%' 같은 확률은 배율 공식에 넣지 않는다. 같은 확률끼리 더하고 100%를 상한으로 둔다.
- 수정자마다 출처(어느 증강인지)를 기록해 값이 바뀐 이유를 추적한다.

## 6. 기존 코드 수정 없이 추가하기

| 추가할 것 | 만드는 것 | 기존 코드 수정 |
|---|---|---|
| 수치형 증강 | `AugmentDefinition` 에셋 + 카탈로그 등록 | 없음 |
| 규칙형 증강 | Effects에 클래스 1개(SO 하위 클래스 + 실행 효과) + 에셋 | 없음 |
| 자원 | `ResourceDefinition` 에셋(칸 수, 사용 효과) + 카탈로그 | 없음. 새 종류의 사용 효과일 때만 효과 클래스 1개 |
| 적 | `EnemyDefinition` 에셋 + 행동(`IEnemyBehaviour`) + 등장 규칙(`IEnemySpawnRule`) | 없음 |
| 날씨 | `WeatherDefinition` 에셋 | 없음 |
| 스탯 | `StatIdAsset` 에셋 | 없음 (읽는 코드는 필요) |

카탈로그 등록은 에셋 수정이지 코드 수정이 아니다.

## 7. 주의할 점

- **Game 씬 → 재시작 (임시, 지영 확인 필요, story-006):** Game 씬(결과 화면)은 Core의 `IRunRestarter`를 주입받아 `Restart()`를 부른다.
  구현은 Bootstrap 씬의 `GameRestarter`이고 Bootstrap 스코프가 등록한다(Game 스코프의 부모라 Game 씬에서 주입된다). 명령은 인터페이스 호출(§4)이라는
  규칙에 따랐다. 다른 선택지는 Core의 재시작 요청 이벤트였다 (`tech-decisions.md` 34번).
- **런 종료 뒤 정지:** 사망이든 구조든 멈춰야 하는 코드는 `PlayerDied`가 아니라 `RunEnded`를 듣는다. `PlayerDied`만 들으면 구조 뒤에 멈추지 않는다.

- Logic은 `UnityEngine`을 쓸 수 없다. `Mathf`·`Vector3`·`Debug` 대신 `System.Math`와 자체 타입.
  불 반경·구역 같은 위치 판정은 View가 하고 결과만 Logic에 넘긴다.
- 시간은 `Tick(dt)`로만 진행 → 테스트에서 시간을 직접 조작할 수 있다.
- **씬 View 주입:** 씬 전체를 타입으로 검색(`FindObjectsByType` 등)해서 주입하지 않는다.
  꼭 있어야 하는 View는 `RegisterComponentInHierarchy<T>()`, 없어도 되는 View(디버그 표시 등)는
  LifetimeScope의 `autoInjectGameObjects` 목록(씬 데이터)으로 주입한다. (`FindObjectsByType`의 정렬 인자는 6.5에서 폐기, engine-reference `deprecated-apis.md`)
- **런타임 머티리얼:** `Shader.Find`로 머티리얼을 만들지 않고, 직렬화된 머티리얼 에셋을 참조한다.
  빌드에서 셰이더가 빠지면 `Shader.Find`가 null을 돌려주기 때문이다 (engine-reference `modules/rendering.md`).
- 시야 가림(레이캐스트)도 위치 판정과 같이 View가 하고, Logic은 '보임/가림' 값만 받는다 (tech-decisions 16).
- 어셈블리 약 20개: 컴파일은 빨라지지만 참조 설정을 관리해야 한다.
- VContainer `1.19.0` (git URL + 태그 고정, `tech-decisions.md` 9번): Unity 6.3 호환을 공식 문서로 확인하지 못함.
  설치 직후 컴파일·간단한 주입 테스트로 확인한다.

## 변경 이력
- 2026-11-12: 최초 작성 (시스템당 어셈블리 2개, VContainer 조합 루트).
- 2026-11-12: 테스트 어셈블리를 선택사항으로 표시, VContainer 버전 1.19.0 명시.
- 2026-11-12: Camp에 모닥불·은신처 설치 추가, Environment에 날씨 예보·거리 링 추가, 이벤트 2종 추가.
- 2026-11-12: Resources에 아이템 체계·제작·요리 추가 (design/quick-specs/items.md). 장비 효과는 스탯 수정자로 처리.
- 2026-11-12: 무전기 → 구조 신호기. 설치·부품 장착은 Camp, 구조 타이머·승패는 RunFlow. 이벤트 `RadioRepaired` → `SignalBeaconCompleted`·`RescueRequested`.
- 2026-11-12: §5에 % 효과 합연산(같은 스탯의 %는 더한 뒤 한 번 곱함), 하한 ×0.1(임시값), 확률 효과 예외(합산, 100% 상한) 명시.
- 2026-11-12: §7에 시야 가림 판정은 View, Logic은 결과만 받는다고 명시.
- 2026-11-12: §4 조회 인터페이스에 `IMapLayout` 추가 (맵 중심·링 정보를 시스템끼리 참조하지 않고 Core 인터페이스로 공유). story-001 구현 전 확정.
- 2026-11-12: §7에 Game 씬→재시작 호출 경로 미정 메모 추가 (story-006 전에 결정).
- 2026-11-12: §7에 씬 View 주입 규칙(전체 검색 금지, RegisterComponentInHierarchy / autoInjectGameObjects)과 런타임 머티리얼 규칙(Shader.Find 금지, 에셋 참조) 추가. 엔진 점검 결과 반영.
- 2026-11-12: §2에 에디터 전용 어셈블리 `Whiteout.Editor` 추가 (포커스 없는 플레이 확인, tech-decisions 18).
- 2026-11-13: §2 Whiteout.Editor 설명에 Play 시 Bootstrap에서 시작(tech-decisions 21) 추가.
- 2026-11-13: §4에 `IWarmthSource` 추가 (Environment가 Camp를 참조하지 않고 모닥불 온기를 받는 통로, story-002). 낮/밤 길이는 RunFlow의 `RunFlowConfig`, 날씨·온도·시야 값은 Environment의 `EnvironmentConfig`에 둔다.
- 2026-11-13: §4에 Game 브리지(`SurvivalPlayerBridge`)와 `PlayerDied` 구독자(Game) 추가 (story-003: 시스템끼리 참조 금지를 지키면서 이동속도 불이익·사망 정지를 연결).
- 2026-11-13: §4에 story-004 계약 추가: 이벤트 `NoticePosted`, 조회 인터페이스 10종(가방·이름·칸 메뉴·회복·휴식·모닥불 상태·설치 목록·플레이어 위치·E 입력·E 행동), Game 브리지 4종. `IWarmthSource` 구현을 Camp로 바꾸고 `NoWarmthSource` 삭제 (story-004, UI 결정 D3~D5 반영으로 계약이 늘었다).
- 2026-11-13: 시스템 폴더 이름 Resources → Gathering (지영 결정, Unity 특수 폴더 이름 회피). §2·§3·§4의 시스템 이름을 바꾸고 §2에 메모 추가. 코드 반영은 S개발.
- 2026-11-13: §4에 조회 인터페이스 `IDayTimeSource` 추가, `RescueRequested`에 값(도착까지 남은 초) 표기, `IAmbientTemperatureSource` 사용처에 UI 추가 (story-010 HUD 타임라인: UI가 RunFlow를 참조하지 않고 게임 시간을 읽는 통로).
- 2026-11-13: §2에 코드 밖 에셋 폴더(Art·Prefabs)와 프리팹 구성 규칙 추가, `asset-rules.md` 연결 (모닥불 에셋 통합, tech-decisions 30번).
- 2026-11-13: story-011 반영: §2 Gathering 폴더·어셈블리 이름 코드 반영(임시), §4 마우스 클릭 상호작용 연결(`InteractionDirector`·`PointerWorld`·`TargetHighlighter`)과 인터페이스 `IPointerWorld`·`IAttackInput`·`PlayerDamaged`, `IInteractionInput`·`IInteractionSource` 변경(휠 = `WheelScroll`, 커서 위치, `TryOfferPointed`).
- 2026-11-13: §4에 조회 인터페이스 `IRescueParts` 추가 (story-012: UI가 부품 수를 받는 통로, 구현은 story-006, 그 전까지 Game의 `PlaceholderRescueParts`). UI의 `IFireStatus` 사용처에 온기 비네트·불 소리 추가.
- 2026-11-13: story-006 반영: §4에 이벤트 `BeaconPartAppeared`·`RunEnded` 추가, `RescueArrived`·`PlayerDied`·`SignalBeaconCompleted` 구독자 수정, 조회 인터페이스 `IRescueCountdown`·`IRunRestarter` 추가, `IRescueParts`(부품별 상태, 구현 Camp)·`IPlacementMenu`(개수 한도) 변경. §7의 재시작 경로 미정 항목을 `IRunRestarter`로 정함(임시, 지영 확인 필요)과 런 종료 뒤 정지 규칙 추가.
