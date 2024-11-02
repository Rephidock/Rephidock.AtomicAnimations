using System;
using Rephidock.AtomicAnimations.Base;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestName(Name = "Excess time/Animation")]
public class TestAnimationExcessTime : VisualTest {

	readonly Animation anim = new EmptyAnimation();

	public override void Start(TimeSpan startTime) {
		anim.StartAndUpdate(startTime);
	}

	public override void Update(TimeSpan deltaTime) {
		anim.Update(deltaTime);
	}

	public override void Draw(Drawer drawer) {

		Vector2f currentPosition = new(80, 100);

		drawer.DrawText($"Time: {anim.ElapsedTime.TotalSeconds:F7}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"HasEnded: {anim.HasEnded}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"ExcessTime: {anim.ExcessTime}", currentPosition);
	}

	public override void HandleNumericEvent(int @event) {
		(anim as EmptyAnimation)!.ForceEnd(TimeSpan.FromSeconds(0.1) * @event);
	}

}

[VisualTestName(Name = "Excess time/TimeSpannedAnimation")]
public class TestTimeSpannedAnimationExcess : VisualTest {

	readonly TimedAnimation anim = new EmptyTimeSpannedAnimation(TimeSpan.FromSeconds(5));

	public override void Start(TimeSpan startTime) {
		anim.StartAndUpdate(startTime);
	}

	public override void Update(TimeSpan deltaTime) {
		anim.Update(deltaTime);
	}

	public override void Draw(Drawer drawer) {

		Vector2f currentPosition = new(80, 100);

		drawer.DrawText($"Time: {anim.ElapsedTime.TotalSeconds:F7} / {anim.Duration.TotalSeconds:F7}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"HasEnded: {anim.HasEnded}", currentPosition);
		currentPosition.Y += drawer.MainFontLineSpacing;

		drawer.DrawText($"ExcessTime: {anim.ExcessTime}", currentPosition);
	}

}

public class EmptyAnimation : Animation {
	protected override void UpdateImpl(TimeSpan deltaTime, TimeSpan elapsedTimePrevious) { }
	public void ForceEnd(TimeSpan excessTime) => End(excessTime);
}

public class EmptyTimeSpannedAnimation : TimedAnimation {
	public EmptyTimeSpannedAnimation(TimeSpan duration) : base(duration) { }
	protected override void UpdateTimeSpannedImpl(TimeSpan deltaTime) { }
}
