using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestRunner : IDisposable {

	#region //// Stroage and creation

	public IReadOnlyList<(VisualTestMetaAttribute meta, Type type)> AllTests { get; set; }

	public int TestCount => AllTests.Count;

	public TestRunner() {

		AllTests = Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.IsSubclassOf(typeof(VisualTest)) && type.IsClass && !type.IsAbstract)
			.Where(type => type.GetConstructor(Type.EmptyTypes) is not null)
			.Select(type => (type.GetCustomAttribute<VisualTestMetaAttribute>(), type))
			.Select(pair => (pair.Item1 ?? VisualTestMetaAttribute.DefaultMeta, pair.Item2))
			.OrderBy(pair => pair.Item1.Name)
			.ToList()
			.AsReadOnly();

	}

	#endregion

	#region //// Runnig test

	public int? RunningTestIndex { get; private set; } = null;

	public VisualTest? RunningTest { get; private set; } = null;

	public bool IsRunningATest => RunningTest is not null;

	public void StartTest(int testIndex) {

		// Guards
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentOutOfRangeException.ThrowIfLessThan(testIndex, 0);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(testIndex, AllTests.Count);

		// Stop existing test
		StopTest();

		// Create the test
		RunningTest = (VisualTest)Activator.CreateInstance(AllTests[testIndex].type)!;
		RunningTestIndex = testIndex;

		// Start the test
		RunningTest.Start(InitialTime);
	}

	public void StopTest() {
		RunningTest?.Dispose();
		RunningTest = null;
		RunningTestIndex = null;
	}

	public void RestartTest() {

		if (!IsRunningATest) return;

		int indexToStart = RunningTestIndex!.Value;
		StopTest();
		StartTest(indexToStart);
	}

	public void UpdateAndDrawTest(TimeSpan deltaTime, Drawer drawer) {
		RunningTest?.Update(deltaTime);
		RunningTest?.Draw(drawer);
	}

	#endregion

	#region //// Running time settings

	public TimeSpan InitialTime { get; set; } = TimeSpan.Zero;


	public TimeSpan PresetInitialTimeStep { get; } = TimeSpan.FromSeconds(0.1); 

	#endregion

	#region //// IDisposable

	private bool isDisposed;

	protected virtual void Dispose(bool disposingManaged) {

		if (isDisposed) return;

		if (disposingManaged) {
			RunningTest?.Dispose();
		}

		isDisposed = true;
	}

	// override finalizer only if 'Dispose(bool)' has code to free unmanaged resources
	// ~TestExplorer() {
	//     Dispose(disposing: false);
	// }

	public void Dispose() {
		Dispose(disposingManaged: true);
		GC.SuppressFinalize(this);
	}

	#endregion

}
