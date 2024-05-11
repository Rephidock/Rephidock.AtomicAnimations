using System;


namespace Rephidock.AtomicAnimations.VisualTests;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class VisualTestMetaAttribute : Attribute {

	public required string Name { get; init; }

	public readonly static VisualTestMetaAttribute DefaultMeta = new() {
		Name = "Unnamed"
	};

}
