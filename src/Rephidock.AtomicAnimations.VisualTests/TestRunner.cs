using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rephidock.GeneralUtilities.Reflection;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestRunner : IDisposable {

	#region //// Runnig test

	public VisualTest? RunningTest { get; private set; } = null;

	public bool IsRunningATest => RunningTest is not null;

	public TimeSpan RunningElapsedTime { get; private set; } = TimeSpan.Zero;

	public void StartTest(Type testClass) {

		// Guards
		ObjectDisposedException.ThrowIf(isDisposed, this);

		if (
			!testClass.IsSubclassOf(typeof(VisualTest)) ||
			testClass.IsAbstract ||
			testClass.GetConstructor(Type.EmptyTypes) is null
		) {
			throw new ArgumentException($"Could not intantiante test {testClass.Name}");
		}


		// Stop existing test
		StopTest();

		// Create the test
		RunningTest = (VisualTest)Activator.CreateInstance(testClass)!;

		// Start the test
		IsPaused = StartPaused;
		ManualStepsQueued = 0;
		RunningElapsedTime = InitialTime;
		RunningTest.Start(InitialTime);
	}

	public void StopTest() {
		RunningTest?.Dispose();
		RunningTest = null;

		IsPaused = false;
	}

	public void RestartTest() {

		if (!IsRunningATest) return;

		Type testClass = RunningTest!.GetType();
		StopTest();
		StartTest(testClass);
	}

	public void UpdateAndDrawTest(TimeSpan deltaTime, Drawer drawer) {

		if (IsManualTimeFlow) {
			// Manual time flow
			TimeSpan multipledDeltaTime = ManualTimeStep.value;
			for (; ManualStepsQueued > 0; ManualStepsQueued--) {
				RunningElapsedTime += multipledDeltaTime;
				RunningTest?.Update(multipledDeltaTime);
			}
			
		} else {
			// Normal time flow with multiplier
			if (!IsPaused) {
				TimeSpan multipledDeltaTime = deltaTime * TimeMultiplier;
				RunningElapsedTime += multipledDeltaTime;
				RunningTest?.Update(multipledDeltaTime);
			}
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
