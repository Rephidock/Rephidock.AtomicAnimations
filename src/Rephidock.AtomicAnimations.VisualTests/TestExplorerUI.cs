using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestExplorerUI : IDisposable {
	
	public required TextWriter StdOut { get; init; }

	#region //// Assets

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected Font MainFont { get; private set; } = null!;

	void LoadAssets() {
		StdOut.WriteLine("Loading assets...");
		MainFont = new Font("Assets/JetBrainsMono-Regular.ttf");
	}

	#endregion

	#region //// Layout, Strings, Formatting

	const uint MainFontSize = 18;

	const int MainFontLineSpacing = 24; // Hardcoded to be const. Retrivied from `MainFont.GetLineSpacing(MainFontSize)`

	public static class Layout {

		public readonly static Vector2f TitleDisplay = new(5, 10);

		public readonly static Vector2f TestSelectStartOffset = new(40, 2 * MainFontLineSpacing);

		public readonly static float TestCursorX = 10;

		public readonly static float TestSelectOptionSpacing = MainFontLineSpacing + 5;

		public readonly static float TestSelectEndY = FpsDisplayOffset.Y - 3 * MainFontLineSpacing;

		public readonly static Vector2f FpsDisplayOffset = new(5, -MainFontLineSpacing * 2);

		public readonly static Vector2f StatusDisplayOffset = new(5, -MainFontLineSpacing);
	}

	const string DefaultTitle = WindowName + " | [f1] for controls";

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
		"[0]..[9],[←],[↓],[↑],[→]: Invoke events (⚡ icon)",
		"[enter]: Restart test",
		"[space]: Pause/Resume test OR Step",
		"[shift]+[↑], [shift]+[↓]: Change speed mult.",
		"[alt]+[↑], [alt]+[↓]: Change initial time"
	];

	string FormatEvents(bool handlesDirectional, bool handlesNumeric) {
		if (!handlesDirectional && !handlesNumeric) return new string(' ', 5);
		return $"⚡: {(handlesDirectional ? '↘' : ' ')}{(handlesNumeric ? '⁹' : ' ')}";
	}

	string FormatDirectory(int totalTests) {
		return $"√ {totalTests,3}";
	}

	string FormatCalatlogueItem(string name, TestCatalogueItem item) {

		// Format as directory
		if (item.IsDirectory) {
			return $"{FormatDirectory(item.CountTests())} {name}";
		}

		// Format as test
		string eventIcons = FormatEvents(item.TestHandlesDirectionalEvents, item.TestHandlesNumericEvents);
		return $"{eventIcons} {name}";

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

		// Enter root directory
		TestCatalogue.ForceUpdateCurrentDirectoryState();

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

	#region //// State

	bool isShowingControls = false;

	TestRunner TestRunner { get; set; } = new();

	string? CurrentTestName { get; set; } = null;

	TestCatalogue TestCatalogue { get; set; } = new();

	#endregion

	void LoadTests() {

		StdOut.WriteLine("Finding tests...");
		TestCatalogue.FindAllTests();

		StdOut.WriteLine($"Found {TestCatalogue.Root.CountTests()} tests.");
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
				CurrentTestName = null;
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

			// Alpha-Numeric -- numeric event
			if (@event.Code >= Keyboard.Key.Num0 && @event.Code <= Keyboard.Key.Num9) {
				TestRunner.RunningTest?.HandleNumericEvent(@event.Code - Keyboard.Key.Num0);
				return;
			}

			// Numpad -- numeric event
			if (@event.Code >= Keyboard.Key.Numpad0 && @event.Code <= Keyboard.Key.Numpad9) {
				TestRunner.RunningTest?.HandleNumericEvent(@event.Code - Keyboard.Key.Numpad0);
				return;
			}

			// Direactional events
			if (@event.Code == Keyboard.Key.Up) {
				TestRunner.RunningTest?.HandleDirectionEvent(ArrowDirection.Up);
				return;
			}

			if (@event.Code == Keyboard.Key.Down) {
				TestRunner.RunningTest?.HandleDirectionEvent(ArrowDirection.Down);
				return;
			}

			if (@event.Code == Keyboard.Key.Left) {
				TestRunner.RunningTest?.HandleDirectionEvent(ArrowDirection.Left);
				return;
			}

			if (@event.Code == Keyboard.Key.Right) {
				TestRunner.RunningTest?.HandleDirectionEvent(ArrowDirection.Right);
				return;
			}

			return;

		// If on test select
		} else {

			// Escape -- hide help or close or back a directory
			if (@event.Code == Keyboard.Key.Escape) {
				if (isShowingControls) {
					isShowingControls = false;
				} else if (TestCatalogue.IsInASubDirectory) {
					TestCatalogue.CursorBack();
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

			// ignore other input if controls are shown or no tests are present
			if (isShowingControls || !TestCatalogue.HasTests) return;

			// Movement on test select
			if (@event.Code == Keyboard.Key.Up) {
				TestCatalogue.CursorMovePrevious();
				return;
			}

			if (@event.Code == Keyboard.Key.Down) {
				TestCatalogue.CursorMoveNext();
				return;
			}

			// Enter -- Start test or move into a subdirectory or back a subdirectory
			if (@event.Code == Keyboard.Key.Enter) {

				KeyValuePair<string, TestCatalogueItem>? selection = TestCatalogue.CursorSelect();

				// Back option selected
				if (selection is null) return;

				// Directory selected
				if (selection.Value.Value.IsDirectory) return;

				// Test selected
				CurrentTestName = selection.Value.Key;

				StdOut.WriteLine($"Starting test \"{CurrentTestName}\" (at {TestCatalogue.CurrentDirectoryPath})...");
				Window.SetTitle($"{WindowName} - {CurrentTestName}");

				TestRunner.StartTest(selection.Value.Value.TestClass!);
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
			WindowDrawer.DrawText($"Running \"{CurrentTestName}\"", Layout.TitleDisplay);
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

		string elapsedTime = "ELAPSED: ";
		if (TestRunner.IsRunningATest) {
			elapsedTime += TestRunner.RunningElapsedTime.ToString();
		} else {
			elapsedTime += "not running";
		}

        WindowDrawer.DrawText($"{initialStatus}; {runStatus}; {elapsedTime}", WindowDrawer.GetBottomLeft() + Layout.StatusDisplayOffset);

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
		} else if (!TestCatalogue.HasTests) {
			WindowDrawer.DrawText("No tests found.", Layout.TestSelectStartOffset);

		// or test select
		} else {

			// Find out which options fit
			int menuFirstDrawnOptionIndex, menuLastDrawnOptionIndex;

			float menuStartY = Layout.TestSelectStartOffset.Y;
			float menuEndY = Window.Size.Y + Layout.TestSelectEndY;

			int numberOfTestLines = (int)Math.Floor((menuEndY - menuStartY) / Layout.TestSelectOptionSpacing) + 1;

			int cursorIndex = TestCatalogue.CursorIndex;
			int optionsCount = TestCatalogue.CurrentDirectoryOptions.Count;

			if (numberOfTestLines <= 1) {
				// Single test fits or no tests fit
				menuFirstDrawnOptionIndex = cursorIndex;
				menuLastDrawnOptionIndex = cursorIndex;

			} else if (numberOfTestLines >= optionsCount) {
				// All tests fit
				menuFirstDrawnOptionIndex = 0;
				menuLastDrawnOptionIndex = optionsCount - 1;

			} else {

				// Some fit

				// Keep cursor in the middle
				int cursorPositionWithinVisible = numberOfTestLines / 2;

				// First are shown
				if (cursorIndex < cursorPositionWithinVisible) {
					menuFirstDrawnOptionIndex = 0;
					menuLastDrawnOptionIndex = numberOfTestLines - 1;

				// Last are shown
				} else if (cursorIndex - cursorPositionWithinVisible + numberOfTestLines >= optionsCount) {
					menuLastDrawnOptionIndex = optionsCount - 1;
					menuFirstDrawnOptionIndex = menuLastDrawnOptionIndex - numberOfTestLines + 1;

				// Middle is shown
				} else {
					menuFirstDrawnOptionIndex = cursorIndex - cursorPositionWithinVisible;
					menuLastDrawnOptionIndex = menuFirstDrawnOptionIndex + numberOfTestLines - 1;
				}

			}


			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = menuFirstDrawnOptionIndex; i <= menuLastDrawnOptionIndex; i++) {

				// Create and draw display row
				var rowItem = TestCatalogue.CurrentDirectoryOptions[i];
				WindowDrawer.DrawText(
					FormatCalatlogueItem(rowItem.Key, rowItem.Value), 
					currentOffset
				);
				
				// Draw cursor
				if (i == TestCatalogue.CursorIndex) {
					WindowDrawer.DrawText(">", new Vector2f(Layout.TestCursorX, currentOffset.Y));
				}

				// Advance positon
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
