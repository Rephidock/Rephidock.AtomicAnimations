using System;
using System.Collections.Generic;
using Rephidock.GeneralUtilities.Collections;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


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

	Animation currentlyPlayingAnimation = null;

	readonly Queue<Lazy<Animation>> animations = new Queue<Lazy<Animation>>();

	/// <inheritdoc cref="Enqueue(Animation)"/>
	/// <remarks>
	/// <para>
	/// A <see langword="null"/> lazily initialized value
	/// will cause a <see cref="NullReferenceException"/>
	/// when that animation is staged.
	/// </para>
	/// <para>
	/// Any exception thrown when the lazy value is initialized
	/// will cause <see cref="Clear"/> to be called.
	/// </para>
	/// </remarks>
	public void Enqueue(Lazy<Animation> animation) {

		// Guards
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);
		if (animation == null) throw new ArgumentNullException(nameof(animation));

		// Add animation
		animations.Enqueue(animation);
		
		// Play if no animation is playing
		if (currentlyPlayingAnimation == null) StageNextAnimation(TimeSpan.Zero);
	}

	/// <summary>
	/// <para>
	/// Adds an animation to play.
	/// Takes ownership of the animation.
	/// </para>
	/// <para>
	/// The animation is played immediately if the queue
	/// has no animation currently playing.
	/// </para>
	/// </summary>
	public void Enqueue(Animation animation) {
		if (animation == null) throw new ArgumentNullException(nameof(animation));
		Enqueue(new Lazy<Animation>(animation));
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
		if (currentlyPlayingAnimation is IDisposable) {
			((IDisposable)currentlyPlayingAnimation).Dispose();
		}
		currentlyPlayingAnimation = null;

		// Queue empty -- done
		if (animations.Count <= 0) return;

		// Set next animation as current one and start it
		try {

			currentlyPlayingAnimation = animations.Dequeue().Value;

			if (currentlyPlayingAnimation == null) {
				throw new NullReferenceException("A previously enqueued lazy animation has a value of null.");
			}

			currentlyPlayingAnimation.StartAndUpdate(prevAnimExcessTime);

		} catch {
			Clear();
			throw;
		}

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
		if (currentlyPlayingAnimation == null) return;

		// Update the current animation
		currentlyPlayingAnimation.Update(deltaTime);
		
		// If finished: stage next animation
		while (currentlyPlayingAnimation != null && currentlyPlayingAnimation.HasEnded) {

			OnAnimationEnd?.Invoke(currentlyPlayingAnimation);

			// (also disposes of current animation)
			StageNextAnimation(currentlyPlayingAnimation.ExcessTime);

		}

	}

	/// <summary>
	/// Clears (forgets) all animations, including the one currently playing.
	/// <see cref="IDisposable"/> animations are disposed of.
	/// </summary>
	/// <remarks>
	/// Lazily initialized animations are initialized and then
	/// disposed of if necessary.
	/// </remarks>
	public void Clear() {

		// Dispose of current animation
		if (currentlyPlayingAnimation is IDisposable) {
			((IDisposable)currentlyPlayingAnimation).Dispose();
		}
		currentlyPlayingAnimation = null;

		// Init and dispose of queued animations
		foreach (var animation in animations) {
			if (animation.Value is IDisposable) {
				((IDisposable)animation.Value).Dispose();
			}
		}
		animations.Clear();

	}

	/// <summary>True if this queue has animations playing.</summary>
	public bool HasAnimations => currentlyPlayingAnimation != null;

	/// <summary>
	/// The number of animations enqueued to play.
	/// The currently playing animation is not counted.
	/// </summary>
	public int EnqueuedCount => animations.Count;

	/// <summary>
	/// Event that is invoked when any given animation completes.
	/// Called right before the animation is disposed of and
	/// the next one in the queue is staged.
	/// Called before <see cref="HasAnimations"/> is updated.
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

}