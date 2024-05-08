using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Coroutines {


/// <summary>
/// Proives methods to work on coroutines.
/// </summary>
public static class CoroutineLinq {

	/// <summary>
	/// Creates a coroutine that is just a given animation.
	/// (Yields animation to be a coroutine).
	/// Also adds a wait for the animation to be finished.
	/// </summary>
	/// <remarks>
	/// If waiting is not desired, use a cast coverision instead.
	/// </remarks>
	public static IEnumerable<CoroutineYield> ToCoroutine(this Animation animation) {
		yield return (CoroutineYield)animation;
		yield return CoroutineYield.WaitPrevious;
	}

	/// <summary>Wraps a coroutine in an animation so it is playable.</summary>
	public static CoroutineAnimation ToAnimation(this IEnumerable<CoroutineYield> coroutine) {
		return new CoroutineAnimation(coroutine);
	}

	/// <summary>
	/// Creates a coroutine based on a given coroutine
	/// with a delegate call added at the end.
	/// </summary>
	public static IEnumerable<CoroutineYield> AppendCall(
		this IEnumerable<CoroutineYield> coroutine,
		Action delegateToCall
	) {

		// Keep coroutine
		foreach (var coroutineYield in coroutine) {
			yield return coroutineYield;
		}

		// Perform a call
		delegateToCall();
	}

	/// <summary>
	/// Creates a coroutine based on a given coroutine
	/// with a delegate call added and the start.
	/// </summary>
	public static IEnumerable<CoroutineYield> PrependCall(
		this IEnumerable<CoroutineYield> coroutine,
		Action delegateToCall
	) {

		// Perform a call
		delegateToCall();

		// Keep coroutine
		foreach (var coroutineYield in coroutine) {
			yield return coroutineYield;
		}

	}

}

}