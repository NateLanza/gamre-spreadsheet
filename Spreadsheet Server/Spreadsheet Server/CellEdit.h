#pragma once
#include <string>
#include <windows.data.json.h>
#include "Cell.h"

#ifndef CellEdit_H
#define CellEdit_H

using namespace std;

class CellEdit
{
private:
	/// <summary>
	/// Previous state of the cell before the change
	/// </summary>
	Cell PriorState;

public:

	/// <summary>
	/// Creates a new CellEdit
	/// </summary>
	/// <param name="state">State of the cell BEFORE this edit</param>
	CellEdit(Cell state);

	/// <summary>
	/// Gets the contents of the prior state of the cell stored here
	/// </summary>
	/// <returns>Cell contents, as a formula</returns>
	Formula GetPriorContents() const;

	/// <summary>
	/// Get cell name
	/// </summary>
	/// <returns>Name of cell stored in this object</returns>
	const string GetName() const;

};
#endif 
