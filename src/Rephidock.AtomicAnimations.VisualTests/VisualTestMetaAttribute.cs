using System;


namespace Rephidock.AtomicAnimations.VisualTests;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class VisualTestMetaAttribute : Attribute {

	/// <summary>
	/// Name of the test.
	/// Use <c>/</c> to separate directories and names.
	/// </summary>
	public required string Name { get; init; }

}
