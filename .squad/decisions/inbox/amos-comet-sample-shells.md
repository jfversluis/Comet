# 2026-03-08 — Pure Comet sample shells should collapse back to CometApp + TabView

**Owner:** Amos  
**Status:** Proposed

## Decision

For legacy samples that are still fundamentally pure Comet apps, prefer a direct `CometApp` root with `TabView` + `NavigationView` tabs instead of a MAUI `Shell` / `ContentPage` host wrapper around Comet pages.

## Why

- It keeps the sample teaching the Comet surface instead of teaching MAUI hosting indirection by accident.
- It matches the evolved starter/template path already established by the counter and coffee reference samples.
- It removes stale reasons to keep `Microsoft.Maui.Controls.Compatibility` around in samples that no longer need MAUI wrapper pages.

## Applied in this wave

- `sample/CometAllTheLists` moved from MAUI Shell-hosted Comet pages to a direct Comet `TabView` shell.

## Exceptions

- Keep the MAUI host wrapper when the sample explicitly demonstrates MAUI hosting, native interop, or mixed MAUI/Comet composition as the lesson.
