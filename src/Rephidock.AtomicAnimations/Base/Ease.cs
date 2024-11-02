using System;


namespace Rephidock.AtomicAnimations.Base;


/// <summary>
/// Base animation for easing a value,
/// additive or exclusive.
/// </summary>
public abstract class Ease : TimedAnimation {

	readonly EasingCurve easingCurve;

	/// <inheritdoc cref="Ease"/>
	public Ease(TimeSpan duration, EasingCurve easingCurve) : base(duration) {

		// Guards
		ArgumentNullException.ThrowIfNull(easingCurve, nameof(easingCurve));

		// Set values
		this.easingCurve = easingCurve;
	}

	/// <summary>
	/// Implimentation of the ease.
	/// </summary>
	/// <param name="valueProgressNew">Next normalized value to ease to in this update</param>
	protected abstract void EaseUpdateImpl(float valueProgressNew);

	/// <inheritdoc/>
	protected sealed override void UpdateTimeSpannedImpl(TimeSpan deltaTime) {
		
		// Calculate new time progress
		float timeProgressNew = (float)(ElapsedTime / Duration);

		// Update till new time progress
		EaseUpdateImpl(easingCurve(timeProgressNew));
	}

	/// <inheritdoc/>
	protected sealed override void UpdateLastTimeSpannedImpl(TimeSpan deltaTimeNoExcess, TimeSpan exessTime) {

		// Update till end
		EaseUpdateImpl(1);
	}

}
