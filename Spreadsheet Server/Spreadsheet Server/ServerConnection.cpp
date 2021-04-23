#include <boost/asio.hpp> 
#include <boost/bind.hpp>
#include <cstdint> 
#include <iostream>
#include <list>
#include <memory>

struct Connection {
	boost::asio::ip::tcp::socket socket;
	boost::asio::streambuf read_buffer;
	Connection(boost::asio::io_service& io_service) : socket(io_service), read_buffer() { }
	Connection(boost::asio::io_service& io_service, size_t max_buffer_size) : socket(io_service), read_buffer(max_buffer_size) { }
};

class ServerConnection {
	boost::asio::io_service s_ioservice;
	boost::asio::ip::tcp::acceptor s_acceptor;
	std::list<Connection> s_connections;
	using con_handle_t = std::list<Connection>::iterator;

public:

	ServerConnection() : s_ioservice(), s_acceptor(s_ioservice), s_connections() { }

	void handle_read(con_handle_t con_handle, boost::system::error_code const& err, size_t bytes_transfered) {
		if (bytes_transfered > 0) {
			con_handle_t con_handle_c = con_handle;
			std::istream is(&con_handle->read_buffer);
			std::string line;
			std::getline(is, line);
			std::cout << "Message Received: " << line << std::endl;

			//This is how you send a message to every client
			/*auto buff = std::make_shared<std::string>("Message sent to all!\r\n\r\n");
			for (std::list<Connection>::iterator it = s_connections.begin(); it != s_connections.end(); ++it) {
				if (it->socket.is_open()) {
					boost::asio::write(it->socket, boost::asio::buffer(*buff));
				}
			}*/

		}

		if (!err) {
			do_async_read(con_handle);
		}
		else {
			std::cerr << "We had an error: " << err.message() << std::endl;
			s_connections.erase(con_handle);
		}
	}

	void do_async_read(con_handle_t con_handle) {
		auto handler = boost::bind(&ServerConnection::handle_read, this, con_handle, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred);
		boost::asio::async_read_until(con_handle->socket, con_handle->read_buffer, "\n", handler);
	}

	void handle_write(con_handle_t con_handle, std::shared_ptr<std::string> msg_buffer, boost::system::error_code const& err) {
		if (!err) {
			std::cout << "Finished sending message\n";

			if (con_handle->socket.is_open()) {
			}
		}
		else {
			std::cerr << "We had an error: " << err.message() << std::endl;
			s_connections.erase(con_handle);
		}
	}

	void handle_accept(con_handle_t con_handle, boost::system::error_code const& err) {
		if (!err) {
			std::cout << "Connection from: Someone \n";
			std::cout << "Sending message\n";
			auto buff = std::make_shared<std::string>("Hello World!\r\n\r\n");
			auto handler = boost::bind(&ServerConnection::handle_write, this, con_handle, buff, boost::asio::placeholders::error);
			boost::asio::async_write(con_handle->socket, boost::asio::buffer(*buff), handler);
			do_async_read(con_handle);
		}
		else {
			std::cerr << "We had an error: " << err.message() << std::endl;
			s_connections.erase(con_handle);
		}
		start_accept();
	}

	void start_accept() {
		auto con_handle = s_connections.emplace(s_connections.begin(), s_ioservice);
		auto handler = boost::bind(&ServerConnection::handle_accept, this, con_handle, boost::asio::placeholders::error);
		s_acceptor.async_accept(con_handle->socket, handler);
	}

	void listen(uint16_t port) {
		auto endpoint = boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port);
		s_acceptor.open(endpoint.protocol());
		s_acceptor.set_option(boost::asio::ip::tcp::acceptor::reuse_address(true));
		s_acceptor.bind(endpoint);
		s_acceptor.listen();
		start_accept();
	}

	void run() {
		s_ioservice.run();
	}
};

int main(int, char**) {
	ServerConnection srv;
	srv.listen(1100);

	srv.run();
	return 0;
}