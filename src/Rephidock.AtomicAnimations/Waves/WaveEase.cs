using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Waves {


/// <summary>
/// Shifts a wave horizontally over time.
/// Is an exclusive ease.
/// </summary>
public class WaveEase : Ease {

	readonly float horizontalShift;
	readonly WaveTransformed baseWave;
	readonly Action<WaveTransformed> updater;

	/// <inheritdoc cref="WaveEase"/>
	public WaveEase(
		WaveTransformed baseWave,
		float horizontalShift,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<WaveTransformed> updater
	)
	: base(duration, easingCurve) {
		this.horizontalShift = horizontalShift;
		this.baseWave = baseWave;
		this.updater = updater;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {

		// Create a new wave
		float currentShift = MoreMath.Lerp(0, horizontalShift, valueProgressNew);
		float baseTimeRangeStart = baseWave.TimeRangeStart ?? 0f;
		float baseTimeRangeEnd = baseWave.TimeRangeEnd ?? 1f;

		var waveProbe = new WaveTransformed() {
			Wave = baseWave.Wave,
			TimeRangeStart = baseTimeRangeStart + currentShift,
			TimeRangeEnd = baseTimeRangeEnd + currentShift,
			ValueRangeStart = baseWave.ValueRangeStart,
			ValueRangeEnd = baseWave.ValueRangeEnd,
		};

		// Update
		updater(waveProbe);

	}

}

}