#include "SpreadsheetState.h"

// See SpreadsheetState.h for full method documentation

/// <summary>
/// Default constructor. Initializes all fields to empty values
/// </summary>
SpreadsheetState::SpreadsheetState(): cells(), edits(), dependencies()
{
}

SpreadsheetState::SpreadsheetState(set<Cell>& cells, stack<CellEdit>& edits) : cells(), edits(edits), dependencies(), threadkey() {
	// Edits are set by the initializer list, now we just need to map dependencies & cells
	for (Cell cell : cells) {
		// Set dependencies
		for (string var : cell.GetVariables()) {
			dependencies.AddDependency(var, cell.GetName());
		}
		// Add to cell list
		this->cells[cell.GetName()] = cell;
	}
}

bool SpreadsheetState::EditCell(const string name, const string content) {
	// Check whether the new content is valid
	try {
		// This will throw on an invalid format
		Formula f(content);
		
		// Check for circular dependencies
		if (CheckNewCellCircular(name, f))
			return false;
		
		// Save old contents of cell
		threadkey.lock_shared(); // Read lock
		string oldContents = CellNotEmpty(name) ? cells[name].GetContents() : "";
		threadkey.unlock_shared(); // Read unlock

		// No circular dependencies found, add cell
		threadkey.lock(); // Write lock
		edits.push(CellEdit(name, oldContents)); // Add cellEdit
		cells[name].SetContents(f); // Modify cell
		dependencies.ReplaceDependents(name, f.GetVariables()); // Modify dependencies
		threadkey.unlock(); // Write unlock
		return true;
	}
	catch (exception) {
		return false;
	}

	
}

const bool SpreadsheetState::CheckNewCellCircular(const string name, const Formula& f) {
	unordered_set<string> visited = unordered_set<string>();
	visited.insert(name);
	threadkey.lock_shared(); // Read lock
	if (CheckCircularDependencies(visited, f.GetVariables())) {
		threadkey.unlock_shared(); // Read unlock
		return true;
	}

	// No circular depen found
	threadkey.unlock_shared(); // Read unlock
	return false;
}

const bool SpreadsheetState::CheckCircularDependencies(unordered_set<string>& visited, const vector<string> toVisit) {
	for (string cell : toVisit) {
		// See if we've already visited this cell
		if (visited.count(cell) == 1)
			return true;
		// Visit this cell
		visited.emplace(cell);
		if (CellNotEmpty(cell) && CheckCircularDependencies(visited, cells[cell].GetVariables())) {
			return true;
		}
	}

	return false;
} 