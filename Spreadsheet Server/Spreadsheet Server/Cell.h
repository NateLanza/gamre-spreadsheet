#pragma once
#include <string>
#include "Formula.h"

#ifndef Cell_H
#define Cell_H

using namespace std;

/// <summary>
/// Represents one cell in the spreadsheet
/// </summary>
class Cell
{
private:
	/// <summary>
	/// Name of this cell
	/// </summary>
	string name;
	/// <summary>
	/// Contents of this cell
	/// </summary>
	Formula contents;

public:
	/// <summary>
	/// Create a new cell
	/// </summary>
	/// <param name="name">Name of cell, should be [A-Z][0-9]</param>
	/// <param name="contents">Contents of cell. Should be a valid formula, string, or double</param>
	Cell(const string name, const Formula contents); // contents of cell get set through method

	/// <summary>
	/// Get cell name
	/// </summary>
	/// <returns>Name of cell</returns>
	const string GetName() const;

	/// <summary>
	/// Get cell contents
	/// </summary>
	/// <returns>Contents of cell, converted to a string</returns>
	const string GetContents() const;

	/// <summary>
	/// Gets all other cells referenced by this cell (variables)
	/// </summary>
	/// <returns>Variables (cells) in this cell's contents</returns>
	const list<string> GetVariables() const;

	/// <summary>
	/// Sets the contents of this cell.
	/// Throws an exception with an appropriate message if there is an issue with the new contents
	/// </summary>
	/// <param name="newContents">New contents of cell</param>
	void SetContents(const Formula newContents);
};

#endif
