#include <string>
#include <vector>
#include <map>

#ifndef Formula_H
#define Formula_H

using namespace std;

class Token
{
private:
	string Type;
	string Content;
	friend class Formula;
	static bool IsValid(string token);
public:
	Token(string token); // s must be valid, or will throw
};

class Formula
{
private:
	//the tokens of this formula
	vector<Token> tokens;
	static vector<string> GetTokens(string s);
	static double applyOperation(double a, double b, char op);

public:
	Formula(const string formula);
	double Evaluate(map<string, double> lookup);
	const vector<string> GetVariables() const;
	const string ToString() const;
};

#endif