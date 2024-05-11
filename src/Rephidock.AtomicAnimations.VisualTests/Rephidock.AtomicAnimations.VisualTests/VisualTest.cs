﻿using System;
using System.Collections.Generic;
using SFML.Graphics;


namespace Rephidock.AtomicAnimations.VisualTests;


public abstract class VisualTest : IDisposable {

	/// <summary>Called one time right after the object is constructed.</summary>
	public abstract void Start(TimeSpan startTime);

	/// <summary>Called every tick during time flow.</summary>
	public abstract void Update(TimeSpan deltaTime);

	/// <summary>Called every frame to render relevant objects.</summary>
	public abstract IEnumerable<Drawable> GetDrawables(FloatRect windowSize);

	/// <summary>Called when managed objects need to be disposed.</summary>
	protected virtual void DisposeManaged() { }

	#region //// IDisposable

	private bool isDisposed;

	protected void Dispose(bool disposingManaged) {

		if (isDisposed) return;

		if (disposingManaged) {
			DisposeManaged();
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
