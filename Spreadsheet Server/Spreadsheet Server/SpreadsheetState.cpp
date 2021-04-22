#include "SpreadsheetState.h"


void NewClientConnected()
{
}

void ParseClientRequest()
{
}

void HandleClientRequest()
{
}

/*
* Given the cell change, does a circular dependency occur?
*
* Returns: True if no circular dependency occurs, false otherwise.
*/
bool IsValid(Cell cellChange)
{
	return false;
}

string MessageBuilder(bool connected)
{
	return "";
}

void SpreadsheetState::SetCurrentSpreadsheet(list<Cell> spreadsheet)
{
	currentSpreadsheet = spreadsheet;
}
