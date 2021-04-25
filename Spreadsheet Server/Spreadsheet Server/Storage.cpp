#include "Storage.h"
#include <iostream>
#include <fstream>
#include <stdexcept>

/// <summary>
/// The constructor for a StoredSpreadsheet()
/// </summary>
/// <param name="cells">The set of cells in the stored spreadsheet</param>
/// <param name="edits">The list of edits in the stored spreadsheet</param>
StoredSpreadsheet::StoredSpreadsheet(set<Cell> cells, list<CellEdit> edits)
{
}

/// <summary>
/// This method opens a spreadsheet for a new client by opening the 
/// file pertaining to said spreadsheet. Once opened, the contents of 
/// the file will be parsed into Cells and CellEdits to then be added 
/// to a StoredSpreadsheet object.
/// </summary>
/// <param name="filename">The name of the file to be opened</param>
/// <returns>Returns the StoredSpreadsheet object containing the cells and cell edits of some spreadsheet</returns>
StoredSpreadsheet Open(string filename)
{
	try
	{
		ifstream file(filename);
		string line;

		set<Cell> ssCells;
		list<CellEdit> ssEdits;

		while (getline(file, line)) // while there are lines in the file
		{
			if (line == "CELL")
			{
				list<Formula> previousList;

				string name;
				string content;
				string prevContent;
				int loop;

				// put cell fields into variables
				file >> name;
				file >> content;
				file >> loop;         // created size variable in save method to know how long to run loop

				for (int i = 0; i < loop; i++)
				{
					file >> prevContent;
					Formula f(prevContent);
					previousList.push_back(f);
				}

				// put variables into fields of new Cell to be added to ss
				Cell cell(name, Formula(content), previousList);
				ssCells.insert(cell);
			}
			else if (line == "CELL_EDIT")
			{
				string name;
				string priorState;

				file >> name;
				file >> priorState;

				Formula f(priorState);

				// put variables into fields of new CellEdit
				CellEdit edit(name, f);
				ssEdits.push_back(edit);
			}
		}

		file.close();

		StoredSpreadsheet ss(ssCells, ssEdits);

		return ss;
	}
	catch(exception e)
	{
		StoredSpreadsheet ss();
		return ss();
	}
}


/// <summary>
/// Saves a spreadsheet by taking all of the cells and cell 
/// edits in a spreadsheet and converting them to text and 
/// saving the text to a file. The file will have the '.sprd'
/// extension.
/// </summary>
/// <param name="spreadsheetName">The name of the spreadsheet to be saved</param>
/// <param name="ss">The stored spreadsheet that contains the list of cells and edits of a certain spreadsheet</param>
void Save(const string spreadsheetName, const StoredSpreadsheet& ss)
{
	try
	{
		string filename = spreadsheetName + ".sprd";
		ofstream file(filename);

		for (Cell cell : ss.cells)
		{
			file << "CELL";
			file << cell.GetName();
			file << cell.GetContents();
			file << cell.GetPreviousStates().size();
			// parse list of previous contents into file
			for (Formula f : cell.GetPreviousStates())
			{
				file << f.ToString();
			}
		}

		for (CellEdit edit : ss.edits)
		{
			file << "CELL_EDIT";
			file << edit.GetName();
			file << edit.GetPriorContents().ToString();
		}

		file.close();
	}
	catch (exception e)
	{
		throw invalid_argument("File could not open or writing to file failed.");
	}
}
