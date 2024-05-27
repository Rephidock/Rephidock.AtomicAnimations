using System;
using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations;
using Rephidock.AtomicAnimations.Waves;
using System.Threading;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Wave Ease Runthrough")]
public class TestWaveEaseAtom : VisualTest {

	// Pillars
	public const int pillarCount = 30;
	public const float pillarWidth = 20;

	readonly float[] pillarHeights = new float[pillarCount];

	// Display info
	const float pillarDisplayStartX = 100;
	const float pillarDisplayStartY = 300;

	// Animation data
	protected readonly static TimeSpan duration = TimeSpan.FromSeconds(10);
	protected readonly static EasingCurve easing = Easing.QuadOut;

	protected Wave wave = new WaveBuilder()
		.Add(Easing.QuintInOut).To(100).Over(pillarWidth * 3)
		.Add(Easing.Linear).To(100).Over(pillarWidth * 3)
		.AddGap().To(50)
		.Add(Easing.Linear).To(50).Over(pillarWidth * 3)
		.Add(Easing.BounceOut).To(0).Over(pillarWidth * 6)
		.ToWave();

	protected WaveEase Anim { get; init; }

	// Additional info
	int pillarUpdatesCalled = 0;
	float lastWaveOffset;

	public TestWaveEaseAtom() {
		Anim = WaveEase.CreateRunthrough(
			wave,
			pillarCount * pillarWidth,
			false,
			duration,
			easing,
			UpdatePillarsFromWave
		);
	}

	public override void Start(TimeSpan startTime) {
		Anim.StartAndUpdate(startTime);
	}

	public override void Update(TimeSpan deltaTime) {
		Anim.Update(deltaTime);
	}

	public void UpdatePillarsFromWave(ShiftedWave wave) {
		pillarUpdatesCalled++;
		lastWaveOffset = wave.Offset;
		for (int i = 0; i < pillarCount; i++) {
			pillarHeights[i] = wave.GetValueAt((i + 0.5f) * pillarWidth);
		}
	}

	public override void Draw(Drawer drawer) {

		// Draw pillars
		for (int i = 0; i < pillarCount; i++) {
			drawer.DrawRectangle(
				new FloatRect(
					left: pillarDisplayStartX + i * pillarWidth,
					top: pillarDisplayStartY - pillarHeights[i],
					width: pillarWidth,
					height: pillarHeights[i]
				),
				Color.White
			);
		}

		// Draw debug text
		drawer.DrawText(
			$"Elaspsed: {Anim.ElapsedTime}/{duration}\n" +
			$"HasEnded: {Anim.HasEnded}\n" +
			$"Pillar update calls: {pillarUpdatesCalled}\n" +
			$"Wave width: {wave.Width:G7}, offset: {lastWaveOffset:F7}",
			new Vector2f(pillarDisplayStartX, pillarDisplayStartY + 5)
		);

	}	

}


[VisualTestMeta(Name = "Wave Ease Runthrough Reversed")]
public class TestWaveEaseAtomReversed : TestWaveEaseAtom {

	public TestWaveEaseAtomReversed() {
		Anim = WaveEase.CreateRunthrough(
			wave,
			pillarCount * pillarWidth,
			true,
			duration,
			easing,
			UpdatePillarsFromWave
		);
	}

}