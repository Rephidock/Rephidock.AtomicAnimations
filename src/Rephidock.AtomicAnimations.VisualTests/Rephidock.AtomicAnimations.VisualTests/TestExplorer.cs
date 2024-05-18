using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Rephidock.GeneralUtilities;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestExplorer : IDisposable {
	
	public required TextWriter StdOut { get; init; }

	#region //// Assets and layout constants

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected Font MainFont { get; private set; } = null!;

	const uint MainFontSize = 18;

	const int MainFontLineSpacing = 24; // Hardcoded to be const. Retrivied from `MainFont.GetLineSpacing(MainFontSize)`

	void LoadAssets() {
		StdOut.WriteLine("Launching assets...");
		MainFont = new Font("Assets/JetBrainsMono-Regular.ttf");
	}


	public static class Layout {

		public readonly static Vector2f TitleDisplay = new(5, 10);

		public readonly static Vector2f TestSelectStartOffset = new(40, 2 * MainFontLineSpacing);

		public readonly static float TestCursorX = 10;

		public readonly static float TestSelectOptionSpacing = MainFontLineSpacing + 5;

		public readonly static float TestSelectEndY = FpsDisplayOffset.Y - 3 * MainFontLineSpacing;

		public readonly static Vector2f FpsDisplayOffset = new(5, -MainFontLineSpacing * 2);

		public readonly static Vector2f StatusDisplayOffset = new(5, -MainFontLineSpacing);
	}

	#endregion

	#region //// Window, Drawer and Main Loop

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected RenderWindow Window { get; private set; } = null!;

	const string WindowName = "Atomic Animations Visual Tests";
	static VideoMode WindowSize = new(800, 600);
	const uint WindowFrameRate = 60;

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected Drawer WindowDrawer { get; private set; } = null!;


	public void Run() {

		ObjectDisposedException.ThrowIf(isDisposed, this);

		StdOut.WriteLine("Launching visual tester...");

		// Load
		LoadAssets();
		LoadTests();

		// Create window
		Window = new(WindowSize, WindowName);
		Window.SetFramerateLimit(WindowFrameRate);
		Window.Closed += (_, _) => OnCloseRequest();
		Window.KeyPressed += (_, @event) => OnKeyPressed(@event);
		Window.Resized += (_, newSize) => OnWindowResize(newSize);

		WindowDrawer = new Drawer(Window, MainFont, MainFontSize, MainFontLineSpacing);

		// Run main event loop
		StdOut.WriteLine("Time to test!");

		using Clock clock = new();
		while (Window.IsOpen) {
			Window.DispatchEvents();
			OnTick(clock.Restart().ToTimeSpan());
			Window.Display();
		}

	}

	void OnCloseRequest() {
		StdOut.WriteLine("Closing...");
		Window.Close();
	}

	private void OnWindowResize(SizeEventArgs newSize) {
		StdOut.WriteLine($"Resizing to {newSize.Width}x{newSize.Height}...");
		using var newView = new View(new FloatRect(0, 0, newSize.Width, newSize.Height));
		Window.SetView(newView);
	}

	#endregion

	const string DefaultTitle = WindowName + " | [f1] for controls";

	bool isShowingControls = false;

	readonly static IReadOnlyList<string> ControlsHelp = [
		"= Test Select =",
		"[f1]: Toggle this help",
		"[↑],[↓]: Choose test",
		"[enter]: Start test",
		"[esc]: Exit",
		"[space]: Toggle start paused",
		"[shift]+[↑], [shift]+[↓]: Change speed mult.",
		"[alt]+[↑], [alt]+[↓]: Change initial time",
		"",
		"= Test =",
		"[esc]: Back",
		"[enter]: Restart test",
		"[space]: Pause/Resume test OR Step",
		"[shift]+[↑], [shift]+[↓]: Change speed mult.",
		"[alt]+[↑], [alt]+[↓]: Change initial time"
	];

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	TestRunner TestRunner { get; set; } = null!;

	int CurrentySelectTestIndex = 0;

	void LoadTests() {
		StdOut.WriteLine("Finding tests...");

		TestRunner = new TestRunner();

		StdOut.WriteLine($"Found {TestRunner.TestCount} tests:");
		foreach (var testName in TestRunner.AllTests.Select(pair => pair.meta.Name)) {
			StdOut.WriteLine(testName);
		}
	}

	void OnKeyPressed(KeyEventArgs @event) {

		// If holding alt
		if (@event.Alt) {

			// Change inital time
			if (@event.Code == Keyboard.Key.Up) {
				TestRunner.InitialTime += TestRunner.PresetInitialTimeStep;
			} else if (@event.Code == Keyboard.Key.Down) {
				TestRunner.InitialTime -= TestRunner.PresetInitialTimeStep;
				if (TestRunner.InitialTime < TimeSpan.Zero) TestRunner.InitialTime = TimeSpan.Zero;
			}

			return;
		}

		// If holding shift
		if (@event.Shift) {

			// Change speed
			if (@event.Code == Keyboard.Key.Up) {
				TestRunner.PresetTimesSwitchToNext();
			} else if (@event.Code == Keyboard.Key.Down) {
				TestRunner.PresetTimesSwitchToPrevious();
			}

			return;
		}

		// If test is running
		if (TestRunner.IsRunningATest) {

			// Escape -- back
			if (@event.Code == Keyboard.Key.Escape) {
				TestRunner.StopTest();
				StdOut.WriteLine("Test stopped");
				Window.SetTitle(WindowName);
				return;
			}

			// Enter -- restart
			if (@event.Code == Keyboard.Key.Enter) {
				StdOut.WriteLine("Restarting the test...");
				TestRunner.RestartTest();
				return;
			}

			// Space -- pause or step
			if (@event.Code == Keyboard.Key.Space) {

				if (TestRunner.IsManualTimeFlow) {
					TestRunner.QueueManualStep();
				} else {
					TestRunner.IsPaused = !TestRunner.IsPaused;
				}
				
				return;
			}

			return;

		// If on test select
		} else {

			// Escape -- close or hide help
			if (@event.Code == Keyboard.Key.Escape) {
				if (isShowingControls) {
					isShowingControls = false;
				} else {
					OnCloseRequest();
				}
				return;
			}

			// Help toggle
			if (@event.Code == Keyboard.Key.F1) {
				isShowingControls = !isShowingControls;
				return;
			}

			// ignore other input if controls are shown
			if (isShowingControls) return;

			// Movement on test select
			if (@event.Code == Keyboard.Key.Up) {
				CurrentySelectTestIndex = (CurrentySelectTestIndex - 1).TrueMod(TestRunner.TestCount);
				return;
			}

			if (@event.Code == Keyboard.Key.Down) {
				CurrentySelectTestIndex = (CurrentySelectTestIndex + 1).TrueMod(TestRunner.TestCount);
				return;
			}

			// Enter -- Start test
			if (@event.Code == Keyboard.Key.Enter) {

				string testName = TestRunner.AllTests[CurrentySelectTestIndex].meta.Name;

				StdOut.WriteLine($"Starting test \"{testName}\" (index {CurrentySelectTestIndex})...");
				Window.SetTitle($"{WindowName} - {testName}");

				TestRunner.StartTest(CurrentySelectTestIndex);
			}

			// Space -- toggle start paused
			if (@event.Code == Keyboard.Key.Space) {
				TestRunner.StartPaused = !TestRunner.StartPaused;
				return;
			}

		}

	}

	void OnTick(TimeSpan deltaTime) {

		// Clear previous frame
		Window.Clear(Color.Black);

		// Draw title
		if (TestRunner.IsRunningATest) {
			string testName = TestRunner.AllTests[CurrentySelectTestIndex].meta.Name;
			WindowDrawer.DrawText($"Running \"{testName}\"", Layout.TitleDisplay);
		} else {
			WindowDrawer.DrawText(DefaultTitle, Layout.TitleDisplay);
		}

		// Draw status bar
		string initialStatus = $"INIT: {TestRunner.InitialTime.TotalSeconds:F1}s";
		if (TestRunner.StartPaused) initialStatus += " paused";

		string runStatus = "RUN: ";
		if (TestRunner.IsManualTimeFlow) {
			runStatus += $"manual +={TestRunner.ManualTimeStep.name}";
		} else {
			runStatus += $"{TestRunner.TimeMultiplier}x";
			if (TestRunner.IsPaused) runStatus += " paused";
		}


		WindowDrawer.DrawText($"{initialStatus}; {runStatus}", WindowDrawer.GetBottomLeft() + Layout.StatusDisplayOffset);

		// Draw delta time and fps
		WindowDrawer.DrawText($"Δt {deltaTime.Milliseconds:D3}ms (~{1 / deltaTime.TotalSeconds:F0} fps)", WindowDrawer.GetBottomLeft() + Layout.FpsDisplayOffset);

		// Update and draw the test
		if (TestRunner.IsRunningATest) {
			TestRunner.UpdateAndDrawTest(deltaTime, WindowDrawer);

		// or draw controls
		} else if (isShowingControls) {

			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = 0; i < ControlsHelp.Count; i++) {

				// Check if this is the last line to be drawn
				bool isThisLineLast = currentOffset.Y + Layout.TestSelectOptionSpacing >= Window.Size.Y + Layout.TestSelectEndY;

				// If it is and there is more text -- draw ....
				if (isThisLineLast && i != ControlsHelp.Count - 1) {
					WindowDrawer.DrawText("...", currentOffset);

				// Otherwise draw if not empty
				} else if (!string.IsNullOrEmpty(ControlsHelp[i])) {
					WindowDrawer.DrawText(ControlsHelp[i], currentOffset);
				}

				// Advance to next line
				if (isThisLineLast) break;
				currentOffset.Y += Layout.TestSelectOptionSpacing;
			}

		// or no tests message
		} else if (TestRunner.TestCount == 0) {
			WindowDrawer.DrawText("No tests found.", Layout.TestSelectStartOffset);

		// or test select
		} else {

			// Find out which options fit
			int menuFirstDrawnOptionIndex, menuLastDrawnOptionIndex;

			float menuStartY = Layout.TestSelectStartOffset.Y;
			float menuEndY = Window.Size.Y + Layout.TestSelectEndY;

			int numberOfTestLines = (int)Math.Floor((menuEndY - menuStartY) / Layout.TestSelectOptionSpacing) + 1;


			if (numberOfTestLines <= 1) {
				// Single test fits or no tests fit
				menuFirstDrawnOptionIndex = CurrentySelectTestIndex;
				menuLastDrawnOptionIndex = CurrentySelectTestIndex;

			} else if (numberOfTestLines >= TestRunner.AllTests.Count) {
				// All tests fit
				menuFirstDrawnOptionIndex = 0;
				menuLastDrawnOptionIndex = TestRunner.AllTests.Count - 1;

			} else {

				// Some fit

				// Keep cursor in the middle
				int cursorPositionWithinVisible = numberOfTestLines / 2;

				// First are shown
				if (CurrentySelectTestIndex < cursorPositionWithinVisible) {
					menuFirstDrawnOptionIndex = 0;
					menuLastDrawnOptionIndex = numberOfTestLines - 1;

				// Last are shown
				} else if (CurrentySelectTestIndex - cursorPositionWithinVisible + numberOfTestLines >= TestRunner.AllTests.Count) {
					menuLastDrawnOptionIndex = TestRunner.AllTests.Count - 1;
					menuFirstDrawnOptionIndex = menuLastDrawnOptionIndex - numberOfTestLines + 1;

				// Middle is shown
				} else {
					menuFirstDrawnOptionIndex = CurrentySelectTestIndex - cursorPositionWithinVisible;
					menuLastDrawnOptionIndex = menuFirstDrawnOptionIndex + numberOfTestLines - 1;
				}

			}


			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = menuFirstDrawnOptionIndex; i <= menuLastDrawnOptionIndex; i++) {

				WindowDrawer.DrawText(TestRunner.AllTests[i].meta.Name, currentOffset);

				if (i == CurrentySelectTestIndex) {
					WindowDrawer.DrawText(">", new Vector2f(Layout.TestCursorX, currentOffset.Y));
				}

				currentOffset.Y += Layout.TestSelectOptionSpacing;
			}

		}

	}

	#region //// IDisposable

	private bool isDisposed;

	protected virtual void Dispose(bool disposingManaged) {

		if (isDisposed) return;

		if (disposingManaged) {
			Window?.Dispose();
			MainFont?.Dispose();
			TestRunner?.Dispose();
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
