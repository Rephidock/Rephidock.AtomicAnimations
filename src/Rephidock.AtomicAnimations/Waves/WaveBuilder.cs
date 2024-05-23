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

	#endregion

}
