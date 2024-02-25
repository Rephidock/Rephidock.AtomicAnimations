using System;


namespace Rephidock.AtomicAnimations.Base;


/// <summary>
/// Base interface for all animations.
/// </summary>
public abstract class Animation {

	#region //// Time and duration

	/// <summary>
	/// The elapsed time for this animation,
	/// starting at <see cref="TimeSpan.Zero"/>.
	/// </summary>
	public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;

	/// <summary>
	/// <para>The duration of the animation.</para>
	/// <para>Timeout duration if <see cref="IsDurationDynamic"/> is <see langword="true"/></para>
	/// </summary>
	public TimeSpan Duration { get; protected init; }

	/// <summary>
	/// <para>
	/// Is <see langword="false"/> if this animation has a
	/// set <see cref="Duration"/> that does not change.
	/// </para>
	/// <para>
	/// Is <see langword="true"/> if 
	/// actual duration can be shorter (e.g. if it depends on outside factors).
	/// In this case <see cref="Duration"/> is a timeout duration.
	/// </para>
	/// </summary>
	public bool IsDurationDynamic { get; protected init; } = false;

	#endregion

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

	#region //// Events

	/// <summary>
	/// The event that is fired when the animation starts.
	/// Invoked before the initial update.
	/// Argument: Initial time of the animation
	/// </summary>
	public event Action<TimeSpan> OnStart = (_) => { };

	/// <summary>
	/// The event that is fired when the animation is finished or halted.
	/// Argument: Excess time since animation is finished. Can be negative if
	/// animation was halted.
	/// </summary>
	public event Action<TimeSpan> OnEnd = (_) => { };

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
	/// <param name="initialTime">The first time </param>
	/// <remarks>
	/// Negative <paramref name="initialTime"/> has undefined behavior.
	/// </remarks>
	public void StartAndUpdate(TimeSpan initialTime) {

		// Set values
		HasStarted = true;
		HasEnded = false;
		WasHalted = false;
		ElapsedTime = TimeSpan.Zero;

		// Call implementations and events
		StartImpl(initialTime);
		OnStart.Invoke(initialTime);
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

		// Do nothing if not animating
		if (!HasStarted || HasEnded) return;

		// Progress
		TimeSpan elapsedTimePrevious = ElapsedTime;
		ElapsedTime += deltaTime;

		// Check end
		if (ElapsedTime >= Duration) {

			// Calculate time
			TimeSpan timeTillDuration = Duration - elapsedTimePrevious;
			TimeSpan excessTime = ElapsedTime - Duration;
			ElapsedTime = Duration;

			// Invoke implementation
			LastUpdateImpl(timeTillDuration, excessTime);

			// End
			HasEnded = true;
			OnEnd.Invoke(excessTime);
			return;
		}

		// Call implementation
		UpdateImpl(deltaTime);
	}

	/// <summary>
	/// Marks animation with dynamic animation as finished.
	/// </summary>
	/// <param name="excessTime">Excess time to pass to <see cref="OnEnd"/></param>
	protected void DynamicEnd(TimeSpan excessTime) {

		// Do nothing if not animating
		if (!HasStarted || HasEnded) return;

		// Do nothing if not dynamic
		if (!IsDurationDynamic) return;

		// End
		HasEnded = true;
		OnEnd.Invoke(excessTime);
		return;

	}
	
	/// <inheritdoc cref="DynamicEnd(TimeSpan)"/>
	protected void DynamicEnd() {
		DynamicEnd(TimeSpan.Zero);
	}

	/// <summary>Halts the animation. (Ends it prematurely).</summary>
	public void Halt() {
		
		// Do nothing if not animating
		if (!HasStarted || HasEnded) return;

		// Set flags
		HasEnded = true;
		WasHalted = true;

		// Invoke event
		OnEnd.Invoke(ElapsedTime - Duration);
	}

	#endregion

	#region //// Internal implementation

	/// <summary>
	/// <para>Implementation that is called before the first update.</para>
	/// <para>Called after flags are set but before corresponding event is invoked.</para>
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	protected virtual void StartImpl(TimeSpan deltaTime) { }

	/// <summary>
	/// <para>Implementation that is called every update.</para>
	/// <para>Called after <see cref="ElapsedTime"/> is set.</para>
	/// </summary>
	/// <param name="deltaTime">Time since last update.</param>
	protected abstract void UpdateImpl(TimeSpan deltaTime);

	/// <summary>
	/// <para>
	/// Implementation that is called as the last update.
	/// Variation of <see cref="UpdateImpl(TimeSpan)"/>.
	/// </para>
	/// <para>
	/// Called after <see cref="ElapsedTime"/> is set but before 
	/// <see cref="HasEnded"/> is set and <see cref="OnEnd"/> is invoked.
	/// </para>
	/// </summary>
	/// <param name="deltaTimeNoExcess">Time since last update clamped to go to duration.</param>
	/// <param name="exessTime">Excess time of the update that would go over duration.</param>
	protected virtual void LastUpdateImpl(TimeSpan deltaTimeNoExcess, TimeSpan exessTime) {
		UpdateImpl(deltaTimeNoExcess);
	}

	#endregion

}
