#pragma once
#include <string>
#include <list>
#include <map>
#include "Cell.h"
#include "SpreadsheetState.h"

using namespace std;

#ifndef Storage_H
#define Storage_H

class Storage
{
private:
	map <string, set<Cell>> spreadsheets;
public:

	set<Cell> Open(string spreadsheetName);		// returns cells so that the spreadsheet may be 'opened' ie. the cells can be sent to the client
	void Save(string spreadsheetName, set<Cell> cellsInSpreadsheet);
	void AddSpreadsheet(string name, set<Cell> newSpreadsheet);
	void RemoveSpreadsheet(string name);

	map<string, set<Cell>> GetSpreadsheets();
	set<Cell> GetSpreadsheet(string key);
};

#endif