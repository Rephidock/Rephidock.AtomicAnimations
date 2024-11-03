using System;
using System.Collections.Generic;
using Rephidock.GeneralUtilities.Collections;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// <para>
/// A class that automatically plays given animations.
/// Animations are started the moment they are added
/// and forgotten the moment they are finished.
/// Plays given animations in parallel.
/// </para>
/// <para>
/// See also: <see cref="AnimationQueue"/>
/// </para>
/// <para>
/// <see cref="IDisposable"/> animations are supported and
/// are disposed of when they are finished or when the runner is cleared.
/// </para>
/// </summary>
/// <remarks>
/// Animations are updated in the order of addition.
/// </remarks>
public class AnimationRunner : IDisposable {

	readonly LinkedList<Animation> animations = new LinkedList<Animation>();

	/// <summary>
	/// Adds an animation to play.
	/// The animation is played immediately.
	/// Takes ownership of the animation.
	/// </summary>
	public void Run(Animation animation, TimeSpan initialTime) {

		// Guards
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);
		if (animation == null) throw new ArgumentNullException(nameof(animation));

		// Add animation
		animation.StartAndUpdate(initialTime);
		animations.AddLast(animation);
	}

	/// <inheritdoc cref="Run(Animation, TimeSpan)"/>
	public void Run(Animation animation) => Run(animation, TimeSpan.Zero);

	/// <inheritdoc cref="Run(Animation, TimeSpan)"/>
	/// <remarks>Initializes the lazy value instantly.</remarks>
	public void Run(Lazy<Animation> animation, TimeSpan initialTime) {
		if (animation == null) throw new ArgumentNullException(nameof(animation));
		Run(animation.Value, initialTime);
	}

	/// <inheritdoc cref="Run(Lazy{Animation}, TimeSpan)"/>
	public void Run(Lazy<Animation> animation) {
		if (animation == null) throw new ArgumentNullException(nameof(animation));
		Run(animation.Value);
	}

	/// <summary>
	/// Updates all currently playing animations with time since last update.
	/// </summary>
	/// <remarks>
	/// Negative <paramref name="deltaTime"/> has undefined behavior.
	/// </remarks>
	public void Update(TimeSpan deltaTime) {

		// Dispose guard
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

		// Update all animations
		LinkedListNode<Animation> nextNode;
		LinkedListNode<Animation> currentNode = animations.First;

		while (currentNode != null) {

			// Update and find next
			currentNode.Value.Update(deltaTime);
			nextNode = currentNode.Next;

			// Remove node with finished animation
			if (currentNode.Value.HasEnded) {

				animations.Remove(currentNode);
				OnAnimationEnd?.Invoke(currentNode.Value);

				if (currentNode.Value is IDisposable) {
					((IDisposable)currentNode.Value).Dispose();
				}

			}

			// Continue to the next node
			currentNode = nextNode;
		}

	}

	/// <summary>
	/// Clears (forgets) all animations.
	/// <see cref="IDisposable"/> animations are disposed of.
	/// </summary>
	public void Clear() {

		// Dispose of disposable animations
		foreach (var animation in animations) {
			if (animation is IDisposable) {
				((IDisposable)animation).Dispose();
			}
		}

		// Clear list
		animations.Clear();
	}

	/// <summary>True if this runner has animations playing</summary>
	public bool HasAnimations => animations.Count > 0;

	/// <summary>The number of animations playing</summary>
	public int PlayingCount => animations.Count;

	/// <summary>
	/// Event that is invoked when any given animation completes.
	/// Called after the runner forgets about the animation
	/// but right before it is disposed of.
	/// </summary>
	/// <remarks>
	/// Does not get invoked when <see cref="Clear"/> is called.
	/// </remarks>
	public event Action<Animation> OnAnimationEnd = null;

	#region //// IDisposable

	bool isDisposed = false;

	/// <inheritdoc/>
	protected virtual void Dispose(bool isDisposingManaged) {

		if (isDisposed) return;

		if (isDisposingManaged) {

			// Dispose of animations and clear the list
			this.Clear();

		}

		isDisposed = true;
	}

	/// <inheritdoc/>
	public void Dispose() {
		Dispose(isDisposingManaged: true);
		GC.SuppressFinalize(this);
	}

	#endregion

}

}