#pragma once
#include <string>
#include <windows.data.json.h>

#ifndef EditRequest_H
#define EditRequest_H

using namespace std;

class EditRequest
{
private:
	string request;

	string requestType;
	string cellName;
	string contents;
public:
	EditRequest(string request);

	string GetRequest();
	string GetType();
	string GetCellName();
	string GetContents();
};

#endif 
