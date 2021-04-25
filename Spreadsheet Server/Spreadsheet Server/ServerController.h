#include <list>
#include "SpreadsheetState.h"
#include <unordered_map>
#include "Client.h"

#ifndef SERVERCONTROLLER_H
#define SERVERCONTROLLER_H

/// <summary>
/// Coordinates between networking/clients, spreadsheet models (SpreadsheetState), and storage
/// </summary>
class ServerController {
public:
	/// <summary>
	/// Creates a new ServerController
	/// </summary>
	ServerController();

	/// <summary>
	/// Starts the server and starts listening to clients
	/// </summary>
	void StartServer();

	/// <summary>
	/// Stops the server, disconnects all connected clients, and saves any open spreadsheets
	/// </summary>
	void StopServer();

private:

	/// <summary>
	/// All spreadsheets which are currently open & being edited by users
	/// Key is the name of the spreadsheet, value is the state of the spreadsheet
	/// </summary>
	unordered_map<string, SpreadsheetState> openSpreadsheets;

	/// <summary>
	/// Maps each spreadsheet name to the list of clients connected to it
	/// </summary>
	unordered_map <string, list<Client>> clientConnections;

};

#endif // SERVERCONTROLLER_H