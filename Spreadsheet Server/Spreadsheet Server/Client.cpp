#include "Client.h"
#include "Connection.h"

// See client.h for method docs

Client::Client(const int ID, const string username, const Connection *state) 
				: username(username), ID(ID)
{
	this->state = state;
	//state = NULL;
}

Client::Client(const int ID, const string username, const boost::asio::ip::tcp::socket* state)
	: username(username), ID(ID)
{
	this->state = NULL;
	this->state2 = state;
}

const int Client::GetID() const {
	return ID;
}

const string Client::GetUsername() const {
	return username;
}