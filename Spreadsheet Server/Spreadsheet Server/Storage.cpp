#include "Storage.h"
#include <iostream>
#include <fstream>
#include <stdexcept>
StoredSpreadsheet Open(string filename)
{
	try
	{

	}
	catch(exception e)
	{
		StoredSpreadsheet ss();
		return ss();
	}
}

void Save(string spreadsheetName, StoredSpreadsheet ss)
{
	//save cells

	for (Cell cell : ss.cells)
	{

	}
}

ostream& operator<<(ostream& out, const StoredSpreadsheet ss)
{

	return out;
}

istream& operator>>(istream& in, StoredSpreadsheet ss)
{
	// insert cells from existing spreadsheet into list of cells of current
	for (int i = 0; i < ss.cells.size(); i++)
	{
		in >> 
	}
	return in;
}
