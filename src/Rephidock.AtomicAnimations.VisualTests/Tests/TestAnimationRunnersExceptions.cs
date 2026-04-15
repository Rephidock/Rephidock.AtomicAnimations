using System;
using SFML.Graphics;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Runners/AnimationRunner/Exception Catching")]
public class TestExceptionsInRunner : TestAnimationRunnerShifts {

	const int ThrowThresholdX = (int)(4.5 * stepDistance);

	int exceptionCount = 0;
	
	public override void Update(TimeSpan deltaTime) {
		try {
			runner.Update(deltaTime);
		} catch (ThresholdBreachedException) {
			exceptionCount++;
		}
	}
	
	public override void Draw(Drawer drawer) {

		// Rectangle
		drawer.DrawRectangle(
			rect: new FloatRect(position, size),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: new Color(255, 255, 255, 127)
		);
		
		// Threshold
		drawer.DrawRectangle(
			rect: new FloatRect(ThrowThresholdX, 0, 1, drawer.GetBottomRight().Y),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0, 0),
			color: new Color(255, 0, 0, 255)
		);
		
		// Position
		drawer.DrawRectangle(
			rect: new FloatRect(position.X, position.Y, 1, 1),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0, 0),
			color: new Color(255, 255, 0, 255)
		);
		
		// Status
		drawer.DrawText(
			$"x: {position.X:F6} y:{position.Y:F6}\n" +
			$"x threshold: {ThrowThresholdX}\n" +
			$"Is running: {runner.HasAnimations} ({runner.PlayingCount})\n" +
			$"Exceptions: {exceptionCount}",
			new Vector2f(100, 100)
		);
	}

	protected override void AddAnimationToRunner(float shiftX, float shiftY) {
		runner.Run(
			new Shift2D(
				shiftX,
				shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {

					if (position.X > ThrowThresholdX && xx > 0) {
						throw new ThresholdBreachedException();
					}
					
					position.X += xx;
					position.Y += yy;
				}
			)
		);
	}

}


[VisualTestName(Name = "Runners/AnimationQueue/Exception Catching")]
public class TestExceptionsInQueue : TestAnimationQueueShifts {
	
	const int ThrowThresholdX = (int)(4.5 * stepDistance);

	int exceptionCount = 0;
	
	public override void Update(TimeSpan deltaTime) {
		try {
			queue.Update(deltaTime);
		} catch (ThresholdBreachedException) {
			exceptionCount++;
		}
	}
	
	public override void Draw(Drawer drawer) {

		// Rectangle
		drawer.DrawRectangle(
			rect: new FloatRect(position, size),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: new Color(255, 255, 255, 127)
		);
		
		// Threshold
		drawer.DrawRectangle(
			rect: new FloatRect(ThrowThresholdX, 0, 1, drawer.GetBottomRight().Y),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0, 0),
			color: new Color(255, 0, 0, 255)
		);
		
		// Position
		drawer.DrawRectangle(
			rect: new FloatRect(position.X, position.Y, 1, 1),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0, 0),
			color: new Color(255, 255, 0, 255)
		);
		
		// Status
		drawer.DrawText(
			$"x: {position.X:F6} y:{position.Y:F6}\n" +
			$"x threshold: {ThrowThresholdX}\n" +
			$"Is running: {queue.HasAnimations} + {queue.EnqueuedCount} queued\n" +
			$"Exceptions: {exceptionCount}",
			new Vector2f(100, 100)
		);

	}

	protected override void EnqueueNewAnimation(float shiftX, float shiftY) {
		queue.Enqueue(
			new Shift2D(
				shiftX,
				shiftY,
				stepDuration,
				stepEasing,
				(xx, yy) => {
					
					if (position.X > ThrowThresholdX && xx > 0) {
						throw new ThresholdBreachedException();
					}
					
					position.X += xx;
					position.Y += yy;
				}
			)
		);
	}

}


public class ThresholdBreachedException : InvalidOperationException {
	public override string Message => "Threshold has been breached";
}
