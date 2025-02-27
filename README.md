# GUID Variant

`System.Guid.NewGuid()` generates a version 4 [UUID](https://datatracker.ietf.org/doc/html/rfc9562). This library generates some of the other versions.

## Namespace-Based

Versions 3 and 5 can be generated with `IHashGuid` objects in the `HashGuid` class. `HashGuid` also includes a custom (version 8) implementation based on SHA-256.

## Timestamp-Based

Version 7 Guids can be generated with the `GuidV7` class. `GuidV7.NewGuid()` does not generate Guids monotonically. `GuidV7.NewGuidBatch(...)` is only monotonic on a per-batch basis. Both methods are thread-safe.
