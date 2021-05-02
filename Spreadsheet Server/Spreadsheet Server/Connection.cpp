#include "Connection.h"

Connection::Connection(boost::asio::io_service& io_service)		// Creates a Connection with io_service, which facilitates ansynchrony. 
	: socket(io_service), read_buffer(), stored_service(io_service), ID(0) {
}

Connection::Connection(boost::asio::io_service& io_service, size_t max_buffer_size) // Creates the connection with an additional buffer_size, if specified.
	: socket(io_service), read_buffer(max_buffer_size), stored_service(io_service), ID(0) {
}

//Connection::Connection(const Connection& copy) 
//	: socket(copy.socket), read_buffer(copy.read_buffer.size()), stored_service(copy.stored_service), ID(0) {
//}

void Connection::setID(int x) {
	ID = x;
}