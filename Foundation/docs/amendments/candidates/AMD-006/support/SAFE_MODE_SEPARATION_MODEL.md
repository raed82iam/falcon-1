# Safe Mode Separation Model

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-062

## 1. Independent State Axes

```text
Platform axis:
PLATFORM_NORMAL
  → PLATFORM_HEIGHTENED
  → PLATFORM_CONTAINMENT
  → PLATFORM_SAFE
  → PLATFORM_RECOVERY_GUARD

Trading axis:
TRADING_NORMAL
  → TRADING_HEIGHTENED
  → TRADING_RESTRICTED
  → TRADING_SAFE
  → TRADING_RECOVERY_GUARD
```

These are not one combined state machine.

## 2. Governing Rules

1. FFG owns Platform state.
2. Trading Guardian owns Trading state within mandate.
3. Trading Guardian may request but cannot activate Platform state.
4. FFG cannot declare Trading-domain safety.
5. `PLATFORM_NORMAL` does not imply `TRADING_NORMAL`.
6. `TRADING_NORMAL` cannot override any Platform restriction.
7. Effective capability is the intersection of all applicable restrictions.
8. restart and time passage do not clear either restriction.

## 3. Examples

| Platform | Trading | Meaning |
|---|---|---|
| NORMAL | SAFE | platform healthy; Trading remains protectively stopped |
| CONTAINMENT | RESTRICTED | shared fault contained; Trading permits only bounded protection |
| SAFE | RECOVERY_GUARD | Platform restriction dominates; Trading recovery cannot broaden it |
| RECOVERY_GUARD | SAFE | platform restoring; Trading danger remains unresolved |

## 4. Release

Platform release requires Foundation technical evidence and FFG release authority.

Trading release requires Trading risk, execution, exposure-management, and authority evidence plus the competent Trading release authority.

Cross-correlated evidence may be shared by reference; release decisions remain separate.
