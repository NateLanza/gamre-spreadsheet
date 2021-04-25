#pragma once
#include <list>
#include <unordered_map>
#include <set>
#include <stack>
#include <windows.data.json.h>
#include <windows.networking.sockets.h>

#include "Cell.h"
#include "CellEdit.h"
#include "EditRequest.h"
#include <shared_mutex>

#include "DependencyGraph.h"

using namespace std;

#ifndef SpreadsheetState_H
#define SpreadsheetState_H

/// <summary>
/// Represents the state of a currently open spreadsheet
/// </summary>
class SpreadsheetState
{

private:
	/// <summary>
	/// All cells in this spreadsheet
	/// </summary>
	unordered_map<string, Cell> cells;

	/// <summary>
	/// All edits made to this spreadsheet, in order of recency.
	/// </summary>
	stack<CellEdit> edits;

	/// <summary>
	/// Stores all dependencies in this SpreadsheetState
	/// </summary>
	DependencyGraph dependencies;

	/// <summary>
	/// Checks cells for circular dependencies.
	/// Should be used before implementing a cell change, by feeding
	/// the list of variables for the new cell to toVisit.
	/// This should be encased in a shared lock
	/// </summary>
	/// <param name="visited">Cells visited so far</param>
	/// <param name="toVisit">Cells to visit</param>
	/// <returns>True if circular dependency is found, else false</returns>
	const bool CheckCircularDependencies(unordered_set<string>& visited, const vector<string> toVisit);

	/// <summary>
	/// Checks whether a hypothetical new cell would create a circular dependency
	/// Will use a shared lock
	/// </summary>
	/// <param name="name">Name of cell</param>
	/// <param name="f">New formula for cell</param>
	/// <returns>True if a circular dependency would be created, else false</returns>
	const bool CheckNewCellCircular(const string name, const Formula& f);

	/// <summary>
	/// Used to lock critical sections of code when the spreadsheet is being read/written
	/// A shared mutex can allow multiple reads simultaneously while forcing writes to
	/// occur sequentially with reads and other writes.
	/// Read operations should use lock_shared and unlock_shared
	/// Write operations should use lock and unlock
	/// </summary>
	shared_mutex threadkey;

public:
	/// <summary>
	/// Creates a new, blank spreadsheet
	/// </summary>
	SpreadsheetState();

	/// <summary>
	/// Creates a spreadsheet from a set of cells
	/// Cell set is usually provided by the storage class, retreiving from a file
	/// </summary>
	/// <param name="cells">Cells to initialize into spreadsheets</param>
	SpreadsheetState(set<Cell>& cells, stack<CellEdit>& edits);

	/// <summary>
	/// Edits the content of a cell and adds the edit to the edit stack
	/// Will use a shared and exclusive lock. Do NOT encase in any locks
	/// </summary>
	/// <param name="name">Cell name</param>
	/// <param name="content">New content</param>
	/// <returns>True if cell contents were edited successfully,
	/// false if content format was invalid or the edit would create a circular dependency</returns>
	bool EditCell(const string name, const string content);

	/// <summary>
	/// Reverts most recent change to a certain cell and adds the revert to the edit stack
	/// </summary>
	/// <param name="cell">Cell to revert</param>
	/// <returns>True if revert successfull, false if revert would create a circular dependency</returns>
	bool RevertCell(const string cell);

	/// <summary>
	/// Undoes the last edit to the spreadsheet
	/// </summary>
	/// <returns>True if edit undone, false if edit would create a circular dependency</returns>
	bool UndoLastEdit();

	/// <summary>
	/// Returns all edits made to this spreadsheet as a stack, with most recent at the top
	/// </summary>
	/// <returns>Stack of edits</returns>
	const stack<CellEdit> GetEditHistory() const;

	/// <summary>
	/// Gets all non-empty cells in this spreadsheet
	/// </summary>
	/// <returns>Cells, as a list</returns>
	const list<Cell> GetPopulatedCells() const;

	/// <summary>
	/// Checks whether a cell has content
	/// </summary>
	/// <param name="cell">Cell to check for content</param>
	/// <returns>True if cell is not empty (has content), else false</returns>
	const bool CellNotEmpty(const string cell) const;

};

#endif