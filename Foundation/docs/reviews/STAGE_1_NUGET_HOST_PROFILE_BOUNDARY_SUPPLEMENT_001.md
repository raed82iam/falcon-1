# Stage 1 NuGet Host Profile Boundary Supplement

## Original failure boundary

The inherited user-level NuGet path resolved to the roaming profile under:

`C:\Users\raeda\AppData\Roaming\NuGet\NuGet.Config`

That boundary was unsuitable for the governed isolated validation path because the execution identity could not rely on the inherited profile location.

## Remediation boundary

The governed remediation path uses:

`C:\falcon\ValidationProfile`

This keeps NuGet validation outside the user roaming profile and outside OneDrive/profile redirection.

## Supplementary conclusion

The remediation is limited to an isolated validation profile and does not alter:

- the active baseline ZIP;
- persistent environment settings;
- ACLs;
- ownership;
- Git history;
- Stage 1 authority state.

