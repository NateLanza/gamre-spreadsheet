#include "Cell.h"

Cell::Cell(string name)
{
	this->name = name;
}


string Cell::GetName()
{
	return name;
}

/*
* Parses the request to see what type of content is contained in the cell, then
* place the contents into the available private variables containted within the
* Cell class.
*/
void Cell::ParseContents(string content)
{
	if (is_number(content))
	{

	}
}

bool is_number(const string s)
{
	string::const_iterator it = s.begin();
	while (it != s.end() && isdigit(*it)) ++it; // check if all chars are digits
	return !s.empty() && it == s.end();
}
