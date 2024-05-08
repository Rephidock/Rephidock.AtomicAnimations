using System;
using Rephidock.GeneralUtilities;


namespace Rephidock.AtomicAnimations.Waves {


/// <summary>
/// A wave that is moved and stretched to the
/// specified ranges along each axis.
/// </summary>
public struct WaveTransformed {

	/// <summary>The wave that is to be moved and stretched.</summary>
	/// <remarks>Required.</remarks>
	public /* required */ Wave Wave { get; set; }

	/// <summary>
	/// The start and end of the wave on the x-axis.
	/// [0..1] if <see langword="null"/>
	/// </summary>
	public float? TimeRangeStart { get; set; }

	/// <inheritdoc cref="TimeRangeStart"/>
	public float? TimeRangeEnd { get; set; }

	/// <summary>
	/// The low and high bounds of the wave on the y-axis.
	/// [0..1] if <see langword="null"/>
	/// </summary>
	public float? ValueRangeStart { get; set; }

	/// <inheritdoc cref="ValueRangeStart"/>
	public float? ValueRangeEnd { get; set; }

	/// <summary>
	/// Returns wave value at specified position in the wave.
	/// Positions before the wave begins and after the wave ends are valid.
	/// See also: <see cref="Wave.GetValueAt(float)"/>
	/// </summary>
	public float GetValueAt(float time) {

		// Un-transform time
		float normalizedTime;
		if (TimeRangeStart.HasValue || TimeRangeEnd.HasValue) {
			normalizedTime = MoreMath.ReverseLerp(TimeRangeStart ?? 0, TimeRangeEnd ?? 1, time);
		} else {
			normalizedTime = time;
		}

		// Probe the wave
		float normalizedValue = Wave.GetValueAt(normalizedTime);

		// Transform the value
		if (!ValueRangeStart.HasValue && !ValueRangeEnd.HasValue) return normalizedValue;
		return MoreMath.Lerp(ValueRangeStart ?? 0, ValueRangeEnd ?? 1, normalizedValue);

	}

}

}