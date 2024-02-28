using System;
using System.Collections;
using System.Collections.Generic;


namespace Rephidock.AtomicAnimations.Base {


/// <summary>
/// A collection of <see cref="Animation"/>s.
/// Is a readonly list.
/// </summary>
public abstract class Chain : Animation, IList<Animation> {

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
		if (animation == null) { 
			throw new ArgumentNullException(nameof(animation));	
		}

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
	public bool IsReadOnly => true;

	/// <inheritdoc/>
	public Animation this[int index] {
		get { return animations[index] ; }
		set { throw new NotSupportedException(); }
	}

	/// <inheritdoc/>
	public int Count => animations.Count;

	/// <inheritdoc/>
	public IEnumerator<Animation> GetEnumerator() => animations.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public int IndexOf(Animation item) => animations.IndexOf(item);

	/// <inheritdoc/>
	public void CopyTo(Animation[] array, int arrayIndex) => animations.CopyTo(array, arrayIndex);
	
	/// <inheritdoc/>	
	public bool Contains(Animation item) => animations.Contains(item);

	/// <inheritdoc/>
	public void Insert(int index, Animation item) { throw new NotSupportedException(); }
	
	/// <inheritdoc/>
	public void RemoveAt(int index) { throw new NotSupportedException(); }
	
	/// <inheritdoc/>	
	public void Clear() { throw new NotSupportedException(); }

	/// <inheritdoc/>
	public bool Remove(Animation item) { throw new NotSupportedException(); }

	#endregion

}

}