# Tech Decisions — Whiteout (가제)

`minimal` 단계: ADR 대신 이 한 장에 기록한다. 게임 규칙은 `design/quick-specs/core-rules.md`.

1. **데이터:** 게임 수치는 모두 ScriptableObject. 코드에 직접 쓰지 않는다.
   나중에 스프레드시트(CSV) 테이블 → ScriptableObject 가져오기 도구 (MVP 밖).
   이유: 에디터에서 바로 수정하고 플레이 중에도 조율할 수 있다.
2. **스탯과 증강:** 스탯 = 기본값 + 수정자 목록(더하기 / 곱하기).
   수치형 증강 = 수정자 묶음(데이터만으로 만든다). ★ 규칙형 증강 = 증강별 코드.
   이유: 증강이 쌓여도 값이 왜 바뀌었는지 추적할 수 있고, 새 분야는 데이터만 추가하면 된다.
3. **로직과 테스트:** 규칙(허들, 환경 온도, 증강 적용)은 MonoBehaviour가 아닌 순수 C# 클래스로 둔다.
   싱글톤 대신 의존성 주입. 단위 테스트는 `Assets/Tests/EditMode/`.
   이유: 에디터 실행 없이 수식을 자동 테스트할 수 있다.
4. **외형 분리:** 프리팹 루트 = 로직, 자식 = 모델. 리소스는 미정이며 처음에는 기본 도형으로 만든다.
5. **플랫폼:** Steam PC, 키보드+마우스, Input System. 게임패드는 나중에 입력 매핑만 추가.
6. **렌더링:** URP, 3D 로우폴리, 고정 60도 탑다운 카메라.
7. **코드 위치:** `Assets/Scripts/<시스템>/` (예: Survival, Environment, Augments, Resources, Wolves).
