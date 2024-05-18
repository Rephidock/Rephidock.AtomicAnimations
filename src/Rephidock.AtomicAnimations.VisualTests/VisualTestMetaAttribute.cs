using System;


namespace Rephidock.AtomicAnimations.VisualTests;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class VisualTestMetaAttribute : Attribute {

	public required string Name { get; init; }

	public bool HandlesDirectionalEvents { get; private init; } = false;

	public bool HandlesNumericEvents { get; private init; } = false;

	internal VisualTestMetaAttribute WithEventHandlingData(bool handlesDirectional, bool handlesNumeric) {
		return new VisualTestMetaAttribute() {
			Name = Name,
			HandlesDirectionalEvents = handlesDirectional,
			HandlesNumericEvents = handlesNumeric
		};
	}

}
