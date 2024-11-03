using System;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// Moves 1 value over time.
/// An excusilve ease.
/// </summary>
public class Move1D : Ease {

	readonly float startX, endX;
	readonly Action<float> setter;

	/// <inheritdoc cref="Move1D"/>
	public Move1D(
		float startX,
		float endX,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<float> setter
	)
	: base(duration, easingCurve) {
		this.startX = startX;
		this.endX = endX;
		this.setter = setter;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {
		float newValueX = MoreMath.Lerp(startX, endX, valueProgressNew);
		setter(newValueX);
	}

}

}