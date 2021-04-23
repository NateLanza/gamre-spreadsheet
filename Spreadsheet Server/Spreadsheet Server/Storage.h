#pragma once
#include <string>
#include <list>
#include <map>
#include "Cell.h"
#include "SpreadsheetState.h"

using namespace std;

#ifndef Storage_H
#define Storage_H

map <string, set<Cell>> spreadsheets;

SpreadsheetState state;

set<Cell> Open(string spreadsheetName);		// returns cells so that the spreadsheet may be 'opened' ie. the cells can be sent to the client
void Save(string spreadsheetName, set<Cell> cellsInSpreadsheet);

map<string, set<Cell>> GetSpreadsheets();
set<Cell> GetSpreadsheet(string key);

#endif