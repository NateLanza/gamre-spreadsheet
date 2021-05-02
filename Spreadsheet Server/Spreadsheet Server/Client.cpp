#include "Client.h"
#include "Connection.h"

// See client.h for method docs

Client::Client(const int ID, const string username, it_connection state)
	: state(state), username(username), ID(ID)
{
}

const int Client::GetID() const {
	return ID;
}

const string Client::GetUsername() const {
	return username;
}