using System;


namespace Rephidock.AtomicAnimations.Waves {

/// <summary>
/// <para>
/// Represents 2 connected easing curves,
/// the first (entrance) going from 0 to 1 and the second (exit) going from 1 to 0.
/// The change happens at switch point of normalized time (from 0 to 1).
/// </para>
/// <para>
/// Curves can be omitted to make the wave one-sided, meaning
/// the sides can be substituted with 1 or 0 completely.
/// </para>
/// </summary>
public class Wave {

	/// <summary>
	/// The rising (enterance) curve of the wave (from 0 to 1),
	/// or <see langword="null"/> to substitute 1 at every point.
	/// </summary>
	public EasingCurve EntranceCurve { get; set; } = null;

	/// <summary>
	/// The falling (exit) curve of the wave (from 1 to 0),
	/// or <see langword="null"/> to substitute 1 at every point.
	/// </summary>
	public EasingCurve ExitCurve { get; set; } = null;

	/// <summary>
	/// The point of normalized time at which the
	/// <see cref="EntranceCurve"/> and <see cref="ExitCurve"/> meet.
	/// Normalized value at this point is 1.
	/// </summary>
	public float SwitchPoint { get; set; } = 0.5f;

	/// <summary>
	/// <para>
	/// Returns the value (0..1) of the curve at <paramref name="normalizedTime"/> (0..1).
	/// </para>
	/// <para>
	/// Out of bounds time is value and will return 0 if
	/// the curve is not one-sided, -or-
	/// 0 or 1 if the curve is one-sided, depending on
	/// the direction of out of bounds.
	/// </para>
	/// </summary>
	public float GetValueAt(float normalizedTime) {

		// Out of bounds before
		if (normalizedTime <= 0) {
			if (EntranceCurve == null) return 1;
			return 0;
		}

		// Entrance
		if (normalizedTime <= SwitchPoint) {

			if (EntranceCurve == null) return 1;

			float entranceTime = normalizedTime;
			float entranceDuration = SwitchPoint;
			return EntranceCurve(entranceTime / entranceDuration);
		}

		// Exit
		if (normalizedTime < 1) {

			if (ExitCurve == null) return 1;

			float exitTime = normalizedTime - SwitchPoint;
			float exitDuration = 1 - SwitchPoint;
			return ExitCurve(exitTime / exitDuration);
		}

		// Out of bounds after
		//if (normalizedTime >= 1):
		if (ExitCurve == null) return 1;
		return 0;
		
	}

}

}