using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rephidock.GeneralUtilities;


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
			.Select(pair => (pair.Item1 ?? new VisualTestMetaAttribute() { Name = pair.Item2.Name }, pair.Item2))
			.Select(
				pair => (
					pair.Item1.WithEventHandlingData(
						pair.Item2.GetMethod(nameof(VisualTest.HandleDirectionEvent))!.IsOverride(),
						pair.Item2.GetMethod(nameof(VisualTest.HandleNumericEvent))!.IsOverride()
					),
					pair.Item2
				)
			)
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
		IsPaused = StartPaused;
		ManualStepsQueued = 0;
		RunningTest.Start(InitialTime);
	}

	public void StopTest() {
		RunningTest?.Dispose();
		RunningTest = null;
		RunningTestIndex = null;

		IsPaused = false;
	}

	public void RestartTest() {

		if (!IsRunningATest) return;

		int indexToStart = RunningTestIndex!.Value;
		StopTest();
		StartTest(indexToStart);
	}

	public void UpdateAndDrawTest(TimeSpan deltaTime, Drawer drawer) {

		if (IsManualTimeFlow) {
			// Manual time flow
			for (; ManualStepsQueued > 0; ManualStepsQueued--) {
				RunningTest?.Update(ManualTimeStep.value);
			}
			
		} else {
			// Time flow with multiplier
			if (!IsPaused) RunningTest?.Update(deltaTime * TimeMultiplier);
		}

		RunningTest?.Draw(drawer);
	}


	int ManualStepsQueued = 0;

	public void QueueManualStep(int count = 1) {
		ManualStepsQueued += count;
	}

	#endregion

	#region //// Running time settings

	public TimeSpan InitialTime { get; set; } = TimeSpan.Zero;
	public bool StartPaused { get; set; } = false;

	public double TimeMultiplier { get; set; } = 1.0;
	public bool IsPaused { get; set; } = false;

	public (TimeSpan value, string name) ManualTimeStep { get; set; } = (TimeSpan.FromSeconds(0.5), "0.5s");
	public bool IsManualTimeFlow { get; set; } = false;


	public TimeSpan PresetInitialTimeStep { get; } = TimeSpan.FromSeconds(0.1);

	/// <remarks>Sorted in ascending order</remarks>
	public IReadOnlyList<double> PresetTimeMultipliers { get; } = new List<double>() {
		0.1, 0.25, 0.5,
		1,
		1.5, 2, 4, 10
	}.AsReadOnly();

	/// <remarks>Sorted in ascending order</remarks>
	public IReadOnlyList<(TimeSpan value, string name)> PresetManualTimeSteps { get; } = new List<(TimeSpan, string)>() {
		(TimeSpan.FromSeconds(1d/100), "frame@100fps (10ms)"),
		(TimeSpan.FromSeconds(1d/60), "frame@60fps (~16.67ms)"),
		(TimeSpan.FromSeconds(1d/30), "frame@30fps (~33.33ms)"),
		(TimeSpan.FromSeconds(1d/24), "frame@24fps (~41.67ms)"),
		(TimeSpan.FromSeconds(0.1), "0.1s"),
		(TimeSpan.FromSeconds(0.25), "0.25s"),
		(TimeSpan.FromSeconds(0.5), "0.5s"),
		(TimeSpan.FromSeconds(1), "1s")
	}.AsReadOnly();

	public void PresetTimesSwitchToPrevious() {

		if (IsManualTimeFlow) {

			// Check if it is the smallest
			if (ManualTimeStep.value <= PresetManualTimeSteps[0].value) {
				IsManualTimeFlow = false;
				TimeMultiplier = PresetTimeMultipliers[^1];
				return;
			}

			// If not -- Find closest smaller
			ManualTimeStep = PresetManualTimeSteps.Last(pair => pair.value < ManualTimeStep.value);
			return;
		}

		// Check if it is the smallest
		if (TimeMultiplier <= PresetTimeMultipliers[0]) {
			IsManualTimeFlow = true;
			ManualTimeStep = PresetManualTimeSteps[^1];
			return;
		}

		// If not -- Find closest smaller
		TimeMultiplier = PresetTimeMultipliers.Last(x => x < TimeMultiplier);
		return;


	}

	public void PresetTimesSwitchToNext() {

		if (IsManualTimeFlow) {

			// Check if it is the largest
			if (ManualTimeStep.value >= PresetManualTimeSteps[^1].value) {
				IsManualTimeFlow = false;
				TimeMultiplier = PresetTimeMultipliers[0];
				return;
			}

			// If not -- Find closest large
			ManualTimeStep = PresetManualTimeSteps.First(pair => pair.value > ManualTimeStep.value);
			return;
		}

		// Check if it is the smallest
		if (TimeMultiplier >= PresetTimeMultipliers[^1]) {
			IsManualTimeFlow = true;
			ManualTimeStep = PresetManualTimeSteps[0];
			return;
		}

		// If not -- Find closest large
		TimeMultiplier = PresetTimeMultipliers.First(x => x > TimeMultiplier);
		return;

	}


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
