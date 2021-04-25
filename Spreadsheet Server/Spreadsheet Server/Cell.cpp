#include "Cell.h"

// See Cell.h for function documentation

Cell::Cell(const string name, const Formula contents) : name(name), contents(contents.ToString()) {
}
