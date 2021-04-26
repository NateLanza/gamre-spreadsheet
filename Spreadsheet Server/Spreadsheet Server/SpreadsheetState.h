#pragma once
#include <list>
#include <unordered_map>
#include <set>

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
	list<CellEdit> edits;

	/// <summary>
	/// Stores all dependencies in this SpreadsheetState
	/// </summary>
	DependencyGraph dependencies;

	/// <summary>
	/// Maps clients IDs to the cell they've selected
	/// </summary>
	unordered_map<int, string> selections;

	/// <summary>
	/// Checks cells for circular dependencies.
	/// Should be used before implementing a cell change, by feeding
	/// the list of variables for the new cell to toVisit.
	/// This should be encased in a read lock
	/// </summary>
	/// <param name="visited">Cells visited so far</param>
	/// <param name="toVisit">Cells to visit</param>
	/// <returns>True if circular dependency is found, else false</returns>
	const bool CheckCircularDependencies(unordered_set<string>& visited, const vector<string> toVisit);

	/// <summary>
	/// Checks whether a hypothetical new cell would create a circular dependency
	/// Can use a read lock, or not if already encased in one
	/// </summary>
	/// <param name="name">Name of cell</param>
	/// <param name="f">New formula for cell</param>
	/// <param name="readLock">Whether to use an internal read lock. Set to false if encased in a lock</param>
	/// <returns>True if a circular dependency would be created, else false</returns>
	const bool CheckNewCellCircular(const string name, Formula& f, const bool readLock);

	/// <summary>
	/// Locks a critical section for writing
	/// For use when writing any part of this object
	/// </summary>
	void WriteLock();
	/// <summary>
	/// Locks a critical section for reading
	/// For use when reading any part of this object
	/// </summary>
	void ReadLock();
	/// <summary>
	/// Unlocks this object for writing. For use at the end of critical sections
	/// </summary>
	void WriteUnlock();
	/// <summary>
	/// Unlocks this object for reading. For use at the end of any critical sections
	/// </summary>
	void ReadUnlock();

	/// <summary>
	/// Used to lock critical sections of code when the spreadsheet is being read/written
	/// A shared mutex can allow multiple reads simultaneously while forcing writes to
	/// occur sequentially with reads and other writes.
	/// Read operations should use lock_shared and unlock_shared
	/// Write operations should use lock and unlock
	/// </summary>
	shared_mutex threadkey;

	/// <summary>
	/// Removes a cell from the cell list if it is empty and
	/// has no prior state to revert to
	/// Uses a write lock if lock = true. Otherwise, should be encapsulated in a write lock
	/// </summary>
	/// <param name="lock">Whether to use a write lock</param>
	/// <param name="cell">Cell to maybe remove</param>
	void RemoveCellIfEmpty(const string cell, const bool lock);

	/// <summary>
	/// Adds a cell to cells if it doesn't already exist,
	/// or updates the existing entry if it does.
	/// Once finished, calls RemoveCellIfEmpty on the cell
	/// to remove it if the update emptied it.
	/// Uses a write lock if lock == true. Otherwise, should be encapsulated in a write lock
	/// </summary>
	/// <param name="lock">Whether to use a write lock</param>
	/// <param name="cellName">Name of cell</param>
	/// <param name="content">Cell content</param>
	void AddOrUpdateCell(const string cellName, const Formula& content, const bool lock);

	/// <summary>
	/// Checks whether a cell exists in this object's map
	/// Should be encased in a read or write lock, does not use one
	/// </summary>
	/// <param name="cell">Cell to check for</param>
	/// <returns>True if cell exists, else false</returns>
	const bool CellExists(const string cell) const;

public:
	/// <summary>
	/// Creates a new, blank spreadsheet
	/// </summary>
	SpreadsheetState();

	// Destructor
	~SpreadsheetState();

	/// <summary>
	/// Creates a spreadsheet from a set of cells
	/// Cell set is usually provided by the storage class, retreiving from a file
	/// Uses a write lock
	/// </summary>
	/// <param name="cells">Cells to initialize into spreadsheets</param>
	SpreadsheetState(set<Cell>& cells, list<CellEdit>& edits);

	/// <summary>
	/// Marks a cell as selected by a client
	/// Uses a write lock
	/// </summary>
	/// <param name="cell">Cell to select</param>
	/// <param name="ClientID">ID of client</param>
	/// <returns></returns>
	void SelectCell(const string cell, const int ClientID);

	/// <summary>
	/// Validates that a client has a cell selected
	/// Uses a read lock
	/// </summary>
	/// <param name="cell">Cell name</param>
	/// <param name="ClientID">ID of client</param>
	/// <returns>True if the client has the cell selected, else false</returns>
	bool ClientSelectedCell(const string cell, const int ClientID);

	/// <summary>
	/// Edits the content of a cell and adds the edit to the edit stack
	/// Will use a write lock. Do NOT encase in any locks
	/// </summary>
	/// <param name="name">Cell name</param>
	/// <param name="content">New content</param>
	/// <param name="ClientID">ID of client</param>
	/// <returns>True if cell contents were edited successfully,
	/// false if content format was invalid,
	///  the edit would create a circular dependency,
	/// or the client does not currently have that cell selected</returns>
	bool EditCell(const string name, const string content, const int ClientID);

	/// <summary>
	/// Reverts most recent change to a certain cell and adds the revert to the edit stack
	/// Will use a write lock. Do NOT encase in any locks
	/// </summary>
	/// <param name="cell">Cell to revert</param>
	/// <returns>True if revert successfull, 
	/// false if revert would create a circular dependency</returns>
	bool RevertCell(const string cell);

	/// <summary>
	/// Undoes the last edit to the spreadsheet
	/// Will use a write lock. Do NOT encase in any locks
	/// </summary>
	/// <returns>True if edit undone, false if edit would create a circular dependency</returns>
	bool UndoLastEdit();

	/// <summary>
	/// Returns all edits made to this spreadsheet as a stack, with most recent at the top
	/// Will use a read lock
	/// </summary>
	/// <returns>Stack of edits</returns>
	list<CellEdit> GetEditHistory();

	/// <summary>
	/// Gets all non-empty cells in this spreadsheet
	/// Also prunes all empty cells from the spreadsheet's map
	/// Will use a read lock and multiple write locks
	/// </summary>
	/// <returns>Cells, as a list</returns>
	set<Cell> GetPopulatedCells();

	/// <summary>
	/// Checks whether a cell has content
	/// Will use a read lock if lock == true, otherwise should be encased in a lock
	/// </summary>
	/// <param name="cell">Cell to check for content</param>
	/// <param name="lock">Whether to use a read lock. MUST be true if used outside this class</param>
	/// <returns>True if cell is not empty (has content), else false</returns>
	const bool CellNotEmpty(const string cell, const bool lock);

	/// <summary>
	/// Gets the contents of a cell
	/// Will use a read lock
	/// Throws an exception if cell is empty
	/// </summary>
	/// <param name="name">Cell to get</param>
	/// <returns>Current contents of the cell</returns>
	const string GetCell(const string name);

};

#endif