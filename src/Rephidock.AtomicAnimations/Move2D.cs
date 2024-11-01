using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// Moves 2 values over time.
/// An excusilve ease.
/// </summary>
public class Move2D : Ease {

	readonly float startX, endX;
	readonly float startY, endY;
	readonly Action<float, float> setter;

	/// <inheritdoc cref="Move2D"/>
	public Move2D(
		float startX,
		float startY,
		float endX,
		float endY,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float, float> setter
	)
	: base(duration, easingCurve) {
		this.startX = startX;
		this.startY = startY;
		this.endX = endX;
		this.endY = endY;
		this.setter = setter;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(startX, endX, valueProgressNew);
		float newValueY = MoreMath.Lerp(startY, endY, valueProgressNew);
		setter(newValueX, newValueY);
	}

}
