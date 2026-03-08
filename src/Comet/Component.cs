using System;
using System.Diagnostics;
using Microsoft.Maui;

namespace Comet
{
	/// <summary>
	/// Base class for MauiReactor-style components that use a Render() method
	/// instead of the [Body] attribute. Extends View so it participates in the
	/// existing Comet lifecycle (handlers, hot reload, diffing, etc.).
	/// </summary>
	public abstract class Component : View
	{
		bool _mounted;

		protected Component()
		{
			// Wire Body to call Render so the existing View pipeline
			// (GetRenderView → Body.Invoke → diff) works unchanged.
			Body = () => Render();
		}

		/// <summary>
		/// Return the view tree for this component. Called on every render cycle.
		/// </summary>
		public abstract View Render();

		/// <summary>
		/// Called once after the component's handler is first set (i.e., the
		/// component becomes part of the live visual tree).
		/// </summary>
		protected virtual void OnMounted() { }

		/// <summary>
		/// Called when the component is being disposed / removed from the tree.
		/// </summary>
		protected virtual void OnWillUnmount() { }

		protected override void OnLoaded()
		{
			base.OnLoaded();
			if (!_mounted)
			{
				_mounted = true;
				OnMounted();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _mounted)
			{
				_mounted = false;
				OnWillUnmount();
			}
			base.Dispose(disposing);
		}
	}

	/// <summary>
	/// Component with a typed state object. Provides SetState() for batched
	/// mutations that trigger a single re-render, similar to MauiReactor's pattern.
	/// </summary>
	/// <typeparam name="TState">
	/// A plain class with a parameterless constructor. Properties are the
	/// component's mutable data. Does NOT need to extend BindingObject.
	/// </typeparam>
	public abstract class Component<TState> : Component, IComponentWithState
		where TState : class, new()
	{
		TState _state;

		/// <summary>
		/// The component's typed state object. Initialized lazily on first access.
		/// Hides View.State (BindingState) — the typed state is the public API for Components.
		/// </summary>
		public new TState State => _state ??= new TState();

		/// <summary>
		/// Mutate state and trigger a re-render. Safe to call from any thread.
		/// Multiple mutations within a single SetState are batched into one render pass.
		/// </summary>
		protected void SetState(Action<TState> mutator)
		{
			if (mutator == null)
				throw new ArgumentNullException(nameof(mutator));

			// Ensure state is initialized
			var state = State;

			StateManager.BeginBatch();
			try
			{
				mutator(state);
			}
			finally
			{
				StateManager.EndBatch();
			}

			// Trigger re-render on the main thread
			ThreadHelper.RunOnMainThread(() => Reload());
		}

		// -- IComponentWithState --

		object IComponentWithState.GetStateObject() => _state;

		void IComponentWithState.TransferStateFrom(IComponentWithState source)
		{
			if (source is Component<TState> typed && typed._state != null)
			{
				_state = typed._state;
			}
		}

		// -- Hot reload integration --

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_state = default;
			}
			base.Dispose(disposing);
		}
	}

	/// <summary>
	/// Component with both state and props. Props are set by the parent component
	/// and flow top-down; state is internal and managed via SetState().
	/// </summary>
	/// <typeparam name="TState">Internal mutable state class.</typeparam>
	/// <typeparam name="TProps">
	/// Parent-supplied props class. Should be treated as read-only by the component.
	/// </typeparam>
	public abstract class Component<TState, TProps> : Component<TState>, IComponentWithState
		where TState : class, new()
		where TProps : class, new()
	{
		TProps _props;

		/// <summary>
		/// Props supplied by the parent. Assigning new Props triggers a re-render.
		/// </summary>
		public TProps Props
		{
			get => _props ??= new TProps();
			set
			{
				_props = value ?? new TProps();
				// Props changed — re-render
				ThreadHelper.RunOnMainThread(() => Reload());
			}
		}

		// -- IComponentWithState (extend base to also transfer props) --

		object IComponentWithState.GetStateObject() =>
			((IComponentWithState)(Component<TState>)this).GetStateObject();

		void IComponentWithState.TransferStateFrom(IComponentWithState source)
		{
			((IComponentWithState)(Component<TState>)this).TransferStateFrom(source);
			if (source is Component<TState, TProps> typed && typed._props != null)
			{
				_props = typed._props;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_props = default;
			}
			base.Dispose(disposing);
		}
	}
}
