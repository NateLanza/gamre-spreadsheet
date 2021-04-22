#pragma once
#include <string>
//#include "Formula.h"

#ifndef Cell_H
#define Cell_H

using namespace std;

class Cell
{
private:
	string name;
	string formula;

	// possible contents
	string stringContent;
	double doubleContent;

	void ParseContents(string content); // determine if content is string, double or formula

public:
	Cell(string name); // contents of cell get set through method

	// setters and getters
	string GetName();
};

bool is_number(const string s);
#endif
