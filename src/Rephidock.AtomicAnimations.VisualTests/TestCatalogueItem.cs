using System;
using System.Collections.Generic;
using System.Linq;
using Rephidock.GeneralUtilities.Reflection;


namespace Rephidock.AtomicAnimations.VisualTests;


public readonly record struct TestCatalogueItem {

	#region //// Directory

	public Dictionary<string, TestCatalogueItem>? DirectoryItems { get; private init; } = null;

	public bool IsDirectory => DirectoryItems is not null;

	

	#endregion

	#region //// Test

	/// <summary>
	/// The class of this test if not a directory,
	/// null otherwise.
	/// </summary>
	public Type? TestClass { get; private init; } = null;

	public bool TestHandlesDirectionalEvents { get; private init; } = false;

	public bool TestHandlesNumericEvents { get; private init; } = false;

	#endregion

	public int CountTests() {
		if (IsDirectory) return DirectoryItems!.Values.Sum(item => item.CountTests());
		return 1;
	}

	#region //// Creation

	public TestCatalogueItem() { }

	public static TestCatalogueItem CreateDirectory() {
		return new TestCatalogueItem() {
			DirectoryItems = new Dictionary<string, TestCatalogueItem>(),
			TestClass = null
		};
	}

	public static TestCatalogueItem CreateTest(Type testClass) {
		return new TestCatalogueItem() {
			DirectoryItems = null,
			TestClass = testClass,
			TestHandlesDirectionalEvents = testClass.GetMethod(nameof(VisualTest.HandleDirectionEvent))!.IsOverride(),
			TestHandlesNumericEvents = testClass.GetMethod(nameof(VisualTest.HandleNumericEvent))!.IsOverride()
		};
	}

	#endregion

}
