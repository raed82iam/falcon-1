# CON-007 — Configuration Contract

**Identifier:** CON-007  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Core Authority  
**Governing Specifications:** SYS-007, SEC-001

## Purpose

This Contract defines governed configuration identity, resolution, activation, and evidence.

## Configuration Definition

Every item SHALL declare:

- canonical key;
- schema and type;
- owner;
- purpose;
- scope;
- allowed source;
- default when permitted;
- validation;
- sensitivity;
- activation mode; and
- compatibility.

## Effective Configuration

Every effective value SHALL expose:

- key and value or protected reference;
- source;
- version;
- precedence;
- authority;
- effective time;
- validation result; and
- integrity evidence.

## Obligations

- **CON-007-REQ-001:** Resolution SHALL be deterministic for the same valid sources.
- **CON-007-REQ-002:** Unknown, invalid, unauthorized, or higher-authority-conflicting values SHALL be rejected.
- **CON-007-REQ-003:** Missing required value SHALL prevent affected unrestricted startup.
- **CON-007-REQ-004:** Secrets SHALL use protected references rather than ordinary values.
- **CON-007-REQ-005:** Activation mode SHALL be enforced.
- **CON-007-REQ-006:** Partial material activation SHALL be detected.
- **CON-007-REQ-007:** Rollback SHALL preserve failed-change evidence.
- **CON-007-REQ-008:** Historical effective configuration SHALL be reconstructable.

## Acceptance

Acceptance requires precedence, invalid value, missing value, secret reference, unauthorized change, partial activation, and rollback examples.
