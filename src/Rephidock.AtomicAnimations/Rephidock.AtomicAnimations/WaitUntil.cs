using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// An empty animation that just waits,
/// until given delegate returns <see langword="true"/>.
/// The predicate given is called every update.
/// </summary>
public class WaitUntil : Animation {

	/// <inheritdoc cref="WaitUntil"/>
	public WaitUntil(Func<bool> predicate) {
		ArgumentNullException.ThrowIfNull(predicate);
		this.predicate = predicate;
	}

	readonly Func<bool> predicate;

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {
		if (predicate()) End();
	}

}
