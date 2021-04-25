#include "Formula.h"
#include <cstring>
#include <iostream>

using namespace std;

/// <summary>
/// Creates a Formula from a string that consists of an infix expression written as
/// described in the class comment.  If the expression is syntactically incorrect,
/// throws an exception with an explanatory Message.
/// 
/// Variables are valid if they are of the form [A-Z][1-99] (i.e., F43, I99, A1)
/// 
/// new Formula("X2+Y3") should succeed
/// new Formula("X+Y3") should throw an exception, since 'X' is not a valid variable
/// new Formula("2X+Y3") should throw an exception, since "2X+Y3" is syntactically incorrect.
/// </summary>
Formula::Formula(string formula)
{
	// Eliminate = at front
	if (formula.size() > 0 && formula[0] == '=') {
		formula = formula.substr(1);
	}
	else {
		throw new exception("no equals sign!");
	}

	// Track last variable for operator following rule and extra following rule
	bool lastWasOpeningOrOperator = false;
	bool lastWasClosingNumberOrVar = false;
	// Track parenthesis counts
	int closedParens = 0;
	int openParens = 0;
	vector<string> allTokens = GetTokens(formula);
	for (int itr = 0; itr < allTokens.size(); itr++)
	{
		Token newToken = Token(allTokens[itr]);
		// Validate based on previous token
		if (newToken.Type == "op")
		{
			if (newToken.Content.compare("(") == 0)
			{
				openParens++;
				lastWasOpeningOrOperator = true;
				if (lastWasClosingNumberOrVar)
				{
					throw exception("( cannot follow a number, variable, or closing parenthesis");
				}
			}
			else if (newToken.Content == ")")
			{
				closedParens++;
				lastWasClosingNumberOrVar = true;
				if (lastWasOpeningOrOperator)
				{
					throw new exception(") cannot follow ( or an operator");
				}
			}
			else {
				if (lastWasOpeningOrOperator)
				{
					throw new exception((newToken.Content + " cannot follow ( or an operator").c_str());
				}
				lastWasOpeningOrOperator = true;
				lastWasClosingNumberOrVar = false;
			}
		}
		else if (newToken.Type == "var")
		{
			if (lastWasClosingNumberOrVar)
			{
				throw new exception(("Error on: " + newToken.Content + " Variables cannot follow ), numbers, or other variables").c_str());
			}
			lastWasOpeningOrOperator = false;
			lastWasClosingNumberOrVar = true;
		}
		else if (newToken.Type == "val")
		{
			if (lastWasClosingNumberOrVar)
			{
				throw new exception(("Error on: " + newToken.Content + " Numbers cannot follow ), numbers, or other variables").c_str());
			}
			lastWasClosingNumberOrVar = true;
			lastWasOpeningOrOperator = false;
		}

		if (closedParens > openParens)
		{
			throw new exception("The number of ) should never exceed the number of (");
		}

		tokens.push_back(newToken);
	}

	// Make sure parethensis counts are equal
	if (openParens != closedParens)
	{
		throw new exception("Closing parenthesis and open parenthesis should be equal");
	}

	// Make sure we have at least one token
	if (tokens.size() == 0)
	{
		throw new exception("Formula must contain at least one token");
	}
	// Make sure start & end tokens are correct
	if (tokens[0].Type == "op" && tokens[0].Content != "(")
	{
		throw new exception("First token must be a number, variable, or (");
	}
	else if (tokens[tokens.size() - 1].Type == "op" && tokens[tokens.size() - 1].Content != ")")
	{
		throw new exception("Last token must be a number, variable, or )");
	}
}

