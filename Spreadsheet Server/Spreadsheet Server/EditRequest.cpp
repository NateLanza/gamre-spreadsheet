#include "EditRequest.h"
#include <string>

EditRequest::EditRequest()
{
	this->type = "none";
	this->cellname = "none";
	this->content = "none";	
}

string EditRequest::GetType()
{
	return type;
}

string EditRequest::GetName()
{
	return cellname;
}

string EditRequest::GetContent()
{
	return content;
}

void EditRequest::SetType(string t)
{
	this->type = t;
}

void EditRequest::SetName(string n)
{
	this->cellname = n;
}

void EditRequest::SetContent(string c)
{
	this->content = c;
}