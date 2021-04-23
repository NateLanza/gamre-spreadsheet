#pragma once
#include <list>
#include <map>
#include <set>
#include <windows.data.json.h>
#include <windows.networking.sockets.h>

#include "Cell.h"
#include "CellEdit.h"
#include "EditRequest.h"

using namespace ABI::Windows::Data::Json;
using namespace std;

#ifndef SpreadsheetState_H
#define SpreadsheetState_H

class SpreadsheetState
{
public:
	SpreadsheetState();

	set<Cell> spreadsheetCells;         // set for cells because the cells are unique
	list<EditRequest> clientRequests;   // list so that we can pop the 'first' element as the first is the first edit that should be handled
	list<CellEdit> cellUpdates;         // list of cells that need to be updated in spreadsheetCells
	//map<int, socket> clients;         // have a map of client IDs and clients (need to know the socket setup for this)

};

#endif