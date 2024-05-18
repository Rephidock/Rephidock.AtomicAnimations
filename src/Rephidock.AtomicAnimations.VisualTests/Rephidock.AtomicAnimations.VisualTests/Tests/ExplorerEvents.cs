﻿using System;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Explorer Events")]
public class ExplorerEvents : VisualTest {

	ArrowDirection? lastArrowDirection = null;
	int? lastNumericEvent = null;

	int eventsRegistered = 0;

	public override void Start(TimeSpan startTime) { }

	public override void Update(TimeSpan deltaTime) { }

	public override void Draw(Drawer drawer) {

		string lastDirectionalString = lastArrowDirection.HasValue ? lastArrowDirection.Value.ToString() : "None";
		string lastNumericString = lastNumericEvent.HasValue ? lastNumericEvent.Value.ToString() : "None";

		Vector2f currentPosition = new(80, 100);

		drawer.DrawText($"Last Directional: {lastDirectionalString}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"Last Numeric: {lastNumericString}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"Event count: {eventsRegistered}", currentPosition);
	}

	public override void HandleDirectionEvent(ArrowDirection @event) {
		lastArrowDirection = @event;
		eventsRegistered++;
	}

	public override void HandleNumericEvent(int @event) {
		lastNumericEvent = @event;
		eventsRegistered++;
	}

}