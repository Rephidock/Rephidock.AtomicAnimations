using System;
using Rephidock.GeneralUtilities.Reflection;


namespace Rephidock.AtomicAnimations.VisualTests;


public readonly struct TestCatalogueItem {

	/// <summary>The name of this item, not including parent directories.</summary>
	public string Name { get; private init; } = "<unknown>";

	public bool IsDirectory { get; private init; } = false;

	/// <summary>
	/// The class of this test if not a directory,
	/// null otherwise.
	/// </summary>
	public Type? TestClass { get; private init; } = null;

	public bool TestHandlesDirectionalEvents { get; private init; } = false;

	public bool TestHandlesNumericEvents { get; private init; } = false;


	#region //// Creation

	public TestCatalogueItem() { }

	public static TestCatalogueItem CreateDirectory(string name) {
		return new TestCatalogueItem() {
			Name = name,
			IsDirectory = true,
			TestClass = null
		};
	}

	public static TestCatalogueItem CreateTest(string name, Type testClass) {
		return new TestCatalogueItem() {
			Name = name,
			IsDirectory = false,
			TestClass = testClass,
			TestHandlesDirectionalEvents = testClass.GetMethod(nameof(VisualTest.HandleDirectionEvent))!.IsOverride(),
			TestHandlesNumericEvents = testClass.GetMethod(nameof(VisualTest.HandleNumericEvent))!.IsOverride()
		};
	}

	#endregion

}
