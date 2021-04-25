#pragma once
#include <string>
#include <windows.data.json.h>
#include "Formula.h"

#ifndef CellEdit_H
#define CellEdit_H

using namespace std;

class CellEdit
{
private:
	/// <summary>
	/// Previous state of the cell before the change
	/// </summary>
	Formula PriorContents;

	/// <summary>
	/// Name of the cell
	/// </summary>
	string name;
public:

	/// <summary>
	/// Creates a new CellEdit
	/// </summary>
	/// <param name="cellName">Name of the cell</param>
	/// <param name="state">Contents of cell BEFORE this edit was made</param>
	CellEdit(string cellName, Formula contents);



};
#endif 
