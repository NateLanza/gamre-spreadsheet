#pragma once
#include <string>
#include "Cell.h"
#include "Client.h"

#ifndef EditRequest_H
#define EditRequest_H

/// <summary>
/// Represents a request to edit a spreadsheet, received from a client
/// </summary>
class EditRequest
{
private:
	/// <summary>
	/// Type of this request
	/// </summary>
	string type;

	/// <summary>
	/// Name of cell to be edited
	/// </summary>
	string cellName;

	/// <summary>
	/// Requested content of cell
	/// </summary>
	string content;

	/// <summary>
	/// Client who sent this request
	/// </summary>
	Client* client;

public:
	/// <summary>
	/// Creates a new EditRequest
	/// </summary>
	/// <param name="type">Type field</param>
	/// <param name="cellName">cellName field</param>
	/// <param name="content">content field</param>
	/// <param name="client">Request sender</param>
	EditRequest(string type, string cellName, string content, Client* client);

	/// <summary>
	/// Gets request type
	/// </summary>
	/// <returns>One of the 4 request types specified in the Jakkpot protocol</returns>
	string GetType();
	/// <summary>
	/// Gets cell name
	/// </summary>
	/// <returns>Name of a cell</returns>
	string GetName();
	string GetContent();
};

#endif 