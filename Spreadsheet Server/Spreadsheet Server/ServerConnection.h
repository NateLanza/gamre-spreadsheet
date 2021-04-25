
#ifndef SERVER_CONNECTION_H
#define SERVER_CONNECTION_H



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
};

class ServerConnection 
{
	boost::asio::io_service s_ioservice;					// Boost class that supports asynchronous functions
	boost::asio::ip::tcp::acceptor s_acceptor;				// Boost class that accepts clients
	std::list<Connection> connections;						// List of Connected clients 
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

	void broadcast(std::list<Connection> clients, std::string message);
};


#endif
