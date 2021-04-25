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

	/// <summary>
	/// Marks a client as disconnected. 
	/// Should be called before the Client object is deleted
	/// </summary>
	/// <param name="client">Client who disconnected</param>
	void DisconnectClient(Client* client);

	/// <summary>
	/// Processes an edit request from the client
	/// </summary>
	/// <param name="request">EditRequest sent by client</param>
	void ProcessClientRequest(EditRequest request);

	/// <summary>
	/// Connects a client to a spreadsheet
	/// </summary>
	/// <param name="client">Client to connect</param>
	/// <param name="spreadsheet">Spreadsheet name</param>
	/// <returns>All cells in this spreadsheet, to be sent to the client</returns>
	const set<Cell> ConnectClientToSpreadsheet(Client* client, string spreadsheet);

private:

	/// <summary>
	/// All spreadsheets which are currently open & being edited by users
	/// Key is the name of the spreadsheet, value is the state of the spreadsheet
	/// </summary>
	unordered_map<string, SpreadsheetState> openSpreadsheets;

	/// <summary>
	/// Maps each spreadsheet name to the list of clients connected to it
	/// </summary>
	unordered_map<string, list<Client*>> clientConnections;

};

#endif // SERVERCONTROLLER_H