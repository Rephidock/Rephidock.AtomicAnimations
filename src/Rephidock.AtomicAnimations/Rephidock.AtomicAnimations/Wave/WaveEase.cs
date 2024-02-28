using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Wave {


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
		(float, float) baseTimeRange = baseWave.TimeRange ?? (0f, 1f);
		
		var waveProbe = new WaveTransformed() {
			Wave = baseWave.Wave,
			TimeRange = (baseTimeRange.Item1 + currentShift, baseTimeRange.Item2 + currentShift),
			ValueRange = baseWave.ValueRange
		};

		// Update
		updater(waveProbe);

	}

}

}