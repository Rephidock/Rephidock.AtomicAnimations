using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Coroutines;


/// <summary>
/// <para>
/// An element and basis of a coroutine.
/// Coroutines are <see cref="IEnumerable{T}"/> of <see cref="CoroutineYield"/>.
/// May hold an animation or a wait instruction.
/// </para>
/// <para>
/// Immutable.
/// </para>
/// <para>
/// See also: <see cref="CoroutineAnimation"/>.
/// </para>
/// </summary>
public record CoroutineYield {

	#region //// Animation

	/// <summary>
	/// The animation to start. Does not imply any delays.
	/// Must be set to <see langword="null"/> for waiting to apply.
	/// (is a discriminated union with delays)
	/// </summary>
	public Animation? Animation { get; init; } = null;

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
	public bool WaitLastYieldedAnimation { get; init; } = false;

	/// <summary>
	/// If <see langword="true"/>, causes the routine to wait for all
	/// previously yielded animations to finish.
	/// Overrules <see cref="WaitLastYieldedAnimation"/>
	/// </summary>
	public bool WaitAllYieldedAnimations { get; init; } = false;

	/// <summary>
	/// The delay to wait.
	/// If <see cref="WaitUntil"/> is set the target time is the maximum
	/// of what either delays achieve.
	/// </summary>
	public TimeSpan WaitFor { get; init; } = TimeSpan.Zero;

	/// <summary>Elapsed time to wait until.</summary>
	/// <remarks>The next animation is started from this timespan.</remarks>
	public TimeSpan? WaitUntil { get; init; } = null;

	/// <summary>
	/// If given, the delegate will deny continuing
	/// until <see langword="true"/> is returned.
	/// Is called every update until satisfied.
	/// </summary>
	/// <remarks>
	/// The only waiting option that affects start times of animations
	/// without relating to <see cref="TimeSpan"/>.
	/// The following animation is launched on the same update
	/// the predicate returns <see langword="true"/>.
	/// </remarks>
	public Func<bool>? WaitUntilPredicate { get; init; } = null;

	/// <summary>
	/// If <see langword="true"/>, suspends execution until the next update call
	/// <u>without</u> influencing the start times of the following animations.
	/// </summary>
	/// <remarks>
	/// Is the only waiting option that does not affect start times of animations.
	/// </remarks>
	public bool SuspendForAnUpdate { get; init; } = false;

	#endregion

	#region //// Static instances

	/// <summary>
	/// A yield that waits for all previously
	/// yielded animations in the coroutine to finish.
	/// </summary>
	/// <remarks>
	/// Is a static instance with just
	/// <see cref="WaitAllYieldedAnimations"/> being enabled.
	/// </remarks>
	public readonly static CoroutineYield Join = new() { WaitAllYieldedAnimations = true };

	/// <summary>
	/// A yield that waits for a single previous
	/// yielded animation in the coroutine to finish.
	/// </summary>
	/// <remarks>
	/// Is a static instance with just
	/// <see cref="WaitLastYieldedAnimation"/> being enabled.
	/// </remarks>
	public readonly static CoroutineYield WaitPrevious = new() { WaitLastYieldedAnimation = true };

	/// <summary>
	/// A yield that suspends execution until the next update call
	/// <u>without</u> influencing the start times of the following animations.
	/// </summary>
	/// <remarks>
	/// Is a static instance with just
	/// <see cref="SuspendForAnUpdate"/> being enabled.
	/// </remarks>
	public readonly static CoroutineYield Suspend = new() { SuspendForAnUpdate = true };

	#endregion

}
