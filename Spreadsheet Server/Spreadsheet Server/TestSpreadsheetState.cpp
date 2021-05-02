//#include <iostream>
//#include "SpreadsheetState.h"
//
//using namespace std;
//
//void Assert(bool val, string message) {
//	if (!val)
//		
//  << message << endl;
//}
//
//// Lil tester function
//int main() {
//	SpreadsheetState ss;
//	// Make sure we can only edit when selected
//	Assert(!ss.EditCell("A1", "3.1", 1), "1 failed");
//	ss.SelectCell("A1", 1);
//	Assert(ss.ClientSelectedCell("A1", 1), "2 failed");
//	Assert(ss.EditCell("A1", "3.1", 1), "3 failed");
//	// Make sure edits are stored properly
//	Assert(ss.GetPopulatedCells().begin()->GetContents() == "3.1", "4 failed");
//	// Test select behavior
//	Assert(!ss.ClientSelectedCell("A1", 2), "5 failed");
//	ss.SelectCell("A1", 2);
//	Assert(ss.ClientSelectedCell("A1", 2), "6 failed");
//	// Make sure we store edit history
//	Assert(ss.GetEditHistory().front().GetPriorContents().ToString() == "", "7 failed");
//	// Make sure two clients can edit the same cell
//	Assert(ss.EditCell("A1", "=3 +1", 2), "8 failed");
//	// Make sure edit history is in order
//	Assert(ss.GetEditHistory().front().GetPriorContents().ToString() == "3.1", "9 failed");
//	// Make sure we can revert all the way back
//	Assert(ss.RevertCell("A1"), "10 failed");
//	Assert(ss.RevertCell("A1"), "11 failed");
//	// Make sure reverts are stored as edits in order
//	Assert(ss.GetEditHistory().front().GetPriorContents().ToString() == "3.1", "12 failed");
//	// Make sure undo works
//	Assert(ss.UndoLastEdit(), "13 failed");
//	Assert(ss.GetPopulatedCells().begin()->GetContents() == "3.1", "14 failed");
//	Assert(ss.UndoLastEdit(), "15 failed");
//	Assert(ss.GetEditHistory().size() == 2, "16 failed");
//	Assert(ss.GetPopulatedCells().begin()->GetContents() == "=3+1", "17 failed");
//	// Make sure we can have multiple cells
//	ss.SelectCell("A2", 1);
//	Assert(ss.EditCell("A2", "hello", 1), "18 failed");
//	Assert(ss.GetPopulatedCells().size() == 2, "19 failed");
//	Assert(ss.GetCell("A2") == "hello", "20 failed");
//	// Make sure we prevent circular dependencies
//	Assert(ss.EditCell("A2", "=A1", 1), "21 failed");
//	Assert(!ss.EditCell("A1", "=A2", 2), "22 failed");
//	Assert(ss.EditCell("A2", "=A3", 1), "23 failed");
//	Assert(!ss.EditCell("A1", "=A2", 2), "24 failed");
//	Assert(!ss.RevertCell("A2"), "25 failed");
//
//	// End of test
//	cout << "End of testing" << endl;
//}