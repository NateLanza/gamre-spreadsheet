#include <list>
#include "SpreadsheetState.h"
#include <map>

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
	/// 
	/// </summary>
	list<SpreadsheetState> openSpreadsheets;

};