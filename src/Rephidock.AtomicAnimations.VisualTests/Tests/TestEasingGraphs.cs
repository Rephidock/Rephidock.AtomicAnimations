using System;
using System.Reflection;
using System.Linq;
using SFML.System;
using SFML.Graphics;
using Rephidock.GeneralUtilities;
using Rephidock.AtomicAnimations;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Easing graphs")]
public class TestEasingGraphs : VisualTest {

	readonly (string name, EasingCurve curve)[] easingCurves;
	int currentCurveIndex = 0;

	const int displayPoints = 100;
	const float padding = 0.25f;
	const float centerRadius = 5;

	public TestEasingGraphs() {
		easingCurves = typeof(Easing)
			.GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Select(mi => (mi.Name, (EasingCurve?)Delegate.CreateDelegate(typeof(EasingCurve), mi, false)))
			.Where(pair => pair.Item2 is not null)
			.Cast<(string, EasingCurve)>()
			//.OrderBy(pair => pair.Item1)
			.ToArray();
	}


	public override void Start(TimeSpan startTime) { }

	public override void Update(TimeSpan deltaTime) { }

	public override void Draw(Drawer drawer) {

		// Get graph bounds
		Vector2f windowSize = drawer.GetBottomRight();
		FloatRect graphBounds = new(windowSize * padding, windowSize - (windowSize * (2 * padding)));

		// Draw graph bounds and center
		drawer.DrawRectangle(graphBounds, new Color(80, 80, 80));
		drawer.DrawCirle(
			new Vector2f(
				graphBounds.Left + graphBounds.Width * 0.5f,
				graphBounds.Top + graphBounds.Height * 0.5f
			),
			centerRadius,
			new Color(255, 255, 255, 127),
			4
		);
		

		// Probe and draw graph
		Vertex[] vertices = new Vertex[displayPoints];

		for (int i = 0; i < displayPoints; i++) {

			float probeX = (float)i / (displayPoints - 1);
			float probeY = easingCurves[currentCurveIndex].curve.Invoke(probeX);

			vertices[i].Position = new(
				graphBounds.Left + graphBounds.Width * probeX,
				graphBounds.Top + graphBounds.Height * (1 - probeY)
			);

			vertices[i].Color = Color.White;

		}

		drawer.DrawPrimitive(PrimitiveType.LineStrip, vertices);
		
		// Draw text
		Vector2f textPosition =  new(graphBounds.Left, graphBounds.Top - drawer.MainFontLineSpacing);
		drawer.DrawText($"{currentCurveIndex + 1}/{easingCurves.Length}: {easingCurves[currentCurveIndex].name}", textPosition);

	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		
		if (@event == ArrowDirection.Up || @event == ArrowDirection.Left) {
			currentCurveIndex = (currentCurveIndex - 1).TrueMod(easingCurves.Length);
		} else if (@event == ArrowDirection.Down || @event == ArrowDirection.Right) {
			currentCurveIndex = (currentCurveIndex + 1).TrueMod(easingCurves.Length);
		}

	}

}