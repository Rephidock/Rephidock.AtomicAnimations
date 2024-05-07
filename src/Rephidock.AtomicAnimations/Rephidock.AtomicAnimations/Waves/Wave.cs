using Rephidock.GeneralUtilities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Rephidock.AtomicAnimations.Waves;


/// <summary>
/// <para>
/// Represents multiple <see cref="EasingCurve"/>s connected together.
/// Each curve's time contributes to wave's width.
/// Each curve can also be scaled.
/// </para>
/// <para>
/// Unlike an <see cref="EasingCurve"/>, a <see cref="Wave"/> has
/// an arbitrary width, starting and endling values.
/// </para>
/// <para>
/// Immutable.
/// </para>
/// </summary>
public record Wave {

	/// <summary>
	/// <para>
	/// The individual curves, contributing to the wave, in normalized form.
	/// </para>
	/// <para>
	/// Not empty.
	/// </para>
	/// </summary>
	public IReadOnlyList<EasingCurve> Curves { get; private init; }

	/// <summary>
	/// <para>
	/// The values at which each curve in <see cref="Curves"/> ends
	/// and the next one begins.
	/// </para>
	/// <para>
	/// Has the same length as <see cref="Curves"/>.
	/// </para>
	/// </summary>
	public IReadOnlyList<float> CurveDestinations { get; private init; }

	/// <summary>
	/// <para>
	/// The "times" at which each curve in <see cref="Curves"/> ends
	/// and the next one begins.
	/// Is also the accumulative width of the wave up to and
	/// including a curve.
	/// </para>
	/// <para>
	/// Has the same length as <see cref="Curves"/>.
	/// Values are always acceding or equal to the previous value.
	/// </para>
	/// <para>
	/// If two or more ends are equal it is a vertical gap. 
	/// The value at the end is the end of the gap.
	/// </para>
	/// </summary>
	public IReadOnlyList<float> CurveHorizontalEnds { get; private init; }

	/// <summary>
	/// The value of the wave at x position <c>0</c>.
	/// Also the value the first curve starts at.
	/// </summary>
	public float StartValue { get; private init; }

	/// <summary>The total width of the curve.</summary>
	public float Width => CurveHorizontalEnds[^1];

	/// <summary>The value the wave ends at.</summary>
	public float EndValue => CurveDestinations[^1];


	internal Wave(
		float start,
		IEnumerable<EasingCurve> curves,
		IEnumerable<float> destinations,
		IEnumerable<float> ends
	) {
		StartValue = start;
		Curves = curves.ToArray();
		CurveDestinations = destinations.ToArray();
		CurveHorizontalEnds = ends.ToArray();

		if (Curves.Count == 0) {
			throw new ArgumentException("Wave must have at least 1 curve");
		}

		if (!(Curves.Count == CurveDestinations.Count && Curves.Count == CurveHorizontalEnds.Count)) {
			throw new ArgumentException("Curve, destination and end arrays must all be of the same length");
		}
	}

	/// <summary>
	/// <para>
	/// Returns the wave's value at a give "time" (<paramref name="horizontalPosition"/>).
	/// </para>
	/// <para>
	/// Out of bounds "time" is a valid input and will return
	/// <see cref="StartValue"/> or <see cref="EndValue"/>,
	/// depending on the bound.
	/// </para>
	/// </summary>
	public float GetValueAt(float horizontalPosition) {

		// Out of bounds checks
		if (horizontalPosition >= Width) return EndValue;
		if (horizontalPosition <= 0) return StartValue;

		// Find the last curve holding the horizontal position
		for (int i = Curves.Count - 1; i >= 0; i--) {

			if (horizontalPosition > CurveHorizontalEnds[i]) continue;

			float currentValueStart = i == 0 ? StartValue : CurveDestinations[i - 1];
			float currentValueEnd = CurveDestinations[i];
			float currentTimeStart = i == 0 ? 0 : CurveHorizontalEnds[i - 1];
			float currentTimeEnd = CurveHorizontalEnds[i];

			// Is a jump (width of 0)
			if (currentTimeStart >= currentTimeEnd) {
				return currentValueEnd;
			}
			
			// Missing curve (just in case)
			if (Curves[i] is null) {
				return currentValueEnd;
			}

			// Find value according to the curve
			float currentNormalizedTime = MoreMath.ReverseLerp(currentTimeStart, currentTimeEnd, horizontalPosition);
			float currentNormalizedValue = Curves[i](currentNormalizedTime);
			return MoreMath.Lerp(currentValueStart, currentValueEnd, currentNormalizedValue);

		}

		// [unreachable]
		return StartValue;

	}

}
