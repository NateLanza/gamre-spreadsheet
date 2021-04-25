#include "Client.h"

// See client.h for method docs

Client::Client(const int ID, const string username) : username(username), ID(ID) {

}

const int Client::GetID() const {
	return ID;
}

const string Client::GetUsername() const {
	return username;
}