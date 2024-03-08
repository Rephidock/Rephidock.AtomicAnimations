using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// An element of <see cref="AnimationChain"/>.
/// Immutable.
/// </summary>
public class ChainElement {

	/// <summary>The delay to wait before starting the animation</summary>
	/// <remarks>Should not be changed after creation</remarks>
	public TimeSpan DelayBeforeStart { get; set; } = TimeSpan.Zero;

	/// <summary>
	/// True if all splits need finish before starting the animation.
	/// Does not overule <see cref="DelayBeforeStart"/>.
	/// </summary>
	/// <remarks>Should not be changed after creation</remarks>
	public bool JoinBeforeStart { get; set; } = false;

	/// <summary>
	/// The animation of this element or <see langword="null"/> to only have a delay.
	/// </summary>
	/// <remarks>Should not be changed after creation</remarks>
	public Animation Animation { get; set; } = null;

	/// <summary>
	/// True if animation is a split.
	/// Splits are run parallel and do not delay next chain elements,
	/// unless those elements have <see cref="JoinBeforeStart"/> flag set.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="DelayBeforeStart"/> is not part of the split.
	/// </para>
	/// <para>
	/// Should not be changed after creation
	/// </para>
	/// </remarks>
	public bool IsASplit { get; set; } = false;

}

}