#include "Storage.h"

void Open(string spreadsheetName)
{
    if (spreadsheets.find(spreadsheetName) != spreadsheets.end())
    {
        state.SetCurrentSpreadsheet(spreadsheets.at(spreadsheetName));
    }

    // display some error
}

void Save(string spreadsheetName, list<Cell> cellsInSpreadsheet)
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

map<string, list<Cell>> GetSpreadsheets()
{
    return spreadsheets;
}

list<Cell> GetSpreadsheet(string key)
{
    return spreadsheets.at(key);
}
