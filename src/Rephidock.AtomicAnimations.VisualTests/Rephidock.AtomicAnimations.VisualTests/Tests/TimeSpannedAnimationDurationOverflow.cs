using System;
using Rephidock.AtomicAnimations.Base;
using SFML.System;


namespace Rephidock.AtomicAnimations.VisualTests.Tests;


[VisualTestMeta(Name = "Excess time: TimeSpannedAnimation")]
public class TimeSpannedAnimationDurationOverflow : VisualTest {

	readonly TimeSpanedAnimation anim = new EmptyTimeSpannedAnimation(TimeSpan.FromSeconds(5));

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

public class EmptyTimeSpannedAnimation : TimeSpanedAnimation {
	public EmptyTimeSpannedAnimation(TimeSpan duration) : base(duration) { }
	protected override void UpdateTimeSpannedImpl(TimeSpan deltaTime) { }
}