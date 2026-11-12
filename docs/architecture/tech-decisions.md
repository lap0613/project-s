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
10. **플레이어 이동:** CharacterController. 물리에 밀리지 않고 입력대로 정확히 움직인다.
11. **카메라:** Cinemachine(Unity 공식 패키지)으로 60도 탑다운 고정, 플레이어 추적.
12. **씬:** Bootstrap 씬(공용 서비스·설정, 한 번만 생성) + Game 씬(추가로 불러옴). 재시작은 Game 씬만 다시 불러온다.
13. **온도 판정:** 장소 구역은 없다. View가 플레이어와 시작 지점 사이 거리(링), 근처 켜진 모닥불을 계산해
    Logic에 넘긴다. 트리거 볼륨은 쓰지 않는다. 은신처 휴식 범위 판정도 View가 한다.

## 변경 이력
- 2026-11-12: 최초 작성 (1~7번).
- 2026-11-12: 8번 코드 구조 추가, 7번 경로를 시스템 폴더 구조에 맞춤.
- 2026-11-12: 3번 테스트를 선택사항으로 변경 (핵심 기능 구현과 눈으로 확인을 우선). 9번 VContainer 설치, 10번 이동, 11번 카메라 추가.
- 2026-11-12: 12번 씬(Bootstrap + Game), 13번 온도 판정(거리 링 + 모닥불, 트리거 볼륨 없음) 추가.
