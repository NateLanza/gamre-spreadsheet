#include "EditRequest.h"

EditRequest::EditRequest(string request)
{
	this->request = request;

	// check request to get type, name, contents and enter into variables
}

string EditRequest::GetRequest()
{
	return request;
}

string EditRequest::GetType()
{
	return requestType;
}

string EditRequest::GetCellName()
{
	return cellName;
}

string EditRequest::GetContents()
{
	return contents;
}
