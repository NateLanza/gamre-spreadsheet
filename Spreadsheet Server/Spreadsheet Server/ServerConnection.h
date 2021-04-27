#pragma once

#include "Client.h"
#include "Connection.h"
#include "ServerController.h"
#include <boost/asio.hpp> 

#ifndef SERVER_CONNECTION_H
#define SERVER_CONNECTION_H




class ServerConnection 
{
	boost::asio::io_service s_ioservice;					// Boost class that supports asynchronous functions
	boost::asio::ip::tcp::acceptor s_acceptor;				// Boost class that accepts clients
	std::list<Connection> connections;						// List of Connected clients
	//ServerController control;

	using it_connection = std::list<Connection>::iterator;	// Iterator for each connection

public:
							/*See ServerConnetion.cpp for method definitions and comments*/

	ServerConnection();

	void run();

	void mng_send(it_connection state, std::shared_ptr<std::string> msg_buffer, boost::system::error_code const& error);

	void mng_receive(it_connection state, boost::system::error_code const& error, size_t bytes);

	void mng_accept(it_connection state, boost::system::error_code const& error);

	void async_receive(it_connection state);

	void begin_accept();

	void listen(uint16_t port);

	void broadcast(std::list<Client*> clients, std::string message);
};


#endif
