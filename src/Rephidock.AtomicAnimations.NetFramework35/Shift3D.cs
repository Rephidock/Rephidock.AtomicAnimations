using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// Shifts 3 values over time.
/// Is not an excusilve ease.
/// </summary>
public class Shift3D : Ease {

	readonly float shiftX;
	readonly float shiftY;
	readonly float shiftZ;
	float accumulatorX;
	float accumulatorY;
	float accumulatorZ;
	readonly Action<float, float, float> adder;

	/// <inheritdoc cref="Shift3D"/>
	public Shift3D(
		float shiftX,
		float shiftY,
		float shiftZ,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float, float> adder
	)
	: base(duration, easingCurve) {
		this.shiftX = shiftX;
		this.shiftY = shiftY;
		this.shiftZ = shiftZ;
		this.accumulatorX = 0;
		this.accumulatorY = 0;
		this.accumulatorZ = 0;
		this.adder = adder;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(0, shiftX, valueProgressNew);
		float newValueY = MoreMath.Lerp(0, shiftY, valueProgressNew);
		float newValueZ = MoreMath.Lerp(0, shiftZ, valueProgressNew);
		adder(newValueX - accumulatorX, newValueY - accumulatorY, newValueZ - accumulatorZ);
		accumulatorX = newValueX;
		accumulatorY = newValueY;
		accumulatorZ = newValueZ;
	}

}

}