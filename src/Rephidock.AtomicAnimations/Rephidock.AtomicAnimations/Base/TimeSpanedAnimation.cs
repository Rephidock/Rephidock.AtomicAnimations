using System;


namespace Rephidock.AtomicAnimations.Base {


/// <summary>
/// Base interface for all animations
/// that have a set duration.
/// </summary>
public abstract class TimeSpanedAnimation : Animation {

	/// <inheritdoc cref="TimeSpanedAnimation"/>
	protected TimeSpanedAnimation(TimeSpan duration) {
		Duration = duration;
	}

	/// <summary>The duration of the animation.</summary>
	public TimeSpan Duration { get; private init; }

	/// <inheritdoc/>
	protected sealed override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// Check end
		if (ElapsedTime >= Duration) {

			// Calculate time
			TimeSpan timeTillDuration = Duration - elapsedTimePrevious;
			TimeSpan excessTime = ElapsedTime - Duration;
			ElapsedTime = Duration;

			// Invoke implementation
			UpdateLastTimeSpannedImpl(timeTillDuration, excessTime);

			// End
			End();
			return;
		}

		// Call implementation
		UpdateTimeSpannedImpl(deltaTime);
	}

	/// <summary>
	/// <para>Implementation that is called every update.</para>
	/// <para>Called after <see cref="Animation.ElapsedTime"/> is set.</para>
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	protected abstract void UpdateTimeSpannedImpl(TimeSpan deltaTime);

	/// <summary>
	/// <para>
	/// Implementation that is called as the last update.
	/// Variation of <see cref="UpdateTimeSpannedImpl(TimeSpan)"/>.
	/// </para>
	/// </summary>
	/// <param name="deltaTimeNoExcess">Time since last update clamped to go to duration.</param>
	/// <param name="exessTime">Excess time of the update that would go over duration.</param>
	protected virtual void UpdateLastTimeSpannedImpl(TimeSpan deltaTimeNoExcess, TimeSpan exessTime) {
		UpdateTimeSpannedImpl(deltaTimeNoExcess);
	}

}

}