using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Coroutines;


#pragma warning disable CA1513 // Use ObjectDisposedException throw helper

/// <summary>
/// <para>
/// A wrapper that animates a coroutine
/// (<see cref="IEnumerable{T}"/> of <see cref="CoroutineYield"/>).
/// </para>
/// <para>
/// Note that a coroutine by itself is not an animation.
/// </para>
/// </summary>
public class CoroutineAnimation : Animation, IDisposable {

	#region //// Storage, Creation

	readonly IEnumerable<CoroutineYield> coroutine;

	/// <summary>
	/// Creates a playable animation based on given coroutine.
	/// (A runnable wrapped for a coroutine).
	/// </summary>
	public CoroutineAnimation(IEnumerable<CoroutineYield> coroutine) {
		this.coroutine = coroutine;
		innerRunner = new AnimationRunner();
		innerRunner.OnAnimationEnd += OnInnerAnimationEnd;
	}

	#endregion

	#region //// Execution

	// Enumeration
	IEnumerator<CoroutineYield>? coroutineEnumerator = null;
	TimeSpan? enumeratorFinishedTime = null;

	// Running
	readonly AnimationRunner innerRunner;
	TimeSpan innerRunnerEndTime = TimeSpan.Zero; // excludes time accounted for

	// Waiting
	Animation? lastStartedAnimation = null;
	TimeSpan lastStartedAnimationStartTime = TimeSpan.Zero;

	CoroutineYield? currentDelayYield = null;
	TimeSpan curentElementStageTime = TimeSpan.Zero;

	void OnInnerAnimationEnd(Animation animation) {
		TimeSpan animationEndTime = ElapsedTime - animation.ExcessTime;
		if (animationEndTime > innerRunnerEndTime) innerRunnerEndTime = animationEndTime;
	}

	/// <inheritdoc/>
	protected override void StartImpl() {

		// Reset enumerator
		coroutineEnumerator?.Dispose();

		coroutineEnumerator = coroutine.GetEnumerator();
		enumeratorFinishedTime = null;

		// Reset runner
		innerRunner.Clear();
		innerRunnerEndTime = TimeSpan.Zero;

		// Reset waiting flags
		lastStartedAnimation = null;
		lastStartedAnimationStartTime = TimeSpan.Zero;

		currentDelayYield = null;
		curentElementStageTime = TimeSpan.Zero;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// Dispose guard
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

		// Update runner
		innerRunner.Update(deltaTime);

		do {

			// Enumerator done: Wait for animations to finish
			if (enumeratorFinishedTime is not null) {

				if (innerRunner.HasAnimations) return;

				if (innerRunnerEndTime > enumeratorFinishedTime.Value) {
					// Animations finished after enumerator
					End(ElapsedTime - innerRunnerEndTime);
				} else {
					// Enumerator finished after animations
					End(ElapsedTime - enumeratorFinishedTime.Value);
				}
				return;
			}

			// Wait for delay to finish
			TimeSpan startTimeTarget = curentElementStageTime;

			if (currentDelayYield is not null) {

				// Suspend until next update
				if (currentDelayYield.SuspendForAnUpdate) {
					currentDelayYield = currentDelayYield with { SuspendForAnUpdate = false };
					return;
				}

				// Wait for time
				startTimeTarget = curentElementStageTime + currentDelayYield.WaitFor;

				if (currentDelayYield.WaitUntil.HasValue && currentDelayYield.WaitUntil.Value > startTimeTarget) {
					startTimeTarget = currentDelayYield.WaitUntil.Value;
				}

				if (ElapsedTime < startTimeTarget) {
					return;
				}

				// Wait for animations
				if (currentDelayYield.WaitAllYieldedAnimations) {

					// Wait
					if (innerRunner.HasAnimations) return;

					// Update target start time
					if (innerRunnerEndTime > startTimeTarget) startTimeTarget = innerRunnerEndTime;

				} else if (currentDelayYield.WaitLastYieldedAnimation && lastStartedAnimation is not null) {

					// Wait
					if (!lastStartedAnimation.HasEnded) return;

					// Update target start time
					TimeSpan animationEndTime = lastStartedAnimationStartTime + lastStartedAnimation.ElapsedTime;
					if (animationEndTime > startTimeTarget) startTimeTarget = animationEndTime;

				}

				// Wait for delegate
				if (currentDelayYield.WaitUntilPredicate is not null) {
					if (!currentDelayYield.WaitUntilPredicate.Invoke()) return;
					startTimeTarget = ElapsedTime;
				}

				// Wait finished
				currentDelayYield = null;

			}

			// Find next element
			if (coroutineEnumerator is null) {
				enumeratorFinishedTime = startTimeTarget;	// failsafe
				continue;
			}

			bool enmeratorHasNewCurrent = coroutineEnumerator.MoveNext();
			curentElementStageTime = startTimeTarget;

			// No more elements
			if (!enmeratorHasNewCurrent) {
				enumeratorFinishedTime = startTimeTarget;
				coroutineEnumerator.Dispose();
				coroutineEnumerator = null;
				continue;
			}

			CoroutineYield nextElement = coroutineEnumerator.Current;

			if (nextElement is null) {
				// Skip null elements as a fail-safe
				currentDelayYield = null;
				continue;
			}

			if (nextElement.Animation is not null) {

				// Start next animation
				lastStartedAnimation = nextElement.Animation;
				lastStartedAnimationStartTime = startTimeTarget;
				innerRunner.Run(lastStartedAnimation, ElapsedTime - startTimeTarget);
				continue;

			} else {

				// Stage delay
				currentDelayYield = nextElement;
				continue;

			}
			
			// [unreachable]

		} while (true);

	}

	#endregion

	#region //// IDisposable

	bool isDisposed = false;

	/// <inheritdoc/>
	protected virtual void Dispose(bool isDisposingManaged) {

		if (isDisposed) return;

		// Disponse managed
		if (isDisposingManaged) {
			innerRunner.Dispose();
			coroutineEnumerator?.Dispose();
			if (coroutine is IDisposable disposable) disposable.Dispose();
		}

		// Remove references
		lastStartedAnimation = null;
		currentDelayYield = null;

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
