using System;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Explorer Time Flow")]
public class TextExplorerTimeFlow : VisualTest {

	TimeSpan currentTime;

	public override void Start(TimeSpan startTime) {
		currentTime = startTime;
	}

	public override void Update(TimeSpan deltaTime) {
		currentTime += deltaTime;
	}

	public override void Draw(Drawer drawer) {
		drawer.DrawText(currentTime.ToString(), new Vector2f(100, 100));
	}

}