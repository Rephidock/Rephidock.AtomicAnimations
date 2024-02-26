using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// Shifts 1 value over time.
/// Is not an excusilve ease.
/// </summary>
public class Shift1D : Ease {

	readonly float shift;
	float accumulator;
	readonly Action<float> adder;

	/// <inheritdoc cref="Shift1D"/>
	public Shift1D(
		float shift,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float> adder
	)
	: base(duration, easingCurve) {
		this.shift = shift;
		this.accumulator = 0;
		this.adder = adder;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValue = MoreMath.Lerp(0, shift, valueProgressNew);
		adder(newValue - accumulator);
		accumulator = newValue;
	}

}
