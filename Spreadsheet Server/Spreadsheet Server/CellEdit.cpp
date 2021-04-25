#include "CellEdit.h"

CellEdit::CellEdit(string name, Formula state): PriorState(state), name(name)
{
}

Formula CellEdit::GetPriorContents() const {
	return PriorState;
}

const string CellEdit::GetName() const {
	return name;
}