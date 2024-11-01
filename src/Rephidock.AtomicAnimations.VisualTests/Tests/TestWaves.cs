using System;
using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;
using Rephidock.GeneralUtilities.Maths;
using Rephidock.AtomicAnimations;
using Rephidock.AtomicAnimations.Waves;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Wave Graphs")]
public class TestWaves : VisualTest {

	readonly Wave[] waves;

	int currentWaveIndex = 0;

	const float horizontalStart = 100;
	const float horizontalStep = 1;
	const float horizontalOutOfBoundsLength = 50;
	const float verticalPadding = 0.25f;
	const float verticalCenterRadius = 5;

	public TestWaves() {

		waves = [
			// display index 1
			new WaveBuilder()
			.Add(Easing.Linear).To(1f).Over(600)
			.ToWave(),

			// display index 2
			new WaveBuilder()
			.SetStartValue(1)
			.Add(Easing.Linear).To(0f).Over(600)
			.ToWave(),

			// display index 3
			new WaveBuilder()
			.Add(Easing.Linear).To(1f).Over(300)
			.Add(Easing.Linear).To(0f).Over(300)
			.ToWave(),

			// display index 4
			new WaveBuilder()
			.Add(Easing.QuadOut).To(1f).Over(300)
			.Add(Easing.QuadOut).To(0f).Over(300)
			.ToWave(),

			// display index 5
			new WaveBuilder()
			.Add(Easing.Linear).To(0.25f).Over(100)
			.Add(Easing.Linear).To(0.50f).Over(200)
			.Add(Easing.Linear).To(0.75f).Over(100)
			.Add(Easing.Linear).To(1.00f).Over(200)
			.ToWave(),

			// display index 6
			new WaveBuilder()
			.Add(Easing.ExpoIn).To(1).Over(200)
			.Add(Easing.BounceOut, destination: 0, width: 400)
			.ToWave(),

			// display index 7
			new WaveBuilder()
			.SetStartValue(1f)
			.Add(Easing.ElasticOut).To(0.5f).Over(200)
			.ToWave(),

			// display index 8
			new WaveBuilder()
			.Add(Easing.Linear).To(0.5f).Over(200)
			.AddGap(0)
			.Add(Easing.Linear).To(1).Over(200)
			.AddGap(0)
			.AddGap(0.5f)
			.Add(Easing.Linear).To(-0.25f).Over(200)
			.ToWave(),

			// display index 9
			new WaveBuilder()
			.Add(Easing.Linear).To(0.5f).Over(200)
			.AddGap(0)
			.Add(Easing.Linear).To(1).Over(200)
			.AddGap(1)
			.Add(Easing.Linear).To(0.2f).Over(200)
			.ToWave(),
		];

	}


	public override void Start(TimeSpan startTime) { }

	public override void Update(TimeSpan deltaTime) { }

	public override void Draw(Drawer drawer) {

		// Get graph bounds
		float windowHeight = drawer.GetBottomRight().Y;
		float verticalTop = windowHeight * verticalPadding;
		float verticalSize = windowHeight - (windowHeight * 2 * verticalPadding);

		// Draw graph bounds and center
		var leftLineColor = new Color(80, 80, 80);
		drawer.DrawPrimitive(
			PrimitiveType.LineStrip,
			new Vertex(new Vector2f(horizontalStart, verticalTop), leftLineColor),
			new Vertex(new Vector2f(horizontalStart, verticalTop + verticalSize), leftLineColor)
		);

		drawer.DrawCirle(
			new Vector2f(horizontalStart, verticalTop + verticalSize * 0.5f),
			verticalCenterRadius,
			new Color(255, 255, 255, 127),
			4
		);


		// Probe and draw graph
		float pointsXStart = -horizontalOutOfBoundsLength;
		float pointsXEnd = waves[currentWaveIndex].Width + horizontalOutOfBoundsLength;

		List<Vertex> vertices = new();

		for (float xx = pointsXStart; xx <= pointsXEnd; xx += horizontalStep) {

			float yProgress = waves[currentWaveIndex].GetValueAt(xx);
			float yy = verticalTop + verticalSize - (verticalSize * yProgress);
			bool isPointOutOfBounds = xx < 0 || xx > waves[currentWaveIndex].Width;

			vertices.Add(
				new Vertex(
					new Vector2f(xx + horizontalStart, yy),
					isPointOutOfBounds ? Color.Yellow : Color.Green
				)
			);

		}

		drawer.DrawPrimitive(PrimitiveType.Points, vertices.ToArray());
		
		// Draw text
		Vector2f textPosition =  new(horizontalStart, verticalTop - drawer.MainFontLineSpacing);
		drawer.DrawText($"Wave {currentWaveIndex + 1}/{waves.Length}", textPosition);

	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		
		if (@event == ArrowDirection.Up || @event == ArrowDirection.Left) {
			currentWaveIndex = (currentWaveIndex - 1).PosMod(waves.Length);
		} else if (@event == ArrowDirection.Down || @event == ArrowDirection.Right) {
			currentWaveIndex = (currentWaveIndex + 1).PosMod(waves.Length);
		}

	}

}