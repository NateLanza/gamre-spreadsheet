#include "SpreadsheetState.h"

// See SpreadsheetState.h for full method documentation

/// <summary>
/// Default constructor. Initializes all fields to empty values
/// </summary>
SpreadsheetState::SpreadsheetState(): cells(), edits(), dependencies()
{
}

SpreadsheetState::SpreadsheetState(set<Cell>& cells, stack<CellEdit>& edits) : cells(), edits(edits), dependencies() {
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
		Formula f(content);
	}
	catch (exception) {
		return false;
	}


}
