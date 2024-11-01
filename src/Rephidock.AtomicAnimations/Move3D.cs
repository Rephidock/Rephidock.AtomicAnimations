using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// Moves 3 values over time.
/// An excusilve ease.
/// </summary>
public class Move3D : Ease {

	readonly float startX, endX;
	readonly float startY, endY;
	readonly float startZ, endZ;
	readonly Action<float, float, float> setter;

	/// <inheritdoc cref="Move3D"/>
	public Move3D(
		float startX,
		float startY,
		float startZ,
		float endX,
		float endY,
		float endZ,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float, float> setter
	)
	: base(duration, easingCurve) {
		this.startX = startX;
		this.startY = startY;
		this.startZ = startZ;
		this.endX = endX;
		this.endY = endY;
		this.endZ = endZ;
		this.setter = setter;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(startX, endX, valueProgressNew);
		float newValueY = MoreMath.Lerp(startY, endY, valueProgressNew);
		float newValueZ = MoreMath.Lerp(startZ, endZ, valueProgressNew);
		setter(newValueX, newValueY, newValueZ);
	}

}
