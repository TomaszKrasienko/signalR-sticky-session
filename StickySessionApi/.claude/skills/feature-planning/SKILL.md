---
name: feature-planning
description: Plan a new feature before implementation — gather clarifying requirements, then write a structured plan file. Use when the user asks to plan a feature, or invokes /feature-planning.
disable-model-invocation: true
---

# Feature Planning

## Workflow

1. **Clarify scope.** Ask 5–10 targeted questions with AskUserQuestion (max 4 per call — split into multiple calls if needed) covering: architecture/storage choices, library/tooling picks, scope boundaries for this iteration vs later, and any ambiguous requirement in the user's request. Skip questions already answered by the user's initial message.
2. **Derive feature name.** Short kebab-case slug from the request (e.g. `user-signalr-messaging`).
3. **Write the plan** to `.claude/plans/<feature-name>/plan.md` (create directories as needed). Structure:
   - `# <Feature Name>`
   - `## Goal` — one paragraph, what & why
   - `## Decisions` — bullet list of resolved choices from clarifying questions, with the reasoning
   - `## Folder Structure` — target layout (e.g. Clean Architecture: Domain/Application/Infrastructure/Api projects)
   - `## Implementation Steps` — ordered, concrete, file-level where possible
   - `## Out of Scope` — explicitly deferred items, so future sessions don't accidentally add them
4. **Do not start implementing** after writing the plan unless the user confirms — present a short summary and ask if they want to proceed.
5. After the plan is approved and implementation happens, update the root `CLAUDE.md` with durable facts a future session would need (new project structure, how to run/build, where key pieces live). Do not restate the plan there — CLAUDE.md is for standing project facts, not task history.

## Notes

- One plan file per feature; if `.claude/plans/<feature-name>/plan.md` already exists, treat this as a revision — read it first, then edit rather than overwrite blindly.
- Keep the plan actionable and concrete, not a restatement of the questions.
