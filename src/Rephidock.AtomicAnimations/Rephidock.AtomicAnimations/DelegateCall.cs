using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// An animation that is just a delegate call.
/// Has a duration of 0.
/// </summary>
public class DelegateCall : Animation {

	readonly Action<TimeSpan> delegateToCall;
	
	/// <summary>
	/// Creates a DelegateCall with a delegate
	/// that accepts initial time, which may be non-zero.
	/// </summary>
	public DelegateCall(Action<TimeSpan> delegateToCall) {
		this.delegateToCall = delegateToCall;
		Duration = TimeSpan.Zero;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime) {
	}

	/// <inheritdoc/>
	protected override void LastUpdateImpl(TimeSpan deltaTimeNoExcess, TimeSpan exessTime) {
		// Done in update so that the delegate is called after the start event
		delegateToCall(exessTime);
	}

}
