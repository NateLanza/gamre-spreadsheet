#pragma once
#include <string>
#include <list>
#include <map>
#include <stack>
#include "Cell.h"
#include "SpreadsheetState.h"

using namespace std;

#ifndef Storage_H
#define Storage_H

/// <summary>
/// Simple wrapper struct for the list of Cells and stack of CellEdits 
/// necessary to represent a spreadsheet
/// </summary>
struct StoredSpreadsheet {
public:
	/// <summary>
	/// Cells in the spreadsheet
	/// </summary>
	set<Cell> cells;
	/// <summary>
	/// Edit history
	/// </summary>
	stack<CellEdit> edits;

	/// <summary>
	/// Creates a new StoredSpreadsheet from cells & edits
	/// </summary>
	/// <param name="cells">Non-empty cells in the spreadsheet</param>
	/// <param name="edits">Edit history; most recent edit at the top</param>
	StoredSpreadsheet(set<Cell>& cells, stack<CellEdit>& edits);
};

/// <summary>
/// Handles storing of spreadsheets to files
/// </summary>
class Storage
{
public:
	/// <summary>
	/// Opens a spreadsheet from file if it exists. 
	/// If not, returns an empty list of cells & empty stack of CellEdits
	/// </summary>
	/// <param name="spreadsheetName">Spreadsheet to open (provided by client)</param>
	/// <returns>Spreadsheet data</returns>
	StoredSpreadsheet Open(const string spreadsheetName);

	/// <summary>
	/// Saves a spreadsheet to file
	/// </summary>
	/// <param name="spreadsheetName">Filename</param>
	/// <param name="ss">Spreadsheet data</param>
	void Save(const string spreadsheetName, const StoredSpreadsheet& ss);

	/// <summary>
	/// Gets all spreadsheets on file
	/// </summary>
	/// <returns>A list of names of files which can be opened</returns>
	list<string> GetSavedSpreadsheetNames();
};

#endif