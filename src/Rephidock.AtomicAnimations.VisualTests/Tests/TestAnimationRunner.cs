using System;
using SFML.Graphics;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Runners/AnimationRunner/Shifts")]
public class TestAnimationRunnerShifts : VisualTest {

	readonly protected AnimationRunner runner = new();

	protected Vector2f position = new(100, 200);
	readonly static protected Vector2f size = new(100, 100);

	const float stepDistance = 100;
	readonly protected static TimeSpan stepDuration = TimeSpan.FromSeconds(0.5);
	readonly protected static EasingCurve stepEasing = Easing.QuadOut;

	protected int animationsStarted = 0;
	protected int animationsFinished = 0;

	public override void Start(TimeSpan startTime) {
		runner.OnAnimationEnd += (_) => animationsFinished++;
	}

	public override void Update(TimeSpan deltaTime) {
		runner.Update(deltaTime);
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
			$"Animation counts: {animationsStarted} started, {animationsFinished} finished",
			new Vector2f(100, 100)
		);
	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		animationsStarted++;

		switch (@event) {
			case ArrowDirection.Left:
				AddAnimationToRunner(-stepDistance, 0);
				break;

			case ArrowDirection.Right:
				AddAnimationToRunner(stepDistance, 0);
				break;

			case ArrowDirection.Up:
				AddAnimationToRunner(0, -stepDistance);
				break;

			case ArrowDirection.Down:
				AddAnimationToRunner(0, stepDistance);
				break;
		}

	}

	protected override void DisposeManaged() {
		runner.Dispose();
	}

	protected virtual void AddAnimationToRunner(float shiftX, float shiftY) {
		runner.Run(
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


[VisualTestName(Name = "Runners/AnimationRunner/Moves")]
public class TestAnimationRunnerMoves : TestAnimationRunnerShifts {

	protected override void AddAnimationToRunner(float shiftX, float shiftY) {
		runner.Run(
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