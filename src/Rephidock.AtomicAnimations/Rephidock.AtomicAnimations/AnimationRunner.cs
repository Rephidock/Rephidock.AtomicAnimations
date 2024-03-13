﻿using System;
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
/// <para>
/// <see cref="IDisposable"/> animations are supported and
/// are disposed of when they are finished.
/// </para>
/// </summary>
/// <remarks>
/// Animations are updated in the order of addition.
/// </remarks>
public class AnimationRunner : IDisposable {

	readonly LinkedList<Animation> animations = new();

	/// <inheritdoc cref="Run(Animation, TimeSpan)"/>
	public void Run(Animation animation) => Run(animation, TimeSpan.Zero);

	/// <summary>
	/// Adds an animation to play.
	/// The animation is played immediately.
	/// Stores a reference to the animation until it ends.
	/// </summary>
	public void Run(Animation animation, TimeSpan initialTime) {

		// Guards
		if (isDiposed) throw new ObjectDisposedException(this.GetType().FullName);
		ArgumentNullException.ThrowIfNull(animation);

		// Add animation
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

		// Dispose guard
		if (isDiposed) throw new ObjectDisposedException(this.GetType().FullName);

		// Update all animations
		LinkedListNode<Animation>? nextNode;
		LinkedListNode<Animation>? currentNode = animations.First;

		while (currentNode is not null) {

			// Update and find next
			currentNode.Value.Update(deltaTime);
			nextNode = currentNode.Next;

			// Remove node with finished animation
			if (currentNode.Value.HasEnded) {

				animations.Remove(currentNode);
				OnAnimationCompletion?.Invoke(currentNode.Value);

				if (currentNode.Value is IDisposable disposable) {
					disposable.Dispose();
				}

			}

			// Continue to the next node
			currentNode = nextNode;
		}

	}

	/// <summary>
	/// Halts and clears (forgets) all animations.
	/// <see cref="IDisposable"/> animations are disposed of.
	/// </summary>
	public void HaltAndClear() {

		// Halt all animations
		foreach (var animation in animations) {

			animation.Halt();

			// Dipose of disposables
			if (animation is IDisposable disposable) {
				disposable.Dispose();
			}
		}

		// Clear list
		animations.Clear();
	}

	/// <summary>True if this player has animations playing</summary>
	public bool HasAnimations => animations.Count > 0;

	/// <summary>
	/// Evenet that invoked when any given animaton completes.
	/// Called after the player forget about the anmation
	/// but right before it is disposed of.
	/// </summary>
	/// <remarks>
	/// Does not get invoked when <see cref="HaltAndClear"/> is called.
	/// </remarks>
	public event Action<Animation>? OnAnimationCompletion = null;

	#region //// Disposing

	bool isDiposed = false;

	/// <inheritdoc/>
	protected virtual void Dispose(bool isDisposingManaged) {

		if (isDiposed) return;

		if (isDisposingManaged) {

			// Dispose of animations
			foreach (var animation in animations) {
				if (animation is IDisposable disposable) {
					disposable.Dispose();
				}
			}

			// Clear the list
			animations.Clear();

		}


		isDiposed = true;
	}

	/// <inheritdoc/>
	public void Dispose() {
		Dispose(isDisposingManaged: true);
		GC.SuppressFinalize(this);
	}

	#endregion

}
