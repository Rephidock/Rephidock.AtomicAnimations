using System;
using System.IO;
using SFML.Graphics;
using SFML.Window;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestExplorer : IDisposable {
	
	public required TextWriter StdOut { get; init; }

	protected RenderWindow Window { get; private set; } = null!;
	const string WindowName = "Atomic Animations Visual Tests";
	static VideoMode WindowSize = new(800, 600);


	public void Run() {

		StdOut.WriteLine("Launching visual tester...");

		// Create window and explorer
		Window = new(WindowSize, WindowName);

		// Setup events
		Window.Closed += (_, _) => OnCloseRequest();
		Window.KeyPressed += (_, @event) => OnKeyPressed(@event);

		// Run main event loop
		while (Window.IsOpen) {
			Window.DispatchEvents();
			StepAndDraw();
			Window.Display();
		}

	}

	void OnCloseRequest() {
		StdOut.WriteLine("Closing...");
		Window.Close();
	}

	void OnKeyPressed(KeyEventArgs @event) {

		// Escape -- close
		if (@event.Code == Keyboard.Key.Escape) {
			OnCloseRequest();
			return;
		}

	}

	void StepAndDraw() {
		Window.Clear(Color.Black);
	}


	#region //// IDisposable


	private bool isDisposed;

	protected virtual void Dispose(bool disposingManaged) {

		if (isDisposed) return;

		if (disposingManaged) {
			Window?.Dispose();
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
