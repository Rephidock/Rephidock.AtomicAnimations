

namespace Rephidock.AtomicAnimations.Waves {


/// <summary>
/// A <see cref="Waves.Wave"/> that is horizontally shifted.
/// </summary>
public struct ShiftedWave {

	/// <summary>The wave that is to be moved.</summary>
	/// <remarks>Required. Init only.</remarks>
	public Wave Wave { get; }

	/// <summary>
	/// The horizontal offset of the wave.
	/// Higher values mean further to the right.
	/// </summary>
	/// <remarks>Init only.</remarks>
	public float Offset { get; }
	
	/// <summary>Creates a new <see cref="ShiftedWave"/>.</summary>
	public ShiftedWave(Wave wave, float offset) {
		Wave = wave;
		Offset = offset;
	}

	/// <inheritdoc cref="Wave.GetValueAt(float)"/>
	public float GetValueAt(float horizontalPosition) {
		return Wave.GetValueAt(horizontalPosition - Offset);
	}

}

}