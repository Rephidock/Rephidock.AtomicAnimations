using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;


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
			.Select(pair => (pair.Item1 ?? VisualTestMetaAttribute.DefaultMeta, pair.Item2))
			.OrderBy(pair => pair.Item1.Name)
			.ToList()
			.AsReadOnly();

	}

	#endregion


	#region //// IDisposable

	private bool isDisposed;

	protected virtual void Dispose(bool disposingManaged) {

		if (isDisposed) return;

		if (disposingManaged) {

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
