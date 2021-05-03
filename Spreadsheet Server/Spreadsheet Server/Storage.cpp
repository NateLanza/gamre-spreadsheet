#include "Storage.h"
#include <iostream>
#include <fstream>
#include <stdexcept>
#include <experimental/filesystem>
#include <boost/filesystem.hpp>

namespace fs = std::experimental::filesystem;
//namespace fs = boost::filesystem;

/// <summary>
/// The constructor for a StoredSpreadsheet()
/// </summary>
/// <param name="cells">The set of cells in the stored spreadsheet</param>
/// <param name="edits">The list of edits in the stored spreadsheet</param>
StoredSpreadsheet::StoredSpreadsheet(set<Cell> cells, list<CellEdit> edits) : cells(cells), edits(edits)
{}

StoredSpreadsheet::StoredSpreadsheet() : cells(), edits() {
}

/// <summary>
/// This method opens a spreadsheet for a new client by opening the 
/// file pertaining to said spreadsheet. Once opened, the contents of 
/// the file will be parsed into Cells and CellEdits to then be added 
/// to a StoredSpreadsheet object.
/// </summary>
/// <param name="filename">The name of the file to be opened</param>
/// <returns>Returns the StoredSpreadsheet object containing the cells and cell edits of some spreadsheet</returns>
StoredSpreadsheet Storage::Open(string filename)
{
	try
	{
		ifstream file("spreadsheets/" + filename);

		string line;

		set<Cell> ssCells;
		list<CellEdit> ssEdits;	

		//if the file isn't good, just make a new spreadsheet
		if (!file.good())
			throw exception();

		while (getline(file, line)) // while there are lines in the file
		{
			if (line == "CELL")
			{
				list<string> previousList;

				string name;
				string content;
				string prevContent;
				int loop;

				// put cell fields into variables
				getline(file, line);
				name = line;
				getline(file, line);
				content = line;
				getline(file, line);
				loop = stoi(line);     // created size variable in save method to know how long to run loop

				for (int i = 0; i < loop; i++)
				{
					getline(file, line);
					prevContent = line;
					previousList.push_back(prevContent);
				}

				// put variables into fields of new Cell to be added to ss
				Cell cell(name, content, previousList);
				ssCells.insert(cell);
			}
			else if (line == "CELL_EDIT")
			{
				string name;
				string priorState;

				getline(file, line);
				name = line;
				getline(file, line);
				priorState = line;

				// put variables into fields of new CellEdit
				CellEdit edit(name, priorState);
				ssEdits.push_back(edit);
			}
		}

		file.close();

		StoredSpreadsheet ss(ssCells, ssEdits);

		return ss;
	}
	catch (exception e)
	{
		StoredSpreadsheet ss;
		return ss;
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
void Storage::Save(const string spreadsheetName, const StoredSpreadsheet& ss)
{
	try
	{
		string filename = spreadsheetName + ".sprd";
		ofstream file("spreadsheets/" + filename, ofstream::out);
		//ofstream file(filename, ofstream::out);

		for (Cell cell : ss.cells)
		{
			file << "CELL";
			file << "\n";
			file << cell.GetName();
			file << "\n";
			file << cell.GetContents();
			file << "\n";
			file << cell.GetPreviousStates().size();
			file << "\n";
			// parse list of previous contents into file
			for (string f : cell.GetPreviousStates())
			{
				file << f;
				file << " ";
			}
			file << "\n";
		}

		for (CellEdit edit : ss.edits)
		{
			file << "CELL_EDIT";
			file << "\n";
			file << edit.GetName();
			file << "\n";
			file << edit.GetPriorContents();
			file << "\n";
		}

		file.close();
	}
	catch (exception e)
	{
		throw invalid_argument("File could not open or writing to file failed.");
	}
}

/// <summary>
/// Search through filesystem and return list of all files
/// with the .sprd extension.
/// </summary>
/// <returns>List of files that contain .sprd extension</returns>
list<string> Storage::GetSavedSpreadsheetNames()
{
	list<string> files;
	//vector<string> files2;

	if (fs::exists("spreadsheets") && fs::is_directory("spreadsheets"))
	{
		for (auto const& entry : fs::recursive_directory_iterator("spreadsheets"))
		{
			if (entry.path().extension() == ".sprd")
				files.push_back(entry.path().filename().string());
		}
	}

	return files;
}
