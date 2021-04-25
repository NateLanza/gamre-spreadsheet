#include "Cell.h"
#include <list>

// See Cell.h for function documentation

Cell::Cell() : name(), contents(""), previousContents("") {
	throw new exception("Do not use Cell's default constructor");
}

Cell::Cell(const string name, const Formula contents) : name(name), contents(contents.ToString()), previousContents("") {
}

Cell::Cell(const string name, const Formula contents, const Formula priorContents) : name(name), contents(contents.ToString()), previousContents(priorContents) {
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

void Cell::Revert() {
	Formula temp = contents;
	contents = previousContents;
	previousContents = temp;
}

const Formula Cell::GetPreviousState() const {
	return previousContents;
}