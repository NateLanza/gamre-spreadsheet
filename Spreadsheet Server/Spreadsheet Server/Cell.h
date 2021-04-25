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

	/// <summary>
	/// Previous contents of this cell
	/// </summary>
	Formula previousContents;

public:

	/// <summary>
	/// Default constructor- throws error
	/// </summary>
	Cell();

	/// <summary>
	/// Create a new cell with no prior state
	/// </summary>
	/// <param name="name">Name of cell, should be [A-Z][0-9]</param>
	/// <param name="contents">Contents of cell. Should be a valid formula, string, or double</param>
	Cell(const string name, const Formula contents); // contents of cell get set through method

	/// <summary>
	/// Creates a new cell with a prior state
	/// </summary>
	/// <param name="name">Cell name, should be [A-Z][0-9]</param>
	/// <param name="contents">Contents of cell. Should be a valid formula, string, or double</param>
	/// <param name="priorContents">Prior contents of cell before last edit. Should be a valid formula, string, or double</param>
	Cell(const string name, const Formula contents, const Formula priorContents);

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
	const vector<string> GetVariables() const;

	/// <summary>
	/// Sets the contents of this cell.
	/// Throws an exception with an appropriate message if there is an issue with the new contents
	/// </summary>
	/// <param name="newContents">New contents of cell</param>
	void SetContents(const Formula newContents);

	/// <summary>
	/// Gets the state of this cell prior to the most recent edit
	/// </summary>
	/// <returns></returns>
	const Formula GetPreviousState() const;

	/// <summary>
	/// Reverts this cell to its previous content.
	/// Cells only store one previous state. On revert, the current state
	/// and previous state of this cell are swapped. Two reverts in a row 
	/// does not change the state of the cell
	/// Caller should check that this does not create a circular dependency
	/// before reverting
	/// </summary>
	void Revert();
};

#endif
