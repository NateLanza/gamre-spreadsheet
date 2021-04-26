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
	client->spreadsheet = spreadsheet;

	// Send ID to client
	network.broadcast(sendTo, client->GetID() + "\n");

	Unlock();
}


void ServerController::ProcessClientRequest(EditRequest request) {
	// First, process select request if applicable
	if (request.GetType() == "selectCell") {
		// Select cell
		openSpreadsheets[request.GetClient()->spreadsheet]->
			SelectCell(request.GetName(), request.GetClient()->GetID());

		// Broadcast select
		network.broadcast(clientConnections[request.GetClient()->spreadsheet],
			SerializeMessage(
				"cellSelected",
				request.GetName(),
				NULL,
				request.GetClient()->GetID(),
				request.GetClient()->GetUsername(),
				NULL
			));

		return;
	}

	bool requestSuccess = false;
	// Take action based on request type
	if (request.GetType() == "editCell") {
		requestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			EditCell(request.GetName(), request.GetContent(), request.GetClient()->GetID());
	} else if (request.GetType() == "revertCell") {
		requestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			RevertCell(request.GetName());
	} else if (request.GetType() == "undo") {
		requestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			UndoLastEdit();
	}

	// If request successful, send out the new cell
	if (requestSuccess) 	{
		network.broadcast(clientConnections[request.GetClient()->spreadsheet],
			SerializeMessage(
				"cellUpdated",
				request.GetName(),
				openSpreadsheets[request.GetClient()->spreadsheet]->GetCell(request.GetName()),
				0,
				NULL,
				NULL
			));
	} else {
		// Send error message to client for bad request
		list<Client*> toSend;
		toSend.push_back(request.GetClient());
		network.broadcast(toSend, SerializeMessage(
			"requestError",
			request.GetName(),
			NULL,
			0,
			NULL,
			"Request rejected"
		));
	}
}

void ServerController::DisconnectClient(Client* client) {
	// Gonna modify connections, so lock
	Lock();
	clientConnections[client->spreadsheet].remove(client);

	// See if that was the last client connected to the spreadsheet
	// If so, close spreadsheet and save
	if (clientConnections[client->spreadsheet].size() == 0) {
		// Save
		SpreadsheetState* ss = openSpreadsheets[client->spreadsheet];
		StoredSpreadsheet toStore(ss->GetPopulatedCells(), ss->GetEditHistory());
		storage.Save(client->spreadsheet, toStore);
		// Delete from current state
		openSpreadsheets.erase(client->spreadsheet);
		clientConnections.erase(client->spreadsheet);
	}
	Unlock();

	// Broadcast disconnect to other clients
	network.broadcast(clientConnections[client->spreadsheet],
		SerializeMessage(
			"disconnected",
			NULL,
			NULL,
			client->GetID(),
			NULL,
			NULL
		));
}