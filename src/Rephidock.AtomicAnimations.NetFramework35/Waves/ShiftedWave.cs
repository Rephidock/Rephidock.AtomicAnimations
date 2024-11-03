

namespace Rephidock.AtomicAnimations.Waves {


/// <summary>
/// A <see cref="Waves.Wave"/> that is horizontally shifted.
/// </summary>
public struct ShiftedWave {

	/// <summary>The wave that is to be moved.</summary>
	/// <remarks>Required. Init only.</remarks>
	public /* required init */ Wave Wave { get; set; }

	/// <summary>
	/// The horizontal offset of the wave.
	/// Higher values mean furhter to the right.
	/// </summary>
	/// <remarks>Init only.</remarks>
	public float Offset { get; set; }

	/// <inheritdoc cref="Wave.GetValueAt(float)"/>
	public float GetValueAt(float horizontalPosition) {
		return Wave.GetValueAt(horizontalPosition - Offset);
	}

}

}