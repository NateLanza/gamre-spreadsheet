#pragma once
#include <string>
#include <vector>
#include <map>
#include <unordered_set>

#ifndef DependencyGraph_H
#define DependencyGraph_H

using namespace std;

class DependencyGraph
{
private:
	map<string, unordered_set<string>> dependees;
	map<string, unordered_set<string>> dependents;
	int size;

public:
	DependencyGraph();

	// indexer not included
	const int Size() const;

	const bool HasDependents(const string& s);
	const bool HasDependees(const string& s);

	const vector<string> GetDependees(const string& s);
	const vector<string> GetDependents(const string& s);

	void AddDependency(const string& s, const string& t);
	void RemoveDependency(const string& s, const string& t);
	void ReplaceDependents(const string& s, const vector<string>& newDependents);
	void ReplaceDependees(const string& s, const vector<string>& newDependees);
};

#endif