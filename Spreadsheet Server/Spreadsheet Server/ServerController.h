#pragma once
#include <list>
#include "SpreadsheetState.h"
#include <unordered_map>
#include "Client.h"
#include "ServerConnection.h"
#include "Storage.h"
#include <mutex>

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
	/// Connects a client to a spreadsheet, 
	/// then sends all cells and selections in that spreadsheet to the client
	/// Will use a thread lock
	/// </summary>
	/// <param name="client">Client to connect</param>
	/// <param name="spreadsheet">Spreadsheet name</param>
	/// <returns>All cells in this spreadsheet, to be sent to the client</returns>
	void ConnectClientToSpreadsheet(Client* client, string spreadsheet);

private:

	/// <summary>
	/// Serializes a message into JSON for sending via the Jakkpot protocol
	/// Any fields not used by a particular message type can be set to any value
	/// </summary>
	/// <param name="messageType">Jakkpot messageType</param>
	/// <param name="cellName">Jakkpot cellName</param>
	/// <param name="contents">Jakkpot contents</param>
	/// <param name="userID">Stand-in for Jakkpot selector and Jakkpot user, since both just take the user ID</param>
	/// <param name="username">Jakkpot selectorName</param>
	/// <param name="message">Jakkpot message</param>
	/// <returns>Valid JSON string, terminated by \n character & ready to send to clients</returns>
	const string SerializeMessage(string messageType, string cellName, string contents, int userID, string username, string message) const;

	/// <summary>
	/// All spreadsheets which are currently open & being edited by users
	/// Key is the name of the spreadsheet, value is the state of the spreadsheet
	/// </summary>
	unordered_map<string, SpreadsheetState*> openSpreadsheets;

	/// <summary>
	/// Maps each spreadsheet name to the list of clients connected to it
	/// </summary>
	unordered_map<string, list<Client*>> clientConnections;

	/// <summary>
	/// Handles connections with clients
	/// </summary>
	ServerConnection *network;

	/// <summary>
	/// Handles storing files
	/// </summary>
	Storage storage;

	/// <summary>
	/// Used for locking this object during non-concurrent operations
	/// </summary>
	mutex threadkey;

	/// <summary>
	/// Acquires a lock on threadkey before a critical section
	/// </summary>
	void Lock();

	/// <summary>
	/// Releases a lock on threadkey after a critical section
	/// </summary>
	void Unlock();

};

#endif // SERVERCONTROLLER_H