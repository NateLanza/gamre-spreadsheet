//Source file for dependency graph class
//Produce for CS 3505 final project. 
#include "DependencyGraph.h"

/// <summary>
/// Constructs a new, empty dependency graph
/// </summary>
DependencyGraph::DependencyGraph() : size(0), dependees(), dependents()
{
}

/// <summary>
/// 
/// </summary>
/// <returns></returns>
int DependencyGraph::Size() const
{
	return size;
}

/// <summary>
/// Returns whether the given node has dependents.
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
bool DependencyGraph::HasDependents(const string& s)
{
	return dependents[s].size() > 0;
}

/// <summary>
/// Returns whether the given node has dependees.
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
bool DependencyGraph::HasDependees(const string& s)
{
	return dependees[s].size() > 0;
}

/// <summary>
/// Returns the dependees of the given node.
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
vector<string> DependencyGraph::GetDependees(const string& s)
{
	vector<string> output = vector<string>();
	for (auto iter = dependees[s].begin(); iter != dependees[s].end(); ++iter)
	{
		output.push_back(*iter);
	}
	return output;
}

/// <summary>
/// Returns the dependents of the given node.
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
vector<string> DependencyGraph::GetDependents(const string& s)
{
	vector<string> output = vector<string>();
	for (auto iter = dependents[s].begin(); iter != dependents[s].end(); ++iter)
	{
		output.push_back(*iter);
	}
	return output;
}

/// <summary>
/// Adds the given dependency: t depends on s. 
/// </summary>
/// <param name="s"></param>
/// <param name="t"></param>
void DependencyGraph::AddDependency(const string& s, const string& t)
{
	if (dependents[s].count(t) == 0)
	{
		size++;
		dependents[s].insert(t);
	}
	else
		return;

	dependents[s].insert(t);


}

/// <summary>
/// Removes the given dependency (t depends on s), if it exists.
/// </summary>
/// <param name="s"></param>
/// <param name="t"></param>
void DependencyGraph::RemoveDependency(const string& s, const string& t)
{
	if (dependents[s].count(t) == 0)
		return;

	dependents[s].erase(t);

	dependees[t].erase(s);
}

/// <summary>
/// Replaces all of the dependents of s with a new list of dependents.
/// </summary>
/// <param name="s"></param>
/// <param name="newDependents"></param>
void DependencyGraph::ReplaceDependents(const string& s, const vector<string>& newDependents)
{
	while (HasDependents(s))
	{
		const string& s_ = *dependents[s].begin();
		RemoveDependency(s, s_);
	}

	// adds the provided new dependents of s
	for (int i = 0; i < newDependents.size(); i++) 
		AddDependency(s, newDependents[i]);
}

/// <summary>
/// Replaces all of the dependees of s with a new list of dependees.
/// </summary>
/// <param name="s"></param>
/// <param name="newDependents"></param>
void DependencyGraph::ReplaceDependees(const string& s, const vector<string>& newDependees)
{
	while (HasDependees(s))
	{
		const string& s_ = *dependents[s].begin();
		RemoveDependency(s_, s);
	}

	for (int i = 0; i < newDependees.size(); i++)
		AddDependency(newDependees[i], s);
}
