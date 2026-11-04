# 0001. Classic Clean Architecture with folder-based modules

- Status: Accepted
- Date: 2026-11-03

## Context
We need a structure that is familiar, fast to build, and keeps the option of
extracting modules later.

## Decision
Four projects (Domain, Application, Infrastructure, Api). Modules (Registry,
Scheduling, ...) are separated by folders/namespaces inside each project.
Aggregates reference each other by ID only. One schema per module, no foreign
keys across schemas. Architecture tests enforce layer and module rules.

## Alternatives considered
- Layers per module (projects per module): stronger compile-time boundaries,
  more boilerplate.
- Single-project module with internal types: fewer projects, same discipline needed.

## Consequences
- Module boundaries are enforced by tests, not by the compiler.
- Single DbContext at first; split per module if it grows too large.

## Revisit when
Modules start coupling in ways the architecture tests cannot catch, or when
extracting the first module (e.g. Notifications).