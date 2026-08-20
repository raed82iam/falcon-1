# Stage 4 VPL-002 FIL-Path Resolution

## Problem

The approved VPL-002 procedure requires the prohibited action to be attempted through:

- the normal FIL path;
- retry;
- replay;
- every declared direct execution boundary.

Stage 5 owns full FIL and Service Bus implementation. Stage 4 must not pull Stage 5 transport into scope.

## Selected Resolution

Stage 4 will use a **verification-only FIL boundary adapter**.

The adapter will model the declared FIL execution boundary required by VPL-002 without implementing Stage 5 transport, routing, delivery, retry infrastructure, or Service Bus behavior.

## Adapter Rules

The verification-only adapter:

- exists only in the Stage 4 verifier or test boundary;
- accepts the same governed authority request identity used by the direct execution boundary;
- invokes the same Authority Engine and execution-boundary contract;
- performs no real transport;
- owns no runtime state;
- introduces no Service Bus;
- does not become production infrastructure;
- cannot authorize actions itself;
- must expose retry and replay attempts;
- must produce evidence showing that the same prohibited action is denied through the modeled FIL boundary.

## Required VPL-002 Paths

The Stage 4 verification must exercise:

1. permitted control action through the direct governed boundary;
2. prohibited action through the verification-only FIL adapter;
3. retry of the prohibited FIL-modeled request;
4. replay of the prohibited FIL-modeled request;
5. every declared direct execution boundary;
6. expired delegation;
7. revoked delegation.

## Pass Conditions

- every prohibited path is denied;
- no side effect occurs;
- authoritative state remains unchanged;
- retry and replay do not create permission;
- denial evidence is attributable;
- independent verifier examines both the adapter result and the authoritative state owner.

## Non-Authority

This document does not authorize:

- production FIL implementation;
- Service Bus implementation;
- transport, routing, delivery, or retry services;
- Stage 5 work;
- any production dependency on the verification adapter.

If a governed FIL implementation becomes available before Stage 4 closure, replacing the adapter requires a reviewed planning amendment.
