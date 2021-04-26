#pragma once

#ifndef CONNECTION_H
#define CONNECTION_H

#include <boost/asio.hpp> 


/// <summary>
/// Represents a single network connection. This contains the user's socket and its state.
/// Similar to SocketState from CS3500
/// </summary>
struct Connection {
	boost::asio::ip::tcp::socket socket;				// The socket
	boost::asio::streambuf read_buffer;					// The data received 

	Connection(boost::asio::io_service& io_service)		// Creates a Connection with io_service, which facilitates ansynchrony. 
		: socket(io_service), read_buffer() { }

	Connection(boost::asio::io_service& io_service, size_t max_buffer_size) // Creates the connection with an additional buffer_size, if specified.
		: socket(io_service), read_buffer(max_buffer_size) { }

	Connection();

};

#endif
