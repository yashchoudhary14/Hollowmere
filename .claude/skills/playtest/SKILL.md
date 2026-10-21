---
name: playtest
description: Record the user's playtest feedback in docs/PLAYTEST_LOG.md and turn it into fixes.
argument-hint: [what you noticed while playing]
disable-model-invocation: true
---

The user just playtested Hollowmere. Their notes: $ARGUMENTS

1. If the notes are vague ("the monster feels off"), ask one or two short questions to pin down what happened, where, and what they expected.
2. Add a dated entry at the top of `docs/PLAYTEST_LOG.md` for the current milestone. Split the notes into separate findings marked **bug**, **feel** or **idea**, each `[open]`.
3. If a finding changes the design (a new rule, or a different number), update `docs/DESIGN.md` too, after confirming with the user.
4. Fix the bugs first, then the feel items, running `check` and `test` as usual. Mark each finding `[fixed <commit>]` as you go.
5. Ideas go into ROADMAP.md under the milestone they belong to, not into the current work, unless the user says otherwise.
6. Finish with what changed and what the user should retest.
