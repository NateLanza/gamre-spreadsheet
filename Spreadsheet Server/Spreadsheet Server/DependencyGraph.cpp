#include "DependencyGraph.h"

DependencyGraph::DependencyGraph()
{
}

int DependencyGraph::Size()
{
    return 0;
}

bool DependencyGraph::HasDependents(string s)
{
    return false;
}

bool DependencyGraph::HasDependees(string s)
{
    return false;
}

vector<string> DependencyGraph::GetDependees(string s)
{
    return vector<string>();
}

vector<string> DependencyGraph::GetDependents(string s)
{
    return vector<string>();
}

void DependencyGraph::AddDependency(string s, string t)
{
}

void DependencyGraph::RemoveDependency(string s, string t)
{
}

void DependencyGraph::ReplaceDependents(string s, vector<string> newDependents)
{
}

void DependencyGraph::ReplaceDependees(string s, vector<string> newDependees)
{
}
