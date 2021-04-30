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
/// <summary>
/// Networking of the server. Able to establish client / server connections via TCP Listener
/// </summary>

class ServerController;
// Creates a new server connection, initializes the members of the Connection
ServerConnection::ServerConnection(ServerController* control) : s_ioservice(), s_acceptor(s_ioservice), connections(), control(control) {
}


/// <summary>
/// Starts the server. The io_service allows for asynchrony
/// </summary>
void ServerConnection::run()
{
	s_ioservice.run();

}

/// <summary>
/// Handles sending data to a client. 
/// </summary>
/// <param name="state"></param>
/// <param name="msg_buffer"></param>
/// <param name="err"></param>
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
		std::cout << "Finished sending message\n";

		std::cout << s << std::endl;
		if (state->socket.is_open())
		{
		}

	}
}

/// <summary>
/// Handles receiving data from a client
/// </summary>
/// <param name="state"></param>
/// <param name="err"></param>
/// <param name="bytes_transfered"></param>
void ServerConnection::mng_receive(it_connection state, boost::system::error_code const& error, size_t bytes)
{
	// Only process if bytes are received
	if (bytes > 0)
	{

		std::string s((std::istreambuf_iterator<char>(&state->read_buffer)), std::istreambuf_iterator<char>());
		std::cout << "Received message: " << s;
		// Checks if this JSON and needs to be serialized
		if (s.at(0) != '{') {
			
			
			if (!state->user_chosen) {

				string userName(s);
				// Creates client if only userName is provided

				// Copies over a connection and registers it as the client's state
				Connection* c = new Connection(state->stored_service);
				Client* client = new Client(ids, userName, c);
				connected_clients.emplace(ids, client);

				// Sets the id of the client and increments the id count
				c->setID(ids);
				ids++;

				state->user_chosen = true;
				std::cout << "Sending spreadsheet names: " << s << std::endl;
				// Sends the names of available spreadsheets to the client, followed by a newline. 
				for (auto name : control->GetSpreadsheetNames()) {
					std::cout << "Spreadsheet: " << s << std::endl;
					auto buffer = std::make_shared<std::string>(name + "\n");
					auto handler = boost::bind(&ServerConnection::mng_send, this, state, buffer, boost::asio::placeholders::error);
					boost::asio::async_write(state->socket, boost::asio::buffer(*buffer), handler);
				}

				auto buffer = std::make_shared<std::string>("\n\n");
				auto handler = boost::bind(&ServerConnection::mng_send, this, state, buffer, boost::asio::placeholders::error);
				boost::asio::async_write(state->socket, boost::asio::buffer(*buffer), handler);
			}

			//Spreadsheet to be chosen, client is connected to it
			else {
				std::string ss_name(s);
				Client* c = connected_clients.at(state->ID);
				control->ConnectClientToSpreadsheet(c, ss_name);
			}
		}
		else {
			//Creates a tree and stream to read the json
			ptree pt2;
			std::stringstream jsonInput;
			jsonInput << s;

			
			try {
				cout << "Parsing json: " << s;
				read_json(jsonInput, pt2);

				// Extracts value from keys. Represents all possible client fields
				std::string cellName = pt2.get<std::string>("cellName", "");
				std::string content = pt2.get<std::string>("contents", "");
				std::string requestType = pt2.get<std::string>("requestType", "");

				// If the client is already connected, sending an edit request

				//Selector and messageType gone!
				// Create a client pointer to add to the stack of requests
				Client* c = connected_clients.at(state->ID);
				EditRequest request(requestType, cellName, content, c);
				control->ProcessClientRequest(request);
			} 	catch (const exception& e) 	{
				cout << "Bad json read: " << e.what() << endl;
			}

		}



	}
	// Reports an error, if one is present
	if (error)
	{
		std::cout << error.message() << std::endl;
		connections.erase(state);
	}

	// Starts asynchronous read again
	else
		async_receive(state);

}

/// <summary>
/// Handles accepting of clients. 
/// </summary>
/// <param name="state"></param>
/// <param name="err"></param>
void ServerConnection::mng_accept(it_connection state, boost::system::error_code const& error)
{
	// Reports an error, if present
	if (error)
	{
		std::cout << error.message() << std::endl;
		connections.erase(state);

	}
	// On receiving a connection, starts ansyncronous read process with the connected socket. 
	else
	{
		async_receive(state);
	}
	// Begin accepting more clients
	begin_accept();
}

/// <summary>
/// Starts asynchronous process of reading data
/// </summary>
/// <param name="state">The state of the connection</param>
void ServerConnection::async_receive(it_connection state)
{
	auto handler = boost::bind(&ServerConnection::mng_receive, this, state, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred);
	boost::asio::async_read_until(state->socket, state->read_buffer, "\n", handler);

}

