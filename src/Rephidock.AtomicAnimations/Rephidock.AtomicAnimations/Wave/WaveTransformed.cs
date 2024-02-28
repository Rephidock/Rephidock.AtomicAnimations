using System;
using Rephidock.GeneralUtilities;


namespace Rephidock.AtomicAnimations.Wave;


/// <summary>
/// A wave that is moved and stretched to the
/// specified ranges along each axis.
/// </summary>
public readonly struct WaveTransformed {

#if NET8_0_OR_GREATER
	/// <summary>The wave that is to be moved and stretched.</summary>
	public required Wave Wave { get; init; }
#else
	/// <summary>The wave that is to be moved and stretched.</summary>
	/// <remarks>Required.</remarks>
	public /* required */ Wave Wave { get; init; }
#endif

	/// <summary>
	/// The start and end of the wave on the x-axis.
	/// [0..1] if <see langword="null"/>
	/// </summary>
	public (float start, float end)? TimeRange { get; init; }

	/// <summary>
	/// The low and high bounds of the wave on the y-axis.
	/// [0..1] if <see langword="null"/>
	/// </summary>
	public (float start, float end)? ValueRange { get; init; }

	/// <summary>
	/// Returns wave value (0..1) at specified position in the wave.
	/// Positions before the wave begins and after the wave ends are valid.
	/// See also: <see cref="Wave.GetValueAt(float)"/>
	/// </summary>
	public float GetValueAt(float time) {

		// Un-transform time
		float normalizedTime;
		if (TimeRange.HasValue) {
			normalizedTime = MoreMath.ReverseLerp(TimeRange.Value.start, TimeRange.Value.end, time);
		} else {
			normalizedTime = time;
		}

		// Prove the wave
		float normalizedValue = Wave.GetValueAt(normalizedTime);

		// Transform the value
		if (!ValueRange.HasValue) return normalizedValue;
		return MoreMath.Lerp(ValueRange.Value.start, ValueRange.Value.end, normalizedValue);

	}

}
