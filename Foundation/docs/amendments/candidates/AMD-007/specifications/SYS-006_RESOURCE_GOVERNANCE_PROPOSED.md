# SYS-006 — Resource Governance

**Identifier:** SYS-006  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Purpose

Govern domain-independent allocation, budgets, reservation, pressure, throttling, shedding, and restoration of Foundation resources.

## Requirements

- ordinary allocation and emergency protection SHALL remain distinct;
- every consumer SHALL have identity, budget, limits, criticality reference, dependencies, and evidence;
- FFG MAY issue authorized emergency directives but Resource Governance executes and reports them;
- Applications SHALL not self-prioritize or bypass admission-approved budgets;
- minimum Platform survival resources and recovery reserve SHALL be protected;
- abnormal consumers may be throttled, restricted, suspended, or isolated;
- restoration SHALL be progressive and shall not infer business priority;
- unavailable/uncertain enforcement SHALL reduce admission and authority.

## Acceptance

CPU, memory, storage, I/O, connection, queue, and communication pressure; conflicting critical workloads; starvation prevention; optional shedding; FFG directive; rollback; and reconstruction.

