#pragma once
#include <string>
#include "Cell.h"

#ifndef CellEdit_H
#define CellEdit_H

using namespace std;

/// <summary>
/// Represents an edit to a cell
/// </summary>
class CellEdit
{
private:
	/// <summary>
	/// Previous state of the cell before the change
	/// </summary>
	string PriorState;

	/// <summary>
	/// Name of the cell
	/// </summary>
	string name;

public:

	/// <summary>
	/// Creates a new CellEdit
	/// </summary>
	/// <param name="state">State of the cell BEFORE this edit</param>
	CellEdit(const string name, const string state);

	/// <summary>
	/// Gets the contents of the prior state of the cell stored here
	/// </summary>
	/// <returns>Cell contents, as a formula</returns>
	string GetPriorContents() const;

	/// <summary>
	/// Get cell name
	/// </summary>
	/// <returns>Name of cell stored in this object</returns>
	const string GetName() const;

};
#endif 
