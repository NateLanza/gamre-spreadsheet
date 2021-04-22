#pragma once
#include <vector>
#include <list>
#include <windows.data.json.h>
#include "Cell.h"

using namespace ABI::Windows::Data::Json;

#ifndef SpreadsheetState_H
#define SpreadsheetState_H

//list<Spreadsheet> spreadsheets;	// s. state contains list of spreadsheets that already exist that the client can open (or client can create new ss)
class SpreadsheetState
{
private:
	list<Cell> currentSpreadsheet;
public:
	void SetCurrentSpreadsheet(list<Cell> spreadsheet);
};

//
//// 'OnNetworkAction' functions (names can be changed) 
//static void NewClientConnected();  // client successfully connects
//static void ParseClientRequest();  // server checks for existing ss, sends current state of ss and client ID
//static void HandleClientRequest(); // continuously receive client requests
//
//// possible helper functions
//bool IsValid(Cell cellChange); // check to see if the change made by client is valid
//string MessageBuilder(bool connected);

#endif