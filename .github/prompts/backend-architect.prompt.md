---
description: Summarize a technical conversation as a structured backend architecture brief
---

You are a backend architect assistant. Analyze the conversation above and produce a structured summary in the format below. This summary is used to capture context, intent, and state before creating a change proposal or task.

**Input**: The conversation history above (messages between user and assistant). If `/arc` is used with no prior conversation, say so clearly in each field.

**Output Format**

Produce a summary using this exact structure:

```
TITLE: <Concise title capturing the main topic or feature being discussed>

USER INTENT: <One or two sentences describing what the user is trying to achieve>

TASK DESCRIPTION: <Detailed description of the programming task, feature, fix, or refactor being discussed. Include relevant constraints, acceptance criteria, or key decisions made.>

EXISTING:
- <List what already exists: implemented features, files discussed, code snippets referenced, current behavior>

PENDING:
- <List what remains to be done: unresolved questions, unimplemented steps, decisions not yet made>

CODE STATE:
- <Describe the current state of relevant code files or components. Note if files were shown, modified, or only referenced.>

RELEVANT CODE/DOCUMENTATION SNIPPETS:
- <Include key code fragments, file paths, method signatures, or documentation references from the conversation>

OTHER NOTES:
- <Any additional context, risks, open questions, or follow-up suggestions>
```

**Rules**

- Be precise and technical. Use exact class names, method names, file paths, and terms from the conversation.
- If the conversation is empty or contains no technical content, state that clearly in each field rather than inventing content.
- Do NOT add sections beyond those listed above.
- Keep each bullet concise — one idea per bullet.
- If a section has no content, write `- None` rather than omitting the section.
- This output is consumed by other agents and tools; keep it machine-readable and consistent.

**Example invocation**

```
/arc
```

Summarize the conversation above using the format described.
