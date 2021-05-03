// Server2.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "ServerController.h"

/// <summary>
/// Main server controller object. Initialized in the global namespace
/// </summary>
ServerController srv;

/// <summary>
/// Runs when the server closes, handles exit event
/// </summary>
void HandleExit() {
	srv.StopServer();
}

int main(int, char**) {
	cout << "Server starting on port 1100" << endl;

	srv.StartServer();
	std::atexit(HandleExit);
	return 0;
}
