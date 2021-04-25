#include "Cell.h"
#include <list>

// See Cell.h for function documentation

Cell::Cell(const string name, const Formula contents) : name(name), contents(contents.ToString()) {
}

const string Cell::GetName() const {
	return name;
}

const string Cell::GetContents() const {
	return contents.ToString();
}

void Cell::SetContents(const Formula newContents) {
	contents = newContents;
}

const vector<string> Cell::GetVariables() const {
	return contents.GetVariables();
}