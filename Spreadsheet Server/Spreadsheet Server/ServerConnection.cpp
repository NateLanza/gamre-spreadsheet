#include <boost/asio.hpp> 
#include <boost/bind.hpp>
#include <cstdint> 
#include <iostream>
#include <list>
#include <memory>

#include "EditRequest.h"
#include "ServerConnection.h"


/// <summary>
/// Networking of the server. Able to establish client / server connections via TCP Listener
/// </summary>


	// Creates a new server connection, initializes the members of the Connection
ServerConnection::ServerConnection() : s_ioservice(), s_acceptor(s_ioservice), connections() { }

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
		std::cout << "Finished sending message\n";
		//TO:DO ! SEND DATA!!
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

		// Processes the stream sent to the server
		std::istream stream(&state->read_buffer);
		std::string line;
		std::getline(stream, line);

		//TO:DO ! PROCESS STRINGS INSTEAD OF PRINTING THEM OUT!!
		// Prints out stream
		std::cout << line << std::endl;

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

		//TODO: COMPLETE THE PROCESS AS DESCRIBED IN THE PROTOCOL, DO NOT SEND A GREETING
		std::cout << "Received Connection" << std::endl;

		auto buffer = std::make_shared<std::string>("Hello World! \n");
		auto handler = boost::bind(&ServerConnection::mng_send, this, state, buffer, boost::asio::placeholders::error);
		boost::asio::async_write(state->socket, boost::asio::buffer(*buffer), handler);
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
void ServerConnection::broadcast(std::list<Connection> clients, std::string message)
{
	//Sends the message to each client in the list
	auto buffer = std::make_shared<std::string>(message);
	for (std::list<Connection>::iterator it = clients.begin(); it != clients.end(); ++it) {
		if (it->socket.is_open())
		{
			auto handler = boost::bind(&ServerConnection::mng_send, this, it, buffer, boost::asio::placeholders::error);
			boost::asio::async_write(it->socket, boost::asio::buffer(*buffer), handler);
		}
	}
}



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