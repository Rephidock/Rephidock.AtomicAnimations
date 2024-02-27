using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// An empty animation that has no operation,
/// but still has events.
/// </summary>
public class AnimationNoOp : TimeSpanedAnimation {

	/// <summary>Creates an empty animation with given duration</summary>
	public AnimationNoOp(TimeSpan duration) {
		Duration = duration;
	}

	/// <summary>Creates an empty animation with duration of 0</summary>
	public AnimationNoOp() : this(TimeSpan.Zero) { }

	/// <inheritdoc/>
	protected override void UpdateTimeSpannedImpl(TimeSpan deltaTime) {
	}

}
