#include "Cell.h"
#include <list>

// See Cell.h for function documentation

Cell::Cell() : name(), contents(""), previousContents() {
}

Cell::Cell(const string name, const string contents) : name(name), contents(contents), previousContents() {
}

Cell::Cell(const string name, const string contents, const list<string> priorContents) : name(name), contents(contents), previousContents(priorContents) {
}

const string Cell::GetName() const {
	return name;
}

const string Cell::GetContents() const {
	return contents;
}

void Cell::SetContents(const string newContents) {
	previousContents.push_front(contents);
	contents = newContents;
}

const vector<string> Cell::GetVariables() const {
	if (contents.size() > 0 && contents[0] == '=')
		return Formula(contents).GetVariables();
	return vector<string>();
}

bool Cell::Revert() {
	if (!CanRevert())
		return false;
	contents = previousContents.front();
	previousContents.pop_front();
	return true;
}

const bool Cell::CanRevert() const {
	return previousContents.size() != 0;
}

const string Cell::GetPreviousState() const {
	return previousContents.front();
}

const list<string> Cell::GetPreviousStates() const {
	return previousContents;
}

bool Cell::operator< (const Cell& other) const {
	return this->name < other.name;
}