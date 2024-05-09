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

		public readonly static Vector2f FpsDisplayOffset = new(5, -MainFontLineSpacing);

		public readonly static Vector2f TestSelectStartOffset = new(5, 5);

		public readonly static float TestSelectOptionSpacing = MainFontLineSpacing + 5;

		public readonly static float TestSelectEndY = FpsDisplayOffset.Y - 2 * MainFontLineSpacing;
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

	#region //// Test exploring and running

	/// <remarks>Initialized in <see cref="Run"/></remarks>
	IReadOnlyList<(VisualTestMetaAttribute meta, Type type)> AllTests { get; set; } = null!;


	void LoadTests() {
		StdOut.WriteLine("Finding tests...");

		AllTests = Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.IsSubclassOf(typeof(VisualTest)) && type.IsClass && !type.IsAbstract)
			.Where(type => type.GetConstructor(Type.EmptyTypes) is not null)
			.Select(type => (type.GetCustomAttribute<VisualTestMetaAttribute>(), type))
			.Where(pair => pair.Item1 is not null)
			.Cast<(VisualTestMetaAttribute, Type)>()
			.OrderBy(pair => pair.Item1.Name)
			.ToList()
			.AsReadOnly();

		StdOut.WriteLine($"Found {AllTests.Count} tests:");
		foreach (var testName in AllTests.Select(pair => pair.meta.Name)) {
			StdOut.WriteLine(testName);
		}
	}

	void OnKeyPressed(KeyEventArgs @event) {

		// Escape -- close
		if (@event.Code == Keyboard.Key.Escape) {
			OnCloseRequest();
			return;
		}

	}

	void OnTick(TimeSpan deltaTime) {

		// Clear previous frame
		Window.Clear(Color.Black);

		// Draw delta time and fps
		DrawText($"Δt={deltaTime.Milliseconds:D3} (~{1 / deltaTime.TotalSeconds:F0} fps)", WindowGetBottomLeft() + Layout.FpsDisplayOffset);

		// Draw test select
		if (AllTests.Count == 0) {
			DrawText("No tests found.", Layout.TestSelectStartOffset);

		} else {

			Vector2f currentOffset = Layout.TestSelectStartOffset;

			for (int i = 0; i < AllTests.Count; i++) {

				DrawText(AllTests[i].meta.Name, currentOffset);

				currentOffset.Y += Layout.TestSelectOptionSpacing;
				if (currentOffset.Y >= Window.Size.Y + Layout.TestSelectEndY) break;
			}

		}

	}

	#endregion

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
