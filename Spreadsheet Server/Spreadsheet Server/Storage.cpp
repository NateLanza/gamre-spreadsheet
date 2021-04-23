#include "Storage.h"
#include <stdexcept>

set<Cell> Open(string spreadsheetName)
{
    if (spreadsheets.find(spreadsheetName) != spreadsheets.end())
    {
        return spreadsheets.at(spreadsheetName);
    }
    else
    {
        throw invalid_argument("Spreadsheet does not exist.");     // display some error
    }
}

void Save(string spreadsheetName, set<Cell> cellsInSpreadsheet)
{
    if (spreadsheets.find(spreadsheetName) != spreadsheets.end()) // element is in spreadsheet
    {
        spreadsheets.at(spreadsheetName) = cellsInSpreadsheet; //update spreadsheet to contains most recent changes
    }
    else // we need to save a new spreadsheet
    {
        spreadsheets.insert({ spreadsheetName, cellsInSpreadsheet });
    }
}

map<string, set<Cell>> GetSpreadsheets()
{
    if (spreadsheets.size() > 0)
        return spreadsheets;
    throw invalid_argument("There are no spreadsheets available.");
}

/*
 * Gets a specific spreadsheet from the possible spreadsheets if the 
 * spreadsheet exists.
 * 
 * Param:   key, the name of the spreadsheet trying to be returned
 * Returns: the spreadsheet pertaining to the name given, empty set otherwise.
 */
set<Cell> GetSpreadsheet(string name)
{
    if (spreadsheets.find(name) != spreadsheets.end())
        return spreadsheets.at(name);
    throw invalid_argument("Spreadsheet not found.");
}
