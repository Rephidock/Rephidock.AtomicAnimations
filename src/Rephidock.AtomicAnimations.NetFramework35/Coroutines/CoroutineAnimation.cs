﻿using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations.Coroutines {


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
	IEnumerator<CoroutineYield> coroutineEnumerator = null;
	TimeSpan? enumeratorFinishedTime = null;

	// Running
	readonly AnimationRunner innerRunner;
	TimeSpan innerRunnerEndTime = TimeSpan.Zero; // excludes time accounted for

	// Waiting
	Animation lastStartedAnimation = null;
	TimeSpan lastStartedAnimationStartTime = TimeSpan.Zero;

	CoroutineYield currentDelayYield = null;
	TimeSpan currentElementStageTime = TimeSpan.Zero;

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
		currentElementStageTime = TimeSpan.Zero;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// Dispose guard
		if (isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

		// Update runner
		innerRunner.Update(deltaTime);

		do {

			// Enumerator done: Wait for animations to finish
			if (enumeratorFinishedTime != null) {

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
			TimeSpan startTimeTarget = currentElementStageTime;

			if (currentDelayYield != null) {

				// Suspend until next update
				if (currentDelayYield.SuspendForAnUpdate) {
					CoroutineYield currentDelayYieldClone = currentDelayYield.Clone() as CoroutineYield;
					currentDelayYieldClone.SuspendForAnUpdate = false;
					currentDelayYield = currentDelayYieldClone;
					return;
				}

				// Wait for time
				startTimeTarget = currentElementStageTime + currentDelayYield.WaitFor;

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

				} else if (currentDelayYield.WaitLastYieldedAnimation && lastStartedAnimation != null) {

					// Wait
					if (!lastStartedAnimation.HasEnded) return;

					// Update target start time
					TimeSpan animationEndTime = lastStartedAnimationStartTime + lastStartedAnimation.ElapsedTime;
					if (animationEndTime > startTimeTarget) startTimeTarget = animationEndTime;

				}

				// Wait for delegate
				if (currentDelayYield.WaitUntilPredicate != null) {
					if (!currentDelayYield.WaitUntilPredicate.Invoke()) return;
					startTimeTarget = ElapsedTime;
				}

				// Wait finished
				currentDelayYield = null;

			}

			// Find next element
			if (coroutineEnumerator == null) {
				enumeratorFinishedTime = startTimeTarget;	// failsafe
				continue;
			}

			bool enumeratorHasNewCurrent = coroutineEnumerator.MoveNext();
			currentElementStageTime = startTimeTarget;

			// No more elements
			if (!enumeratorHasNewCurrent) {
				enumeratorFinishedTime = startTimeTarget;
				coroutineEnumerator.Dispose();
				coroutineEnumerator = null;
				continue;
			}

			CoroutineYield nextElement = coroutineEnumerator.Current;

			if (nextElement == null) {
				// Skip null elements as a fail-safe
				currentDelayYield = null;
				continue;
			}

			if (nextElement.Animation != null) {

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
			if (coroutine is IDisposable) ((IDisposable)coroutine).Dispose();
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

}