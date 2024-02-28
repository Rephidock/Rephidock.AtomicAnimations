using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// Shifts 4 values over time.
/// Is not an excusilve ease.
/// </summary>
public class Shift4D : Ease {

	readonly float shiftX;
	readonly float shiftY;
	readonly float shiftZ;
	readonly float shiftW;
	float accumulatorX;
	float accumulatorY;
	float accumulatorZ;
	float accumulatorW;
	readonly Action<float, float, float, float> adder;

	/// <inheritdoc cref="Shift4D"/>
	public Shift4D(
		float shiftX,
		float shiftY,
		float shiftZ,
		float shiftW,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float, float, float> adder
	)
	: base(duration, easingCurve) {
		this.shiftX = shiftX;
		this.shiftY = shiftY;
		this.shiftY = shiftZ;
		this.shiftY = shiftW;
		this.accumulatorX = 0;
		this.accumulatorY = 0;
		this.accumulatorZ = 0;
		this.accumulatorW = 0;
		this.adder = adder;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(0, shiftX, valueProgressNew);
		float newValueY = MoreMath.Lerp(0, shiftY, valueProgressNew);
		float newValueZ = MoreMath.Lerp(0, shiftZ, valueProgressNew);
		float newValueW = MoreMath.Lerp(0, shiftW, valueProgressNew);
		adder(newValueX - accumulatorX, newValueY - accumulatorY, newValueZ - accumulatorZ, newValueW - accumulatorW);
		accumulatorX = newValueX;
		accumulatorY = newValueY;
		accumulatorZ = newValueZ;
		accumulatorW = newValueW;
	}

}

}