/// <summary>
/// Provides a list of tokens from the provided string.
/// Automatically capitalizes all letters in variable names.
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
vector<string> Formula::GetTokens(string s) {
	//I would LOVE to use the regex that kopta provided last semeseter,
	//but i think it will be easier and more understandable to just 
	//use lots of conditionals instead
	vector<string> output;

	for (int i = 0; i < s.size(); i++)
		s[i] = toupper(s[i]);

	for (int i = 0; i < s.size(); i++) {
		//parse as an operator
		if (s[i] == '(' || s[i] == ')' || s[i] == '*' || s[i] == '/' || s[i] == '+' || s[i] == '-')
		{
			output.push_back(string(1, s[i])); continue;
		}

		//parse as a variable
		//65 is ascii for 'A', 90 is ascii for 'Z'
		if ((int)s[i] >= 65 && (int)s[i] <= 90) {
			int j = i + 1;

			while (j < s.size() && (((int)s[j] >= 65 && (int)s[j] <= 90) || ((int)s[j] >= 48 && (int)s[j] <= 57)))
				j++;

			output.push_back(s.substr(i, j - i));
			i = j - 1;
			continue;
		}

		//parse as a double
		//48 is ascii for '0', 57 is ascii for '9', 46 is ascii for '.'
		if ((int)s[i] == 46 || ((int)s[i] >= 48 && (int)s[i] <= 57))
		{

			int j = i + 1;
			while (j < s.size() && ((int)s[j] == 46 || ((int)s[j] >= 48 && (int)s[j] <= 57)))
				j++;

			output.push_back(s.substr(i, j - i));
			i = j - 1;
		}

		//if s[i] was none of the above, we just ignore it and move on
	}

	return output;
}

/// <summary>
/// Evaluates this Formula, using the lookup delegate to determine the values of
/// variables.  When a variable symbol v needs to be determined, it should be looked up
/// via lookup[v].
/// 
/// If no undefined variables or divisions by zero are encountered when evaluating 
/// this Formula, the value is returned. Otherwise, an error is THROWN - this is different
/// than original behavior, since I didn't wanna deal with a formula error class.
/// </summary>
double Formula::Evaluate(map<string, double> lookup) {
	// Init stacks
	vector<double> values;
	vector<char> ops;
	for (int itr = 0; itr < tokens.size(); ++itr) {
		Token s = tokens[itr];
		double value;
		// Evaluate if token is a number or variable
		if (s.Type != "op") {
			// Assign value if token is a variable
			if (s.Type == "var")
			{
				value = lookup[s.Content];
			}
			else
			{
				value = stod(s.Content);
			}
			// Evaluate / or *
			if (ops.size() != 0 && (ops.back() == '/' || ops.back() == '*'))
			{
				if (values.size() == 0)
				{
					throw new exception("Operators must be preceded by numbers or variables");
				}
				else
				{
					values.push_back(applyOperation(value, values.back(), ops.back()));
					values.pop_back();
					ops.pop_back();
				}
			}
			else {
				values.push_back(value);
			}
			// Evaluate if token is + or -
		}
		else if (s.Content.compare("+") == 0 || s.Content.compare("-") == 0) {
			if (values.size() > 1 && ops.size() != 0 && ((ops.back() == '+') || (ops.back() == '-'))) {
				double a = values.back(); values.pop_back();
				double b = values.back(); values.pop_back();
				char c = ops.back(); ops.pop_back();
				values.push_back(applyOperation(a, b, c));
			}
			ops.push_back(s.Content[0]);
			// Push *, /, or ( to ops stack
		}
		else if (s.Content.compare("*") == 0 || s.Content.compare("/") == 0 || s.Content.compare("(") == 0) {
			ops.push_back(s.Content[0]);
			// Evaluate closing parenthesis
		}
		else if (s.Content.compare(")") == 0) {
			// See if parenthesis is matched
			if (ops.size() != 0 && ops.back() == '(')
			{
				ops.pop_back();
			}
			// Make sure we have enough values
			if (values.size() <= 1) {
				if (ops.size() != 0 && ops.back() == '(') {
					ops.pop_back();
					continue;
				}
			}
			// Run + or -
			if (ops.size() != 0 && ops.back() == '+' || ops.back() == '-') {
				double a = values.back(); values.pop_back();
				double b = values.back(); values.pop_back();
				char c = ops.back(); ops.pop_back();
				values.push_back(applyOperation(a, b, c));
				if (ops.size() != 0 && ops.back() == '(') {
					ops.pop_back();
				}
			}
			// Run * or /
			if (ops.size() != 0 && ops.back() == '*' || ops.back() == '/') {
				double a = values.back(); values.pop_back();
				double b = values.back(); values.pop_back();
				char c = ops.back(); ops.pop_back();
				values.push_back(applyOperation(a, b, c));
			}
		}
	}

	double result = 0;

	// Final check & return
	if (ops.size() == 0) {
		if (values.size() == 1) {
			result = values.back();
			values.pop_back();
		}
	}
	else if (ops.size() == 1) {
		if (values.size() == 2) {
			double a = values.back(); values.pop_back();
			double b = values.back(); values.pop_back();
			char c = ops.back(); ops.pop_back();
			result = applyOperation(a, b, c);
		}
	}

	return result;
}

