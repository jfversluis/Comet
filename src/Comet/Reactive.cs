using System;

namespace Comet
{
	/// <summary>
	/// Forward-facing reactive state wrapper. Thin subclass of <see cref="State{T}"/>
	/// that works in both the classic View/[Body] pattern and the new Component pattern.
	///
	/// Usage:
	///   readonly Reactive&lt;int&gt; count = 0;        // implicit conversion
	///   count.Value++;                               // triggers re-render
	///   new Text(() =&gt; $"Count: {count.Value}")   // automatic binding
	/// </summary>
	public class Reactive<T> : State<T>
	{
		public Reactive() : base() { }

		public Reactive(T value) : base(value) { }

		public static implicit operator T(Reactive<T> reactive) => reactive.Value;
		public static implicit operator Reactive<T>(T value) => new Reactive<T>(value);
	}
}
