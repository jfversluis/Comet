# Decision: Handler Mapper Event Subscription Pattern

**Author:** Amos (Controls & API Dev)
**Date:** 2025-03-08
**Status:** Proposed

## Context

`AppendToMapping` callbacks fire on every `SetVirtualView()` call, not just initial handler connection. In Comet, `ResetView()` calls `ViewHandler?.SetVirtualView(this)` on every state change, which re-fires all mapper callbacks.

Subscribing native events (e.g., `UITextField.EditingDidEnd`) inside mapper callbacks without tracking causes duplicate subscriptions that accumulate with each re-render. This caused a P0 crash in CometBaristaNotes.

## Decision

When subscribing to native platform events inside handler mapper callbacks:

1. **Track the current handler** using `ConditionalWeakTable<NativeView, EventHandler>` (or appropriate delegate type)
2. **Unsubscribe the old handler** before subscribing the new one
3. **Wrap callbacks in try-catch** as defense-in-depth against stale view references
4. **Never use anonymous lambdas** for event subscriptions that must be unsubscribed

## Pattern

```csharp
static readonly ConditionalWeakTable<NativeView, EventHandler> _trackedHandlers = new();

SomeHandler.Mapper.AppendToMapping("Key", (handler, view) =>
{
    var platformView = handler.PlatformView;
    if (_trackedHandlers.TryGetValue(platformView, out var oldHandler))
    {
        platformView.SomeEvent -= oldHandler;
        _trackedHandlers.Remove(platformView);
    }
    EventHandler newHandler = (s, e) =>
    {
        try { /* callback logic */ }
        catch (Exception ex) { Debug.WriteLine($"[Comet] callback failed: {ex.Message}"); }
    };
    platformView.SomeEvent += newHandler;
    _trackedHandlers.AddOrUpdate(platformView, newHandler);
});
```

## Consequences

- Prevents duplicate event subscriptions across re-renders
- `ConditionalWeakTable` allows native views to be GC'd normally
- try-catch prevents unhandled exceptions from crashing the app
