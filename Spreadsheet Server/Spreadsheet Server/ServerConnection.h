#pragma once

#include "Client.h"
#include "Connection.h"
#include "EditRequest.h"

#include <stack>
#include <boost/asio.hpp> 
#include <unordered_map>


#ifndef SERVER_CONNECTION_H
#define SERVER_CONNECTION_H

// Forward declare so we can use ptrs to it
class ServerController;

/// <summary>
/// Networking of the server. Able to establish client / server connections via TCP Listener
/// </summary>
class ServerConnection
{
	boost::asio::io_service s_ioservice;					// Boost class that supports asynchronous functions
	boost::asio::ip::tcp::acceptor s_acceptor;				// Boost class that accepts clients					
	shared_ptr<ServerController> control;								// The ServerController class associated with this connection
	std::list<Connection> connections;						// List used to generate iterator for a connection
		
	unordered_map<int, shared_ptr<Client>> connected_clients;			// List of Connected clients
	int ids = 0;											// Integer used to assign ID's (potential race condition)
	

	using it_connection = std::list<Connection>::iterator;	// Iterator header used to represent a connection

public:
	/// <summary>
	/// Creates a new server connection, initializes the members of the Connection
	/// </summary>
	/// <param name="control"></param>
	ServerConnection(ServerController* control);

	/// <summary>
	/// Starts the server. The io_service allows for asynchrony
	/// </summary>
	void run();

	/// <summary>
	/// Handles when an error occurs in sending data to a client.
	/// </summary>
	/// <param name="state"></param>
	/// <param name="msg_buffer"></param>
	/// <param name="err"></param>
	void mng_send(it_connection state, std::shared_ptr<std::string> msg_buffer, boost::system::error_code const& error);

	/// <summary>
	/// Handles receiving data from a client
	/// </summary>
	/// <param name="state"></param>
	/// <param name="err"></param>
	/// <param name="bytes_transfered"></param>
	void mng_receive(it_connection state, boost::system::error_code const& error, size_t bytes);

	/// <summary>
	/// Handles accepting of clients. 
	/// </summary>
	/// <param name="state"></param>
	/// <param name="err"></param>
	void mng_accept(it_connection state, boost::system::error_code const& error);

	/// <summary>
	/// Starts asynchronous process of reading data
	/// </summary>
	/// <param name="state">The state of the connection</param>
	void async_receive(it_connection state);

	/// <summary>
	/// Begins accepting new clients
	/// </summary>
	void begin_accept();

	/// <summary>
	/// Listens for connections on the specified ports. Creates an endpoint used to open an acceptor and begin listening
	/// </summary>
	/// <param name="port">Specified port to listen</param>
	void listen(uint16_t port);

	/// <summary>
	/// Sends out the given message to the given list of clients.
	/// </summary>
	/// <param name="clients"></param>
	/// <param name="message"></param>
	void broadcast(std::list<shared_ptr<Client>> &clients, std::string message);

	/// <summary>
	/// Deletes the specified client
	/// </summary>
	/// <param name="terminate"></param>
	void delete_client(int ID);
};


#endif
