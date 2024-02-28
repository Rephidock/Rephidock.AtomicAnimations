using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>An empty animation that serves as a delay.</summary>
public class Delay : TimeSpanedAnimation {

	/// <summary>Creates an empty animation with given duration</summary>
	public Delay(TimeSpan duration) : base(duration) { }

	/// <inheritdoc/>
	protected override void UpdateTimeSpannedImpl(TimeSpan deltaTime) {
	}

}
