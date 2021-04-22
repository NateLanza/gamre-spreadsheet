#pragma once
#include <string>
#include <list>
#include <map>
#include "Cell.h"
#include "SpreadsheetState.h"

using namespace std;

#ifndef Storage_H
#define Storage_H

//list//<string> spreadsheets; // possibly contain a list of spreadsheets
map <string, list<Cell>> spreadsheets;

SpreadsheetState state;

void Open(string spreadsheetName);
void Save(string spreadsheetName, list<Cell> cellsInSpreadsheet);

map<string, list<Cell>> GetSpreadsheets();
list<Cell> GetSpreadsheet(string key);

#endif