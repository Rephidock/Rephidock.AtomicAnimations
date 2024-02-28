using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations {


/// <summary>
/// A collection of <see cref="Animation"/>s, which are played in series.
/// </summary>
public class SerialChain : Chain {

	/// <summary>Crates a <see cref="SerialChain"/> with default capacity.</summary>
	public SerialChain() : base() { }

	/// <summary>Crates a <see cref="SerialChain"/> with <paramref name="capacity"/>.</summary>
	public SerialChain(int capacity) : base(capacity) { }

	int currentIndex = 0;
	bool currentStarted = false;

	/// <inheritdoc/>
	protected override void StartImpl() {
		currentIndex = 0;
		currentStarted = false;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// Starting check with current index inside bounds
		while (currentIndex < animations.Count) {

			Animation currentAnimation = animations[currentIndex];

			// Start or update current animation
			if (!currentStarted) {
				currentAnimation.StartAndUpdate(deltaTime);
				currentStarted = true;
			} else if (!currentAnimation.HasEnded) {
				currentAnimation.Update(deltaTime);
			}

			// If animation has ended -- move to the next one
			if (currentAnimation.HasEnded) {
				currentIndex++;
				currentStarted = false;
				deltaTime = currentAnimation.ExcessTime;
				continue;
			}

			// If animation needs next update -- wait ourselves
			return;

		}

		// If all animations are done
		End(deltaTime);

	}

	/// <inheritdoc/>
	protected override void HaltImpl() {

		// Halt current animation
		if (currentIndex >= animations.Count) return;
		animations[currentIndex].Halt();
	}

}

}