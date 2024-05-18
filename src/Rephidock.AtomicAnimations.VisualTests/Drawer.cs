using SFML.System;
using SFML.Graphics;


namespace Rephidock.AtomicAnimations.VisualTests;


public class Drawer {

	#region //// Creation and storage

	/// <summary>
	/// The target to draw to.
	/// Injected into and not disposed of by the current <see cref="Drawer"/> instance.
	/// </summary>
	protected RenderTarget DrawTarget { get; }

	/// <summary>
	/// The default font.
	/// Injected into and not disposed of by the current <see cref="Drawer"/> instance.
	/// </summary>
	protected Font MainFont { get; }

	/// <summary>The default font size</summary>
	protected uint MainFontSize { get; }

	/// <summary>The line spacing of the default font</summary>
	public int MainFontLineSpacing { get; }

	public Drawer(RenderTarget drawTarget, Font defaultFont, uint defaultFontDefaultSize, int defaultFontLineSpacing) {
		DrawTarget = drawTarget;
		MainFont = defaultFont;
		MainFontSize = defaultFontDefaultSize;
		MainFontLineSpacing = defaultFontLineSpacing;
	}

	#endregion
	
	#region //// Positions

	public Vector2f GetBottomLeft() => new(0, DrawTarget.Size.Y);

	public Vector2f GetBottomRight() => new(DrawTarget.Size.X, DrawTarget.Size.Y);

	public Vector2f GetTopRight() => new(DrawTarget.Size.X, 0);

	#endregion

	#region //// Drawing

	public void DrawDrawable(Drawable drawable) {
		DrawTarget.Draw(drawable);
	}

	public void DrawText(string text, Vector2f position) {
		DrawText(text, position, Color.White);
	}

	public void DrawText(string text, Vector2f position, Color color) {

		using Text textObject = new() {
			Font = MainFont,
			Position = position,
			CharacterSize = MainFontSize,
			DisplayedString = text,
			FillColor = color
		};

		DrawTarget.Draw(textObject);
	}

	public void DrawRectangle(FloatRect rect, Color color) {

		using RectangleShape rectShape = new() {
			Position = new Vector2f(rect.Left, rect.Top),
			Size = new Vector2f(rect.Width, rect.Height),
			FillColor = color
		};

		DrawTarget.Draw(rectShape);
	}

	public void DrawRectangle(FloatRect rect, Vector2f scale, float rotation, Vector2f normalizedOrgin, Color color) {

		using RectangleShape rectShape = new() {
			Position = new Vector2f(rect.Left, rect.Top),
			Size = new Vector2f(rect.Width, rect.Height),
			Origin = new Vector2f(rect.Width * normalizedOrgin.X, rect.Height * normalizedOrgin.Y),
			Scale = scale,
			Rotation = rotation,
			FillColor = color
		};

		DrawTarget.Draw(rectShape);
	}

	public void DrawCirle(Vector2f position, float radius, Color color, uint pointCount = 32) {

		using CircleShape rectShape = new(radius, pointCount) {
			Position = position,
			FillColor = color
		};

		DrawTarget.Draw(rectShape);
	}

	#endregion

}
