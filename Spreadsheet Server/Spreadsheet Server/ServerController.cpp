#include "ServerController.h"
#include "Storage.h"
#include <iostream>
#include <algorithm>

//#include <boost/json.hpp>


using namespace std;
// See ServerController.h for method documentation

ServerController::ServerController() : openSpreadsheets(), clientConnections(), storage(), threadkey(), network(make_shared<ServerConnection>(this)) {
}

void ServerController::StartServer() {

	network->listen(1100);
	network->run();
}

void ServerController::ConnectClientToSpreadsheet(shared_ptr<Client> client, string spreadsheet) {
	Lock();

	// See if any clients have this spreadsheet open, open if not
	if (clientConnections.count(spreadsheet) < 1) {
		StoredSpreadsheet newSS = storage.Open(spreadsheet);
		shared_ptr<SpreadsheetState> toAdd = make_shared<SpreadsheetState>(newSS.cells, newSS.edits);
		openSpreadsheets[spreadsheet] = toAdd;
		list<shared_ptr<Client>> clientList;
		clientConnections[spreadsheet] = clientList;
	}

	// Connect the client
	clientConnections[spreadsheet].push_back(client);
	client->spreadsheet = spreadsheet;

	// Send spreadsheet cells to client
	list<shared_ptr<Client>> sendTo = clientConnections[spreadsheet];
	for (Cell cell : openSpreadsheets[spreadsheet]->GetPopulatedCells()) {
		// Skip empty cells
		if (cell.GetContents() == "")
			continue;
		network->broadcast(sendTo, SerializeMessage(
			"cellUpdated",
			cell.GetName(),
			cell.GetContents(),
			NULL,
			"",
			""
		));
	}

	// Send ID to client
	network->broadcast(sendTo, to_string(client->GetID()) + "\n");

	Unlock();
}


void ServerController::ProcessClientRequest(EditRequest request) {
	// First, process select request if applicable
	if (request.GetType() == "selectCell") {
		// Select cell
		openSpreadsheets[request.GetClient()->spreadsheet]->
			SelectCell(request.GetName(), request.GetClient()->GetID());

		string message = SerializeMessage(
			"cellSelected",
			request.GetName(),
			"",
			request.GetClient()->GetID(),
			request.GetClient()->GetUsername(),
			""
		);

		// Broadcast select
		network->broadcast(clientConnections[request.GetClient()->spreadsheet], message);

		return;
	}

	if (request.GetType() == "undo") {
		tuple<bool, string> undoRequestSuccess;
		undoRequestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			UndoLastEdit();

		// If request successful, send out the new cell
		if (get<0>(undoRequestSuccess)) {
			// We save on every change, since this should be relatively fast
			shared_ptr<SpreadsheetState> ss = openSpreadsheets[request.GetClient()->spreadsheet];
			StoredSpreadsheet toStore(ss->GetPopulatedCells(), ss->GetEditHistory());
			storage.Save(request.GetClient()->spreadsheet, toStore);
			network->broadcast(clientConnections[request.GetClient()->spreadsheet],
				SerializeMessage(
					"cellUpdated",
					get<1>(undoRequestSuccess),
					openSpreadsheets[request.GetClient()->spreadsheet]->GetCell(get<1>(undoRequestSuccess)),
					0,
					"",
					""
				));
			return;
		}
		else {
			// Send error message to client for bad request
			list<shared_ptr<Client>> toSend;
			toSend.push_back(request.GetClient());
			network->broadcast(toSend, SerializeMessage(
				"requestError",
				"",
				"",
				0,
				"",
				get<1>(undoRequestSuccess)
			));
			return;
		}
	}

	if (request.GetType() == "JSONerror") {
		// Send error message to client for bad request
		list<shared_ptr<Client>> toSend;
		toSend.push_back(request.GetClient());
		network->broadcast(toSend, SerializeMessage(
			"requestError",
			request.GetName(),
			"",
			0,
			"",
			"Request rejected"
		));
		return;
	}

	bool requestSuccess = false;
	// Take action based on request type
	if (request.GetType() == "editCell") {
		//if the contents are the same, ignore this request
		try {
			if (request.GetContent() == openSpreadsheets[request.GetClient()->spreadsheet]->GetCell(request.GetName()))
				return;
		}
		catch (exception e) { 
			/*this should occur when the cell doesn't exist yet in the spreadsheet*/ 
			if (request.GetContent() == "")
				return;
		}
		requestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			EditCell(request.GetName(), request.GetContent(), request.GetClient()->GetID());
	}
	else if (request.GetType() == "revertCell") {
		requestSuccess = openSpreadsheets[request.GetClient()->spreadsheet]->
			RevertCell(request.GetName());
	}

	// If request successful, send out the new cell
	if (requestSuccess) {
		// We save on every change, since this should be relatively fast
		shared_ptr<SpreadsheetState> ss = openSpreadsheets[request.GetClient()->spreadsheet];
		StoredSpreadsheet toStore(ss->GetPopulatedCells(), ss->GetEditHistory());
		storage.Save(request.GetClient()->spreadsheet, toStore);
		network->broadcast(clientConnections[request.GetClient()->spreadsheet],
			SerializeMessage(
				"cellUpdated",
				request.GetName(),
				openSpreadsheets[request.GetClient()->spreadsheet]->GetCell(request.GetName()),
				0,
				"",
				""
			));
		return;
	}
	else {
		// Send error message to client for bad request
		list<shared_ptr<Client>> toSend;
		toSend.push_back(request.GetClient());
		network->broadcast(toSend, SerializeMessage(
			"requestError",
			request.GetName(),
			"",
			0,
			"",
			"Request rejected"
		));
		return;
	}
}

