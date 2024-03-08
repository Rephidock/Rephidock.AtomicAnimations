using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// An element of <see cref="AnimationChain"/>.
/// Immutable.
/// </summary>
public record ChainElement {

	/// <summary>The delay to wait before starting the animation</summary>
	public TimeSpan DelayBeforeStart { get; init; } = TimeSpan.Zero;

	/// <summary>
	/// True if all splits need finish before starting the animation.
	/// Does not overule <see cref="DelayBeforeStart"/>.
	/// </summary>
	public bool JoinBeforeStart { get; init; } = false;

	/// <summary>
	/// The animation of this element or <see langword="null"/> to only have a delay.
	/// </summary>
	public Animation? Animation { get; init; } = null;

	/// <summary>
	/// True if animation is a split.
	/// Splits are run parallel and do not delay next chain elements,
	/// unless those elements have <see cref="JoinBeforeStart"/> flag set.
	/// </summary>
	/// <remarks>
	/// <see cref="DelayBeforeStart"/> is not part of the split.
	/// </remarks>
	public bool IsASplit { get; init; } = false;

}
