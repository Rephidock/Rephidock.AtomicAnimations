using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// <para>
/// A player that automatically plays given animations.
/// Animations are started the moment they are added
/// and forgotten the moment they are finished.
/// Multiple animations can be played at the same time.
/// </para>
/// </summary>
/// <remarks>
/// Animations are updated in the order of addition.
/// </remarks>
public class AnimationPlayer {

	readonly LinkedList<Animation> animations = new();

	/// <inheritdoc cref="Run(Animation, TimeSpan)"/>
	public void Run(Animation animation) => Run(animation, TimeSpan.Zero);

	/// <summary>
	/// Adds an animation to play.
	/// The animation is played immediately.
	/// Stores a reference to the animation until it ends.
	/// </summary>
	public void Run(Animation animation, TimeSpan initialTime) {
		animation.StartAndUpdate(initialTime);
		animations.AddLast(animation);
	}

	/// <summary>
	/// Updates all currently playing animations with time since last update.
	/// </summary>
	/// <remarks>
	/// Negative <paramref name="deltaTime"/> has undefined behavior.
	/// </remarks>
	public void Update(TimeSpan deltaTime) {

		LinkedListNode<Animation>? nextNode;
		LinkedListNode<Animation>? currentNode = animations.First;

		// Update all animations
		while (currentNode is not null) {

			// Update and find next
			currentNode.Value.Update(deltaTime);
			nextNode = currentNode.Next;

			// Remove node with finished animation
			if (currentNode.Value.HasEnded) {
				animations.Remove(currentNode);
			}

			// Continue to the next node
			currentNode = nextNode;
		}

	}

	/// <summary>
	/// <para>
	/// Halts and clears all given animations,
	/// effectively halting and forgetting them.
	/// </para>
	/// </summary>
	public void HaltAndClear() {

		// Halt all animations
		foreach (var animation in animations) {
			animation.Halt();
		}

		// Clear references
		animations.Clear();
	}

	/// <summary>True if this player has animations playing</summary>
	public bool HasAnimations => animations.Count > 0;

}
