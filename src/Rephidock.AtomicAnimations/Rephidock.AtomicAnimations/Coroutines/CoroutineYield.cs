using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Coroutines {


/// <summary>
/// <para>
/// An element and basis of a coroutine.
/// Coroutines are <see cref="IEnumerable{T}"/> of <see cref="CoroutineYield"/>.
/// </para>
/// <para>
/// Immutable.
/// </para>
/// </summary>
public class CoroutineYield {

	#region //// Animation

	/// <summary>
	/// The animation to start. Does not imply any delays.
	/// Must be set to <see langword="null"/> for waiting to apply.
	/// (is a discriminated union with delays)
	/// </summary>
	/// <remarks>May be null; Must not be set after init.</remarks>
	public Animation Animation { get; set; } = null;

	/// <summary>
	/// Wraps a given animation in a <see cref="CoroutineYield"/>.
	/// Does not imply any delay.
	/// </summary>
	/// <param name="animation"></param>
	public static implicit operator CoroutineYield(Animation animation) {
		return new CoroutineYield() { Animation = animation };
	}

	#endregion

	#region //// Waiting

	/// <summary>
	/// If <see langword="true"/>, causes the routine to wait for the single
	/// previously yielded animations to finish.
	/// </summary>
	/// <remarks>Must not be set after init.</remarks>
	public bool WaitLastYieldedAnimation { get; set; } = false;

	/// <summary>
	/// If <see langword="true"/>, causes the routine to wait for all
	/// previously yielded animations to finish.
	/// Overrules <see cref="WaitLastYieldedAnimation"/>
	/// </summary>
	/// <remarks>Must not be set after init.</remarks>
	public bool WaitAllYieldedAnimations { get; set; } = false;

	/// <summary>
	/// The delay to wait.
	/// If <see cref="WaitUntil"/> is set the target time is the maximum
	/// of what either delays achive.
	/// </summary>
	/// <remarks>Must not be set after init.</remarks>
	public TimeSpan WaitFor { get; set; } = TimeSpan.Zero;

	/// <summary>Elapsed time to wait until.</summary>
	/// <remarks>The next animation is started from this timespan; Must not be set after init.</remarks>
	public TimeSpan? WaitUntil { get; set; } = null;

	/// <summary>
	/// If given, the delegate will block execution
	/// until <see langword="true"/> is returned.
	/// </summary>
	/// <remarks>May be null; Must not be set after init.</remarks>
	public Func<bool> WaitUntilPredicate { get; set; } = null;

	#endregion

	#region //// Static instances

	/// <summary>
	/// A yeild that waits for all previously
	/// yieled animations in the coroutine to finish.
	/// </summary>
	/// <remarks>
	/// Is a static instance with just
	/// <see cref="WaitAllYieldedAnimations"/> being enabled.
	/// </remarks>
	public readonly static CoroutineYield Join = new CoroutineYield() { WaitAllYieldedAnimations = true };

	/// <summary>
	/// A yeild that waits for a single previous
	/// yieled animation in the coroutine to finish.
	/// </summary>
	/// <remarks>
	/// Is a static instance with just
	/// <see cref="WaitLastYieldedAnimation"/> being enabled.
	/// </remarks>
	public readonly static CoroutineYield WaitPrevious = new CoroutineYield() { WaitLastYieldedAnimation = true };

	#endregion

}

}