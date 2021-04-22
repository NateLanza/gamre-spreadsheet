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
	int Size();
	bool HasDependents(string s);
	bool HasDependees(string s);
	vector<string> GetDependees(string s);
	vector<string> GetDependents(string s);
	void AddDependency(string s, string t);
	void RemoveDependency(string s, string t);
	void ReplaceDependents(string s, vector<string> newDependents);
	void ReplaceDependees(string s, vector<string> newDependees);
};

#endif