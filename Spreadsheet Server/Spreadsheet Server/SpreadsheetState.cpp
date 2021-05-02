#include "SpreadsheetState.h"
#include <iostream>

// See SpreadsheetState.h for full method documentation

/// <summary>
/// Default constructor. Initializes all fields to empty values
/// </summary>
SpreadsheetState::SpreadsheetState() : cells(), edits(), dependencies(), selections(), threadkey()
{
	threadkey = new shared_mutex();
}

SpreadsheetState::SpreadsheetState(set<Cell>& cells, list<CellEdit>& edits) : cells(), edits(edits), dependencies(), threadkey(), selections() {
	threadkey = new shared_mutex();
	// Edits are set by the initializer list, now we just need to map dependencies & cells
	WriteLock();
	for (Cell cell : cells) {
		// Set dependencies
		for (string var : cell.GetVariables()) {
			dependencies.AddDependency(var, cell.GetName());
		}
		// Add to cell list
		AddOrUpdateCell(cell.GetName(), cell.GetContents(), false);
	}
	WriteUnlock();
}

/// <summary>
/// Deletes all ptrs in this class
/// </summary>
SpreadsheetState::~SpreadsheetState() {
	cells.clear();
	while (!edits.empty())
		edits.pop_front();
	selections.clear();
	// We have to explicitly call delete on pointers
	delete threadkey;
	// Destructors are called automatically
}

void SpreadsheetState::SelectCell(const string cell, const int ClientID) {
	WriteLock();
	selections[ClientID] = cell;
	WriteUnlock();
}

bool SpreadsheetState::ClientSelectedCell(const string cell, const int ClientID) {
	ReadLock();
	bool result = selections.count(ClientID) == 1 && selections[ClientID] == cell;
	ReadUnlock();
	return result;
}

bool SpreadsheetState::EditCell(const string name, const string content, const int ClientID) {
	// Make sure the client has this cell selected
	if (!ClientSelectedCell(name, ClientID)) {
		return false;
	}

	// Check whether the new content is valid
	try {
		if (!ValidCellContents(content))
			return false;

		// Check for circular dependencies
		if (CheckNewCellCircular(name, content, true))
			return false;

		WriteLock();
		// Save old contents of cell
		string oldContents = CellExists(name) ? cells[name].GetContents() : "";

		// No circular dependencies found, add cell
		edits.push_front(CellEdit(name, oldContents)); // Add cellEdit
		AddOrUpdateCell(name, content, false); // Modify cell
		dependencies.ReplaceDependents(name, cells[name].GetVariables()); // Modify dependencies
		WriteUnlock();
		return true;
	}
	catch (exception) {
		WriteUnlock();
		return false;
	}

}

const bool SpreadsheetState::CheckNewCellCircular(const string name, const string& f, const bool readLock) {
	unordered_set<string> visited = unordered_set<string>();
	visited.insert(name);
	if (readLock)
		ReadLock(); // Read lock
	if (CheckCircularDependencies(visited, Cell(name, f).GetVariables())) {
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
		if (CellExists(cell) && CheckCircularDependencies(visited, cells[cell].GetVariables()))
			return true;
	}

	return false;
}

bool SpreadsheetState::RevertCell(const string cell) {
	WriteLock();
	// Make sure cell exists & can be reverted
	if (!CellExists(cell) || !cells[cell].CanRevert()) {
		WriteUnlock();
		cout << "cell doesn't exist or something" << endl;
		cout << !CellExists(cell) << endl;
		cout << !cells[cell].CanRevert() << endl;
		return false;
	}

	// Get cell's old state
	string oldState = cells[cell].GetPreviousState();

	// Check for circular dependencies
	if (CheckNewCellCircular(cell, oldState, false)) {
		WriteUnlock();
		return false;
	}

	// No circular dependencies found, go through with revert
	//possible error with the new
	edits.push_front(CellEdit(cell, cells[cell].GetContents())); // Add cellEdit
	bool result = cells[cell].Revert(); // Revert cell
	dependencies.ReplaceDependents(cell, cells[cell].GetVariables()); // Modify dependencies
	WriteUnlock();
	return true;
}

tuple<bool, string> SpreadsheetState::UndoLastEdit() {
	// Writelock the method so that the edit stack doesn't change
	WriteLock();
	if (edits.size() == 0) {
		WriteUnlock();
		return tuple<bool,string>(false,"No more edits to undo");
	}

	// Validate undo
	string name = edits.front().GetName();
	string f = edits.front().GetPriorContents();
	if (CheckNewCellCircular(name, f, false)) {
		WriteUnlock();
		return tuple<bool, string>(false, "Invalid cell change");
	}

	// Undo validated, implement it
	AddOrUpdateCell(name, f, false);
	dependencies.ReplaceDependents(name, cells[name].GetVariables());
	edits.pop_front();
	WriteUnlock();

	return tuple<bool, string>(true, name);
}

void SpreadsheetState::AddOrUpdateCell(const string cellName, const string& content, const bool lock) {
	if (lock)
		WriteLock();
	if (CellExists(cellName)) {
		cells[cellName].SetContents(content);
	}
	else {
		if (!CellExists(cellName))
			cells.emplace(cellName, Cell(cellName, ""));
		cells[cellName].SetContents(content);
	}
	if (lock)
		WriteUnlock();
}

const bool SpreadsheetState::CellExists(const string cell) const
{
	return cells.count(cell) == 1;
}

bool SpreadsheetState::ValidCellContents(const string contents)
{
	//if empty string, its fine
	if (contents.size() == 0)
		return true;

	//if the first character is '=', we check if its a valid formula
	if (contents[0] == '=') {
		try
		{
			Formula f = Formula(contents);
			return true;
		}
		catch (exception e)
		{
			return false;
		}
	}

	//if its not a formula, any valid string is fine
	return true;
}

list<CellEdit> SpreadsheetState::GetEditHistory() {
	ReadLock();
	list<CellEdit> result(edits);
	ReadUnlock();
	return result;
}

set<Cell> SpreadsheetState::GetPopulatedCells() {
	set<Cell> result = set<Cell>();

	ReadLock();
	// Put all cells in result list
	for (pair<string, Cell> cellEntry : cells)
		result.insert(cellEntry.second);
	ReadUnlock();
	return result;
}

void SpreadsheetState::WriteLock() {
	threadkey->lock();
}

void SpreadsheetState::ReadLock() {
	threadkey->lock_shared();
}

void SpreadsheetState::WriteUnlock() {
	threadkey->unlock();
}

void SpreadsheetState::ReadUnlock() {
	threadkey->unlock_shared();
}

const string SpreadsheetState::GetCell(const string name) {
	ReadLock();
	if (CellExists(name)) {
		string result = cells[name].GetContents();
		ReadUnlock();
		return result;
	}
	else {
		ReadUnlock();
		throw runtime_error(string("Cell " + name + " has no content to get"));
	}
}