/// <summary>
/// Begins accepting new clients
/// </summary>
void ServerConnection::begin_accept()
{
	auto state = connections.emplace(connections.begin(), s_ioservice);
	auto handler = boost::bind(&ServerConnection::mng_accept, this, state, boost::asio::placeholders::error);

	s_acceptor.async_accept(state->socket, handler);
}

/// <summary>
/// Listens for connections on the specified ports. Creates an endpoint used to open an acceptor and begin listening
/// </summary>
/// <param name="port">Specified port to listen</param>
void ServerConnection::listen(uint16_t port)
{
	auto endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port);
	s_acceptor.open(endpoint.protocol());
	s_acceptor.set_option(boost::asio::ip::tcp::acceptor::reuse_address(true));
	s_acceptor.bind(endpoint);
	s_acceptor.listen();
	begin_accept();
}


/// <summary>
/// Sends out the given message to the given list of clients
/// </summary>
/// <param name="clients"></param>
/// <param name="message"></param>
void ServerConnection::broadcast(std::list<Client*> clients, std::string message)
{

	std::list<Connection> cln_con;
	//Sends the message to each client in the list
	auto buffer = std::make_shared<std::string>(message + "\n");

	for (Client* client : clients)
	{
		if (client->state->socket.is_open())
			cln_con.push_back(*(client->state));
	}

	//it_connection its = clients2.begin();

	for (std::list<Connection>::iterator its = cln_con.begin(); its != cln_con.end(); ++its)
	{
		auto handler = boost::bind(&ServerConnection::mng_send, this, its, buffer, boost::asio::placeholders::error);
		//its->socket.async_send(boost::asio::buffer(buffer), handler);
		boost::asio::async_write((its->socket), boost::asio::buffer(*buffer), handler);
	}

	//for (Connection con : cln_con) 
	//{
	//    auto handler = boost::bind(&ServerConnection::mng_send, this, con, buffer, boost::asio::placeholders::error);
	//    //its->socket.async_send(boost::asio::buffer(buffer), handler);
	//    boost::asio::async_write((con->socket), boost::asio::buffer(*buffer), handler);
	//}

	//std::list<Connection*> cln_con;
	////Sends the message to each client in the list
	//std::list<int> l2;
	//
	//auto buffer = std::make_shared<std::string>(message + "/n");
	//for (Client* client : clients)
	//{
	//	if (client->state->socket.is_open()) {
	//		Connection* c = new Connection(client->state->stored_service);
	//		cln_con.push_back(c);
	//		auto handler = boost::bind(&ServerConnection::mng_send, this, client->state->socket, buffer, boost::asio::placeholders::error);
	//		boost::asio::async_write(client->state->socket, boost::asio::buffer(*buffer), handler);
	//		
	//	}
	//		
	//}

	//it_connection its = clients2.begin();
	
	//for (std::list<Connection>::iterator its = cln_con.begin(); its != cln_con.end(); ++its)
	//{
	//	auto handler = boost::bind(&ServerConnection::mng_send, this, its, buffer, boost::asio::placeholders::error);
	//	//its->socket.async_send(boost::asio::buffer(*buffer), handler);
	//	boost::asio::async_write((its->socket), boost::asio::buffer(*buffer), handler);
	//}



	//for (Connection con : cln_con) 
	//{
	//	auto handler = boost::bind(&ServerConnection::mng_send, this, con, buffer, boost::asio::placeholders::error);
	//	//its->socket.async_send(boost::asio::buffer(*buffer), handler);
	//	boost::asio::async_write((con->socket), boost::asio::buffer(*buffer), handler);
	//}
}

/// <summary>
/// Deletes the specified client
/// </summary>
/// <param name="terminate"></param>
void ServerConnection::delete_client(Client* terminate) {

	connected_clients.erase(terminate->GetID());
	delete terminate;
}

///// <summary>
///// Sends out the given message to the given list of clients
///// </summary>
///// <param name="clients"></param>
///// <param name="message"></param>
//void ServerConnection::broadcast(std::list<Client> clients, EditRequest request)
//{
//	//Sends the message to each client in the list
//	auto buffer = std::make_shared<std::string>(message);
//	for (std::list<Client>::iterator it = clients.begin(); it != clients.end(); ++it) {
//		if (it->state.socket.is_open())
//		{
//			auto handler = boost::bind(&ServerConnection::mng_send, this, it, buffer, boost::asio::placeholders::error);
//			boost::asio::async_write(it->state.socket, boost::asio::buffer(*buffer), handler);
//
//
//		}
//	}
//}



/// <summary>
/// Starts the server
/// 
/// TO DO ! REMOVE THIS METHOD, USE AS REFERENCE AS THIS CLASS SHOULD NOT HAVE A MAIN METHOD!
/// </summary>
/// <param name=""></param>
/// <param name=""></param>
/// <returns></returns>
//

//int main(int, char**) {
//	ServerConnection srv;
//	srv.listen(1100);
//
//	srv.run();
//	return 0;
// 
//}