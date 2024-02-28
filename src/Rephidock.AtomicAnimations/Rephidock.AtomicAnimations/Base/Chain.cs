using System;
using System.Collections;
using System.Collections.Generic;


namespace Rephidock.AtomicAnimations.Base;


/// <summary>
/// A collection of <see cref="Animation"/>s.
/// </summary>
public abstract class Chain : Animation, IReadOnlyList<Animation> {

	#region //// Stroage, Creation

	/// <summary>List of animations in the chain.</summary>
	protected readonly List<Animation> animations;

	/// <summary>Crates a <see cref="Chain"/> with default capacity.</summary>
	public Chain() {
		animations = new List<Animation>();
	}

	/// <summary>Crates a <see cref="Chain"/> with <paramref name="capacity"/>.</summary>
	public Chain(int capacity) {
		animations = new List<Animation>(capacity);
	}

	/// <summary>Add an animation to the chain.</summary>
	/// <param name="animation">Animation to add</param>
	/// <exception cref="ArgumentNullException"><paramref name="animation"/> is null</exception>
	/// <exception cref="InvalidOperationException">Called after <see cref="Animation.StartAndUpdate()"/></exception>
	public void Add(Animation animation) {

		// Guards
		ArgumentNullException.ThrowIfNull(animation);

		if (HasStarted) {
			throw new InvalidOperationException("Cannot add animations after chain was started");
		}

		// Add animation to the list
		animations.Add(animation);
	}

	/// <summary>Adds multiple animations to the chain</summary>
	/// <param name="animations">Animations to add</param>
	/// <exception cref="ArgumentNullException">Any animation is null</exception>
	/// <exception cref="InvalidOperationException">Called after <see cref="Animation.StartAndUpdate()"/></exception>
	public void AddRange(IEnumerable<Animation> animations) {
		foreach (var animation in animations) Add(animation);
	}

	#endregion

	#region //// IReadOnlyList

	/// <inheritdoc/>
	public Animation this[int index] => animations[index];

	/// <inheritdoc/>
	public int Count => animations.Count;

	/// <inheritdoc/>
	public IEnumerator<Animation> GetEnumerator() => animations.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	#endregion

}
