using System;
using Rephidock.AtomicAnimations.Base;
using SFML.System;
using SFML.Graphics;
using SFML.Window;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


public abstract class TestEaseBase : VisualTest {

	protected readonly static TimeSpan duration = TimeSpan.FromSeconds(5);
	protected readonly static EasingCurve easing = Easing.QuadOut;

	protected const float width = 50, height = 50;

	public required Ease Anim { get; init; }

	public sealed override void Start(TimeSpan startTime) {
		Anim.StartAndUpdate(startTime);
	}

	public sealed override void Update(TimeSpan deltaTime) {
		Anim.Update(deltaTime);
	}

}


#region //// 4D

[VisualTestMeta(Name = "Shift 4D (x,y,rot,xscale)")]
public class TestShift4D : TestEaseBase {

	public const float startX = 100, startY = 100, startRot = 0, startXScale = 1;
	public const float shiftX = 100, shiftY = 50, shiftRot = -90, shiftXScale = 0.5f;
	protected float curX, curY, curRot, curXScale;
	
	public TestShift4D() {

		curX = startX;
		curY = startY;
		curRot = startRot;
		curXScale = startXScale;

		Anim = new Shift4D(
			shiftX, shiftY, shiftRot, shiftXScale,
			duration,
			easing,
			(x, y, r, xs) => {
				curX += x;
				curY += y;
				curRot += r;
				curXScale += xs;
			}
		);

	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(curX, curY, width, height),
			scale: new Vector2f(curXScale, 1),
			rotation: curRot,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: Color.White
		);

		drawer.DrawText(
			$"x: {curX:F6}\n" +
			$"y: {curY:F6}\n" +
			$"r: {curRot:F6}\n" +
			$"s: {curXScale:F6}",
			new Vector2f(100, 200)
		);

	}

}


[VisualTestMeta(Name = "Move 4D (x,y,rot,xscale)")]
public class TestMove4D : TestShift4D {

	public TestMove4D() : base() {

		Anim = new Move4D(
			startX, startY, startRot, startXScale,
			startX + shiftX, startY + shiftY, startRot + shiftRot, startXScale + shiftXScale,
			duration,
			easing,
			(x, y, r, xs) => {
				curX = x;
				curY = y;
				curRot = r;
				curXScale = xs;
			}
		);

	}

}

#endregion


#region //// 3D

[VisualTestMeta(Name = "Shift 3D (x,y,rot)")]
public class TestShift3D : TestEaseBase {

	public const float startX = 100, startY = 100, startRot = 0;
	public const float shiftX = 100, shiftY = 50, shiftRot = -90;
	protected float curX, curY, curRot;

	public TestShift3D() {

		curX = startX;
		curY = startY;
		curRot = startRot;

		Anim = new Shift3D(
			shiftX, shiftY, shiftRot,
			duration,
			easing,
			(x, y, r) => {
				curX += x;
				curY += y;
				curRot += r;
			}
		);

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
			$"x: {curX:F6}\n" +
			$"y: {curY:F6}\n" +
			$"r: {curRot:F6}",
			new Vector2f(100, 200)
		);

	}

}


[VisualTestMeta(Name = "Move 3D (x,y,rot)")]
public class TestMove3D : TestShift3D {

	public TestMove3D() : base() {

		Anim = new Move3D(
			startX, startY, startRot,
			startX + shiftX, startY + shiftY, startRot + shiftRot,
			duration,
			easing,
			(x, y, r) => {
				curX = x;
				curY = y;
				curRot = r;
			}
		);

	}

}

#endregion


#region //// 2D

[VisualTestMeta(Name = "Shift 2D (x,y)")]
public class TestShift2D : TestEaseBase {

	public const float startX = 100, startY = 100;
	public const float shiftX = 100, shiftY = 50;
	protected float curX, curY;

	public TestShift2D() {

		curX = startX;
		curY = startY;

		Anim = new Shift2D(
			shiftX, shiftY,
			duration,
			easing,
			(x, y) => {
				curX += x;
				curY += y;
			}
		);

	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(curX, curY, width, height),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: Color.White
		);

		drawer.DrawText(
			$"x: {curX:F6}\n" +
			$"y: {curY:F6}\n",
			new Vector2f(100, 200)
		);

	}

}

[VisualTestMeta(Name = "Move 2D (x,y)")]
public class TestMove2D : TestShift2D {

	public TestMove2D() : base() {

		Anim = new Move2D(
			startX, startY,
			startX + shiftX, startY + shiftY,
			duration,
			easing,
			(x, y) => {
				curX = x;
				curY = y;
			}
		);

	}

}


#endregion


#region //// 1D

[VisualTestMeta(Name = "Shift 1D (x)")]
public class TestShift1D : TestEaseBase {

	public const float startX = 100, yPos = 100;
	public const float shiftX = 100;
	protected float curX;

	public TestShift1D() {

		curX = startX;

		Anim = new Shift1D(
			shiftX,
			duration,
			easing,
			(x) => {
				curX += x;
			}
		);

	}

	public override void Draw(Drawer drawer) {

		drawer.DrawRectangle(
			rect: new FloatRect(curX, yPos, width, height),
			scale: new Vector2f(1, 1),
			rotation: 0,
			normalizedOrigin: new Vector2f(0.5f, 0.5f),
			color: Color.White
		);

		drawer.DrawText($"x: {curX:F6}\n", new Vector2f(100, 200));

	}

}


[VisualTestMeta(Name = "Move 1D (x)")]
public class TestMove1D : TestShift1D {

	public TestMove1D() : base() {

		Anim = new Move1D(
			startX,
			startX + shiftX,
			duration,
			easing,
			(x) => {
				curX = x;
			}
		);

	}

}


#endregion
