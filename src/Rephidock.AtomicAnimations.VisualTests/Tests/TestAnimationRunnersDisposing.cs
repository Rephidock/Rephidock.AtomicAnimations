using System;
using SFML.Graphics;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Runners/AnimationRunner/Disposing")]
public class TestAnimationRunnerDisposing : TestAnimationRunnerShifts {

	bool runnerDisposed = false;
	int disposedCount = 0;

	public override void Update(TimeSpan deltaTime) {
		if (runnerDisposed) return;
		base.Update(deltaTime);
	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(position, size),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: new Color(255, 255, 255, 127)
		);

		drawer.DrawText(
			$"x: {position.X:F6} y:{position.Y:F6}\n" +
			$"Is running: {runner.HasAnimations} ({runner.PlayingCount})\n" +
			$"Counts: {animationsStarted} started, {animationsFinished} finished, {disposedCount} disposed\n" +
			$"Runner disposed?: {runnerDisposed}",
			new Vector2f(100, 100)
		);
	}

	protected override void AddAnimationToRunner(float shiftX, float shiftY) {

		var animation = new DisposableShift2D(
				shiftX,
				shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {
					position.X += xx;
					position.Y += yy;
				}
			);

		animation.OnDispose += () => { disposedCount++; };

		runner.Run(animation);
	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		if (runnerDisposed) return;
		base.HandleDirectionEvent(@event);
	}

	public override void HandleNumericEvent(int @event) {

		if (runnerDisposed) return;

		if (@event == 1) runner.Clear();

		if (@event == 2) {
			runnerDisposed = true;
			runner.Dispose();
		}

	}

}

[VisualTestName(Name = "Runners/AnimationQueue/Disposing")]
public class TestAnimationQueueDisposing : TestAnimationQueueShifts {

	bool queueDisposed = false;
	int disposedCount = 0;

	public override void Update(TimeSpan deltaTime) {
		if (queueDisposed) return;
		base.Update(deltaTime);
	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(position, size),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: new Color(255, 255, 255, 127)
		);

		drawer.DrawText(
			$"x: {position.X:F6} y:{position.Y:F6}\n" +
			$"Is running: {queue.HasAnimations} + {queue.EnqueuedCount} queued\n" +
			$"Animation counts: {animationsEnqueued} enqueued, {animationsFinished} finished, {disposedCount} disposed\n" +
			$"Queue disposed?: {queueDisposed}",
			new Vector2f(100, 100)
		);
	}

	protected override void EnqueueNewAnimation(float shiftX, float shiftY) {

		var animation = new DisposableShift2D(
				shiftX,
				shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {
					position.X += xx;
					position.Y += yy;
				}
			);

		animation.OnDispose += () => { disposedCount++; };

		queue.Enqueue(animation);
	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		if (queueDisposed) return;
		base.HandleDirectionEvent(@event);
	}

	public override void HandleNumericEvent(int @event) {

		if (queueDisposed) return;

		if (@event == 1) queue.Clear();

		if (@event == 2) {
			queueDisposed = true;
			queue.Dispose();
		}

	}

}


public class DisposableShift2D : Shift2D, IDisposable {

	public DisposableShift2D(
		float shiftX, 
		float shiftY, 
		TimeSpan duration, 
		EasingCurve easingCurve, 
		Action<float, float> adder
	) : base(shiftX, shiftY, duration, easingCurve, adder) { }

	public bool IsDisposed { get; private set; }

	public event Action OnDispose = () => { };

	protected void Dispose(bool disposingManaged) {

		if (IsDisposed) return;

		IsDisposed = true;
		OnDispose();
	}

	public void Dispose() {
		Dispose(disposingManaged: true);
		GC.SuppressFinalize(this);
	}

}
