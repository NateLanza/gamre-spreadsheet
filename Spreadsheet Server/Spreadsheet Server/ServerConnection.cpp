#include <boost/asio.hpp> 
#include <boost/bind.hpp>
#include <cstdint> 
#include <iostream>
#include <list>
#include <memory>
#include <sstream>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>

#include "EditRequest.h"
#include "ServerConnection.h"
#include "ServerController.h"

using boost::property_tree::ptree;
using boost::property_tree::read_json;
using boost::property_tree::write_json;

ServerConnection::ServerConnection(ServerController* control) : s_ioservice(), s_acceptor(s_ioservice), connections(), control(control) {
}

void ServerConnection::run()
{
	s_ioservice.run();

}

void ServerConnection::mng_send(it_connection state, std::shared_ptr<std::string> msg_buffer, boost::system::error_code const& error)
{
	// Reports an error message, if present
	if (error)
	{
		std::cout << error.message() << std::endl;
		connections.erase(state);
	}
	else {
		std::string s((std::istreambuf_iterator<char>(&state->read_buffer)), std::istreambuf_iterator<char>());
	}
}

void ServerConnection::mng_receive(it_connection state, boost::system::error_code const& error, size_t bytes)
{
	// Check for client disconnect
	if ((boost::asio::error::eof == error) || (boost::asio::error::connection_reset == error))     {
		delete_client(state->ID);
		connections.erase(state);
		return;
	}

	// Only process if bytes are received
	if (bytes > 0)
	{
		std::string s((std::istreambuf_iterator<char>(&state->read_buffer)), std::istreambuf_iterator<char>());
		std::cout << "Received message: " << s;
		// Checks if this JSON and needs to be serialized
		if (s.at(0) != '{') {


			if (!state->user_chosen) {
				
				string userName(s);
				if (s[s.size() - 1] == '\n')
					userName = s.substr(0, s.size() - 1);
				// Creates client if userName is provided

				state->setID(ids);
				shared_ptr<Client> client = make_shared<Client>(ids, userName, state);
				connected_clients.emplace(ids, client);
				ids++;
				state->user_chosen = true;

				std::cout << "Sending spreadsheet names to: " << s << std::endl;

				//if no spreadsheets are sent, we must still send two /n characters
				//this bool is here to detect that case
				bool sentSpreadsheet = false;
				// Sends the names of available spreadsheets to the client, followed by a newline. 
				for (auto name : control->GetSpreadsheetNames()) {
					sentSpreadsheet = true;
					std::cout << "Spreadsheet: " << name << std::endl;
					auto buffer = std::make_shared<std::string>(name + "\n");
					auto handler = boost::bind(&ServerConnection::mng_send, this, state, buffer, boost::asio::placeholders::error);
					boost::asio::async_write(state->socket, boost::asio::buffer(*buffer), handler);
				}

				auto buffer = std::make_shared<std::string>("\n\n");
				if (sentSpreadsheet)
					buffer = std::make_shared<std::string>("\n");

				auto handler = boost::bind(&ServerConnection::mng_send, this, state, buffer, boost::asio::placeholders::error);
				boost::asio::async_write(state->socket, boost::asio::buffer(*buffer), handler);
			}

			//Spreadsheet to be chosen, client is connected to it
			else {
				std::string ss_name(s);
				if (s[s.size() - 1] == '\n')
					ss_name = s.substr(0, s.size() - 1);
				shared_ptr<Client> c = connected_clients.at(state->ID);
				control->ConnectClientToSpreadsheet(c, ss_name);
			}
		}
		else {
			//Creates a tree and stream to read the json
			ptree pt2;
			std::stringstream jsonInput;
			jsonInput << s;


			try {
				read_json(jsonInput, pt2);

				// Extracts value from keys. Represents all possible client fields
				std::string cellName = pt2.get<std::string>("cellName", "");
				std::string content = pt2.get<std::string>("contents", "");
				std::string requestType = pt2.get<std::string>("requestType", "");

				// If the client is already connected, sending an edit request

				//Selector and messageType gone!
				// Create a client pointer to add to the stack of requests
				shared_ptr<Client> c = connected_clients.at(state->ID);
				EditRequest request(requestType, cellName, content, c);
				control->ProcessClientRequest(request);
			}
			catch (const exception& e) {
				cout << "Bad json read: " << e.what() << endl;
			}
		}
	}

	// Reports an error, if one is present
	// This also detects when sockets are disconnected!
	if (error)
	{
		delete_client(state->ID);
		std::cout << error.message() << std::endl;
	}

	// Starts asynchronous read again
	else {
		async_receive(state);
	}

}

void ServerConnection::mng_accept(it_connection state, boost::system::error_code const& error)
{
	// Reports an error, if present
	if (error)
	{
		connections.erase(state);
		std::cout << "Cannot establish connection with client: " << error.message() << std::endl;

	}
	// On receiving a connection, starts ansyncronous read process with the connected socket. 
	else
	{
		async_receive(state);
	}
	// Begin accepting more clients
	begin_accept();
}

void ServerConnection::async_receive(it_connection state)
{
	auto handler = boost::bind(&ServerConnection::mng_receive, this, state, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred);
	boost::asio::async_read_until(state->socket, state->read_buffer, "\n", handler);
}

void ServerConnection::begin_accept()
{
	auto state = connections.emplace(connections.begin(), s_ioservice);
	auto handler = boost::bind(&ServerConnection::mng_accept, this, state, boost::asio::placeholders::error);

	s_acceptor.async_accept(state->socket, handler);
}

void ServerConnection::listen(uint16_t port)
{
	auto endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port);
	s_acceptor.open(endpoint.protocol());
	s_acceptor.set_option(boost::asio::ip::tcp::acceptor::reuse_address(true));
	s_acceptor.bind(endpoint);
	s_acceptor.listen();
	begin_accept();
}

void ServerConnection::broadcast(std::list<shared_ptr<Client>>& clients, std::string message)
{

	cout << "Sending message: " << message;

	auto buffer = std::make_shared<std::string>(message);

	for (shared_ptr<Client> client : clients)
	{
		try {
			if (client->state->socket.is_open()) {
				auto handler = boost::bind(&ServerConnection::mng_send, this, client->state, buffer, boost::asio::placeholders::error);
				boost::asio::async_write(client->state->socket, boost::asio::buffer(*buffer), handler);
			}
		}
		catch (exception e) {
			cout << "Could not send message to client " << client->GetID() << endl;
			cout << "Error message: " << e.what() << endl;
		}
	}
}

void ServerConnection::delete_client(int ID) {
	//in this case, nothing to delete
	if (connected_clients.count(ID) == 0)
		return;

	shared_ptr<Client> c = connected_clients.at(ID);
	if (c->spreadsheet != "")
		control->DisconnectClient(c);

	connected_clients.erase(ID);
}

