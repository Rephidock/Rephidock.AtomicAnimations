using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// Shifts 2 values over time.
/// Is not an excusilve ease.
/// </summary>
public class Shift2D : Ease {

	readonly float shiftX;
	readonly float shiftY;
	float accumulatorX;
	float accumulatorY;
	readonly Action<float, float> adder;

	/// <inheritdoc cref="Shift2D"/>
	public Shift2D(
		float shiftX,
		float shiftY,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float> adder
	)
	: base(duration, easingCurve) {
		this.shiftX = shiftX;
		this.shiftY = shiftY;
		this.accumulatorX = 0;
		this.accumulatorY = 0;
		this.adder = adder;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(0, shiftX, valueProgressNew);
		float newValueY = MoreMath.Lerp(0, shiftY, valueProgressNew);
		adder(newValueX - accumulatorX, newValueY - accumulatorY);
		accumulatorX = newValueX;
		accumulatorY = newValueY;
	}

}

}