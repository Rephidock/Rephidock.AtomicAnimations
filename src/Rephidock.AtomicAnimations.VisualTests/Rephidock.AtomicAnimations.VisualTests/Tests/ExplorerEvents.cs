using System;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Explorer Events")]
public class ExplorerEvents : VisualTest {

	ArrowDirection? lastArrowDirection = null;
	int? lastNumericEvent = null;

	public override void Start(TimeSpan startTime) { }

	public override void Update(TimeSpan deltaTime) { }

	public override void Draw(Drawer drawer) {

		string lastDirectionalString = lastArrowDirection.HasValue ? lastArrowDirection.Value.ToString() : "None";
		string lastNumericString = lastNumericEvent.HasValue ? lastNumericEvent.Value.ToString() : "None";

		drawer.DrawText($"Last Directional: {lastDirectionalString}", new Vector2f(100, 100));
		drawer.DrawText($"Last Numeric: {lastNumericString}", new Vector2f(100, 100 + drawer.MainFontLineSpacing));
	}

	public override void HandleDirectionEvent(ArrowDirection @event) => lastArrowDirection = @event;

	public override void HandleNumericEvent(int @event) => lastNumericEvent = @event;

}