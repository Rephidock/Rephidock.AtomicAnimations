using System;
using System.Collections;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Base;


namespace Rephidock.AtomicAnimations;


/// <summary>
/// A collection of <see cref="Animation"/>s
/// that are run in series or parrelel with possible
/// delays in-between.
/// </summary>
public class AnimationChain : Animation, IReadOnlyList<ChainElement> {

	/// <summary>List of animations in the chain.</summary>
	readonly List<ChainElement> elements;

	#region //// Creation

	/// <summary>Crates a <see cref="AnimationChain"/> with default capacity.</summary>
	public AnimationChain() {
		elements = new List<ChainElement>();
		splitsPlayer = new AnimationRunner();
		splitsPlayer.OnAnimationCompletion += OnSplitEnd;
	}

	/// <summary>Crates a <see cref="AnimationChain"/> with <paramref name="capacity"/>.</summary>
	public AnimationChain(int capacity) {
		elements = new List<ChainElement>(capacity);
		splitsPlayer = new AnimationRunner();
		splitsPlayer.OnAnimationCompletion += OnSplitEnd;
	}

	/// <summary>Add an animation to the chain.</summary>
	/// <param name="chainElement">Chain element to add</param>
	/// <exception cref="ArgumentNullException"><paramref name="chainElement"/> is null</exception>
	/// <exception cref="InvalidOperationException">Called after <see cref="Animation.StartAndUpdate()"/></exception>
	public void Add(ChainElement chainElement) {

		// Guards
		ArgumentNullException.ThrowIfNull(chainElement);

		if (HasStarted) {
			throw new InvalidOperationException("Cannot add animations after chain was started");
		}

		// Add animation to the list
		elements.Add(chainElement);
	}

	/// <summary>Add an animation to the chain.</summary>
	/// <param name="delay">Delay before animation</param>
	/// <param name="animation">Animation to add or <see langword="null"/> to add a delay only</param>
	/// <param name="isASplit">A flag that dictates if the next chain needs to ne a split</param>
	/// <exception cref="InvalidOperationException">Called after <see cref="Animation.StartAndUpdate()"/></exception>
	public void Add(TimeSpan delay, Animation? animation, bool isASplit = true) {
		elements.Add(
			new ChainElement() {
				DelayBeforeStart = delay,
				Animation = animation,
				IsASplit = isASplit
			}
		);
	}

	/// <summary>Adds multiple chain elements to the chain</summary>
	/// <param name="chainElements">Animations to add</param>
	/// <exception cref="ArgumentNullException">Any element is null</exception>
	/// <exception cref="InvalidOperationException">Called after <see cref="Animation.StartAndUpdate()"/></exception>
	public void AddRange(IEnumerable<ChainElement> chainElements) {
		foreach (var chainElement in chainElements) Add(chainElement);
	}

	#endregion

	#region //// Animation

	// Splits
	readonly AnimationRunner splitsPlayer;
	TimeSpan splitLastEndTime = TimeSpan.Zero; // excludes excess time

	// Current element
	int currentElementIndex = 0; // includes 1 after list ends
	bool currentElementAnimationStarted = false;
	TimeSpan currentElementStagedTime = TimeSpan.Zero; // includes 1 after list ends

	void OnSplitEnd(Animation animation) {
		TimeSpan animationEndTime = ElapsedTime - animation.ExcessTime;
		if (animationEndTime > splitLastEndTime) splitLastEndTime = animationEndTime;
	}

	/// <inheritdoc/>
	protected override void StartImpl() {
		// Reset
		splitsPlayer.Clear();
		splitLastEndTime = TimeSpan.Zero;
		currentElementIndex = 0;
		currentElementAnimationStarted = false;
		currentElementStagedTime = TimeSpan.Zero;
	}

	/// <inheritdoc/>
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) {

		// Update splits
		splitsPlayer.Update(deltaTime);

		// Reusable stage snippet
		void StageNextElement(TimeSpan currentAnimationExcessTime) {
			currentElementIndex++;
			currentElementAnimationStarted = false;
			currentElementStagedTime = ElapsedTime - currentAnimationExcessTime;
			deltaTime = currentAnimationExcessTime;
		}

		do {

			// Safety check for overstage
			if (deltaTime <= TimeSpan.Zero) return;

			// All non-splits are done
			if (currentElementIndex >= elements.Count) {

				// Wait for splits
				if (splitsPlayer.HasAnimations) return;

				if (splitLastEndTime > currentElementStagedTime) {
					// Splits ended after non-splits
					End(ElapsedTime - splitLastEndTime);
				} else {
					// Splits ended before non-splits
					End(ElapsedTime - currentElementStagedTime);
				}

				return;
			}

			ChainElement currentElement = elements[currentElementIndex];

			// Update current element
			if (currentElementAnimationStarted) {

				// Safety check just in case
				if (currentElement.Animation is null) {
					StageNextElement(TimeSpan.Zero);
					continue;
				}

				currentElement.Animation.Update(deltaTime);
				if (currentElement.Animation.HasEnded) {
					// Non-split ended -- stage next
					StageNextElement(currentElement.Animation.ExcessTime);
					continue;
				} else {
					// Wait for non-split animation to end
					return;
				}

				// [unreachable]
			}

			// Wait for delay
			TimeSpan startTimeTarget = currentElementStagedTime + currentElement.DelayBeforeStart;
			if (ElapsedTime < startTimeTarget) {
				return;
			}

			// Wait for spurs
			if (currentElement.JoinBeforeStart) {

				// Wait
				if (splitsPlayer.HasAnimations) return;

				// Update target start time
				if (splitLastEndTime > startTimeTarget) startTimeTarget = splitLastEndTime;
			}

			if (ElapsedTime < startTimeTarget) {
				return;
			}

			// Wait finished -- start animation if exists
			if (currentElement.Animation is not null) {

				if (currentElement.IsASplit) {

					// Start split and stage next
					splitsPlayer.Run(currentElement.Animation, ElapsedTime - startTimeTarget);
					StageNextElement(ElapsedTime - startTimeTarget);
					continue;

				} else {

					// Start non-split
					currentElement.Animation.StartAndUpdate(ElapsedTime - startTimeTarget);
					currentElementAnimationStarted = true;

					if (currentElement.Animation.HasEnded) {
						// Non-split ended -- stage next
						StageNextElement(currentElement.Animation.ExcessTime);
						continue;
					} else {
						// Wait for non-split animation to end
						return;
					}

				}

				// [unreachable]

			} else {

				// No animation -- stage next
				StageNextElement(ElapsedTime - startTimeTarget);
				continue;

			}

			// [unreachable]

		} while (true);

		// [unreachable]

	}

	/// <inheritdoc/>
	protected override void HaltImpl() {

		// Halt all splits
		splitsPlayer.HaltAndClear();

		// Halt current animation
		if (currentElementAnimationStarted) {
			elements[currentElementIndex].Animation?.Halt();
		}
	}

	#endregion

	#region //// IReadOnlyCollection

	/// <inheritdoc/>
	public int Count => elements.Count;

	/// <inheritdoc/>
	public ChainElement this[int index] => elements[index];

	/// <inheritdoc/>
	public IEnumerator<ChainElement> GetEnumerator() => elements.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
	#endregion

}
