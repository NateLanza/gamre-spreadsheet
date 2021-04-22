#pragma once
#include <string>
#include "Cell.h"

#ifndef CellEdit_H
#define CellEdit_H

class CellEdit
{
private:
	string message; // message to be sent to the client describing the cell edit

public:
	CellEdit();

	void SetMessage(string type, string cellname, string contents);
};

#endif 