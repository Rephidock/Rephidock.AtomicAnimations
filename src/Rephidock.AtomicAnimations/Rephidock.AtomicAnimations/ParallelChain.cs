using System;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// A collection of <see cref="Animation"/>s, which are played in parallel.
/// </summary>
public class ParallelChain : Chain {

	/// <summary>Crates a <see cref="ParallelChain"/> with default capacity.</summary>
	public ParallelChain() : base() { }

	/// <summary>Crates a <see cref="ParallelChain"/> with <paramref name="capacity"/>.</summary>
	public ParallelChain(int capacity) : base(capacity) { }

	bool hasStarted = false;

	/// <inheritdoc/>
	protected override void StartImpl() {
		hasStarted = false;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// First update: start all animations
		if (!hasStarted) {

			// Check empty
			if (animations.Count == 0) {
				End(deltaTime);
				return;
			}

			foreach (Animation animation in animations) {
				animation.StartAndUpdate(deltaTime);
			}

			hasStarted = true;
			return;
		}

		// Update animations
		bool allDone = true;
		TimeSpan maxExessTime = TimeSpan.Zero;
		foreach (Animation animation in animations) {

			// Skip updating finished
			if (animation.HasEnded) {

				// Update excess time
				TimeSpan thisAnimExcessTime = animation.ExcessTime + deltaTime;
				if (maxExessTime < thisAnimExcessTime) maxExessTime = thisAnimExcessTime;

				continue;
			}

			// Update non finished
			animation.Update(deltaTime);

			// Check if its done
			if (allDone && animation.HasEnded) {
				// Update excess time
				TimeSpan thisAnimExcessTime = animation.ExcessTime;
				if (maxExessTime < thisAnimExcessTime) maxExessTime = thisAnimExcessTime;
			} else {
				allDone = false;
			}

		}

		// End if done
		if (allDone) {
			End(maxExessTime);
		}

	}

	/// <inheritdoc/>
	protected override void HaltImpl() {

		// Halt all not finished animations
		foreach (Animation animation in animations) { 
			if (!animation.HasEnded) animation.Halt();
		}

	}

}
