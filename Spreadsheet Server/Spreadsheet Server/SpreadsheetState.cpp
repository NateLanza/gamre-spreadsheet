#include "SpreadsheetState.h"

// See SpreadsheetState.h for full method documentation

/// <summary>
/// Default constructor. Initializes all fields to empty values
/// </summary>
SpreadsheetState::SpreadsheetState(): cells(), edits(), dependencies()
{
}

SpreadsheetState::SpreadsheetState(set<Cell>& cells, list<CellEdit>& edits) : cells(), edits(edits), dependencies(), threadkey() {
	// Edits are set by the initializer list, now we just need to map dependencies & cells
	WriteLock();
	for (Cell cell : cells) {
		// Set dependencies
		for (string var : cell.GetVariables()) {
			dependencies.AddDependency(var, cell.GetName());
		}
		// Add to cell list
		this->cells[cell.GetName()] = *new Cell(cell.GetName(), Formula(cell.GetContents()));
	}
	WriteUnlock();
}

SpreadsheetState::~SpreadsheetState() {
	cells.clear();
	while (!edits.empty())
		edits.pop_front();
	delete &dependencies;
}

bool SpreadsheetState::EditCell(const string name, const string content) {
	WriteLock();
	// Check whether the new content is valid
	try {
		// This will throw on an invalid format
		Formula f(content);
		
		// Check for circular dependencies
		if (CheckNewCellCircular(name, f, false)) {
			WriteUnlock();
			return false;
		}
			
		
		// Save old contents of cell
		string oldContents = CellNotEmpty(name) ? cells[name].GetContents() : "";

		// No circular dependencies found, add cell
		edits.push_front(* new CellEdit(cells[name])); // Add cellEdit
		AddOrUpdateCell(name, f, false); // Modify cell
		dependencies.ReplaceDependents(name, f.GetVariables()); // Modify dependencies
		WriteUnlock();
		return true;
	}
	catch (exception) {
	}

	WriteUnlock();
	return false;
}

const bool SpreadsheetState::CheckNewCellCircular(const string name, Formula& f, const bool readLock) {
	unordered_set<string> visited = unordered_set<string>();
	visited.insert(name);
	if (readLock)
		ReadLock(); // Read lock
	if (CheckCircularDependencies(visited, f.GetVariables())) {
		if (readLock)
			ReadUnlock(); // Read unlock
		return true;
	}

	// No circular depen found
	if (readLock)
		ReadUnlock();
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

bool SpreadsheetState::RevertCell(const string cell) {
	WriteLock();

	// Get cell's old state
	Formula oldState = cells[cell].GetPreviousState();

	// Check for circular dependencies
	if (CheckNewCellCircular(cell, oldState, false)) {
		WriteUnlock();
		return false;
	}

	// No circular dependencies found, go through with revert
	edits.push_front(* new CellEdit(cells[cell])); // Add cellEdit
	cells[cell].Revert(); // Revert cell
	dependencies.ReplaceDependents(cell, oldState.GetVariables()); // Modify dependencies
	WriteUnlock();
	return true;
}

bool SpreadsheetState::UndoLastEdit() {
	// Writelock the method so that the edit stack doesn't change
	WriteLock();
	// Validate undo
	string name = edits.front().GetName();
	Formula f = edits.front().GetPriorContents();
	if (CheckNewCellCircular(name, f, false)) {
		WriteUnlock();
		return false;
	}

	// Undo validated, implement it
	AddOrUpdateCell(name, f, false);
	dependencies.ReplaceDependents(name, f.GetVariables());
	edits.pop_front();
	WriteUnlock();

	return true;
}

void SpreadsheetState::AddOrUpdateCell(const string cellName, const Formula& content, const bool lock) {
	if (lock)
		WriteLock();
	if (cells.count(cellName) == 1) {
		cells[cellName].SetContents(content);
	} else {
		cells[cellName] = *(new Cell(cellName, content));
	}
	RemoveCellIfEmpty(cellName, false);
	if (lock)
		WriteUnlock();
}

list<CellEdit> SpreadsheetState::GetEditHistory() {
	ReadLock();
	list<CellEdit> result(edits);
	ReadUnlock();
	return result;
}

list<Cell> SpreadsheetState::GetPopulatedCells() {
	list<Cell> result = list<Cell>();
	ReadLock();
	// Put all cells in list
	for (pair<string, Cell> cellEntry : cells)
		result.push_back(cellEntry.second);
	ReadUnlock();
	return result;
}

const bool SpreadsheetState::CellNotEmpty(const string cell) {
	ReadLock();
	if (cells.count(cell) == 1) {
		if (cells[cell].GetContents() != "") {
			ReadUnlock();
			return true;
		} else {
			ReadUnlock();
			RemoveCellIfEmpty(cell, true);
			return false;
		}
	} else {
		ReadUnlock();
		return false;
	}
}

void SpreadsheetState::RemoveCellIfEmpty(const string cell, const bool lock) {
	if (lock)
		WriteLock();
	if (cells.count(cell) == 1 && 
		cells[cell].GetContents() == "" && 
		cells[cell].GetPreviousState().ToString() == "") {
		cells.erase(cell);
	}
	if (lock)
		WriteUnlock();
}

void SpreadsheetState::WriteLock() {
	threadkey.lock();
}

void SpreadsheetState::ReadLock() {
	threadkey.lock_shared();
}

void SpreadsheetState::WriteUnlock() {
	threadkey.unlock();
}

void SpreadsheetState::ReadUnlock() {
	threadkey.unlock_shared();
}