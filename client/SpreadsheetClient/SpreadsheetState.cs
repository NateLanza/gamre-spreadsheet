using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SS {

    /// <summary>
    /// An SpreadsheetState object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class SpreadsheetState {
        /// <summary>
        /// Maps cell names to their contents
        /// </summary>
        private Dictionary<String, Object> Cells;

        /// <summary>
        /// Maps cell names to their values
        /// </summary>
        private Dictionary<String, Object> Values;

        /// <summary>
        /// Tracks all cell dependencies
        /// </summary>
        private DependencyGraph Dependencies;

        /// <summary>
        /// Creates a new, empty spreadsheet
        /// </summary>
        public SpreadsheetState() {
            Cells = new Dictionary<string, object>();
            Dependencies = new DependencyGraph();
            Values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Sets a cell's content. Content string will be parsed into a double, formula if it
        /// starts with =, or string if the other two fail. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <exception cref="ArgumentNullException">If content or name is null</exception>
        /// <exception cref="ArgumentException">If name is not a cell in this spreadsheet</exception>
        /// <exception cref="FormulaFormatException">If the formula is formatted incorrectly</exception>
        public IList<string> SetContentsOfCell(string name, string content) {
            // Check for invalid values
            ValidateParameters(name, content);

            double value;
            IList<string> result;
            // Try formula
            if (content.Length > 0 && content[0] == '=') {
                result = SetCellContents(name, new Formula(content.Substring(1)));
                // Try double
            } else if (Double.TryParse(content, out value)) {
                result = SetCellContents(name, value);
                // Try string
            } else {
                result = SetCellContents(name, content);
            }

            // If an error was not thrown above, the cell is assigned and needs evaluation
            EvaluateCell(name);

            return result;
        }

        /// <summary>
        /// Returns the value of the named cell
        /// </summary>
        /// <param name="name">The named cell</param>
        /// <returns>String, double, or SpreadsheetUtilities.FormulaError</returns>
        public object GetCellValue(string name) {
            if (Values.ContainsKey(name)) {
                return Values[name];
            } else {
                return "";
            }

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet
        /// </summary>
        /// <returns>Enumerable HashSet</returns>
        public IEnumerable<String> GetNamesOfAllNonemptyCells() {
            // Get all cell names
            List<String> keys = Cells.Keys.ToList();

            // Prune empty cells
            foreach (String key in Cells.Keys) {
                if (typeof(String).IsInstanceOfType(Cells[key]) && (String)Cells[key] == "") {
                    keys.Remove(key);
                }
            }

            return keys;
        }

        /// <summary>
        /// Validates a cell name and an argument, for use in other methods. The argument must
        /// not be null, otherwise an ArgumentNullException is thrown. The cell name must be valid
        /// as defined in the class comment. If both parameters are valid, no action is taken.
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="arg">Object to check for null</param>
        /// <exception cref="ArgumentException">If name is invalid</exception>
        /// <exception cref="ArgumentNullException">If arg is null</exception>
        private void ValidateParameters(String name, Object arg) {
            // Validate arg
            if (arg == null) {
                throw new ArgumentNullException();
            }

            // Validate cell name
            ValidateName(name);
        }

        /// <summary>
        /// Validates a cell name, for use in other methods. The cell name must be valid
        /// as defined in the class comment. If cell name is valid, no action is taken.
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <exception cref="ArgumentException">If name is invalid</exception>
        private void ValidateName(String name) {
            // Validate cell name
            if (name != null && Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$")) {
                return;
            } else {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the contents (not value) of the named cell.
        /// </summary>
        /// <param name="name">Named cell</param>
        /// <exception cref="ArgumentException">If name is null or invalid</exception>
        /// <returns>string, double, or Formula</returns>
        public object GetCellContents(String name) {
            // Check name
            ValidateName(name);

            // Get value
            Object result;
            Cells.TryGetValue(name, out result);

            // Return "" if result was null, else result
            return result == null ? "" : result;
        }

        /// <summary>
        /// Sets the contents of a cell to a number, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="number">Number to set cell to</param>
        /// <exception cref="ArgumentException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        private IList<String> SetCellContents(String name, double number) {
            // Check name
            ValidateParameters(name, number);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            Cells[name] = number;

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Sets the contents of a cell to text, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="text">Text to set cell to</param>
        /// <exception cref="ArgumentNullException">If text is null</exception>
        /// <exception cref="ArgumentException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        private IList<String> SetCellContents(String name, String text) {
            // Check name
            ValidateParameters(name, text);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            Cells[name] = text;

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Sets the contents of a cell to text, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="formula">Text to set cell to</param>
        /// <exception cref="ArgumentNullException">If formula is null</exception>
        /// <exception cref="ArgumentException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        private IList<String> SetCellContents(String name, Formula formula) {
            // Check name
            ValidateParameters(name, formula);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            Cells[name] = formula;

            // Add dependencies
            foreach (String var in formula.GetVariables()) {
                Dependencies.AddDependency(var, name);
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Removes all dependencies associated with a cell when it is reassigned
        /// </summary>
        /// <param name="name"></param>
        private void RemoveDependencies(String name) {
            if (Cells.ContainsKey(name) && typeof(Formula).IsInstanceOfType(Cells[name])) {
                foreach (String var in ((Formula)Cells[name]).GetVariables()) {
                    Dependencies.RemoveDependency(var, name);
                }
            }
        }

        /// <summary>
        /// Helper method which reassigns the value of a cell and any cells which depend on it
        /// Should be run after the content of the cell in this.cells has been reassigned
        /// </summary>
        /// <param name="name">Cell to reevaluate</param>
        /// <exception cref="CircularException">If a circular dependency is found while evaluating cells</exception>
        private void EvaluateCell(String name) {
            // Reassign if string or double
            if (typeof(string).IsInstanceOfType(Cells[name]) || typeof(double).IsInstanceOfType(Cells[name])) {
                Values[name] = Cells[name];
            } else if (typeof(Formula).IsInstanceOfType(Cells[name])) {
                Values[name] = ((Formula)Cells[name]).Evaluate(CellLookup);
            }

            // Recalculate all dependent cells
            foreach (String cell in GetCellsToRecalculate(name)) {
                if (cell == name) {
                    continue;
                }
                EvaluateCell(cell);
            }
        }

        /// <summary>
        /// Helper method for EvaluateCell which looks up the value of a cell as a double
        /// </summary>
        /// <param name="name">Name of cell</param>
        /// <returns>Value of cell as double</returns>
        /// <exception cref="InvalidOperationException">If the cell's value is a string, FormulaError, or null</exception>
        private double CellLookup(String name) {
            if (typeof(double).IsInstanceOfType(Values[name])) {
                return (double)Values[name];
            } else {
                throw new InvalidOperationException("Cell " + name + " is not a double");
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// </summary>
        /// <param name="name">The named cell</param>
        /// <returns>Enumerable list of cell names</returns>
        protected IEnumerable<String> GetDirectDependents(String name) {
            return Dependencies.GetDependents(name);
        }

        /// <summary>
        /// A convenience method for invoking the other version of GetCellsToRecalculate
        /// with a singleton set of names.  See the other version for details.
        /// </summary>
        protected IEnumerable<String> GetCellsToRecalculate(String name) {
            return GetCellsToRecalculate(new HashSet<String>() { name });
        }

        /// <summary>
        /// Requires that names be non-null.  Also requires that if names contains s,
        /// then s must be a valid non-null cell name.
        /// 
        /// If any of the named cells are involved in a circular dependency,
        /// throws a CircularException.
        /// 
        /// Otherwise, returns an enumeration of the names of all cells whose values must
        /// be recalculated, assuming that the contents of each cell named in names has changed.
        /// The names are enumerated in the order in which the calculations should be done.  
        /// 
        /// For example, suppose that 
        /// A1 contains 5
        /// B1 contains 7
        /// C1 contains the formula A1 + B1
        /// D1 contains the formula A1 * C1
        /// E1 contains 15
        /// 
        /// If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
        /// and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
        /// The method will produce one of those enumerations.
        /// 
        /// Please note that this method depends on the abstract GetDirectDependents.
        /// It won't work until GetDirectDependents is implemented correctly.
        /// </summary>
        protected IEnumerable<String> GetCellsToRecalculate(ISet<String> names) {
            LinkedList<String> changed = new LinkedList<String>();
            HashSet<String> visited = new HashSet<String>();
            foreach (String name in names) {
                if (!visited.Contains(name)) {
                    Visit(name, name, visited, changed);
                }
            }
            return changed;
        }

        /// <summary>
        /// A helper for the GetCellsToRecalculate method.
        /// </summary>
        private void Visit(String start, String name, ISet<String> visited, LinkedList<String> changed) {
            visited.Add(name);
            foreach (String n in GetDirectDependents(name)) {
                if (n.Equals(start)) {
                    throw new ArgumentException("Circular dependency detected at cell: " + n);
                } else if (!visited.Contains(n)) {
                    Visit(start, n, visited, changed);
                }
            }
            changed.AddFirst(name);
        }
    }
}
