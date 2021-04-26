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
#define _SILENCE_EXPERIMENTAL_FILESYSTEM_DEPRECATION_WARNING

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
	list<CellEdit> edits;

	/// <summary>
	/// Creates a new StoredSpreadsheet from cells & edits
	/// </summary>
	/// <param name="cells">Non-empty cells in the spreadsheet</param>
	/// <param name="edits">Edit history; most recent edit at the top</param>
	StoredSpreadsheet(set<Cell> cells, list<CellEdit> edits);
};

class Storage
{
public:
	/// <summary>
	/// This method opens a spreadsheet for a new client by opening the 
	/// file pertaining to said spreadsheet. Once opened, the contents of 
	/// the file will be parsed into Cells and CellEdits to then be added 
	/// to a StoredSpreadsheet object.
	/// </summary>
	/// <param name="filename">The name of the file to be opened</param>
	/// <returns>Returns the StoredSpreadsheet object containing the cells and cell edits of some spreadsheet</returns>

	StoredSpreadsheet Open(string spreadsheetName);

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