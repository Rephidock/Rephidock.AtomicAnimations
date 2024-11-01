using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// Shifts 1 value over time.
/// Is not an excusilve ease.
/// </summary>
public class Shift1D : Ease {

	readonly float shiftX;
	float accumulatorX;
	readonly Action<float> adder;

	/// <inheritdoc cref="Shift1D"/>
	public Shift1D(
		float shiftX,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float> adder
	)
	: base(duration, easingCurve) {
		this.shiftX = shiftX;
		this.accumulatorX = 0;
		this.adder = adder;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(0, shiftX, valueProgressNew);
		adder(newValueX - accumulatorX);
		accumulatorX = newValueX;
	}

}
