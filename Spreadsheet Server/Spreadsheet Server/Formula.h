#pragma once
#include <string>

#ifndef Formula_H
#define Formula_H

using namespace std;

class Formula
{
private:
	bool valid;
	string formulaString;
	int value;

public:
	Formula(string formula);
};

#endif