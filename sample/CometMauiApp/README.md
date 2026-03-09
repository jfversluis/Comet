# Comet Counter

This sample refreshes the starter app to show the evolved Comet MVU surface while keeping the app rooted in `CometApp`.

## What it demonstrates

- `Component<CounterState>` with `Render()`
- `SetState(...)` for batched state mutations
- `Reactive<string>` for lightweight reactive status text
- Current MAUI 10-safe control choices (`Border`, `Slider`, `Toggle`, `NavigationView`)

## Key files

- `MyApp.cs` — keeps `CometApp` as the root application and hosts the counter page
- `MainPage.cs` — evolved counter sample using `Component<CounterState>`

## Build

From the repository root:

```bash
dotnet build src/Comet.SourceGenerator/Comet.SourceGenerator.csproj -c Release
dotnet build src/Comet/Comet.csproj -c Release
dotnet build sample/CometMauiApp/CometMauiApp.csproj -c Release -f net10.0-maccatalyst
```

## Why the app class still inherits `CometApp`

`UseCometApp<TApp>()` still expects an `IApplication` implementation. The evolved surface usually starts at the page/component level, while the application root remains `CometApp`.
