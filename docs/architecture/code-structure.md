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
    Camp/                모닥불·은신처 설치, 연료, 점화(부싯돌·부싯깃), 재점화, 온기·휴식 범위
    Resources/           아이템 정의(재료·소모품·도구·장비·부품), 가방, 채집, 배치, 제작, 요리
    Augments/            증강 정의, 풀, 선택, 수정자 적용
    Augments.Effects/    Whiteout.Augments.Effects (규칙형 증강 모음, Unity 어셈블리 1개)
    Threats/             적 정의, 등장 규칙, 행동, 전투
    RunFlow/             낮/밤, 날짜, 부품 등장, 무전기, 구조, 승패
    Player/              이동, 공격
  Input/                 Whiteout.Input           (Input System → Core 인터페이스)
  UI/                    Whiteout.UI              (HUD, 증강 선택 화면)
  Game/                  Whiteout.Game            (조합 루트)
Assets/Tests/EditMode/   Whiteout.Tests.EditMode  (Core + 모든 Logic 테스트, 지금은 선택사항, 나중에 추가)
```

각 시스템 폴더는 `Logic/`(Whiteout.<System>)과 `Unity/`(Whiteout.<System>.Unity, 하위에 `Data/`·`View/`)로 같은 모양이다.
`Unity/Data/`는 `Unity/View/`를 참조하지 않는다 (폴더 규칙, 리뷰로 지킨다).

## 3. 의존 그래프 (화살표 = 참조 방향)

```
                       Whiteout.Core   ← 아무것도 참조하지 않음
                             ▲
   Survival  Environment  Camp  Resources  Augments  Threats  RunFlow  Player   (Logic)
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

| 이벤트 | 발행 | 구독 |
|---|---|---|
| `DayStarted` / `NightStarted` | RunFlow | Threats, View(조명·시야) |
| `DayEnded` | RunFlow | Augments(선택 열기), UI |
| `WeatherChanged` | Environment | UI, View |
| `FireStateChanged` | Camp | UI, View |
| `ResourceGathered` | Resources | UI |
| `PlayerDamaged` | Threats | Survival |
| `PlayerDied` | Survival | RunFlow |
| `AugmentChosen` | Augments | UI |
| `RadioRepaired` / `RescueArrived` | RunFlow | UI |
| `StructurePlaced` | Camp | UI, View |
| `WeatherForecastUpdated` | Environment | UI |

| 조회 인터페이스 | 구현 | 사용 |
|---|---|---|
| `IAmbientTemperatureSource` | Environment | Survival |
| `IStatRegistry` | Core | 스탯을 쓰는 모든 시스템, Augments |
| `IRandom` | Core (시드 지정 가능) | Resources(배치), Environment(날씨), Augments(뽑기) |

## 5. 스탯과 수정자

- `StatId`: Core의 문자열 기반 값 타입. enum이 아니므로 새 스탯 추가 시 기존 코드 수정 없음.
  에디터에서는 `StatIdAsset`(Core.Unity)으로 고른다.
- 최종값 = (기본값 + Σ더하기) × (1 + Σ곱하기)
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

- Logic은 `UnityEngine`을 쓸 수 없다. `Mathf`·`Vector3`·`Debug` 대신 `System.Math`와 자체 타입.
  불 반경·구역 같은 위치 판정은 View가 하고 결과만 Logic에 넘긴다.
- 시간은 `Tick(dt)`로만 진행 → 테스트에서 시간을 직접 조작할 수 있다.
- 어셈블리 약 20개: 컴파일은 빨라지지만 참조 설정을 관리해야 한다.
- VContainer `1.19.0` (git URL + 태그 고정, `tech-decisions.md` 9번): Unity 6.3 호환을 공식 문서로 확인하지 못함.
  설치 직후 컴파일·간단한 주입 테스트로 확인한다.

## 변경 이력
- 2026-11-12: 최초 작성 (시스템당 어셈블리 2개, VContainer 조합 루트).
- 2026-11-12: 테스트 어셈블리를 선택사항으로 표시, VContainer 버전 1.19.0 명시.
- 2026-11-12: Camp에 모닥불·은신처 설치 추가, Environment에 날씨 예보·거리 링 추가, 이벤트 2종 추가.
- 2026-11-12: Resources에 아이템 체계·제작·요리 추가 (design/quick-specs/items.md). 장비 효과는 스탯 수정자로 처리.
