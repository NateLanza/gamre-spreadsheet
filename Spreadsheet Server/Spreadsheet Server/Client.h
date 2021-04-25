#pragma once
#include <string>

using namespace std;

#ifndef CLIENT_H
#define CLIENT_H

/// <summary>
/// Represents a client currently connected to the server
/// </summary>
class Client {
public:
	/// <summary>
	/// Creates a new client
	/// </summary>
	/// <param name="ID">Unique ID of the client</param>
	/// <param name="username">Client username</param>
	Client(const int ID, const string username);

	/// <summary>
	/// Gets ID
	/// </summary>
	/// <returns>ID of this client</returns>
	int GetID();

	/// <summary>
	/// Gets username
	/// </summary>
	/// <returns>Username of this client</returns>
	string GetUsername();
	
private:
	/// <summary>
	/// Client ID
	/// </summary>
	int ID;

	/// <summary>
	/// Client username
	/// </summary>
	string username;

};

#endif // !CLIENT_H