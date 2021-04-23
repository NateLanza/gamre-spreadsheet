#pragma once
#include <string>
#include "Cell.h"
#include <windows.data.json.h>

#ifndef EditRequest_H
#define EditRequest_H

class EditRequest
{
private:
	
	string type;
	string cellname;
	string content;

public:
	EditRequest();

	string GetType();
	string GetName();
	string GetContent();

	void SetType(string t);
	void SetName(string n);
	void SetContent(string c);
};

#endif 