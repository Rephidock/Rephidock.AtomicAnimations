using System;
using Rephidock.AtomicAnimations.Base;
using SFML.Graphics;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Runners/AnimationQueue/Shifts")]
public class TestAnimationQueueShifts : VisualTest {

	readonly protected AnimationQueue queue = new();

	protected Vector2f position = new(100, 200);
	int animationsEnqueued = 0;
	int animationsFinished = 0;

	readonly static Vector2f size = new(100, 100);
	const float stepDistance = 100;
	readonly protected static TimeSpan stepDuration = TimeSpan.FromSeconds(0.5);
	readonly protected static EasingCurve stepEasing = Easing.QuadOut;

	public override void Start(TimeSpan startTime) {
		queue.OnAnimationEnd += (_) => animationsFinished++;
	}

	public override void Update(TimeSpan deltaTime) {
		queue.Update(deltaTime);
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
			$"Animation counts: {animationsEnqueued} enqueued, {animationsFinished} finished",
			new Vector2f(100, 100)
		);
	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		animationsEnqueued++;

		switch (@event) {
			case ArrowDirection.Left:
				EnqueueNewAnimation(-stepDistance, 0);
				break;

			case ArrowDirection.Right:
				EnqueueNewAnimation(stepDistance, 0);
				break;

			case ArrowDirection.Up:
				EnqueueNewAnimation(0, -stepDistance);
				break;

			case ArrowDirection.Down:
				EnqueueNewAnimation(0, stepDistance);
				break;
		}

	}

	protected override void DisposeManaged() {
		queue.Dispose();
	}

	protected virtual void EnqueueNewAnimation(float shiftX, float shiftY) {
		queue.Enqueue(
			new Shift2D(
				shiftX,
				shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {
					position.X += xx;
					position.Y += yy;
				}
			)
		);
	}

}


[VisualTestName(Name = "Runners/AnimationQueue/Moves")]
public class TestAnimationQueueMoves : TestAnimationQueueShifts {

	protected override void EnqueueNewAnimation(float shiftX, float shiftY) {
		queue.Enqueue(
			new Move2D(
				position.X,
				position.Y,
				position.X + shiftX,
				position.Y + shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {
					position.X = xx;
					position.Y = yy;
				}
			)
		);
	}

}

[VisualTestName(Name = "Runners/AnimationQueue/Moves (lazy factory)")]
public class TestAnimationQueueMoveLazy : TestAnimationQueueShifts {

	protected override void EnqueueNewAnimation(float shiftX, float shiftY) {
		queue.Enqueue(
			new Lazy<Animation>(
				() => new Move2D(
					position.X,
					position.Y,
					position.X + shiftX,
					position.Y + shiftY,
					stepDuration,
					stepEasing,
					(xx, yy) => {
						position.X = xx;
						position.Y = yy;
					}
				)
			)
		);
	}

}
