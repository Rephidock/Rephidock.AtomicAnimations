using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


#pragma warning disable CA1513 // Use ObjectDisposedException throw helper

/// <summary>
/// <para>
/// A class that automatically plays given animations.
/// In contrast to <see cref="AnimationRunner"/>, only runs
/// one animation at a time, keeping other in a queue.
/// </para>
/// <para>
/// Accounts for excess time but only for already enqueued animations.
/// </para>
/// <para>
/// See also: <see cref="AnimationRunner"/>, <see cref="Coroutines.CoroutineAnimation"/>.
/// </para>
/// <para>
/// <see cref="IDisposable"/> animations are supported and
/// are disposed of when they are finished or when the queue is cleared.
/// </para>
/// </summary>
public class AnimationQueue : IDisposable {

	Animation? currentlyPlayingAnimation = null;

	readonly Queue<Animation> animations = new();

	/// <summary>
	/// <para>
	/// Adds an animation to play.
	///	Takes owenership of the animation.
	/// </para>
	/// <para>
	/// The animation is played immediately if the queue
	/// has no animation currently playing.
	/// </para>
	/// </summary>
	public void Enqueue(Animation animation) {

		// Guards
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);
		ArgumentNullException.ThrowIfNull(animation);

		// Add animation
		animations.Enqueue(animation);
		
		// Play if no animation is playing
		if (currentlyPlayingAnimation is null) StageNextAnimation(TimeSpan.Zero);
	}

	/// <summary>
	/// Disposes of the current animation and sets the
	/// next animation to be the current.
	/// </summary>
	/// <param name="prevAnimExcessTime">
	/// The excess time of the previous animation
	/// to serve as start time for the next animation.
	/// </param>
	private void StageNextAnimation(TimeSpan prevAnimExcessTime) {

		// Clear current animation
		if (currentlyPlayingAnimation is IDisposable disposable) disposable.Dispose();
		currentlyPlayingAnimation = null;

		// Queue empty -- done
		if (animations.Count <= 0) return;

		// Set next anmation as current one and start it
		currentlyPlayingAnimation = animations.Dequeue();
		currentlyPlayingAnimation.StartAndUpdate(prevAnimExcessTime);
	}

	/// <summary>
	/// Updates the currently playing animation and
	/// stages the next one if it finishes.
	/// </summary>
	/// <remarks>
	/// Negative <paramref name="deltaTime"/> has undefined behavior.
	/// </remarks>
	public void Update(TimeSpan deltaTime) {

		// Dispose guard
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

		// Do nothing if no animation is playing
		if (currentlyPlayingAnimation is null) return;

		// Update the current animation
		currentlyPlayingAnimation.Update(deltaTime);
		
		// If finished: stage next animation
		while (currentlyPlayingAnimation is not null && currentlyPlayingAnimation.HasEnded) {

			OnAnimationEnd?.Invoke(currentlyPlayingAnimation);

			// (also disposes of current animation)
			StageNextAnimation(currentlyPlayingAnimation.ExcessTime);

		}

	}

	/// <summary>
	/// Clears (forgets) all animations, including the one currently playing.
	/// <see cref="IDisposable"/> animations are disposed of.
	/// </summary>
	public void Clear() {

		// Dispose of current animation
		if (currentlyPlayingAnimation is IDisposable disposableCurrent) {
			disposableCurrent.Dispose();
		}
		currentlyPlayingAnimation = null;

		// Dispose of queued animations
		foreach (var animation in animations) {
			if (animation is IDisposable disposable) {
				disposable.Dispose();
			}
		}
		animations.Clear();

	}

	/// <summary>True if this queue has animations playing.</summary>
	public bool HasAnimations => currentlyPlayingAnimation is not null;

	/// <summary>
	/// The number of animations enqueued to play.
	/// The currently playing animation is not counted.
	/// </summary>
	public int EnqueuedCount => animations.Count;

	/// <summary>
	/// Event that is invoked when any given animaton completes.
	/// Called right before the animation is disposed of and
	/// the next one in the queue is staged.
	/// </summary>
	/// <remarks>
	/// Does not get invoked when <see cref="Clear"/> is called.
	/// </remarks>
	public event Action<Animation>? OnAnimationEnd = null;

	#region //// IDisposable

	bool isDisposed = false;

	/// <inheritdoc/>
	protected virtual void Dispose(bool isDisposingManaged) {

		if (isDisposed) return;

		if (isDisposingManaged) {

			// Dispose of animations and clear the queue
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

#pragma warning restore CA1513 // Use ObjectDisposedException throw helper