void ServerController::DisconnectClient(shared_ptr<Client> client) {
	// Gonna modify connections, so lock
	Lock();
	string ssname = client->spreadsheet;
	clientConnections[ssname].erase(find(clientConnections[ssname].begin(), clientConnections[ssname].end(), client));

	// See if that was the last client connected to the spreadsheet
	// If so, close spreadsheet and save
	if (clientConnections[ssname].size() == 0) {
		// Save
		shared_ptr<SpreadsheetState> ss = openSpreadsheets[client->spreadsheet];
		StoredSpreadsheet toStore(ss->GetPopulatedCells(), ss->GetEditHistory());
		storage.Save(ssname, toStore);
		// Delete from current state
		openSpreadsheets.erase(ssname);
		clientConnections.erase(ssname);
	}
	Unlock();

	// Broadcast disconnect to other clients
	if (clientConnections.count(ssname) > 0)
		network->broadcast(clientConnections[client->spreadsheet],
			SerializeMessage(
				"disconnected",
				"",
				"",
				client->GetID(),
				"",
				""
			));
}

string ServerController::SerializeMessage(string messageType, string cellName, string contents, int userID, string username, string message) const {
	string result = "";
	// Generate message based on type
	if (messageType == "cellUpdated") {
		result += "{\"messageType\": \"cellUpdated\", \"cellName\": \"" + cellName + "\", \"contents\": \"" + contents + "\"}";
	}
	else if (messageType == "cellSelected") {
		result += "{\"messageType\": \"cellSelected\", \"cellName\": \"" + cellName + "\", \"selector\": \"" + to_string(userID) + "\", \"selectorName\": \"" + username + "\"}";
	}
	else if (messageType == "disconnected") {
		result += "{\"messageType\": \"disconnected\", \"user\": \"" + to_string(userID) + "\"}";
	}
	else if (messageType == "requestError") {
		result += "{\"messageType\": \"requestError\", \"cellName\": \"" + cellName + "\", \"message\": \"" + message + "\"}";
	}
	else if (messageType == "serverError") {
		result += "{\"messageType\": \"serverError\", \"message\": \"" + message + "\"}";
	}

	result += "\n";

	return result;
}

list<string> ServerController::GetSpreadsheetNames() {
	list<string> names;

	// Add stored spreadsheets
	for (string s : storage.GetSavedSpreadsheetNames()) {
		if (!s.empty())
			names.push_back(s);
	}

	// Add open spreadsheets
	for (pair<string, shared_ptr<SpreadsheetState>> openSheet : openSpreadsheets) {
		// Check if names already contains the name
		if (std::find(names.begin(), names.end(), openSheet.first) == names.end() && !openSheet.first.empty())
			names.push_back(openSheet.first);
	}

	return names;
}

void ServerController::Lock() {
	threadkey.lock();
}

void ServerController::Unlock() {
	threadkey.unlock();
}

void ServerController::StopServer() {
	// Save spreadsheet
	for (pair<string, shared_ptr<SpreadsheetState>> ssPair : openSpreadsheets) {
		shared_ptr<SpreadsheetState> ss = ssPair.second;
		StoredSpreadsheet toStore(ss->GetPopulatedCells(), ss->GetEditHistory());
		storage.Save(ssPair.first, toStore);
	}

	// Inform clients of disconnect
	list<shared_ptr<Client>> toSend;
	for (pair<string, list<shared_ptr<Client>>> Clients : clientConnections) {
		// Send message
		network->broadcast(Clients.second, SerializeMessage(
			"serverError",
			"",
			"",
			0,
			"",
			"Server closing"
		));

		// Delete list
		Clients.second.clear();
		clientConnections.erase(Clients.first);
	}
}