/// <summary>
/// Runs an algebraic operation on two integers
/// </summary>
/// <param name="op">Operator to use. Accepts +-*/</param>
/// <param name="a">Int before the operator</param>
/// <param name="a">Int after the operator</param>
/// <exception cref="System.ArgumentException">Thrown when op isn't one of the four
/// valid operators </exception>
double Formula::applyOperation(double a, double b, char op) {
	if (op == '+')
		return a + b;
	else if (op == '-')
		return b - a;
	else if (op == '*')
		return a * b;
	else if (op == '/')
		if (a == 0)
			throw new exception("NO dividing by zero :(");
		else
			return b / a;
	else
		throw new exception(op + " is not an operator :(((((");
}

/// <summary>
/// Returns a list of the variables that occur in this 
/// formula. Variables are tokens that are not operations or doubles.
/// </summary>
vector<string> Formula::GetVariables() {
	vector<string> result;

	// Loop through tokens, add vars to result
	for (auto itr = tokens.begin(); itr != tokens.end(); ++itr) {
		if (itr->Type == "var") {
			result.push_back(itr->Content);
		}
	}

	return result;
}

/// <summary>
/// Returns a string containing no spaces which represents the formula.
/// </summary>
string Formula::ToString() {
	string result("=");

	// Loop through tokens, add each to result
	for (auto itr = tokens.begin(); itr != tokens.end(); ++itr) {
		result += (itr->Content);
	}

	return result;
}

/// <summary>
/// Constructs a new token class from the given string.
/// This is where the validator is run, to ensure that the given
/// string is a valid token.
/// </summary>
/// <param name="token"></param>
Token::Token(string token) {
	// Check if token is value, normalize if so
	double output;
	try {
		output = stod(token);
		Content = token;
		Type = "val";
		return;
	}
	catch (exception e)
	{
	}

	if (token.compare("*") == 0 || token.compare("/") == 0 || token.compare("-") == 0 || token.compare("+") == 0 ||
		token.compare("(") == 0 || token.compare(")") == 0) {
		Content = token;
		Type = "op";
	}
	else if (IsValid(token)) {
		Type = "var";
		Content = token;
	}
	else {
		throw new exception(("Token " + token + " is not a valid double, operator, or variable").c_str());
	}
}

/// <summary>
/// Returns whether the given token is a valid variable.
/// For this spreadsheet application, this means it is some number of upper case letters
/// followed by a sequence of digits.
/// </summary>
/// <param name="token"></param>
/// <returns></returns>
bool Token::IsValid(string token) {
	if (token.size() == 0)
		return false;
	//65 is ascii for 'A', 90 is ascii for 'Z'
	if ((int)token[0] < 65 || (int)token[0] > 90)
		return false;

	int i = 1;
	while (i < token.size() && (int)token[i] >= 65 && (int)token[i] <= 90)
		i++;

	for (; i < token.size(); i++)
		//48 is ascii for '0', 57 is ascii for '9'
		if ((int)token[i] < 48 || (int)token[i] > 57)
			return false;

	return true;
}
