using System;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Waves;


/// <summary>
/// Shifts a <see cref="Wave"/> horizontally over time.
/// Is an exclusive ease.
/// </summary>
public class WaveEase : Ease {

	readonly float startOffset;
	readonly float endOffset;
	readonly Wave baseWave;
	readonly Action<ShiftedWave> updater;

	/// <inheritdoc cref="WaveEase"/>
	public WaveEase(
		Wave wave,
		float startOffset,
		float endOffset,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<ShiftedWave> updater
	)
	: base(duration, easingCurve) {
		this.startOffset = startOffset;
		this.endOffset = endOffset;
		this.baseWave = wave;
		this.updater = updater;
	}

	/// <inheritdoc/>
	protected override void EaseUpdateImpl(float valueProgressNew) {

		// Create a shifted wave
		var shiftedWave = new ShiftedWave() {
			Wave = baseWave,
			Offset = MoreMath.Lerp(startOffset, endOffset, valueProgressNew),
		};

		// Update
		updater(shiftedWave);

	}

}
