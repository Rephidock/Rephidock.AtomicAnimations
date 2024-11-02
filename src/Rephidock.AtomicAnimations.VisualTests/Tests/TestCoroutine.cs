using System;
using System.Collections.Generic;
using Rephidock.AtomicAnimations.Coroutines;
using SFML.System;
using SFML.Graphics;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Coroutine")]
public class TestCouroutine : VisualTest {

	protected const float width = 50, height = 50;
	public const float startX = 100, startY = 100, startRot = 0;
	protected float curX, curY, curRot;

	public int branchKey = 0;

	public required CoroutineAnimation CoroutineAnim { get; init; }

	public TestCouroutine() {
		curX = startX;
		curY = startY;
		curRot = startRot;
		CoroutineAnim = CoroutineAnimation().ToAnimation();
	}

	public override void Start(TimeSpan startTime) {
		CoroutineAnim.StartAndUpdate(startTime);
	}

	public override void Update(TimeSpan deltaTime) {
		CoroutineAnim.Update(deltaTime);
	}

	protected override void DisposeManaged() {
		CoroutineAnim.Dispose();
	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(curX, curY, width, height),
			scale: new Vector2f(1, 1),
			rotation: curRot,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: Color.White
		);

		drawer.DrawText(
			$"Brnach key: {branchKey}; Ended: {CoroutineAnim.HasEnded}\n" +
			$"x: {curX:F6}\n" +
			$"y: {curY:F6}\n" +
			$"r: {curRot:F6}",
			new Vector2f(200, 100)
		);

	}

	public override void HandleNumericEvent(int @event) {
		branchKey = @event;
	}

	IEnumerable<CoroutineYield> CoroutineAnimation() {

		// Simple aniation and wait
		yield return new Shift2D(
			50, 200,
			TimeSpan.FromSeconds(5),
			Easing.ExpoOut,
			(xx, yy) => { curX += xx; curY += yy; }
		);
		yield return CoroutineYield.WaitPrevious;

		yield return new Shift1D(
			50,
			TimeSpan.FromSeconds(2),
			Easing.SineInOut,
			xx => { curX += xx; }
		);
		yield return CoroutineYield.WaitPrevious;
		yield return CoroutineYield.WaitPrevious;
		yield return CoroutineYield.WaitPrevious;
		yield return CoroutineYield.WaitPrevious;
		yield return CoroutineYield.WaitPrevious;

		// Parallel
		yield return new Shift1D(
			100 - 10 * branchKey,
			TimeSpan.FromSeconds(3),
			Easing.Linear,
			xx => { curX += xx; }
		);

		yield return new CoroutineYield() { WaitFor = TimeSpan.FromSeconds(1) };

		yield return new Shift1D(
			90,
			TimeSpan.FromSeconds(1),
			Easing.BounceOut,
			rr => { curRot += rr; }
		);

		yield return CoroutineYield.Join;

		// Some sleep
		yield return CoroutineYield.Sleep(TimeSpan.FromSeconds(1));

		// Enumerator: possible to run code
		if (branchKey == 1) yield break;

		if (branchKey == 2) {
			curRot = -45;
		}

		if (branchKey == 3) {
			curRot = 30;
			branchKey = 0;
		}

		// Wait until
		yield return new CoroutineYield() { WaitUntilPredicate = () => branchKey == 0 };
		branchKey = 8;

		yield return new Shift1D(
			90,
			TimeSpan.FromSeconds(1),
			Easing.BounceOut,
			rr => { curRot += rr; }
		);
	}

}

