#pragma once
#include <string>
#include <boost/asio.hpp> 
#include "Connection.h"
#include <list>

using namespace std;
using it_connection = std::list<Connection>::iterator;

#ifndef CLIENT_H
#define CLIENT_H

/// <summary>
/// Represents a client currently connected to the server
/// </summary>
class Client {

	friend class ServerConnection;		// Allows ServerConnection to access Client networking data

public:
	/// <summary>
	/// Creates a new client
	/// </summary>
	/// <param name="ID">Unique ID of the client</param>
	/// <param name="username">Client username</param>
	Client(const int ID, const string username, it_connection state);

	/// <summary>
	/// Gets ID
	/// </summary>
	/// <returns>ID of this client</returns>
	const int GetID() const;

	/// <summary>
	/// Gets username
	/// </summary>
	/// <returns>Username of this client</returns>
	const string GetUsername() const;

	/// <summary>
	/// Spreadsheet that this client is connected to
	/// </summary>
	string spreadsheet;

private:
	/// <summary>
	/// Client ID
	/// </summary>
	int ID;

	/// <summary>
	/// Client username
	/// </summary>
	string username;

	/// <summary>
	/// Client networking information
	/// </summary>
	it_connection state;

};

#endif // !CLIENT_H