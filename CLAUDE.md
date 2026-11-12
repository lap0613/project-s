# Claude Code Game Studios -- Game Studio Agent Architecture

Indie game development managed through 49 coordinated Claude Code subagents.
Each agent owns a specific domain, enforcing separation of concerns and quality.

## Technology Stack

- **Engine**: Unity 6000.3.28f1
- **Language**: C#
- **Version Control**: Git with trunk-based development
- **Build System**: Unity Build Pipeline
- **Asset Pipeline**: Unity Asset Import Pipeline + Addressables

> **Note**: Engine-specialist agents exist for Godot, Unity, and Unreal with
> dedicated sub-specialists. Use the set matching your engine.

## Project Structure

@.claude/docs/directory-structure.md

## Engine Version Reference

<!-- ENGINE-REFERENCE-IMPORT: the line under this comment is engine-specific,
     and there is none until an engine is set. /setup-engine, the grow's record
     step and /project-settings (engine.name) write
     @docs/engine-reference/<engine>/VERSION.md here for the chosen engine. Skills
     that need the pinned version read that file on demand either way. -->
@docs/engine-reference/unity/VERSION.md


## Technical Preferences

`project.yaml` at the repo root is the primary config store — engine, specialists,
naming, platform, performance, modes. Skills resolve it via `resolve_config`
(see `.claude/docs/config-resolution.md`).

`.claude/docs/technical-preferences.md` is the **legacy fallback**, read on demand
when a key is absent from `project.yaml`. It is not imported here: before
`/setup-engine` runs it is almost entirely `[TO BE CONFIGURED]` placeholders, and
after it runs `project.yaml` holds the real values.

## Coordination Rules

@.claude/docs/coordination-rules.md

## Collaboration Protocol

**User-driven collaboration, not autonomous execution.**
Every task follows: **Question -> Options -> Decision -> Draft -> Approval**

- Agents MUST ask "May I write this to [filepath]?" before using Write/Edit tools
  (exempt: `active.md` appends, agent memory, a subagent's new file at the path
  its orchestrating skill named; see `.claude/rules/agent-memory.md`)
- Agents MUST show drafts or summaries before requesting approval
- Multi-file changes require explicit approval for the full changeset
- No commits without user instruction
- A skipped or dismissed question is not an answer: act on nothing, re-ask in
  plain text and wait
- Started outside this file's folder? Warn first: hooks, deny rules and the
  ask-first mode load only when Claude Code starts here

See `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` for full protocol and examples.

## Document-First Rule (project-s 고정 규칙)

이 프로젝트는 문서 우선(document-first)으로 진행한다. 모든 에이전트와 스킬에 적용되며,
스킬 본문의 tier 규칙(예: `minimal`에서 GDD·ADR을 읽지 않음)보다 우선한다.

**이유 (지영):** AI로 개발할 때는 사람이 코드를 보고 현상을 짚는 것보다, 구현 방식과 진행 흐름을
문서로 한눈에 파악하는 것이 쉽다. 작은 수정이 쌓이면서 코드만 바뀌고 문서 반영이 빠지면
흐름을 놓치게 되고, 그러면 버그를 고치기 어려워진다.

1. **기획·설계가 먼저다.** 결정은 코드보다 먼저 문서(`design/`, `docs/architecture/`)에 기록한다.
2. **문서를 먼저 확인하고, 문서에 근거해 진행한다.** 작업 전에 아래 문서 중 관련 부분을 읽는다.
   문서에 없거나 문서와 다른 구현은 하지 않는다. 먼저 문서를 정하고(사용자 승인) 그다음 구현한다.
3. **문서와 코드는 함께 바뀐다.** 방향이 바뀌면 코드만 고치지 않고, 같은 작업 안에서
   해당 문서도 갱신한다. 문서와 코드가 다른 상태로 작업을 끝내지 않는다.
4. **스토리·작업마다 근거 문서 경로를 적는다.** 문서 경로와 섹션
   (예: `design/quick-specs/core-rules.md §2 허들`)을 스토리 파일과 커밋 본문에 남긴다.
5. **작은 수정도 예외가 아니다.** 수치 조정, 버그 수정, 동작 변경이라도 정책·기획 규칙·구현 방식에
   영향이 있으면 같은 작업에서 관련 문서를 갱신한다.
6. **구현 전에 충돌부터 보고한다.** 지영이 아이디어나 수정을 지시하면, 구현하기 전에 기존 정책·기획
   문서와 어긋나거나 충돌하는 지점을 먼저 찾아 보고한다. 충돌이 있으면 선택지를 제시하고,
   지영이 정한 뒤 문서를 고치고 그다음 구현한다.
7. **정책·기획 규칙은 임의로 정하지 않는다.** 문서에 없는 규칙이 필요하면 추천값과 선택지를 제시하고
   지영에게 확인받는다.
8. **각 문서에 변경 이력을 남긴다.** 문서 끝의 `변경 이력`(브리프는 `Change log`)에
   날짜, 무엇을, 왜를 한 줄로 적는다.

| 문서 | 무엇을 정하는가 |
|---|---|
| `design/game-brief.md` | 게임 전체 방향, Features, 범위, Build order |
| `design/quick-specs/core-rules.md` | 게임 규칙, 수치, 공식, 증강 목록 |
| `design/quick-specs/items.md` | 아이템 목록, 가방, 제작, 요리, 시작 소지품 |
| `docs/architecture/tech-decisions.md` | 기술 결정 요약 |
| `docs/architecture/code-structure.md` | 폴더, 어셈블리, 의존 방향, 시스템 간 통신, 확장 방식 |

새 설계 문서를 추가하면 이 표에도 추가한다.

> **First session?** Say "make me a game" (`/prototype`), or run `/start` to begin
> the guided onboarding flow.

## Coding Standards

@.claude/docs/coding-standards.md

## Context Management

Read `.claude/docs/context-management.md` on demand — it is a reference, not
session context. Two of its conventions are load-bearing and cited by name
elsewhere in the repo, so they are restated here rather than lost:

- **`production/session-state/active.md` is the session checkpoint.** The file is
  the memory, not the conversation. Read it first after any compaction, crash, or
  `/clear`.
- **Helpers in `.claude/scripts/` emit observations, never verdicts.** A script
  that scores or judges will eventually contradict a mode or override it cannot
  see. (Cited by `artifact-check.sh` and `adr-dep-graph.sh`.)
