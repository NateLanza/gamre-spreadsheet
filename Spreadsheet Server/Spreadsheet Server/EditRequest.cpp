#include "EditRequest.h"
#include <string>

EditRequest::EditRequest(string type, string cellName, string content, shared_ptr<Client> client) :
	type(type), cellName(cellName), content(content), client(client)
{
}

string EditRequest::GetType()
{
	return type;
}

string EditRequest::GetName()
{
	return cellName;
}

string EditRequest::GetContent()
{
	return content;
}

shared_ptr<Client> EditRequest::GetClient() {
	return client;
}