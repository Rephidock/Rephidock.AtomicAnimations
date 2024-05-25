using System;
using System.Collections.Generic;
using System.Linq;


namespace Rephidock.AtomicAnimations.Waves;


/// <summary>
/// A class that allows to create a <see cref="Wave"/>
/// by appending segments.
/// </summary>
public class WaveBuilder {

	#region //// Storage

	/// <inheritdoc cref="Wave.StartValue"/>
	/// <remarks><c>0</c> by default.</remarks>
	public float StartValue { get; set; } = 0;

	readonly List<EasingCurve> curves = new();

	readonly List<float> destinations = new();

	readonly List<float> ends = new();

	#endregion

	#region //// Wave equivalent getters

	/// <inheritdoc cref="Wave.Curves"/>
	/// <remarks>The given list changes as more curves are added.</remarks>
	public IReadOnlyList<EasingCurve> Curves => curves.AsReadOnly();

	/// <inheritdoc cref="Wave.CurveDestinations"/>
	/// <remarks>The given list changes as more curves are added.</remarks>
	public IReadOnlyList<float> CurveDestinations => destinations.AsReadOnly();

	/// <inheritdoc cref="Wave.CurveHorizontalEnds"/>
	/// <remarks>The given list changes as more curves are added.</remarks>
	public IReadOnlyList<float> CurveHorizontalEnds => ends.AsReadOnly();

	/// <inheritdoc cref="Wave.Width"/>
	/// <remarks>The given value changes as more curves are added.</remarks>
	public float Width => curves.Count == 0 ? 0 : ends[^1];

	/// <inheritdoc cref="Wave.EndValue"/>
	/// <remarks>The given value changes as more curves are added.</remarks>
	public float EndValue => curves.Count == 0 ? StartValue : destinations[^1];

	#endregion

	#region //// Building

	/// <summary>Fluently sets <see cref="StartValue"/></summary>
	/// <returns>this</returns>
	public WaveBuilder SetStartValue(float value) {
		StartValue = value;
		return this;
	}

	/// <summary>Fluently adds a single segment to the curve.</summary>
	/// <returns>this</returns>
	/// <exception cref="ArgumentException"><paramref name="width"/> is negative.</exception>
	public WaveBuilder Add(EasingCurve curve, float destination = 1, float width = 0) {

		if (width < 0) throw new ArgumentException("Width of a curve cannot be negative", nameof(width));

		float oldWidth = Width;

		curves.Add(curve);
		destinations.Add(destination);
		ends.Add(oldWidth + width);

		return this;
	}

	/// <summary>Fluently adds a vertical gap.</summary>
	/// <returns>this</returns>
	public WaveBuilder AddGap(float destination = 1) => Add(Easing.Linear, destination, 0);

	/// <summary>Changes the destination of the most recently added gap or segment.</summary>
	/// <returns>this</returns>
	/// <exception cref="InvalidOperationException">No curve segments where added.</exception>
	public WaveBuilder To(float destination) {

		if (curves.Count == 0) throw new InvalidOperationException("Cannot change destination: no curve segments were added.");

		destinations[^1] = destination;
		return this;
	}

	/// <summary>Changes the width of the most recently added segment.</summary>
	/// <returns>this</returns>
	/// <exception cref="InvalidOperationException">No curve segments where added.</exception>
	/// <exception cref="ArgumentException"><paramref name="width"/> is negative.</exception>
	public WaveBuilder Over(float width) {

		if (curves.Count == 0) throw new InvalidOperationException("Cannot change width: no curve segments were added.");

		if (width < 0) throw new ArgumentException("Width of a curve cannot be negative", nameof(width));

		if (curves.Count == 1) {
			ends[0] = width;
		} else {
			ends[^1] = ends[^2] + width;
		}

		return this;
	}

	/// <summary>
	/// Changes the horizontal ends of the curves such that
	/// the total width of the wave is <c>1</c>.
	/// </summary>
	/// <returns>this</returns>
	public WaveBuilder NormalizeWidth() {

		if (curves.Count == 0) return this;

		float oldWidth = Width;
		for (int i = 0; i < curves.Count; i++) {
			ends[i] /= oldWidth;
		}

		return this;
	}

	/// <summary>
	/// Creates a new <see cref="Wave"/>.
	/// </summary>
	/// <exception cref="ArgumentException">No segments have been added.</exception>
	public Wave ToWave() {
		return new Wave(StartValue, curves, destinations, ends);
	}

	#endregion

}
