using SFML.System;
using SFML.Graphics;
using Rephidock.AtomicAnimations.Waves;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Waves/Wave as easing (shift)")]
public class TestWaveAsEasingShift : TestShift2D {

	protected Wave easingWave;

	public TestWaveAsEasingShift() {

		easingWave = new WaveBuilder()
			.Add(Easing.Linear).To(0.5f).Over(20)
			.Add(Easing.BounceOut).To(0).Over(100)
			.Add(Easing.BackOut).To(1).Over(100)
			.NormalizeWidth()
			.ToWave();

		curX = startX;
		curY = startY;

		Anim = new Shift2D(
			shiftX, shiftY,
			duration,
			easingWave.GetValueAt,
			(x, y) => {
				curX += x;
				curY += y;
			}
		);

	}

}


[VisualTestName(Name = "Waves/Wave as easing (move)")]
public class TestWaveAsEasingMove : TestWaveAsEasingShift {

	public TestWaveAsEasingMove() : base() {

		curX = 0;
		curY = 0;

		Anim = new Move2D(
			startX, startY,
			startX + shiftX, startY + shiftY,
			duration,
			easingWave.GetValueAt,
			(x, y) => {
				curX = x;
				curY = y;
			}
		);

	}

}
