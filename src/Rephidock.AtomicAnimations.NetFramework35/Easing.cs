﻿using System;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// <para>A delegate that represents an easing function curve.</para>
/// <para>Parameter: normalized time -a.k.a.- time progress (0..1) -a.k.a.- t</para>
/// <para>Returns: normalized value -a.k.a.- value progress (0..1)</para>
/// </summary>
/// <param name="progress">normalized time -a.k.a.- time progress (0..1) -a.k.a.- t</param>
public delegate float EasingCurve(float progress);

/// <summary>
/// A class that provides easing/tweening math
/// in a form of easing curves.
/// See also: <see cref="EasingCurve"/>.
/// </summary>
/// <remarks>
/// Derived from <see href="https://gist.github.com/Kryzarel/bba64622057f21a1d6d44879f9cd7bd4"/>
/// </remarks>
public static class Easing {

	#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	
	#region //// Linear

	public static float Linear(float t) => t;

	#endregion

	#region //// Power

	// General curves
	public static float PowerIn(float t, float power) => (float)Math.Pow(t, power);
	public static float PowerOut(float t, float power) => 1 - PowerIn(1 - t, power);
	public static float PowerInOut(float t, float power) {
		if (t < 0.5) return PowerIn(t * 2, power) / 2;
		return 1 - PowerIn((1 - t) * 2, power) / 2;
	}

	// Shortcuts for first 5 powers
	public static float QuadIn(float t) => PowerIn(t, 2);
	public static float QuadOut(float t) => PowerOut(t, 2);
	public static float QuadInOut(float t) => PowerInOut(t, 2);

	public static float CubicIn(float t) => PowerIn(t, 3);
	public static float CubicOut(float t) => PowerOut(t, 3);
	public static float CubicInOut(float t) => PowerInOut(t, 3);

	public static float QuartIn(float t) => PowerIn(t, 4);
	public static float QuartOut(float t) => PowerOut(t, 4);
	public static float QuartInOut(float t) => PowerInOut(t, 4);

	public static float QuintIn(float t) => PowerIn(t, 5);
	public static float QuintOut(float t) => PowerOut(t, 5);
	public static float QuintInOut(float t) => PowerInOut(t, 5);

	#endregion

	#region //// Sine

	public static float SineIn(float t) => 1 - (float)Math.Cos(t * (float)Math.PI / 2);
	public static float SineOut(float t) => (float)Math.Sin(t * (float)Math.PI / 2);
	public static float SineInOut(float t) => ((float)Math.Cos(t * (float)Math.PI) - 1) / -2;

	#endregion

	#region //// Expo

	public static float ExpoIn(float t) => (float)Math.Pow(2, 10 * (t - 1));
	public static float ExpoOut(float t) => 1 - ExpoIn(1 - t);
	public static float ExpoInOut(float t) {
		if (t < 0.5) return ExpoIn(t * 2) / 2;
		return 1 - ExpoIn((1 - t) * 2) / 2;
	}

	#endregion

	#region //// Circ

	public static float CircIn(float t) => -(float)Math.Sqrt(1 - t * t) + 1;
	public static float CircOut(float t) => 1 - CircIn(1 - t);
	public static float CircInOut(float t) {
		if (t < 0.5) return CircIn(t * 2) / 2;
		return 1 - CircIn((1 - t) * 2) / 2;
	}

	#endregion

	#region //// Elastic

	const float baseElasticityPower = 0.3f;

	public static float ElasticIn(float t, float elasticityMultiplier) => 1 - ElasticOut(1 - t, elasticityMultiplier);

	public static float ElasticOut(float t, float elasticityMultiplier) {
		float eleasticityPower = baseElasticityPower / elasticityMultiplier;
		return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - eleasticityPower / 4) * (2 * (float)Math.PI) / eleasticityPower) + 1;
	}

	public static float ElasticInOut(float t, float elasticityMultiplier) {
		if (t < 0.5) return ElasticIn(t * 2, elasticityMultiplier) / 2;
		return 1 - ElasticIn((1 - t) * 2, elasticityMultiplier) / 2;
	}

	// Without multiplier => multiplier is 1
	public static float ElasticIn(float t) => ElasticIn(t, 1);
	public static float ElasticOut(float t) => ElasticOut(t, 1);
	public static float ElasticInOut(float t) => ElasticInOut(t, 1);

	#endregion

	#region //// Back

	const float baseBackConstant = 1.70158f;

	public static float BackIn(float t, float backMultiplier) {
		float backValue = baseBackConstant * backMultiplier;
		return t * t * ((backValue + 1) * t - backValue);
	}

	public static float BackOut(float t, float backMultiplier) {
		return 1 - BackIn(1 - t, backMultiplier);
	}

	public static float BackInOut(float t, float backMultiplier) {
		if (t < 0.5) return BackIn(t * 2, backMultiplier) / 2;
		return 1 - BackIn((1 - t) * 2, backMultiplier) / 2;
	}

	// Without multiplier => multiplier is 1
	public static float BackIn(float t) => BackIn(t, 1);
	public static float BackOut(float t) => BackOut(t, 1);
	public static float BackInOut(float t) => BackInOut(t, 1);

	#endregion

	#region //// Bounce

	public static float BounceIn(float t) => 1 - BounceOut(1 - t);
	
	public static float BounceOut(float t) {

		const float div = 2.75f;
		const float mult = 7.5625f;

		if (t < 1 / div) {
			return mult * t * t;
		} else if (t < 2 / div) {
			t -= 1.5f / div;
			return mult * t * t + 0.75f;
		} else if (t < 2.5 / div) {
			t -= 2.25f / div;
			return mult * t * t + 0.9375f;
		} else {
			t -= 2.625f / div;
			return mult * t * t + 0.984375f;
		}
	}

	public static float BounceInOut(float t) {
		if (t < 0.5) return BounceIn(t * 2) / 2;
		return 1 - BounceIn((1 - t) * 2) / 2;
	}

	#endregion

	#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}

}