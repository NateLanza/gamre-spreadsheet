#pragma once
#include <string>
#include <windows.data.json.h>
#include "SpreadsheetState.h"

#ifndef CellEdit_H
#define CellEdit_H

using namespace std;

class CellEdit
{
private:
	string type;
	string cellName;
	string clientID;   // client that made the edit
	string clientName;
public:
	CellEdit();

	void NotifyClients(); // notify all clients, except for client that made the request, of the client's actions (cell selected, cell changed)
};

#endif 
