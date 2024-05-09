using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestExplorer : IDisposable {
	
	public required TextWriter StdOut { get; init; }

	#region //// Assets and layout constants

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected Font MainFont { get; private set; } = null!;

	const uint MainFontSize = 18;

	const uint MainFontLineSpacing = 24; // Hardcoded to be const. Retrivied from `MainFont.GetLineSpacing(MainFontSize)`

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

	#region //// Window and Main Loop

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	protected RenderWindow Window { get; private set; } = null!;

	const string WindowName = "Atomic Animations Visual Tests";
	static VideoMode WindowSize = new(800, 600);
	const uint WindowFrameRate = 60;


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
		"",
		"= Test =",
		"[esc]: Back",
		"[enter]: Restart test",
		"[space]: Pause/Resume test OR Step",
		"",
		"= Either =",
		"[shift]+[↑], [shift]+[↓]: Change speed mult.",
		"[alt]+[↑], [alt]+[↓]: Change initial time"
	];

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	TestRunner TestRunner { get; set; } = null!;


	void LoadTests() {
		StdOut.WriteLine("Finding tests...");

		TestRunner = new TestRunner();

		StdOut.WriteLine($"Found {TestRunner.TestCount} tests:");
		foreach (var testName in TestRunner.AllTests.Select(pair => pair.meta.Name)) {
			StdOut.WriteLine(testName);
		}
	}

	void OnKeyPressed(KeyEventArgs @event) {

		// Escape -- close
		if (@event.Code == Keyboard.Key.Escape) {
			OnCloseRequest();
			return;
		}

		// Help toggle
		if (@event.Code == Keyboard.Key.F1) {
			isShowingControls = !isShowingControls;
		}

	}

	void OnTick(TimeSpan deltaTime) {

		// Clear previous frame
		Window.Clear(Color.Black);

		// Draw title
		DrawText(DefaultTitle, Layout.TitleDisplay);
		// Draw delta time and fps
		DrawText($"Δt={deltaTime.Milliseconds:D3} (~{1 / deltaTime.TotalSeconds:F0} fps)", WindowGetBottomLeft() + Layout.FpsDisplayOffset);

		// Draw controls
		if (isShowingControls) {

			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = 0; i < ControlsHelp.Count; i++) {

				// Check if this is the last line to be drawn
				bool isThisLineLast = currentOffset.Y + Layout.TestSelectOptionSpacing >= Window.Size.Y + Layout.TestSelectEndY;

				// If it is and there is more text -- draw ....
				if (isThisLineLast && i != ControlsHelp.Count - 1) {
					DrawText("...", currentOffset);

				// Otherwise draw if not empty
				} else if (!string.IsNullOrEmpty(ControlsHelp[i])) {
					DrawText(ControlsHelp[i], currentOffset);
				}

				// Advance to next line
				if (isThisLineLast) break;
				currentOffset.Y += Layout.TestSelectOptionSpacing;
			}

		// or no tests message
		} else if (TestRunner.TestCount == 0) {
			DrawText("No tests found.", Layout.TestSelectStartOffset);

		// or test select
		} else {

			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = 0; i < TestRunner.AllTests.Count; i++) {

				DrawText(TestRunner.AllTests[i].meta.Name, currentOffset);

				currentOffset.Y += Layout.TestSelectOptionSpacing;
				if (currentOffset.Y >= Window.Size.Y + Layout.TestSelectEndY) break;
			}

		}

	}

	#region //// Draw shortcuts

	Vector2f WindowGetBottomLeft() => new Vector2f(0, Window.Size.Y);

	void DrawText(string text, Vector2f position) {

		using Text textObject = new() {
			Font = MainFont,
			Position = position,
			CharacterSize = MainFontSize,
			DisplayedString = text
		};

		Window.Draw(textObject);
	}

	#endregion

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
