using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Rephidock.GeneralUtilities.Collections;
using Rephidock.GeneralUtilities.Maths;


namespace Rephidock.AtomicAnimations.VisualTests;


public class TestCatalogue {

	#region //// Storage and creation

	public TestCatalogueItem Root { get; private init; } = TestCatalogueItem.CreateDirectory();

	public bool HasTests => Root.DirectoryItems!.Count > 0;

	public void FindAllTests() {

		IEnumerable<KeyValuePair<string, Type>> testsWithPaths =
			Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.IsSubclassOf(typeof(VisualTest)) && type.IsClass && !type.IsAbstract)
			.Where(type => type.GetConstructor(Type.EmptyTypes) is not null)
			.Select(
				type => KeyValuePair.Create(
					type.GetCustomAttribute<VisualTestMetaAttribute>()?.Name ?? type.Name,
					type
				)
			);

		foreach ((string testPath, Type testClass) in testsWithPaths) {

			string[] testPathParts = testPath.Split('/');

			// Traverse directories, creating missing
			TestCatalogueItem currentDirectory = Root;

			for (int i = 0; i < testPathParts.Length - 1; i++) {
				
				// Duplicate
				if (currentDirectory.DirectoryItems!.TryGetValue(testPathParts[i], out TestCatalogueItem existantItem)) {

					// Directory already exists -- traverse
					if (existantItem.IsDirectory) {
						currentDirectory = existantItem;
						continue;
					}

					throw new InvalidOperationException($"Duplicate name \"{testPathParts[i]}\" in a directory found.");
					
				}

				// Otherwise -- create new directory and traverse
				var newDirectory = TestCatalogueItem.CreateDirectory();
				currentDirectory.DirectoryItems!.Add(testPathParts[i], newDirectory);
				currentDirectory = newDirectory;
			}

			// Add an item unless already exists
			string testName = testPathParts[^1];
			if (currentDirectory.DirectoryItems!.ContainsKey(testName)) {
				throw new InvalidOperationException($"Duplicate name \"{testName}\" in a directory found.");
			}

			currentDirectory.DirectoryItems!.Add(testName, TestCatalogueItem.CreateTest(testClass));

		}

	}

	#endregion

	#region //// Live Traversal

	public TestCatalogue() {
		CurrentDirectoryOptions = new(currentDirectoryOrdered);
	}

	// Directory model
	readonly Stack<KeyValuePair<string, TestCatalogueItem>> subdirectoryStack = new();
	readonly List<KeyValuePair<string, TestCatalogueItem>> currentDirectoryOrdered = new();

	// Directory display
	public bool IsInASubDirectory => subdirectoryStack.Count > 0;
	public string CurrentDirectoryPath { get; private set; } = "/";
	public ReadOnlyCollection<KeyValuePair<string, TestCatalogueItem>> CurrentDirectoryOptions { get; }

	public void ForceUpdateCurrentDirectoryState() {

		// Update display path
		CurrentDirectoryPath = "/" + subdirectoryStack.Select(path => path.Key).Reverse().JoinString('/');

		// Make a list of options
		currentDirectoryOrdered.Clear();

		if (IsInASubDirectory) {
			currentDirectoryOrdered.Add(KeyValuePair.Create("..", new TestCatalogueItem()));
		}

		var currentDirectory = subdirectoryStack.Count == 0 ? Root : subdirectoryStack.Peek().Value;

		currentDirectoryOrdered.AddRange(
			currentDirectory
			.DirectoryItems!
			.Where(item => item.Value.IsDirectory)
			.OrderBy(item => item.Key)
		);

		currentDirectoryOrdered.AddRange(
			currentDirectory
			.DirectoryItems!
			.Where(item => !item.Value.IsDirectory)
			.OrderBy(item => item.Key)
		);

	}

	// Cursor
	public int CursorIndex { get; private set; } = 0;

	public void CursorMoveNext() {
		CursorIndex = (CursorIndex + 1).PosMod(currentDirectoryOrdered.Count);
	}

	public void CursorMovePrevious() {
		CursorIndex = (CursorIndex - 1).PosMod(currentDirectoryOrdered.Count);
	}

	/// <summary>
	/// Return the selected item name and item,
	/// or null if the back option was selected.
	/// </summary>
	public KeyValuePair<string, TestCatalogueItem>? CursorSelect() {

		KeyValuePair<string, TestCatalogueItem> selection = currentDirectoryOrdered[CursorIndex];

		// Back option ".."
		if (CursorIndex == 0 && IsInASubDirectory) {
			CursorBack();
			return null;
		}

		// Directory
		if (selection.Value.IsDirectory) {
			subdirectoryStack.Push(selection);
			ForceUpdateCurrentDirectoryState();
			CursorIndex = 0;
			return selection;
		}

		// Test
		return selection;

	}

	public void CursorBack() {

		if (!IsInASubDirectory) return;

		(string nameOfPoppedDirectory, _) = subdirectoryStack.Pop();
		ForceUpdateCurrentDirectoryState();

		CursorIndex = currentDirectoryOrdered.FindIndex(item => item.Key == nameOfPoppedDirectory);

		if (CursorIndex == -1) CursorIndex = 0; // Failsafe, should never occur.

	}

	#endregion

}
