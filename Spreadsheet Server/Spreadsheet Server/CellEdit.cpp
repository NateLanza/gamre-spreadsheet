#include "CellEdit.h"

CellEdit::CellEdit(Cell state): PriorState(state)
{
}

Formula CellEdit::GetPriorContents() const {
	return PriorState.GetContents();
}

const string CellEdit::GetName() const {
	return PriorState.GetName();
}