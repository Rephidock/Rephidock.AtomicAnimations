using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// Moves 4 values over time.
/// An excusilve ease.
/// </summary>
public class Move4D : Ease {

	readonly float startX, endX;
	readonly float startY, endY;
	readonly float startZ, endZ;
	readonly float startW, endW;
	readonly Action<float, float, float, float> setter;

	/// <inheritdoc cref="Move4D"/>
	public Move4D(
		float startX,
		float startY,
		float startZ,
		float startW,
		float endX,
		float endY,
		float endZ,
		float endW,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float, float, float> setter
	)
	: base(duration, easingCurve) {
		this.startX = startX;
		this.startY = startY;
		this.startZ = startZ;
		this.startW = startW;
		this.endX = endX;
		this.endY = endY;
		this.endZ = endZ;
		this.endW = endW;
		this.setter = setter;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(startX, endX, valueProgressNew);
		float newValueY = MoreMath.Lerp(startY, endY, valueProgressNew);
		float newValueZ = MoreMath.Lerp(startZ, endZ, valueProgressNew);
		float newValueW = MoreMath.Lerp(startW, endW, valueProgressNew);
		setter(newValueX, newValueY, newValueZ, newValueW);
	}

}

}