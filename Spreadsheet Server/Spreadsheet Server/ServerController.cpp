#include "ServerController.h"
#include "Storage.h"

using namespace std;
// See ServerController.h for method documentation

ServerController::ServerController() : openSpreadsheets(), clientConnections(), network(), storage() {
}

void ServerController::StartServer() {
	network.listen(1100);
	network.run();
}

void ServerController::ConnectClientToSpreadsheet(Client* client, string spreadsheet) {
	Lock();
	// See if any clients have this spreadsheet open, connect client if so
	if (clientConnections.count(spreadsheet) == 1) {
		clientConnections[spreadsheet].push_back(client);
		return;
	}

	bool foundFile = false;
	// No clients have this spreadsheet open, so open it
	// See if we have it stored on file first
	for (string ssName : storage.GetSavedSpreadsheetNames()) {
		// Got it, open the spreadsheet
		if (ssName == spreadsheet) {
			StoredSpreadsheet newSS = storage.Open(ssName);
			SpreadsheetState* toAdd = new SpreadsheetState(newSS.cells, newSS.edits);
			openSpreadsheets[ssName] = toAdd;
			foundFile = true;
			break;
		}
	}

	// Not stored on file, create a blank one
	if (!foundFile) {
		openSpreadsheets[spreadsheet] = new SpreadsheetState();
	}

	// Send spreadsheet cells to client
	list<Client*> sendTo;
	sendTo.push_back(client);
	for (Cell cell : openSpreadsheets[spreadsheet]->GetPopulatedCells()) {
		network.broadcast(sendTo, SerializeMessage(
			"cellUpdated",
			cell.GetName(),
			cell.GetContents(),
			0,
			NULL,
			NULL
		));
	}

	// Connect the client
	list<Client*> clientList;
	clientConnections[spreadsheet] = clientList;
	clientConnections[spreadsheet].push_back(client);

	// Send ID to client
	network.broadcast(sendTo, client->GetID() + "\n");

	Unlock();
}


void ServerController::ProcessClientRequest(EditRequest request) {

}