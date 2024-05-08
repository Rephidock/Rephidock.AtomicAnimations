using System;
using SFML.Graphics;
using SFML.Window;


namespace Rephidock.AtomicAnimations.VisualTests;


internal class Program {


	static void Main(string[] args) {

		
		Console.WriteLine("Hello, World!");

		RenderWindow window = new(new VideoMode(800, 600), "Atomic Animations Visual Tests");
		window.Closed += (_, _) => window.Close();


		while (window.IsOpen) {

			window.DispatchEvents();

			window.Clear(Color.Black);
			window.Display();

		}

		window.Dispose();
       
	}

}
