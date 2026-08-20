# Stage 1 Isolated NuGet Validation Profile

## Profile root

`C:\falcon\ValidationProfile`

## Required structure

- `C:\falcon\ValidationProfile\Roaming\NuGet\NuGet.Config`
- `C:\falcon\ValidationProfile\Local\NuGet`
- `C:\falcon\ValidationProfile\Packages`
- `C:\falcon\ValidationProfile\HttpCache`
- `C:\falcon\ValidationProfile\PluginsCache`
- `C:\falcon\ValidationProfile\Scratch`
- `C:\falcon\ValidationProfile\Temp`

## NuGet.Config

Content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
  </packageSources>
</configuration>
```

SHA-256:

`74AB78580D36190042CCB4552E2EA8983A93BE90016232B2142698D0BB1FE279`

## Access validation

| Check | Result |
|---|---|
| profile root exists | PASS |
| all required directories exist | PASS |
| all directories readable | PASS |
| all directories writable | PASS |
| NuGet.Config exists | PASS |
| NuGet.Config readable | PASS |
| NuGet.Config valid XML | PASS |
| no reparse points | PASS |
| no OneDrive paths | PASS |
| no profile redirection | PASS |
| no inaccessible objects | PASS |
| no secrets | PASS |

## Conclusion

The isolated NuGet profile is valid, offline, and governed for revalidation use only.

