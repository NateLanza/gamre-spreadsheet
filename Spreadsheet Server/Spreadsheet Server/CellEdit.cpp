#include "CellEdit.h"

CellEdit::CellEdit(string name, string state) : PriorState(state), name(name)
{
}

string CellEdit::GetPriorContents() const {
	return PriorState;
}

const string CellEdit::GetName() const {
	return name;
}