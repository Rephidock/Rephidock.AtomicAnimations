using System;


namespace Rephidock.AtomicAnimations.Base;


/// <summary>
/// Base interface for all animations.
/// Has no set duration.
/// </summary>
public abstract class Animation {

	/// <summary>
	/// The elapsed time for this animation,
	/// starting at <see cref="TimeSpan.Zero"/>.
	/// </summary>
	public TimeSpan ElapsedTime { get; protected set; } = TimeSpan.Zero;

	/// <summary>
	/// Time that is considered excess after animation has ended.
	/// Set after animation ends. Is <see cref="TimeSpan.Zero"/> otherwise.
	/// </summary>
	public TimeSpan ExcessTime { get; private set; }

	#region //// Flags

	/// <summary>
	/// Is <see langword="true"/> if <see cref="StartAndUpdate(TimeSpan)"/>.
	/// </summary>
	public bool HasStarted { get; private set; } = false;

	/// <summary>
	/// Is <see langword="true"/> if the animation is finished, halted or otherwise.
	/// </summary>
	public bool HasEnded { get; private set; } = false;

	/// <summary>
	/// Is <see langword="true"/> if the animation was halted before it finished.
	/// </summary>
	public bool WasHalted { get; private set; } = false;

	#endregion

	#region //// Main API

	/// <summary>
	/// <para>
	/// Starts animation and performs initial update.
	/// The time of the first update may be set to non-zero.
	/// </para>
	/// <para>
	/// Animations are assumed to be resettable unless documented otherwise.
	/// Calling this method again should perform the animation again,
	/// without undoing the result of the previous animation, partial or not.
	/// </para>
	/// </summary>
	/// <param name="initialTime">The time of the initial update.</param>
	/// <remarks>
	/// Negative <paramref name="initialTime"/> has undefined behavior.
	/// </remarks>
	public void StartAndUpdate(TimeSpan initialTime) {

		// Set values
		HasStarted = true;
		HasEnded = false;
		WasHalted = false;
		ElapsedTime = TimeSpan.Zero;
		ExcessTime = TimeSpan.Zero;

		// Call implementations and events
		StartImpl();
		Update(initialTime);
	}

	/// <inheritdoc cref="StartAndUpdate(TimeSpan)"/>
	public void StartAndUpdate() => StartAndUpdate(TimeSpan.Zero);

	/// <summary>
	/// Updates the animation with time since last update.
	/// </summary>
	/// <remarks>
	/// Negative <paramref name="deltaTime"/> has undefined behavior.
	/// </remarks>
	public void Update(TimeSpan deltaTime) {

		// Progress
		TimeSpan elapsedTimePrevious = ElapsedTime;
		ElapsedTime += deltaTime;

		// Call implementation
		UpdateImpl(deltaTime, elapsedTimePrevious);
	}

	/// <summary>Successfully ends the animation.</summary>
	protected void End(TimeSpan excessTime) {

		// Do nothing if not animating
		if (!HasStarted || HasEnded) return;

		// Set values
		HasEnded = true;
		ExcessTime = excessTime;
	}
	
	/// <inheritdoc cref="End(TimeSpan)"/>
	protected void End() => End(TimeSpan.Zero);

	/// <summary>Halts the animation. (Ends it prematurely).</summary>
	public void Halt() {
		
		// Do nothing if not animating
		if (!HasStarted || HasEnded) return;

		// Set flags
		HasEnded = true;
		WasHalted = true;
		ExcessTime = TimeSpan.Zero;

		// Invoke implementation
		HaltImpl();
	}

	#endregion

	#region //// Internal implementation

	/// <summary>
	/// <para>Implementation that is called before the first update.</para>
	/// <para>Called after flags are set.</para>
	/// </summary>
	protected virtual void StartImpl() { }

	/// <summary>
	/// <para>Implementation that is called every update.</para>
	/// <para>Called after <see cref="ElapsedTime"/> is set.</para>
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	/// <param name="elapsedTimePrevious"><see cref="ElapsedTime"/> before current update.</param>
	protected abstract void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious);

	/// <summary>
	/// <para>Implementation of the halting.</para>
	/// <para>Called after flags are set.</para>
	/// </summary>
	protected virtual void HaltImpl() { }

	#endregion

}
