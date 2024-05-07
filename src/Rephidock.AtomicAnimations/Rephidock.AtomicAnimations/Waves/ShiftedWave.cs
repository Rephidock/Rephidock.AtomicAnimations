

namespace Rephidock.AtomicAnimations.Waves;


/// <summary>
/// A <see cref="Waves.Wave"/> that is horizontally shifted.
/// </summary>
public readonly struct ShiftedWave {

#if NET8_0_OR_GREATER
	/// <summary>The wave that is to be moved and stretched.</summary>
	public required Wave Wave { get; init; }
#else
	/// <summary>The wave that is to be moved and stretched.</summary>
	/// <remarks>Required.</remarks>
	public /* required */ Wave Wave { get; init; }
#endif

	/// <summary>The horizontal offset of the wave</summary>
	public float Offset { get; init; }

	/// <inheritdoc cref="Wave.GetValueAt(float)"/>
	public float GetValueAt(float horizontalPosition) {
		return Wave.GetValueAt(horizontalPosition - Offset);
	}

}
