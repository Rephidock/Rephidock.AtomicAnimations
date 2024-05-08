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

	/// <summary>
	/// Creates a <see cref="WaveEase"/> that shifts the <paramref name="wave"/>
	/// from behind an abitrary span of width <paramref name="spanWidth"/>, starting at
	/// horizontal position 0, to right beyond it,
	/// as if the wave runs through said abitrary span.
	/// </summary>
	public static WaveEase CreateRunthrough(
		Wave wave,
		float spanWidth,
		bool isDirectionReversed,
		TimeSpan duration,
		EasingCurve easingCurve,
		Action<ShiftedWave> updater
	) {

		float startOffset = -wave.Width;
		float shift = spanWidth + wave.Width;

		if (isDirectionReversed) {
			startOffset += shift;
			shift = -shift;
		}

		return new WaveEase(
			wave,
			startOffset,
			startOffset + shift,
			duration,
			easingCurve,
			updater
		);
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
