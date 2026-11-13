# Epic: whiteout

> **Status**: In Progress
> **Tier**: workflow minimal (no GDD, ADR, TR registry, control manifest)
> **Created**: 2026-11-12

## Goal
눈보라 속에서 체온·허기·체력을 붙들고 하루씩 버티며, 매일 고르는 증강으로 나만의 생존법을 쌓아 끝내 탈출하는 탑다운 혹한 생존 게임. (`design/game-brief.md` Pitch)

## Scope
`design/game-brief.md`의 Features 7개 전부 (feature 3은 두 스토리로 나눔).
범위 밖은 브리프의 Out of scope를 따른다.

## Order
`design/game-brief.md` Build order 기준. 두 가지 예외:
- 002(낮/밤·날씨·환경 온도)는 Build order 3번이지만 체온이 환경 온도에 의존해 003 앞으로 당김.
- feature 3(아이템)은 004(채집·설치·요리)와 008(도구·장비·제작)로 나눠, 제작을 Build order 6번 자리에 둔다.

## 근거 문서
| 문서 | 무엇 |
|---|---|
| `design/game-brief.md` | 방향, Features, 범위, Build order |
| `design/quick-specs/core-rules.md` | 규칙, 수치, 공식, 증강 목록 |
| `design/quick-specs/items.md` | 아이템, 가방, 제작, 요리, 시작 소지품 |
| `docs/architecture/tech-decisions.md` | 기술 결정 |
| `docs/architecture/code-structure.md` | 폴더, 어셈블리, 의존 방향, 통신, 확장 방식 |

문서 우선 규칙(`CLAUDE.md`): 스토리 구현 중 문서와 다른 점이 생기면 코드보다 문서를 먼저 정하고, 같은 작업에서 문서를 갱신한다.

## Stories

| # | Story | Type | Status | ADR |
|---|-------|------|--------|-----|
| 001 | 기반 + 이동 + 설원 맵 | Integration | Complete | N/A (minimal) |
| 002 | 낮/밤·날씨(3일 예보)·환경 온도 | Integration | Complete | N/A (minimal) |
| 003 | 허들 3종과 사망 | Logic | Complete | N/A (minimal) |
| 004 | 아이템 채집·가방·모닥불/은신처 설치·요리 | Integration | Ready | N/A (minimal) |
| 005 | 증강 3택 1 (16개) | Integration | Ready | N/A (minimal) |
| 006 | 구조 신호기·부품·구조·결과 화면 | Integration | Ready | N/A (minimal) |
| 007 | 늑대 무리와 단순 공격 | Integration | Ready | N/A (minimal) |
| 008 | 도구·장비·한 단계 제작 | Integration | Ready | N/A (minimal) |
